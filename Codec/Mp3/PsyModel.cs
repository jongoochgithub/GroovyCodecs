using System;
using System.Diagnostics;
using GroovyMp3.Types;

/*  *      psymodel.c  *
 *      Copyright (c) 1999-2000 Mark Taylor
 * *      Copyright (c) 2001-2002 Naoki Shibata
 * *      Copyright (c) 2000-2003 Takehiro Tominaga
 * *      Copyright (c) 2000-2008 Robert Hegemann
 * *      Copyright (c) 2000-2005 Gabriel Bouvigne
 * *      Copyright (c) 2000-2005 Alexander Leidinger
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

/* $Id: PsyModel.java,v 1.27 2011/05/24 20:48:06 kenchis Exp $ */

/* PSYCHO ACOUSTICS   This routine computes the psycho acoustics, delayed by one granule.  
Input: buffer of PCM data (1024 samples).  
This window should be centered over the 576 sample granule window. The routine will compute the psycho acoustics for this granule, but return the psycho acoustics computed for the *previous* granule.  This is because the block type of the previous granule can only be determined after we have computed the psycho acoustics for the following granule.  
Output:  maskings and energies for each scalefactor band. block type, PE, and some correlation measures.  The PE is used by CBR modes to determine if extra bits from the bit reservoir should be used.  The correlation measures are used to determine mid/side or regular stereo. */ /* Notation:  barks:  a non-linear frequency scale.  Mapping from frequency to         barks is given by freq2bark()  scalefactor bands: The spectrum (frequencies) are broken into                    SBMAX "scalefactor bands".  Thes bands                    are determined by the MPEG ISO spec.  In                    the noise shaping/quantization code, we allocate                    bits among the partition bands to achieve the                    best possible quality  partition bands:   The spectrum is also broken into about                    64 "partition bands".  Each partition                    band is about .34 barks wide.  There are about 2-5                    partition bands for each scalefactor band.  LAME computes all psycho acoustic information for each partition band.  Then at the end of the computations, this information is mapped to scalefactor bands.  The energy in each scalefactor band is taken as the sum of the energy in all partition bands which overlap the scalefactor band.  The maskings can be computed in the same way (and thus represent the average masking in that band) or by taking the minmum value multiplied by the number of partition bands used (which represents a minimum masking in that band). */ /* The general outline is as follows:  1. compute the energy in each partition band 2. compute the tonality in each partition band 3. compute the strength of each partion band "masker" 4. compute the masking (via the spreading function applied to each masker) 5. Modifications for mid/side masking.  
Each partition band is considiered a "masker".  The strength of the i'th masker in band j is given by:      s3(bark(i)-bark(j))*strength(i)  The strength of the masker is a function of the energy and tonality. The more tonal, the less masking.  LAME uses a simple linear formula (controlled by NMT and TMN) which says the strength is given by the energy divided by a linear function of the tonality. */ /* s3() is the "spreading function".  It is given by a formula determined via listening tests.  
The total masking in the j'th partition band is the sum over all maskings i.  It is thus given by the convolution of the strength with s3(), the "spreading function."  masking(j) = sum_over_i  s3(i-j)*strength(i)  = s3 o strength  where "o" = convolution operator.  s3 is given by a formula determined via listening tests.  It is normalized so that s3 o 1 = 1.  Note: instead of a simple convolution, LAME also has the option of using "additive masking"  The most critical part is step 2, computing the tonality of each partition band.  LAME has two tonality estimators.  The first is based on the ISO spec, and measures how predictiable the signal is over time.  The more predictable, the more tonal. The second measure is based on looking at the spectrum of a single granule.  The more peaky the spectrum, the more tonal.  By most indications, the latter approach is better.  Finally, in step 5, the maskings for the mid and side channel are possibly increased.  Under certain circumstances, noise in the mid & side channels is assumed to also be masked by strong maskers in the L or R channels.   Other data computed by the psy-model:  ms_ratio        side-channel / mid-channel masking ratio (for previous granule) ms_ratio_next   side-channel / mid-channel masking ratio for this granule  percep_entropy[2]     L and R values (prev granule) of PE - A measure of how                       much pre-echo is in the previous granule percep_entropy_MS[2]  mid and side channel values (prev granule) of percep_entropy energy[4]             L,R,M,S energy in each channel, prev granule blocktype_d[2]        block type to use for previous granule */
namespace GroovyMp3.Codec.Mp3
{
    internal class PsyModel
    {

        private const float DELBARK = .34f;

        private static readonly float[] fircoef =
        {
            -8.65163e-18f * 2,
            -0.00851586f * 2,
            -6.74764e-18f * 2,
            0.0209036f * 2,
            -3.36639e-17f * 2,
            -0.0438162f * 2,
            -1.54175e-17f * 2,
            0.0931738f * 2,
            -5.52212e-17f * 2,
            -0.313819f * 2
        };

        private static readonly float[] fircoef_ =
        {
            -8.65163e-18f * 2,
            -0.00851586f * 2,
            -6.74764e-18f * 2,
            0.0209036f * 2,
            -3.36639e-17f * 2,
            -0.0438162f * 2,
            -1.54175e-17f * 2,
            0.0931738f * 2,
            -5.52212e-17f * 2,
            -0.313819f * 2
        };

        private const int I1LIMIT = 8;

        private const int I2LIMIT = 23;

        private const float LN_TO_LOG10 = 0.2302585093f;

        private const float LOG10 = 2.30258509299404568402f;

        private const int MLIMIT = 15;

        private const float NS_MSFIX = 3.5f;

        private const float NS_PREECHO_ATT0 = 0.8f;

        private const float NS_PREECHO_ATT1 = 0.6f;

        private const float NS_PREECHO_ATT2 = 0.3f;

        internal const float NSATTACKTHRE = 4.4f;

        internal const int NSATTACKTHRE_S = 25;

        private const int NSFIRLEN = 21;

        private static readonly float[] regcoef_l =
        {
            6.8f,
            5.8f,
            5.8f,
            6.4f,
            6.5f,
            9.9f,
            12.1f,
            14.4f,
            15f,
            18.9f,
            21.6f,
            26.9f,
            34.2f,
            40.2f,
            46.8f,
            56.5f,
            60.7f,
            73.9f,
            85.7f,
            93.4f,
            126.1f
        };

        private static readonly float[] regcoef_s =
        {
            11.8f,
            13.6f,
            17.2f,
            32f,
            46.5f,
            51.3f,
            57.5f,
            67.1f,
            71.5f,
            84.6f,
            97.6f,
            130f
        };

        private const int rpelev = 2;

        private const int rpelev_s = 2;

        private const int rpelev2 = 16;

        private const int rpelev2_s = 16;

        private static readonly float[] tab =
        {
            1.0f,
            0.79433f,
            0.63096f,
            0.63096f,
            0.63096f,
            0.63096f,
            0.63096f,
            0.25119f,
            0.11749f
        };

        private static readonly float[] table1 =
        {
            3.3246f * 3.3246f,
            3.23837f * 3.23837f,
            3.15437f * 3.15437f,
            3.00412f * 3.00412f,
            2.86103f * 2.86103f,
            2.65407f * 2.65407f,
            2.46209f * 2.46209f,
            2.284f * 2.284f,
            2.11879f * 2.11879f,
            1.96552f * 1.96552f,
            1.82335f * 1.82335f,
            1.69146f * 1.69146f,
            1.56911f * 1.56911f,
            1.46658f * 1.46658f,
            1.37074f * 1.37074f,
            1.31036f * 1.31036f,
            1.25264f * 1.25264f,
            1.20648f * 1.20648f,
            1.16203f * 1.16203f,
            1.12765f * 1.12765f,
            1.09428f * 1.09428f,
            1.0659f * 1.0659f,
            1.03826f * 1.03826f,
            1.01895f * 1.01895f,
            1
        };

        private static readonly float[] table2 =
        {
            1.33352f * 1.33352f,
            1.35879f * 1.35879f,
            1.38454f * 1.38454f,
            1.39497f * 1.39497f,
            1.40548f * 1.40548f,
            1.3537f * 1.3537f,
            1.30382f * 1.30382f,
            1.22321f * 1.22321f,
            1.14758f * 1.14758f,
            1
        };

        private static readonly float[] table2_ =
        {
            1.33352f * 1.33352f,
            1.35879f * 1.35879f,
            1.38454f * 1.38454f,
            1.39497f * 1.39497f,
            1.40548f * 1.40548f,
            1.3537f * 1.3537f,
            1.30382f * 1.30382f,
            1.22321f * 1.22321f,
            1.14758f * 1.14758f,
            1
        };

        private static readonly float[] table3 =
        {
            2.35364f * 2.35364f,
            2.29259f * 2.29259f,
            2.23313f * 2.23313f,
            2.12675f * 2.12675f,
            2.02545f * 2.02545f,
            1.87894f * 1.87894f,
            1.74303f * 1.74303f,
            1.61695f * 1.61695f,
            1.49999f * 1.49999f,
            1.39148f * 1.39148f,
            1.29083f * 1.29083f,
            1.19746f * 1.19746f,
            1.11084f * 1.11084f,
            1.03826f * 1.03826f
        };

        private const float temporalmask_sustain_sec = 0.01f;

        private static readonly float VO_SCALE = 1.0f / (14752 * 14752) / (Encoder.BLKSIZE / 2);

        private readonly FFT fft = new FFT();

        private float ma_max_i1;

        private float ma_max_i2;

        private float ma_max_m;

        private static float NON_LINEAR_SCALE_ENERGY(float x)
        {
            return x;
        }

        private float psycho_loudness_approx(float[] energy, LameInternalFlags gfc)
        {
            var loudness_power = 0.0f;
            for (var i = 0; i < Encoder.BLKSIZE / 2; ++i)
                loudness_power += energy[i] * gfc.ATH.eql_w[i];

            loudness_power *= VO_SCALE;
            return loudness_power;
        }

        private void compute_ffts(
            LameGlobalFlags gfp,
            float[] fftenergy,
            float[][] fftenergy_s,
            float[][] wsamp_l,
            int wsamp_lPos,
            float[][][] wsamp_s,
            int wsamp_sPos,
            int gr_out,
            int chn,
            float[][] buffer,
            int bufPos)
        {

            var gfc = gfp.internal_flags;
            if (chn < 2)
            {
                fft.fft_long(gfc, wsamp_l[wsamp_lPos], chn, buffer, bufPos);
                fft.fft_short(gfc, wsamp_s[wsamp_sPos], chn, buffer, bufPos);
            }
            else if (chn == 2)
            {
                for (var j = Encoder.BLKSIZE - 1; j >= 0; --j)
                {

                    var l = wsamp_l[wsamp_lPos + 0][j];

                    var r = wsamp_l[wsamp_lPos + 1][j];
                    wsamp_l[wsamp_lPos + 0][j] = (l + r) * Util.SQRT2 * 0.5f;
                    wsamp_l[wsamp_lPos + 1][j] = (l - r) * Util.SQRT2 * 0.5f;
                }

                for (var b = 2; b >= 0; --b)
                for (var j = Encoder.BLKSIZE_s - 1; j >= 0; --j)
                {

                    var l = wsamp_s[wsamp_sPos + 0][b][j];

                    var r = wsamp_s[wsamp_sPos + 1][b][j];
                    wsamp_s[wsamp_sPos + 0][b][j] = (l + r) * Util.SQRT2 * 0.5f;
                    wsamp_s[wsamp_sPos + 1][b][j] = (l - r) * Util.SQRT2 * 0.5f;
                }
            }

            fftenergy[0] = NON_LINEAR_SCALE_ENERGY(wsamp_l[wsamp_lPos + 0][0]);
            fftenergy[0] *= fftenergy[0];
            for (var j = Encoder.BLKSIZE / 2 - 1; j >= 0; --j)
            {

                var re = wsamp_l[wsamp_lPos + 0][Encoder.BLKSIZE / 2 - j];

                var im = wsamp_l[wsamp_lPos + 0][Encoder.BLKSIZE / 2 + j];
                fftenergy[Encoder.BLKSIZE / 2 - j] = NON_LINEAR_SCALE_ENERGY((re * re + im * im) * 0.5f);
            }

            for (var b = 2; b >= 0; --b)
            {
                fftenergy_s[b][0] = wsamp_s[wsamp_sPos + 0][b][0];
                fftenergy_s[b][0] *= fftenergy_s[b][0];
                for (var j = Encoder.BLKSIZE_s / 2 - 1; j >= 0; --j)
                {

                    var re = wsamp_s[wsamp_sPos + 0][b][Encoder.BLKSIZE_s / 2 - j];

                    var im = wsamp_s[wsamp_sPos + 0][b][Encoder.BLKSIZE_s / 2 + j];
                    fftenergy_s[b][Encoder.BLKSIZE_s / 2 - j] = NON_LINEAR_SCALE_ENERGY((re * re + im * im) * 0.5f);
                }
            }

            {
                var totalenergy = 0.0f;
                for (var j = 11; j < Encoder.HBLKSIZE; j++)
                    totalenergy += fftenergy[j];

                gfc.tot_ener[chn] = totalenergy;
            }
            if (gfp.analysis)
            {
                for (var j = 0; j < Encoder.HBLKSIZE; j++)
                {
                    gfc.pinfo.energy[gr_out][chn][j] = gfc.pinfo.energy_save[chn][j];
                    gfc.pinfo.energy_save[chn][j] = fftenergy[j];
                }

                gfc.pinfo.pe[gr_out][chn] = gfc.pe[chn];
            }

            if (gfp.athaa_loudapprox == 2 && chn < 2)
            {
                gfc.loudness_sq[gr_out][chn] = gfc.loudness_sq_save[chn];
                gfc.loudness_sq_save[chn] = psycho_loudness_approx(fftenergy, gfc);
            }
        }

