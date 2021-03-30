﻿// ReSharper disable InconsistentNaming

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace IcyRain.Compression.LZ4
{
    /// <summary>Compression level.</summary>
    internal enum LZ4Level
    {
        /// <summary>Fast compression.</summary>
        L00_FAST = 0,

        /// <summary>High compression, level 3.</summary>
        L03_HC = 3,

        /// <summary>High compression, level 4.</summary>
        L04_HC = 4,

        /// <summary>High compression, level 5.</summary>
        L05_HC = 5,

        /// <summary>High compression, level 6.</summary>
        L06_HC = 6,

        /// <summary>High compression, level 7.</summary>
        L07_HC = 7,

        /// <summary>High compression, level 8.</summary>
        L08_HC = 8,

        /// <summary>High compression, level 9.</summary>
        L09_HC = 9,

        /// <summary>Optimal compression, level 10.</summary>
        L10_OPT = 10,

        /// <summary>Optimal compression, level 11.</summary>
        L11_OPT = 11,

        /// <summary>Maximum compression, level 12.</summary>
        L12_MAX = 12,
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
