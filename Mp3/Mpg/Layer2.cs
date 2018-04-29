//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using GroovyCodecs.Mp3.Common;

/* 
 * layer2.c: Mpeg Layer-2 audio decoder 
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
/* $Id: Layer2.java,v 1.4 2011/05/31 19:42:01 kenchis Exp $ */

namespace GroovyCodecs.Mp3.Mpg
{

    internal class Layer2
    {

        private static readonly double[] mulmul =
        {
            0.0,
            -2.0 / 3.0,
            2.0 / 3.0,
            2.0 / 7.0,
            2.0 / 15.0,
            2.0 / 31.0,
            2.0 / 63.0,
            2.0 / 127.0,
            2.0 / 255.0,
            2.0 / 511.0,
            2.0 / 1023.0,
            2.0 / 2047.0,
            2.0 / 4095.0,
            2.0 / 8191.0,
            2.0 / 16383.0,
            2.0 / 32767.0,
            2.0 / 65535.0,
            -4.0 / 5.0,
            -2.0 / 5.0,
            2.0 / 5.0,
            4.0 / 5.0,
            -8.0 / 9.0,
            -4.0 / 9.0,
            -2.0 / 9.0,
            2.0 / 9.0,
            4.0 / 9.0,
            8.0 / 9.0
        };

        private static readonly int[][][] translate =
        {
            new[]
            {
                new[]
                {
                    0,
                    2,
                    2,
                    2,
                    2,
                    2,
                    2,
                    0,
                    0,
                    0,
                    1,
                    1,
                    1,
                    1,
                    1,
                    0
                },
                new[]
                {
                    0,
                    2,
                    2,
                    0,
                    0,
                    0,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    0
                }
            },
            new[]
            {
                new[]
                {
                    0,
                    2,
                    2,
                    2,
                    2,
                    2,
                    2,
                    0,
                    0,
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
                    2,
                    2,
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
                }
            },
            new[]
            {
                new[]
                {
                    0,
                    3,
                    3,
                    3,
                    3,
                    3,
                    3,
                    0,
                    0,
                    0,
                    1,
                    1,
                    1,
                    1,
                    1,
                    0
                },
                new[]
                {
                    0,
                    3,
                    3,
                    0,
                    0,
                    0,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    0
                }
            }
        };

        private readonly int[][] @base =
        {
            new[]
            {
                1,
                0,
                2
            },
            new[]
            {
                17,
                18,
                0,
                19,
                20
            },
            new[]
            {
                21,
                1,
                22,
                23,
                0,
                24,
                25,
                2,
                26
            }
        };

        private readonly int[] grp_3tab = new int[32 * 3]; // used: 27

        private readonly int[] grp_5tab = new int[128 * 3]; // used: 125

        private readonly int[] grp_9tab = new int[1024 * 3]; // used: 729

        private readonly int[] sblims =
        {
            27,
            30,
            8,
            12,
            30
        };

        private readonly int[] tablen =
        {
            3,
            5,
            9
        };

        private readonly L2Tables.al_table2[][] tables2 =
        {
            L2Tables.alloc_0,
            L2Tables.alloc_1,
            L2Tables.alloc_2,
            L2Tables.alloc_3,
            L2Tables.alloc_4
        };

        private Common common;

        private readonly bool InstanceFieldsInitialized;

        private int itable;

        private readonly int[] scfsi_buf = new int[64];

        private int[][] table;

        private int[][] tables;

        internal virtual Common Modules
        {
            set => common = value;
        }

        internal Layer2()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            tables = new[]
            {
                grp_3tab,
                grp_5tab,
                grp_9tab
            };
            table = new[]
            {
                null,
                null,
                null,
                grp_3tab,
                null,
                grp_5tab,
                null,
                null,
                null,
                grp_9tab
            };
        }

        internal virtual void init_layer2()
        {
            int i, j, k, l, len;
            float[] table;

            for (i = 0; i < 3; i++)
            {
                itable = 0;
                len = tablen[i];
                for (j = 0; j < len; j++)
                for (k = 0; k < len; k++)
                for (l = 0; l < len; l++)
                {
                    tables[i][itable++] = @base[i][l];
                    tables[i][itable++] = @base[i][k];
                    tables[i][itable++] = @base[i][j];
                }
            }

            for (k = 0; k < 27; k++)
            {
                var m = mulmul[k];
                table = common.muls[k];
                var tablePos = 0;
                for (j = 3, i = 0; i < 63; i++, j--)
                    table[tablePos++] = (float)(m * Math.Pow(2.0, j / 3.0));

                table[tablePos++] = 0.0f;
            }
        }

