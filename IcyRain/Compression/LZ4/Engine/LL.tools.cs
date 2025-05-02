using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4.Internal;
using IcyRain.Internal;
#if DEBUG
using System;
using System.Diagnostics;
#endif

namespace IcyRain.Compression.LZ4.Engine;

internal unsafe partial class LL
{
#if DEBUG
    [Conditional("DEBUG")]
    [MethodImpl(Flags.HotPath)]
    public static void Assert(bool condition, string message = null)
    {
        if (!condition)
            throw new ArgumentException(message ?? "Assert failed");
    }
#endif

    [MethodImpl(Flags.HotPath)]
    internal static int LZ4_compressBound(int isize)
        => isize > LZ4_MAX_INPUT_SIZE ? 0 : isize + isize / 255 + 16;

    [MethodImpl(Flags.HotPath)]
    protected static uint LZ4_hash4(uint sequence, tableType_t tableType)
    {
        var hashLog = tableType == tableType_t.byU16 ? LZ4_HASHLOG + 1 : LZ4_HASHLOG;
        return unchecked((sequence * 2654435761u) >> (MINMATCH * 8 - hashLog));
    }

    [MethodImpl(Flags.HotPath)]
    protected static uint LZ4_hash5(ulong sequence, tableType_t tableType)
    {
        var hashLog = tableType == tableType_t.byU16 ? LZ4_HASHLOG + 1 : LZ4_HASHLOG;
        return unchecked((uint)(((sequence << 24) * 889523592379ul) >> (64 - hashLog)));
    }

    [MethodImpl(Flags.HotPath)]
    protected static void LZ4_clearHash(uint h, void* tableBase, tableType_t tableType)
    {
        switch (tableType)
        {
            case tableType_t.byPtr:
                ((byte**)tableBase)[h] = null;
                return;
            case tableType_t.byU32:
                ((uint*)tableBase)[h] = 0;
                return;
            case tableType_t.byU16:
                ((ushort*)tableBase)[h] = 0;
                return;
#if DEBUG
            default:
                Assert(false);
                return;
#endif
        }
    }

    [MethodImpl(Flags.HotPath)]
    protected static void LZ4_putIndexOnHash(
        uint idx, uint h, void* tableBase, tableType_t tableType)
    {
        switch (tableType)
        {
            case tableType_t.byU32:
                ((uint*)tableBase)[h] = idx;
                return;
            case tableType_t.byU16:
#if DEBUG
                Assert(idx < 65536);
#endif
                ((ushort*)tableBase)[h] = (ushort)idx;
                return;
#if DEBUG
            default:
                Assert(false);
                return;
#endif
        }
    }

    [MethodImpl(Flags.HotPath)]
    protected static void LZ4_putPositionOnHash(
        byte* p, uint h, void* tableBase, tableType_t tableType, byte* srcBase)
    {
        switch (tableType)
        {
            case tableType_t.byPtr:
                ((byte**)tableBase)[h] = p;
                return;
            case tableType_t.byU32:
                ((uint*)tableBase)[h] = (uint)(p - srcBase);
                return;
            case tableType_t.byU16:
                ((ushort*)tableBase)[h] = (ushort)(p - srcBase);
                return;
#if DEBUG
            default:
                Assert(false);
                return;
#endif
        }
    }

    [MethodImpl(Flags.HotPath)]
    protected static uint LZ4_getIndexOnHash(uint h, void* tableBase, tableType_t tableType)
    {
#if DEBUG
        Assert(LZ4_MEMORY_USAGE > 2);
#endif
        switch (tableType)
        {
            case tableType_t.byU32:
#if DEBUG
                Assert(h < (1U << (LZ4_MEMORY_USAGE - 2)));
#endif
                return ((uint*)tableBase)[h];
            case tableType_t.byU16:
#if DEBUG
                Assert(h < (1U << (LZ4_MEMORY_USAGE - 1)));
#endif
                return ((ushort*)tableBase)[h];
            default:
#if DEBUG
                Assert(false);
#endif
                return 0;
        }
    }

    [MethodImpl(Flags.HotPath)]
    protected static byte* LZ4_getPositionOnHash(uint h, void* tableBase, tableType_t tableType, byte* srcBase)
        => tableType switch
        {
            tableType_t.byPtr => ((byte**)tableBase)[h],
            tableType_t.byU32 => ((uint*)tableBase)[h] + srcBase,
            _ => ((ushort*)tableBase)[h] + srcBase,
        };

    [MethodImpl(Flags.HotPath)]
    public static int MIN(int a, int b) => a < b ? a : b;

    [MethodImpl(Flags.HotPath)]
    public static uint MIN(uint a, uint b) => a < b ? a : b;

    [MethodImpl(Flags.HotPath)]
    public static uint MAX(uint a, uint b) => a < b ? b : a;

    [MethodImpl(Flags.HotPath)]
    public static long MAX(long a, long b) => a < b ? b : a;

    [MethodImpl(Flags.HotPath)]
    public static long MIN(long a, long b) => a < b ? a : b;

    [MethodImpl(Flags.HotPath)]
    protected static uint LZ4_readVLE(
        byte** ip, byte* lencheck,
        bool loop_check, bool initial_check,
        variable_length_error* error)
    {
        uint length = 0;
        uint s;
        if (initial_check && ((*ip) >= lencheck))
        {
            *error = variable_length_error.initial_error;
            return length;
        }

        do
        {
            s = **ip;
            (*ip)++;
            length += s;
            if (loop_check && ((*ip) >= lencheck))
            {
                *error = variable_length_error.loop_error;
                return length;
            }
        }
#pragma warning disable CA1508 // Avoid dead conditional code
        while (s == 255);
#pragma warning restore CA1508 // Avoid dead conditional code

        return length;
    }

    public static LZ4_stream_t* LZ4_initStream(LZ4_stream_t* buffer)
    {
        Mem.Zero((byte*)buffer, sizeof(LZ4_stream_t));
        return buffer;
    }

    private static readonly uint[] _inc32table = [0, 1, 2, 1, 0, 4, 4, 4];
    private static readonly int[] _dec64table = [0, 0, 0, -1, -4, 1, 2, 3];

    protected static readonly uint* inc32table = Mem.CloneArray(_inc32table);
    protected static readonly int* dec64table = Mem.CloneArray(_dec64table);
}
