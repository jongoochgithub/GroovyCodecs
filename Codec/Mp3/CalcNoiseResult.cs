//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Codec.Mp3
{
    internal class CalcNoiseResult
    {

        internal int bits;

        /// <summary>
        ///     max quantization noise
        /// </summary>
        internal float max_noise;

        /// <summary>
        ///     number of quantization noise > masking
        /// </summary>
        internal int over_count;

        /// <summary>
        ///     sum of quantization noise > masking
        /// </summary>
        internal float over_noise;

        /// <summary>
        ///     SSD-like cost of distorted bands
        /// </summary>
        internal int over_SSD;

        /// <summary>
        ///     sum of all quantization noise
        /// </summary>
        internal float tot_noise;
    }
}