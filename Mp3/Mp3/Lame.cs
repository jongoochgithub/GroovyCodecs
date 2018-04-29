using System;
using System.Diagnostics;
using GroovyCodecs.Mp3.Common;
using GroovyCodecs.Mp3.Mpg;
using GroovyCodecs.Types;

/*  *      LAME MP3 encoding engine  *
 *      Copyright (c) 1999-2000 Mark Taylor
 *      Copyright (c) 2000-2005 Takehiro Tominaga
 *      Copyright (c) 2000-2005 Robert Hegemann
 *      Copyright (c) 2000-2005 Gabriel Bouvigne
 *      Copyright (c) 2000-2004 Alexander Leidinger
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
/* $Id: Lame.java,v 1.38 2011/05/24 21:15:54 kenchis Exp $
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        */
namespace GroovyCodecs.Mp3.Mp3
{

    internal class Lame
    {

        protected internal class LowPassHighPass
        {
            internal double lowerlimit;
        }

        private class BandPass
        {

            internal readonly int lowpass;

            internal BandPass(int bitrate, int lPass)
            {
                lowpass = lPass;
            }
        }

        protected internal class NumUsed
        {
            internal int num_used;
        }

        protected internal class InOut
        {
            internal int n_in;

            internal int n_out;
        }

        internal const int EXTREME = 1002;

        internal const int EXTREME_FAST = 1005;

        internal const int INSANE = 1003;

        private const int LAME_DEFAULT_QUALITY = 3;

        internal const long LAME_ID = 0xFFF88E3B;

        internal const int LAME_MAXALBUMART = 128 * 1024;

        internal static readonly int LAME_MAXMP3BUFFER = 16384 + LAME_MAXALBUMART;

        internal const int MEDIUM = 1006;

        internal const int MEDIUM_FAST = 1007;

        internal const int R3MIX = 1000;

        internal const int STANDARD = 1001;

        internal const int STANDARD_FAST = 1004;

        internal const int V0 = 500;

        internal const int V1 = 490;

        internal const int V2 = 480;

        internal const int V3 = 470;

        internal const int V4 = 460;

        internal const int V5 = 450;

        internal const int V6 = 440;

        internal const int V7 = 430;

        internal const int V8 = 420;

        internal const int V9 = 410;

        internal BitStream bs;

        internal Encoder enc = new Encoder();

        internal GainAnalysis ga;

        internal ID3Tag id3;

        internal MPGLib mpglib;

        internal Presets p;

        internal PsyModel psy = new PsyModel();

        internal Quantize qu;

        internal QuantizePVT qupvt;

        internal VBRTag vbr;

        internal Mp3Version ver;

        internal void setModules(
            GainAnalysis ga,
            BitStream bs,
            Presets p,
            QuantizePVT qupvt,
            Quantize qu,
            VBRTag vbr,
            Mp3Version ver,
            ID3Tag id3,
            MPGLib mpglib)
        {
            this.ga = ga;
            this.bs = bs;
            this.p = p;
            this.qupvt = qupvt;
            this.qu = qu;
            this.vbr = vbr;
            this.ver = ver;
            this.id3 = id3;
            this.mpglib = mpglib;
            enc.setModules(bs, psy, qupvt, vbr);
        }

        private float filter_coef(float x)
        {
            if (x > 1.0)
                return 0.0f;

            if (x <= 0.0)
                return 1.0f;

            return (float)Math.Cos(Math.PI / 2 * x);
        }

        private void lame_init_params_ppflt(LameGlobalFlags gfp)
        {

            var gfc = gfp.internal_flags;
            var lowpass_band = 32;
            var highpass_band = -1;
            if (gfc.lowpass1 > 0)
            {
                var minband = 999;
                for (var band = 0; band <= 31; band++)
                {
                    var freq = (float)(band / 31.0);
                    if (freq >= gfc.lowpass2)
                        lowpass_band = Math.Min(lowpass_band, band);

                    if (gfc.lowpass1 < freq && freq < gfc.lowpass2)
                        minband = Math.Min(minband, band);
                }

                if (minband == 999)
                    gfc.lowpass1 = (lowpass_band - .75f) / 31.0f;
                else
                    gfc.lowpass1 = (minband - .75f) / 31.0f;

                gfc.lowpass2 = lowpass_band / 31.0f;
            }

            if (gfc.highpass2 > 0)
                if (gfc.highpass2 < .9 * (.75 / 31.0))
                {
                    gfc.highpass1 = 0;
                    gfc.highpass2 = 0;
                    Console.Error.WriteLine("Warning: highpass filter disabled.  " + "highpass frequency too small\n");
                }

            if (gfc.highpass2 > 0)
            {
                var maxband = -1;
                for (var band = 0; band <= 31; band++)
                {
                    var freq = band / 31.0f;
                    if (freq <= gfc.highpass1)
                        highpass_band = Math.Max(highpass_band, band);

                    if (gfc.highpass1 < freq && freq < gfc.highpass2)
                        maxband = Math.Max(maxband, band);
                }

                gfc.highpass1 = highpass_band / 31.0f;
                if (maxband == -1)
                    gfc.highpass2 = (highpass_band + .75f) / 31.0f;
                else
                    gfc.highpass2 = (maxband + .75f) / 31.0f;
            }

            for (var band = 0; band < 32; band++)
            {
                double fc1, fc2;
                var freq = band / 31.0f;
                if (gfc.highpass2 > gfc.highpass1)
                    fc1 = filter_coef((gfc.highpass2 - freq) / (gfc.highpass2 - gfc.highpass1 + 1e-20f));
                else
                    fc1 = 1.0;

                if (gfc.lowpass2 > gfc.lowpass1)
                    fc2 = filter_coef((freq - gfc.lowpass1) / (gfc.lowpass2 - gfc.lowpass1 + 1e-20f));
                else
                    fc2 = 1.0;

                gfc.amp_filter[band] = (float)(fc1 * fc2);
            }
        }

        private void optimum_bandwidth(LowPassHighPass lh, int bitrate)
        {

            var freq_map = new[]
            {
                new BandPass(8, 2000),
                new BandPass(16, 3700),
                new BandPass(24, 3900),
                new BandPass(32, 5500),
                new BandPass(40, 7000),
                new BandPass(48, 7500),
                new BandPass(56, 10000),
                new BandPass(64, 11000),
                new BandPass(80, 13500),
                new BandPass(96, 15100),
                new BandPass(112, 15600),
                new BandPass(128, 17000),
                new BandPass(160, 17500),
                new BandPass(192, 18600),
                new BandPass(224, 19400),
                new BandPass(256, 19700),
                new BandPass(320, 20500)
            };
            var table_index = nearestBitrateFullIndex(bitrate);
            lh.lowerlimit = freq_map[table_index].lowpass;
        }

        private int optimum_samplefreq(int lowpassfreq, int input_samplefreq)
        {
            var suggested_samplefreq = 44100;
            if (input_samplefreq >= 48000)
                suggested_samplefreq = 48000;
            else if (input_samplefreq >= 44100)
                suggested_samplefreq = 44100;
            else if (input_samplefreq >= 32000)
                suggested_samplefreq = 32000;
            else if (input_samplefreq >= 24000)
                suggested_samplefreq = 24000;
            else if (input_samplefreq >= 22050)
                suggested_samplefreq = 22050;
            else if (input_samplefreq >= 16000)
                suggested_samplefreq = 16000;
            else if (input_samplefreq >= 12000)
                suggested_samplefreq = 12000;
            else if (input_samplefreq >= 11025)
                suggested_samplefreq = 11025;
            else if (input_samplefreq >= 8000)
                suggested_samplefreq = 8000;

            if (lowpassfreq == -1)
                return suggested_samplefreq;

            if (lowpassfreq <= 15960)
                suggested_samplefreq = 44100;

            if (lowpassfreq <= 15250)
                suggested_samplefreq = 32000;

            if (lowpassfreq <= 11220)
                suggested_samplefreq = 24000;

            if (lowpassfreq <= 9970)
                suggested_samplefreq = 22050;

            if (lowpassfreq <= 7230)
                suggested_samplefreq = 16000;

            if (lowpassfreq <= 5420)
                suggested_samplefreq = 12000;

            if (lowpassfreq <= 4510)
                suggested_samplefreq = 11025;

            if (lowpassfreq <= 3970)
                suggested_samplefreq = 8000;

            if (input_samplefreq < suggested_samplefreq)
            {
                if (input_samplefreq > 44100)
                    return 48000;

                if (input_samplefreq > 32000)
                    return 44100;

                if (input_samplefreq > 24000)
                    return 32000;

                if (input_samplefreq > 22050)
                    return 24000;

                if (input_samplefreq > 16000)
                    return 22050;

                if (input_samplefreq > 12000)
                    return 16000;

                if (input_samplefreq > 11025)
                    return 12000;

                if (input_samplefreq > 8000)
                    return 11025;

                return 8000;
            }

            return suggested_samplefreq;
        }

