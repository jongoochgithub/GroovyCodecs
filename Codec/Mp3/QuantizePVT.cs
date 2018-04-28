//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Diagnostics;

/*  *      quantize_pvt source file  *
 *      Copyright (c) 1999-2002 Takehiro Tominaga
 * *      Copyright (c) 2000-2002 Robert Hegemann
 * *      Copyright (c) 2001 Naoki Shibata
 * *      Copyright (c) 2002-2005 Gabriel Bouvigne
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

namespace GroovyMp3.Codec.Mp3
{
    internal class QuantizePVT
    {

        private class StartLine
        {

            internal int s;

            internal StartLine(int j)
            {
                s = j;
            }
        }

        private const float DBL_EPSILON = 2.2204460492503131e-016f;

        internal const int IXMAX_VAL = 8206;

        internal const int LARGE_BITS = 100000;

        private const int NSATHSCALE = 100;

        private static readonly int PRECALC_SIZE = IXMAX_VAL + 2;

        private const int Q_MAX = 256 + 1;

        internal const int Q_MAX2 = 116;

        internal readonly int[][][] nr_of_sfb_block =
        {
            new[]
            {
                new[]
                {
                    6,
                    5,
                    5,
                    5
                },
                new[]
                {
                    9,
                    9,
                    9,
                    9
                },
                new[]
                {
                    6,
                    9,
                    9,
                    9
                }
            },
            new[]
            {
                new[]
                {
                    6,
                    5,
                    7,
                    3
                },
                new[]
                {
                    9,
                    9,
                    12,
                    6
                },
                new[]
                {
                    6,
                    9,
                    12,
                    6
                }
            },
            new[]
            {
                new[]
                {
                    11,
                    10,
                    0,
                    0
                },
                new[]
                {
                    18,
                    18,
                    0,
                    0
                },
                new[]
                {
                    15,
                    18,
                    0,
                    0
                }
            },
            new[]
            {
                new[]
                {
                    7,
                    7,
                    7,
                    0
                },
                new[]
                {
                    12,
                    12,
                    12,
                    0
                },
                new[]
                {
                    6,
                    15,
                    12,
                    0
                }
            },
            new[]
            {
                new[]
                {
                    6,
                    6,
                    6,
                    3
                },
                new[]
                {
                    12,
                    9,
                    9,
                    6
                },
                new[]
                {
                    6,
                    12,
                    9,
                    6
                }
            },
            new[]
            {
                new[]
                {
                    8,
                    8,
                    5,
                    0
                },
                new[]
                {
                    15,
                    12,
                    9,
                    0
                },
                new[]
                {
                    6,
                    18,
                    9,
                    0
                }
            }
        };

        internal readonly int[] pretab =
        {
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1,
            1,
            1,
            1,
            2,
            2,
            3,
            3,
            3,
            2,
            0
        };

        internal readonly ScaleFac[] sfBandIndex =
        {
            new ScaleFac(
                new[]
                {
                    0,
                    6,
                    12,
                    18,
                    24,
                    30,
                    36,
                    44,
                    54,
                    66,
                    80,
                    96,
                    116,
                    140,
                    168,
                    200,
                    238,
                    284,
                    336,
                    396,
                    464,
                    522,
                    576
                },
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    18,
                    24,
                    32,
                    42,
                    56,
                    74,
                    100,
                    132,
                    174,
                    192
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    6,
                    12,
                    18,
                    24,
                    30,
                    36,
                    44,
                    54,
                    66,
                    80,
                    96,
                    114,
                    136,
                    162,
                    194,
                    232,
                    278,
                    332,
                    394,
                    464,
                    540,
                    576
                },
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    18,
                    26,
                    36,
                    48,
                    62,
                    80,
                    104,
                    136,
                    180,
                    192
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    6,
                    12,
                    18,
                    24,
                    30,
                    36,
                    44,
                    54,
                    66,
                    80,
                    96,
                    116,
                    140,
                    168,
                    200,
                    238,
                    284,
                    336,
                    396,
                    464,
                    522,
                    576
                },
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    18,
                    26,
                    36,
                    48,
                    62,
                    80,
                    104,
                    134,
                    174,
                    192
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    16,
                    20,
                    24,
                    30,
                    36,
                    44,
                    52,
                    62,
                    74,
                    90,
                    110,
                    134,
                    162,
                    196,
                    238,
                    288,
                    342,
                    418,
                    576
                },
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    16,
                    22,
                    30,
                    40,
                    52,
                    66,
                    84,
                    106,
                    136,
                    192
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    16,
                    20,
                    24,
                    30,
                    36,
                    42,
                    50,
                    60,
                    72,
                    88,
                    106,
                    128,
                    156,
                    190,
                    230,
                    276,
                    330,
                    384,
                    576
                },
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    16,
                    22,
                    28,
                    38,
                    50,
                    64,
                    80,
                    100,
                    126,
                    192
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    16,
                    20,
                    24,
                    30,
                    36,
                    44,
                    54,
                    66,
                    82,
                    102,
                    126,
                    156,
                    194,
                    240,
                    296,
                    364,
                    448,
                    550,
                    576
                },
                new[]
                {
                    0,
                    4,
                    8,
                    12,
                    16,
                    22,
                    30,
                    42,
                    58,
                    78,
                    104,
                    138,
                    180,
                    192
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    6,
                    12,
                    18,
                    24,
                    30,
                    36,
                    44,
                    54,
                    66,
                    80,
                    96,
                    116,
                    140,
                    168,
                    200,
                    238,
                    284,
                    336,
                    396,
                    464,
                    522,
                    576
                },
                new[]
                {
                    0 / 3,
                    12 / 3,
                    24 / 3,
                    36 / 3,
                    54 / 3,
                    78 / 3,
                    108 / 3,
                    144 / 3,
                    186 / 3,
                    240 / 3,
                    312 / 3,
                    402 / 3,
                    522 / 3,
                    576 / 3
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    6,
                    12,
                    18,
                    24,
                    30,
                    36,
                    44,
                    54,
                    66,
                    80,
                    96,
                    116,
                    140,
                    168,
                    200,
                    238,
                    284,
                    336,
                    396,
                    464,
                    522,
                    576
                },
                new[]
                {
                    0 / 3,
                    12 / 3,
                    24 / 3,
                    36 / 3,
                    54 / 3,
                    78 / 3,
                    108 / 3,
                    144 / 3,
                    186 / 3,
                    240 / 3,
                    312 / 3,
                    402 / 3,
                    522 / 3,
                    576 / 3
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                }),
            new ScaleFac(
                new[]
                {
                    0,
                    12,
                    24,
                    36,
                    48,
                    60,
                    72,
                    88,
                    108,
                    132,
                    160,
                    192,
                    232,
                    280,
                    336,
                    400,
                    476,
                    566,
                    568,
                    570,
                    572,
                    574,
                    576
                },
                new[]
                {
                    0 / 3,
                    24 / 3,
                    48 / 3,
                    72 / 3,
                    108 / 3,
                    156 / 3,
                    216 / 3,
                    288 / 3,
                    372 / 3,
                    480 / 3,
                    486 / 3,
                    492 / 3,
                    498 / 3,
                    576 / 3
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                new[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                })
        };

        internal float[] adj43 = new float[PRECALC_SIZE];

        internal float[] ipow20 = new float[Q_MAX];

        internal float[] pow20 = new float[Q_MAX + Q_MAX2 + 1];

        internal float[] pow43 = new float[PRECALC_SIZE];

        internal PsyModel psy;

        internal Reservoir rv;

        internal Takehiro tak;

        internal void setModules(Takehiro tk, Reservoir rv, PsyModel psy)
        {
            tak = tk;
            this.rv = rv;
            this.psy = psy;
        }

        internal float POW20(int x)
        {
            Debug.Assert(0 <= x + Q_MAX2 && x < Q_MAX);
            return pow20[x + Q_MAX2];
        }

        internal float IPOW20(int x)
        {
            Debug.Assert(0 <= x && x < Q_MAX);
            return ipow20[x];
        }

        private float ATHmdct(LameGlobalFlags gfp, float f)
        {
            var ath = psy.ATHformula(f, gfp);
            ath -= NSATHSCALE;
            ath = (float)Math.Pow(10.0, ath / 10.0 + gfp.ATHlower);
            return ath;
        }

        private void compute_ath(LameGlobalFlags gfp)
        {

            var ATH_l = gfp.internal_flags.ATH.l;

            var ATH_psfb21 = gfp.internal_flags.ATH.psfb21;

            var ATH_s = gfp.internal_flags.ATH.s;

            var ATH_psfb12 = gfp.internal_flags.ATH.psfb12;

            var gfc = gfp.internal_flags;

            float samp_freq = gfp.out_samplerate;
            for (var sfb = 0; sfb < Encoder.SBMAX_l; sfb++)
            {
                var start = gfc.scalefac_band.l[sfb];
                var end = gfc.scalefac_band.l[sfb + 1];
                ATH_l[sfb] = float.MaxValue;
                for (var i = start; i < end; i++)
                {

                    var freq = i * samp_freq / (2 * 576);
                    var ATH_f = ATHmdct(gfp, freq);
                    ATH_l[sfb] = Math.Min(ATH_l[sfb], ATH_f);
                }
            }

            for (var sfb = 0; sfb < Encoder.PSFB21; sfb++)
            {
                var start = gfc.scalefac_band.psfb21[sfb];
                var end = gfc.scalefac_band.psfb21[sfb + 1];
                ATH_psfb21[sfb] = float.MaxValue;
                for (var i = start; i < end; i++)
                {

                    var freq = i * samp_freq / (2 * 576);
                    var ATH_f = ATHmdct(gfp, freq);
                    ATH_psfb21[sfb] = Math.Min(ATH_psfb21[sfb], ATH_f);
                }
            }

            for (var sfb = 0; sfb < Encoder.SBMAX_s; sfb++)
            {
                var start = gfc.scalefac_band.s[sfb];
                var end = gfc.scalefac_band.s[sfb + 1];
                ATH_s[sfb] = float.MaxValue;
                for (var i = start; i < end; i++)
                {

                    var freq = i * samp_freq / (2 * 192);
                    var ATH_f = ATHmdct(gfp, freq);
                    ATH_s[sfb] = Math.Min(ATH_s[sfb], ATH_f);
                }

                ATH_s[sfb] *= gfc.scalefac_band.s[sfb + 1] - gfc.scalefac_band.s[sfb];
            }

            for (var sfb = 0; sfb < Encoder.PSFB12; sfb++)
            {
                var start = gfc.scalefac_band.psfb12[sfb];
                var end = gfc.scalefac_band.psfb12[sfb + 1];
                ATH_psfb12[sfb] = float.MaxValue;
                for (var i = start; i < end; i++)
                {

                    var freq = i * samp_freq / (2 * 192);
                    var ATH_f = ATHmdct(gfp, freq);
                    ATH_psfb12[sfb] = Math.Min(ATH_psfb12[sfb], ATH_f);
                }

                ATH_psfb12[sfb] *= gfc.scalefac_band.s[13] - gfc.scalefac_band.s[12];
            }

            if (gfp.noATH)
            {
                for (var sfb = 0; sfb < Encoder.SBMAX_l; sfb++)
                    ATH_l[sfb] = 1E-20f;

                for (var sfb = 0; sfb < Encoder.PSFB21; sfb++)
                    ATH_psfb21[sfb] = 1E-20f;

                for (var sfb = 0; sfb < Encoder.SBMAX_s; sfb++)
                    ATH_s[sfb] = 1E-20f;

                for (var sfb = 0; sfb < Encoder.PSFB12; sfb++)
                    ATH_psfb12[sfb] = 1E-20f;
            }

            gfc.ATH.floor = 10.0f * (float)Math.Log10(ATHmdct(gfp, -1.0f));
        }

        internal void iteration_init(LameGlobalFlags gfp)
        {

            var gfc = gfp.internal_flags;

            var l3_side = gfc.l3_side;
            int i;
            if (gfc.iteration_init_init == 0)
            {
                gfc.iteration_init_init = 1;
                l3_side.main_data_begin = 0;
                compute_ath(gfp);
                pow43[0] = 0.0f;
                for (i = 1; i < PRECALC_SIZE; i++)
                    pow43[i] = (float)Math.Pow(i, 4.0 / 3.0);

                for (i = 0; i < PRECALC_SIZE - 1; i++)
                    adj43[i] = (float)(i + 1 - Math.Pow(0.5 * (pow43[i] + pow43[i + 1]), 0.75));

                adj43[i] = 0.5f;
                for (i = 0; i < Q_MAX; i++)
                    ipow20[i] = (float)Math.Pow(2.0, (i - 210) * -0.1875);

                for (i = 0; i <= Q_MAX + Q_MAX2; i++)
                    pow20[i] = (float)Math.Pow(2.0, (i - 210 - Q_MAX2) * 0.25);

                tak.huffman_init(gfc);
                {
                    float bass, alto, treble, sfb21;
                    i = (gfp.exp_nspsytune >> 2) & 63;
                    if (i >= 32)
                        i -= 64;

                    bass = (float)Math.Pow(10, i / 4.0 / 10.0);
                    i = (gfp.exp_nspsytune >> 8) & 63;
                    if (i >= 32)
                        i -= 64;

                    alto = (float)Math.Pow(10, i / 4.0 / 10.0);
                    i = (gfp.exp_nspsytune >> 14) & 63;
                    if (i >= 32)
                        i -= 64;

                    treble = (float)Math.Pow(10, i / 4.0 / 10.0);
                    i = (gfp.exp_nspsytune >> 20) & 63;
                    if (i >= 32)
                        i -= 64;

                    sfb21 = treble * (float)Math.Pow(10, i / 4.0 / 10.0);
                    for (i = 0; i < Encoder.SBMAX_l; i++)
                    {
                        float f;
                        if (i <= 6)
                            f = bass;
                        else if (i <= 13)
                            f = alto;
                        else if (i <= 20)
                            f = treble;
                        else
                            f = sfb21;

                        gfc.nsPsy.longfact[i] = f;
                    }

                    for (i = 0; i < Encoder.SBMAX_s; i++)
                    {
                        float f;
                        if (i <= 5)
                            f = bass;
                        else if (i <= 10)
                            f = alto;
                        else if (i <= 11)
                            f = treble;
                        else
                            f = sfb21;

                        gfc.nsPsy.shortfact[i] = f;
                    }
                }
            }
        }

        internal int on_pe(LameGlobalFlags gfp, float[][] pe, int[] targ_bits, int mean_bits, int gr, int cbr)
        {

            var gfc = gfp.internal_flags;
            int tbits = 0, bits;
            var add_bits = new int[2];
            int ch;
            var mb = new MeanBits(tbits);
            var extra_bits = rv.ResvMaxBits(gfp, mean_bits, mb, cbr);
            tbits = mb.bits;
            var max_bits = tbits + extra_bits;
            if (max_bits > LameInternalFlags.MAX_BITS_PER_GRANULE)
                max_bits = LameInternalFlags.MAX_BITS_PER_GRANULE;

            for (bits = 0, ch = 0; ch < gfc.channels_out; ++ch)
            {
                targ_bits[ch] = Math.Min(LameInternalFlags.MAX_BITS_PER_CHANNEL, tbits / gfc.channels_out);
                add_bits[ch] = (int)(targ_bits[ch] * pe[gr][ch] / 700.0 - targ_bits[ch]);
                if (add_bits[ch] > mean_bits * 3 / 4)
                    add_bits[ch] = mean_bits * 3 / 4;

                if (add_bits[ch] < 0)
                    add_bits[ch] = 0;

                if (add_bits[ch] + targ_bits[ch] > LameInternalFlags.MAX_BITS_PER_CHANNEL)
                    add_bits[ch] = Math.Max(0, LameInternalFlags.MAX_BITS_PER_CHANNEL - targ_bits[ch]);

                bits += add_bits[ch];
            }

            if (bits > extra_bits)
                for (ch = 0; ch < gfc.channels_out; ++ch)
                    add_bits[ch] = extra_bits * add_bits[ch] / bits;

            for (ch = 0; ch < gfc.channels_out; ++ch)
            {
                targ_bits[ch] += add_bits[ch];
                extra_bits -= add_bits[ch];
            }

            for (bits = 0, ch = 0; ch < gfc.channels_out; ++ch)
                bits += targ_bits[ch];

            if (bits > LameInternalFlags.MAX_BITS_PER_GRANULE)
            {
                var sum = 0;
                for (ch = 0; ch < gfc.channels_out; ++ch)
                {
                    targ_bits[ch] *= LameInternalFlags.MAX_BITS_PER_GRANULE;
                    targ_bits[ch] /= bits;
                    sum += targ_bits[ch];
                }

                Debug.Assert(sum <= LameInternalFlags.MAX_BITS_PER_GRANULE);
            }

            return max_bits;
        }

        internal void reduce_side(int[] targ_bits, float ms_ener_ratio, int mean_bits, int max_bits)
        {
            Debug.Assert(max_bits <= LameInternalFlags.MAX_BITS_PER_GRANULE);
            Debug.Assert(targ_bits[0] + targ_bits[1] <= LameInternalFlags.MAX_BITS_PER_GRANULE);
            var fac = .33f * (.5f - ms_ener_ratio) / .5f;
            if (fac < 0)
                fac = 0;

            if (fac > .5)
                fac = .5f;

            var move_bits = (int)(fac * .5 * (targ_bits[0] + targ_bits[1]));
            if (move_bits > LameInternalFlags.MAX_BITS_PER_CHANNEL - targ_bits[0])
                move_bits = LameInternalFlags.MAX_BITS_PER_CHANNEL - targ_bits[0];

            if (move_bits < 0)
                move_bits = 0;

            if (targ_bits[1] >= 125)
                if (targ_bits[1] - move_bits > 125)
                {
                    if (targ_bits[0] < mean_bits)
                        targ_bits[0] += move_bits;

                    targ_bits[1] -= move_bits;
                }
                else
                {
                    targ_bits[0] += targ_bits[1] - 125;
                    targ_bits[1] = 125;
                }

            move_bits = targ_bits[0] + targ_bits[1];
            if (move_bits > max_bits)
            {
                targ_bits[0] = max_bits * targ_bits[0] / move_bits;
                targ_bits[1] = max_bits * targ_bits[1] / move_bits;
            }

            Debug.Assert(targ_bits[0] <= LameInternalFlags.MAX_BITS_PER_CHANNEL);
            Debug.Assert(targ_bits[1] <= LameInternalFlags.MAX_BITS_PER_CHANNEL);
            Debug.Assert(targ_bits[0] + targ_bits[1] <= LameInternalFlags.MAX_BITS_PER_GRANULE);
        }

        internal float athAdjust(float a, float x, float athFloor)
        {
            const float o = 90.30873362f;
            const float p = 94.82444863f;
            var u = Util.FAST_LOG10_X(x, 10.0f);

            var v = a * a;
            var w = 0.0f;
            u -= athFloor;
            if (v > 1E-20)
                w = 1.0f + Util.FAST_LOG10_X(v, 10.0f / o);

            if (w < 0)
                w = 0.0f;

            u *= w;
            u += athFloor + o - p;
            return (float)Math.Pow(10.0, 0.1 * u);
        }

        internal int calc_xmin(LameGlobalFlags gfp, III_psy_ratio ratio, GrInfo cod_info, float[] pxmin)
        {
            var pxminPos = 0;

            var gfc = gfp.internal_flags;
            int gsfb, j = 0, ath_over = 0;

            var ATH = gfc.ATH;

            var xr = cod_info.xr;

            var enable_athaa_fix = gfp.VBR == VbrMode.vbr_mtrh ? 1 : 0;
            var masking_lower = gfc.masking_lower;
            if (gfp.VBR == VbrMode.vbr_mtrh || gfp.VBR == VbrMode.vbr_mt)
                masking_lower = 1.0f;

            for (gsfb = 0; gsfb < cod_info.psy_lmax; gsfb++)
            {
                float en0, xmin;
                float rh1, rh2;
                int width, l;
                if (gfp.VBR == VbrMode.vbr_rh || gfp.VBR == VbrMode.vbr_mtrh)
                    xmin = athAdjust(ATH.adjust, ATH.l[gsfb], ATH.floor);
                else
                    xmin = ATH.adjust * ATH.l[gsfb];

                width = cod_info.width[gsfb];
                rh1 = xmin / width;
                rh2 = DBL_EPSILON;
                l = width >> 1;
                en0 = 0.0f;
                do
                {
                    float xa, xb;
                    xa = xr[j] * xr[j];
                    en0 += xa;
                    rh2 += xa < rh1 ? xa : rh1;
                    j++;
                    xb = xr[j] * xr[j];
                    en0 += xb;
                    rh2 += xb < rh1 ? xb : rh1;
                    j++;
                }
                while (--l > 0);

                if (en0 > xmin)
                    ath_over++;

                if (gsfb == Encoder.SBPSY_l)
                {
                    var x = xmin * gfc.nsPsy.longfact[gsfb];
                    if (rh2 < x)
                        rh2 = x;
                }

                if (enable_athaa_fix != 0)
                    xmin = rh2;

                if (!gfp.ATHonly)
                {

                    var e = ratio.en.l[gsfb];
                    if (e > 0.0f)
                    {
                        float x;
                        x = en0 * ratio.thm.l[gsfb] * masking_lower / e;
                        if (enable_athaa_fix != 0)
                            x *= gfc.nsPsy.longfact[gsfb];

                        if (xmin < x)
                            xmin = x;
                    }
                }

                if (enable_athaa_fix != 0)
                    pxmin[pxminPos++] = xmin;
                else
                    pxmin[pxminPos++] = xmin * gfc.nsPsy.longfact[gsfb];
            }

            var max_nonzero = 575;
            if (cod_info.block_type != Encoder.SHORT_TYPE)
            {
                var k = 576;
                while (k-- != 0 && BitStream.EQ(xr[k], 0))
                    max_nonzero = k;
            }

            cod_info.max_nonzero_coeff = max_nonzero;
            for (var sfb = cod_info.sfb_smin; gsfb < cod_info.psymax; sfb++, gsfb += 3)
            {
                int width, b;
                float tmpATH;
                if (gfp.VBR == VbrMode.vbr_rh || gfp.VBR == VbrMode.vbr_mtrh)
                    tmpATH = athAdjust(ATH.adjust, ATH.s[sfb], ATH.floor);
                else
                    tmpATH = ATH.adjust * ATH.s[sfb];

                width = cod_info.width[gsfb];
                for (b = 0; b < 3; b++)
                {
                    float en0 = 0.0f, xmin;
                    float rh1, rh2;
                    var l = width >> 1;
                    rh1 = tmpATH / width;
                    rh2 = DBL_EPSILON;
                    do
                    {
                        float xa, xb;
                        xa = xr[j] * xr[j];
                        en0 += xa;
                        rh2 += xa < rh1 ? xa : rh1;
                        j++;
                        xb = xr[j] * xr[j];
                        en0 += xb;
                        rh2 += xb < rh1 ? xb : rh1;
                        j++;
                    }
                    while (--l > 0);

                    if (en0 > tmpATH)
                        ath_over++;

                    if (sfb == Encoder.SBPSY_s)
                    {
                        var x = tmpATH * gfc.nsPsy.shortfact[sfb];
                        if (rh2 < x)
                            rh2 = x;
                    }

                    if (enable_athaa_fix != 0)
                        xmin = rh2;
                    else
                        xmin = tmpATH;

                    if (!gfp.ATHonly && !gfp.ATHshort)
                    {

                        var e = ratio.en.s[sfb][b];
                        if (e > 0.0f)
                        {
                            float x;
                            x = en0 * ratio.thm.s[sfb][b] * masking_lower / e;
                            if (enable_athaa_fix != 0)
                                x *= gfc.nsPsy.shortfact[sfb];

                            if (xmin < x)
                                xmin = x;
                        }
                    }

                    if (enable_athaa_fix != 0)
                        pxmin[pxminPos++] = xmin;
                    else
                        pxmin[pxminPos++] = xmin * gfc.nsPsy.shortfact[sfb];
                }

                if (gfp.useTemporal == true)
                {
                    if (pxmin[pxminPos - 3] > pxmin[pxminPos - 3 + 1])
                        pxmin[pxminPos - 3 + 1] += (pxmin[pxminPos - 3] - pxmin[pxminPos - 3 + 1]) * gfc.decay;

                    if (pxmin[pxminPos - 3 + 1] > pxmin[pxminPos - 3 + 2])
                        pxmin[pxminPos - 3 + 2] += (pxmin[pxminPos - 3 + 1] - pxmin[pxminPos - 3 + 2]) * gfc.decay;
                }
            }

            return ath_over;
        }

        private float calc_noise_core(GrInfo cod_info, StartLine startline, int l, float step)
        {
            float noise = 0;
            var j = startline.s;

            var ix = cod_info.l3_enc;
            if (j > cod_info.count1)
            {
                while (l-- != 0)
                {
                    float temp;
                    temp = cod_info.xr[j];
                    j++;
                    noise += temp * temp;
                    temp = cod_info.xr[j];
                    j++;
                    noise += temp * temp;
                }
            }
            else if (j > cod_info.big_values)
            {
                var ix01 = new float[2];
                ix01[0] = 0;
                ix01[1] = step;
                while (l-- != 0)
                {
                    float temp;
                    temp = Math.Abs(cod_info.xr[j]) - ix01[ix[j]];
                    j++;
                    noise += temp * temp;
                    temp = Math.Abs(cod_info.xr[j]) - ix01[ix[j]];
                    j++;
                    noise += temp * temp;
                }
            }
            else
            {
                while (l-- != 0)
                {
                    float temp;
                    temp = Math.Abs(cod_info.xr[j]) - pow43[ix[j]] * step;
                    j++;
                    noise += temp * temp;
                    temp = Math.Abs(cod_info.xr[j]) - pow43[ix[j]] * step;
                    j++;
                    noise += temp * temp;
                }
            }

            startline.s = j;
            return noise;
        }

        internal int calc_noise(
            GrInfo cod_info,
            float[] l3_xmin,
            float[] distort,
            CalcNoiseResult res,
            CalcNoiseData prev_noise)
        {
            var distortPos = 0;
            var l3_xminPos = 0;
            int sfb, l, over = 0;
            float over_noise_db = 0;
            float tot_noise_db = 0;
            var max_noise = -20.0f;
            var j = 0;

            var scalefac = cod_info.scalefac;
            var scalefacPos = 0;
            res.over_SSD = 0;
            for (sfb = 0; sfb < cod_info.psymax; sfb++)
            {

                var s = cod_info.global_gain -
                        ((scalefac[scalefacPos++] + (cod_info.preflag != 0 ? pretab[sfb] : 0)) <<
                         (cod_info.scalefac_scale + 1)) - cod_info.subblock_gain[cod_info.window[sfb]] * 8;
                var noise = 0.0f;
                if (prev_noise != null && prev_noise.step[sfb] == s)
                {
                    noise = prev_noise.noise[sfb];
                    j += cod_info.width[sfb];
                    distort[distortPos++] = noise / l3_xmin[l3_xminPos++];
                    noise = prev_noise.noise_log[sfb];
                }
                else
                {

                    var step = POW20(s);
                    l = cod_info.width[sfb] >> 1;
                    if (j + cod_info.width[sfb] > cod_info.max_nonzero_coeff)
                    {
                        int usefullsize;
                        usefullsize = cod_info.max_nonzero_coeff - j + 1;
                        if (usefullsize > 0)
                            l = usefullsize >> 1;
                        else
                            l = 0;
                    }

                    var sl = new StartLine(j);
                    noise = calc_noise_core(cod_info, sl, l, step);
                    j = sl.s;
                    if (prev_noise != null)
                    {
                        prev_noise.step[sfb] = s;
                        prev_noise.noise[sfb] = noise;
                    }

                    noise = distort[distortPos++] = noise / l3_xmin[l3_xminPos++];
                    noise = Util.FAST_LOG10((float)Math.Max(noise, 1E-20));
                    if (prev_noise != null)
                        prev_noise.noise_log[sfb] = noise;
                }

                if (prev_noise != null)
                    prev_noise.global_gain = cod_info.global_gain;

                tot_noise_db += noise;
                if (noise > 0.0)
                {
                    int tmp;
                    tmp = Math.Max((int)(noise * 10 + .5), 1);
                    res.over_SSD += tmp * tmp;
                    over++;
                    over_noise_db += noise;
                }

                max_noise = Math.Max(max_noise, noise);
            }

            res.over_count = over;
            res.tot_noise = tot_noise_db;
            res.over_noise = over_noise_db;
            res.max_noise = max_noise;
            return over;
        }

        private void set_pinfo(LameGlobalFlags gfp, GrInfo cod_info, III_psy_ratio ratio, int gr, int ch)
        {

            var gfc = gfp.internal_flags;
            int sfb, sfb2;
            int l;
            float en0, en1;
            var ifqstep = cod_info.scalefac_scale == 0 ? .5f : 1.0f;
            var scalefac = cod_info.scalefac;
            var l3_xmin = new float[L3Side.SFBMAX];
            var xfsf = new float[L3Side.SFBMAX];
            var noise = new CalcNoiseResult();
            calc_xmin(gfp, ratio, cod_info, l3_xmin);
            calc_noise(cod_info, l3_xmin, xfsf, noise, null);
            var j = 0;
            sfb2 = cod_info.sfb_lmax;
            if (cod_info.block_type != Encoder.SHORT_TYPE && 0 == cod_info.mixed_block_flag)
                sfb2 = 22;

            for (sfb = 0; sfb < sfb2; sfb++)
            {
                var start = gfc.scalefac_band.l[sfb];
                var end = gfc.scalefac_band.l[sfb + 1];
                var bw = end - start;
                for (en0 = 0.0f; j < end; j++)
                    en0 += cod_info.xr[j] * cod_info.xr[j];

                en0 /= bw;
                en1 = 1e15f;
                gfc.pinfo.en[gr][ch][sfb] = en1 * en0;
                gfc.pinfo.xfsf[gr][ch][sfb] = en1 * l3_xmin[sfb] * xfsf[sfb] / bw;
                if (ratio.en.l[sfb] > 0 && !gfp.ATHonly)
                    en0 = en0 / ratio.en.l[sfb];
                else
                    en0 = 0.0f;

                gfc.pinfo.thr[gr][ch][sfb] = en1 * Math.Max(en0 * ratio.thm.l[sfb], gfc.ATH.l[sfb]);
                gfc.pinfo.LAMEsfb[gr][ch][sfb] = 0;
                if (cod_info.preflag != 0 && sfb >= 11)
                    gfc.pinfo.LAMEsfb[gr][ch][sfb] = -ifqstep * pretab[sfb];

                if (sfb < Encoder.SBPSY_l)
                {
                    Debug.Assert(scalefac[sfb] >= 0);
                    gfc.pinfo.LAMEsfb[gr][ch][sfb] -= ifqstep * scalefac[sfb];
                }
            }

            if (cod_info.block_type == Encoder.SHORT_TYPE)
            {
                sfb2 = sfb;
                for (sfb = cod_info.sfb_smin; sfb < Encoder.SBMAX_s; sfb++)
                {
                    var start = gfc.scalefac_band.s[sfb];
                    var end = gfc.scalefac_band.s[sfb + 1];
                    var bw = end - start;
                    for (var i = 0; i < 3; i++)
                    {
                        for (en0 = 0.0f, l = start; l < end; l++)
                        {
                            en0 += cod_info.xr[j] * cod_info.xr[j];
                            j++;
                        }

                        en0 = (float)Math.Max(en0 / bw, 1e-20);
                        en1 = 1e15f;
                        gfc.pinfo.en_s[gr][ch][3 * sfb + i] = en1 * en0;
                        gfc.pinfo.xfsf_s[gr][ch][3 * sfb + i] = en1 * l3_xmin[sfb2] * xfsf[sfb2] / bw;
                        if (ratio.en.s[sfb][i] > 0)
                            en0 = en0 / ratio.en.s[sfb][i];
                        else
                            en0 = 0.0f;

                        if (gfp.ATHonly || gfp.ATHshort)
                            en0 = 0;

                        gfc.pinfo.thr_s[gr][ch][3 * sfb + i] =
                            en1 * Math.Max(en0 * ratio.thm.s[sfb][i], gfc.ATH.s[sfb]);
                        gfc.pinfo.LAMEsfb_s[gr][ch][3 * sfb + i] = -2.0 * cod_info.subblock_gain[i];
                        if (sfb < Encoder.SBPSY_s)
                            gfc.pinfo.LAMEsfb_s[gr][ch][3 * sfb + i] -= ifqstep * scalefac[sfb2];

                        sfb2++;
                    }
                }
            }

            gfc.pinfo.LAMEqss[gr][ch] = cod_info.global_gain;
            gfc.pinfo.LAMEmainbits[gr][ch] = cod_info.part2_3_length + cod_info.part2_length;
            gfc.pinfo.LAMEsfbits[gr][ch] = cod_info.part2_length;
            gfc.pinfo.over[gr][ch] = noise.over_count;
            gfc.pinfo.max_noise[gr][ch] = noise.max_noise * 10.0;
            gfc.pinfo.over_noise[gr][ch] = noise.over_noise * 10.0;
            gfc.pinfo.tot_noise[gr][ch] = noise.tot_noise * 10.0;
            gfc.pinfo.over_SSD[gr][ch] = noise.over_SSD;
        }

        internal void set_frame_pinfo(LameGlobalFlags gfp, III_psy_ratio[][] ratio)
        {

            var gfc = gfp.internal_flags;
            gfc.masking_lower = 1.0f;
            for (var gr = 0; gr < gfc.mode_gr; gr++)
            for (var ch = 0; ch < gfc.channels_out; ch++)
            {
                var cod_info = gfc.l3_side.tt[gr][ch];
                var scalefac_sav = new int[L3Side.SFBMAX];
                Array.Copy(cod_info.scalefac, 0, scalefac_sav, 0, scalefac_sav.Length);
                if (gr == 1)
                {
                    int sfb;
                    for (sfb = 0; sfb < cod_info.sfb_lmax; sfb++)
                        if (cod_info.scalefac[sfb] < 0)
                            cod_info.scalefac[sfb] = gfc.l3_side.tt[0][ch].scalefac[sfb];
                }

                set_pinfo(gfp, cod_info, ratio[gr][ch], gr, ch);
                Array.Copy(scalefac_sav, 0, cod_info.scalefac, 0, scalefac_sav.Length);
            }
        }
    }
}