        private void init_mask_add_max_values()
        {
            ma_max_i1 = (float)Math.Pow(10, (I1LIMIT + 1) / 16.0);
            ma_max_i2 = (float)Math.Pow(10, (I2LIMIT + 1) / 16.0);
            ma_max_m = (float)Math.Pow(10, MLIMIT / 10.0);
        }

        private float mask_add(float m1, float m2, int kk, int b, LameInternalFlags gfc, int shortblock)
        {
            float ratio;
            if (m2 > m1)
            {
                if (m2 < m1 * ma_max_i2)
                    ratio = m2 / m1;
                else
                    return m1 + m2;
            }
            else
            {
                if (m1 >= m2 * ma_max_i2)
                    return m1 + m2;

                ratio = m1 / m2;
            }

            Debug.Assert(m1 >= 0);
            Debug.Assert(m2 >= 0);
            m1 += m2;
            if (((b + 3) & 0xffffffffL) <= 3 + 3)
            {
                if (ratio >= ma_max_i1)
                    return m1;

                var ii = (int)Util.FAST_LOG10_X(ratio, 16.0f);
                return m1 * table2[ii];
            }

            var i = (int)Util.FAST_LOG10_X(ratio, 16.0f);
            if (shortblock != 0)
                m2 = gfc.ATH.cb_s[kk] * gfc.ATH.adjust;
            else
                m2 = gfc.ATH.cb_l[kk] * gfc.ATH.adjust;

            Debug.Assert(m2 >= 0);
            if (m1 < ma_max_m * m2)
            {
                if (m1 > m2)
                {
                    float f, r;
                    f = 1.0f;
                    if (i <= 13)
                        f = table3[i];

                    r = Util.FAST_LOG10_X(m1 / m2, 10.0f / 15.0f);
                    return m1 * ((table1[i] - f) * r + f);
                }

                if (i > 13)
                    return m1;

                return m1 * table3[i];
            }

            return m1 * table1[i];
        }

        private float vbrpsy_mask_add(float m1, float m2, int b)
        {
            float ratio;
            if (m1 < 0)
                m1 = 0;

            if (m2 < 0)
                m2 = 0;

            if (m1 <= 0)
                return m2;

            if (m2 <= 0)
                return m1;

            if (m2 > m1)
                ratio = m2 / m1;
            else
                ratio = m1 / m2;

            if (-2 <= b && b <= 2)
                if (ratio >= ma_max_i1)
                {
                    return m1 + m2;
                }
                else
                {
                    var i = (int)Util.FAST_LOG10_X(ratio, 16.0f);
                    return (m1 + m2) * table2_[i];
                }

            if (ratio < ma_max_i2)
                return m1 + m2;

            if (m1 < m2)
                m1 = m2;

            return m1;
        }

        private void calc_interchannel_masking(LameGlobalFlags gfp, float ratio)
        {

            var gfc = gfp.internal_flags;
            if (gfc.channels_out > 1)
            {
                for (var sb = 0; sb < Encoder.SBMAX_l; sb++)
                {
                    var l = gfc.thm[0].l[sb];
                    var r = gfc.thm[1].l[sb];
                    gfc.thm[0].l[sb] += r * ratio;
                    gfc.thm[1].l[sb] += l * ratio;
                }

                for (var sb = 0; sb < Encoder.SBMAX_s; sb++)
                for (var sblock = 0; sblock < 3; sblock++)
                {
                    var l = gfc.thm[0].s[sb][sblock];
                    var r = gfc.thm[1].s[sb][sblock];
                    gfc.thm[0].s[sb][sblock] += r * ratio;
                    gfc.thm[1].s[sb][sblock] += l * ratio;
                }
            }
        }

        private void msfix1(LameInternalFlags gfc)
        {
            for (var sb = 0; sb < Encoder.SBMAX_l; sb++)
            {
                if (gfc.thm[0].l[sb] > 1.58 * gfc.thm[1].l[sb] || gfc.thm[1].l[sb] > 1.58 * gfc.thm[0].l[sb])
                    continue;

                var mld = gfc.mld_l[sb] * gfc.en[3].l[sb];
                var rmid = Math.Max(gfc.thm[2].l[sb], Math.Min(gfc.thm[3].l[sb], mld));
                mld = gfc.mld_l[sb] * gfc.en[2].l[sb];
                var rside = Math.Max(gfc.thm[3].l[sb], Math.Min(gfc.thm[2].l[sb], mld));
                gfc.thm[2].l[sb] = rmid;
                gfc.thm[3].l[sb] = rside;
            }

            for (var sb = 0; sb < Encoder.SBMAX_s; sb++)
            for (var sblock = 0; sblock < 3; sblock++)
            {
                if (gfc.thm[0].s[sb][sblock] > 1.58 * gfc.thm[1].s[sb][sblock] ||
                    gfc.thm[1].s[sb][sblock] > 1.58 * gfc.thm[0].s[sb][sblock])
                    continue;

                var mld = gfc.mld_s[sb] * gfc.en[3].s[sb][sblock];
                var rmid = Math.Max(gfc.thm[2].s[sb][sblock], Math.Min(gfc.thm[3].s[sb][sblock], mld));
                mld = gfc.mld_s[sb] * gfc.en[2].s[sb][sblock];
                var rside = Math.Max(gfc.thm[3].s[sb][sblock], Math.Min(gfc.thm[2].s[sb][sblock], mld));
                gfc.thm[2].s[sb][sblock] = rmid;
                gfc.thm[3].s[sb][sblock] = rside;
            }
        }

        private void ns_msfix(LameInternalFlags gfc, float msfix, float athadjust)
        {
            var msfix2 = msfix;
            var athlower = (float)Math.Pow(10, athadjust);
            msfix *= 2.0f;
            msfix2 *= 2.0f;
            for (var sb = 0; sb < Encoder.SBMAX_l; sb++)
            {
                float thmLR, thmM, thmS, ath;
                ath = gfc.ATH.cb_l[gfc.bm_l[sb]] * athlower;
                thmLR = Math.Min(Math.Max(gfc.thm[0].l[sb], ath), Math.Max(gfc.thm[1].l[sb], ath));
                thmM = Math.Max(gfc.thm[2].l[sb], ath);
                thmS = Math.Max(gfc.thm[3].l[sb], ath);
                if (thmLR * msfix < thmM + thmS)
                {
                    var f = thmLR * msfix2 / (thmM + thmS);
                    thmM *= f;
                    thmS *= f;
                    Debug.Assert(thmM + thmS > 0);
                }

                gfc.thm[2].l[sb] = Math.Min(thmM, gfc.thm[2].l[sb]);
                gfc.thm[3].l[sb] = Math.Min(thmS, gfc.thm[3].l[sb]);
            }

            athlower *= (float)Encoder.BLKSIZE_s / Encoder.BLKSIZE;
            for (var sb = 0; sb < Encoder.SBMAX_s; sb++)
            for (var sblock = 0; sblock < 3; sblock++)
            {
                float thmLR, thmM, thmS, ath;
                ath = gfc.ATH.cb_s[gfc.bm_s[sb]] * athlower;
                thmLR = Math.Min(Math.Max(gfc.thm[0].s[sb][sblock], ath), Math.Max(gfc.thm[1].s[sb][sblock], ath));
                thmM = Math.Max(gfc.thm[2].s[sb][sblock], ath);
                thmS = Math.Max(gfc.thm[3].s[sb][sblock], ath);
                if (thmLR * msfix < thmM + thmS)
                {
                    var f = thmLR * msfix / (thmM + thmS);
                    thmM *= f;
                    thmS *= f;
                    Debug.Assert(thmM + thmS > 0);
                }

                gfc.thm[2].s[sb][sblock] = Math.Min(gfc.thm[2].s[sb][sblock], thmM);
                gfc.thm[3].s[sb][sblock] = Math.Min(gfc.thm[3].s[sb][sblock], thmS);
            }
        }

        private void convert_partition2scalefac_s(LameInternalFlags gfc, float[] eb, float[] thr, int chn, int sblock)
        {
            int sb, b;
            var enn = 0.0f;
            var thmm = 0.0f;
            for (sb = b = 0; sb < Encoder.SBMAX_s; ++b, ++sb)
            {
                var bo_s_sb = gfc.bo_s[sb];
                var npart_s = gfc.npart_s;
                var b_lim = bo_s_sb < npart_s ? bo_s_sb : npart_s;
                while (b < b_lim)
                {
                    Debug.Assert(eb[b] >= 0);
                    Debug.Assert(thr[b] >= 0);
                    enn += eb[b];
                    thmm += thr[b];
                    b++;
                }

                gfc.en[chn].s[sb][sblock] = enn;
                gfc.thm[chn].s[sb][sblock] = thmm;
                if (b >= npart_s)
                {
                    ++sb;
                    break;
                }

                Debug.Assert(eb[b] >= 0);
                Debug.Assert(thr[b] >= 0);
                {
                    var w_curr = gfc.PSY.bo_s_weight[sb];
                    var w_next = 1.0f - w_curr;
                    enn = w_curr * eb[b];
                    thmm = w_curr * thr[b];
                    gfc.en[chn].s[sb][sblock] += enn;
                    gfc.thm[chn].s[sb][sblock] += thmm;
                    enn = w_next * eb[b];
                    thmm = w_next * thr[b];
                }
            }

            for (; sb < Encoder.SBMAX_s; ++sb)
            {
                gfc.en[chn].s[sb][sblock] = 0;
                gfc.thm[chn].s[sb][sblock] = 0;
            }
        }

        private void convert_partition2scalefac_l(LameInternalFlags gfc, float[] eb, float[] thr, int chn)
        {
            int sb, b;
            var enn = 0.0f;
            var thmm = 0.0f;
            for (sb = b = 0; sb < Encoder.SBMAX_l; ++b, ++sb)
            {
                var bo_l_sb = gfc.bo_l[sb];
                var npart_l = gfc.npart_l;
                var b_lim = bo_l_sb < npart_l ? bo_l_sb : npart_l;
                while (b < b_lim)
                {
                    Debug.Assert(eb[b] >= 0);
                    Debug.Assert(thr[b] >= 0);
                    enn += eb[b];
                    thmm += thr[b];
                    b++;
                }

                gfc.en[chn].l[sb] = enn;
                gfc.thm[chn].l[sb] = thmm;
                if (b >= npart_l)
                {
                    ++sb;
                    break;
                }

                Debug.Assert(eb[b] >= 0);
                Debug.Assert(thr[b] >= 0);
                {
                    var w_curr = gfc.PSY.bo_l_weight[sb];
                    var w_next = 1.0f - w_curr;
                    enn = w_curr * eb[b];
                    thmm = w_curr * thr[b];
                    gfc.en[chn].l[sb] += enn;
                    gfc.thm[chn].l[sb] += thmm;
                    enn = w_next * eb[b];
                    thmm = w_next * thr[b];
                }
            }

            for (; sb < Encoder.SBMAX_l; ++sb)
            {
                gfc.en[chn].l[sb] = 0;
                gfc.thm[chn].l[sb] = 0;
            }
        }

