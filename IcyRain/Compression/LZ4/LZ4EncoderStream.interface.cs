using System;
using System.Threading;
using System.Threading.Tasks;
using IcyRain.Compression.LZ4.Internal;

namespace IcyRain.Compression.LZ4
{
    /// <summary>
    /// LZ4 compression stream. 
    /// </summary>
    internal partial class LZ4EncoderStream
    {
        /// <inheritdoc />
        public override void Flush() =>
            InnerFlush(EmptyToken.Value);

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken) =>
            InnerFlush(cancellationToken);

        /// <inheritdoc />
        public override void WriteByte(byte value) =>
            WriteImpl(EmptyToken.Value, Stash.OneByteSpan(value));

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) =>
            WriteImpl(EmptyToken.Value, buffer.AsSpan(offset, count));

        /// <inheritdoc />
        public override Task WriteAsync(
            byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            WriteImpl(buffer.AsMemory(offset, count), cancellationToken);

#if !NETFRAMEWORK

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<byte> buffer) =>
            WriteImpl(EmptyToken.Value, buffer);

        /// <inheritdoc />
        public override ValueTask WriteAsync(
            ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
            new ValueTask(WriteImpl(default, cancellationToken));

#endif

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) DisposeImpl(EmptyToken.Value);
            base.Dispose(disposing);
        }

#if NETSTANDARD2_1

		/// <inheritdoc />
		public override async ValueTask DisposeAsync()
		{
			await DisposeImpl(CancellationToken.None).Weave();
			await base.DisposeAsync().Weave();
		}

#endif

        /// <inheritdoc />
        public override bool CanRead => false;

        /// <summary>Length of the stream and number of bytes written so far.</summary>
        public override long Length => _position;

        /// <summary>Read-only position in the stream. Trying to set it will throw
        /// <see cref="InvalidOperationException"/>.</summary>
        public override long Position => _position;
    }
}