        private void lame_init_qval(LameGlobalFlags gfp)
        {

            var gfc = gfp.internal_flags;
            switch (gfp.quality)
            {
                default:
                    goto case 9;
                case 9:
                    gfc.psymodel = 0;
                    gfc.noise_shaping = 0;
                    gfc.noise_shaping_amp = 0;
                    gfc.noise_shaping_stop = 0;
                    gfc.use_best_huffman = 0;
                    gfc.full_outer_loop = 0;
                    break;
                case 8:
                    gfp.quality = 7;
                    goto case 7;
                case 7:
                    gfc.psymodel = 1;
                    gfc.noise_shaping = 0;
                    gfc.noise_shaping_amp = 0;
                    gfc.noise_shaping_stop = 0;
                    gfc.use_best_huffman = 0;
                    gfc.full_outer_loop = 0;
                    break;
                case 6:
                    gfc.psymodel = 1;
                    if (gfc.noise_shaping == 0)
                        gfc.noise_shaping = 1;

                    gfc.noise_shaping_amp = 0;
                    gfc.noise_shaping_stop = 0;
                    if (gfc.subblock_gain == -1)
                        gfc.subblock_gain = 1;

                    gfc.use_best_huffman = 0;
                    gfc.full_outer_loop = 0;
                    break;
                case 5:
                    gfc.psymodel = 1;
                    if (gfc.noise_shaping == 0)
                        gfc.noise_shaping = 1;

                    gfc.noise_shaping_amp = 0;
                    gfc.noise_shaping_stop = 0;
                    if (gfc.subblock_gain == -1)
                        gfc.subblock_gain = 1;

                    gfc.use_best_huffman = 0;
                    gfc.full_outer_loop = 0;
                    break;
                case 4:
                    gfc.psymodel = 1;
                    if (gfc.noise_shaping == 0)
                        gfc.noise_shaping = 1;

                    gfc.noise_shaping_amp = 0;
                    gfc.noise_shaping_stop = 0;
                    if (gfc.subblock_gain == -1)
                        gfc.subblock_gain = 1;

                    gfc.use_best_huffman = 1;
                    gfc.full_outer_loop = 0;
                    break;
                case 3:
                    gfc.psymodel = 1;
                    if (gfc.noise_shaping == 0)
                        gfc.noise_shaping = 1;

                    gfc.noise_shaping_amp = 1;
                    gfc.noise_shaping_stop = 1;
                    if (gfc.subblock_gain == -1)
                        gfc.subblock_gain = 1;

                    gfc.use_best_huffman = 1;
                    gfc.full_outer_loop = 0;
                    break;
                case 2:
                    gfc.psymodel = 1;
                    if (gfc.noise_shaping == 0)
                        gfc.noise_shaping = 1;

                    if (gfc.substep_shaping == 0)
                        gfc.substep_shaping = 2;

                    gfc.noise_shaping_amp = 1;
                    gfc.noise_shaping_stop = 1;
                    if (gfc.subblock_gain == -1)
                        gfc.subblock_gain = 1;

                    gfc.use_best_huffman = 1;
                    gfc.full_outer_loop = 0;
                    break;
                case 1:
                    gfc.psymodel = 1;
                    if (gfc.noise_shaping == 0)
                        gfc.noise_shaping = 1;

                    if (gfc.substep_shaping == 0)
                        gfc.substep_shaping = 2;

                    gfc.noise_shaping_amp = 2;
                    gfc.noise_shaping_stop = 1;
                    if (gfc.subblock_gain == -1)
                        gfc.subblock_gain = 1;

                    gfc.use_best_huffman = 1;
                    gfc.full_outer_loop = 0;
                    break;
                case 0:
                    gfc.psymodel = 1;
                    if (gfc.noise_shaping == 0)
                        gfc.noise_shaping = 1;

                    if (gfc.substep_shaping == 0)
                        gfc.substep_shaping = 2;

                    gfc.noise_shaping_amp = 2;
                    gfc.noise_shaping_stop = 1;
                    if (gfc.subblock_gain == -1)
                        gfc.subblock_gain = 1;

                    gfc.use_best_huffman = 1;
                    gfc.full_outer_loop = 0;
                    break;
            }
        }

        private double linear_int(double a, double b, double m)
        {
            return a + m * (b - a);
        }

        private int FindNearestBitrate(int bRate, int version, int samplerate)
        {
            if (samplerate < 16000)
                version = 2;

            var bitrate = Tables.bitrate_table[version][1];
            for (var i = 2; i <= 14; i++)
                if (Tables.bitrate_table[version][i] > 0)
                    if (Math.Abs(Tables.bitrate_table[version][i] - bRate) < Math.Abs(bitrate - bRate))
                        bitrate = Tables.bitrate_table[version][i];

            return bitrate;
        }

        internal int nearestBitrateFullIndex(int bitrate)
        {

            var full_bitrate_table = new[]
            {
                8,
                16,
                24,
                32,
                40,
                48,
                56,
                64,
                80,
                96,
                112,
                128,
                160,
                192,
                224,
                256,
                320
            };
            int lower_range = 0, lower_range_kbps = 0, upper_range = 0, upper_range_kbps = 0;
            upper_range_kbps = full_bitrate_table[16];
            upper_range = 16;
            lower_range_kbps = full_bitrate_table[16];
            lower_range = 16;
            for (var b = 0; b < 16; b++)
                if (Math.Max(bitrate, full_bitrate_table[b + 1]) != bitrate)
                {
                    upper_range_kbps = full_bitrate_table[b + 1];
                    upper_range = b + 1;
                    lower_range_kbps = full_bitrate_table[b];
                    lower_range = b;
                    break;
                }

            if (upper_range_kbps - bitrate > bitrate - lower_range_kbps)
                return lower_range;

            return upper_range;
        }

        private int map2MP3Frequency(int freq)
        {
            if (freq <= 8000)
                return 8000;

            if (freq <= 11025)
                return 11025;

            if (freq <= 12000)
                return 12000;

            if (freq <= 16000)
                return 16000;

            if (freq <= 22050)
                return 22050;

            if (freq <= 24000)
                return 24000;

            if (freq <= 32000)
                return 32000;

            if (freq <= 44100)
                return 44100;

            return 48000;
        }

        private int SmpFrqIndex(int sample_freq, LameGlobalFlags gpf)
        {
            switch (sample_freq)
            {
                case 44100:
                    gpf.version = 1;
                    return 0;
                case 48000:
                    gpf.version = 1;
                    return 1;
                case 32000:
                    gpf.version = 1;
                    return 2;
                case 22050:
                    gpf.version = 0;
                    return 0;
                case 24000:
                    gpf.version = 0;
                    return 1;
                case 16000:
                    gpf.version = 0;
                    return 2;
                case 11025:
                    gpf.version = 0;
                    return 0;
                case 12000:
                    gpf.version = 0;
                    return 1;
                case 8000:
                    gpf.version = 0;
                    return 2;
                default:
                    gpf.version = 0;
                    return -1;
            }
        }

        internal int BitrateIndex(int bRate, int version, int samplerate)
        {
            if (samplerate < 16000)
                version = 2;

            for (var i = 0; i <= 14; i++)
                if (Tables.bitrate_table[version][i] > 0)
                    if (Tables.bitrate_table[version][i] == bRate)
                        return i;

            return -1;
        }

        private float blackman(float x, float fcn, int l)
        {
            var wcn = (float)(Math.PI * fcn);
            x /= l;
            if (x < 0)
                x = 0;

            if (x > 1)
                x = 1;

            var x2 = x - .5f;
            var bkwn = 0.42f - 0.5f * (float)Math.Cos(2 * x * Math.PI) + 0.08f * (float)Math.Cos(4 * x * Math.PI);
            if (Math.Abs(x2) < 1e-9)
                return (float)(wcn / Math.PI);
            return (float)(bkwn * Math.Sin(l * wcn * x2) / (Math.PI * l * x2));
        }

        private int gcd(int i, int j)
        {
            return j != 0 ? gcd(j, i % j) : i;
        }

