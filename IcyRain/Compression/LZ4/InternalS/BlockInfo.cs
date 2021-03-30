using System;
using IcyRain.Compression.LZ4.Encoders;

namespace IcyRain.Compression.LZ4.Internal
{
    internal readonly struct BlockInfo
    {
        private readonly byte[] _buffer;
        private readonly int _length;

        public byte[] Buffer => _buffer;

#pragma warning disable CA1822 // Mark members as static
        public int Offset => 0;
#pragma warning restore CA1822 // Mark members as static

        public int Length => Math.Abs(_length);
        public bool Compressed => _length > 0;
        public bool Ready => _length != 0;

        public BlockInfo(byte[] buffer, EncoderAction action, int length)
        {
            _buffer = buffer;
            _length = action switch
            {
                EncoderAction.Encoded => length,
                EncoderAction.Copied => -length,
                _ => 0,
            };
        }
    }
}
