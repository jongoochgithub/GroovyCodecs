//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Diagnostics;
using GroovyMp3.Types;
using GroovyMp3.Codec.Mp3;

/*
 *      LAME MP3 encoding engine
 *
 *      Copyright (c) 1999-2000 Mark Taylor
 *      Copyright (c) 2003 Olcios
 *      Copyright (c) 2008 Robert Hegemann
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */
namespace GroovyMp3.Codec.Mpg
{

    internal class MPGLib
    {

        internal class buf
        {

            internal buf next;

            internal byte[] pnt;

            internal int pos;

            internal buf prev;

            internal int size;
        }

        internal class framebuf
        {
            internal buf buf;

            internal Frame next;

            internal long pos;

            internal Frame prev;
        }

        internal class mpstr_tag
        {

            internal int bitindex;

            internal int bsize;

            internal int bsnum;

            internal byte[][]
                bsspace = Arrays.ReturnRectangularArray<byte>(2, MPG123.MAXFRAMESIZE + 1024); // MAXFRAMESIZE

            internal bool data_parsed;

            internal int dsize;

            internal int enc_delay; // set if vbr header present

            internal int enc_padding; // set if vbr header present

            internal Frame fr = new Frame(); // holds the parameters decoded from the header

            internal int framesize;

            internal bool free_format; // 1 = free format frame

            internal int fsizeold; // size of previous frame, -1 for first

            internal int fsizeold_nopadding;

            internal buf head, tail; // buffer linked list pointers, tail points to oldest buffer

            internal long header;

            /* header_parsed, side_parsed and data_parsed must be all set 1
               before the full frame has been parsed */
            internal bool header_parsed; // 1 = header of current frame has been parsed

            internal int[] hybrid_blc = new int[2];

            internal float[][][] hybrid_block =
                Arrays.ReturnRectangularArray<float>(2, 2, MPG123.SBLIMIT * MPG123.SSLIMIT);

            internal int num_frames; // set if vbr header present

            internal bool old_free_format; // 1 = last frame was free format

            internal PlottingData pinfo;

            internal bool side_parsed; // 1 = header of sideinfo of current frame has been parsed

            internal int ssize; // number of bytes used for side information, including 2 bytes for CRC-16 if present

            internal int sync_bitstream; // 1 = bitstream is yet to be synchronized

            internal int synth_bo;

            internal float[][][] synth_buffs = Arrays.ReturnRectangularArray<float>(2, 2, 0x110);

            internal bool vbr_header; // 1 if valid Xing vbr header detected

            internal byte[] wordpointer;

            internal int wordpointerPos;
        }

        internal class ProcessedBytes
        {
            internal int pb;
        }

        internal interface IDecoder
        {
            int decode<T>(
                mpstr_tag mp,
                byte[] @in,
                int bufferPos,
                int isize,
                T[] @out,
                int osize,
                ProcessedBytes done,
                Decode.Factory<T> tFactory);
        }

        private class IDecoderAnonymousInnerClass : IDecoder
        {
            private readonly MPGLib outerInstance;

            private int bufferPos;

            internal IDecoderAnonymousInnerClass(MPGLib outerInstance, int bufferPos)
            {
                this.outerInstance = outerInstance;
                this.bufferPos = bufferPos;
            }

            public virtual int decode<X>(
                mpstr_tag mp,
                byte[] @in,
                int bufferPos,
                int isize,
                X[] @out,
                int osize,
                ProcessedBytes done,
                Decode.Factory<X> tFactory)
            {
                return outerInstance.interf.decodeMP3_unclipped(mp, @in, bufferPos, isize, @out, osize, done, tFactory);
            }
        }

        private class FactoryAnonymousInnerClass : Decode.Factory<float?>
        {
            private readonly MPGLib outerInstance;

            internal FactoryAnonymousInnerClass(MPGLib outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public float? create(float x)
            {
                return Convert.ToSingle(x);
            }
        }

        private class IDecoderAnonymousInnerClass2 : IDecoder
        {
            private readonly MPGLib outerInstance;

            internal IDecoderAnonymousInnerClass2(MPGLib outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual int decode<X>(
                mpstr_tag mp,
                byte[] @in,
                int bufferPos,
                int isize,
                X[] @out,
                int osize,
                ProcessedBytes done,
                Decode.Factory<X> tFactory)
            {
                return outerInstance.interf.decodeMP3(mp, @in, bufferPos, isize, @out, osize, done, tFactory);
            }
        }

        private class FactoryAnonymousInnerClass2 : Decode.Factory<short?>
        {
            private readonly MPGLib outerInstance;

            internal FactoryAnonymousInnerClass2(MPGLib outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public short? create(float x)
            {
                return Convert.ToInt16((short)x);
            }
        }

        internal const int MP3_ERR = -1;

        internal const int MP3_NEED_MORE = 1;

        internal const int MP3_OK = 0;

        private const int OUTSIZE_CLIPPED = 4096;