        private int fill_buffer_resample(
            LameGlobalFlags gfp,
            float[] outbuf,
            int outbufPos,
            int desired_len,
            float[] inbuf,
            int in_bufferPos,
            int len,
            NumUsed num_used,
            int ch)
        {

            var gfc = gfp.internal_flags;
            int i, j = 0, k;
            var bpc = gfp.out_samplerate / gcd(gfp.out_samplerate, gfp.in_samplerate);
            if (bpc > LameInternalFlags.BPC)
                bpc = LameInternalFlags.BPC;

            float intratio = Math.Abs(gfc.resample_ratio - Math.Floor(.5 + gfc.resample_ratio)) < .0001 ? 1 : 0;
            var fcn = 1.00f / (float)gfc.resample_ratio;
            if (fcn > 1.00)
                fcn = 1.00f;

            var filter_l = 31;
            if (0 == filter_l % 2)
                --filter_l;

            filter_l += (int)intratio;
            var BLACKSIZE = filter_l + 1;
            if (gfc.fill_buffer_resample_init == 0)
            {
                gfc.inbuf_old[0] = new float[BLACKSIZE];
                gfc.inbuf_old[1] = new float[BLACKSIZE];
                for (i = 0; i <= 2 * bpc; ++i)
                    gfc.blackfilt[i] = new float[BLACKSIZE];

                gfc.itime[0] = 0;
                gfc.itime[1] = 0;
                for (j = 0; j <= 2 * bpc; j++)
                {
                    var sum = 0.0f;
                    var offset = (j - bpc) / (2.0f * bpc);
                    for (i = 0; i <= filter_l; i++)
                        sum += gfc.blackfilt[j][i] = blackman(i - offset, fcn, filter_l);

                    for (i = 0; i <= filter_l; i++)
                        gfc.blackfilt[j][i] /= sum;
                }

                gfc.fill_buffer_resample_init = 1;
            }

            var inbuf_old = gfc.inbuf_old[ch];
            for (k = 0; k < desired_len; k++)
            {
                double time0;
                int joff;
                time0 = k * gfc.resample_ratio;
                j = (int)Math.Floor(time0 - gfc.itime[ch]);
                if (filter_l + j - filter_l / 2 >= len)
                    break;

                var offset = (float)(time0 - gfc.itime[ch] - (j + .5 * (filter_l % 2)));
                Debug.Assert(Math.Abs(offset) <= .501);
                joff = (int)Math.Floor(offset * 2 * bpc + bpc + .5);
                var xvalue = 0.0f;
                for (i = 0; i <= filter_l; ++i)
                {
                    var j2 = i + j - filter_l / 2;
                    float y;
                    Debug.Assert(j2 < len);
                    Debug.Assert(j2 + BLACKSIZE >= 0);
                    y = j2 < 0 ? inbuf_old[BLACKSIZE + j2] : inbuf[in_bufferPos + j2];
                    xvalue += y * gfc.blackfilt[joff][i];
                }

                outbuf[outbufPos + k] = xvalue;
            }

            num_used.num_used = Math.Min(len, filter_l + j - filter_l / 2);
            gfc.itime[ch] += num_used.num_used - k * gfc.resample_ratio;
            if (num_used.num_used >= BLACKSIZE)
            {
                for (i = 0; i < BLACKSIZE; i++)
                    inbuf_old[i] = inbuf[in_bufferPos + num_used.num_used + i - BLACKSIZE];
            }
            else
            {
                var n_shift = BLACKSIZE - num_used.num_used;
                for (i = 0; i < n_shift; ++i)
                    inbuf_old[i] = inbuf_old[i + num_used.num_used];

                for (j = 0; i < BLACKSIZE; ++i, ++j)
                    inbuf_old[i] = inbuf[in_bufferPos + j];

                Debug.Assert(j == num_used.num_used);
            }

            return k;
        }

        private void fill_buffer(
            LameGlobalFlags gfp,
            float[][] mfbuf,
            float[][] in_buffer,
            int in_bufferPos,
            int nsamples,
            InOut io)
        {

            var gfc = gfp.internal_flags;
            if (gfc.resample_ratio < .9999 || gfc.resample_ratio > 1.0001)
            {
                for (var ch = 0; ch < gfc.channels_out; ch++)
                {
                    var numUsed = new NumUsed();
                    io.n_out = fill_buffer_resample(
                        gfp,
                        mfbuf[ch],
                        gfc.mf_size,
                        gfp.framesize,
                        in_buffer[ch],
                        in_bufferPos,
                        nsamples,
                        numUsed,
                        ch);
                    io.n_in = numUsed.num_used;
                }
            }
            else
            {
                io.n_out = Math.Min(gfp.framesize, nsamples);
                io.n_in = io.n_out;
                for (var i = 0; i < io.n_out; ++i)
                {
                    mfbuf[0][gfc.mf_size + i] = in_buffer[0][in_bufferPos + i];
                    if (gfc.channels_out == 2)
                        mfbuf[1][gfc.mf_size + i] = in_buffer[1][in_bufferPos + i];
                }
            }
        }

