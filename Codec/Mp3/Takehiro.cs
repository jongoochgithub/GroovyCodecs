//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Diagnostics;
using GroovyMp3.Types;

/*  *	MP3 huffman table selecting and bit counting  *
 *	Copyright (c) 1999-2005 Takehiro TOMINAGA
 * *	Copyright (c) 2002-2005 Gabriel Bouvigne
 * *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Library General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.	 See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */


/* $Id: Takehiro.java,v 1.26 2011/05/24 20:48:06 kenchis Exp $ */
namespace GroovyMp3.Codec.Mp3
{
    internal class Takehiro
    {

        internal class Bits
        {

            internal int bits;

            internal Bits(int b)
            {
                bits = b;
            }
        }

        private static readonly int[] huf_tbl_noESC =
        {
            1,
            2,
            5,
            7,
            7,
            10,
            10,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13
        };

        private static readonly int[] log2tab =
        {
            0,
            1,
            2,
            2,
            3,
            3,
            3,
            3,
            4,
            4,
            4,
            4,
            4,
            4,
            4,
            4
        };

        private static readonly int[][] max_range_sfac_tab =
        {
            new[]
            {
                15,
                15,
                7,
                7
            },
            new[]
            {
                15,
                15,
                7,
                0
            },
            new[]
            {
                7,
                3,
                0,
                0
            },
            new[]
            {
                15,
                31,
                31,
                0
            },
            new[]
            {
                7,
                7,
                7,
                0
            },
            new[]
            {
                3,
                3,
                0,
                0
            }
        };

        private static readonly int[] scale_long =
        {
            0,
            10,
            20,
            30,
            33,
            21,
            31,
            41,
            32,
            42,
            52,
            43,
            53,
            63,
            64,
            74
        };

        private static readonly int[] scale_mixed =
        {
            0,
            18,
            36,
            54,
            51,
            35,
            53,
            71,
            52,
            70,
            88,
            69,
            87,
            105,
            104,
            122
        };

        private static readonly int[] scale_short =
        {
            0,
            18,
            36,
            54,
            54,
            36,
            54,
            72,
            54,
            72,
            90,
            72,
            90,
            108,
            108,
            126
        };

        private static readonly int[] slen1_n =
        {
            1,
            1,
            1,
            1,
            8,
            2,
            2,
            2,
            4,
            4,
            4,
            8,
            8,
            8,
            16,
            16
        };

        internal static readonly int[] slen1_tab =
        {
            0,
            0,
            0,
            0,
            3,
            1,
            1,
            1,
            2,
            2,
            2,
            3,
            3,
            3,
            4,
            4
        };

        private static readonly int[] slen2_n =
        {
            1,
            2,
            4,
            8,
            1,
            2,
            4,
            8,
            2,
            4,
            8,
            2,
            4,
            8,
            4,
            8
        };

        internal static readonly int[] slen2_tab =
        {
            0,
            1,
            2,
            3,
            0,
            1,
            2,
            3,
            1,
            2,
            3,
            1,
            2,
            3,
            2,
            3
        };

        internal QuantizePVT qupvt;

        private readonly int[][] subdv_table =
        {
            new[]
            {
                0,
                0
            },
            new[]
            {
                0,
                0
            },
            new[]
            {
                0,
                0
            },
            new[]
            {
                0,
                0
            },
            new[]
            {
                0,
                0
            },
            new[]
            {
                0,
                1
            },
            new[]
            {
                1,
                1
            },
            new[]
            {
                1,
                1
            },
            new[]
            {
                1,
                2
            },
            new[]
            {
                2,
                2
            },
            new[]
            {
                2,
                3
            },
            new[]
            {
                2,
                3
            },
            new[]
            {
                3,
                4
            },
            new[]
            {
                3,
                4
            },
            new[]
            {
                3,
                4
            },
            new[]
            {
                4,
                5
            },
            new[]
            {
                4,
                5
            },
            new[]
            {
                4,
                6
            },
            new[]
            {
                5,
                6
            },
            new[]
            {
                5,
                6
            },
            new[]
            {
                5,
                7
            },
            new[]
            {
                6,
                7
            },
            new[]
            {
                6,
                7
            }
        };

        internal QuantizePVT Modules
        {
            set => qupvt = value;
        }

