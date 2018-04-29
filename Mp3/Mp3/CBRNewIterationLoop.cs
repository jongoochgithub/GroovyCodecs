//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Diagnostics;

namespace GroovyCodecs.Mp3.Mp3
{
    /// <summary>
    ///     author/date??
    ///     encodes one frame of MP3 data with constant bitrate
    ///     @author Ken
    /// </summary>
    internal sealed class CBRNewIterationLoop : IIterationLoop
    {
        /// 
        private readonly Quantize quantize;

        /// <param name="quantize"> </param>
        internal CBRNewIterationLoop(Quantize quantize)
        {
            this.quantize = quantize;
        }

        public void iteration_loop(LameGlobalFlags gfp, float[][] pe, float[] ms_ener_ratio, III_psy_ratio[][] ratio)
        {

            var gfc = gfp.internal_flags;
            var l3_xmin = new float[L3Side.SFBMAX];
            var xrpow = new float[576];
            var targ_bits = new int[2];
            int mean_bits = 0, max_bits;

            var l3_side = gfc.l3_side;

            var mb = new MeanBits(mean_bits);
            quantize.rv.ResvFrameBegin(gfp, mb);
            mean_bits = mb.bits;

            /* quantize! */
            for (var gr = 0; gr < gfc.mode_gr; gr++)
            {

                /*
                 * calculate needed bits
                 */
                max_bits = quantize.qupvt.on_pe(gfp, pe, targ_bits, mean_bits, gr, gr);

                if (gfc.mode_ext == Encoder.MPG_MD_MS_LR)
                {
                    quantize.ms_convert(gfc.l3_side, gr);
                    quantize.qupvt.reduce_side(targ_bits, ms_ener_ratio[gr], mean_bits, max_bits);
                }

                for (var ch = 0; ch < gfc.channels_out; ch++)
                {
                    float adjust, masking_lower_db;
                    var cod_info = l3_side.tt[gr][ch];

                    if (cod_info.block_type != Encoder.SHORT_TYPE)
                    {
                        // NORM, START or STOP type
                        adjust = 0;
                        masking_lower_db = gfc.PSY.mask_adjust - adjust;
                    }
                    else
                    {
                        adjust = 0;
                        masking_lower_db = gfc.PSY.mask_adjust_short - adjust;
                    }

                    gfc.masking_lower = (float)Math.Pow(10.0, masking_lower_db * 0.1);

                    /*
                     * init_outer_loop sets up cod_info, scalefac and xrpow
                     */
                    quantize.init_outer_loop(gfc, cod_info);
                    if (quantize.init_xrpow(gfc, cod_info, xrpow))
                    {
                        /*
                         * xr contains energy we will have to encode calculate the
                         * masking abilities find some good quantization in
                         * outer_loop
                         */
                        quantize.qupvt.calc_xmin(gfp, ratio[gr][ch], cod_info, l3_xmin);
                        quantize.outer_loop(gfp, cod_info, l3_xmin, xrpow, ch, targ_bits[ch]);
                    }

                    quantize.iteration_finish_one(gfc, gr, ch);
                    Debug.Assert(cod_info.part2_3_length <= LameInternalFlags.MAX_BITS_PER_CHANNEL);
                    Debug.Assert(cod_info.part2_3_length <= targ_bits[ch]);
                } // for ch
            } // for gr

            quantize.rv.ResvFrameEnd(gfc, mean_bits);
        }
    }
}