        internal int lame_init_params(LameGlobalFlags gfp)
        {
            var gfc = gfp.internal_flags;
            gfc.Class_ID = 0;
            if (gfc.ATH == null)
                gfc.ATH = new ATH();

            if (gfc.PSY == null)
                gfc.PSY = new PSY();

            if (gfc.rgdata == null)
                gfc.rgdata = new ReplayGain();

            gfc.channels_in = gfp.num_channels;
            if (gfc.channels_in == 1)
                gfp.mode = MPEGMode.MONO;

            gfc.channels_out = gfp.mode == MPEGMode.MONO ? 1 : 2;
            gfc.mode_ext = Encoder.MPG_MD_MS_LR;
            if (gfp.mode == MPEGMode.MONO)
                gfp.force_ms = false;

            if (gfp.VBR == VbrMode.vbr_off && gfp.VBR_mean_bitrate_kbps != 128 && gfp.brate == 0)
                gfp.brate = gfp.VBR_mean_bitrate_kbps;

            if (gfp.VBR == VbrMode.vbr_off || gfp.VBR == VbrMode.vbr_mtrh ||
                gfp.VBR == VbrMode.vbr_mt)
            {
            }
            else
            {
                gfp.free_format = false;
            }

            if (gfp.VBR == VbrMode.vbr_off && gfp.brate == 0)
                if (BitStream.EQ(gfp.compression_ratio, 0))
                    gfp.compression_ratio = 11.025f;

            if (gfp.VBR == VbrMode.vbr_off && gfp.compression_ratio > 0)
            {
                if (gfp.out_samplerate == 0)
                    gfp.out_samplerate = map2MP3Frequency((int)(0.97 * gfp.in_samplerate));

                gfp.brate = (int)(gfp.out_samplerate * 16 * gfc.channels_out / (1000 * gfp.compression_ratio));
                gfc.samplerate_index = SmpFrqIndex(gfp.out_samplerate, gfp);
                if (!gfp.free_format)
                    gfp.brate = FindNearestBitrate(gfp.brate, gfp.version, gfp.out_samplerate);
            }

            if (gfp.out_samplerate != 0)
                if (gfp.out_samplerate < 16000)
                {
                    gfp.VBR_mean_bitrate_kbps = Math.Max(gfp.VBR_mean_bitrate_kbps, 8);
                    gfp.VBR_mean_bitrate_kbps = Math.Min(gfp.VBR_mean_bitrate_kbps, 64);
                }
                else if (gfp.out_samplerate < 32000)
                {
                    gfp.VBR_mean_bitrate_kbps = Math.Max(gfp.VBR_mean_bitrate_kbps, 8);
                    gfp.VBR_mean_bitrate_kbps = Math.Min(gfp.VBR_mean_bitrate_kbps, 160);
                }
                else
                {
                    gfp.VBR_mean_bitrate_kbps = Math.Max(gfp.VBR_mean_bitrate_kbps, 32);
                    gfp.VBR_mean_bitrate_kbps = Math.Min(gfp.VBR_mean_bitrate_kbps, 320);
                }

            if (gfp.lowpassfreq == 0)
            {
                double lowpass = 16000;
                switch (gfp.VBR)
                {
                    case VbrMode.vbr_off:
                    {
                        var lh = new LowPassHighPass();
                        optimum_bandwidth(lh, gfp.brate);
                        lowpass = lh.lowerlimit;
                        break;
                    }
                    case VbrMode.vbr_abr:
                    {
                        var lh = new LowPassHighPass();
                        optimum_bandwidth(lh, gfp.VBR_mean_bitrate_kbps);
                        lowpass = lh.lowerlimit;
                        break;
                    }
                    case VbrMode.vbr_rh:
                    {

                        var x = new[]
                        {
                            19500,
                            19000,
                            18600,
                            18000,
                            17500,
                            16000,
                            15600,
                            14900,
                            12500,
                            10000,
                            3950
                        };
                        if (0 <= gfp.VBR_q && gfp.VBR_q <= 9)
                        {
                            double a = x[gfp.VBR_q], b = x[gfp.VBR_q + 1], m = gfp.VBR_q_frac;
                            lowpass = linear_int(a, b, m);
                        }
                        else
                        {
                            lowpass = 19500;
                        }

                        break;
                    }
                    default:
                    {

                        var x = new[]
                        {
                            19500,
                            19000,
                            18500,
                            18000,
                            17500,
                            16500,
                            15500,
                            14500,
                            12500,
                            9500,
                            3950
                        };
                        if (0 <= gfp.VBR_q && gfp.VBR_q <= 9)
                        {
                            double a = x[gfp.VBR_q], b = x[gfp.VBR_q + 1], m = gfp.VBR_q_frac;
                            lowpass = linear_int(a, b, m);
                        }
                        else
                        {
                            lowpass = 19500;
                        }
                    }
                        break;
                }

                if (gfp.mode == MPEGMode.MONO &&
                    (gfp.VBR == VbrMode.vbr_off || gfp.VBR == VbrMode.vbr_abr))
                    lowpass *= 1.5;

                gfp.lowpassfreq = (int)lowpass;
            }

            if (gfp.out_samplerate == 0)
            {
                if (2 * gfp.lowpassfreq > gfp.in_samplerate)
                    gfp.lowpassfreq = gfp.in_samplerate / 2;

                gfp.out_samplerate = optimum_samplefreq(gfp.lowpassfreq, gfp.in_samplerate);
            }

            gfp.lowpassfreq = Math.Min(20500, gfp.lowpassfreq);
            gfp.lowpassfreq = Math.Min(gfp.out_samplerate / 2, gfp.lowpassfreq);
            if (gfp.VBR == VbrMode.vbr_off)
                gfp.compression_ratio = gfp.out_samplerate * 16 * gfc.channels_out / (1000 * gfp.brate);

            if (gfp.VBR == VbrMode.vbr_abr)
                gfp.compression_ratio = gfp.out_samplerate * 16 * gfc.channels_out / (1000 * gfp.VBR_mean_bitrate_kbps);

            if (!gfp.bWriteVbrTag)
            {
                gfp.findReplayGain = false;
                gfp.decode_on_the_fly = false;
                gfc.findPeakSample = false;
            }

            gfc.findReplayGain = gfp.findReplayGain;
            gfc.decode_on_the_fly = gfp.decode_on_the_fly;
            if (gfc.decode_on_the_fly)
                gfc.findPeakSample = true;

            if (gfc.findReplayGain)
                if (ga.InitGainAnalysis(gfc.rgdata, gfp.out_samplerate) == GainAnalysis.INIT_GAIN_ANALYSIS_ERROR)
                {
                    gfp.internal_flags = null;
                    return -6;
                }

            if (gfc.decode_on_the_fly && !gfp.decode_only)
            {
                if (gfc.hip != null)
                    mpglib.hip_decode_exit(gfc.hip);

                gfc.hip = mpglib.hip_decode_init();
            }

            gfc.mode_gr = gfp.out_samplerate <= 24000 ? 1 : 2;
            gfp.framesize = 576 * gfc.mode_gr;
            gfp.encoder_delay = Encoder.ENCDELAY;
            gfc.resample_ratio = (double)gfp.in_samplerate / gfp.out_samplerate;
            switch (gfp.VBR)
            {
                case VbrMode.vbr_mt:
                case VbrMode.vbr_rh:
                case VbrMode.vbr_mtrh:
                {

                    var cmp = new[]
                    {
                        5.7f,
                        6.5f,
                        7.3f,
                        8.2f,
                        10f,
                        11.9f,
                        13f,
                        14f,
                        15f,
                        16.5f
                    };
                    gfp.compression_ratio = cmp[gfp.VBR_q];
                }
                    break;
                case VbrMode.vbr_abr:
                    gfp.compression_ratio =
                        (float)(gfp.out_samplerate * 16 * gfc.channels_out / (1000.0 * gfp.VBR_mean_bitrate_kbps));
                    break;
                default:
                    gfp.compression_ratio = (float)(gfp.out_samplerate * 16 * gfc.channels_out / (1000.0 * gfp.brate));
                    break;
            }

            if (gfp.mode == MPEGMode.NOT_SET)
                gfp.mode = MPEGMode.JOINT_STEREO;

            if (gfp.highpassfreq > 0)
            {
                gfc.highpass1 = 2.0f * gfp.highpassfreq;
                if (gfp.highpasswidth >= 0)
                    gfc.highpass2 = 2.0f * (gfp.highpassfreq + gfp.highpasswidth);
                else
                    gfc.highpass2 = (1 + 0.00f) * 2.0f * gfp.highpassfreq;

                gfc.highpass1 /= gfp.out_samplerate;
                gfc.highpass2 /= gfp.out_samplerate;
            }
            else
            {
                gfc.highpass1 = 0;
                gfc.highpass2 = 0;
            }

            if (gfp.lowpassfreq > 0)
            {
                gfc.lowpass2 = 2.0f * gfp.lowpassfreq;
                if (gfp.lowpasswidth >= 0)
                {
                    gfc.lowpass1 = 2.0f * (gfp.lowpassfreq - gfp.lowpasswidth);
                    if (gfc.lowpass1 < 0)
                        gfc.lowpass1 = 0;
                }
                else
                {
                    gfc.lowpass1 = (1 - 0.00f) * 2.0f * gfp.lowpassfreq;
                }

                gfc.lowpass1 /= gfp.out_samplerate;
                gfc.lowpass2 /= gfp.out_samplerate;
            }
            else
            {
                gfc.lowpass1 = 0;
                gfc.lowpass2 = 0;
            }

            lame_init_params_ppflt(gfp);
            gfc.samplerate_index = SmpFrqIndex(gfp.out_samplerate, gfp);
            if (gfc.samplerate_index < 0)
            {
                gfp.internal_flags = null;
                return -1;
            }

            if (gfp.VBR == VbrMode.vbr_off)
                if (gfp.free_format)
                {
                    gfc.bitrate_index = 0;
                }
                else
                {
                    gfp.brate = FindNearestBitrate(gfp.brate, gfp.version, gfp.out_samplerate);
                    gfc.bitrate_index = BitrateIndex(gfp.brate, gfp.version, gfp.out_samplerate);
                    if (gfc.bitrate_index <= 0)
                    {
                        gfp.internal_flags = null;
                        return -1;
                    }
                }
            else
                gfc.bitrate_index = 1;

            if (gfp.analysis)
                gfp.bWriteVbrTag = false;

            if (gfc.pinfo != null)
                gfp.bWriteVbrTag = false;

            bs.init_bit_stream_w(gfc);
            var j = gfc.samplerate_index + 3 * gfp.version + 6 * (gfp.out_samplerate < 16000 ? 1 : 0);
            for (var i = 0; i < Encoder.SBMAX_l + 1; i++)
                gfc.scalefac_band.l[i] = qupvt.sfBandIndex[j].l[i];

            for (var i = 0; i < Encoder.PSFB21 + 1; i++)
            {

                var size = (gfc.scalefac_band.l[22] - gfc.scalefac_band.l[21]) / Encoder.PSFB21;

                var start = gfc.scalefac_band.l[21] + i * size;
                gfc.scalefac_band.psfb21[i] = start;
            }

            gfc.scalefac_band.psfb21[Encoder.PSFB21] = 576;
            for (var i = 0; i < Encoder.SBMAX_s + 1; i++)
                gfc.scalefac_band.s[i] = qupvt.sfBandIndex[j].s[i];

            for (var i = 0; i < Encoder.PSFB12 + 1; i++)
            {

                var size = (gfc.scalefac_band.s[13] - gfc.scalefac_band.s[12]) / Encoder.PSFB12;

                var start = gfc.scalefac_band.s[12] + i * size;
                gfc.scalefac_band.psfb12[i] = start;
            }

            gfc.scalefac_band.psfb12[Encoder.PSFB12] = 192;
            if (gfp.version == 1)
                gfc.sideinfo_len = gfc.channels_out == 1 ? 4 + 17 : 4 + 32;
            else
                gfc.sideinfo_len = gfc.channels_out == 1 ? 4 + 9 : 4 + 17;

            if (gfp.error_protection)
                gfc.sideinfo_len += 2;

            lame_init_bitstream(gfp);
            gfc.Class_ID = LAME_ID;
            {
                int k;
                for (k = 0; k < 19; k++)
                    gfc.nsPsy.pefirbuf[k] = 700 * gfc.mode_gr * gfc.channels_out;

                if (gfp.ATHtype == -1)
                    gfp.ATHtype = 4;
            }
            Debug.Assert(gfp.VBR_q <= 9);
            Debug.Assert(gfp.VBR_q >= 0);
            if (gfp.VBR == VbrMode.vbr_mt)
                gfp.VBR = VbrMode.vbr_mtrh;

            switch (gfp.VBR)
            {
                case VbrMode.vbr_mtrh:
                {
                    if (gfp.useTemporal == null)
                        gfp.useTemporal = false;

                    p.apply_preset(gfp, 500 - gfp.VBR_q * 10, 0);
                    if (gfp.quality < 0)
                        gfp.quality = LAME_DEFAULT_QUALITY;

                    if (gfp.quality < 5)
                        gfp.quality = 0;

                    if (gfp.quality > 5)
                        gfp.quality = 5;

                    gfc.PSY.mask_adjust = gfp.maskingadjust;
                    gfc.PSY.mask_adjust_short = gfp.maskingadjust_short;
                    if (gfp.experimentalY)
                        gfc.sfb21_extra = false;
                    else
                        gfc.sfb21_extra = gfp.out_samplerate > 44000;

                    gfc.iteration_loop = new VBRNewIterationLoop(qu);
                    break;
                }
                case VbrMode.vbr_rh:
                {
                    p.apply_preset(gfp, 500 - gfp.VBR_q * 10, 0);
                    gfc.PSY.mask_adjust = gfp.maskingadjust;
                    gfc.PSY.mask_adjust_short = gfp.maskingadjust_short;
                    if (gfp.experimentalY)
                        gfc.sfb21_extra = false;
                    else
                        gfc.sfb21_extra = gfp.out_samplerate > 44000;

                    if (gfp.quality > 6)
                        gfp.quality = 6;

                    if (gfp.quality < 0)
                        gfp.quality = LAME_DEFAULT_QUALITY;

                    gfc.iteration_loop = new VBROldIterationLoop(qu);
                    break;
                }
                default:
                {
                    VbrMode vbrmode;
                    gfc.sfb21_extra = false;
                    if (gfp.quality < 0)
                        gfp.quality = LAME_DEFAULT_QUALITY;

                    vbrmode = gfp.VBR;
                    if (vbrmode == VbrMode.vbr_off)
                        gfp.VBR_mean_bitrate_kbps = gfp.brate;

                    p.apply_preset(gfp, gfp.VBR_mean_bitrate_kbps, 0);
                    gfp.VBR = vbrmode;
                    gfc.PSY.mask_adjust = gfp.maskingadjust;
                    gfc.PSY.mask_adjust_short = gfp.maskingadjust_short;
                    if (vbrmode == VbrMode.vbr_off)
                        gfc.iteration_loop = new CBRNewIterationLoop(qu);
                    else
                        gfc.iteration_loop = new ABRIterationLoop(qu);

                    break;
                }
            }

            if (gfp.VBR != VbrMode.vbr_off)
            {
                gfc.VBR_min_bitrate = 1;
                gfc.VBR_max_bitrate = 14;
                if (gfp.out_samplerate < 16000)
                    gfc.VBR_max_bitrate = 8;

                if (gfp.VBR_min_bitrate_kbps != 0)
                {
                    gfp.VBR_min_bitrate_kbps = FindNearestBitrate(
                        gfp.VBR_min_bitrate_kbps,
                        gfp.version,
                        gfp.out_samplerate);
                    gfc.VBR_min_bitrate = BitrateIndex(gfp.VBR_min_bitrate_kbps, gfp.version, gfp.out_samplerate);
                    if (gfc.VBR_min_bitrate < 0)
                        return -1;
                }

                if (gfp.VBR_max_bitrate_kbps != 0)
                {
                    gfp.VBR_max_bitrate_kbps = FindNearestBitrate(
                        gfp.VBR_max_bitrate_kbps,
                        gfp.version,
                        gfp.out_samplerate);
                    gfc.VBR_max_bitrate = BitrateIndex(gfp.VBR_max_bitrate_kbps, gfp.version, gfp.out_samplerate);
                    if (gfc.VBR_max_bitrate < 0)
                        return -1;
                }

                gfp.VBR_min_bitrate_kbps = Tables.bitrate_table[gfp.version][gfc.VBR_min_bitrate];
                gfp.VBR_max_bitrate_kbps = Tables.bitrate_table[gfp.version][gfc.VBR_max_bitrate];
                gfp.VBR_mean_bitrate_kbps = Math.Min(
                    Tables.bitrate_table[gfp.version][gfc.VBR_max_bitrate],
                    gfp.VBR_mean_bitrate_kbps);
                gfp.VBR_mean_bitrate_kbps = Math.Max(
                    Tables.bitrate_table[gfp.version][gfc.VBR_min_bitrate],
                    gfp.VBR_mean_bitrate_kbps);
            }

            if (gfp.tune)
            {
                gfc.PSY.mask_adjust += gfp.tune_value_a;
                gfc.PSY.mask_adjust_short += gfp.tune_value_a;
            }

            lame_init_qval(gfp);
            if (gfp.athaa_type < 0)
                gfc.ATH.useAdjust = 3;
            else
                gfc.ATH.useAdjust = gfp.athaa_type;

            gfc.ATH.aaSensitivityP = (float)Math.Pow(10.0, gfp.athaa_sensitivity / -10.0);
            if (gfp.short_blocks == null)
                gfp.short_blocks = ShortBlock.short_block_allowed;

            if (gfp.short_blocks == ShortBlock.short_block_allowed &&
                (gfp.mode == MPEGMode.JOINT_STEREO || gfp.mode == MPEGMode.STEREO))
                gfp.short_blocks = ShortBlock.short_block_coupled;

            if (gfp.quant_comp < 0)
                gfp.quant_comp = 1;

            if (gfp.quant_comp_short < 0)
                gfp.quant_comp_short = 0;

            if (gfp.msfix < 0)
                gfp.msfix = 0;

            gfp.exp_nspsytune = gfp.exp_nspsytune | 1;
            if (gfp.internal_flags.nsPsy.attackthre < 0)
                gfp.internal_flags.nsPsy.attackthre = PsyModel.NSATTACKTHRE;

            if (gfp.internal_flags.nsPsy.attackthre_s < 0)
                gfp.internal_flags.nsPsy.attackthre_s = PsyModel.NSATTACKTHRE_S;

            if (gfp.scale < 0)
                gfp.scale = 1;

            if (gfp.ATHtype < 0)
                gfp.ATHtype = 4;

            if (gfp.ATHcurve < 0)
                gfp.ATHcurve = 4;

            if (gfp.athaa_loudapprox < 0)
                gfp.athaa_loudapprox = 2;

            if (gfp.interChRatio < 0)
                gfp.interChRatio = 0;

            if (gfp.useTemporal == null)
                gfp.useTemporal = true;

            gfc.slot_lag = gfc.frac_SpF = 0;
            if (gfp.VBR == VbrMode.vbr_off)
                gfc.slot_lag = gfc.frac_SpF = (int)((gfp.version + 1) * 72000L * gfp.brate % gfp.out_samplerate);

            qupvt.iteration_init(gfp);
            psy.psymodel_init(gfp);
            return 0;
        }