        private void compute_masking_s(
            LameGlobalFlags gfp,
            float[][] fftenergy_s,
            float[] eb,
            float[] thr,
            int chn,
            int sblock)
        {

            var gfc = gfp.internal_flags;
            int j, b;
            for (b = j = 0; b < gfc.npart_s; ++b)
            {
                float ebb = 0, m = 0;
                var n = gfc.numlines_s[b];
                for (var i = 0; i < n; ++i, ++j)
                {
                    var el = fftenergy_s[sblock][j];
                    ebb += el;
                    if (m < el)
                        m = el;
                }

                eb[b] = ebb;
            }

            Debug.Assert(b == gfc.npart_s);
            Debug.Assert(j == 129);
            for (j = b = 0; b < gfc.npart_s; b++)
            {
                var kk = gfc.s3ind_s[b][0];
                var ecb = gfc.s3_ss[j++] * eb[kk];
                ++kk;
                while (kk <= gfc.s3ind_s[b][1])
                {
                    ecb += gfc.s3_ss[j] * eb[kk];
                    ++j;
                    ++kk;
                }

                {
                    var x = rpelev_s * gfc.nb_s1[chn][b];
                    thr[b] = Math.Min(ecb, x);
                }
                if (gfc.blocktype_old[chn & 1] == Encoder.SHORT_TYPE)
                {
                    var x = rpelev2_s * gfc.nb_s2[chn][b];
                    var y = thr[b];
                    thr[b] = Math.Min(x, y);
                }

                gfc.nb_s2[chn][b] = gfc.nb_s1[chn][b];
                gfc.nb_s1[chn][b] = ecb;
                Debug.Assert(thr[b] >= 0);
            }

            for (; b <= Encoder.CBANDS; ++b)
            {
                eb[b] = 0;
                thr[b] = 0;
            }
        }

        private void block_type_set(LameGlobalFlags gfp, int[] uselongblock, int[] blocktype_d, int[] blocktype)
        {

            var gfc = gfp.internal_flags;
            if (gfp.short_blocks == ShortBlock.short_block_coupled && !(uselongblock[0] != 0 && uselongblock[1] != 0))
                uselongblock[0] = uselongblock[1] = 0;

            for (var chn = 0; chn < gfc.channels_out; chn++)
            {
                blocktype[chn] = Encoder.NORM_TYPE;
                if (gfp.short_blocks == ShortBlock.short_block_dispensed)
                    uselongblock[chn] = 1;

                if (gfp.short_blocks == ShortBlock.short_block_forced)
                    uselongblock[chn] = 0;

                if (uselongblock[chn] != 0)
                {
                    Debug.Assert(gfc.blocktype_old[chn] != Encoder.START_TYPE);
                    if (gfc.blocktype_old[chn] == Encoder.SHORT_TYPE)
                        blocktype[chn] = Encoder.STOP_TYPE;
                }
                else
                {
                    blocktype[chn] = Encoder.SHORT_TYPE;
                    if (gfc.blocktype_old[chn] == Encoder.NORM_TYPE)
                        gfc.blocktype_old[chn] = Encoder.START_TYPE;

                    if (gfc.blocktype_old[chn] == Encoder.STOP_TYPE)
                        gfc.blocktype_old[chn] = Encoder.SHORT_TYPE;
                }

                blocktype_d[chn] = gfc.blocktype_old[chn];
                gfc.blocktype_old[chn] = blocktype[chn];
            }
        }

        private float NS_INTERP(float x, float y, float r)
        {
            if (r >= 1.0)
                return x;

            if (r <= 0.0)
                return y;

            if (y > 0.0)
                return (float)(Math.Pow(x / y, r) * y);

            return 0.0f;
        }

        private float pecalc_s(III_psy_ratio mr, float masking_lower)
        {
            var pe_s = 1236.28f / 4;
            for (var sb = 0; sb < Encoder.SBMAX_s - 1; sb++)
            for (var sblock = 0; sblock < 3; sblock++)
            {
                var thm = mr.thm.s[sb][sblock];
                Debug.Assert(sb < regcoef_s.Length);
                if (thm > 0.0)
                {
                    var x = thm * masking_lower;
                    var en = mr.en.s[sb][sblock];
                    if (en > x)
                        if (en > x * 1e10)
                        {
                            pe_s += regcoef_s[sb] * (10.0f * LOG10);
                        }
                        else
                        {
                            Debug.Assert(x > 0);
                            pe_s += regcoef_s[sb] * Util.FAST_LOG10(en / x);
                        }
                }
            }

            return pe_s;
        }

        private float pecalc_l(III_psy_ratio mr, float masking_lower)
        {
            var pe_l = 1124.23f / 4;
            for (var sb = 0; sb < Encoder.SBMAX_l - 1; sb++)
            {
                var thm = mr.thm.l[sb];
                Debug.Assert(sb < regcoef_l.Length);
                if (thm > 0.0)
                {
                    var x = thm * masking_lower;
                    var en = mr.en.l[sb];
                    if (en > x)
                        if (en > x * 1e10)
                        {
                            pe_l += regcoef_l[sb] * (10.0f * LOG10);
                        }
                        else
                        {
                            Debug.Assert(x > 0);
                            pe_l += regcoef_l[sb] * Util.FAST_LOG10(en / x);
                        }
                }
            }

            return pe_l;
        }

        private void calc_energy(LameInternalFlags gfc, float[] fftenergy, float[] eb, float[] max, float[] avg)
        {
            int b, j;
            for (b = j = 0; b < gfc.npart_l; ++b)
            {
                float ebb = 0, m = 0;
                int i;
                for (i = 0; i < gfc.numlines_l[b]; ++i, ++j)
                {
                    var el = fftenergy[j];
                    Debug.Assert(el >= 0);
                    ebb += el;
                    if (m < el)
                        m = el;
                }

                eb[b] = ebb;
                max[b] = m;
                avg[b] = ebb * gfc.rnumlines_l[b];
                Debug.Assert(gfc.rnumlines_l[b] >= 0);
                Debug.Assert(ebb >= 0);
                Debug.Assert(eb[b] >= 0);
                Debug.Assert(max[b] >= 0);
                Debug.Assert(avg[b] >= 0);
            }
        }

        private void calc_mask_index_l(LameInternalFlags gfc, float[] max, float[] avg, int[] mask_idx)
        {
            var last_tab_entry = tab.Length - 1;
            var b = 0;
            var a = avg[b] + avg[b + 1];
            Debug.Assert(a >= 0);
            if (a > 0.0)
            {
                var m = max[b];
                if (m < max[b + 1])
                    m = max[b + 1];

                Debug.Assert(gfc.numlines_l[b] + gfc.numlines_l[b + 1] - 1 > 0);
                a = 20.0f * (m * 2.0f - a) / (a * (gfc.numlines_l[b] + gfc.numlines_l[b + 1] - 1));
                var k = (int)a;
                if (k > last_tab_entry)
                    k = last_tab_entry;

                mask_idx[b] = k;
            }
            else
            {
                mask_idx[b] = 0;
            }

            for (b = 1; b < gfc.npart_l - 1; b++)
            {
                a = avg[b - 1] + avg[b] + avg[b + 1];
                Debug.Assert(a >= 0);
                if (a > 0.0)
                {
                    var m = max[b - 1];
                    if (m < max[b])
                        m = max[b];

                    if (m < max[b + 1])
                        m = max[b + 1];

                    Debug.Assert(gfc.numlines_l[b - 1] + gfc.numlines_l[b] + gfc.numlines_l[b + 1] - 1 > 0);
                    a = 20.0f * (m * 3.0f - a) /
                        (a * (gfc.numlines_l[b - 1] + gfc.numlines_l[b] + gfc.numlines_l[b + 1] - 1));
                    var k = (int)a;
                    if (k > last_tab_entry)
                        k = last_tab_entry;

                    mask_idx[b] = k;
                }
                else
                {
                    mask_idx[b] = 0;
                }
            }

            Debug.Assert(b > 0);
            Debug.Assert(b == gfc.npart_l - 1);
            a = avg[b - 1] + avg[b];
            Debug.Assert(a >= 0);
            if (a > 0.0)
            {
                var m = max[b - 1];
                if (m < max[b])
                    m = max[b];

                Debug.Assert(gfc.numlines_l[b - 1] + gfc.numlines_l[b] - 1 > 0);
                a = 20.0f * (m * 2.0f - a) / (a * (gfc.numlines_l[b - 1] + gfc.numlines_l[b] - 1));
                var k = (int)a;
                if (k > last_tab_entry)
                    k = last_tab_entry;

                mask_idx[b] = k;
            }
            else
            {
                mask_idx[b] = 0;
            }

            Debug.Assert(b == gfc.npart_l - 1);
        }