        /* we forbid input with more than 1152 samples per channel for output in the unclipped mode */
        private const int OUTSIZE_UNCLIPPED = 1152 * 2;

        private static readonly int[][] smpls =
        {
            new[]
            {
                0,
                384,
                1152,
                1152
            },
            new[]
            {
                0,
                384,
                1152,
                576
            }
        };

        internal Common common;

        internal Interface interf;

        internal virtual void setModules(Interface i, Common c)
        {
            interf = i;
            common = c;
        }

        /* copy mono samples */
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

        protected internal virtual void COPY_MONO<SRC_TYPE>(
            SRC_TYPE[] pcm_l,
            int pcm_lPos,
            int processed_samples,
            SRC_TYPE[] p)

        {
            var p_samples = 0;
            for (var i = 0; i < processed_samples; i++)
                pcm_l[pcm_lPos++] = p[p_samples++];
        }

        /* copy stereo samples */
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

        protected internal virtual void COPY_STEREO<SRC_TYPE>(
            SRC_TYPE[] pcm_l,
            int pcm_lPos,
            SRC_TYPE[] pcm_r,
            int pcm_rPos,
            int processed_samples,
            SRC_TYPE[] p)
        {
            var p_samples = 0;
            for (var i = 0; i < processed_samples; i++)
            {
                pcm_l[pcm_lPos++] = p[p_samples++];
                pcm_r[pcm_rPos++] = p[p_samples++];
            }
        }

        /*
         * For lame_decode:  return code
         * -1     error
         *  0     ok, but need more data before outputing any samples
         *  n     number of samples output.  either 576 or 1152 depending on MP3 file.
         */

        private int decode1_headersB_clipchoice<T>(
            mpstr_tag pmp,
            byte[] buffer,
            int bufferPos,
            int len,
            T[] pcm_l,
            int pcm_lPos,
            T[] pcm_r,
            int pcm_rPos,
            MP3Data mp3data,
            Enc enc,
            T[] p,
            int psize,
            IDecoder decodeMP3_ptr,
            Decode.Factory<T> tFactory)
        {

            int processed_samples; // processed samples per channel
            int ret;

            mp3data.header_parsed = false;

            var pb = new ProcessedBytes();
            ret = decodeMP3_ptr.decode(pmp, buffer, bufferPos, len, p, psize, pb, tFactory);
            processed_samples = pb.pb;
            /* three cases:  
             * 1. headers parsed, but data not complete
             *       pmp.header_parsed==1 
             *       pmp.framesize=0           
             *       pmp.fsizeold=size of last frame, or 0 if this is first frame
             *
             * 2. headers, data parsed, but ancillary data not complete
             *       pmp.header_parsed==1 
             *       pmp.framesize=size of frame           
             *       pmp.fsizeold=size of last frame, or 0 if this is first frame
             *
             * 3. frame fully decoded:  
             *       pmp.header_parsed==0 
             *       pmp.framesize=0           
             *       pmp.fsizeold=size of frame (which is now the last frame)
             *
             */
            if (pmp.header_parsed || pmp.fsizeold > 0 || pmp.framesize > 0)
            {
                mp3data.header_parsed = true;
                mp3data.stereo = pmp.fr.stereo;
                mp3data.samplerate = Common.freqs[pmp.fr.sampling_frequency];
                mp3data.mode = pmp.fr.mode;
                mp3data.mode_ext = pmp.fr.mode_ext;
                mp3data.framesize = smpls[pmp.fr.lsf][pmp.fr.lay];

                /* free format, we need the entire frame before we can determine
                 * the bitrate.  If we haven't gotten the entire frame, bitrate=0 */
                if (pmp.fsizeold > 0) // works for free format and fixed, no overrun, temporal results are < 400.e6
                    mp3data.bitrate =
                        (int)(8 * (4 + pmp.fsizeold) * mp3data.samplerate / (1000 * mp3data.framesize) + 0.5);
                else if (pmp.framesize > 0)
                    mp3data.bitrate =
                        (int)(8 * (4 + pmp.framesize) * mp3data.samplerate / (1000 * mp3data.framesize) + 0.5);
                else
                    mp3data.bitrate = Common.tabsel_123[pmp.fr.lsf][pmp.fr.lay - 1][pmp.fr.bitrate_index];

                if (pmp.num_frames > 0)
                {
                    /* Xing VBR header found and num_frames was set */
                    mp3data.totalframes = pmp.num_frames;
                    mp3data.nsamp = mp3data.framesize * pmp.num_frames;
                    enc.enc_delay = pmp.enc_delay;
                    enc.enc_padding = pmp.enc_padding;
                }
            }

            switch (ret)
            {
                case MP3_OK:
                    switch (pmp.fr.stereo)
                    {
                        case 1:
                            COPY_MONO(pcm_l, pcm_lPos, processed_samples, p);
                            break;
                        case 2:
                            processed_samples = processed_samples >> 1;
                            COPY_STEREO(pcm_l, pcm_lPos, pcm_r, pcm_rPos, processed_samples, p);
                            break;
                        default:
                            processed_samples = -1;
                            Debug.Assert(false);
                            break;
                    }

                    break;

                case MP3_NEED_MORE:
                    processed_samples = 0;
                    break;

                case MP3_ERR:
                    processed_samples = -1;
                    break;

                default:
                    processed_samples = -1;
                    Debug.Assert(false);
                    break;
            }

            /*fprintf(stderr,"ok, more, err:  %i %i %i\n", MP3_OK, MP3_NEED_MORE, MP3_ERR ); */
            /*fprintf(stderr,"ret = %i out=%i\n", ret, processed_samples ); */
            return processed_samples;
        }