        internal void lame_print_config(LameGlobalFlags gfp)
        {

            var gfc = gfp.internal_flags;
            double out_samplerate = gfp.out_samplerate;
            var in_samplerate = gfp.out_samplerate * gfc.resample_ratio;
            Console.Write("LAME {0} {1} ({2})\n", ver.LameVersion, ver.LameOsBitness, ver.LameUrl);
            if (gfp.num_channels == 2 && gfc.channels_out == 1)
                Console.Write("Autoconverting from stereo to mono. Setting encoding to mono mode.\n");

            if (BitStream.NEQ((float)gfc.resample_ratio, 1.0f))
                Console.Write(
                    "Resampling:  input {0:g} kHz  output {1:g} kHz\n",
                    0.001 * in_samplerate,
                    0.001 * out_samplerate);

            if (gfc.highpass2 > 0.0)
                Console.Write(
                    "Using polyphase highpass filter, transition band: {0,5:F0} Hz - {1,5:F0} Hz\n",
                    0.5 * gfc.highpass1 * out_samplerate,
                    0.5 * gfc.highpass2 * out_samplerate);

            if (0.0 < gfc.lowpass1 || 0.0 < gfc.lowpass2)
                Console.Write(
                    "Using polyphase lowpass filter, transition band: {0,5:F0} Hz - {1,5:F0} Hz\n",
                    0.5 * gfc.lowpass1 * out_samplerate,
                    0.5 * gfc.lowpass2 * out_samplerate);
            else
                Console.Write("polyphase lowpass filter disabled\n");

            if (gfp.free_format)
            {
                Console.WriteLine("Warning: many decoders cannot handle free format bitstreams\n");
                if (gfp.brate > 320)
                    Console.WriteLine(
                        "Warning: many decoders cannot handle free format bitrates >320 kbps (see documentation)\n");
            }
        }

