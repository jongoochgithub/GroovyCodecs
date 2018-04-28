//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using GroovyMp3.Types;

namespace GroovyMp3.Codec.Mp3
{
    /// <summary>
    ///     tries to find out how many bits are needed for each granule and channel
    ///     to get an acceptable quantization. An appropriate bitrate will then be
    ///     chosen for quantization. rh 8/99
    ///     Robert Hegemann 2000-09-06 rewrite
    ///     @author Ken
    /// </summary>
    internal sealed class VBROldIterationLoop : IIterationLoop
    {

        /// 
        private readonly Quantize quantize;

        /// <param name="quantize"> </param>
        internal VBROldIterationLoop(Quantize quantize)
        {
            this.quantize = quantize;
        }

        public void iteration_loop(LameGlobalFlags gfp, float[][] pe, float[] ms_ener_ratio, III_psy_ratio[][] ratio)
        {

            var gfc = gfp.internal_flags;

            var l3_xmin = Arrays.ReturnRectangularArray<float>(2, 2, L3Side.SFBMAX);

            var xrpow = new float[576];

            var bands = Arrays.ReturnRectangularArray<int>(2, 2);
            var frameBits = new int[15];

            var min_bits = Arrays.ReturnRectangularArray<int>(2, 2);
            var max_bits = Arrays.ReturnRectangularArray<int>(2, 2);
            var mean_bits = 0;

            var l3_side = gfc.l3_side;

            var analog_silence = quantize.VBR_old_prepare(
                gfp,
                pe,
                ms_ener_ratio,
                ratio,
                l3_xmin,
                frameBits,
                min_bits,
                max_bits,
                bands);

            /*---------------------------------*/
            for (;;)
            {
                /*
                 * quantize granules with lowest possible number of bits
                 */
                var used_bits = 0;

                for (var gr = 0; gr < gfc.mode_gr; gr++)
                for (var ch = 0; ch < gfc.channels_out; ch++)
                {

                    var cod_info = l3_side.tt[gr][ch];

                    /*
                         * init_outer_loop sets up cod_info, scalefac and xrpow
                         */
                    var ret = quantize.init_xrpow(gfc, cod_info, xrpow);
                    if (!ret || max_bits[gr][ch] == 0)
                        continue; // with next channel

                    quantize.VBR_encode_granule(
                        gfp,
                        cod_info,
                        l3_xmin[gr][ch],
                        xrpow,
                        ch,
                        min_bits[gr][ch],
                        max_bits[gr][ch]);

                    /*
                         * do the 'substep shaping'
                         */
                    if ((gfc.substep_shaping & 1) != 0)
                        quantize.trancate_smallspectrums(gfc, l3_side.tt[gr][ch], l3_xmin[gr][ch], xrpow);

                    var usedB = cod_info.part2_3_length + cod_info.part2_length;
                    used_bits += usedB;
                } // for ch

                /*
                 * find lowest bitrate able to hold used bits
                 */
                if (analog_silence != 0 && 0 == gfp.VBR_hard_min)
                    gfc.bitrate_index = 1;
                else
                    gfc.bitrate_index = gfc.VBR_min_bitrate;

                for (; gfc.bitrate_index < gfc.VBR_max_bitrate; gfc.bitrate_index++)
                    if (used_bits <= frameBits[gfc.bitrate_index])
                        break;

                var mb = new MeanBits(mean_bits);
                var bits = quantize.rv.ResvFrameBegin(gfp, mb);
                mean_bits = mb.bits;

                if (used_bits <= bits)
                    break;

                quantize.bitpressure_strategy(gfc, l3_xmin, min_bits, max_bits);

            }
            /* breaks adjusted */
            /*--------------------------------------*/

            for (var gr = 0; gr < gfc.mode_gr; gr++)
            for (var ch = 0; ch < gfc.channels_out; ch++)
                quantize.iteration_finish_one(gfc, gr, ch);

            quantize.rv.ResvFrameEnd(gfc, mean_bits);
        }
    }
}