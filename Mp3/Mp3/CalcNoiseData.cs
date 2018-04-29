//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    /// <summary>
    ///     allows re-use of previously computed noise values
    /// </summary>
    internal class CalcNoiseData
    {
        internal int global_gain;

        internal float[] noise = new float[39];

        internal float[] noise_log = new float[39];

        internal int sfb_count1;

        internal int[] step = new int[39];
    }
}