        internal void lame_print_internals(LameGlobalFlags gfp)
        {

            var gfc = gfp.internal_flags;
            Console.WriteLine("\nmisc:\n\n");
            Console.WriteLine("\tscaling: %g\n", gfp.scale);
            Console.WriteLine("\tch0 (left) scaling: %g\n", gfp.scale_left);
            Console.WriteLine("\tch1 (right) scaling: %g\n", gfp.scale_right);
            string pc;
            switch (gfc.use_best_huffman)
            {
                default:
                    pc = "normal";
                    break;
                case 1:
                    pc = "best (outside loop)";
                    break;
                case 2:
                    pc = "best (inside loop, slow)";
                    break;
            }

            Console.WriteLine("\thuffman search: %s\n", pc);
            Console.WriteLine("\texperimental Y=%d\n", gfp.experimentalY);
            Console.WriteLine("\t...\n");
            Console.WriteLine("\nstream format:\n\n");
            switch (gfp.version)
            {
                case 0:
                    pc = "2.5";
                    break;
                case 1:
                    pc = "1";
                    break;
                case 2:
                    pc = "2";
                    break;
                default:
                    pc = "?";
                    break;
            }

            Console.WriteLine("\tMPEG-%s Layer 3\n", pc);
            switch (gfp.mode)
            {
                case MPEGMode.JOINT_STEREO:
                    pc = "joint stereo";
                    break;
                case MPEGMode.STEREO:
                    pc = "stereo";
                    break;
                case MPEGMode.DUAL_CHANNEL:
                    pc = "dual channel";
                    break;
                case MPEGMode.MONO:
                    pc = "mono";
                    break;
                case MPEGMode.NOT_SET:
                    pc = "not set (error)";
                    break;
                default:
                    pc = "unknown (error)";
                    break;
            }

            Console.WriteLine("\t%d channel - %s\n", gfc.channels_out, pc);
            switch (gfp.VBR)
            {
                case VbrMode.vbr_off:
                    pc = "off";
                    break;
                default:
                    pc = "all";
                    break;
            }

            Console.WriteLine("\tpadding: %s\n", pc);
            if (VbrMode.vbr_default == gfp.VBR)
            {
                pc = "(default)";
                gfp.VBR = VbrMode.vbr_mtrh;

            }
            else if (gfp.free_format)
            {
                pc = "(free format)";
            }
            else
            {
                pc = "";
            }

            switch (gfp.VBR)
            {
                case VbrMode.vbr_off:
                    Console.WriteLine("\tconstant bitrate - CBR %s\n", pc);
                    break;
                case VbrMode.vbr_abr:
                    Console.WriteLine("\tvariable bitrate - ABR %s\n", pc);
                    break;
                case VbrMode.vbr_rh:
                    Console.WriteLine("\tvariable bitrate - VBR rh %s\n", pc);
                    break;
                case VbrMode.vbr_mt:
                    Console.WriteLine("\tvariable bitrate - VBR mt %s\n", pc);
                    break;
                case VbrMode.vbr_mtrh:
                    Console.WriteLine("\tvariable bitrate - VBR mtrh %s\n", pc);
                    break;
                default:
                    Console.WriteLine("\t ?? oops, some new one ?? \n");
                    break;
            }

            if (gfp.bWriteVbrTag)
                Console.WriteLine("\tusing LAME Tag\n");

            Console.WriteLine("\t...\n");
            Console.WriteLine("\npsychoacoustic:\n\n");
            switch (gfp.short_blocks)
            {
                default:
                    pc = "?";
                    break;
                case ShortBlock.short_block_allowed:
                    pc = "allowed";
                    break;
                case ShortBlock.short_block_coupled:
                    pc = "channel coupled";
                    break;
                case ShortBlock.short_block_dispensed:
                    pc = "dispensed";
                    break;
                case ShortBlock.short_block_forced:
                    pc = "forced";
                    break;
            }

            Console.WriteLine("\tusing short blocks: %s\n", pc);
            Console.WriteLine("\tsubblock gain: %d\n", gfc.subblock_gain);
            Console.WriteLine("\tadjust masking: %g dB\n", gfc.PSY.mask_adjust);
            Console.WriteLine("\tadjust masking short: %g dB\n", gfc.PSY.mask_adjust_short);
            Console.WriteLine("\tquantization comparison: %d\n", gfp.quant_comp);
            Console.WriteLine("\t ^ comparison short blocks: %d\n", gfp.quant_comp_short);
            Console.WriteLine("\tnoise shaping: %d\n", gfc.noise_shaping);
            Console.WriteLine("\t ^ amplification: %d\n", gfc.noise_shaping_amp);
            Console.WriteLine("\t ^ stopping: %d\n", gfc.noise_shaping_stop);
            pc = "using";
            if (gfp.ATHshort)
                pc = "the only masking for short blocks";

            if (gfp.ATHonly)
                pc = "the only masking";

            if (gfp.noATH)
                pc = "not used";

            Console.WriteLine("\tATH: %s\n", pc);
            Console.WriteLine("\t ^ type: %d\n", gfp.ATHtype);
            Console.WriteLine("\t ^ shape: %g%s\n", gfp.ATHcurve, " (only for type 4)");
            Console.WriteLine("\t ^ level adjustement: %g\n", gfp.ATHlower);
            Console.WriteLine("\t ^ adjust type: %d\n", gfc.ATH.useAdjust);
            Console.WriteLine("\t ^ adjust sensitivity power: %f\n", gfc.ATH.aaSensitivityP);
            Console.WriteLine("\t ^ adapt threshold type: %d\n", gfp.athaa_loudapprox);
            Console.WriteLine("\texperimental psy tunings by Naoki Shibata\n");
            Console.WriteLine(
                "\t   adjust masking bass=%g dB, alto=%g dB, treble=%g dB, sfb21=%g dB\n",
                10 * Math.Log10(gfc.nsPsy.longfact[0]),
                10 * Math.Log10(gfc.nsPsy.longfact[7]),
                10 * Math.Log10(gfc.nsPsy.longfact[14]),
                10 * Math.Log10(gfc.nsPsy.longfact[21]));
            pc = gfp.useTemporal == true ? "yes" : "no";
            Console.WriteLine("\tusing temporal masking effect: %s\n", pc);
            Console.WriteLine("\tinterchannel masking ratio: %g\n", gfp.interChRatio);
            Console.WriteLine("\t...\n");
            Console.WriteLine("\n");
        }

        private int lame_encode_frame(
            LameGlobalFlags gfp,
            float[] inbuf_l,
            float[] inbuf_r,
            byte[] mp3buf,
            int mp3bufPos,
            int mp3buf_size)
        {
            var ret = enc.lame_encode_mp3_frame(gfp, inbuf_l, inbuf_r, mp3buf, mp3bufPos, mp3buf_size);
            gfp.frameNum++;
            return ret;
        }

        private void update_inbuffer_size(LameInternalFlags gfc, int nsamples)
        {
            if (gfc.in_buffer_0 == null || gfc.in_buffer_nsamples < nsamples)
            {
                gfc.in_buffer_0 = new float[nsamples];
                gfc.in_buffer_1 = new float[nsamples];
                gfc.in_buffer_nsamples = nsamples;
            }
        }

        private int calcNeeded(LameGlobalFlags gfp)
        {
            var mf_needed = Encoder.BLKSIZE + gfp.framesize - Encoder.FFTOFFSET;
            mf_needed = Math.Max(mf_needed, 512 + gfp.framesize - 32);
            Debug.Assert(LameInternalFlags.MFSIZE >= mf_needed);
            return mf_needed;
        }