        private void quantize_lines_xrpow_01(int l, float istep, float[] xr, int xrPos, int[] ix, int ixPos)
        {

            var compareval0 = (1.0f - 0.4054f) / istep;
            Debug.Assert(l > 0);
            l = l >> 1;
            while (l-- != 0)
            {
                ix[ixPos++] = compareval0 > xr[xrPos++] ? 0 : 1;
                ix[ixPos++] = compareval0 > xr[xrPos++] ? 0 : 1;
            }
        }

        private void quantize_lines_xrpow(int l, float istep, float[] xr, int xrPos, int[] ix, int ixPos)
        {
            Debug.Assert(l > 0);
            l = l >> 1;
            var remaining = l % 2;
            l = l >> 1;
            while (l-- != 0)
            {
                float x0, x1, x2, x3;
                int rx0, rx1, rx2, rx3;
                x0 = xr[xrPos++] * istep;
                x1 = xr[xrPos++] * istep;
                rx0 = (int)x0;
                x2 = xr[xrPos++] * istep;
                rx1 = (int)x1;
                x3 = xr[xrPos++] * istep;
                rx2 = (int)x2;
                x0 += qupvt.adj43[rx0];
                rx3 = (int)x3;
                x1 += qupvt.adj43[rx1];
                ix[ixPos++] = (int)x0;
                x2 += qupvt.adj43[rx2];
                ix[ixPos++] = (int)x1;
                x3 += qupvt.adj43[rx3];
                ix[ixPos++] = (int)x2;
                ix[ixPos++] = (int)x3;
            }

            if (remaining != 0)
            {
                float x0, x1;
                int rx0, rx1;
                x0 = xr[xrPos++] * istep;
                x1 = xr[xrPos++] * istep;
                rx0 = (int)x0;
                rx1 = (int)x1;
                x0 += qupvt.adj43[rx0];
                x1 += qupvt.adj43[rx1];
                ix[ixPos++] = (int)x0;
                ix[ixPos++] = (int)x1;
            }
        }

