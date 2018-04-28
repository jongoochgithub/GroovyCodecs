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
    internal sealed class VBRNewIterationLoop : IIterationLoop
    {

        /// 
        private readonly Quantize quantize;

        /// <param name="quantize"> </param>
        internal VBRNewIterationLoop(Quantize quantize)
        {
            this.quantize = quantize;
        }

        public void iteration_loop(LameGlobalFlags gfp, float[][] pe, float[] ms_ener_ratio, III_psy_ratio[][] ratio)
        {
            var gfc = gfp.internal_flags;

            var l3_xmin = Arrays.ReturnRectangularArray<float>(2, 2, L3Side.SFBMAX);

            var xrpow = Arrays.ReturnRectangularArray<float>(2, 2, 576);
            var frameBits = new int[15];

            var max_bits = Arrays.ReturnRectangularArray<int>(2, 2);

            var l3_side = gfc.l3_side;

            var analog_silence = quantize.VBR_new_prepare(gfp, pe, ratio, l3_xmin, frameBits, max_bits);

            for (var gr = 0; gr < gfc.mode_gr; gr++)
            for (var ch = 0; ch < gfc.channels_out; ch++)
            {

                var cod_info = l3_side.tt[gr][ch];

                /*
                     * init_outer_loop sets up cod_info, scalefac and xrpow
                     */
                if (!quantize.init_xrpow(gfc, cod_info, xrpow[gr][ch]))
                    max_bits[gr][ch] = 0;
            } // for ch

            /*
             * quantize granules with lowest possible number of bits
             */

            var used_bits = quantize.vbr.VBR_encode_frame(gfc, xrpow, l3_xmin, max_bits);

            if (!gfp.free_format)
            {
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

                if (gfc.bitrate_index > gfc.VBR_max_bitrate)
                    gfc.bitrate_index = gfc.VBR_max_bitrate;
            }
            else
            {
                gfc.bitrate_index = 0;
            }

            if (used_bits <= frameBits[gfc.bitrate_index])
            {
                /* update Reservoire status */
                int mean_bits = 0, fullframebits;
                var mb = new MeanBits(mean_bits);
                fullframebits = quantize.rv.ResvFrameBegin(gfp, mb);
                mean_bits = mb.bits;
                Debug.Assert(used_bits <= fullframebits);
                for (var gr = 0; gr < gfc.mode_gr; gr++)
                for (var ch = 0; ch < gfc.channels_out; ch++)
                {

                    var cod_info = l3_side.tt[gr][ch];
                    quantize.rv.ResvAdjust(gfc, cod_info);
                }

                quantize.rv.ResvFrameEnd(gfc, mean_bits);
            }
            else
            {
                /*
                 * SHOULD NOT HAPPEN INTERNAL ERROR
                 */
                throw new Exception("INTERNAL ERROR IN VBR NEW CODE, please send bug report");
            }
        }
    }
}