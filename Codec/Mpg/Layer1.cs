//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

/* 
 * layer1.c: Mpeg Layer-1 audio decoder 
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

/* $Id: Layer1.java,v 1.5 2011/05/31 19:42:01 kenchis Exp $ */

using System.Diagnostics;
using GroovyMp3.Types;

namespace GroovyMp3.Codec.Mpg
{

    internal class Layer1
    {
        private Common common;

        private Decode decode;

        internal virtual void setModules(Common c, Decode d)
        {
            common = c;
            decode = d;
        }

        private void I_step_one(MPGLib.mpstr_tag mp, int[] balloc, int[] scale_index, Frame fr)
        {
            var ba = 0;
            var sca = 0;

            Debug.Assert(fr.stereo == 1 || fr.stereo == 2);
            if (fr.stereo == 2)
            {
                int i;
                var jsbound = fr.jsbound;
                for (i = 0; i < jsbound; i++)
                {
                    balloc[ba++] = common.getbits(mp, 4);
                    balloc[ba++] = common.getbits(mp, 4);
                }

                for (i = jsbound; i < MPG123.SBLIMIT; i++)
                    balloc[ba++] = common.getbits(mp, 4);

                ba = 0;

                for (i = 0; i < jsbound; i++)
                {
                    if (balloc[ba]++ != 0)
                        scale_index[sca++] = common.getbits(mp, 6);

                    if (balloc[ba++] != 0)
                        scale_index[sca++] = common.getbits(mp, 6);
                }

                for (i = jsbound; i < MPG123.SBLIMIT; i++)
                    if (balloc[ba++] != 0)
                    {
                        scale_index[sca++] = common.getbits(mp, 6);
                        scale_index[sca++] = common.getbits(mp, 6);
                    }
            }
            else
            {
                int i;
                for (i = 0; i < MPG123.SBLIMIT; i++)
                    balloc[ba++] = common.getbits(mp, 4);

                ba = 0;
                for (i = 0; i < MPG123.SBLIMIT; i++)
                    if (balloc[ba++] != 0)
                        scale_index[sca++] = common.getbits(mp, 6);
            }
        }

        private void I_step_two(MPGLib.mpstr_tag mp, float[][] fraction, int[] balloc, int[] scale_index, Frame fr)
        {
            int i, n;
            var smpb = new int[2 * MPG123.SBLIMIT]; // values: 0-65535
            int sample;
            var ba = 0;
            var sca = 0;

            Debug.Assert(fr.stereo == 1 || fr.stereo == 2);
            if (fr.stereo == 2)
            {
                var jsbound = fr.jsbound;
                var f0 = 0;
                var f1 = 0;
                ba = 0;
                for (sample = 0, i = 0; i < jsbound; i++)
                {
                    n = balloc[ba++];
                    if (n != 0)
                        smpb[sample++] = common.getbits(mp, n + 1);

                    n = balloc[ba++];
                    if (n != 0)
                        smpb[sample++] = common.getbits(mp, n + 1);
                }

                for (i = jsbound; i < MPG123.SBLIMIT; i++)
                {
                    n = balloc[ba++];
                    if (n != 0)
                        smpb[sample++] = common.getbits(mp, n + 1);
                }

                ba = 0;
                for (sample = 0, i = 0; i < jsbound; i++)
                {
                    n = balloc[ba++];
                    if (n != 0)
                        fraction[0][f0++] = ((-1 << n) + smpb[sample++] + 1) *
                                            common.muls[n + 1][scale_index[sca++]];
                    else
                        fraction[0][f0++] = 0.0f;

                    n = balloc[ba++];
                    if (n != 0)
                        fraction[1][f1++] = ((-1 << n) + smpb[sample++] + 1) *
                                            common.muls[n + 1][scale_index[sca++]];
                    else
                        fraction[1][f1++] = 0.0f;
                }

                for (i = jsbound; i < MPG123.SBLIMIT; i++)
                {
                    n = balloc[ba++];
                    if (n != 0)
                    {
                        var samp = (float)((-1 << n) + smpb[sample++] + 1);
                        fraction[0][f0++] = samp * common.muls[n + 1][scale_index[sca++]];
                        fraction[1][f1++] = samp * common.muls[n + 1][scale_index[sca++]];
                    }
                    else
                    {
                        fraction[0][f0++] = fraction[1][f1++] = 0.0f;
                    }
                }

                for (i = fr.down_sample_sblimit; i < 32; i++)
                    fraction[0][i] = fraction[1][i] = 0.0f;
            }
            else
            {
                var f0 = 0;
                ba = 0;
                for (sample = 0, i = 0; i < MPG123.SBLIMIT; i++)
                {
                    n = balloc[ba++];
                    if (n != 0)
                        smpb[sample++] = common.getbits(mp, n + 1);
                }

                ba = 0;
                for (sample = 0, i = 0; i < MPG123.SBLIMIT; i++)
                {
                    n = balloc[ba++];
                    if (n != 0)
                        fraction[0][f0++] = ((-1 << n) + smpb[sample++] + 1) *
                                            common.muls[n + 1][scale_index[sca++]];
                    else
                        fraction[0][f0++] = 0.0f;
                }

                for (i = fr.down_sample_sblimit; i < 32; i++)
                    fraction[0][i] = 0.0f;
            }
        }

        internal virtual int do_layer1<T>(
            MPGLib.mpstr_tag mp,
            T[] pcm_sample,
            MPGLib.ProcessedBytes pcm_point,
            Decode.Factory<T> tFactory)
        {
            var clip = 0;
            var balloc = new int[2 * MPG123.SBLIMIT];
            var scale_index = new int[2 * MPG123.SBLIMIT];

            var fraction = Arrays.ReturnRectangularArray<float>(2, MPG123.SBLIMIT);
            var fr = mp.fr;
            int i, stereo = fr.stereo;
            var single = fr.single;

            fr.jsbound = fr.mode == MPG123.MPG_MD_JOINT_STEREO ? (fr.mode_ext << 2) + 4 : 32;

            if (stereo == 1 || single == 3)
                single = 0;

            I_step_one(mp, balloc, scale_index, fr);

            for (i = 0; i < MPG123.SCALE_BLOCK; i++)
            {
                I_step_two(mp, fraction, balloc, scale_index, fr);

                if (single >= 0)
                {
                    clip += decode.synth_1to1_mono(mp, fraction[single], 0, pcm_sample, pcm_point, tFactory);
                }
                else
                {
                    var p1 = new MPGLib.ProcessedBytes();
                    p1.pb = pcm_point.pb;
                    clip += decode.synth_1to1(mp, fraction[0], 0, 0, pcm_sample, p1, tFactory);
                    clip += decode.synth_1to1(mp, fraction[1], 0, 1, pcm_sample, pcm_point, tFactory);
                }
            }

            return clip;
        }
    }

}