        internal virtual mpstr_tag hip_decode_init()
        {
            return interf.InitMP3();
        }

        internal virtual int hip_decode_exit(mpstr_tag hip)
        {
            if (hip != null)
            {
                interf.ExitMP3(hip);
                hip = null;
            }

            return 0;
        }

        /*
         * same as hip_decode1 (look in lame.h), but returns unclipped raw
         * floating-point samples. It is declared here, not in lame.h, because it
         * returns LAME's internal type sample_t. No more than 1152 samples per
         * channel are allowed.
         */

        internal virtual int hip_decode1_unclipped(
            mpstr_tag hip,
            byte[] buffer,
            int bufferPos,
            int len,
            float[] pcm_l,
            float[] pcm_r)
        {

            var mp3data = new MP3Data();
            var enc = new Enc();

            if (hip != null)
            {
                IDecoder dec = new IDecoderAnonymousInnerClass(this, bufferPos);
                var @out = new float?[OUTSIZE_UNCLIPPED];
                Decode.Factory<float?> tFactory = new FactoryAnonymousInnerClass(this);

                // XXX should we avoid the primitive type version?
                var pcmL = new float?[pcm_l.Length];
                for (var i = 0; i < pcmL.Length; i++)
                    pcmL[i] = Convert.ToSingle(pcm_l[i]);

                var pcmR = new float?[pcm_r.Length];
                for (var i = 0; i < pcmR.Length; i++)
                    pcmR[i] = Convert.ToSingle(pcm_r[i]);

                var decode1_headersB_clipchoice = this.decode1_headersB_clipchoice(
                    hip,
                    buffer,
                    bufferPos,
                    len,
                    pcmL,
                    0,
                    pcmR,
                    0,
                    mp3data,
                    enc,
                    @out,
                    OUTSIZE_UNCLIPPED,
                    dec,
                    tFactory);
                for (var i = 0; i < pcmL.Length; i++)
                    pcm_l[i] = pcmL[i].Value;

                for (var i = 0; i < pcmR.Length; i++)
                    pcm_r[i] = pcmR[i].Value;

                return decode1_headersB_clipchoice;
            }

            return 0;
        }

        /*
         * For lame_decode:  return code
         *  -1     error
         *   0     ok, but need more data before outputing any samples
         *   n     number of samples output.  Will be at most one frame of
         *         MPEG data.  
         */
        internal virtual int hip_decode1_headers(
            mpstr_tag hip,
            byte[] buffer,
            int len,
            short[] pcm_l,
            short[] pcm_r,
            MP3Data mp3data)
        {
            var enc = new Enc();
            return hip_decode1_headersB(hip, buffer, len, pcm_l, pcm_r, mp3data, enc);
        }

        internal virtual int hip_decode1_headersB(
            mpstr_tag hip,
            byte[] buffer,
            int len,
            short[] pcm_l,
            short[] pcm_r,
            MP3Data mp3data,
            Enc enc)
        {
            if (hip != null)
            {
                IDecoder dec = new IDecoderAnonymousInnerClass2(this);
                var @out = new short?[OUTSIZE_CLIPPED];
                Decode.Factory<short?> tFactory = new FactoryAnonymousInnerClass2(this);

                // XXX should we avoid the primitive type version?
                var pcmL = new short?[pcm_l.Length];
                for (var i = 0; i < pcmL.Length; i++)
                    pcmL[i] = Convert.ToInt16(pcm_l[i]);

                var pcmR = new short?[pcm_r.Length];
                for (var i = 0; i < pcmR.Length; i++)
                    pcmR[i] = Convert.ToInt16(pcm_r[i]);

                var decode1_headersB_clipchoice = this.decode1_headersB_clipchoice(
                    hip,
                    buffer,
                    0,
                    len,
                    pcmL,
                    0,
                    pcmR,
                    0,
                    mp3data,
                    enc,
                    @out,
                    OUTSIZE_CLIPPED,
                    dec,
                    tFactory);
                for (var i = 0; i < pcmL.Length; i++)
                    pcm_l[i] = pcmL[i].Value;

                for (var i = 0; i < pcmR.Length; i++)
                    pcm_r[i] = pcmR[i].Value;

                return decode1_headersB_clipchoice;
            }

            return -1;
        }

        internal virtual void hip_set_pinfo(mpstr_tag hip, PlottingData pinfo)
        {
            if (hip != null)
                hip.pinfo = pinfo;
        }
    }

}