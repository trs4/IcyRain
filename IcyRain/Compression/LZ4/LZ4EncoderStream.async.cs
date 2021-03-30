using System;
using IcyRain.Compression.LZ4.Internal;
using System.Threading;

#if BLOCKING
using ReadableBuffer = System.ReadOnlySpan<byte>;
using Token = IcyRain.Compression.LZ4.Internal.EmptyToken;
#else
using System.Threading.Tasks;
//using ReadableBuffer = System.ReadOnlyMemory<byte>;
//using Token = System.Threading.CancellationToken;
#endif

namespace IcyRain.Compression.LZ4
{
    internal partial class LZ4EncoderStream
    {
        private async Task WriteBlock(BlockInfo block, CancellationToken token)
        {
            if (!block.Ready) return;

            Stash.Stash4(BlockLengthCode(block));
            await Stash.Flush(token).Weave();

            await InnerWriteBlock(block.Buffer, block.Offset, block.Length, token).Weave();

            Stash.TryStash4(BlockChecksum());
            await Stash.Flush(token).Weave();
        }

#if BLOCKING || NETSTANDARD2_1

		private async Task CloseFrame(Token token)
		{
			if (_encoder == null)
				return;

			var block = FlushAndEncode();
			if (block.Ready) await WriteBlock(token, block).Weave();

			Stash.Stash4(0);
			Stash.TryStash4(ContentChecksum());
			await Stash.Flush(token).Weave();
		}

		private async Task DisposeImpl(Token token)
		{
			await CloseFrame(token).Weave();
			await InnerDispose(token, false).Weave();
		}

#endif

        private async Task WriteImpl(Memory<byte> buffer, CancellationToken token)
        {
            if (TryStashFrame())
                await Stash.Flush(token).Weave();

            var offset = 0;
            var count = buffer.Length;

            while (count > 0)
            {
                var block = TopupAndEncode(buffer.ToSpan(), ref offset, ref count);
                if (block.Ready) await WriteBlock(block, token).Weave();
            }
        }

    }
}