        internal int L3psycho_anal_ns(
            LameGlobalFlags gfp,
            float[][] buffer,
            int bufPos,
            int gr_out,
            III_psy_ratio[][] masking_ratio,
            III_psy_ratio[][] masking_MS_ratio,
            float[] percep_entropy,
            float[] percep_MS_entropy,
            float[] energy,
            int[] blocktype_d)
        {

            var gfc = gfp.internal_flags;

            var wsamp_L = Arrays.ReturnRectangularArray<float>(2, Encoder.BLKSIZE);

            var wsamp_S = Arrays.ReturnRectangularArray<float>(2, 3, Encoder.BLKSIZE_s);
            var eb_l = new float[Encoder.CBANDS + 1];
            var eb_s = new float[Encoder.CBANDS + 1];
            var thr = new float[Encoder.CBANDS + 2];
            var blocktype = new int[2];
            var uselongblock = new int[2];
            int numchn, chn;
            int b, i, j, k;
            int sb, sblock;

            var ns_hpfsmpl = Arrays.ReturnRectangularArray<float>(2, 576);
            float pcfact;
            var mask_idx_l = new int[Encoder.CBANDS + 2];
            var mask_idx_s = new int[Encoder.CBANDS + 2];
            Arrays.Fill(mask_idx_s, 0);
            numchn = gfc.channels_out;
            if (gfp.mode == MPEGMode.JOINT_STEREO)
                numchn = 4;

            if (gfp.VBR == VbrMode.vbr_off)
                pcfact = gfc.ResvMax == 0 ? 0f : (float)gfc.ResvSize / gfc.ResvMax * 0.5f;
            else if (gfp.VBR == VbrMode.vbr_rh || gfp.VBR == VbrMode.vbr_mtrh ||
                     gfp.VBR == VbrMode.vbr_mt)
                pcfact = 0.6f;
            else
                pcfact = 1.0f;

            for (chn = 0; chn < gfc.channels_out; chn++)
            {

                var firbuf = buffer[chn];
                var firbufPos = bufPos + 576 - 350 - NSFIRLEN + 192;
                Debug.Assert(fircoef.Length == (NSFIRLEN - 1) / 2);
                for (i = 0; i < 576; i++)
                {
                    float sum1, sum2;
                    sum1 = firbuf[firbufPos + i + 10];
                    sum2 = 0.0f;
                    for (j = 0; j < (NSFIRLEN - 1) / 2 - 1; j += 2)
                    {
                        sum1 += fircoef[j] * (firbuf[firbufPos + i + j] + firbuf[firbufPos + i + NSFIRLEN - j]);
                        sum2 += fircoef[j + 1] *
                                (firbuf[firbufPos + i + j + 1] + firbuf[firbufPos + i + NSFIRLEN - j - 1]);
                    }

                    ns_hpfsmpl[chn][i] = sum1 + sum2;
                }

                masking_ratio[gr_out][chn].en.assign(gfc.en[chn]);
                masking_ratio[gr_out][chn].thm.assign(gfc.thm[chn]);
                if (numchn > 2)
                {
                    masking_MS_ratio[gr_out][chn].en.assign(gfc.en[chn + 2]);
                    masking_MS_ratio[gr_out][chn].thm.assign(gfc.thm[chn + 2]);
                }
            }

            for (chn = 0; chn < numchn; chn++)
            {
                float[][] wsamp_l;
                float[][][] wsamp_s;
                var en_subshort = new float[12];
                var en_short = new float[]
                {
                    0,
                    0,
                    0,
                    0
                };
                var attack_intensity = new float[12];
                var ns_uselongblock = 1;
                float attackThreshold;
                var max = new float[Encoder.CBANDS];
                var avg = new float[Encoder.CBANDS];
                var ns_attacks = new[]
                {
                    0,
                    0,
                    0,
                    0
                };
                var fftenergy = new float[Encoder.HBLKSIZE];

                var fftenergy_s = Arrays.ReturnRectangularArray<float>(3, Encoder.HBLKSIZE_s);
                Debug.Assert(gfc.npart_s <= Encoder.CBANDS);
                Debug.Assert(gfc.npart_l <= Encoder.CBANDS);
                for (i = 0; i < 3; i++)
                {
                    en_subshort[i] = gfc.nsPsy.last_en_subshort[chn][i + 6];
                    Debug.Assert(gfc.nsPsy.last_en_subshort[chn][i + 4] > 0);
                    attack_intensity[i] = en_subshort[i] / gfc.nsPsy.last_en_subshort[chn][i + 4];
                    en_short[0] += en_subshort[i];
                }

                if (chn == 2)
                    for (i = 0; i < 576; i++)
                    {
                        float l, r;
                        l = ns_hpfsmpl[0][i];
                        r = ns_hpfsmpl[1][i];
                        ns_hpfsmpl[0][i] = l + r;
                        ns_hpfsmpl[1][i] = l - r;
                    }

                {
                    var pf = ns_hpfsmpl[chn & 1];
                    var pfPos = 0;
                    for (i = 0; i < 9; i++)
                    {
                        var pfe = pfPos + 576 / 9;
                        var p = 1.0f;
                        for (; pfPos < pfe; pfPos++)
                            if (p < Math.Abs(pf[pfPos]))
                                p = Math.Abs(pf[pfPos]);

                        gfc.nsPsy.last_en_subshort[chn][i] = en_subshort[i + 3] = p;
                        en_short[1 + i / 3] += p;
                        if (p > en_subshort[i + 3 - 2])
                        {
                            Debug.Assert(en_subshort[i + 3 - 2] > 0);
                            p = p / en_subshort[i + 3 - 2];
                        }
                        else if (en_subshort[i + 3 - 2] > p * 10.0f)
                        {
                            Debug.Assert(p > 0);
                            p = en_subshort[i + 3 - 2] / (p * 10.0f);
                        }
                        else
                        {
                            p = 0.0f;
                        }

                        attack_intensity[i + 3] = p;
                    }
                }
                if (gfp.analysis)
                {
                    var x = attack_intensity[0];
                    for (i = 1; i < 12; i++)
                        if (x < attack_intensity[i])
                            x = attack_intensity[i];

                    gfc.pinfo.ers[gr_out][chn] = gfc.pinfo.ers_save[chn];
                    gfc.pinfo.ers_save[chn] = x;
                }

                attackThreshold = chn == 3 ? gfc.nsPsy.attackthre_s : gfc.nsPsy.attackthre;
                for (i = 0; i < 12; i++)
                    if (0 == ns_attacks[i / 3] && attack_intensity[i] > attackThreshold)
                        ns_attacks[i / 3] = i % 3 + 1;

                for (i = 1; i < 4; i++)
                {
                    float ratio;
                    if (en_short[i - 1] > en_short[i])
                    {
                        Debug.Assert(en_short[i] > 0);
                        ratio = en_short[i - 1] / en_short[i];
                    }
                    else
                    {
                        Debug.Assert(en_short[i - 1] > 0);
                        ratio = en_short[i] / en_short[i - 1];
                    }

                    if (ratio < 1.7)
                    {
                        ns_attacks[i] = 0;
                        if (i == 1)
                            ns_attacks[0] = 0;
                    }
                }

                if (ns_attacks[0] != 0 && gfc.nsPsy.lastAttacks[chn] != 0)
                    ns_attacks[0] = 0;

                if (gfc.nsPsy.lastAttacks[chn] == 3 ||
                    ns_attacks[0] + ns_attacks[1] + ns_attacks[2] + ns_attacks[3] != 0)
                {
                    ns_uselongblock = 0;
                    if (ns_attacks[1] != 0 && ns_attacks[0] != 0)
                        ns_attacks[1] = 0;

                    if (ns_attacks[2] != 0 && ns_attacks[1] != 0)
                        ns_attacks[2] = 0;

                    if (ns_attacks[3] != 0 && ns_attacks[2] != 0)
                        ns_attacks[3] = 0;
                }

                if (chn < 2)
                {
                    uselongblock[chn] = ns_uselongblock;
                }
                else
                {
                    if (ns_uselongblock == 0)
                        uselongblock[0] = uselongblock[1] = 0;
                }

                energy[chn] = gfc.tot_ener[chn];
                wsamp_s = wsamp_S;
                wsamp_l = wsamp_L;
                compute_ffts(
                    gfp,
                    fftenergy,
                    fftenergy_s,
                    wsamp_l,
                    chn & 1,
                    wsamp_s,
                    chn & 1,
                    gr_out,
                    chn,
                    buffer,
                    bufPos);
                calc_energy(gfc, fftenergy, eb_l, max, avg);
                calc_mask_index_l(gfc, max, avg, mask_idx_l);
                for (sblock = 0; sblock < 3; sblock++)
                {
                    float enn, thmm;
                    compute_masking_s(gfp, fftenergy_s, eb_s, thr, chn, sblock);
                    convert_partition2scalefac_s(gfc, eb_s, thr, chn, sblock);
                    for (sb = 0; sb < Encoder.SBMAX_s; sb++)
                    {
                        thmm = gfc.thm[chn].s[sb][sblock];
                        thmm *= NS_PREECHO_ATT0;
                        if (ns_attacks[sblock] >= 2 || ns_attacks[sblock + 1] == 1)
                        {
                            var idx = sblock != 0 ? sblock - 1 : 2;
                            double p = NS_INTERP(gfc.thm[chn].s[sb][idx], thmm, NS_PREECHO_ATT1 * pcfact);
                            thmm = (float)Math.Min(thmm, p);
                        }

                        if (ns_attacks[sblock] == 1)
                        {
                            var idx = sblock != 0 ? sblock - 1 : 2;
                            double p = NS_INTERP(gfc.thm[chn].s[sb][idx], thmm, NS_PREECHO_ATT2 * pcfact);
                            thmm = (float)Math.Min(thmm, p);
                        }
                        else if (sblock != 0 && ns_attacks[sblock - 1] == 3 ||
                                 sblock == 0 && gfc.nsPsy.lastAttacks[chn] == 3)
                        {
                            var idx = sblock != 2 ? sblock + 1 : 0;
                            double p = NS_INTERP(gfc.thm[chn].s[sb][idx], thmm, NS_PREECHO_ATT2 * pcfact);
                            thmm = (float)Math.Min(thmm, p);
                        }

                        enn = en_subshort[sblock * 3 + 3] + en_subshort[sblock * 3 + 4] + en_subshort[sblock * 3 + 5];
                        if (en_subshort[sblock * 3 + 5] * 6 < enn)
                        {
                            thmm *= (float)0.5;
                            if (en_subshort[sblock * 3 + 4] * 6 < enn)
                                thmm *= (float)0.5;
                        }

                        gfc.thm[chn].s[sb][sblock] = thmm;
                    }
                }

                gfc.nsPsy.lastAttacks[chn] = ns_attacks[2];
                k = 0;
                {
                    for (b = 0; b < gfc.npart_l; b++)
                    {
                        var kk = gfc.s3ind[b][0];
                        var eb2 = eb_l[kk] * tab[mask_idx_l[kk]];
                        var ecb = gfc.s3_ll[k++] * eb2;
                        while (++kk <= gfc.s3ind[b][1])
                        {
                            eb2 = eb_l[kk] * tab[mask_idx_l[kk]];
                            ecb = mask_add(ecb, gfc.s3_ll[k++] * eb2, kk, kk - b, gfc, 0);
                        }

                        ecb *= (float)0.158489319246111;
                        if (gfc.blocktype_old[chn & 1] == Encoder.SHORT_TYPE)
                            thr[b] = ecb;
                        else
                            thr[b] = NS_INTERP(
                                Math.Min(ecb, Math.Min(rpelev * gfc.nb_1[chn][b], rpelev2 * gfc.nb_2[chn][b])),
                                ecb,
                                pcfact);

                        gfc.nb_2[chn][b] = gfc.nb_1[chn][b];
                        gfc.nb_1[chn][b] = ecb;
                    }
                }
                for (; b <= Encoder.CBANDS; ++b)
                {
                    eb_l[b] = 0;
                    thr[b] = 0;
                }

                convert_partition2scalefac_l(gfc, eb_l, thr, chn);
            }

            if (gfp.mode == MPEGMode.STEREO || gfp.mode == MPEGMode.JOINT_STEREO)
                if (gfp.interChRatio > 0.0)
                    calc_interchannel_masking(gfp, gfp.interChRatio);

            if (gfp.mode == MPEGMode.JOINT_STEREO)
            {
                float msfix;
                msfix1(gfc);
                msfix = gfp.msfix;
                if (Math.Abs(msfix) > 0.0)
                    ns_msfix(gfc, msfix, gfp.ATHlower * gfc.ATH.adjust);
            }

            block_type_set(gfp, uselongblock, blocktype_d, blocktype);
            for (chn = 0; chn < numchn; chn++)
            {
                float[] ppe;
                var ppePos = 0;
                int type;
                III_psy_ratio mr;
                if (chn > 1)
                {
                    ppe = percep_MS_entropy;
                    ppePos = -2;
                    type = Encoder.NORM_TYPE;
                    if (blocktype_d[0] == Encoder.SHORT_TYPE || blocktype_d[1] == Encoder.SHORT_TYPE)
                        type = Encoder.SHORT_TYPE;

                    mr = masking_MS_ratio[gr_out][chn - 2];
                }
                else
                {
                    ppe = percep_entropy;
                    ppePos = 0;
                    type = blocktype_d[chn];
                    mr = masking_ratio[gr_out][chn];
                }

                if (type == Encoder.SHORT_TYPE)
                    ppe[ppePos + chn] = pecalc_s(mr, gfc.masking_lower);
                else
                    ppe[ppePos + chn] = pecalc_l(mr, gfc.masking_lower);

                if (gfp.analysis)
                    gfc.pinfo.pe[gr_out][chn] = ppe[ppePos + chn];
            }

            return 0;
        }

        private void vbrpsy_compute_fft_l(
            LameGlobalFlags gfp,
            float[][] buffer,
            int bufPos,
            int chn,
            int gr_out,
            float[] fftenergy,
            float[][] wsamp_l,
            int wsamp_lPos)
        {

            var gfc = gfp.internal_flags;
            if (chn < 2)
                fft.fft_long(gfc, wsamp_l[wsamp_lPos], chn, buffer, bufPos);
            else if (chn == 2)
                for (var j = Encoder.BLKSIZE - 1; j >= 0; --j)
                {
                    var l = wsamp_l[wsamp_lPos + 0][j];
                    var r = wsamp_l[wsamp_lPos + 1][j];
                    wsamp_l[wsamp_lPos + 0][j] = (l + r) * Util.SQRT2 * 0.5f;
                    wsamp_l[wsamp_lPos + 1][j] = (l - r) * Util.SQRT2 * 0.5f;
                }

            fftenergy[0] = NON_LINEAR_SCALE_ENERGY(wsamp_l[wsamp_lPos + 0][0]);
            fftenergy[0] *= fftenergy[0];
            for (var j = Encoder.BLKSIZE / 2 - 1; j >= 0; --j)
            {
                var re = wsamp_l[wsamp_lPos + 0][Encoder.BLKSIZE / 2 - j];
                var im = wsamp_l[wsamp_lPos + 0][Encoder.BLKSIZE / 2 + j];
                fftenergy[Encoder.BLKSIZE / 2 - j] = NON_LINEAR_SCALE_ENERGY((re * re + im * im) * 0.5f);
            }

            {
                var totalenergy = 0.0f;
                for (var j = 11; j < Encoder.HBLKSIZE; j++)
                    totalenergy += fftenergy[j];

                gfc.tot_ener[chn] = totalenergy;
            }
            if (gfp.analysis)
            {
                for (var j = 0; j < Encoder.HBLKSIZE; j++)
                {
                    gfc.pinfo.energy[gr_out][chn][j] = gfc.pinfo.energy_save[chn][j];
                    gfc.pinfo.energy_save[chn][j] = fftenergy[j];
                }

                gfc.pinfo.pe[gr_out][chn] = gfc.pe[chn];
            }
        }