        private int lame_encode_buffer_sample(
            LameGlobalFlags gfp,
            float[] buffer_l,
            float[] buffer_r,
            int nsamples,
            byte[] mp3buf,
            int mp3bufPos,
            int mp3buf_size)
        {

            var gfc = gfp.internal_flags;
            int mp3size = 0, ret, i, ch, mf_needed;
            int mp3out;
            var mfbuf = new float[2][];
            var in_buffer = new float[2][];
            if (gfc.Class_ID != LAME_ID)
                return -3;

            if (nsamples == 0)
                return 0;

            mp3out = bs.copy_buffer(gfc, mp3buf, mp3bufPos, mp3buf_size, 0);
            if (mp3out < 0)
                return mp3out;

            mp3bufPos += mp3out;
            mp3size += mp3out;
            in_buffer[0] = buffer_l;
            in_buffer[1] = buffer_r;
            if (BitStream.NEQ(gfp.scale, 0) && BitStream.NEQ(gfp.scale, 1.0f))
                for (i = 0; i < nsamples; ++i)
                {
                    in_buffer[0][i] *= gfp.scale;
                    if (gfc.channels_out == 2)
                        in_buffer[1][i] *= gfp.scale;
                }

            if (BitStream.NEQ(gfp.scale_left, 0) && BitStream.NEQ(gfp.scale_left, 1.0f))
                for (i = 0; i < nsamples; ++i)
                    in_buffer[0][i] *= gfp.scale_left;

            if (BitStream.NEQ(gfp.scale_right, 0) && BitStream.NEQ(gfp.scale_right, 1.0f))
                for (i = 0; i < nsamples; ++i)
                    in_buffer[1][i] *= gfp.scale_right;

            if (gfp.num_channels == 2 && gfc.channels_out == 1)
                for (i = 0; i < nsamples; ++i)
                {
                    in_buffer[0][i] = 0.5f * (in_buffer[0][i] + in_buffer[1][i]);
                    in_buffer[1][i] = 0.0f;
                }

            mf_needed = calcNeeded(gfp);
            mfbuf[0] = gfc.mfbuf[0];
            mfbuf[1] = gfc.mfbuf[1];
            var in_bufferPos = 0;
            while (nsamples > 0)
            {

                var in_buffer_ptr = new float[2][];
                var n_in = 0;
                var n_out = 0;
                in_buffer_ptr[0] = in_buffer[0];
                in_buffer_ptr[1] = in_buffer[1];
                var inOut = new InOut();
                fill_buffer(gfp, mfbuf, in_buffer_ptr, in_bufferPos, nsamples, inOut);
                n_in = inOut.n_in;
                n_out = inOut.n_out;
                if (gfc.findReplayGain && !gfc.decode_on_the_fly)
                    if (ga.AnalyzeSamples(
                            gfc.rgdata,
                            mfbuf[0],
                            gfc.mf_size,
                            mfbuf[1],
                            gfc.mf_size,
                            n_out,
                            gfc.channels_out) == GainAnalysis.GAIN_ANALYSIS_ERROR)
                        return -6;

                nsamples -= n_in;
                in_bufferPos += n_in;
                if (gfc.channels_out == 2)
                    ;

                gfc.mf_size += n_out;
                Debug.Assert(gfc.mf_size <= LameInternalFlags.MFSIZE);
                if (gfc.mf_samples_to_encode < 1)
                    gfc.mf_samples_to_encode = Encoder.ENCDELAY + Encoder.POSTDELAY;

                gfc.mf_samples_to_encode += n_out;
                if (gfc.mf_size >= mf_needed)
                {
                    var buf_size = mp3buf_size - mp3size;
                    if (mp3buf_size == 0)
                        buf_size = 0;

                    ret = lame_encode_frame(gfp, mfbuf[0], mfbuf[1], mp3buf, mp3bufPos, buf_size);
                    if (ret < 0)
                        return ret;

                    mp3bufPos += ret;
                    mp3size += ret;
                    gfc.mf_size -= gfp.framesize;
                    gfc.mf_samples_to_encode -= gfp.framesize;
                    for (ch = 0; ch < gfc.channels_out; ch++)
                    for (i = 0; i < gfc.mf_size; i++)
                        mfbuf[ch][i] = mfbuf[ch][i + gfp.framesize];
                }
            }

            Debug.Assert(nsamples == 0);
            return mp3size;
        }

        private int lame_encode_buffer(
            LameGlobalFlags gfp,
            short[] buffer_l,
            short[] buffer_r,
            int nsamples,
            byte[] mp3buf,
            int mp3bufPos,
            int mp3buf_size)
        {

            var gfc = gfp.internal_flags;
            var in_buffer = new float[2][];
            if (gfc.Class_ID != LAME_ID)
                return -3;

            if (nsamples == 0)
                return 0;

            update_inbuffer_size(gfc, nsamples);
            in_buffer[0] = gfc.in_buffer_0;
            in_buffer[1] = gfc.in_buffer_1;
            for (var i = 0; i < nsamples; i++)
            {
                in_buffer[0][i] = buffer_l[i];
                if (gfc.channels_in > 1)
                    in_buffer[1][i] = buffer_r[i];
            }

            return lame_encode_buffer_sample(gfp, in_buffer[0], in_buffer[1], nsamples, mp3buf, mp3bufPos, mp3buf_size);
        }

        internal virtual int lame_encode_buffer_int(
            LameGlobalFlags gfp,
            int[] buffer_l,
            int[] buffer_r,
            int nsamples,
            byte[] mp3buf,
            int mp3bufPos,
            int mp3buf_size)
        {

            var gfc = gfp.internal_flags;
            var in_buffer = new float[2][];
            if (gfc.Class_ID != LAME_ID)
                return -3;

            if (nsamples == 0)
                return 0;

            update_inbuffer_size(gfc, nsamples);
            in_buffer[0] = gfc.in_buffer_0;
            in_buffer[1] = gfc.in_buffer_1;
            for (var i = 0; i < nsamples; i++)
            {
                in_buffer[0][i] = buffer_l[i] * (1.0f / (1L << 16));
                if (gfc.channels_in > 1)
                    in_buffer[1][i] = buffer_r[i] * (1.0f / (1L << 16));
            }

            return lame_encode_buffer_sample(gfp, in_buffer[0], in_buffer[1], nsamples, mp3buf, mp3bufPos, mp3buf_size);
        }

        internal int lame_encode_flush_nogap(LameGlobalFlags gfp, byte[] mp3buffer, int mp3buffer_size)
        {

            var gfc = gfp.internal_flags;
            bs.flush_bitstream(gfp);
            return bs.copy_buffer(gfc, mp3buffer, 0, mp3buffer_size, 1);
        }

        internal void lame_init_bitstream(LameGlobalFlags gfp)
        {

            var gfc = gfp.internal_flags;
            gfp.frameNum = 0;
            if (gfp.write_id3tag_automatic)
                id3.id3tag_write_v2(gfp);

            gfc.bitrate_stereoMode_Hist = Arrays.ReturnRectangularArray<int>(16, 4 + 1);

            gfc.bitrate_blockType_Hist = Arrays.ReturnRectangularArray<int>(16, 4 + 1 + 1);
            gfc.PeakSample = 0.0f;
            if (gfp.bWriteVbrTag)
                vbr.InitVbrTag(gfp);
        }

        internal int lame_encode_flush(LameGlobalFlags gfp, byte[] mp3buffer, int mp3bufferPos, int mp3buffer_size)
        {

            var gfc = gfp.internal_flags;

            var buffer = Arrays.ReturnRectangularArray<short>(2, 1152);
            int imp3 = 0, mp3count, mp3buffer_size_remaining;
            int end_padding;
            int frames_left;
            var samples_to_encode = gfc.mf_samples_to_encode - Encoder.POSTDELAY;
            var mf_needed = calcNeeded(gfp);
            if (gfc.mf_samples_to_encode < 1)
                return 0;

            mp3count = 0;
            if (gfp.in_samplerate != gfp.out_samplerate)
                samples_to_encode += (int)(16.0 * gfp.out_samplerate / gfp.in_samplerate);

            end_padding = gfp.framesize - samples_to_encode % gfp.framesize;
            if (end_padding < 576)
                end_padding += gfp.framesize;

            gfp.encoder_padding = end_padding;
            frames_left = (samples_to_encode + end_padding) / gfp.framesize;
            while (frames_left > 0 && imp3 >= 0)
            {
                var bunch = mf_needed - gfc.mf_size;
                var frame_num = gfp.frameNum;
                bunch *= gfp.in_samplerate;
                bunch /= gfp.out_samplerate;
                if (bunch > 1152)
                    bunch = 1152;

                if (bunch < 1)
                    bunch = 1;

                mp3buffer_size_remaining = mp3buffer_size - mp3count;
                if (mp3buffer_size == 0)
                    mp3buffer_size_remaining = 0;

                imp3 = lame_encode_buffer(
                    gfp,
                    buffer[0],
                    buffer[1],
                    bunch,
                    mp3buffer,
                    mp3bufferPos,
                    mp3buffer_size_remaining);
                mp3bufferPos += imp3;
                mp3count += imp3;
                frames_left -= frame_num != gfp.frameNum ? 1 : 0;
            }

            gfc.mf_samples_to_encode = 0;
            if (imp3 < 0)
                return imp3;

            mp3buffer_size_remaining = mp3buffer_size - mp3count;
            if (mp3buffer_size == 0)
                mp3buffer_size_remaining = 0;

            bs.flush_bitstream(gfp);
            imp3 = bs.copy_buffer(gfc, mp3buffer, mp3bufferPos, mp3buffer_size_remaining, 1);
            if (imp3 < 0)
                return imp3;

            mp3bufferPos += imp3;
            mp3count += imp3;
            mp3buffer_size_remaining = mp3buffer_size - mp3count;
            if (mp3buffer_size == 0)
                mp3buffer_size_remaining = 0;

            if (gfp.write_id3tag_automatic)
            {
                id3.id3tag_write_v1(gfp);
                imp3 = bs.copy_buffer(gfc, mp3buffer, mp3bufferPos, mp3buffer_size_remaining, 0);
                if (imp3 < 0)
                    return imp3;

                mp3count += imp3;
            }

            return mp3count;
        }

