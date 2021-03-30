namespace IcyRain.Compression.LZ4.Internal
{
    /// <summary>
    /// Completely empty class to do nothing.
    /// It is used internally instead of CancellationToken to make sure
    /// blocking operations are easily distinguishable from async ones
    /// (you cannot call blocking operation by accident as they *require* EmptyToken).
    /// </summary>
    internal readonly struct EmptyToken
    {
        public static readonly EmptyToken Value = new EmptyToken();
    }
}