        private void vbrpsy_compute_fft_s(
            LameGlobalFlags gfp,
            float[][] buffer,
            int bufPos,
            int chn,
            int sblock,
            float[][] fftenergy_s,
            float[][][] wsamp_s,
            int wsamp_sPos)
        {

            var gfc = gfp.internal_flags;
            if (sblock == 0 && chn < 2)
                fft.fft_short(gfc, wsamp_s[wsamp_sPos], chn, buffer, bufPos);

            if (chn == 2)
                for (var j = Encoder.BLKSIZE_s - 1; j >= 0; --j)
                {
                    var l = wsamp_s[wsamp_sPos + 0][sblock][j];
                    var r = wsamp_s[wsamp_sPos + 1][sblock][j];
                    wsamp_s[wsamp_sPos + 0][sblock][j] = (l + r) * Util.SQRT2 * 0.5f;
                    wsamp_s[wsamp_sPos + 1][sblock][j] = (l - r) * Util.SQRT2 * 0.5f;
                }

            fftenergy_s[sblock][0] = wsamp_s[wsamp_sPos + 0][sblock][0];
            fftenergy_s[sblock][0] *= fftenergy_s[sblock][0];
            for (var j = Encoder.BLKSIZE_s / 2 - 1; j >= 0; --j)
            {
                var re = wsamp_s[wsamp_sPos + 0][sblock][Encoder.BLKSIZE_s / 2 - j];
                var im = wsamp_s[wsamp_sPos + 0][sblock][Encoder.BLKSIZE_s / 2 + j];
                fftenergy_s[sblock][Encoder.BLKSIZE_s / 2 - j] = NON_LINEAR_SCALE_ENERGY((re * re + im * im) * 0.5f);
            }
        }

        private void vbrpsy_compute_loudness_approximation_l(
            LameGlobalFlags gfp,
            int gr_out,
            int chn,
            float[] fftenergy)
        {

            var gfc = gfp.internal_flags;
            if (gfp.athaa_loudapprox == 2 && chn < 2)
            {
                gfc.loudness_sq[gr_out][chn] = gfc.loudness_sq_save[chn];
                gfc.loudness_sq_save[chn] = psycho_loudness_approx(fftenergy, gfc);
            }
        }

        private void vbrpsy_attack_detection(
            LameGlobalFlags gfp,
            float[][] buffer,
            int bufPos,
            int gr_out,
            III_psy_ratio[][] masking_ratio,
            III_psy_ratio[][] masking_MS_ratio,
            float[] energy,
            float[][] sub_short_factor,
            int[][] ns_attacks,
            int[] uselongblock)
        {

            var ns_hpfsmpl = Arrays.ReturnRectangularArray<float>(2, 576);

            var gfc = gfp.internal_flags;
            var n_chn_out = gfc.channels_out;
            var n_chn_psy = gfp.mode == MPEGMode.JOINT_STEREO ? 4 : n_chn_out;
            for (var chn = 0; chn < n_chn_out; chn++)
            {

                var firbuf = buffer[chn];
                var firbufPos = bufPos + 576 - 350 - NSFIRLEN + 192;
                Debug.Assert(fircoef_.Length == (NSFIRLEN - 1) / 2);
                for (var i = 0; i < 576; i++)
                {
                    float sum1, sum2;
                    sum1 = firbuf[firbufPos + i + 10];
                    sum2 = 0.0f;
                    for (var j = 0; j < (NSFIRLEN - 1) / 2 - 1; j += 2)
                    {
                        sum1 += fircoef_[j] * (firbuf[firbufPos + i + j] + firbuf[firbufPos + i + NSFIRLEN - j]);
                        sum2 += fircoef_[j + 1] *
                                (firbuf[firbufPos + i + j + 1] + firbuf[firbufPos + i + NSFIRLEN - j - 1]);
                    }

                    ns_hpfsmpl[chn][i] = sum1 + sum2;
                }

                masking_ratio[gr_out][chn].en.assign(gfc.en[chn]);
                masking_ratio[gr_out][chn].thm.assign(gfc.thm[chn]);
                if (n_chn_psy > 2)
                {
                    masking_MS_ratio[gr_out][chn].en.assign(gfc.en[chn + 2]);
                    masking_MS_ratio[gr_out][chn].thm.assign(gfc.thm[chn + 2]);
                }
            }

            for (var chn = 0; chn < n_chn_psy; chn++)
            {
                var attack_intensity = new float[12];
                var en_subshort = new float[12];
                var en_short = new float[]
                {
                    0,
                    0,
                    0,
                    0
                };
                var pf = ns_hpfsmpl[chn & 1];
                var pfPos = 0;

                var attackThreshold = chn == 3 ? gfc.nsPsy.attackthre_s : gfc.nsPsy.attackthre;
                var ns_uselongblock = 1;
                if (chn == 2)
                    for (int i = 0, j = 576; j > 0; ++i, --j)
                    {

                        var l = ns_hpfsmpl[0][i];

                        var r = ns_hpfsmpl[1][i];
                        ns_hpfsmpl[0][i] = l + r;
                        ns_hpfsmpl[1][i] = l - r;
                    }

                for (var i = 0; i < 3; i++)
                {
                    en_subshort[i] = gfc.nsPsy.last_en_subshort[chn][i + 6];
                    Debug.Assert(gfc.nsPsy.last_en_subshort[chn][i + 4] > 0);
                    attack_intensity[i] = en_subshort[i] / gfc.nsPsy.last_en_subshort[chn][i + 4];
                    en_short[0] += en_subshort[i];
                }

                for (var i = 0; i < 9; i++)
                {

                    var pfe = pfPos + 576 / 9;
                    var p = 1.0f;
                    for (; pfPos < pfe; pfPos++)
                        if (p < Math.Abs(pf[pfPos]))
                            p = Math.Abs(pf[pfPos]);

                    gfc.nsPsy.last_en_subshort[chn][i] = en_subshort[i + 3] = p;
                    en_short[1 + i / 3] += p;
                    if (p > en_subshort[i + 3 - 2])
                    {
                        Debug.Assert(en_subshort[i + 3 - 2] > 0);
                        p = p / en_subshort[i + 3 - 2];
                    }
                    else if (en_subshort[i + 3 - 2] > p * 10.0)
                    {
                        Debug.Assert(p > 0);
                        p = en_subshort[i + 3 - 2] / (p * 10.0f);
                    }
                    else
                    {
                        p = 0.0f;
                    }

                    attack_intensity[i + 3] = p;
                }

                for (var i = 0; i < 3; ++i)
                {

                    var enn = en_subshort[i * 3 + 3] + en_subshort[i * 3 + 4] + en_subshort[i * 3 + 5];
                    var factor = 1.0f;
                    if (en_subshort[i * 3 + 5] * 6 < enn)
                    {
                        factor *= 0.5f;
                        if (en_subshort[i * 3 + 4] * 6 < enn)
                            factor *= 0.5f;
                    }

                    sub_short_factor[chn][i] = factor;
                }

                if (gfp.analysis)
                {
                    var x = attack_intensity[0];
                    for (var i = 1; i < 12; i++)
                        if (x < attack_intensity[i])
                            x = attack_intensity[i];

                    gfc.pinfo.ers[gr_out][chn] = gfc.pinfo.ers_save[chn];
                    gfc.pinfo.ers_save[chn] = x;
                }

                for (var i = 0; i < 12; i++)
                    if (0 == ns_attacks[chn][i / 3] && attack_intensity[i] > attackThreshold)
                        ns_attacks[chn][i / 3] = i % 3 + 1;

                for (var i = 1; i < 4; i++)
                {

                    var u = en_short[i - 1];

                    var v = en_short[i];

                    var m = Math.Max(u, v);
                    if (m < 40000)
                        if (u < 1.7 * v && v < 1.7 * u)
                        {
                            if (i == 1 && ns_attacks[chn][0] <= ns_attacks[chn][i])
                                ns_attacks[chn][0] = 0;

                            ns_attacks[chn][i] = 0;
                        }
                }

                if (ns_attacks[chn][0] <= gfc.nsPsy.lastAttacks[chn])
                    ns_attacks[chn][0] = 0;

                if (gfc.nsPsy.lastAttacks[chn] == 3 ||
                    ns_attacks[chn][0] + ns_attacks[chn][1] + ns_attacks[chn][2] + ns_attacks[chn][3] != 0)
                {
                    ns_uselongblock = 0;
                    if (ns_attacks[chn][1] != 0 && ns_attacks[chn][0] != 0)
                        ns_attacks[chn][1] = 0;

                    if (ns_attacks[chn][2] != 0 && ns_attacks[chn][1] != 0)
                        ns_attacks[chn][2] = 0;

                    if (ns_attacks[chn][3] != 0 && ns_attacks[chn][2] != 0)
                        ns_attacks[chn][3] = 0;
                }

                if (chn < 2)
                {
                    uselongblock[chn] = ns_uselongblock;
                }
                else
                {
                    if (ns_uselongblock == 0)
                        uselongblock[0] = uselongblock[1] = 0;
                }

                energy[chn] = gfc.tot_ener[chn];
            }
        }

        private void vbrpsy_skip_masking_s(LameInternalFlags gfc, int chn, int sblock)
        {
            if (sblock == 0)
                for (var b = 0; b < gfc.npart_s; b++)
                {
                    gfc.nb_s2[chn][b] = gfc.nb_s1[chn][b];
                    gfc.nb_s1[chn][b] = 0;
                }
        }

        private void vbrpsy_skip_masking_l(LameInternalFlags gfc, int chn)
        {
            for (var b = 0; b < gfc.npart_l; b++)
            {
                gfc.nb_2[chn][b] = gfc.nb_1[chn][b];
                gfc.nb_1[chn][b] = 0;
            }
        }

        private void psyvbr_calc_mask_index_s(LameInternalFlags gfc, float[] max, float[] avg, int[] mask_idx)
        {
            var last_tab_entry = tab.Length - 1;
            var b = 0;
            var a = avg[b] + avg[b + 1];
            Debug.Assert(a >= 0);
            if (a > 0.0)
            {
                var m = max[b];
                if (m < max[b + 1])
                    m = max[b + 1];

                Debug.Assert(gfc.numlines_s[b] + gfc.numlines_s[b + 1] - 1 > 0);
                a = 20.0f * (m * 2.0f - a) / (a * (gfc.numlines_s[b] + gfc.numlines_s[b + 1] - 1));
                var k = (int)a;
                if (k > last_tab_entry)
                    k = last_tab_entry;

                mask_idx[b] = k;
            }
            else
            {
                mask_idx[b] = 0;
            }

            for (b = 1; b < gfc.npart_s - 1; b++)
            {
                a = avg[b - 1] + avg[b] + avg[b + 1];
                Debug.Assert(b + 1 < gfc.npart_s);
                Debug.Assert(a >= 0);
                if (a > 0.0)
                {
                    var m = max[b - 1];
                    if (m < max[b])
                        m = max[b];

                    if (m < max[b + 1])
                        m = max[b + 1];

                    Debug.Assert(gfc.numlines_s[b - 1] + gfc.numlines_s[b] + gfc.numlines_s[b + 1] - 1 > 0);
                    a = 20.0f * (m * 3.0f - a) /
                        (a * (gfc.numlines_s[b - 1] + gfc.numlines_s[b] + gfc.numlines_s[b + 1] - 1));
                    var k = (int)a;
                    if (k > last_tab_entry)
                        k = last_tab_entry;

                    mask_idx[b] = k;
                }
                else
                {
                    mask_idx[b] = 0;
                }
            }

            Debug.Assert(b > 0);
            Debug.Assert(b == gfc.npart_s - 1);
            a = avg[b - 1] + avg[b];
            Debug.Assert(a >= 0);
            if (a > 0.0)
            {
                var m = max[b - 1];
                if (m < max[b])
                    m = max[b];

                Debug.Assert(gfc.numlines_s[b - 1] + gfc.numlines_s[b] - 1 > 0);
                a = 20.0f * (m * 2.0f - a) / (a * (gfc.numlines_s[b - 1] + gfc.numlines_s[b] - 1));
                var k = (int)a;
                if (k > last_tab_entry)
                    k = last_tab_entry;

                mask_idx[b] = k;
            }
            else
            {
                mask_idx[b] = 0;
            }

            Debug.Assert(b == gfc.npart_s - 1);
        }