        internal int lame_close(LameGlobalFlags gfp)
        {
            var ret = 0;
            if (gfp != null && gfp.class_id == LAME_ID)
            {

                var gfc = gfp.internal_flags;
                gfp.class_id = 0;
                if (null == gfc || gfc.Class_ID != LAME_ID)
                    ret = -3;

                gfc.Class_ID = 0;
                gfp.internal_flags = null;
                gfp.lame_allocated_gfp = 0;
            }

            return ret;
        }

        private int lame_init_old(LameGlobalFlags gfp)
        {
            LameInternalFlags gfc;
            gfp.class_id = LAME_ID;
            gfc = gfp.internal_flags = new LameInternalFlags();
            gfp.mode = MPEGMode.NOT_SET;
            gfp.original = 1;
            gfp.in_samplerate = 44100;
            gfp.num_channels = 2;
            gfp.num_samples = -1;
            gfp.bWriteVbrTag = true;
            gfp.quality = -1;
            gfp.short_blocks = null;
            gfc.subblock_gain = -1;
            gfp.lowpassfreq = 0;
            gfp.highpassfreq = 0;
            gfp.lowpasswidth = -1;
            gfp.highpasswidth = -1;
            gfp.VBR = VbrMode.vbr_off;
            gfp.VBR_q = 4;
            gfp.ATHcurve = -1;
            gfp.VBR_mean_bitrate_kbps = 128;
            gfp.VBR_min_bitrate_kbps = 0;
            gfp.VBR_max_bitrate_kbps = 0;
            gfp.VBR_hard_min = 0;
            gfc.VBR_min_bitrate = 1;
            gfc.VBR_max_bitrate = 13;
            gfp.quant_comp = -1;
            gfp.quant_comp_short = -1;
            gfp.msfix = -1;
            gfc.resample_ratio = 1;
            gfc.OldValue[0] = 180;
            gfc.OldValue[1] = 180;
            gfc.CurrentStep[0] = 4;
            gfc.CurrentStep[1] = 4;
            gfc.masking_lower = 1;
            gfc.nsPsy.attackthre = -1;
            gfc.nsPsy.attackthre_s = -1;
            gfp.scale = -1;
            gfp.athaa_type = -1;
            gfp.ATHtype = -1;
            gfp.athaa_loudapprox = -1;
            gfp.athaa_sensitivity = 0.0f;
            gfp.useTemporal = null;
            gfp.interChRatio = -1;
            gfc.mf_samples_to_encode = Encoder.ENCDELAY + Encoder.POSTDELAY;
            gfp.encoder_padding = 0;
            gfc.mf_size = Encoder.ENCDELAY - Encoder.MDCTDELAY;
            gfp.findReplayGain = false;
            gfp.decode_on_the_fly = false;
            gfc.decode_on_the_fly = false;
            gfc.findReplayGain = false;
            gfc.findPeakSample = false;
            gfc.RadioGain = 0;
            gfc.AudiophileGain = 0;
            gfc.noclipGainChange = 0;
            gfc.noclipScale = -1.0f;
            gfp.preset = 0;
            gfp.write_id3tag_automatic = true;
            return 0;
        }

        internal LameGlobalFlags lame_init()
        {
            var gfp = new LameGlobalFlags();
            var ret = lame_init_old(gfp);
            if (ret != 0)
                return null;

            gfp.lame_allocated_gfp = 1;
            return gfp;
        }

        internal void lame_bitrate_kbps(LameGlobalFlags gfp, int[] bitrate_kbps)
        {

            LameInternalFlags gfc;
            if (null == bitrate_kbps)
                return;

            if (null == gfp)
                return;

            gfc = gfp.internal_flags;
            if (null == gfc)
                return;

            if (gfp.free_format)
            {
                for (var i = 0; i < 14; i++)
                    bitrate_kbps[i] = -1;

                bitrate_kbps[0] = gfp.brate;
            }
            else
            {
                for (var i = 0; i < 14; i++)
                    bitrate_kbps[i] = Tables.bitrate_table[gfp.version][i + 1];
            }
        }

        internal void lame_bitrate_hist(LameGlobalFlags gfp, int[] bitrate_count)
        {
            if (null == bitrate_count)
                return;

            if (null == gfp)
                return;

            var gfc = gfp.internal_flags;
            if (null == gfc)
                return;

            if (gfp.free_format)
            {
                for (var i = 0; i < 14; i++)
                    bitrate_count[i] = 0;

                bitrate_count[0] = gfc.bitrate_stereoMode_Hist[0][4];
            }
            else
            {
                for (var i = 0; i < 14; i++)
                    bitrate_count[i] = gfc.bitrate_stereoMode_Hist[i + 1][4];
            }
        }

        internal void lame_stereo_mode_hist(LameGlobalFlags gfp, int[] stmode_count)
        {
            if (null == stmode_count)
                return;

            if (null == gfp)
                return;

            var gfc = gfp.internal_flags;
            if (null == gfc)
                return;

            for (var i = 0; i < 4; i++)
                stmode_count[i] = gfc.bitrate_stereoMode_Hist[15][i];
        }

        internal void lame_bitrate_stereo_mode_hist(LameGlobalFlags gfp, int[][] bitrate_stmode_count)
        {
            if (null == bitrate_stmode_count)
                return;

            if (null == gfp)
                return;

            var gfc = gfp.internal_flags;
            if (null == gfc)
                return;

            if (gfp.free_format)
            {
                for (var j = 0; j < 14; j++)
                for (var i = 0; i < 4; i++)
                    bitrate_stmode_count[j][i] = 0;

                for (var i = 0; i < 4; i++)
                    bitrate_stmode_count[0][i] = gfc.bitrate_stereoMode_Hist[0][i];
            }
            else
            {
                for (var j = 0; j < 14; j++)
                for (var i = 0; i < 4; i++)
                    bitrate_stmode_count[j][i] = gfc.bitrate_stereoMode_Hist[j + 1][i];
            }
        }

        internal void lame_block_type_hist(LameGlobalFlags gfp, int[] btype_count)
        {
            if (null == btype_count)
                return;

            if (null == gfp)
                return;

            var gfc = gfp.internal_flags;
            if (null == gfc)
                return;

            for (var i = 0; i < 6; ++i)
                btype_count[i] = gfc.bitrate_blockType_Hist[15][i];
        }

        internal void lame_bitrate_block_type_hist(LameGlobalFlags gfp, int[][] bitrate_btype_count)
        {
            if (null == bitrate_btype_count)
                return;

            if (null == gfp)
                return;

            var gfc = gfp.internal_flags;
            if (null == gfc)
                return;

            if (gfp.free_format)
            {
                for (var j = 0; j < 14; ++j)
                for (var i = 0; i < 6; ++i)
                    bitrate_btype_count[j][i] = 0;

                for (var i = 0; i < 6; ++i)
                    bitrate_btype_count[0][i] = gfc.bitrate_blockType_Hist[0][i];
            }
            else
            {
                for (var j = 0; j < 14; ++j)
                for (var i = 0; i < 6; ++i)
                    bitrate_btype_count[j][i] = gfc.bitrate_blockType_Hist[j + 1][i];
            }
        }
    }
}