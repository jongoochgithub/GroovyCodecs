//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Diagnostics;
using GroovyMp3.Types;

namespace GroovyMp3.Codec.Mp3
{
    /// <summary>
    ///     encode a frame with a desired average bitrate
    ///     mt 2000/05/31
    ///     @author Ken
    /// </summary>
    internal sealed class ABRIterationLoop : IIterationLoop
    {

        /// 
        private readonly Quantize quantize;

        /// <param name="quantize"> </param>
        internal ABRIterationLoop(Quantize quantize)
        {
            this.quantize = quantize;
        }

        public void iteration_loop(LameGlobalFlags gfp, float[][] pe, float[] ms_ener_ratio, III_psy_ratio[][] ratio)
        {

            var gfc = gfp.internal_flags;
            var l3_xmin = new float[L3Side.SFBMAX];
            var xrpow = new float[576];

            var targ_bits = Arrays.ReturnRectangularArray<int>(2, 2);
            var max_frame_bits = new int[1];
            var analog_silence_bits = new int[1];

            var l3_side = gfc.l3_side;

            var mean_bits = 0;

            quantize.calc_target_bits(gfp, pe, ms_ener_ratio, targ_bits, analog_silence_bits, max_frame_bits);

            /*
             * encode granules
             */
            for (var gr = 0; gr < gfc.mode_gr; gr++)
            {

                if (gfc.mode_ext == Encoder.MPG_MD_MS_LR)
                    quantize.ms_convert(gfc.l3_side, gr);

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
                     * cod_info, scalefac and xrpow get initialized in
                     * init_outer_loop
                     */
                    quantize.init_outer_loop(gfc, cod_info);
                    if (quantize.init_xrpow(gfc, cod_info, xrpow))
                    {
                        /*
                         * xr contains energy we will have to encode calculate the
                         * masking abilities find some good quantization in
                         * outer_loop
                         */
                        var ath_over = quantize.qupvt.calc_xmin(gfp, ratio[gr][ch], cod_info, l3_xmin);
                        if (0 == ath_over) // analog silence
                            targ_bits[gr][ch] = analog_silence_bits[0];

                        quantize.outer_loop(gfp, cod_info, l3_xmin, xrpow, ch, targ_bits[gr][ch]);
                    }

                    quantize.iteration_finish_one(gfc, gr, ch);
                } // ch
            } // gr

            /*
             * find a bitrate which can refill the resevoir to positive size.
             */
            for (gfc.bitrate_index = gfc.VBR_min_bitrate; gfc.bitrate_index <= gfc.VBR_max_bitrate; gfc.bitrate_index++)
            {

                var mb = new MeanBits(mean_bits);
                var rc = quantize.rv.ResvFrameBegin(gfp, mb);
                mean_bits = mb.bits;
                if (rc >= 0)
                    break;
            }

            Debug.Assert(gfc.bitrate_index <= gfc.VBR_max_bitrate);

            quantize.rv.ResvFrameEnd(gfc, mean_bits);
        }
    }
}