        private void vbrpsy_compute_masking_s(
            LameGlobalFlags gfp,
            float[][] fftenergy_s,
            float[] eb,
            float[] thr,
            int chn,
            int sblock)
        {

            var gfc = gfp.internal_flags;
            var max = new float[Encoder.CBANDS];
            var avg = new float[Encoder.CBANDS];
            int i, j, b;
            var mask_idx_s = new int[Encoder.CBANDS];
            for (b = j = 0; b < gfc.npart_s; ++b)
            {
                float ebb = 0, m = 0;
                var n = gfc.numlines_s[b];
                for (i = 0; i < n; ++i, ++j)
                {
                    var el = fftenergy_s[sblock][j];
                    ebb += el;
                    if (m < el)
                        m = el;
                }

                eb[b] = ebb;
                Debug.Assert(ebb >= 0);
                max[b] = m;
                Debug.Assert(n > 0);
                avg[b] = ebb / n;
                Debug.Assert(avg[b] >= 0);
            }

            Debug.Assert(b == gfc.npart_s);
            Debug.Assert(j == 129);
            for (; b < Encoder.CBANDS; ++b)
            {
                max[b] = 0;
                avg[b] = 0;
            }

            psyvbr_calc_mask_index_s(gfc, max, avg, mask_idx_s);
            for (j = b = 0; b < gfc.npart_s; b++)
            {
                var kk = gfc.s3ind_s[b][0];
                var last = gfc.s3ind_s[b][1];
                int dd, dd_n;
                float x, ecb, avg_mask;
                dd = mask_idx_s[kk];
                dd_n = 1;
                ecb = gfc.s3_ss[j] * eb[kk] * tab[mask_idx_s[kk]];
                ++j;
                ++kk;
                while (kk <= last)
                {
                    dd += mask_idx_s[kk];
                    dd_n += 1;
                    x = gfc.s3_ss[j] * eb[kk] * tab[mask_idx_s[kk]];
                    ecb = vbrpsy_mask_add(ecb, x, kk - b);
                    ++j;
                    ++kk;
                }

                dd = (1 + 2 * dd) / (2 * dd_n);
                avg_mask = tab[dd] * 0.5f;
                ecb *= avg_mask;
                thr[b] = ecb;
                gfc.nb_s2[chn][b] = gfc.nb_s1[chn][b];
                gfc.nb_s1[chn][b] = ecb;
                {
                    x = max[b];
                    x *= gfc.minval_s[b];
                    x *= avg_mask;
                    if (thr[b] > x)
                        thr[b] = x;
                }
                if (gfc.masking_lower > 1)
                    thr[b] *= gfc.masking_lower;

                if (thr[b] > eb[b])
                    thr[b] = eb[b];

                if (gfc.masking_lower < 1)
                    thr[b] *= gfc.masking_lower;

                Debug.Assert(thr[b] >= 0);
            }

            for (; b < Encoder.CBANDS; ++b)
            {
                eb[b] = 0;
                thr[b] = 0;
            }
        }

        private void vbrpsy_compute_masking_l(
            LameInternalFlags gfc,
            float[] fftenergy,
            float[] eb_l,
            float[] thr,
            int chn)
        {
            var max = new float[Encoder.CBANDS];
            var avg = new float[Encoder.CBANDS];
            var mask_idx_l = new int[Encoder.CBANDS + 2];
            int b;
            calc_energy(gfc, fftenergy, eb_l, max, avg);
            calc_mask_index_l(gfc, max, avg, mask_idx_l);
            var k = 0;
            for (b = 0; b < gfc.npart_l; b++)
            {
                float x, ecb, avg_mask, t;
                var kk = gfc.s3ind[b][0];
                var last = gfc.s3ind[b][1];
                int dd = 0, dd_n = 0;
                dd = mask_idx_l[kk];
                dd_n += 1;
                ecb = gfc.s3_ll[k] * eb_l[kk] * tab[mask_idx_l[kk]];
                ++k;
                ++kk;
                while (kk <= last)
                {
                    dd += mask_idx_l[kk];
                    dd_n += 1;
                    x = gfc.s3_ll[k] * eb_l[kk] * tab[mask_idx_l[kk]];
                    t = vbrpsy_mask_add(ecb, x, kk - b);
                    ecb = t;
                    ++k;
                    ++kk;
                }

                dd = (1 + 2 * dd) / (2 * dd_n);
                avg_mask = tab[dd] * 0.5f;
                ecb *= avg_mask;
                if (gfc.blocktype_old[chn & 0x01] == Encoder.SHORT_TYPE)
                {
                    var ecb_limit = rpelev * gfc.nb_1[chn][b];
                    if (ecb_limit > 0)
                        thr[b] = Math.Min(ecb, ecb_limit);
                    else
                        thr[b] = Math.Min(ecb, eb_l[b] * NS_PREECHO_ATT2);
                }
                else
                {
                    var ecb_limit_2 = rpelev2 * gfc.nb_2[chn][b];
                    var ecb_limit_1 = rpelev * gfc.nb_1[chn][b];
                    float ecb_limit;
                    if (ecb_limit_2 <= 0)
                        ecb_limit_2 = ecb;

                    if (ecb_limit_1 <= 0)
                        ecb_limit_1 = ecb;

                    if (gfc.blocktype_old[chn & 0x01] == Encoder.NORM_TYPE)
                        ecb_limit = Math.Min(ecb_limit_1, ecb_limit_2);
                    else
                        ecb_limit = ecb_limit_1;

                    thr[b] = Math.Min(ecb, ecb_limit);
                }

                gfc.nb_2[chn][b] = gfc.nb_1[chn][b];
                gfc.nb_1[chn][b] = ecb;
                {
                    x = max[b];
                    x *= gfc.minval_l[b];
                    x *= avg_mask;
                    if (thr[b] > x)
                        thr[b] = x;
                }
                if (gfc.masking_lower > 1)
                    thr[b] *= gfc.masking_lower;

                if (thr[b] > eb_l[b])
                    thr[b] = eb_l[b];

                if (gfc.masking_lower < 1)
                    thr[b] *= gfc.masking_lower;

                Debug.Assert(thr[b] >= 0);
            }

            for (; b < Encoder.CBANDS; ++b)
            {
                eb_l[b] = 0;
                thr[b] = 0;
            }
        }

        private void vbrpsy_compute_block_type(LameGlobalFlags gfp, int[] uselongblock)
        {

            var gfc = gfp.internal_flags;
            if (gfp.short_blocks == ShortBlock.short_block_coupled && !(uselongblock[0] != 0 && uselongblock[1] != 0))
                uselongblock[0] = uselongblock[1] = 0;

            for (var chn = 0; chn < gfc.channels_out; chn++)
            {
                if (gfp.short_blocks == ShortBlock.short_block_dispensed)
                    uselongblock[chn] = 1;

                if (gfp.short_blocks == ShortBlock.short_block_forced)
                    uselongblock[chn] = 0;
            }
        }

        private void vbrpsy_apply_block_type(LameGlobalFlags gfp, int[] uselongblock, int[] blocktype_d)
        {

            var gfc = gfp.internal_flags;
            for (var chn = 0; chn < gfc.channels_out; chn++)
            {
                var blocktype = Encoder.NORM_TYPE;
                if (uselongblock[chn] != 0)
                {
                    Debug.Assert(gfc.blocktype_old[chn] != Encoder.START_TYPE);
                    if (gfc.blocktype_old[chn] == Encoder.SHORT_TYPE)
                        blocktype = Encoder.STOP_TYPE;
                }
                else
                {
                    blocktype = Encoder.SHORT_TYPE;
                    if (gfc.blocktype_old[chn] == Encoder.NORM_TYPE)
                        gfc.blocktype_old[chn] = Encoder.START_TYPE;

                    if (gfc.blocktype_old[chn] == Encoder.STOP_TYPE)
                        gfc.blocktype_old[chn] = Encoder.SHORT_TYPE;
                }

                blocktype_d[chn] = gfc.blocktype_old[chn];
                gfc.blocktype_old[chn] = blocktype;
            }
        }

        private void vbrpsy_compute_MS_thresholds(
            float[][] eb,
            float[][] thr,
            float[] cb_mld,
            float[] ath_cb,
            float athadjust,
            float msfix,
            int n)
        {
            var msfix2 = msfix * 2;
            var athlower = msfix > 0 ? (float)Math.Pow(10, athadjust) : 1f;
            float rside, rmid;
            for (var b = 0; b < n; ++b)
            {
                var ebM = eb[2][b];
                var ebS = eb[3][b];
                var thmL = thr[0][b];
                var thmR = thr[1][b];
                var thmM = thr[2][b];
                var thmS = thr[3][b];
                if (thmL <= 1.58 * thmR && thmR <= 1.58 * thmL)
                {
                    var mld_m = cb_mld[b] * ebS;
                    var mld_s = cb_mld[b] * ebM;
                    rmid = Math.Max(thmM, Math.Min(thmS, mld_m));
                    rside = Math.Max(thmS, Math.Min(thmM, mld_s));
                }
                else
                {
                    rmid = thmM;
                    rside = thmS;
                }

                if (msfix > 0)
                {
                    float thmLR, thmMS;
                    var ath = ath_cb[b] * athlower;
                    thmLR = Math.Min(Math.Max(thmL, ath), Math.Max(thmR, ath));
                    thmM = Math.Max(rmid, ath);
                    thmS = Math.Max(rside, ath);
                    thmMS = thmM + thmS;
                    if (thmMS > 0 && thmLR * msfix2 < thmMS)
                    {
                        var f = thmLR * msfix2 / thmMS;
                        thmM *= f;
                        thmS *= f;
                        Debug.Assert(thmMS > 0);
                    }

                    rmid = Math.Min(thmM, rmid);
                    rside = Math.Min(thmS, rside);
                }

                if (rmid > ebM)
                    rmid = ebM;

                if (rside > ebS)
                    rside = ebS;

                thr[2][b] = rmid;
                thr[3][b] = rside;
            }
        }

