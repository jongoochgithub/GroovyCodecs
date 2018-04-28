//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Codec.Mp3
{
    /// <summary>
    ///     ATH related stuff, if something new ATH related has to be added, please plug
    ///     it here into the ATH.
    /// </summary>
    internal class ATH
    {

        /// <summary>
        ///     factor for tuning the (sample power) point below which adaptive threshold
        ///     of hearing adjustment occurs
        /// </summary>
        internal float aaSensitivityP;

        /// <summary>
        ///     Lowering based on peak volume, 1 = no lowering.
        /// </summary>
        internal float adjust;

        /// <summary>
        ///     Limit for dynamic ATH adjust.
        /// </summary>
        internal float adjustLimit;

        /// <summary>
        ///     ATH for long block convolution bands.
        /// </summary>
        internal float[] cb_l = new float[Encoder.CBANDS];

        /// <summary>
        ///     ATH for short block convolution bands.
        /// </summary>
        internal float[] cb_s = new float[Encoder.CBANDS];

        /// <summary>
        ///     Determined to lower x dB each second.
        /// </summary>
        internal float decay;

        /// <summary>
        ///     Equal loudness weights (based on ATH).
        /// </summary>
        internal float[] eql_w = new float[Encoder.BLKSIZE / 2];

        /// <summary>
        ///     Lowest ATH value.
        /// </summary>
        internal float floor;

        /// <summary>
        ///     ATH for sfbs in long blocks.
        /// </summary>
        internal float[] l = new float[Encoder.SBMAX_l];

        /// <summary>
        ///     ATH for partitioned sfb12 in short blocks.
        /// </summary>
        internal float[] psfb12 = new float[Encoder.PSFB12];

        /// <summary>
        ///     ATH for partitioned sfb21 in long blocks.
        /// </summary>
        internal float[] psfb21 = new float[Encoder.PSFB21];

        /// <summary>
        ///     ATH for sfbs in short blocks.
        /// </summary>
        internal float[] s = new float[Encoder.SBMAX_s];

        /// <summary>
        ///     Method for the auto adjustment.
        /// </summary>
        internal int useAdjust;
    }
}