        private void quantize_xrpow(float[] xp, int[] pi, float istep, GrInfo codInfo, CalcNoiseData prevNoise)
        {
            int sfb;
            int sfbmax;
            var j = 0;
            bool prev_data_use;
            var accumulate = 0;
            var accumulate01 = 0;
            var xpPos = 0;
            var iData = pi;
            var iDataPos = 0;
            var acc_iData = iData;
            var acc_iDataPos = 0;
            var acc_xp = xp;
            var acc_xpPos = 0;
            prev_data_use = prevNoise != null && codInfo.global_gain == prevNoise.global_gain;
            if (codInfo.block_type == Encoder.SHORT_TYPE)
                sfbmax = 38;
            else
                sfbmax = 21;

            for (sfb = 0; sfb <= sfbmax; sfb++)
            {
                var step = -1;
                if (prev_data_use || codInfo.block_type == Encoder.NORM_TYPE)
                    step = codInfo.global_gain -
                           ((codInfo.scalefac[sfb] + (codInfo.preflag != 0 ? qupvt.pretab[sfb] : 0)) <<
                            (codInfo.scalefac_scale + 1)) - codInfo.subblock_gain[codInfo.window[sfb]] * 8;

                Debug.Assert(codInfo.width[sfb] >= 0);
                if (prev_data_use && prevNoise.step[sfb] == step)
                {
                    if (accumulate != 0)
                    {
                        quantize_lines_xrpow(accumulate, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                        accumulate = 0;
                    }

                    if (accumulate01 != 0)
                    {
                        quantize_lines_xrpow_01(accumulate01, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                        accumulate01 = 0;
                    }
                }
                else
                {
                    var l = codInfo.width[sfb];
                    if (j + codInfo.width[sfb] > codInfo.max_nonzero_coeff)
                    {
                        int usefullsize;
                        usefullsize = codInfo.max_nonzero_coeff - j + 1;
                        Arrays.Fill(pi, codInfo.max_nonzero_coeff, 576, 0);
                        l = usefullsize;
                        if (l < 0)
                            l = 0;

                        sfb = sfbmax + 1;
                    }

                    if (0 == accumulate && 0 == accumulate01)
                    {
                        acc_iData = iData;
                        acc_iDataPos = iDataPos;
                        acc_xp = xp;
                        acc_xpPos = xpPos;
                    }

                    if (prevNoise != null && prevNoise.sfb_count1 > 0 && sfb >= prevNoise.sfb_count1 &&
                        prevNoise.step[sfb] > 0 && step >= prevNoise.step[sfb])
                    {
                        if (accumulate != 0)
                        {
                            quantize_lines_xrpow(accumulate, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                            accumulate = 0;
                            acc_iData = iData;
                            acc_iDataPos = iDataPos;
                            acc_xp = xp;
                            acc_xpPos = xpPos;
                        }

                        accumulate01 += l;
                    }
                    else
                    {
                        if (accumulate01 != 0)
                        {
                            quantize_lines_xrpow_01(accumulate01, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                            accumulate01 = 0;
                            acc_iData = iData;
                            acc_iDataPos = iDataPos;
                            acc_xp = xp;
                            acc_xpPos = xpPos;
                        }

                        accumulate += l;
                    }

                    if (l <= 0)
                    {
                        if (accumulate01 != 0)
                        {
                            quantize_lines_xrpow_01(accumulate01, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                            accumulate01 = 0;
                        }

                        if (accumulate != 0)
                        {
                            quantize_lines_xrpow(accumulate, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                            accumulate = 0;
                        }

                        break;
                    }
                }

                if (sfb <= sfbmax)
                {
                    iDataPos += codInfo.width[sfb];
                    xpPos += codInfo.width[sfb];
                    j += codInfo.width[sfb];
                }
            }

            if (accumulate != 0)
            {
                quantize_lines_xrpow(accumulate, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                accumulate = 0;
            }

            if (accumulate01 != 0)
            {
                quantize_lines_xrpow_01(accumulate01, istep, acc_xp, acc_xpPos, acc_iData, acc_iDataPos);
                accumulate01 = 0;
            }
        }

        private int ix_max(int[] ix, int ixPos, int endPos)
        {
            int max1 = 0, max2 = 0;
            do
            {

                var x1 = ix[ixPos++];

                var x2 = ix[ixPos++];
                if (max1 < x1)
                    max1 = x1;

                if (max2 < x2)
                    max2 = x2;
            }
            while (ixPos < endPos);

            if (max1 < max2)
                max1 = max2;

            return max1;
        }

        private int count_bit_ESC(int[] ix, int ixPos, int end, int t1, int t2, Bits s)
        {

            var linbits = Tables.ht[t1].xlen * 65536 + Tables.ht[t2].xlen;
            int sum = 0, sum2;
            do
            {
                var x = ix[ixPos++];
                var y = ix[ixPos++];
                if (x != 0)
                {
                    if (x > 14)
                    {
                        x = 15;
                        sum += linbits;
                    }

                    x *= 16;
                }

                if (y != 0)
                {
                    if (y > 14)
                    {
                        y = 15;
                        sum += linbits;
                    }

                    x += y;
                }

                sum += Tables.largetbl[x];
            }
            while (ixPos < end);

            sum2 = sum & 0xffff;
            sum >>= 16;
            if (sum > sum2)
            {
                sum = sum2;
                t1 = t2;
            }

            s.bits += sum;
            return t1;
        }

        private int count_bit_noESC(int[] ix, int ixPos, int end, Bits s)
        {
            var sum1 = 0;

            var hlen1 = Tables.ht[1].hlen;
            do
            {

                var x = ix[ixPos + 0] * 2 + ix[ixPos + 1];
                ixPos += 2;
                sum1 += hlen1[x];
            }
            while (ixPos < end);

            s.bits += sum1;
            return 1;
        }

        private int count_bit_noESC_from2(int[] ix, int ixPos, int end, int t1, Bits s)
        {
            int sum = 0, sum2;

            var xlen = Tables.ht[t1].xlen;

            int[] hlen;
            if (t1 == 2)
                hlen = Tables.table23;
            else
                hlen = Tables.table56;

            do
            {

                var x = ix[ixPos + 0] * xlen + ix[ixPos + 1];
                ixPos += 2;
                sum += hlen[x];
            }
            while (ixPos < end);

            sum2 = sum & 0xffff;
            sum >>= 16;
            if (sum > sum2)
            {
                sum = sum2;
                t1++;
            }

            s.bits += sum;
            return t1;
        }

        private int count_bit_noESC_from3(int[] ix, int ixPos, int end, int t1, Bits s)
        {
            var sum1 = 0;
            var sum2 = 0;
            var sum3 = 0;

            var xlen = Tables.ht[t1].xlen;

            var hlen1 = Tables.ht[t1].hlen;

            var hlen2 = Tables.ht[t1 + 1].hlen;

            var hlen3 = Tables.ht[t1 + 2].hlen;
            do
            {

                var x = ix[ixPos + 0] * xlen + ix[ixPos + 1];
                ixPos += 2;
                sum1 += hlen1[x];
                sum2 += hlen2[x];
                sum3 += hlen3[x];
            }
            while (ixPos < end);

            var t = t1;
            if (sum1 > sum2)
            {
                sum1 = sum2;
                t++;
            }

            if (sum1 > sum3)
            {
                sum1 = sum3;
                t = t1 + 2;
            }

            s.bits += sum1;
            return t;
        }

        private int choose_table(int[] ix, int ixPos, int endPos, Bits s)
        {
            var max = ix_max(ix, ixPos, endPos);
            switch (max)
            {
                case 0:
                    return max;
                case 1:
                    return count_bit_noESC(ix, ixPos, endPos, s);
                case 2:
                case 3:
                    return count_bit_noESC_from2(ix, ixPos, endPos, huf_tbl_noESC[max - 1], s);
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    return count_bit_noESC_from3(ix, ixPos, endPos, huf_tbl_noESC[max - 1], s);
                default:
                    if (max > QuantizePVT.IXMAX_VAL)
                    {
                        s.bits = QuantizePVT.LARGE_BITS;
                        return -1;
                    }

                    max -= 15;
                    int choice2;
                    for (choice2 = 24; choice2 < 32; choice2++)
                        if (Tables.ht[choice2].linmax >= max)
                            break;

                    int choice;
                    for (choice = choice2 - 8; choice < 24; choice++)
                        if (Tables.ht[choice].linmax >= max)
                            break;

                    return count_bit_ESC(ix, ixPos, endPos, choice, choice2, s);
            }
        }

        internal virtual int noquant_count_bits(LameInternalFlags gfc, GrInfo gi, CalcNoiseData prev_noise)
        {

            var ix = gi.l3_enc;
            var i = Math.Min(576, ((gi.max_nonzero_coeff + 2) >> 1) << 1);
            if (prev_noise != null)
                prev_noise.sfb_count1 = 0;

            for (; i > 1; i -= 2)
                if ((ix[i - 1] | ix[i - 2]) != 0)
                    break;

            gi.count1 = i;
            var a1 = 0;
            var a2 = 0;
            for (; i > 3; i -= 4)
            {
                int p;
                if (((ix[i - 1] | (long)ix[i - 2] | ix[i - 3] | ix[i - 4]) & 0xffffffffL) > 1L)
                    break;

                p = ((ix[i - 4] * 2 + ix[i - 3]) * 2 + ix[i - 2]) * 2 + ix[i - 1];
                a1 += Tables.t32l[p];
                a2 += Tables.t33l[p];
            }

            var bits = a1;
            gi.count1table_select = 0;
            if (a1 > a2)
            {
                bits = a2;
                gi.count1table_select = 1;
            }

            gi.count1bits = bits;
            gi.big_values = i;
            if (i == 0)
                return bits;

            if (gi.block_type == Encoder.SHORT_TYPE)
            {
                a1 = 3 * gfc.scalefac_band.s[3];
                if (a1 > gi.big_values)
                    a1 = gi.big_values;

                a2 = gi.big_values;
            }
            else if (gi.block_type == Encoder.NORM_TYPE)
            {
                Debug.Assert(i <= 576);
                a1 = gi.region0_count = gfc.bv_scf[i - 2];
                a2 = gi.region1_count = gfc.bv_scf[i - 1];
                Debug.Assert(a1 + a2 + 2 < Encoder.SBPSY_l);
                a2 = gfc.scalefac_band.l[a1 + a2 + 2];
                a1 = gfc.scalefac_band.l[a1 + 1];
                if (a2 < i)
                {
                    var bi = new Bits(bits);
                    gi.table_select[2] = choose_table(ix, a2, i, bi);
                    bits = bi.bits;
                }
            }
            else
            {
                gi.region0_count = 7;
                gi.region1_count = Encoder.SBMAX_l - 1 - 7 - 1;
                a1 = gfc.scalefac_band.l[7 + 1];
                a2 = i;
                if (a1 > a2)
                    a1 = a2;
            }

            a1 = Math.Min(a1, i);
            a2 = Math.Min(a2, i);
            Debug.Assert(a1 >= 0);
            Debug.Assert(a2 >= 0);
            if (0 < a1)
            {
                var bi = new Bits(bits);
                gi.table_select[0] = choose_table(ix, 0, a1, bi);
                bits = bi.bits;
            }

            if (a1 < a2)
            {
                var bi = new Bits(bits);
                gi.table_select[1] = choose_table(ix, a1, a2, bi);
                bits = bi.bits;
            }

            if (gfc.use_best_huffman == 2)
            {
                gi.part2_3_length = bits;
                best_huffman_divide(gfc, gi);
                bits = gi.part2_3_length;
            }

            if (prev_noise != null)
                if (gi.block_type == Encoder.NORM_TYPE)
                {
                    var sfb = 0;
                    while (gfc.scalefac_band.l[sfb] < gi.big_values)
                        sfb++;

                    prev_noise.sfb_count1 = sfb;
                }

            return bits;
        }

        internal virtual int count_bits(LameInternalFlags gfc, float[] xr, GrInfo gi, CalcNoiseData prev_noise)
        {

            var ix = gi.l3_enc;

            var w = QuantizePVT.IXMAX_VAL / qupvt.IPOW20(gi.global_gain);
            if (gi.xrpow_max > w)
                return QuantizePVT.LARGE_BITS;

            quantize_xrpow(xr, ix, qupvt.IPOW20(gi.global_gain), gi, prev_noise);
            if ((gfc.substep_shaping & 2) != 0)
            {
                var j = 0;

                var gain = gi.global_gain + gi.scalefac_scale;

                var roundfac = 0.634521682242439f / qupvt.IPOW20(gain);
                for (var sfb = 0; sfb < gi.sfbmax; sfb++)
                {

                    var width = gi.width[sfb];
                    Debug.Assert(width >= 0);
                    if (0 == gfc.pseudohalf[sfb])
                    {
                        j += width;
                    }
                    else
                    {
                        int k;
                        for (k = j, j += width; k < j; ++k)
                            ix[k] = xr[k] >= roundfac ? ix[k] : 0;
                    }
                }
            }

            return noquant_count_bits(gfc, gi, prev_noise);
        }

        private void recalc_divide_init(
            LameInternalFlags gfc,
            GrInfo cod_info,
            int[] ix,
            int[] r01_bits,
            int[] r01_div,
            int[] r0_tbl,
            int[] r1_tbl)
        {
            var bigv = cod_info.big_values;
            for (var r0 = 0; r0 <= 7 + 15; r0++)
                r01_bits[r0] = QuantizePVT.LARGE_BITS;

            for (var r0 = 0; r0 < 16; r0++)
            {

                var a1 = gfc.scalefac_band.l[r0 + 1];
                if (a1 >= bigv)
                    break;

                var r0bits = 0;
                var bi = new Bits(r0bits);
                var r0t = choose_table(ix, 0, a1, bi);
                r0bits = bi.bits;
                for (var r1 = 0; r1 < 8; r1++)
                {

                    var a2 = gfc.scalefac_band.l[r0 + r1 + 2];
                    if (a2 >= bigv)
                        break;

                    var bits = r0bits;
                    bi = new Bits(bits);
                    var r1t = choose_table(ix, a1, a2, bi);
                    bits = bi.bits;
                    if (r01_bits[r0 + r1] > bits)
                    {
                        r01_bits[r0 + r1] = bits;
                        r01_div[r0 + r1] = r0;
                        r0_tbl[r0 + r1] = r0t;
                        r1_tbl[r0 + r1] = r1t;
                    }
                }
            }
        }

        private void recalc_divide_sub(
            LameInternalFlags gfc,
            GrInfo cod_info2,
            GrInfo gi,
            int[] ix,
            int[] r01_bits,
            int[] r01_div,
            int[] r0_tbl,
            int[] r1_tbl)
        {
            var bigv = cod_info2.big_values;
            for (var r2 = 2; r2 < Encoder.SBMAX_l + 1; r2++)
            {
                var a2 = gfc.scalefac_band.l[r2];
                if (a2 >= bigv)
                    break;

                var bits = r01_bits[r2 - 2] + cod_info2.count1bits;
                if (gi.part2_3_length <= bits)
                    break;

                var bi = new Bits(bits);
                var r2t = choose_table(ix, a2, bigv, bi);
                bits = bi.bits;
                if (gi.part2_3_length <= bits)
                    continue;

                gi.assign(cod_info2);
                gi.part2_3_length = bits;
                gi.region0_count = r01_div[r2 - 2];
                gi.region1_count = r2 - 2 - r01_div[r2 - 2];
                gi.table_select[0] = r0_tbl[r2 - 2];
                gi.table_select[1] = r1_tbl[r2 - 2];
                gi.table_select[2] = r2t;
            }
        }

        internal virtual void best_huffman_divide(LameInternalFlags gfc, GrInfo gi)
        {
            var cod_info2 = new GrInfo();

            var ix = gi.l3_enc;
            var r01_bits = new int[7 + 15 + 1];
            var r01_div = new int[7 + 15 + 1];
            var r0_tbl = new int[7 + 15 + 1];
            var r1_tbl = new int[7 + 15 + 1];
            if (gi.block_type == Encoder.SHORT_TYPE && gfc.mode_gr == 1)
                return;

            cod_info2.assign(gi);
            if (gi.block_type == Encoder.NORM_TYPE)
            {
                recalc_divide_init(gfc, gi, ix, r01_bits, r01_div, r0_tbl, r1_tbl);
                recalc_divide_sub(gfc, cod_info2, gi, ix, r01_bits, r01_div, r0_tbl, r1_tbl);
            }

            var i = cod_info2.big_values;
            if (i == 0 || (ix[i - 2] | ix[i - 1]) > 1)
                return;

            i = gi.count1 + 2;
            if (i > 576)
                return;

            cod_info2.assign(gi);
            cod_info2.count1 = i;
            var a1 = 0;
            var a2 = 0;
            Debug.Assert(i <= 576);
            for (; i > cod_info2.big_values; i -= 4)
            {

                var p = ((ix[i - 4] * 2 + ix[i - 3]) * 2 + ix[i - 2]) * 2 + ix[i - 1];
                a1 += Tables.t32l[p];
                a2 += Tables.t33l[p];
            }

            cod_info2.big_values = i;
            cod_info2.count1table_select = 0;
            if (a1 > a2)
            {
                a1 = a2;
                cod_info2.count1table_select = 1;
            }

            cod_info2.count1bits = a1;
            if (cod_info2.block_type == Encoder.NORM_TYPE)
            {
                recalc_divide_sub(gfc, cod_info2, gi, ix, r01_bits, r01_div, r0_tbl, r1_tbl);
            }
            else
            {
                cod_info2.part2_3_length = a1;
                a1 = gfc.scalefac_band.l[7 + 1];
                if (a1 > i)
                    a1 = i;

                if (a1 > 0)
                {
                    var bi = new Bits(cod_info2.part2_3_length);
                    cod_info2.table_select[0] = choose_table(ix, 0, a1, bi);
                    cod_info2.part2_3_length = bi.bits;
                }

                if (i > a1)
                {
                    var bi = new Bits(cod_info2.part2_3_length);
                    cod_info2.table_select[1] = choose_table(ix, a1, i, bi);
                    cod_info2.part2_3_length = bi.bits;
                }

                if (gi.part2_3_length > cod_info2.part2_3_length)
                    gi.assign(cod_info2);
            }
        }

        private void scfsi_calc(int ch, IIISideInfo l3_side)
        {
            int sfb;

            var gi = l3_side.tt[1][ch];

            var g0 = l3_side.tt[0][ch];
            for (var i = 0; i < Tables.scfsi_band.Length - 1; i++)
            {
                for (sfb = Tables.scfsi_band[i]; sfb < Tables.scfsi_band[i + 1]; sfb++)
                    if (g0.scalefac[sfb] != gi.scalefac[sfb] && gi.scalefac[sfb] >= 0)
                        break;

                if (sfb == Tables.scfsi_band[i + 1])
                {
                    for (sfb = Tables.scfsi_band[i]; sfb < Tables.scfsi_band[i + 1]; sfb++)
                        gi.scalefac[sfb] = -1;

                    l3_side.scfsi[ch][i] = 1;
                }
            }

            var s1 = 0;
            var c1 = 0;
            for (sfb = 0; sfb < 11; sfb++)
            {
                if (gi.scalefac[sfb] == -1)
                    continue;

                c1++;
                if (s1 < gi.scalefac[sfb])
                    s1 = gi.scalefac[sfb];
            }

            var s2 = 0;
            var c2 = 0;
            for (; sfb < Encoder.SBPSY_l; sfb++)
            {
                if (gi.scalefac[sfb] == -1)
                    continue;

                c2++;
                if (s2 < gi.scalefac[sfb])
                    s2 = gi.scalefac[sfb];
            }

            for (var i = 0; i < 16; i++)
                if (s1 < slen1_n[i] && s2 < slen2_n[i])
                {

                    var c = slen1_tab[i] * c1 + slen2_tab[i] * c2;
                    if (gi.part2_length > c)
                    {
                        gi.part2_length = c;
                        gi.scalefac_compress = i;
                    }
                }
        }

        internal virtual void best_scalefac_store(LameInternalFlags gfc, int gr, int ch, IIISideInfo l3_side)
        {

            var gi = l3_side.tt[gr][ch];
            int sfb, i, j, l;
            var recalc = 0;
            j = 0;
            for (sfb = 0; sfb < gi.sfbmax; sfb++)
            {

                var width = gi.width[sfb];
                Debug.Assert(width >= 0);
                j += width;
                for (l = -width; l < 0; l++)
                    if (gi.l3_enc[l + j] != 0)
                        break;

                if (l == 0)
                    gi.scalefac[sfb] = recalc = -2;
            }

            if (0 == gi.scalefac_scale && 0 == gi.preflag)
            {
                var s = 0;
                for (sfb = 0; sfb < gi.sfbmax; sfb++)
                    if (gi.scalefac[sfb] > 0)
                        s |= gi.scalefac[sfb];

                if (0 == (s & 1) && s != 0)
                {
                    for (sfb = 0; sfb < gi.sfbmax; sfb++)
                        if (gi.scalefac[sfb] > 0)
                            gi.scalefac[sfb] >>= 1;

                    gi.scalefac_scale = recalc = 1;
                }
            }

            if (0 == gi.preflag && gi.block_type != Encoder.SHORT_TYPE && gfc.mode_gr == 2)
            {
                for (sfb = 11; sfb < Encoder.SBPSY_l; sfb++)
                    if (gi.scalefac[sfb] < qupvt.pretab[sfb] && gi.scalefac[sfb] != -2)
                        break;

                if (sfb == Encoder.SBPSY_l)
                {
                    for (sfb = 11; sfb < Encoder.SBPSY_l; sfb++)
                        if (gi.scalefac[sfb] > 0)
                            gi.scalefac[sfb] -= qupvt.pretab[sfb];

                    gi.preflag = recalc = 1;
                }
            }

            for (i = 0; i < 4; i++)
                l3_side.scfsi[ch][i] = 0;

            if (gfc.mode_gr == 2 && gr == 1 && l3_side.tt[0][ch].block_type != Encoder.SHORT_TYPE &&
                l3_side.tt[1][ch].block_type != Encoder.SHORT_TYPE)
            {
                scfsi_calc(ch, l3_side);
                recalc = 0;
            }

            for (sfb = 0; sfb < gi.sfbmax; sfb++)
                if (gi.scalefac[sfb] == -2)
                    gi.scalefac[sfb] = 0;

            if (recalc != 0)
                if (gfc.mode_gr == 2)
                    scale_bitcount(gi);
                else
                    scale_bitcount_lsf(gfc, gi);
        }

        private bool all_scalefactors_not_negative(int[] scalefac, int n)
        {
            for (var i = 0; i < n; ++i)
                if (scalefac[i] < 0)
                    return false;

            return true;
        }

        internal virtual bool scale_bitcount(GrInfo cod_info)
        {
            int k, sfb, max_slen1 = 0, max_slen2 = 0;
            int[] tab;

            var scalefac = cod_info.scalefac;
            Debug.Assert(all_scalefactors_not_negative(scalefac, cod_info.sfbmax));
            if (cod_info.block_type == Encoder.SHORT_TYPE)
            {
                tab = scale_short;
                if (cod_info.mixed_block_flag != 0)
                    tab = scale_mixed;
            }
            else
            {
                tab = scale_long;
                if (0 == cod_info.preflag)
                {
                    for (sfb = 11; sfb < Encoder.SBPSY_l; sfb++)
                        if (scalefac[sfb] < qupvt.pretab[sfb])
                            break;

                    if (sfb == Encoder.SBPSY_l)
                    {
                        cod_info.preflag = 1;
                        for (sfb = 11; sfb < Encoder.SBPSY_l; sfb++)
                            scalefac[sfb] -= qupvt.pretab[sfb];
                    }
                }
            }

            for (sfb = 0; sfb < cod_info.sfbdivide; sfb++)
                if (max_slen1 < scalefac[sfb])
                    max_slen1 = scalefac[sfb];

            for (; sfb < cod_info.sfbmax; sfb++)
                if (max_slen2 < scalefac[sfb])
                    max_slen2 = scalefac[sfb];

            cod_info.part2_length = QuantizePVT.LARGE_BITS;
            for (k = 0; k < 16; k++)
                if (max_slen1 < slen1_n[k] && max_slen2 < slen2_n[k] && cod_info.part2_length > tab[k])
                {
                    cod_info.part2_length = tab[k];
                    cod_info.scalefac_compress = k;
                }

            return cod_info.part2_length == QuantizePVT.LARGE_BITS;
        }

        internal virtual bool scale_bitcount_lsf(LameInternalFlags gfc, GrInfo cod_info)
        {
            int table_number, row_in_table, partition, nr_sfb, window;
            bool over;
            int i;
            int sfb;
            var max_sfac = new int[4];

            int[] partition_table;

            var scalefac = cod_info.scalefac;
            if (cod_info.preflag != 0)
                table_number = 2;
            else
                table_number = 0;

            for (i = 0; i < 4; i++)
                max_sfac[i] = 0;

            if (cod_info.block_type == Encoder.SHORT_TYPE)
            {
                row_in_table = 1;
                partition_table = qupvt.nr_of_sfb_block[table_number][row_in_table];
                for (sfb = 0, partition = 0; partition < 4; partition++)
                {
                    nr_sfb = partition_table[partition] / 3;
                    for (i = 0; i < nr_sfb; i++, sfb++)
                    for (window = 0; window < 3; window++)
                        if (scalefac[sfb * 3 + window] > max_sfac[partition])
                            max_sfac[partition] = scalefac[sfb * 3 + window];
                }
            }
            else
            {
                row_in_table = 0;
                partition_table = qupvt.nr_of_sfb_block[table_number][row_in_table];
                for (sfb = 0, partition = 0; partition < 4; partition++)
                {
                    nr_sfb = partition_table[partition];
                    for (i = 0; i < nr_sfb; i++, sfb++)
                        if (scalefac[sfb] > max_sfac[partition])
                            max_sfac[partition] = scalefac[sfb];
                }
            }

            for (over = false, partition = 0; partition < 4; partition++)
                if (max_sfac[partition] > max_range_sfac_tab[table_number][partition])
                    over = true;

            if (!over)
            {
                int slen1, slen2, slen3, slen4;
                cod_info.sfb_partition_table = qupvt.nr_of_sfb_block[table_number][row_in_table];
                for (partition = 0; partition < 4; partition++)
                    cod_info.slen[partition] = log2tab[max_sfac[partition]];

                slen1 = cod_info.slen[0];
                slen2 = cod_info.slen[1];
                slen3 = cod_info.slen[2];
                slen4 = cod_info.slen[3];
                switch (table_number)
                {
                    case 0:
                        cod_info.scalefac_compress = ((slen1 * 5 + slen2) << 4) + (slen3 << 2) + slen4;
                        break;
                    case 1:
                        cod_info.scalefac_compress = 400 + ((slen1 * 5 + slen2) << 2) + slen3;
                        break;
                    case 2:
                        cod_info.scalefac_compress = 500 + slen1 * 3 + slen2;
                        break;
                    default:
                        Console.WriteLine("intensity stereo not implemented yet\n");
                        break;
                }
            }

            if (!over)
            {
                Debug.Assert(cod_info.sfb_partition_table != null);
                cod_info.part2_length = 0;
                for (partition = 0; partition < 4; partition++)
                    cod_info.part2_length += cod_info.slen[partition] * cod_info.sfb_partition_table[partition];
            }

            return over;
        }

        internal virtual void huffman_init(LameInternalFlags gfc)
        {
            for (var i = 2; i <= 576; i += 2)
            {
                int scfb_anz = 0, bv_index;
                while (gfc.scalefac_band.l[++scfb_anz] < i)
                    ;

                bv_index = subdv_table[scfb_anz][0];
                while (gfc.scalefac_band.l[bv_index + 1] > i)
                    bv_index--;

                if (bv_index < 0)
                    bv_index = subdv_table[scfb_anz][0];

                gfc.bv_scf[i - 2] = bv_index;
                bv_index = subdv_table[scfb_anz][1];
                while (gfc.scalefac_band.l[bv_index + gfc.bv_scf[i - 2] + 2] > i)
                    bv_index--;

                if (bv_index < 0)
                    bv_index = subdv_table[scfb_anz][1];

                gfc.bv_scf[i - 1] = bv_index;
            }
        }
    }
}