        internal int L3psycho_anal_vbr(
            LameGlobalFlags gfp,
            float[][] buffer,
            int bufPos,
            int gr_out,
            III_psy_ratio[][] masking_ratio,
            III_psy_ratio[][] masking_MS_ratio,
            float[] percep_entropy,
            float[] percep_MS_entropy,
            float[] energy,
            int[] blocktype_d)
        {

            var gfc = gfp.internal_flags;
            float[][] wsamp_l;
            float[][][] wsamp_s;
            var fftenergy = new float[Encoder.HBLKSIZE];

            var fftenergy_s = Arrays.ReturnRectangularArray<float>(3, Encoder.HBLKSIZE_s);

            var wsamp_L = Arrays.ReturnRectangularArray<float>(2, Encoder.BLKSIZE);

            var wsamp_S = Arrays.ReturnRectangularArray<float>(2, 3, Encoder.BLKSIZE_s);

            var eb = Arrays.ReturnRectangularArray<float>(4, Encoder.CBANDS);
            var thr = Arrays.ReturnRectangularArray<float>(4, Encoder.CBANDS);

            var sub_short_factor = Arrays.ReturnRectangularArray<float>(4, 3);
            var pcfact = 0.6f;
            var ns_attacks = new[]
            {
                new[]
                {
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
                    0
                },
                new[]
                {
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
                    0
                }
            };
            var uselongblock = new int[2];
            var n_chn_psy = gfp.mode == MPEGMode.JOINT_STEREO ? 4 : gfc.channels_out;
            vbrpsy_attack_detection(
                gfp,
                buffer,
                bufPos,
                gr_out,
                masking_ratio,
                masking_MS_ratio,
                energy,
                sub_short_factor,
                ns_attacks,
                uselongblock);
            vbrpsy_compute_block_type(gfp, uselongblock);
            {
                for (var chn = 0; chn < n_chn_psy; chn++)
                {
                    var ch01 = chn & 0x01;
                    wsamp_l = wsamp_L;
                    vbrpsy_compute_fft_l(gfp, buffer, bufPos, chn, gr_out, fftenergy, wsamp_l, ch01);
                    vbrpsy_compute_loudness_approximation_l(gfp, gr_out, chn, fftenergy);
                    if (uselongblock[ch01] != 0)
                        vbrpsy_compute_masking_l(gfc, fftenergy, eb[chn], thr[chn], chn);
                    else
                        vbrpsy_skip_masking_l(gfc, chn);
                }

                if (uselongblock[0] + uselongblock[1] == 2)
                    if (gfp.mode == MPEGMode.JOINT_STEREO)
                        vbrpsy_compute_MS_thresholds(
                            eb,
                            thr,
                            gfc.mld_cb_l,
                            gfc.ATH.cb_l,
                            gfp.ATHlower * gfc.ATH.adjust,
                            gfp.msfix,
                            gfc.npart_l);

                for (var chn = 0; chn < n_chn_psy; chn++)
                {
                    var ch01 = chn & 0x01;
                    if (uselongblock[ch01] != 0)
                        convert_partition2scalefac_l(gfc, eb[chn], thr[chn], chn);
                }
            }
            {
                for (var sblock = 0; sblock < 3; sblock++)
                {
                    for (var chn = 0; chn < n_chn_psy; ++chn)
                    {
                        var ch01 = chn & 0x01;
                        if (uselongblock[ch01] != 0)
                        {
                            vbrpsy_skip_masking_s(gfc, chn, sblock);
                        }
                        else
                        {
                            wsamp_s = wsamp_S;
                            vbrpsy_compute_fft_s(gfp, buffer, bufPos, chn, sblock, fftenergy_s, wsamp_s, ch01);
                            vbrpsy_compute_masking_s(gfp, fftenergy_s, eb[chn], thr[chn], chn, sblock);
                        }
                    }

                    if (uselongblock[0] + uselongblock[1] == 0)
                        if (gfp.mode == MPEGMode.JOINT_STEREO)
                            vbrpsy_compute_MS_thresholds(
                                eb,
                                thr,
                                gfc.mld_cb_s,
                                gfc.ATH.cb_s,
                                gfp.ATHlower * gfc.ATH.adjust,
                                gfp.msfix,
                                gfc.npart_s);

                    for (var chn = 0; chn < n_chn_psy; ++chn)
                    {
                        var ch01 = chn & 0x01;
                        if (0 == uselongblock[ch01])
                            convert_partition2scalefac_s(gfc, eb[chn], thr[chn], chn, sblock);
                    }
                }

                for (var chn = 0; chn < n_chn_psy; chn++)
                {
                    var ch01 = chn & 0x01;
                    if (uselongblock[ch01] != 0)
                        continue;

                    for (var sb = 0; sb < Encoder.SBMAX_s; sb++)
                    {
                        var new_thmm = new float[3];
                        for (var sblock = 0; sblock < 3; sblock++)
                        {
                            var thmm = gfc.thm[chn].s[sb][sblock];
                            thmm *= NS_PREECHO_ATT0;
                            if (ns_attacks[chn][sblock] >= 2 || ns_attacks[chn][sblock + 1] == 1)
                            {
                                var idx = sblock != 0 ? sblock - 1 : 2;
                                double p = NS_INTERP(gfc.thm[chn].s[sb][idx], thmm, NS_PREECHO_ATT1 * pcfact);
                                thmm = (float)Math.Min(thmm, p);
                            }
                            else if (ns_attacks[chn][sblock] == 1)
                            {
                                var idx = sblock != 0 ? sblock - 1 : 2;
                                double p = NS_INTERP(gfc.thm[chn].s[sb][idx], thmm, NS_PREECHO_ATT2 * pcfact);
                                thmm = (float)Math.Min(thmm, p);
                            }
                            else if (sblock != 0 && ns_attacks[chn][sblock - 1] == 3 ||
                                     sblock == 0 && gfc.nsPsy.lastAttacks[chn] == 3)
                            {
                                var idx = sblock != 2 ? sblock + 1 : 0;
                                double p = NS_INTERP(gfc.thm[chn].s[sb][idx], thmm, NS_PREECHO_ATT2 * pcfact);
                                thmm = (float)Math.Min(thmm, p);
                            }

                            thmm *= sub_short_factor[chn][sblock];
                            new_thmm[sblock] = thmm;
                        }

                        for (var sblock = 0; sblock < 3; sblock++)
                            gfc.thm[chn].s[sb][sblock] = new_thmm[sblock];
                    }
                }
            }
            for (var chn = 0; chn < n_chn_psy; chn++)
                gfc.nsPsy.lastAttacks[chn] = ns_attacks[chn][2];

            vbrpsy_apply_block_type(gfp, uselongblock, blocktype_d);
            for (var chn = 0; chn < n_chn_psy; chn++)
            {
                float[] ppe;
                int ppePos;
                int type;
                III_psy_ratio mr;
                if (chn > 1)
                {
                    ppe = percep_MS_entropy;
                    ppePos = -2;
                    type = Encoder.NORM_TYPE;
                    if (blocktype_d[0] == Encoder.SHORT_TYPE || blocktype_d[1] == Encoder.SHORT_TYPE)
                        type = Encoder.SHORT_TYPE;

                    mr = masking_MS_ratio[gr_out][chn - 2];
                }
                else
                {
                    ppe = percep_entropy;
                    ppePos = 0;
                    type = blocktype_d[chn];
                    mr = masking_ratio[gr_out][chn];
                }

                if (type == Encoder.SHORT_TYPE)
                    ppe[ppePos + chn] = pecalc_s(mr, gfc.masking_lower);
                else
                    ppe[ppePos + chn] = pecalc_l(mr, gfc.masking_lower);

                if (gfp.analysis)
                    gfc.pinfo.pe[gr_out][chn] = ppe[ppePos + chn];
            }

            return 0;
        }

        private float s3_func_x(float bark, float hf_slope)
        {
            float tempx = bark, tempy;
            if (tempx >= 0)
                tempy = -tempx * 27;
            else
                tempy = tempx * hf_slope;

            if (tempy <= -72.0)
                return 0;

            return (float)Math.Exp(tempy * LN_TO_LOG10);
        }

        private float norm_s3_func_x(float hf_slope)
        {
            double lim_a = 0, lim_b = 0;
            {
                double x = 0, l, h;
                for (x = 0; s3_func_x((float)x, hf_slope) > 1e-20; x -= 1)
                    ;

                l = x;
                h = 0;
                while (Math.Abs(h - l) > 1e-12)
                {
                    x = (h + l) / 2;
                    if (s3_func_x((float)x, hf_slope) > 0)
                        h = x;
                    else
                        l = x;
                }

                lim_a = l;
            }
            {
                double x = 0, l, h;
                for (x = 0; s3_func_x((float)x, hf_slope) > 1e-20; x += 1)
                    ;

                l = 0;
                h = x;
                while (Math.Abs(h - l) > 1e-12)
                {
                    x = (h + l) / 2;
                    if (s3_func_x((float)x, hf_slope) > 0)
                        l = x;
                    else
                        h = x;
                }

                lim_b = h;
            }
            {
                double sum = 0;
                const int m = 1000;
                int i;
                for (i = 0; i <= m; ++i)
                {
                    var x = lim_a + i * (lim_b - lim_a) / m;
                    double y = s3_func_x((float)x, hf_slope);
                    sum += y;
                }

                {
                    var norm = (m + 1) / (sum * (lim_b - lim_a));
                    return (float)norm;
                }
            }
        }

        private float s3_func(float bark)
        {
            float tempx, x, tempy, temp;
            tempx = bark;
            if (tempx >= 0)
                tempx *= 3;
            else
                tempx *= (float)1.5;

            if (tempx >= 0.5 && tempx <= 2.5)
            {
                temp = tempx - 0.5f;
                x = 8.0f * (temp * temp - 2.0f * temp);
            }
            else
            {
                x = 0.0f;
            }

            tempx += (float)0.474;
            tempy = 15.811389f + 7.5f * tempx - 17.5f * (float)Math.Sqrt(1.0 + tempx * tempx);
            if (tempy <= -60.0)
                return 0.0f;

            tempx = (float)Math.Exp((x + tempy) * LN_TO_LOG10);
            tempx /= (float).6609193;
            return tempx;
        }

        private float freq2bark(float freq)
        {
            if (freq < 0)
                freq = 0;

            freq = freq * 0.001f;
            return 13.0f * (float)Math.Atan(.76 * freq) + 3.5f * (float)Math.Atan(freq * freq / (7.5 * 7.5));
        }

        private int init_numline(
            int[] numlines,
            int[] bo,
            int[] bm,
            float[] bval,
            float[] bval_width,
            float[] mld,
            float[] bo_w,
            float sfreq,
            int blksize,
            int[] scalepos,
            float deltafreq,
            int sbmax)
        {
            var b_frq = new float[Encoder.CBANDS + 1];
            var sample_freq_frac = sfreq / (sbmax > 15 ? 2 * 576 : 2 * 192);
            var partition = new int[Encoder.HBLKSIZE];
            int i;
            sfreq /= blksize;
            var j = 0;
            var ni = 0;
            for (i = 0; i < Encoder.CBANDS; i++)
            {
                float bark1;
                int j2;
                bark1 = freq2bark(sfreq * j);
                b_frq[i] = sfreq * j;
                for (j2 = j; freq2bark(sfreq * j2) - bark1 < DELBARK && j2 <= blksize / 2; j2++)
                    ;

                numlines[i] = j2 - j;
                ni = i + 1;
                while (j < j2)
                {
                    Debug.Assert(j < Encoder.HBLKSIZE);
                    partition[j++] = i;
                }

                if (j > blksize / 2)
                {
                    j = blksize / 2;
                    ++i;
                    break;
                }
            }

            Debug.Assert(i < Encoder.CBANDS);
            b_frq[i] = sfreq * j;
            for (var sfb = 0; sfb < sbmax; sfb++)
            {
                int i1, i2, start, end;
                float arg;
                start = scalepos[sfb];
                end = scalepos[sfb + 1];
                i1 = (int)Math.Floor(.5 + deltafreq * (start - .5));
                if (i1 < 0)
                    i1 = 0;

                i2 = (int)Math.Floor(.5 + deltafreq * (end - .5));
                if (i2 > blksize / 2)
                    i2 = blksize / 2;

                bm[sfb] = (partition[i1] + partition[i2]) / 2;
                bo[sfb] = partition[i2];
                var f_tmp = sample_freq_frac * end;
                bo_w[sfb] = (f_tmp - b_frq[bo[sfb]]) / (b_frq[bo[sfb] + 1] - b_frq[bo[sfb]]);
                if (bo_w[sfb] < 0)
                {
                    bo_w[sfb] = 0;
                }
                else
                {
                    if (bo_w[sfb] > 1)
                        bo_w[sfb] = 1;
                }

                arg = freq2bark(sfreq * scalepos[sfb] * deltafreq);
                arg = (float)Math.Min(arg, 15.5) / 15.5f;
                mld[sfb] = (float)Math.Pow(10.0, 1.25 * (1 - Math.Cos(Math.PI * arg)) - 2.5);
            }

            j = 0;
            for (var k = 0; k < ni; k++)
            {

                var w = numlines[k];
                float bark1, bark2;
                bark1 = freq2bark(sfreq * j);
                bark2 = freq2bark(sfreq * (j + w - 1));
                bval[k] = .5f * (bark1 + bark2);
                bark1 = freq2bark(sfreq * (j - .5f));
                bark2 = freq2bark(sfreq * (j + w - .5f));
                bval_width[k] = bark2 - bark1;
                j += w;
            }

            return ni;
        }

