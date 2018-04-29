//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using GroovyCodecs.Mp3.Common;

namespace GroovyCodecs.Mp3.Mp3
{
    /// <summary>
    ///     Variables used for --nspsytune
    ///     @author Ken
    /// </summary>
    internal class NsPsy
    {

        /// <summary>
        ///     short block tuning
        /// </summary>
        internal float attackthre;

        internal float attackthre_s;

        internal float[][] last_en_subshort = Arrays.ReturnRectangularArray<float>(4, 9);

        internal int[] lastAttacks = new int[4];

        internal float[] longfact = new float[Encoder.SBMAX_l];

        internal float[] pefirbuf = new float[19];

        internal float[] shortfact = new float[Encoder.SBMAX_s];
    }
}