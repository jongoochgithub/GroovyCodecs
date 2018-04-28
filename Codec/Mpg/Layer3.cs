using System;
using GroovyMp3.Types;

/* layer3.c: Mpeg Layer-3 audio decoder
 *
 * Copyright (C) 1999-2010 The L.A.M.E. project
 *
 * Initially written by Michael Hipp, see also AUTHORS and README.
 *
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


/* $Id: Layer3.java,v 1.19 2011/06/17 05:26:42 kenchis Exp $ */
namespace GroovyMp3.Codec.Mpg
{

    internal class Layer3
    {

        private class bandInfoStruct
        {

            internal readonly short[] longDiff = new short[22];

            internal readonly short[] longIdx = new short[23];

            internal readonly short[] shortDiff = new short[13];

            internal readonly short[] shortIdx = new short[14];

            internal bandInfoStruct(short[] lIdx, short[] lDiff, short[] sIdx, short[] sDiff)
            {
                longIdx = lIdx;
                longDiff = lDiff;
                shortIdx = sIdx;
                shortDiff = sDiff;
            }
        }

        private static readonly bandInfoStruct[] bandInfo =
        {
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    4,
                    4,
                    4,
                    4,
                    4,
                    4,
                    6,
                    6,
                    8,
                    8,
                    10,
                    12,
                    16,
                    20,
                    24,
                    28,
                    34,
                    42,
                    50,
                    54,
                    76,
                    158
                },
                new short[]
                {
                    0,
                    4 * 3,
                    8 * 3,
                    12 * 3,
                    16 * 3,
                    22 * 3,
                    30 * 3,
                    40 * 3,
                    52 * 3,
                    66 * 3,
                    84 * 3,
                    106 * 3,
                    136 * 3,
                    192 * 3
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    4,
                    6,
                    8,
                    10,
                    12,
                    14,
                    18,
                    22,
                    30,
                    56
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    4,
                    4,
                    4,
                    4,
                    4,
                    4,
                    6,
                    6,
                    6,
                    8,
                    10,
                    12,
                    16,
                    18,
                    22,
                    28,
                    34,
                    40,
                    46,
                    54,
                    54,
                    192
                },
                new short[]
                {
                    0,
                    4 * 3,
                    8 * 3,
                    12 * 3,
                    16 * 3,
                    22 * 3,
                    28 * 3,
                    38 * 3,
                    50 * 3,
                    64 * 3,
                    80 * 3,
                    100 * 3,
                    126 * 3,
                    192 * 3
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    4,
                    6,
                    6,
                    10,
                    12,
                    14,
                    16,
                    20,
                    26,
                    66
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    4,
                    4,
                    4,
                    4,
                    4,
                    4,
                    6,
                    6,
                    8,
                    10,
                    12,
                    16,
                    20,
                    24,
                    30,
                    38,
                    46,
                    56,
                    68,
                    84,
                    102,
                    26
                },
                new short[]
                {
                    0,
                    4 * 3,
                    8 * 3,
                    12 * 3,
                    16 * 3,
                    22 * 3,
                    30 * 3,
                    42 * 3,
                    58 * 3,
                    78 * 3,
                    104 * 3,
                    138 * 3,
                    180 * 3,
                    192 * 3
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    4,
                    6,
                    8,
                    12,
                    16,
                    20,
                    26,
                    34,
                    42,
                    12
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    6,
                    6,
                    6,
                    6,
                    6,
                    6,
                    8,
                    10,
                    12,
                    14,
                    16,
                    20,
                    24,
                    28,
                    32,
                    38,
                    46,
                    52,
                    60,
                    68,
                    58,
                    54
                },
                new short[]
                {
                    0,
                    4 * 3,
                    8 * 3,
                    12 * 3,
                    18 * 3,
                    24 * 3,
                    32 * 3,
                    42 * 3,
                    56 * 3,
                    74 * 3,
                    100 * 3,
                    132 * 3,
                    174 * 3,
                    192 * 3
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    6,
                    6,
                    8,
                    10,
                    14,
                    18,
                    26,
                    32,
                    42,
                    18
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    6,
                    6,
                    6,
                    6,
                    6,
                    6,
                    8,
                    10,
                    12,
                    14,
                    16,
                    18,
                    22,
                    26,
                    32,
                    38,
                    46,
                    54,
                    62,
                    70,
                    76,
                    36
                },
                new short[]
                {
                    0,
                    4 * 3,
                    8 * 3,
                    12 * 3,
                    18 * 3,
                    26 * 3,
                    36 * 3,
                    48 * 3,
                    62 * 3,
                    80 * 3,
                    104 * 3,
                    136 * 3,
                    180 * 3,
                    192 * 3
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    6,
                    8,
                    10,
                    12,
                    14,
                    18,
                    24,
                    32,
                    44,
                    12
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    6,
                    6,
                    6,
                    6,
                    6,
                    6,
                    8,
                    10,
                    12,
                    14,
                    16,
                    20,
                    24,
                    28,
                    32,
                    38,
                    46,
                    52,
                    60,
                    68,
                    58,
                    54
                },
                new short[]
                {
                    0,
                    4 * 3,
                    8 * 3,
                    12 * 3,
                    18 * 3,
                    26 * 3,
                    36 * 3,
                    48 * 3,
                    62 * 3,
                    80 * 3,
                    104 * 3,
                    134 * 3,
                    174 * 3,
                    192 * 3
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    6,
                    8,
                    10,
                    12,
                    14,
                    18,
                    24,
                    30,
                    40,
                    18
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    6,
                    6,
                    6,
                    6,
                    6,
                    6,
                    8,
                    10,
                    12,
                    14,
                    16,
                    20,
                    24,
                    28,
                    32,
                    38,
                    46,
                    52,
                    60,
                    68,
                    58,
                    54
                },
                new short[]
                {
                    0,
                    12,
                    24,
                    36,
                    54,
                    78,
                    108,
                    144,
                    186,
                    240,
                    312,
                    402,
                    522,
                    576
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    6,
                    8,
                    10,
                    12,
                    14,
                    18,
                    24,
                    30,
                    40,
                    18
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    6,
                    6,
                    6,
                    6,
                    6,
                    6,
                    8,
                    10,
                    12,
                    14,
                    16,
                    20,
                    24,
                    28,
                    32,
                    38,
                    46,
                    52,
                    60,
                    68,
                    58,
                    54
                },
                new short[]
                {
                    0,
                    12,
                    24,
                    36,
                    54,
                    78,
                    108,
                    144,
                    186,
                    240,
                    312,
                    402,
                    522,
                    576
                },
                new short[]
                {
                    4,
                    4,
                    4,
                    6,
                    8,
                    10,
                    12,
                    14,
                    18,
                    24,
                    30,
                    40,
                    18
                }),
            new bandInfoStruct(
                new short[]
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
                new short[]
                {
                    12,
                    12,
                    12,
                    12,
                    12,
                    12,
                    16,
                    20,
                    24,
                    28,
                    32,
                    40,
                    48,
                    56,
                    64,
                    76,
                    90,
                    2,
                    2,
                    2,
                    2,
                    2
                },
                new short[]
                {
                    0,
                    24,
                    48,
                    72,
                    108,
                    156,
                    216,
                    288,
                    372,
                    480,
                    486,
                    492,
                    498,
                    576
                },
                new short[]
                {
                    8,
                    8,
                    8,
                    12,
                    16,
                    20,
                    24,
                    28,
                    36,
                    2,
                    2,
                    2,
                    26
                })
        };

        private static readonly double[] Ci =
        {
            -0.6,
            -0.535,
            -0.33,
            -0.185,
            -0.095,
            -0.041,
            -0.0142,
            -0.0037
        };

        private static readonly int[] len =
        {
            36,
            36,
            12,
            36
        };

        private static readonly int[] pretab1 =
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

        private static readonly int[] pretab2 =
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
            0
        };

        private static readonly int[][] slen =
        {
            new[]
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
            },
            new[]
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
            }
        };

        private static readonly int[][][] stab =
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
                    6,
                    5,
                    7,
                    3
                },
                new[]
                {
                    11,
                    10,
                    0,
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
                    6,
                    6,
                    6,
                    3
                },
                new[]
                {
                    8,
                    8,
                    5,
                    0
                }
            },
            new[]
            {
                new[]
                {
                    9,
                    9,
                    9,
                    9
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
                    18,
                    18,
                    0,
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
                    12,
                    9,
                    9,
                    6
                },
                new[]
                {
                    15,
                    12,
                    9,
                    0
                }
            },
            new[]
            {
                new[]
                {
                    6,
                    9,
                    9,
                    9
                },
                new[]
                {
                    6,
                    9,
                    12,
                    6
                },
                new[]
                {
                    15,
                    18,
                    0,
                    0
                },
                new[]
                {
                    6,
                    15,
                    12,
                    0
                },
                new[]
                {
                    6,
                    12,
                    9,
                    6
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

        private readonly float[] aa_ca = new float[8];

        private readonly float[] aa_cs = new float[8];

        private Common common;

        private readonly float[][] COS1 = Arrays.ReturnRectangularArray<float>(12, 6);

        private float COS6_1, COS6_2;

        private readonly float[] COS9 = new float[9];

        private readonly float[] gainpow2 = new float[256 + 118 + 4];

        private readonly float[][] hybridIn =
            Arrays.ReturnRectangularArray<float>(2, MPG123.SBLIMIT * MPG123.SSLIMIT);

        private readonly float[][] hybridOut =
            Arrays.ReturnRectangularArray<float>(2, MPG123.SSLIMIT * MPG123.SBLIMIT);

        private readonly int[] i_slen2 = new int[256];

        private readonly float[] ispow = new float[8207];

        private readonly int[][] longLimit = Arrays.ReturnRectangularArray<int>(9, 23);

        private readonly int[][][] map = Arrays.ReturnRectangularArray<int>(9, 3, -1);

        private readonly int[][] mapbuf0 = Arrays.ReturnRectangularArray<int>(9, 152);

        private readonly int[][] mapbuf1 = Arrays.ReturnRectangularArray<int>(9, 156);

        private readonly int[][] mapbuf2 = Arrays.ReturnRectangularArray<int>(9, 44);

        private readonly int[][] mapend = Arrays.ReturnRectangularArray<int>(9, 3);

        private readonly int[] n_slen2 = new int[512];

        private readonly float[][] pow1_1 = Arrays.ReturnRectangularArray<float>(2, 16);

        private readonly float[][] pow1_2 = Arrays.ReturnRectangularArray<float>(2, 16);

        private readonly float[][] pow2_1 = Arrays.ReturnRectangularArray<float>(2, 16);

        private readonly float[][] pow2_2 = Arrays.ReturnRectangularArray<float>(2, 16);

        private readonly int[][] shortLimit = Arrays.ReturnRectangularArray<int>(9, 14);

        private readonly MPG123.III_sideinfo sideinfo = new MPG123.III_sideinfo();

        private readonly float[] tan1_1 = new float[16];

        private readonly float[] tan1_2 = new float[16];

        private readonly float[] tan2_1 = new float[16];

        private readonly float[] tan2_2 = new float[16];

        private readonly float[] tfcos12 = new float[3];

        private readonly float[] tfcos36 = new float[9];

        private readonly float[][] win = Arrays.ReturnRectangularArray<float>(4, 36);

        private readonly float[][] win1 = Arrays.ReturnRectangularArray<float>(4, 36);

        internal virtual Common Modules
        {
            set => common = value;
        }

        private int get1bit(MPGLib.mpstr_tag mp)
        {
            var rval = (mp.wordpointer[mp.wordpointerPos] & 0xff) << mp.bitindex;
            rval &= 0xff;
            mp.bitindex++;
            mp.wordpointerPos += mp.bitindex >> 3;
            mp.bitindex &= 7;
            return rval >> 7;
        }

        internal virtual void init_layer3(int down_sample_sblimit)
        {
            for (var i = -256; i < 118 + 4; i++)
                gainpow2[i + 256] = (float)Math.Pow(2.0, -0.25 * (i + 210));

            for (var i = 0; i < 8207; i++)
                ispow[i] = (float)Math.Pow(i, 4.0 / 3.0);

            for (var i = 0; i < 8; i++)
            {
                var sq = Math.Sqrt(1.0 + Ci[i] * Ci[i]);
                aa_cs[i] = (float)(1.0 / sq);
                aa_ca[i] = (float)(Ci[i] / sq);
            }

            for (var i = 0; i < 18; i++)
            {
                win[0][i] = win[1][i] = (float)(0.5 * Math.Sin(MPG123.M_PI / 72.0 * (2 * (i + 0) + 1)) /
                                                Math.Cos(MPG123.M_PI * (2 * (i + 0) + 19) / 72.0));
                win[0][i + 18] = win[3][i + 18] =
                    (float)(0.5 * Math.Sin(MPG123.M_PI / 72.0 * (2 * (i + 18) + 1)) /
                            Math.Cos(MPG123.M_PI * (2 * (i + 18) + 19) / 72.0));
            }

            for (var i = 0; i < 6; i++)
            {
                win[1][i + 18] = (float)(0.5 / Math.Cos(MPG123.M_PI * (2 * (i + 18) + 19) / 72.0));
                win[3][i + 12] = (float)(0.5 / Math.Cos(MPG123.M_PI * (2 * (i + 12) + 19) / 72.0));
                win[1][i + 24] = (float)(0.5 * Math.Sin(MPG123.M_PI / 24.0 * (2 * i + 13)) /
                                         Math.Cos(MPG123.M_PI * (2 * (i + 24) + 19) / 72.0));
                win[1][i + 30] = win[3][i] = 0.0f;
                win[3][i + 6] = (float)(0.5 * Math.Sin(MPG123.M_PI / 24.0 * (2 * i + 1)) /
                                        Math.Cos(MPG123.M_PI * (2 * (i + 6) + 19) / 72.0));
            }

            for (var i = 0; i < 9; i++)
                COS9[i] = (float)Math.Cos(MPG123.M_PI / 18.0 * i);

            for (var i = 0; i < 9; i++)
                tfcos36[i] = (float)(0.5 / Math.Cos(MPG123.M_PI * (i * 2 + 1) / 36.0));

            for (var i = 0; i < 3; i++)
                tfcos12[i] = (float)(0.5 / Math.Cos(MPG123.M_PI * (i * 2 + 1) / 12.0));

            COS6_1 = (float)Math.Cos(MPG123.M_PI / 6.0 * 1);
            COS6_2 = (float)Math.Cos(MPG123.M_PI / 6.0 * 2);
            for (var i = 0; i < 12; i++)
            {
                win[2][i] = (float)(0.5 * Math.Sin(MPG123.M_PI / 24.0 * (2 * i + 1)) /
                                    Math.Cos(MPG123.M_PI * (2 * i + 7) / 24.0));
                for (var j = 0; j < 6; j++)
                    COS1[i][j] = (float)Math.Cos(MPG123.M_PI / 24.0 * ((2 * i + 7) * (2 * j + 1)));
            }

            for (var j = 0; j < 4; j++)
            {
                for (var i = 0; i < len[j]; i += 2)
                    win1[j][i] = +win[j][i];

                for (var i = 1; i < len[j]; i += 2)
                    win1[j][i] = -win[j][i];
            }

            for (var i = 0; i < 16; i++)
            {
                var t = Math.Tan(i * MPG123.M_PI / 12.0);
                tan1_1[i] = (float)(t / (1.0 + t));
                tan2_1[i] = (float)(1.0 / (1.0 + t));
                tan1_2[i] = (float)(MPG123.M_SQRT2 * t / (1.0 + t));
                tan2_2[i] = (float)(MPG123.M_SQRT2 / (1.0 + t));
                for (var j = 0; j < 2; j++)
                {
                    var @base = Math.Pow(2.0, -0.25 * (j + 1.0));
                    double p1 = 1.0, p2 = 1.0;
                    if (i > 0)
                        if ((i & 1) != 0)
                            p1 = Math.Pow(@base, (i + 1.0) * 0.5);
                        else
                            p2 = Math.Pow(@base, i * 0.5);

                    pow1_1[j][i] = (float)p1;
                    pow2_1[j][i] = (float)p2;
                    pow1_2[j][i] = (float)(MPG123.M_SQRT2 * p1);
                    pow2_2[j][i] = (float)(MPG123.M_SQRT2 * p2);
                }
            }

            for (var j = 0; j < 9; j++)
            {

                var bi = bandInfo[j];
                int mp;
                int cb, lwin;
                int bdf;
                map[j][0] = mapbuf0[j];
                mp = 0;
                bdf = 0;
                int i;
                for (i = 0, cb = 0; cb < 8; cb++, i += bi.longDiff[bdf++])
                {
                    map[j][0][mp++] = bi.longDiff[bdf] >> 1;
                    map[j][0][mp++] = i;
                    map[j][0][mp++] = 3;
                    map[j][0][mp++] = cb;
                }

                bdf = +3;
                for (cb = 3; cb < 13; cb++)
                {
                    var l = bi.shortDiff[bdf++] >> 1;
                    for (lwin = 0; lwin < 3; lwin++)
                    {
                        map[j][0][mp++] = l;
                        map[j][0][mp++] = i + lwin;
                        map[j][0][mp++] = lwin;
                        map[j][0][mp++] = cb;
                    }

                    i += 6 * l;
                }

                mapend[j][0] = mp;
                map[j][1] = mapbuf1[j];
                mp = 0;
                bdf = 0;
                for (i = 0, cb = 0; cb < 13; cb++)
                {
                    var l = bi.shortDiff[bdf++] >> 1;
                    for (lwin = 0; lwin < 3; lwin++)
                    {
                        map[j][1][mp++] = l;
                        map[j][1][mp++] = i + lwin;
                        map[j][1][mp++] = lwin;
                        map[j][1][mp++] = cb;
                    }

                    i += 6 * l;
                }

                mapend[j][1] = mp;
                map[j][2] = mapbuf2[j];
                mp = 0;
                bdf = 0;
                for (cb = 0; cb < 22; cb++)
                {
                    map[j][2][mp++] = bi.longDiff[bdf++] >> 1;
                    map[j][2][mp++] = cb;
                }

                mapend[j][2] = mp;
            }

            for (var j = 0; j < 9; j++)
            {
                for (var i = 0; i < 23; i++)
                {
                    longLimit[j][i] = (bandInfo[j].longIdx[i] - 1 + 8) / 18 + 1;
                    if (longLimit[j][i] > down_sample_sblimit)
                        longLimit[j][i] = down_sample_sblimit;
                }

                for (var i = 0; i < 14; i++)
                {
                    shortLimit[j][i] = (bandInfo[j].shortIdx[i] - 1) / 18 + 1;
                    if (shortLimit[j][i] > down_sample_sblimit)
                        shortLimit[j][i] = down_sample_sblimit;
                }
            }

            for (var i = 0; i < 5; i++)
            for (var j = 0; j < 6; j++)
            for (var k = 0; k < 6; k++)
            {
                var n = k + j * 6 + i * 36;
                i_slen2[n] = i | (j << 3) | (k << 6) | (3 << 12);
            }

            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            for (var k = 0; k < 4; k++)
            {
                var n = k + j * 4 + i * 16;
                i_slen2[n + 180] = i | (j << 3) | (k << 6) | (4 << 12);
            }

            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 3; j++)
            {
                var n = j + i * 3;
                i_slen2[n + 244] = i | (j << 3) | (5 << 12);
                n_slen2[n + 500] = i | (j << 3) | (2 << 12) | (1 << 15);
            }

            for (var i = 0; i < 5; i++)
            for (var j = 0; j < 5; j++)
            for (var k = 0; k < 4; k++)
            {
                int l;
                for (l = 0; l < 4; l++)
                {
                    var n = l + k * 4 + j * 16 + i * 80;
                    n_slen2[n] = i | (j << 3) | (k << 6) | (l << 9) | (0 << 12);
                }
            }

            for (var i = 0; i < 5; i++)
            for (var j = 0; j < 5; j++)
            for (var k = 0; k < 4; k++)
            {
                var n = k + j * 4 + i * 20;
                n_slen2[n + 400] = i | (j << 3) | (k << 6) | (1 << 12);
            }
        }

        private void III_get_side_info_1(
            MPGLib.mpstr_tag mp,
            MPG123.III_sideinfo si,
            int stereo,
            int ms_stereo,
            int sfreq,
            int single)
        {
            int ch, gr;
            var powdiff = single == 3 ? 4 : 0;
            si.main_data_begin = common.getbits(mp, 9);
            if (stereo == 1)
                si.private_bits = common.getbits_fast(mp, 5);
            else
                si.private_bits = common.getbits_fast(mp, 3);

            for (ch = 0; ch < stereo; ch++)
            {
                si.ch[ch].gr[0].scfsi = -1;
                si.ch[ch].gr[1].scfsi = common.getbits_fast(mp, 4);
            }

            for (gr = 0; gr < 2; gr++)
            for (ch = 0; ch < stereo; ch++)
            {
                var gr_infos = si.ch[ch].gr[gr];
                gr_infos.part2_3_length = common.getbits(mp, 12);
                gr_infos.big_values = common.getbits_fast(mp, 9);
                if (gr_infos.big_values > 288)
                {
                    Console.WriteLine("big_values too large! %d\n", gr_infos.big_values);
                    gr_infos.big_values = 288;
                }

                {
                    var qss = common.getbits_fast(mp, 8);
                    gr_infos.pow2gain = gainpow2;
                    gr_infos.pow2gainPos = 256 - qss + powdiff;
                    if (mp.pinfo != null)
                        mp.pinfo.qss[gr][ch] = qss;
                }
                if (ms_stereo != 0)
                    gr_infos.pow2gainPos += 2;

                gr_infos.scalefac_compress = common.getbits_fast(mp, 4);
                if (get1bit(mp) != 0)
                {
                    int i;
                    gr_infos.block_type = common.getbits_fast(mp, 2);
                    gr_infos.mixed_block_flag = get1bit(mp);
                    gr_infos.table_select[0] = common.getbits_fast(mp, 5);
                    gr_infos.table_select[1] = common.getbits_fast(mp, 5);
                    gr_infos.table_select[2] = 0;
                    for (i = 0; i < 3; i++)
                    {
                        var sbg = common.getbits_fast(mp, 3) << 3;
                        gr_infos.full_gain[i] = gr_infos.pow2gain;
                        gr_infos.full_gainPos[i] = gr_infos.pow2gainPos + sbg;
                        if (mp.pinfo != null)
                            mp.pinfo.sub_gain[gr][ch][i] = sbg / 8;
                    }

                    if (gr_infos.block_type == 0)
                        Console.WriteLine("Blocktype == 0 and window-switching == 1 not allowed.\n");

                    gr_infos.region1start = 36 >> 1;
                    gr_infos.region2start = 576 >> 1;
                }
                else
                {
                    int i, r0c, r1c;
                    for (i = 0; i < 3; i++)
                        gr_infos.table_select[i] = common.getbits_fast(mp, 5);

                    r0c = common.getbits_fast(mp, 4);
                    r1c = common.getbits_fast(mp, 3);
                    gr_infos.region1start = bandInfo[sfreq].longIdx[r0c + 1] >> 1;
                    gr_infos.region2start = bandInfo[sfreq].longIdx[r0c + 1 + r1c + 1] >> 1;
                    gr_infos.block_type = 0;
                    gr_infos.mixed_block_flag = 0;
                }

                gr_infos.preflag = get1bit(mp);
                gr_infos.scalefac_scale = get1bit(mp);
                gr_infos.count1table_select = get1bit(mp);
            }
        }

        private void III_get_side_info_2(
            MPGLib.mpstr_tag mp,
            MPG123.III_sideinfo si,
            int stereo,
            int ms_stereo,
            int sfreq,
            int single)
        {
            int ch;
            var powdiff = single == 3 ? 4 : 0;
            si.main_data_begin = common.getbits(mp, 8);
            if (stereo == 1)
                si.private_bits = get1bit(mp);
            else
                si.private_bits = common.getbits_fast(mp, 2);

            for (ch = 0; ch < stereo; ch++)
            {
                var gr_infos = si.ch[ch].gr[0];
                int qss;
                gr_infos.part2_3_length = common.getbits(mp, 12);
                gr_infos.big_values = common.getbits_fast(mp, 9);
                if (gr_infos.big_values > 288)
                {
                    Console.WriteLine("big_values too large! %d\n", gr_infos.big_values);
                    gr_infos.big_values = 288;
                }

                qss = common.getbits_fast(mp, 8);
                gr_infos.pow2gain = gainpow2;
                gr_infos.pow2gainPos = 256 - qss + powdiff;
                if (mp.pinfo != null)
                    mp.pinfo.qss[0][ch] = qss;

                if (ms_stereo != 0)
                    gr_infos.pow2gainPos += 2;

                gr_infos.scalefac_compress = common.getbits(mp, 9);
                if (get1bit(mp) != 0)
                {
                    int i;
                    gr_infos.block_type = common.getbits_fast(mp, 2);
                    gr_infos.mixed_block_flag = get1bit(mp);
                    gr_infos.table_select[0] = common.getbits_fast(mp, 5);
                    gr_infos.table_select[1] = common.getbits_fast(mp, 5);
                    gr_infos.table_select[2] = 0;
                    for (i = 0; i < 3; i++)
                    {
                        var sbg = common.getbits_fast(mp, 3) << 3;
                        gr_infos.full_gain[i] = gr_infos.pow2gain;
                        gr_infos.full_gainPos[i] = gr_infos.pow2gainPos + sbg;
                        if (mp.pinfo != null)
                            mp.pinfo.sub_gain[0][ch][i] = sbg / 8;
                    }

                    if (gr_infos.block_type == 0)
                        Console.WriteLine("Blocktype == 0 and window-switching == 1 not allowed.\n");

                    if (gr_infos.block_type == 2)
                        if (sfreq == 8)
                            gr_infos.region1start = 36;
                        else
                            gr_infos.region1start = 36 >> 1;
                    else if (sfreq == 8)
                        gr_infos.region1start = 108 >> 1;
                    else
                        gr_infos.region1start = 54 >> 1;

                    gr_infos.region2start = 576 >> 1;
                }
                else
                {
                    int i, r0c, r1c;
                    for (i = 0; i < 3; i++)
                        gr_infos.table_select[i] = common.getbits_fast(mp, 5);

                    r0c = common.getbits_fast(mp, 4);
                    r1c = common.getbits_fast(mp, 3);
                    gr_infos.region1start = bandInfo[sfreq].longIdx[r0c + 1] >> 1;
                    gr_infos.region2start = bandInfo[sfreq].longIdx[r0c + 1 + r1c + 1] >> 1;
                    gr_infos.block_type = 0;
                    gr_infos.mixed_block_flag = 0;
                }

                gr_infos.scalefac_scale = get1bit(mp);
                gr_infos.count1table_select = get1bit(mp);
            }
        }

        private int III_get_scale_factors_1(MPGLib.mpstr_tag mp, int[] scf, MPG123.gr_info_s gr_infos)
        {
            var scfPos = 0;
            int numbits;
            var num0 = slen[0][gr_infos.scalefac_compress];
            var num1 = slen[1][gr_infos.scalefac_compress];
            if (gr_infos.block_type == 2)
            {
                var i = 18;
                numbits = (num0 + num1) * 18;
                if (gr_infos.mixed_block_flag != 0)
                {
                    for (i = 8; i != 0; i--)
                        scf[scfPos++] = common.getbits_fast(mp, num0);

                    i = 9;
                    numbits -= num0;
                }

                for (; i != 0; i--)
                    scf[scfPos++] = common.getbits_fast(mp, num0);

                for (i = 18; i != 0; i--)
                    scf[scfPos++] = common.getbits_fast(mp, num1);

                scf[scfPos++] = 0;
                scf[scfPos++] = 0;
                scf[scfPos++] = 0;
            }
            else
            {
                int i;
                var scfsi = gr_infos.scfsi;
                if (scfsi < 0)
                {
                    for (i = 11; i != 0; i--)
                        scf[scfPos++] = common.getbits_fast(mp, num0);

                    for (i = 10; i != 0; i--)
                        scf[scfPos++] = common.getbits_fast(mp, num1);

                    numbits = (num0 + num1) * 10 + num0;
                }
                else
                {
                    numbits = 0;
                    if (0 == (scfsi & 0x8))
                    {
                        for (i = 6; i != 0; i--)
                            scf[scfPos++] = common.getbits_fast(mp, num0);

                        numbits += num0 * 6;
                    }
                    else
                    {
                        scfPos += 6;
                    }

                    if (0 == (scfsi & 0x4))
                    {
                        for (i = 5; i != 0; i--)
                            scf[scfPos++] = common.getbits_fast(mp, num0);

                        numbits += num0 * 5;
                    }
                    else
                    {
                        scfPos += 5;
                    }

                    if (0 == (scfsi & 0x2))
                    {
                        for (i = 5; i != 0; i--)
                            scf[scfPos++] = common.getbits_fast(mp, num1);

                        numbits += num1 * 5;
                    }
                    else
                    {
                        scfPos += 5;
                    }

                    if (0 == (scfsi & 0x1))
                    {
                        for (i = 5; i != 0; i--)
                            scf[scfPos++] = common.getbits_fast(mp, num1);

                        numbits += num1 * 5;
                    }
                    else
                    {
                        scfPos += 5;
                    }
                }

                scf[scfPos++] = 0;
            }

            return numbits;
        }

        private int III_get_scale_factors_2(MPGLib.mpstr_tag mp, int[] scf, MPG123.gr_info_s gr_infos, int i_stereo)
        {
            var scfPos = 0;
            int[] pnt;
            int i, j;
            int slen;
            var n = 0;
            var numbits = 0;
            if (i_stereo != 0)
                slen = i_slen2[gr_infos.scalefac_compress >> 1];
            else
                slen = n_slen2[gr_infos.scalefac_compress];

            gr_infos.preflag = (slen >> 15) & 0x1;
            n = 0;
            if (gr_infos.block_type == 2)
            {
                n++;
                if (gr_infos.mixed_block_flag != 0)
                    n++;
            }

            pnt = stab[n][(slen >> 12) & 0x7];
            for (i = 0; i < 4; i++)
            {
                var num = slen & 0x7;
                slen >>= 3;
                if (num != 0)
                {
                    for (j = 0; j < pnt[i]; j++)
                        scf[scfPos++] = common.getbits_fast(mp, num);

                    numbits += pnt[i] * num;
                }
                else
                {
                    for (j = 0; j < pnt[i]; j++)
                        scf[scfPos++] = 0;
                }
            }

            n = (n << 1) + 1;
            for (i = 0; i < n; i++)
                scf[scfPos++] = 0;

            return numbits;
        }

        private int III_dequantize_sample(
            MPGLib.mpstr_tag mp,
            float[] xr,
            int[] scf,
            MPG123.gr_info_s gr_infos,
            int sfreq,
            int part2bits)
        {
            var scfPos = 0;
            var shift = 1 + gr_infos.scalefac_scale;
            var xrpnt = xr;
            var xrpntPos = 0;
            var l = new int[3];
            int l3;
            var part2remain = gr_infos.part2_3_length - part2bits;
            int me;
            {
                int i;
                for (i = (MPG123.SBLIMIT * MPG123.SSLIMIT - xrpntPos) >> 1; i > 0; i--)
                {
                    xrpnt[xrpntPos++] = 0.0f;
                    xrpnt[xrpntPos++] = 0.0f;
                }

                xrpnt = xr;
                xrpntPos = 0;
            }
            {
                var bv = gr_infos.big_values;
                var region1 = gr_infos.region1start;
                var region2 = gr_infos.region2start;
                l3 = ((576 >> 1) - bv) >> 1;
                if (bv <= region1)
                {
                    l[0] = bv;
                    l[1] = 0;
                    l[2] = 0;
                }
                else
                {
                    l[0] = region1;
                    if (bv <= region2)
                    {
                        l[1] = bv - l[0];
                        l[2] = 0;
                    }
                    else
                    {
                        l[1] = region2 - l[0];
                        l[2] = bv - region2;
                    }
                }
            }
            {
                int i;
                for (i = 0; i < 3; i++)
                    if (l[i] < 0)
                    {
                        Console.WriteLine("hip: Bogus region length (%d)\n", l[i]);
                        l[i] = 0;
                    }
            }
            if (gr_infos.block_type == 2)
            {
                int i;
                var max = new int[4];
                int step = 0, lwin = 0, cb = 0;
                var v = 0.0f;
                int[] m;
                int mc;
                var mPos = 0;
                if (gr_infos.mixed_block_flag != 0)
                {
                    max[3] = -1;
                    max[0] = max[1] = max[2] = 2;
                    m = map[sfreq][0];
                    mPos = 0;
                    me = mapend[sfreq][0];
                }
                else
                {
                    max[0] = max[1] = max[2] = max[3] = -1;
                    m = map[sfreq][1];
                    mPos = 0;
                    me = mapend[sfreq][1];
                }

                mc = 0;
                for (i = 0; i < 2; i++)
                {
                    var lp = l[i];
                    var h = Huffman.ht;
                    var hPos = gr_infos.table_select[i];
                    for (; lp != 0; lp--, mc--)
                    {
                        int x, y;
                        if (0 == mc)
                        {
                            mc = m[mPos++];
                            xrpnt = xr;
                            xrpntPos = m[mPos++];
                            lwin = m[mPos++];
                            cb = m[mPos++];
                            if (lwin == 3)
                            {
                                v = gr_infos.pow2gain[gr_infos.pow2gainPos + (scf[scfPos++] << shift)];
                                step = 1;
                            }
                            else
                            {
                                v = gr_infos.full_gain[lwin][gr_infos.full_gainPos[lwin] + (scf[scfPos++] << shift)];
                                step = 3;
                            }
                        }

                        {
                            var val = h[hPos].table;
                            var valPos = 0;
                            while ((y = val[valPos++]) < 0)
                            {
                                if (get1bit(mp) != 0)
                                    valPos -= y;

                                part2remain--;
                            }

                            x = y >> 4;
                            y &= 0xf;
                        }
                        if (x == 15)
                        {
                            max[lwin] = cb;
                            part2remain -= h[hPos].linbits + 1;
                            x += common.getbits(mp, h[hPos].linbits);
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos] = -ispow[x] * v;
                            else
                                xrpnt[xrpntPos] = ispow[x] * v;
                        }
                        else if (x != 0)
                        {
                            max[lwin] = cb;
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos] = -ispow[x] * v;
                            else
                                xrpnt[xrpntPos] = ispow[x] * v;

                            part2remain--;
                        }
                        else
                        {
                            xrpnt[xrpntPos] = 0.0f;
                        }

                        xrpntPos += step;
                        if (y == 15)
                        {
                            max[lwin] = cb;
                            part2remain -= h[hPos].linbits + 1;
                            y += common.getbits(mp, h[hPos].linbits);
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos] = -ispow[y] * v;
                            else
                                xrpnt[xrpntPos] = ispow[y] * v;
                        }
                        else if (y != 0)
                        {
                            max[lwin] = cb;
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos] = -ispow[y] * v;
                            else
                                xrpnt[xrpntPos] = ispow[y] * v;

                            part2remain--;
                        }
                        else
                        {
                            xrpnt[xrpntPos] = 0.0f;
                        }

                        xrpntPos += step;
                    }
                }

                for (; l3 != 0 && part2remain > 0; l3--)
                {
                    var h = Huffman.htc;
                    var hPos = gr_infos.count1table_select;
                    var val = h[hPos].table;
                    var valPos = 0;
                    short a;
                    while ((a = val[valPos++]) < 0)
                    {
                        part2remain--;
                        if (part2remain < 0)
                        {
                            part2remain++;
                            a = 0;
                            break;
                        }

                        if (get1bit(mp) != 0)
                            valPos -= a;
                    }

                    for (i = 0; i < 4; i++)
                    {
                        if (0 == (i & 1))
                        {
                            if (0 == mc)
                            {
                                mc = m[mPos++];
                                xrpnt = xr;
                                xrpntPos = m[mPos++];
                                lwin = m[mPos++];
                                cb = m[mPos++];
                                if (lwin == 3)
                                {
                                    v = gr_infos.pow2gain[gr_infos.pow2gainPos + (scf[scfPos++] << shift)];
                                    step = 1;
                                }
                                else
                                {
                                    v = gr_infos.full_gain[lwin][
                                        gr_infos.full_gainPos[lwin] + (scf[scfPos++] << shift)];
                                    step = 3;
                                }
                            }

                            mc--;
                        }

                        if ((a & (0x8 >> i)) != 0)
                        {
                            max[lwin] = cb;
                            part2remain--;
                            if (part2remain < 0)
                            {
                                part2remain++;
                                break;
                            }

                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos] = -v;
                            else
                                xrpnt[xrpntPos] = v;
                        }
                        else
                        {
                            xrpnt[xrpntPos] = 0.0f;
                        }

                        xrpntPos += step;
                    }
                }

                while (mPos < me)
                {
                    if (0 == mc)
                    {
                        mc = m[mPos++];
                        xrpnt = xr;
                        xrpntPos = m[mPos++];
                        if (m[mPos++] == 3)
                            step = 1;
                        else
                            step = 3;

                        mPos++;
                    }

                    mc--;
                    xrpnt[xrpntPos] = 0.0f;
                    xrpntPos += step;
                    xrpnt[xrpntPos] = 0.0f;
                    xrpntPos += step;
                }

                gr_infos.maxband[0] = max[0] + 1;
                gr_infos.maxband[1] = max[1] + 1;
                gr_infos.maxband[2] = max[2] + 1;
                gr_infos.maxbandl = max[3] + 1;
                {
                    var rmax = max[0] > max[1] ? max[0] : max[1];
                    rmax = (rmax > max[2] ? rmax : max[2]) + 1;
                    gr_infos.maxb = rmax != 0 ? shortLimit[sfreq][rmax] : longLimit[sfreq][max[3] + 1];
                }
            }
            else
            {
                var pretab = gr_infos.preflag != 0 ? pretab1 : pretab2;
                var pretabPos = 0;
                int i, max = -1;
                var cb = 0;
                var m = map[sfreq][2];
                var mPos = 0;
                var v = 0.0f;
                var mc = 0;
                for (i = 0; i < 3; i++)
                {
                    var lp = l[i];
                    var h = Huffman.ht;
                    var hPos = gr_infos.table_select[i];
                    for (; lp != 0; lp--, mc--)
                    {
                        int x, y;
                        if (0 == mc)
                        {
                            mc = m[mPos++];
                            v = gr_infos.pow2gain[gr_infos.pow2gainPos +
                                                  ((scf[scfPos++] + pretab[pretabPos++]) << shift)];
                            cb = m[mPos++];
                        }

                        {
                            var val = h[hPos].table;
                            var valPos = 0;
                            while ((y = val[valPos++]) < 0)
                            {
                                if (get1bit(mp) != 0)
                                    valPos -= y;

                                part2remain--;
                            }

                            x = y >> 4;
                            y &= 0xf;
                        }
                        if (x == 15)
                        {
                            max = cb;
                            part2remain -= h[hPos].linbits + 1;
                            x += common.getbits(mp, h[hPos].linbits);
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos++] = -ispow[x] * v;
                            else
                                xrpnt[xrpntPos++] = ispow[x] * v;
                        }
                        else if (x != 0)
                        {
                            max = cb;
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos++] = -ispow[x] * v;
                            else
                                xrpnt[xrpntPos++] = ispow[x] * v;

                            part2remain--;
                        }
                        else
                        {
                            xrpnt[xrpntPos++] = 0.0f;
                        }

                        if (y == 15)
                        {
                            max = cb;
                            part2remain -= h[hPos].linbits + 1;
                            y += common.getbits(mp, h[hPos].linbits);
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos++] = -ispow[y] * v;
                            else
                                xrpnt[xrpntPos++] = ispow[y] * v;
                        }
                        else if (y != 0)
                        {
                            max = cb;
                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos++] = -ispow[y] * v;
                            else
                                xrpnt[xrpntPos++] = ispow[y] * v;

                            part2remain--;
                        }
                        else
                        {
                            xrpnt[xrpntPos++] = 0.0f;
                        }
                    }
                }

                for (; l3 != 0 && part2remain > 0; l3--)
                {
                    var h = Huffman.htc;
                    var hPos = gr_infos.count1table_select;
                    var val = h[hPos].table;
                    var valPos = 0;
                    short a;
                    while ((a = val[valPos++]) < 0)
                    {
                        part2remain--;
                        if (part2remain < 0)
                        {
                            part2remain++;
                            a = 0;
                            break;
                        }

                        if (get1bit(mp) != 0)
                            valPos -= a;
                    }

                    for (i = 0; i < 4; i++)
                    {
                        if (0 == (i & 1))
                        {
                            if (0 == mc)
                            {
                                mc = m[mPos++];
                                cb = m[mPos++];
                                v = gr_infos.pow2gain[gr_infos.pow2gainPos +
                                                      ((scf[scfPos++] + pretab[pretabPos++]) << shift)];
                            }

                            mc--;
                        }

                        if ((a & (0x8 >> i)) != 0)
                        {
                            max = cb;
                            part2remain--;
                            if (part2remain < 0)
                            {
                                part2remain++;
                                break;
                            }

                            if (get1bit(mp) != 0)
                                xrpnt[xrpntPos++] = -v;
                            else
                                xrpnt[xrpntPos++] = v;
                        }
                        else
                        {
                            xrpnt[xrpntPos++] = 0.0f;
                        }
                    }
                }

                for (i = (MPG123.SBLIMIT * MPG123.SSLIMIT - xrpntPos) >> 1; i != 0; i--)
                {
                    xrpnt[xrpntPos++] = 0.0f;
                    xrpnt[xrpntPos++] = 0.0f;
                }

                gr_infos.maxbandl = max + 1;
                gr_infos.maxb = longLimit[sfreq][gr_infos.maxbandl];
            }

            while (part2remain > 16)
            {
                common.getbits(mp, 16);
                part2remain -= 16;
            }

            if (part2remain > 0)
            {
                common.getbits(mp, part2remain);
            }
            else if (part2remain < 0)
            {
                Console.WriteLine("hip: Can't rewind stream by %d bits!\n", -part2remain);
                return 1;
            }

            return 0;
        }

        private void III_i_stereo(
            float[][] xr_buf,
            int[] scalefac,
            MPG123.gr_info_s gr_infos,
            int sfreq,
            int ms_stereo,
            int lsf)
        {
            var xr = xr_buf;

            var bi = bandInfo[sfreq];
            float[] tabl1, tabl2;
            if (lsf != 0)
            {
                var p = gr_infos.scalefac_compress & 0x1;
                if (ms_stereo != 0)
                {
                    tabl1 = pow1_2[p];
                    tabl2 = pow2_2[p];
                }
                else
                {
                    tabl1 = pow1_1[p];
                    tabl2 = pow2_1[p];
                }
            }
            else
            {
                if (ms_stereo != 0)
                {
                    tabl1 = tan1_2;
                    tabl2 = tan2_2;
                }
                else
                {
                    tabl1 = tan1_1;
                    tabl2 = tan2_1;
                }
            }

            if (gr_infos.block_type == 2)
            {
                int lwin, do_l = 0;
                if (gr_infos.mixed_block_flag != 0)
                    do_l = 1;

                for (lwin = 0; lwin < 3; lwin++)
                {
                    int is_p, sb, idx, sfb = gr_infos.maxband[lwin];
                    if (sfb > 3)
                        do_l = 0;

                    for (; sfb < 12; sfb++)
                    {
                        is_p = scalefac[sfb * 3 + lwin - gr_infos.mixed_block_flag];
                        if (is_p != 7)
                        {
                            float t1, t2;
                            sb = bi.shortDiff[sfb];
                            idx = bi.shortIdx[sfb] + lwin;
                            t1 = tabl1[is_p];
                            t2 = tabl2[is_p];
                            for (; sb > 0; sb--, idx += 3)
                            {
                                var v = xr[0][idx];
                                xr[0][idx] = v * t1;
                                xr[1][idx] = v * t2;
                            }
                        }
                    }

                    is_p = scalefac[11 * 3 + lwin - gr_infos.mixed_block_flag];
                    sb = bi.shortDiff[12];
                    idx = bi.shortIdx[12] + lwin;
                    if (is_p != 7)
                    {
                        float t1, t2;
                        t1 = tabl1[is_p];
                        t2 = tabl2[is_p];
                        for (; sb > 0; sb--, idx += 3)
                        {
                            var v = xr[0][idx];
                            xr[0][idx] = v * t1;
                            xr[1][idx] = v * t2;
                        }
                    }
                }

                if (do_l != 0)
                {
                    var sfb = gr_infos.maxbandl;
                    int idx = bi.longIdx[sfb];
                    for (; sfb < 8; sfb++)
                    {
                        int sb = bi.longDiff[sfb];
                        var is_p = scalefac[sfb];
                        if (is_p != 7)
                        {
                            float t1, t2;
                            t1 = tabl1[is_p];
                            t2 = tabl2[is_p];
                            for (; sb > 0; sb--, idx++)
                            {
                                var v = xr[0][idx];
                                xr[0][idx] = v * t1;
                                xr[1][idx] = v * t2;
                            }
                        }
                        else
                        {
                            idx += sb;
                        }
                    }
                }
            }
            else
            {
                var sfb = gr_infos.maxbandl;
                int is_p, idx = bi.longIdx[sfb];
                for (; sfb < 21; sfb++)
                {
                    int sb = bi.longDiff[sfb];
                    is_p = scalefac[sfb];
                    if (is_p != 7)
                    {
                        float t1, t2;
                        t1 = tabl1[is_p];
                        t2 = tabl2[is_p];
                        for (; sb > 0; sb--, idx++)
                        {
                            var v = xr[0][idx];
                            xr[0][idx] = v * t1;
                            xr[1][idx] = v * t2;
                        }
                    }
                    else
                    {
                        idx += sb;
                    }
                }

                is_p = scalefac[20];
                if (is_p != 7)
                {
                    int sb;
                    float t1 = tabl1[is_p], t2 = tabl2[is_p];
                    for (sb = bi.longDiff[21]; sb > 0; sb--, idx++)
                    {
                        var v = xr[0][idx];
                        xr[0][idx] = v * t1;
                        xr[1][idx] = v * t2;
                    }
                }
            }
        }

        private void III_antialias(float[] xr, MPG123.gr_info_s gr_infos)
        {
            int sblim;
            if (gr_infos.block_type == 2)
            {
                if (0 == gr_infos.mixed_block_flag)
                    return;

                sblim = 1;
            }
            else
            {
                sblim = gr_infos.maxb - 1;
            }

            {
                int sb;
                var xr1 = xr;
                var xr1Pos = MPG123.SSLIMIT;
                for (sb = sblim; sb != 0; sb--, xr1Pos += 10)
                {
                    int ss;
                    var cs = aa_cs;
                    var ca = aa_ca;
                    var caPos = 0;
                    var csPos = 0;
                    var xr2 = xr1;
                    var xr2Pos = xr1Pos;
                    for (ss = 7; ss >= 0; ss--)
                    {
                        float bu = xr2[--xr2Pos], bd = xr1[xr1Pos];
                        xr2[xr2Pos] = bu * cs[csPos] - bd * ca[caPos];
                        xr1[xr1Pos++] = bd * cs[csPos++] + bu * ca[caPos++];
                    }
                }
            }
        }

        private void dct36(
            float[] inbuf,
            int inbufPos,
            float[] o1,
            int o1Pos,
            float[] o2,
            int o2Pos,
            float[] wintab,
            float[] tsbuf,
            int tsPos)
        {
            {
                var @in = inbuf;
                var inPos = inbufPos;
                @in[inPos + 17] += @in[inPos + 16];
                @in[inPos + 16] += @in[inPos + 15];
                @in[inPos + 15] += @in[inPos + 14];
                @in[inPos + 14] += @in[inPos + 13];
                @in[inPos + 13] += @in[inPos + 12];
                @in[inPos + 12] += @in[inPos + 11];
                @in[inPos + 11] += @in[inPos + 10];
                @in[inPos + 10] += @in[inPos + 9];
                @in[inPos + 9] += @in[inPos + 8];
                @in[inPos + 8] += @in[inPos + 7];
                @in[inPos + 7] += @in[inPos + 6];
                @in[inPos + 6] += @in[inPos + 5];
                @in[inPos + 5] += @in[inPos + 4];
                @in[inPos + 4] += @in[inPos + 3];
                @in[inPos + 3] += @in[inPos + 2];
                @in[inPos + 2] += @in[inPos + 1];
                @in[inPos + 1] += @in[inPos + 0];
                @in[inPos + 17] += @in[inPos + 15];
                @in[inPos + 15] += @in[inPos + 13];
                @in[inPos + 13] += @in[inPos + 11];
                @in[inPos + 11] += @in[inPos + 9];
                @in[inPos + 9] += @in[inPos + 7];
                @in[inPos + 7] += @in[inPos + 5];
                @in[inPos + 5] += @in[inPos + 3];
                @in[inPos + 3] += @in[inPos + 1];
                {

                    var c = COS9;
                    var out2 = o2;
                    var out2Pos = o2Pos;
                    var w = wintab;
                    var out1 = o1;
                    var out1Pos = o1Pos;
                    var ts = tsbuf;
                    float ta33, ta66, tb33, tb66;
                    ta33 = @in[inPos + 2 * 3 + 0] * c[3];
                    ta66 = @in[inPos + 2 * 6 + 0] * c[6];
                    tb33 = @in[inPos + 2 * 3 + 1] * c[3];
                    tb66 = @in[inPos + 2 * 6 + 1] * c[6];
                    {
                        float tmp1a, tmp2a, tmp1b, tmp2b;
                        tmp1a = @in[inPos + 2 * 1 + 0] * c[1] + ta33 + @in[inPos + 2 * 5 + 0] * c[5] +
                                @in[inPos + 2 * 7 + 0] * c[7];
                        tmp1b = @in[inPos + 2 * 1 + 1] * c[1] + tb33 + @in[inPos + 2 * 5 + 1] * c[5] +
                                @in[inPos + 2 * 7 + 1] * c[7];
                        tmp2a = @in[inPos + 2 * 0 + 0] + @in[inPos + 2 * 2 + 0] * c[2] + @in[inPos + 2 * 4 + 0] * c[4] +
                                ta66 + @in[inPos + 2 * 8 + 0] * c[8];
                        tmp2b = @in[inPos + 2 * 0 + 1] + @in[inPos + 2 * 2 + 1] * c[2] + @in[inPos + 2 * 4 + 1] * c[4] +
                                tb66 + @in[inPos + 2 * 8 + 1] * c[8];
                        {
                            var sum0 = tmp1a + tmp2a;
                            var sum1 = (tmp1b + tmp2b) * tfcos36[0];
                            float tmp;
                            out2[out2Pos + 9 + 0] = (tmp = sum0 + sum1) * w[27 + 0];
                            out2[out2Pos + 8 - 0] = tmp * w[26 - 0];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 0)] = out1[out1Pos + 8 - 0] + sum0 * w[8 - 0];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 0)] = out1[out1Pos + 9 + 0] + sum0 * w[9 + 0];
                        }
                        {
                            float sum0, sum1;
                            sum0 = tmp2a - tmp1a;
                            sum1 = (tmp2b - tmp1b) * tfcos36[8];
                            float tmp;
                            out2[out2Pos + 9 + 8] = (tmp = sum0 + sum1) * w[27 + 8];
                            out2[out2Pos + 8 - 8] = tmp * w[26 - 8];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 8)] = out1[out1Pos + 8 - 8] + sum0 * w[8 - 8];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 8)] = out1[out1Pos + 9 + 8] + sum0 * w[9 + 8];
                        }
                    }
                    {
                        float tmp1a, tmp2a, tmp1b, tmp2b;
                        tmp1a = (@in[inPos + 2 * 1 + 0] - @in[inPos + 2 * 5 + 0] - @in[inPos + 2 * 7 + 0]) * c[3];
                        tmp1b = (@in[inPos + 2 * 1 + 1] - @in[inPos + 2 * 5 + 1] - @in[inPos + 2 * 7 + 1]) * c[3];
                        tmp2a = (@in[inPos + 2 * 2 + 0] - @in[inPos + 2 * 4 + 0] - @in[inPos + 2 * 8 + 0]) * c[6] -
                                @in[inPos + 2 * 6 + 0] + @in[inPos + 2 * 0 + 0];
                        tmp2b = (@in[inPos + 2 * 2 + 1] - @in[inPos + 2 * 4 + 1] - @in[inPos + 2 * 8 + 1]) * c[6] -
                                @in[inPos + 2 * 6 + 1] + @in[inPos + 2 * 0 + 1];
                        {
                            var sum0 = tmp1a + tmp2a;
                            var sum1 = (tmp1b + tmp2b) * tfcos36[1];
                            float tmp;
                            out2[out2Pos + 9 + 1] = (tmp = sum0 + sum1) * w[27 + 1];
                            out2[out2Pos + 8 - 1] = tmp * w[26 - 1];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 1)] = out1[out1Pos + 8 - 1] + sum0 * w[8 - 1];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 1)] = out1[out1Pos + 9 + 1] + sum0 * w[9 + 1];
                        }
                        {
                            float sum0, sum1;
                            sum0 = tmp2a - tmp1a;
                            sum1 = (tmp2b - tmp1b) * tfcos36[7];
                            float tmp;
                            out2[out2Pos + 9 + 7] = (tmp = sum0 + sum1) * w[27 + 7];
                            out2[out2Pos + 8 - 7] = tmp * w[26 - 7];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 7)] = out1[out1Pos + 8 - 7] + sum0 * w[8 - 7];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 7)] = out1[out1Pos + 9 + 7] + sum0 * w[9 + 7];
                        }
                    }
                    {
                        float tmp1a, tmp2a, tmp1b, tmp2b;
                        tmp1a = @in[inPos + 2 * 1 + 0] * c[5] - ta33 - @in[inPos + 2 * 5 + 0] * c[7] +
                                @in[inPos + 2 * 7 + 0] * c[1];
                        tmp1b = @in[inPos + 2 * 1 + 1] * c[5] - tb33 - @in[inPos + 2 * 5 + 1] * c[7] +
                                @in[inPos + 2 * 7 + 1] * c[1];
                        tmp2a = @in[inPos + 2 * 0 + 0] - @in[inPos + 2 * 2 + 0] * c[8] - @in[inPos + 2 * 4 + 0] * c[2] +
                                ta66 + @in[inPos + 2 * 8 + 0] * c[4];
                        tmp2b = @in[inPos + 2 * 0 + 1] - @in[inPos + 2 * 2 + 1] * c[8] - @in[inPos + 2 * 4 + 1] * c[2] +
                                tb66 + @in[inPos + 2 * 8 + 1] * c[4];
                        {
                            var sum0 = tmp1a + tmp2a;
                            var sum1 = (tmp1b + tmp2b) * tfcos36[2];
                            float tmp;
                            out2[out2Pos + 9 + 2] = (tmp = sum0 + sum1) * w[27 + 2];
                            out2[out2Pos + 8 - 2] = tmp * w[26 - 2];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 2)] = out1[out1Pos + 8 - 2] + sum0 * w[8 - 2];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 2)] = out1[out1Pos + 9 + 2] + sum0 * w[9 + 2];
                        }
                        {
                            float sum0, sum1;
                            sum0 = tmp2a - tmp1a;
                            sum1 = (tmp2b - tmp1b) * tfcos36[6];
                            float tmp;
                            out2[out2Pos + 9 + 6] = (tmp = sum0 + sum1) * w[27 + 6];
                            out2[out2Pos + 8 - 6] = tmp * w[26 - 6];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 6)] = out1[out1Pos + 8 - 6] + sum0 * w[8 - 6];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 6)] = out1[out1Pos + 9 + 6] + sum0 * w[9 + 6];
                        }
                    }
                    {
                        float tmp1a, tmp2a, tmp1b, tmp2b;
                        tmp1a = @in[inPos + 2 * 1 + 0] * c[7] - ta33 + @in[inPos + 2 * 5 + 0] * c[1] -
                                @in[inPos + 2 * 7 + 0] * c[5];
                        tmp1b = @in[inPos + 2 * 1 + 1] * c[7] - tb33 + @in[inPos + 2 * 5 + 1] * c[1] -
                                @in[inPos + 2 * 7 + 1] * c[5];
                        tmp2a = @in[inPos + 2 * 0 + 0] - @in[inPos + 2 * 2 + 0] * c[4] + @in[inPos + 2 * 4 + 0] * c[8] +
                                ta66 - @in[inPos + 2 * 8 + 0] * c[2];
                        tmp2b = @in[inPos + 2 * 0 + 1] - @in[inPos + 2 * 2 + 1] * c[4] + @in[inPos + 2 * 4 + 1] * c[8] +
                                tb66 - @in[inPos + 2 * 8 + 1] * c[2];
                        {
                            var sum0 = tmp1a + tmp2a;
                            var sum1 = (tmp1b + tmp2b) * tfcos36[3];
                            float tmp;
                            out2[out2Pos + 9 + 3] = (tmp = sum0 + sum1) * w[27 + 3];
                            out2[out2Pos + 8 - 3] = tmp * w[26 - 3];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 3)] = out1[out1Pos + 8 - 3] + sum0 * w[8 - 3];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 3)] = out1[out1Pos + 9 + 3] + sum0 * w[9 + 3];
                        }
                        {
                            float sum0, sum1;
                            sum0 = tmp2a - tmp1a;
                            sum1 = (tmp2b - tmp1b) * tfcos36[5];
                            float tmp;
                            out2[out2Pos + 9 + 5] = (tmp = sum0 + sum1) * w[27 + 5];
                            out2[out2Pos + 8 - 5] = tmp * w[26 - 5];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 5)] = out1[out1Pos + 8 - 5] + sum0 * w[8 - 5];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 5)] = out1[out1Pos + 9 + 5] + sum0 * w[9 + 5];
                        }
                    }
                    {
                        float sum0, sum1;
                        sum0 = @in[inPos + 2 * 0 + 0] - @in[inPos + 2 * 2 + 0] + @in[inPos + 2 * 4 + 0] -
                               @in[inPos + 2 * 6 + 0] + @in[inPos + 2 * 8 + 0];
                        sum1 = (@in[inPos + 2 * 0 + 1] - @in[inPos + 2 * 2 + 1] + @in[inPos + 2 * 4 + 1] -
                                @in[inPos + 2 * 6 + 1] + @in[inPos + 2 * 8 + 1]) * tfcos36[4];
                        {
                            float tmp;
                            out2[out2Pos + 9 + 4] = (tmp = sum0 + sum1) * w[27 + 4];
                            out2[out2Pos + 8 - 4] = tmp * w[26 - 4];
                            sum0 -= sum1;
                            ts[tsPos + MPG123.SBLIMIT * (8 - 4)] = out1[out1Pos + 8 - 4] + sum0 * w[8 - 4];
                            ts[tsPos + MPG123.SBLIMIT * (9 + 4)] = out1[out1Pos + 9 + 4] + sum0 * w[9 + 4];
                        }
                    }
                }
            }
        }

        private void dct12(
            float[] @in,
            int inbufPos,
            float[] rawout1,
            int rawout1Pos,
            float[] rawout2,
            int rawout2Pos,
            float[] wi,
            float[] ts,
            int tsPos)
        {
            {
                float in0, in1, in2, in3, in4, in5;
                var out1 = rawout1;
                var out1Pos = rawout1Pos;
                ts[tsPos + MPG123.SBLIMIT * 0] = out1[out1Pos + 0];
                ts[tsPos + MPG123.SBLIMIT * 1] = out1[out1Pos + 1];
                ts[tsPos + MPG123.SBLIMIT * 2] = out1[out1Pos + 2];
                ts[tsPos + MPG123.SBLIMIT * 3] = out1[out1Pos + 3];
                ts[tsPos + MPG123.SBLIMIT * 4] = out1[out1Pos + 4];
                ts[tsPos + MPG123.SBLIMIT * 5] = out1[out1Pos + 5];
                {
                    in5 = @in[inbufPos + 5 * 3];
                    in5 += in4 = @in[inbufPos + 4 * 3];
                    in4 += in3 = @in[inbufPos + 3 * 3];
                    in3 += in2 = @in[inbufPos + 2 * 3];
                    in2 += in1 = @in[inbufPos + 1 * 3];
                    in1 += in0 = @in[inbufPos + 0 * 3];
                    in5 += in3;
                    in3 += in1;
                    in2 *= COS6_1;
                    in3 *= COS6_1;
                }
                {
                    float tmp0, tmp1 = in0 - in4;
                    {
                        var tmp2 = (in1 - in5) * tfcos12[1];
                        tmp0 = tmp1 + tmp2;
                        tmp1 -= tmp2;
                    }
                    ts[tsPos + (17 - 1) * MPG123.SBLIMIT] = out1[out1Pos + 17 - 1] + tmp0 * wi[11 - 1];
                    ts[tsPos + (12 + 1) * MPG123.SBLIMIT] = out1[out1Pos + 12 + 1] + tmp0 * wi[6 + 1];
                    ts[tsPos + (6 + 1) * MPG123.SBLIMIT] = out1[out1Pos + 6 + 1] + tmp1 * wi[1];
                    ts[tsPos + (11 - 1) * MPG123.SBLIMIT] = out1[out1Pos + 11 - 1] + tmp1 * wi[5 - 1];
                }
                {
                    in0 += in4 * COS6_2;
                    in4 = in0 + in2;
                    in0 -= in2;
                    in1 += in5 * COS6_2;
                    in5 = (in1 + in3) * tfcos12[0];
                    in1 = (in1 - in3) * tfcos12[2];
                    in3 = in4 + in5;
                    in4 -= in5;
                    in2 = in0 + in1;
                    in0 -= in1;
                }
                ts[tsPos + (17 - 0) * MPG123.SBLIMIT] = out1[out1Pos + 17 - 0] + in2 * wi[11 - 0];
                ts[tsPos + (12 + 0) * MPG123.SBLIMIT] = out1[out1Pos + 12 + 0] + in2 * wi[6 + 0];
                ts[tsPos + (12 + 2) * MPG123.SBLIMIT] = out1[out1Pos + 12 + 2] + in3 * wi[6 + 2];
                ts[tsPos + (17 - 2) * MPG123.SBLIMIT] = out1[out1Pos + 17 - 2] + in3 * wi[11 - 2];
                ts[tsPos + (6 + 0) * MPG123.SBLIMIT] = out1[out1Pos + 6 + 0] + in0 * wi[0];
                ts[tsPos + (11 - 0) * MPG123.SBLIMIT] = out1[out1Pos + 11 - 0] + in0 * wi[5 - 0];
                ts[tsPos + (6 + 2) * MPG123.SBLIMIT] = out1[out1Pos + 6 + 2] + in4 * wi[2];
                ts[tsPos + (11 - 2) * MPG123.SBLIMIT] = out1[out1Pos + 11 - 2] + in4 * wi[5 - 2];
            }
            inbufPos++;
            {
                float in0, in1, in2, in3, in4, in5;
                var out2 = rawout2;
                var out2Pos = rawout2Pos;
                {
                    in5 = @in[inbufPos + 5 * 3];
                    in5 += in4 = @in[inbufPos + 4 * 3];
                    in4 += in3 = @in[inbufPos + 3 * 3];
                    in3 += in2 = @in[inbufPos + 2 * 3];
                    in2 += in1 = @in[inbufPos + 1 * 3];
                    in1 += in0 = @in[inbufPos + 0 * 3];
                    in5 += in3;
                    in3 += in1;
                    in2 *= COS6_1;
                    in3 *= COS6_1;
                }
                {
                    float tmp0, tmp1 = in0 - in4;
                    {
                        var tmp2 = (in1 - in5) * tfcos12[1];
                        tmp0 = tmp1 + tmp2;
                        tmp1 -= tmp2;
                    }
                    out2[out2Pos + 5 - 1] = tmp0 * wi[11 - 1];
                    out2[out2Pos + 0 + 1] = tmp0 * wi[6 + 1];
                    ts[tsPos + (12 + 1) * MPG123.SBLIMIT] += tmp1 * wi[1];
                    ts[tsPos + (17 - 1) * MPG123.SBLIMIT] += tmp1 * wi[5 - 1];
                }
                {
                    in0 += in4 * COS6_2;
                    in4 = in0 + in2;
                    in0 -= in2;
                    in1 += in5 * COS6_2;
                    in5 = (in1 + in3) * tfcos12[0];
                    in1 = (in1 - in3) * tfcos12[2];
                    in3 = in4 + in5;
                    in4 -= in5;
                    in2 = in0 + in1;
                    in0 -= in1;
                }
                out2[out2Pos + 5 - 0] = in2 * wi[11 - 0];
                out2[out2Pos + 0 + 0] = in2 * wi[6 + 0];
                out2[out2Pos + 0 + 2] = in3 * wi[6 + 2];
                out2[out2Pos + 5 - 2] = in3 * wi[11 - 2];
                ts[tsPos + (12 + 0) * MPG123.SBLIMIT] += in0 * wi[0];
                ts[tsPos + (17 - 0) * MPG123.SBLIMIT] += in0 * wi[5 - 0];
                ts[tsPos + (12 + 2) * MPG123.SBLIMIT] += in4 * wi[2];
                ts[tsPos + (17 - 2) * MPG123.SBLIMIT] += in4 * wi[5 - 2];
            }
            inbufPos++;
            {
                float in0, in1, in2, in3, in4, in5;
                var out2 = rawout2;
                var out2Pos = rawout2Pos;
                out2[out2Pos + 12] = out2[out2Pos + 13] = out2[out2Pos + 14] =
                    out2[out2Pos + 15] = out2[out2Pos + 16] = out2[out2Pos + 17] = 0.0f;
                {
                    in5 = @in[inbufPos + 5 * 3];
                    in5 += in4 = @in[inbufPos + 4 * 3];
                    in4 += in3 = @in[inbufPos + 3 * 3];
                    in3 += in2 = @in[inbufPos + 2 * 3];
                    in2 += in1 = @in[inbufPos + 1 * 3];
                    in1 += in0 = @in[inbufPos + 0 * 3];
                    in5 += in3;
                    in3 += in1;
                    in2 *= COS6_1;
                    in3 *= COS6_1;
                }
                {
                    float tmp0, tmp1 = in0 - in4;
                    {
                        var tmp2 = (in1 - in5) * tfcos12[1];
                        tmp0 = tmp1 + tmp2;
                        tmp1 -= tmp2;
                    }
                    out2[out2Pos + 11 - 1] = tmp0 * wi[11 - 1];
                    out2[out2Pos + 6 + 1] = tmp0 * wi[6 + 1];
                    out2[out2Pos + 0 + 1] += tmp1 * wi[1];
                    out2[out2Pos + 5 - 1] += tmp1 * wi[5 - 1];
                }
                {
                    in0 += in4 * COS6_2;
                    in4 = in0 + in2;
                    in0 -= in2;
                    in1 += in5 * COS6_2;
                    in5 = (in1 + in3) * tfcos12[0];
                    in1 = (in1 - in3) * tfcos12[2];
                    in3 = in4 + in5;
                    in4 -= in5;
                    in2 = in0 + in1;
                    in0 -= in1;
                }
                out2[out2Pos + 11 - 0] = in2 * wi[11 - 0];
                out2[out2Pos + 6 + 0] = in2 * wi[6 + 0];
                out2[out2Pos + 6 + 2] = in3 * wi[6 + 2];
                out2[out2Pos + 11 - 2] = in3 * wi[11 - 2];
                out2[out2Pos + 0 + 0] += in0 * wi[0];
                out2[out2Pos + 5 - 0] += in0 * wi[5 - 0];
                out2[out2Pos + 0 + 2] += in4 * wi[2];
                out2[out2Pos + 5 - 2] += in4 * wi[5 - 2];
            }
        }

        private void III_hybrid(MPGLib.mpstr_tag mp, float[] fsIn, float[] tsOut, int ch, MPG123.gr_info_s gr_infos)
        {
            var tspnt = tsOut;
            var tspntPos = 0;
            var block = mp.hybrid_block;
            var blc = mp.hybrid_blc;
            float[] rawout1, rawout2;
            int rawout1Pos, rawout2Pos;
            int bt;
            var sb = 0;
            {
                var b = blc[ch];
                rawout1 = block[b][ch];
                rawout1Pos = 0;
                b = -b + 1;
                rawout2 = block[b][ch];
                rawout2Pos = 0;
                blc[ch] = b;
            }
            if (gr_infos.mixed_block_flag != 0)
            {
                sb = 2;
                dct36(fsIn, 0 * MPG123.SSLIMIT, rawout1, rawout1Pos, rawout2, rawout2Pos, win[0], tspnt, tspntPos + 0);
                dct36(
                    fsIn,
                    1 * MPG123.SSLIMIT,
                    rawout1,
                    rawout1Pos + 18,
                    rawout2,
                    rawout2Pos + 18,
                    win1[0],
                    tspnt,
                    tspntPos + 1);
                rawout1Pos += 36;
                rawout2Pos += 36;
                tspntPos += 2;
            }

            bt = gr_infos.block_type;
            if (bt == 2)
                for (; sb < gr_infos.maxb; sb += 2, tspntPos += 2, rawout1Pos += 36, rawout2Pos += 36)
                {
                    dct12(
                        fsIn,
                        sb * MPG123.SSLIMIT,
                        rawout1,
                        rawout1Pos,
                        rawout2,
                        rawout2Pos,
                        win[2],
                        tspnt,
                        tspntPos + 0);
                    dct12(
                        fsIn,
                        (sb + 1) * MPG123.SSLIMIT,
                        rawout1,
                        rawout1Pos + 18,
                        rawout2,
                        rawout2Pos + 18,
                        win1[2],
                        tspnt,
                        tspntPos + 1);
                }
            else
                for (; sb < gr_infos.maxb; sb += 2, tspntPos += 2, rawout1Pos += 36, rawout2Pos += 36)
                {
                    dct36(
                        fsIn,
                        sb * MPG123.SSLIMIT,
                        rawout1,
                        rawout1Pos,
                        rawout2,
                        rawout2Pos,
                        win[bt],
                        tspnt,
                        tspntPos + 0);
                    dct36(
                        fsIn,
                        (sb + 1) * MPG123.SSLIMIT,
                        rawout1,
                        rawout1Pos + 18,
                        rawout2,
                        rawout2Pos + 18,
                        win1[bt],
                        tspnt,
                        tspntPos + 1);
                }

            for (; sb < MPG123.SBLIMIT; sb++, tspntPos++)
            {
                int i;
                for (i = 0; i < MPG123.SSLIMIT; i++)
                {
                    tspnt[tspntPos + i * MPG123.SBLIMIT] = rawout1[rawout1Pos++];
                    rawout2[rawout2Pos++] = 0.0f;
                }
            }
        }

        internal int layer3_audiodata_precedesframes(MPGLib.mpstr_tag mp)
        {
            int audioDataInFrame;
            int framesToBacktrack;
            audioDataInFrame = mp.bsize - 4 - mp.ssize;
            framesToBacktrack = (sideinfo.main_data_begin + audioDataInFrame - 1) / audioDataInFrame;
            return framesToBacktrack;
        }

        internal int do_layer3_sideinfo(MPGLib.mpstr_tag mp)
        {
            var fr = mp.fr;
            var stereo = fr.stereo;
            var single = fr.single;
            int ms_stereo;
            var sfreq = fr.sampling_frequency;
            int granules;
            int ch, gr, databits;
            if (stereo == 1)
                single = 0;

            if (fr.mode == MPG123.MPG_MD_JOINT_STEREO)
                ms_stereo = fr.mode_ext & 0x2;
            else
                ms_stereo = 0;

            if (fr.lsf != 0)
            {
                granules = 1;
                III_get_side_info_2(mp, sideinfo, stereo, ms_stereo, sfreq, single);
            }
            else
            {
                granules = 2;
                III_get_side_info_1(mp, sideinfo, stereo, ms_stereo, sfreq, single);
            }

            databits = 0;
            for (gr = 0; gr < granules; ++gr)
            for (ch = 0; ch < stereo; ++ch)
            {
                var gr_infos = sideinfo.ch[ch].gr[gr];
                databits += gr_infos.part2_3_length;
            }

            return databits - 8 * sideinfo.main_data_begin;
        }

        internal int do_layer3<T>(
            MPGLib.mpstr_tag mp,
            T[] pcm_sample,
            MPGLib.ProcessedBytes pcm_point,
            Interface.ISynth synth,
            Decode.Factory<T> tFactory)
        {
            int gr, ch, ss, clip = 0;

            var scalefacs = Arrays.ReturnRectangularArray<int>(2, 39);
            var fr = mp.fr;
            var stereo = fr.stereo;
            var single = fr.single;
            int ms_stereo, i_stereo;
            var sfreq = fr.sampling_frequency;
            int stereo1, granules;
            if (common.set_pointer(mp, sideinfo.main_data_begin) == MPGLib.MP3_ERR)
                return 0;

            if (stereo == 1)
            {
                stereo1 = 1;
                single = 0;
            }
            else if (single >= 0)
            {
                stereo1 = 1;
            }
            else
            {
                stereo1 = 2;
            }

            if (fr.mode == MPG123.MPG_MD_JOINT_STEREO)
            {
                ms_stereo = fr.mode_ext & 0x2;
                i_stereo = fr.mode_ext & 0x1;
            }
            else
            {
                ms_stereo = i_stereo = 0;
            }

            if (fr.lsf != 0)
                granules = 1;
            else
                granules = 2;

            for (gr = 0; gr < granules; gr++)
            {
                {
                    var gr_infos = sideinfo.ch[0].gr[gr];
                    int part2bits;
                    if (fr.lsf != 0)
                        part2bits = III_get_scale_factors_2(mp, scalefacs[0], gr_infos, 0);
                    else
                        part2bits = III_get_scale_factors_1(mp, scalefacs[0], gr_infos);

                    if (mp.pinfo != null)
                    {
                        int i;
                        mp.pinfo.sfbits[gr][0] = part2bits;
                        for (i = 0; i < 39; i++)
                            mp.pinfo.sfb_s[gr][0][i] = scalefacs[0][i];
                    }

                    if (III_dequantize_sample(mp, hybridIn[0], scalefacs[0], gr_infos, sfreq, part2bits) != 0)
                        return clip;
                }
                if (stereo == 2)
                {
                    var gr_infos = sideinfo.ch[1].gr[gr];
                    int part2bits;
                    if (fr.lsf != 0)
                        part2bits = III_get_scale_factors_2(mp, scalefacs[1], gr_infos, i_stereo);
                    else
                        part2bits = III_get_scale_factors_1(mp, scalefacs[1], gr_infos);

                    if (mp.pinfo != null)
                    {
                        int i;
                        mp.pinfo.sfbits[gr][1] = part2bits;
                        for (i = 0; i < 39; i++)
                            mp.pinfo.sfb_s[gr][1][i] = scalefacs[1][i];
                    }

                    if (III_dequantize_sample(mp, hybridIn[1], scalefacs[1], gr_infos, sfreq, part2bits) != 0)
                        return clip;

                    if (ms_stereo != 0)
                    {
                        int i;
                        for (i = 0; i < MPG123.SBLIMIT * MPG123.SSLIMIT; i++)
                        {
                            float tmp0, tmp1;
                            tmp0 = hybridIn[0][i];
                            tmp1 = hybridIn[1][i];
                            hybridIn[1][i] = tmp0 - tmp1;
                            hybridIn[0][i] = tmp0 + tmp1;
                        }
                    }

                    if (i_stereo != 0)
                        III_i_stereo(hybridIn, scalefacs[1], gr_infos, sfreq, ms_stereo, fr.lsf);

                    if (ms_stereo != 0 || i_stereo != 0 || single == 3)
                        if (gr_infos.maxb > sideinfo.ch[0].gr[gr].maxb)
                            sideinfo.ch[0].gr[gr].maxb = gr_infos.maxb;
                        else
                            gr_infos.maxb = sideinfo.ch[0].gr[gr].maxb;

                    switch (single)
                    {
                        case 3:
                        {
                            int i;
                            var in0 = hybridIn[0];
                            var in1 = hybridIn[1];
                            int in0Pos = 0, in1Pos = 0;
                            for (i = 0; i < MPG123.SSLIMIT * gr_infos.maxb; i++, in0Pos++)
                                in0[in0Pos] = in0[in0Pos] + in1[in1Pos++];
                        }
                            break;
                        case 1:
                        {
                            int i;
                            var in0 = hybridIn[0];
                            var in1 = hybridIn[1];
                            int in0Pos = 0, in1Pos = 0;
                            for (i = 0; i < MPG123.SSLIMIT * gr_infos.maxb; i++)
                                in0[in0Pos++] = in1[in1Pos++];
                        }
                            break;
                    }
                }

                if (mp.pinfo != null)
                {
                    int i, sb;
                    float ifqstep;
                    mp.pinfo.bitrate = Common.tabsel_123[fr.lsf][fr.lay - 1][fr.bitrate_index];
                    mp.pinfo.sampfreq = Common.freqs[sfreq];
                    mp.pinfo.emph = fr.emphasis;
                    mp.pinfo.crc = fr.error_protection ? 1 : 0;
                    mp.pinfo.padding = fr.padding;
                    mp.pinfo.stereo = fr.stereo;
                    mp.pinfo.js = fr.mode == MPG123.MPG_MD_JOINT_STEREO ? 1 : 0;
                    mp.pinfo.ms_stereo = ms_stereo;
                    mp.pinfo.i_stereo = i_stereo;
                    mp.pinfo.maindata = sideinfo.main_data_begin;
                    for (ch = 0; ch < stereo1; ch++)
                    {
                        var gr_infos = sideinfo.ch[ch].gr[gr];
                        mp.pinfo.big_values[gr][ch] = gr_infos.big_values;
                        mp.pinfo.scalefac_scale[gr][ch] = gr_infos.scalefac_scale;
                        mp.pinfo.mixed[gr][ch] = gr_infos.mixed_block_flag;
                        mp.pinfo.mpg123blocktype[gr][ch] = gr_infos.block_type;
                        mp.pinfo.mainbits[gr][ch] = gr_infos.part2_3_length;
                        mp.pinfo.preflag[gr][ch] = gr_infos.preflag;
                        if (gr == 1)
                            mp.pinfo.scfsi[ch] = gr_infos.scfsi;
                    }

                    for (ch = 0; ch < stereo1; ch++)
                    {
                        var gr_infos = sideinfo.ch[ch].gr[gr];
                        ifqstep = mp.pinfo.scalefac_scale[gr][ch] == 0 ? .5f : 1.0f;
                        if (2 == gr_infos.block_type)
                        {
                            for (i = 0; i < 3; i++)
                            {
                                for (sb = 0; sb < 12; sb++)
                                {
                                    var j = 3 * sb + i;
                                    mp.pinfo.sfb_s[gr][ch][j] =
                                        -ifqstep * mp.pinfo.sfb_s[gr][ch][j - gr_infos.mixed_block_flag];
                                    mp.pinfo.sfb_s[gr][ch][j] -= 2 * mp.pinfo.sub_gain[gr][ch][i];
                                }

                                mp.pinfo.sfb_s[gr][ch][3 * sb + i] = -2 * mp.pinfo.sub_gain[gr][ch][i];
                            }
                        }
                        else
                        {
                            for (sb = 0; sb < 21; sb++)
                            {
                                mp.pinfo.sfb[gr][ch][sb] = mp.pinfo.sfb_s[gr][ch][sb];
                                if (gr_infos.preflag != 0)
                                    mp.pinfo.sfb[gr][ch][sb] += pretab1[sb];

                                mp.pinfo.sfb[gr][ch][sb] *= -ifqstep;
                            }

                            mp.pinfo.sfb[gr][ch][21] = 0;
                        }
                    }

                    for (ch = 0; ch < stereo1; ch++)
                    {
                        var j = 0;
                        for (sb = 0; sb < MPG123.SBLIMIT; sb++)
                        for (ss = 0; ss < MPG123.SSLIMIT; ss++, j++)
                            mp.pinfo.mpg123xr[gr][ch][j] = hybridIn[ch][sb * MPG123.SSLIMIT + ss];
                    }
                }

                for (ch = 0; ch < stereo1; ch++)
                {
                    var gr_infos = sideinfo.ch[ch].gr[gr];
                    III_antialias(hybridIn[ch], gr_infos);
                    III_hybrid(mp, hybridIn[ch], hybridOut[ch], ch, gr_infos);
                }

                for (ss = 0; ss < MPG123.SSLIMIT; ss++)
                    if (single >= 0)
                    {
                        clip += synth.synth_1to1_mono_ptr(
                            mp,
                            hybridOut[0],
                            ss * MPG123.SBLIMIT,
                            pcm_sample,
                            pcm_point,
                            tFactory);
                    }
                    else
                    {
                        var p1 = new MPGLib.ProcessedBytes();
                        p1.pb = pcm_point.pb;
                        clip += synth.synth_1to1_ptr(
                            mp,
                            hybridOut[0],
                            ss * MPG123.SBLIMIT,
                            0,
                            pcm_sample,
                            p1,
                            tFactory);
                        clip += synth.synth_1to1_ptr(
                            mp,
                            hybridOut[1],
                            ss * MPG123.SBLIMIT,
                            1,
                            pcm_sample,
                            pcm_point,
                            tFactory);
                    }
            }

            return clip;
        }
    }
}