        private float[] init_s3_values(
            int[][] s3ind,
            int npart,
            float[] bval,
            float[] bval_width,
            float[] norm,
            bool use_old_s3)
        {

            var s3 = Arrays.ReturnRectangularArray<float>(Encoder.CBANDS, Encoder.CBANDS);
            int j;
            var numberOfNoneZero = 0;
            if (use_old_s3)
                for (var i = 0; i < npart; i++)
                for (j = 0; j < npart; j++)
                {
                    var v = s3_func(bval[i] - bval[j]) * bval_width[j];
                    s3[i][j] = v * norm[i];
                }
            else
                for (j = 0; j < npart; j++)
                {
                    var hf_slope = 15 + Math.Min(21 / bval[j], 12);
                    var s3_x_norm = norm_s3_func_x(hf_slope);
                    for (var i = 0; i < npart; i++)
                    {
                        var v = s3_x_norm * s3_func_x(bval[i] - bval[j], hf_slope) * bval_width[j];
                        s3[i][j] = v * norm[i];
                    }
                }

            for (var i = 0; i < npart; i++)
            {
                for (j = 0; j < npart; j++)
                    if (s3[i][j] > 0.0f)
                        break;

                s3ind[i][0] = j;
                for (j = npart - 1; j > 0; j--)
                    if (s3[i][j] > 0.0f)
                        break;

                s3ind[i][1] = j;
                numberOfNoneZero += s3ind[i][1] - s3ind[i][0] + 1;
            }

            var p = new float[numberOfNoneZero];
            var k = 0;
            for (var i = 0; i < npart; i++)
            for (j = s3ind[i][0]; j <= s3ind[i][1]; j++)
                p[k++] = s3[i][j];

            return p;
        }

        private float stereo_demask(double f)
        {
            double arg = freq2bark((float)f);
            arg = Math.Min(arg, 15.5) / 15.5;
            return (float)Math.Pow(10.0, 1.25 * (1 - Math.Cos(Math.PI * arg)) - 2.5);
        }

        internal int psymodel_init(LameGlobalFlags gfp)
        {

            var gfc = gfp.internal_flags;
            int i;
            var useOldS3 = true;
            float bvl_a = 13, bvl_b = 24;
            float snr_l_a = 0, snr_l_b = 0;
            float snr_s_a = -8.25f, snr_s_b = -4.5f;
            var bval = new float[Encoder.CBANDS];
            var bval_width = new float[Encoder.CBANDS];
            var norm = new float[Encoder.CBANDS];

            float sfreq = gfp.out_samplerate;
            switch (gfp.experimentalZ)
            {
                default:
                    goto case 0;
                case 0:
                    useOldS3 = true;
                    break;
                case 1:
                    useOldS3 = gfp.VBR == VbrMode.vbr_mtrh || gfp.VBR == VbrMode.vbr_mt
                        ? false
                        : true;
                    break;
                case 2:
                    useOldS3 = false;
                    break;
                case 3:
                    bvl_a = 8;
                    snr_l_a = -1.75f;
                    snr_l_b = -0.0125f;
                    snr_s_a = -8.25f;
                    snr_s_b = -2.25f;
                    break;
            }

            gfc.ms_ener_ratio_old = .25f;
            gfc.blocktype_old[0] = gfc.blocktype_old[1] = Encoder.NORM_TYPE;
            for (i = 0; i < 4; ++i)
            {
                for (var jk = 0; jk < Encoder.CBANDS; ++jk)
                {
                    gfc.nb_1[i][jk] = 1e20f;
                    gfc.nb_2[i][jk] = 1e20f;
                    gfc.nb_s1[i][jk] = gfc.nb_s2[i][jk] = 1.0f;
                }

                for (var sb = 0; sb < Encoder.SBMAX_l; sb++)
                {
                    gfc.en[i].l[sb] = 1e20f;
                    gfc.thm[i].l[sb] = 1e20f;
                }

                for (var jj = 0; jj < 3; ++jj)
                {
                    for (var sb = 0; sb < Encoder.SBMAX_s; sb++)
                    {
                        gfc.en[i].s[sb][jj] = 1e20f;
                        gfc.thm[i].s[sb][jj] = 1e20f;
                    }

                    gfc.nsPsy.lastAttacks[i] = 0;
                }

                for (var jjj = 0; jjj < 9; jjj++)
                    gfc.nsPsy.last_en_subshort[i][jjj] = 10.0f;
            }

            gfc.loudness_sq_save[0] = gfc.loudness_sq_save[1] = 0.0f;
            gfc.npart_l = init_numline(
                gfc.numlines_l,
                gfc.bo_l,
                gfc.bm_l,
                bval,
                bval_width,
                gfc.mld_l,
                gfc.PSY.bo_l_weight,
                sfreq,
                Encoder.BLKSIZE,
                gfc.scalefac_band.l,
                Encoder.BLKSIZE / (2.0f * 576f),
                Encoder.SBMAX_l);
            Debug.Assert(gfc.npart_l < Encoder.CBANDS);
            for (i = 0; i < gfc.npart_l; i++)
            {
                double snr = snr_l_a;
                if (bval[i] >= bvl_a)
                    snr = snr_l_b * (bval[i] - bvl_a) / (bvl_b - bvl_a) + snr_l_a * (bvl_b - bval[i]) / (bvl_b - bvl_a);

                norm[i] = (float)Math.Pow(10.0, snr / 10.0);
                if (gfc.numlines_l[i] > 0)
                    gfc.rnumlines_l[i] = 1.0f / gfc.numlines_l[i];
                else
                    gfc.rnumlines_l[i] = 0;
            }

            gfc.s3_ll = init_s3_values(gfc.s3ind, gfc.npart_l, bval, bval_width, norm, useOldS3);
            var j = 0;
            for (i = 0; i < gfc.npart_l; i++)
            {
                double x;
                x = float.MaxValue;
                for (var k = 0; k < gfc.numlines_l[i]; k++, j++)
                {

                    var freq = sfreq * j / (1000.0f * Encoder.BLKSIZE);
                    float level;
                    level = ATHformula(freq * 1000, gfp) - 20;
                    level = (float)Math.Pow(10.0, 0.1 * level);
                    level *= gfc.numlines_l[i];
                    if (x > level)
                        x = level;
                }

                gfc.ATH.cb_l[i] = (float)x;
                x = -20 + bval[i] * 20 / 10;
                if (x > 6)
                    x = 100;

                if (x < -15)
                    x = -15;

                x -= 8.0;
                gfc.minval_l[i] = (float)(Math.Pow(10.0, x / 10.0) * gfc.numlines_l[i]);
            }

            gfc.npart_s = init_numline(
                gfc.numlines_s,
                gfc.bo_s,
                gfc.bm_s,
                bval,
                bval_width,
                gfc.mld_s,
                gfc.PSY.bo_s_weight,
                sfreq,
                Encoder.BLKSIZE_s,
                gfc.scalefac_band.s,
                Encoder.BLKSIZE_s / (2.0f * 192),
                Encoder.SBMAX_s);
            Debug.Assert(gfc.npart_s < Encoder.CBANDS);
            j = 0;
            for (i = 0; i < gfc.npart_s; i++)
            {
                double x;
                double snr = snr_s_a;
                if (bval[i] >= bvl_a)
                    snr = snr_s_b * (bval[i] - bvl_a) / (bvl_b - bvl_a) + snr_s_a * (bvl_b - bval[i]) / (bvl_b - bvl_a);

                norm[i] = (float)Math.Pow(10.0, snr / 10.0);
                x = float.MaxValue;
                for (var k = 0; k < gfc.numlines_s[i]; k++, j++)
                {

                    var freq = sfreq * j / (1000.0f * Encoder.BLKSIZE_s);
                    float level;
                    level = ATHformula(freq * 1000, gfp) - 20;
                    level = (float)Math.Pow(10.0, 0.1 * level);
                    level *= gfc.numlines_s[i];
                    if (x > level)
                        x = level;
                }

                gfc.ATH.cb_s[i] = (float)x;
                x = -7.0 + bval[i] * 7.0 / 12.0;
                if (bval[i] > 12)
                    x *= 1 + Math.Log(1 + x) * 3.1;

                if (bval[i] < 12)
                    x *= 1 + Math.Log(1 - x) * 2.3;

                if (x < -15)
                    x = -15;

                x -= 8;
                gfc.minval_s[i] = (float)Math.Pow(10.0, x / 10) * gfc.numlines_s[i];
            }

            gfc.s3_ss = init_s3_values(gfc.s3ind_s, gfc.npart_s, bval, bval_width, norm, useOldS3);
            init_mask_add_max_values();
            fft.init_fft(gfc);
            gfc.decay = (float)Math.Exp(-1.0 * LOG10 / (temporalmask_sustain_sec * sfreq / 192.0));
            {
                float msfix;
                msfix = NS_MSFIX;
                if ((gfp.exp_nspsytune & 2) != 0)
                    msfix = 1.0f;

                if (Math.Abs(gfp.msfix) > 0.0)
                    msfix = gfp.msfix;

                gfp.msfix = msfix;
                for (var b = 0; b < gfc.npart_l; b++)
                    if (gfc.s3ind[b][1] > gfc.npart_l - 1)
                        gfc.s3ind[b][1] = gfc.npart_l - 1;
            }
            var frame_duration = 576.0f * gfc.mode_gr / sfreq;
            gfc.ATH.decay = (float)Math.Pow(10.0, -12.0 / 10.0 * frame_duration);
            gfc.ATH.adjust = 0.01f;
            gfc.ATH.adjustLimit = 1.0f;
            Debug.Assert(gfc.bo_l[Encoder.SBMAX_l - 1] <= gfc.npart_l);
            Debug.Assert(gfc.bo_s[Encoder.SBMAX_s - 1] <= gfc.npart_s);
            if (gfp.ATHtype != -1)
            {
                float freq;

                var freq_inc = gfp.out_samplerate / (float)Encoder.BLKSIZE;
                var eql_balance = 0.0f;
                freq = 0.0f;
                for (i = 0; i < Encoder.BLKSIZE / 2; ++i)
                {
                    freq += freq_inc;
                    gfc.ATH.eql_w[i] = 1.0f / (float)Math.Pow(10, ATHformula(freq, gfp) / 10);
                    eql_balance += gfc.ATH.eql_w[i];
                }

                eql_balance = 1.0f / eql_balance;
                for (i = Encoder.BLKSIZE / 2; --i >= 0;)
                    gfc.ATH.eql_w[i] *= eql_balance;
            }

            {
                for (var b = j = 0; b < gfc.npart_s; ++b)
                for (i = 0; i < gfc.numlines_s[b]; ++i)
                    ++j;

                Debug.Assert(j == 129);
                for (var b = j = 0; b < gfc.npart_l; ++b)
                for (i = 0; i < gfc.numlines_l[b]; ++i)
                    ++j;

                Debug.Assert(j == 513);
            }
            j = 0;
            for (i = 0; i < gfc.npart_l; i++)
            {

                var freq = sfreq * (j + gfc.numlines_l[i] / 2) / (1.0f * Encoder.BLKSIZE);
                gfc.mld_cb_l[i] = stereo_demask(freq);
                j += gfc.numlines_l[i];
            }

            for (; i < Encoder.CBANDS; ++i)
                gfc.mld_cb_l[i] = 1;

            j = 0;
            for (i = 0; i < gfc.npart_s; i++)
            {

                var freq = sfreq * (j + gfc.numlines_s[i] / 2) / (1.0f * Encoder.BLKSIZE_s);
                gfc.mld_cb_s[i] = stereo_demask(freq);
                j += gfc.numlines_s[i];
            }

            for (; i < Encoder.CBANDS; ++i)
                gfc.mld_cb_s[i] = 1;

            return 0;
        }

        private float ATHformula_GB(float f, float value)
        {
            if (f < -.3)
                f = 3410;

            f /= 1000;
            f = (float)Math.Max(0.1, f);
            var ath = 3.640f * (float)Math.Pow(f, -0.8) - 6.800f * (float)Math.Exp(-0.6 * Math.Pow(f - 3.4, 2.0)) +
                      6.000f * (float)Math.Exp(-0.15 * Math.Pow(f - 8.7, 2.0)) +
                      (0.6f + 0.04f * value) * 0.001f * (float)Math.Pow(f, 4.0);
            return ath;
        }

        internal float ATHformula(float f, LameGlobalFlags gfp)
        {
            float ath;
            switch (gfp.ATHtype)
            {
                case 0:
                    ath = ATHformula_GB(f, 9);
                    break;
                case 1:
                    ath = ATHformula_GB(f, -1);
                    break;
                case 2:
                    ath = ATHformula_GB(f, 0);
                    break;
                case 3:
                    ath = ATHformula_GB(f, 1) + 6;
                    break;
                case 4:
                    ath = ATHformula_GB(f, gfp.ATHcurve);
                    break;
                default:
                    ath = ATHformula_GB(f, 0);
                    break;
            }

            return ath;
        }
    }
}