        private void II_step_one(MPGLib.mpstr_tag mp, int[] bit_alloc, int[] scale, Frame fr)
        {
            var scalePos = 0;
            var stereo = fr.stereo - 1;
            var sblimit = fr.II_sblimit;
            var jsbound = fr.jsbound;
            var sblimit2 = fr.II_sblimit << stereo;
            var alloc1 = 0;
            int i;
            int scfsi, bita;
            int sc, step;

            bita = 0;
            if (stereo != 0)
            {
                for (i = jsbound; i != 0; i--, alloc1 += 1 << step)
                {
                    bit_alloc[bita++] = (char)common.getbits(mp, step = fr.alloc[alloc1].bits);
                    bit_alloc[bita++] = (char)common.getbits(mp, step);
                }

                for (i = sblimit - jsbound; i != 0; i--, alloc1 += 1 << step)
                {
                    bit_alloc[bita + 0] = (char)common.getbits(mp, step = fr.alloc[alloc1].bits);
                    bit_alloc[bita + 1] = bit_alloc[bita + 0];
                    bita += 2;
                }

                bita = 0;
                scfsi = 0;
                for (i = sblimit2; i != 0; i--)
                    if (bit_alloc[bita++] != 0)
                        scfsi_buf[scfsi++] = (char)common.getbits_fast(mp, 2);
            }
            else
            {
                // mono

                for (i = sblimit; i != 0; i--, alloc1 += 1 << step)
                    bit_alloc[bita++] = (char)common.getbits(mp, step = fr.alloc[alloc1].bits);

                bita = 0;
                scfsi = 0;
                for (i = sblimit; i != 0; i--)
                    if (bit_alloc[bita++] != 0)
                        scfsi_buf[scfsi++] = (char)common.getbits_fast(mp, 2);
            }

            bita = 0;
            scfsi = 0;
            for (i = sblimit2; i != 0; i--)
                if (bit_alloc[bita++] != 0)
                    switch (scfsi_buf[scfsi++])
                    {
                        case 0:
                            scale[scalePos++] = common.getbits_fast(mp, 6);
                            scale[scalePos++] = common.getbits_fast(mp, 6);
                            scale[scalePos++] = common.getbits_fast(mp, 6);
                            break;
                        case 1:
                            scale[scalePos++] = sc = common.getbits_fast(mp, 6);
                            scale[scalePos++] = sc;
                            scale[scalePos++] = common.getbits_fast(mp, 6);
                            break;
                        case 2:
                            scale[scalePos++] = sc = common.getbits_fast(mp, 6);
                            scale[scalePos++] = sc;
                            scale[scalePos++] = sc;
                            break;
                        default: // case 3
                            scale[scalePos++] = common.getbits_fast(mp, 6);
                            scale[scalePos++] = sc = common.getbits_fast(mp, 6);
                            scale[scalePos++] = sc;
                            break;
                    }

        }

