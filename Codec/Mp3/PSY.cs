//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Codec.Mp3
{
    /// <summary>
    ///     PSY Model related stuff
    /// </summary>
    internal class PSY
    {

        /* at transition from one scalefactor band to next */
        /// <summary>
        ///     Band weight long scalefactor bands.
        /// </summary>
        internal float[] bo_l_weight = new float[Encoder.SBMAX_l];

        /// <summary>
        ///     Band weight short scalefactor bands.
        /// </summary>
        internal float[] bo_s_weight = new float[Encoder.SBMAX_s];

        /// <summary>
        ///     The dbQ stuff.
        /// </summary>
        internal float mask_adjust;

        /// <summary>
        ///     The dbQ stuff.
        /// </summary>
        internal float mask_adjust_short;
    }
}