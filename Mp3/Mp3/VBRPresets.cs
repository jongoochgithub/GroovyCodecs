//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    internal class VBRPresets
    {

        internal float ath_curve;

        internal float ath_lower;

        internal float ath_sensitivity;

        internal int expY;

        internal float interch;

        internal float masking_adj;

        internal float masking_adj_short;

        internal float msfix;

        internal int quant_comp;

        internal int quant_comp_s;

        internal int safejoint;

        internal int sfb21mod;

        /// <summary>
        ///     short threshold
        /// </summary>
        internal float st_lrm;

        internal float st_s;

        internal int vbr_q;

        internal VBRPresets(
            int qual,
            int comp,
            int compS,
            int y,
            float shThreshold,
            float shThresholdS,
            float adj,
            float adjShort,
            float lower,
            float curve,
            float sens,
            float inter,
            int joint,
            int mod,
            float fix)
        {
            vbr_q = qual;
            quant_comp = comp;
            quant_comp_s = compS;
            expY = y;
            st_lrm = shThreshold;
            st_s = shThresholdS;
            masking_adj = adj;
            masking_adj_short = adjShort;
            ath_lower = lower;
            ath_curve = curve;
            ath_sensitivity = sens;
            interch = inter;
            safejoint = joint;
            sfb21mod = mod;
            msfix = fix;
        }
    }
}