//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Codec.Mp3
{
    internal class ABRPresets
    {

        internal float ath_curve;

        internal float ath_lower;

        internal float interch;

        internal float masking_adj;

        internal float nsbass;

        internal float nsmsfix;

        internal int quant_comp;

        internal int quant_comp_s;

        internal int safejoint;

        internal float scale;

        internal int sfscale;

        /// <summary>
        ///     short threshold
        /// </summary>
        internal float st_lrm;

        internal float st_s;

        internal ABRPresets(
            int kbps,
            int comp,
            int compS,
            int joint,
            float fix,
            float shThreshold,
            float shThresholdS,
            float bass,
            float sc,
            float mask,
            float lower,
            float curve,
            float interCh,
            int sfScale)
        {
            quant_comp = comp;
            quant_comp_s = compS;
            safejoint = joint;
            nsmsfix = fix;
            st_lrm = shThreshold;
            st_s = shThresholdS;
            nsbass = bass;
            scale = sc;
            masking_adj = mask;
            ath_lower = lower;
            ath_curve = curve;
            interch = interCh;
            sfscale = sfScale;
        }
    }
}