        private void II_step_two(
            MPGLib.mpstr_tag mp,
            int[] bit_alloc,
            float[][][] fraction,
            int[] scale,
            Frame fr,
            int x1)
        {
            var scalePos = 0;
            int i, j, k, ba;
            var stereo = fr.stereo;
            var sblimit = fr.II_sblimit;
            var jsbound = fr.jsbound;
            int alloc2, alloc1 = 0;
            var bita = 0;
            int d1, step;

            for (i = 0; i < jsbound; i++, alloc1 += 1 << step)
            {
                step = fr.alloc[alloc1].bits;
                for (j = 0; j < stereo; j++)
                {
                    ba = bit_alloc[bita++];
                    if (ba != 0)
                    {
                        k = fr.alloc[alloc2 = alloc1 + ba].bits;
                        if ((d1 = fr.alloc[alloc2].d) < 0)
                        {
                            var cm = common.muls[k][scale[scalePos + x1]];
                            fraction[j][0][i] = (common.getbits(mp, k) + d1) * cm;
                            fraction[j][1][i] = (common.getbits(mp, k) + d1) * cm;
                            fraction[j][2][i] = (common.getbits(mp, k) + d1) * cm;
                        }
                        else
                        {
                            int idx, tab, m = scale[scalePos + x1];
                            idx = common.getbits(mp, k);
                            tab = idx + idx + idx;
                            fraction[j][0][i] = common.muls[table[d1][tab++]][m];
                            fraction[j][1][i] = common.muls[table[d1][tab++]][m];
                            fraction[j][2][i] = common.muls[table[d1][tab]][m];
                        }

                        scalePos += 3;
                    }
                    else
                    {
                        fraction[j][0][i] = fraction[j][1][i] = fraction[j][2][i] = 0.0f;
                    }
                }
            }

            for (i = jsbound; i < sblimit; i++, alloc1 += 1 << step)
            {
                step = fr.alloc[alloc1].bits;
                bita++; // channel 1 and channel 2 bitalloc are the same
                ba = bit_alloc[bita++];
                if (ba != 0)
                {
                    k = fr.alloc[alloc2 = alloc1 + ba].bits;
                    if ((d1 = fr.alloc[alloc2].d) < 0)
                    {
                        float cm;
                        cm = common.muls[k][scale[scalePos + x1 + 3]];
                        fraction[1][0][i] = (fraction[0][0][i] = common.getbits(mp, k) + d1) * cm;
                        fraction[1][1][i] = (fraction[0][1][i] = common.getbits(mp, k) + d1) * cm;
                        fraction[1][2][i] = (fraction[0][2][i] = common.getbits(mp, k) + d1) * cm;
                        cm = common.muls[k][scale[scalePos + x1]];
                        fraction[0][0][i] *= cm;
                        fraction[0][1][i] *= cm;
                        fraction[0][2][i] *= cm;
                    }
                    else
                    {
                        int idx, tab, m1, m2;
                        m1 = scale[scalePos + x1];
                        m2 = scale[scalePos + x1 + 3];
                        idx = common.getbits(mp, k);
                        tab = idx + idx + idx;
                        fraction[0][0][i] = common.muls[table[d1][tab]][m1];
                        fraction[1][0][i] = common.muls[table[d1][tab++]][m2];
                        fraction[0][1][i] = common.muls[table[d1][tab]][m1];
                        fraction[1][1][i] = common.muls[table[d1][tab++]][m2];
                        fraction[0][2][i] = common.muls[table[d1][tab]][m1];
                        fraction[1][2][i] = common.muls[table[d1][tab]][m2];
                    }

                    scalePos += 6;
                }
                else
                {
                    fraction[0][0][i] = fraction[0][1][i] = fraction[0][2][i] =
                        fraction[1][0][i] = fraction[1][1][i] = fraction[1][2][i] = 0.0f;
                }

                /* 
                   should we use individual scalefac for channel 2 or
                   is the current way the right one , where we just copy channel 1 to
                   channel 2 ?? 
                   The current 'strange' thing is, that we throw away the scalefac
                   values for the second channel ...!!
                . changed .. now we use the scalefac values of channel one !! 
                */
            }
            /*  if(sblimit > (fr.down_sample_sblimit) ) */
            /*    sblimit = fr.down_sample_sblimit; */

            for (i = sblimit; i < MPG123.SBLIMIT; i++)
            for (j = 0; j < stereo; j++)
                fraction[j][0][i] = fraction[j][1][i] = fraction[j][2][i] = 0.0f;

        }

        private void II_select_table(Frame fr)
        {
            int table, sblim;

            if (fr.lsf != 0)
                table = 4;
            else
                table = translate[fr.sampling_frequency][2 - fr.stereo][fr.bitrate_index];

            sblim = sblims[table];

            fr.alloc = tables2[table];
            fr.II_sblimit = sblim;
        }

        internal virtual int do_layer2<T>(
            MPGLib.mpstr_tag mp,
            T[] pcm_sample,
            MPGLib.ProcessedBytes pcm_point,
            Interface.ISynth synth,
            Decode.Factory<T> tFactory)
        {
            var clip = 0;
            int i, j;

            var fraction =
                Arrays.ReturnRectangularArray<float>(
                    2,
                    4,
                    MPG123.SBLIMIT); // pick_table clears unused subbands
            var bit_alloc = new int[64];
            var scale = new int[192];
            var fr = mp.fr;
            var stereo = fr.stereo;
            var single = fr.single;

            II_select_table(fr);
            fr.jsbound = fr.mode == MPG123.MPG_MD_JOINT_STEREO ? (fr.mode_ext << 2) + 4 : fr.II_sblimit;

            if (stereo == 1 || single == 3)
                single = 0;

            II_step_one(mp, bit_alloc, scale, fr);

            for (i = 0; i < MPG123.SCALE_BLOCK; i++)
            {
                II_step_two(mp, bit_alloc, fraction, scale, fr, i >> 2);
                for (j = 0; j < 3; j++)
                    if (single >= 0)
                    {
                        clip += synth.synth_1to1_mono_ptr(mp, fraction[single][j], 0, pcm_sample, pcm_point, tFactory);
                    }
                    else
                    {
                        var p1 = new MPGLib.ProcessedBytes();
                        p1.pb = pcm_point.pb;
                        clip += synth.synth_1to1_ptr(mp, fraction[0][j], 0, 0, pcm_sample, p1, tFactory);
                        clip += synth.synth_1to1_ptr(mp, fraction[1][j], 0, 1, pcm_sample, pcm_point, tFactory);
                    }
            }

            return clip;
        }
    }

}