using System;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IcyRain.Internal;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace IcyRain.Benchmarks;

[MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
public unsafe class StringReaderBenchmarks
{
    private const string _value = "testтест";

    private static unsafe Span<byte> WriteString(string value, out int stringSize)
    {
        fixed (char* ptrString = value)
        {
            int byteCount = Encoding.UTF8.GetByteCount(ptrString, value.Length);
            Span<byte> span = new byte[byteCount];

            fixed (byte* ptr = span)
                stringSize = Encoding.UTF8.GetBytes(ptrString, value.Length, ptr, byteCount);

            return span;
        }
    }

    private static unsafe Span<byte> WriteChars(string value, out int stringSize)
    {
        stringSize = value.Length;

        fixed (char* ptrString = value)
        {
            int byteCount = stringSize * 2;
            Span<byte> span = new byte[byteCount];

            fixed (byte* ptr = span)
            fixed (char* ptrValue = value)
                Unsafe.CopyBlock(ptr, ptrValue, (uint)byteCount);

            return span;
        }
    }

    [Benchmark]
    public unsafe string GetStringPtr()
    {
        var span = WriteString(_value, out int stringSize);

        fixed (byte* ptr = span)
            return StringEncoding.UTF8.GetString(ptr, stringSize);
    }

    [Benchmark]
    public string GetStringSpan()
    {
#if NETFRAMEWORK
        throw new NotSupportedException();
#else
        var span = WriteString(_value, out _);
        return StringEncoding.UTF8.GetString(span);
#endif
    }

    [Benchmark]
    public string GetStringBytes()
    {
        var span = WriteString(_value, out int stringSize);
        return StringEncoding.UTF8.GetString(span.ToArray(), 0, stringSize);
    }

    [Benchmark]
    public string CreateStringPtrGetChars()
    {
        var span = WriteString(_value, out int stringSize);

        fixed (byte* ptr = span)
        {
            int charCount = StringEncoding.UTF8.GetCharCount(ptr, span.Length);
            char* charsPtr = stackalloc char[charCount];
            StringEncoding.UTF8.GetChars(ptr, span.Length, charsPtr, charCount);
            return new string(charsPtr);
        }
    }

    [Benchmark]
    public string CreateStringPtr()
    {
        var span = WriteChars(_value, out int stringSize);

        fixed (byte* ptr = span)
        {
            char[] chars = new char[stringSize];

            fixed (char* charsPtr = chars)
                Unsafe.CopyBlock(charsPtr, ptr, (uint)(stringSize * 2));

            return new string(chars);
        }
    }

    [Benchmark]
    public string CreateStringPtrSpan()
    {
#if NETFRAMEWORK
        throw new NotSupportedException();
#else
        var span = WriteChars(_value, out int stringSize);

        fixed (byte* ptr = span)
        {
            Span<char> chars = stackalloc char[stringSize];

            fixed (char* charsPtr = chars)
                Unsafe.CopyBlock(charsPtr, ptr, (uint)(stringSize * 2));

            return new string(chars);
        }
#endif
    }

}
#pragma warning restore CA1707 // Identifiers should not contain underscores
