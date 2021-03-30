namespace IcyRain.Compression.LZ4
{
    /// <summary>
    /// Decoder settings.
    /// </summary>
    internal class LZ4DecoderSettings
    {
        internal static LZ4DecoderSettings Default { get; } = new LZ4DecoderSettings();

        /// <summary>Extra memory for decompression.</summary>
        public int ExtraMemory { get; set; }
    }
}
