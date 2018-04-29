//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using GroovyCodecs.Mp3.Mp3;

/*
 * interface.c
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */
/* $Id: Interface.java,v 1.12 2011/08/27 18:57:09 kenchis Exp $ */

namespace GroovyCodecs.Mp3.Mpg
{

    internal class Interface
    {

        internal interface ISynth
        {
            int synth_1to1_mono_ptr<T>(
                MPGLib.mpstr_tag mp,
                float[] @in,
                int inPos,
                T[] @out,
                MPGLib.ProcessedBytes p,
                Decode.Factory<T> tFactory);

            int synth_1to1_ptr<T>(
                MPGLib.mpstr_tag mp,
                float[] @in,
                int inPos,
                int i,
                T[] @out,
                MPGLib.ProcessedBytes p,
                Decode.Factory<T> tFactory);
        }

        private class ISynthAnonymousInnerClass<T> : ISynth
        {
            private readonly Interface outerInstance;

            private byte[] @in;

            private MPGLib.mpstr_tag mp;

            private T[] @out;

            private Decode.Factory<T> tFactory;

            internal ISynthAnonymousInnerClass(
                Interface outerInstance,
                MPGLib.mpstr_tag mp,
                byte[] @in,
                T[] @out,
                Decode.Factory<T> tFactory)
            {
                this.outerInstance = outerInstance;
                this.mp = mp;
                this.@in = @in;
                this.@out = @out;
                this.tFactory = tFactory;
            }

            public virtual int synth_1to1_mono_ptr<X>(
                MPGLib.mpstr_tag mp,
                float[] @in,
                int inPos,
                X[] @out,
                MPGLib.ProcessedBytes p,
                Decode.Factory<X> tFactory)
            {
                return outerInstance.decode.synth_1to1_mono(mp, @in, inPos, @out, p, tFactory);
            }

            public virtual int synth_1to1_ptr<X>(
                MPGLib.mpstr_tag mp,
                float[] @in,
                int inPos,
                int i,
                X[] @out,
                MPGLib.ProcessedBytes p,
                Decode.Factory<X> tFactory)
            {
                return outerInstance.decode.synth_1to1(mp, @in, inPos, i, @out, p, tFactory);
            }
        }

        private class ISynthAnonymousInnerClass2<T> : ISynth
        {
            private readonly Interface outerInstance;

            private byte[] @in;

            private MPGLib.mpstr_tag mp;

            private T[] @out;

            private Decode.Factory<T> tFactory;

            internal ISynthAnonymousInnerClass2(
                Interface outerInstance,
                MPGLib.mpstr_tag mp,
                byte[] @in,
                T[] @out,
                Decode.Factory<T> tFactory)
            {
                this.outerInstance = outerInstance;
                this.mp = mp;
                this.@in = @in;
                this.@out = @out;
                this.tFactory = tFactory;
            }

            public virtual int synth_1to1_mono_ptr<X>(
                MPGLib.mpstr_tag mp,
                float[] @in,
                int inPos,
                X[] @out,
                MPGLib.ProcessedBytes p,
                Decode.Factory<X> tFactory)
            {
                return outerInstance.decode.synth_1to1_mono_unclipped(mp, @in, inPos, @out, p, tFactory);
            }

            public virtual int synth_1to1_ptr<X>(
                MPGLib.mpstr_tag mp,
                float[] @in,
                int inPos,
                int i,
                X[] @out,
                MPGLib.ProcessedBytes p,
                Decode.Factory<X> tFactory)
            {
                return outerInstance.decode.synth_1to1_unclipped(mp, @in, inPos, i, @out, p, tFactory);
            }
        }

        /* number of bytes needed by GetVbrTag to parse header */
        internal const int XING_HEADER_SIZE = 194;

        private Common common;

        private readonly DCT64 dct = new DCT64();

        protected internal Decode decode = new Decode();

        private readonly Layer1 layer1 = new Layer1();

        private readonly Layer2 layer2 = new Layer2();

        private readonly Layer3 layer3 = new Layer3();

        private readonly TabInit tab = new TabInit();

        private VBRTag vbr;

        internal virtual void setModules(VBRTag v, Common c)
        {
            vbr = v;
            common = c;
            layer1.setModules(common, decode);
            layer2.Modules = common;
            layer3.Modules = common;
            decode.setModules(tab, dct);
            dct.Modules = tab;
        }

        internal virtual MPGLib.mpstr_tag InitMP3()
        {
            var mp = new MPGLib.mpstr_tag();

            mp.framesize = 0;
            mp.num_frames = 0;
            mp.enc_delay = -1;
            mp.enc_padding = -1;
            mp.vbr_header = false;
            mp.header_parsed = false;
            mp.side_parsed = false;
            mp.data_parsed = false;
            mp.free_format = false;
            mp.old_free_format = false;
            mp.ssize = 0;
            mp.dsize = 0;
            mp.fsizeold = -1;
            mp.bsize = 0;
            mp.head = mp.tail = null;
            mp.fr.single = -1;
            mp.bsnum = 0;
            mp.wordpointer = mp.bsspace[mp.bsnum];
            mp.wordpointerPos = 512;
            mp.bitindex = 0;
            mp.synth_bo = 1;
            mp.sync_bitstream = 1;

            tab.make_decode_tables(32767);

            layer3.init_layer3(MPG123.SBLIMIT);

            layer2.init_layer2();

            return mp;
        }

        internal virtual void ExitMP3(MPGLib.mpstr_tag mp)
        {
            MPGLib.buf b, bn;

            b = mp.tail;
            while (b != null)
            {
                b.pnt = null;
                bn = b.next;
                b = bn;
            }
        }

        internal virtual MPGLib.buf addbuf(MPGLib.mpstr_tag mp, byte[] de, int bufPos, int size)
        {
            var nbuf = new MPGLib.buf
            {
                pnt = new byte[size],
                size = size,
                next = null,
                prev = mp.head,
                pos = 0
            };
            Array.Copy(de, bufPos, nbuf.pnt, 0, size);

            if (null == mp.tail)
                mp.tail = nbuf;
            else
                mp.head.next = nbuf;

            mp.head = nbuf;
            mp.bsize += size;

            return nbuf;
        }

        internal virtual void remove_buf(MPGLib.mpstr_tag mp)
        {
            var buf = mp.tail;

            mp.tail = buf.next;
            if (mp.tail != null)
                mp.tail.prev = null;
            else
                mp.tail = mp.head = null;

            buf.pnt = null;
            buf = null;

        }

        internal virtual int read_buf_byte(MPGLib.mpstr_tag mp)
        {
            int b;

            int pos;

            pos = mp.tail.pos;
            while (pos >= mp.tail.size)
            {
                remove_buf(mp);
                if (null == mp.tail)
                    throw new Exception("hip: Fatal error! tried to read past mp buffer");

                pos = mp.tail.pos;
            }

            b = mp.tail.pnt[pos] & 0xff;
            mp.bsize--;
            mp.tail.pos++;

            return b;
        }

        internal virtual void read_head(MPGLib.mpstr_tag mp)
        {

            long head = read_buf_byte(mp);
            head <<= 8;
            head |= read_buf_byte(mp);
            head <<= 8;
            head |= read_buf_byte(mp);
            head <<= 8;
            head |= read_buf_byte(mp);

            mp.header = head;
        }

        internal virtual void copy_mp(MPGLib.mpstr_tag mp, int size, byte[] ptr, int ptrPos)
        {
            var len = 0;

            while (len < size && mp.tail != null)
            {
                int nlen;
                var blen = mp.tail.size - mp.tail.pos;
                if (size - len <= blen)
                    nlen = size - len;
                else
                    nlen = blen;

                Array.Copy(mp.tail.pnt, mp.tail.pos, ptr, ptrPos + len, nlen);
                len += nlen;
                mp.tail.pos += nlen;
                mp.bsize -= nlen;
                if (mp.tail.pos == mp.tail.size)
                    remove_buf(mp);
            }
        }

        /*
        traverse mp data structure without changing it
        (just like sync_buffer)
        pull out Xing bytes
        call vbr header check code from LAME
        if we find a header, parse it and also compute the VBR header size
        if no header, do nothing.
    
        bytes = number of bytes before MPEG header.  skip this many bytes
        before starting to read
        return value: number of bytes in VBR header, including syncword
        */
        internal virtual int check_vbr_header(MPGLib.mpstr_tag mp, int bytes)
        {
            int i, pos;
            var buf = mp.tail;
            var xing = new byte[XING_HEADER_SIZE];

            pos = buf.pos;
            /* skip to valid header */
            for (i = 0; i < bytes; ++i)
            {
                while (pos >= buf.size)
                {
                    buf = buf.next;
                    if (null == buf)
                        return -1; // fatal error

                    pos = buf.pos;
                }

                ++pos;
            }

            /* now read header */
            for (i = 0; i < XING_HEADER_SIZE; ++i)
            {
                while (pos >= buf.size)
                {
                    buf = buf.next;
                    if (null == buf)
                        return -1; // fatal error

                    pos = buf.pos;
                }

                xing[i] = buf.pnt[pos];
                ++pos;
            }

            /* check first bytes for Xing header */
            var pTagData = vbr.getVbrTag(xing);
            mp.vbr_header = pTagData != null;
            if (mp.vbr_header)
            {
                mp.num_frames = pTagData.frames;
                mp.enc_delay = pTagData.encDelay;
                mp.enc_padding = pTagData.encPadding;

                /* fprintf(stderr,"hip: delays: %i %i \n",mp.enc_delay,mp.enc_padding); */
                /* fprintf(stderr,"hip: Xing VBR header dectected.  MP3 file has %i frames\n", pTagData.frames); */
                if (pTagData.headersize < 1)
                    return 1;

                return pTagData.headersize;
            }

            return 0;
        }

        internal virtual int sync_buffer(MPGLib.mpstr_tag mp, bool free_match)
        {
            /* traverse mp structure without modifying pointers, looking
             * for a frame valid header.
             * if free_format, valid header must also have the same
             * samplerate.   
             * return number of bytes in mp, before the header
             * return -1 if header is not found
             */
            var b = new[]
            {
                0,
                0,
                0,
                0
            };
            int i, pos;
            bool h;
            var buf = mp.tail;
            if (null == buf)
                return -1;

            pos = buf.pos;
            for (i = 0; i < mp.bsize; i++)
            {
                /* get 4 bytes */

                b[0] = b[1];
                b[1] = b[2];
                b[2] = b[3];
                while (pos >= buf.size)
                {
                    buf = buf.next;
                    pos = buf.pos;
                }

                b[3] = buf.pnt[pos] & 0xff;
                ++pos;

                if (i >= 3)
                {
                    var fr = mp.fr;
                    long head;

                    head = b[0];
                    head <<= 8;
                    head |= b[1];
                    head <<= 8;
                    head |= b[2];
                    head <<= 8;
                    head |= b[3];
                    h = common.head_check(head, fr.lay);

                    if (h && free_match)
                    {
                        /* just to be even more thorough, match the sample rate */
                        int mode, stereo, sampling_frequency, lsf;
                        bool mpeg25;

                        if ((head & (1 << 20)) != 0)
                        {
                            lsf = (head & (1 << 19)) != 0 ? 0x0 : 0x1;
                            mpeg25 = false;
                        }
                        else
                        {
                            lsf = 1;
                            mpeg25 = true;
                        }

                        mode = (int)((head >> 6) & 0x3);
                        stereo = mode == MPG123.MPG_MD_MONO ? 1 : 2;

                        if (mpeg25)
                            sampling_frequency = (int)(6 + ((head >> 10) & 0x3));
                        else
                            sampling_frequency = (int)(((head >> 10) & 0x3) + lsf * 3);

                        h = stereo == fr.stereo && lsf == fr.lsf && mpeg25 == fr.mpeg25 &&
                            sampling_frequency == fr.sampling_frequency;
                    }

                    if (h)
                        return i - 3;
                }
            }

            return -1;
        }

        internal virtual MPGLib.mpstr_tag decode_reset()
        {
            return InitMP3(); // Less error prone to just to reinitialise.
        }

        internal virtual int audiodata_precedesframes(MPGLib.mpstr_tag mp)
        {
            if (mp.fr.lay == 3)
                return layer3.layer3_audiodata_precedesframes(mp);
            return
                0; // For Layer 1 & 2 the audio data starts at the frame that describes it, so no audio data precedes.
        }

        internal virtual int decodeMP3_clipchoice<T>(
            MPGLib.mpstr_tag mp,
            byte[] @in,
            int inPos,
            int isize,
            T[] @out,
            MPGLib.ProcessedBytes done,
            ISynth synth,
            Decode.Factory<T> tFactory)
        {
            int i, iret, bits, bytes;

            if (@in != null && isize != 0 && addbuf(mp, @in, inPos, isize) == null)
                return MPGLib.MP3_ERR;

            /* First decode header */
            if (!mp.header_parsed)
            {

                if (mp.fsizeold == -1 || mp.sync_bitstream != 0)
                {
                    int vbrbytes;
                    mp.sync_bitstream = 0;

                    /* This is the very first call.   sync with anything */
                    /* bytes= number of bytes before header */
                    bytes = sync_buffer(mp, false);

                    /* now look for Xing VBR header */
                    if (mp.bsize >= bytes + XING_HEADER_SIZE)
                        vbrbytes = check_vbr_header(mp, bytes);
                    else
                        return MPGLib.MP3_NEED_MORE;

                    if (mp.vbr_header)
                    {
                        /* do we have enough data to parse entire Xing header? */
                        if (bytes + vbrbytes > mp.bsize)
                            return MPGLib.MP3_NEED_MORE;

                        /* read in Xing header.  Buffer data in case it
                         * is used by a non zero main_data_begin for the next
                         * frame, but otherwise dont decode Xing header */
                        //	#ifdef HIP_DEBUG
                        //	                fprintf(stderr, "hip: found xing header, skipping %i bytes\n", vbrbytes + bytes);
                        //	#endif
                        for (i = 0; i < vbrbytes + bytes; ++i)
                            read_buf_byte(mp);
                        /* now we need to find another syncword */
                        /* just return and make user send in more data */

                        return MPGLib.MP3_NEED_MORE;
                    }
                }
                else
                {
                    /* match channels, samplerate, etc, when syncing */
                    bytes = sync_buffer(mp, true);
                }

                /* buffer now synchronized */
                if (bytes < 0)
                    return MPGLib.MP3_NEED_MORE;

                if (bytes > 0)
                {
                    /* there were some extra bytes in front of header.
                     * bitstream problem, but we are now resynced 
                     * should try to buffer previous data in case new
                     * frame has nonzero main_data_begin, but we need
                     * to make sure we do not overflow buffer
                     */
                    int size;
                    Console.WriteLine("hip: bitstream problem, resyncing skipping %d bytes...\n", bytes);
                    mp.old_free_format = false;

                    /* FIXME: correct ??? */
                    mp.sync_bitstream = 1;

                    /* skip some bytes, buffer the rest */
                    size = mp.wordpointerPos - 512;

                    if (size > MPG123.MAXFRAMESIZE)
                    {
                        /* wordpointer buffer is trashed.  probably cant recover, but try anyway */
                        Console.WriteLine(
                            "hip: wordpointer trashed.  size=%i (%i)  bytes=%i \n",
                            size,
                            MPG123.MAXFRAMESIZE,
                            bytes);
                        size = 0;
                        mp.wordpointer = mp.bsspace[mp.bsnum];
                        mp.wordpointerPos = 512;
                    }

                    /* buffer contains 'size' data right now 
                       we want to add 'bytes' worth of data, but do not 
                       exceed MAXFRAMESIZE, so we through away 'i' bytes */
                    i = size + bytes - MPG123.MAXFRAMESIZE;
                    for (; i > 0; --i)
                    {
                        --bytes;
                        read_buf_byte(mp);
                    }

                    copy_mp(mp, bytes, mp.wordpointer, mp.wordpointerPos);
                    mp.fsizeold += bytes;
                }

                read_head(mp);
                common.decode_header(mp.fr, mp.header);
                mp.header_parsed = true;
                mp.framesize = mp.fr.framesize;
                mp.free_format = mp.framesize == 0;

                if (mp.fr.lsf != 0)
                    mp.ssize = mp.fr.stereo == 1 ? 9 : 17;
                else
                    mp.ssize = mp.fr.stereo == 1 ? 17 : 32;

                if (mp.fr.error_protection)
                    mp.ssize += 2;

                mp.bsnum = 1 - mp.bsnum; // toggle buffer
                mp.wordpointer = mp.bsspace[mp.bsnum];
                mp.wordpointerPos = 512;
                mp.bitindex = 0;

                /* for very first header, never parse rest of data */
                if (mp.fsizeold == -1)
                    return MPGLib.MP3_NEED_MORE;
            } // end of header parsing block

            /* now decode side information */
            if (!mp.side_parsed)
            {

                /* Layer 3 only */
                if (mp.fr.lay == 3)
                {
                    if (mp.bsize < mp.ssize)
                        return MPGLib.MP3_NEED_MORE;

                    copy_mp(mp, mp.ssize, mp.wordpointer, mp.wordpointerPos);

                    if (mp.fr.error_protection)
                        common.getbits(mp, 16);

                    bits = layer3.do_layer3_sideinfo(mp);
                    /* bits = actual number of bits needed to parse this frame */
                    /* can be negative, if all bits needed are in the reservoir */
                    if (bits < 0)
                        bits = 0;

                    /* read just as many bytes as necessary before decoding */
                    mp.dsize = (bits + 7) / 8;

                    //	#ifdef HIP_DEBUG
                    //	            fprintf(stderr,
                    //	                    "hip: %d bits needed to parse layer III frame, number of bytes to read before decoding dsize = %d\n",
                    //	                    bits, mp.dsize);
                    //	#endif

                    /* this will force mpglib to read entire frame before decoding */
                    /* mp.dsize= mp.framesize - mp.ssize; */

                }
                else
                {
                    /* Layers 1 and 2 */

                    /* check if there is enough input data */
                    if (mp.fr.framesize > mp.bsize)
                        return MPGLib.MP3_NEED_MORE;

                    /* takes care that the right amount of data is copied into wordpointer */
                    mp.dsize = mp.fr.framesize;
                    mp.ssize = 0;
                }

                mp.side_parsed = true;
            }

            /* now decode main data */
            iret = MPGLib.MP3_NEED_MORE;
            if (!mp.data_parsed)
            {
                if (mp.dsize > mp.bsize)
                    return MPGLib.MP3_NEED_MORE;

                copy_mp(mp, mp.dsize, mp.wordpointer, mp.wordpointerPos);

                done.pb = 0;

                /*do_layer3(&mp.fr,(unsigned char *) out,done); */
                switch (mp.fr.lay)
                {
                    case 1:
                        if (mp.fr.error_protection)
                            common.getbits(mp, 16);

                        layer1.do_layer1(mp, @out, done, tFactory);
                        break;

                    case 2:
                        if (mp.fr.error_protection)
                            common.getbits(mp, 16);

                        layer2.do_layer2(mp, @out, done, synth, tFactory);
                        break;

                    case 3:
                        layer3.do_layer3(mp, @out, done, synth, tFactory);
                        break;
                    default:
                        Console.WriteLine("hip: invalid layer %d\n", mp.fr.lay);
                        break;
                }

                mp.wordpointer = mp.bsspace[mp.bsnum];
                mp.wordpointerPos = 512 + mp.ssize + mp.dsize;

                mp.data_parsed = true;
                iret = MPGLib.MP3_OK;
            }

            /* remaining bits are ancillary data, or reservoir for next frame 
             * If free format, scan stream looking for next frame to determine
             * mp.framesize */
            if (mp.free_format)
                if (mp.old_free_format)
                {
                    /* free format.  bitrate must not vary */
                    mp.framesize = mp.fsizeold_nopadding + mp.fr.padding;
                }
                else
                {
                    bytes = sync_buffer(mp, true);
                    if (bytes < 0)
                        return iret;

                    mp.framesize = bytes + mp.ssize + mp.dsize;
                    mp.fsizeold_nopadding = mp.framesize - mp.fr.padding;
                    /*
                       fprintf(stderr,"hip: freeformat bitstream:  estimated bitrate=%ikbs  \n",
                       8*(4+mp.framesize)*freqs[mp.fr.sampling_frequency]/
                       (1000*576*(2-mp.fr.lsf)));
                     */
                }

            /* buffer the ancillary data and reservoir for next frame */
            bytes = mp.framesize - (mp.ssize + mp.dsize);
            if (bytes > mp.bsize)
                return iret;

            if (bytes > 0)
            {
                int size;
                copy_mp(mp, bytes, mp.wordpointer, mp.wordpointerPos);
                mp.wordpointerPos += bytes;

                size = mp.wordpointerPos - 512;
                if (size > MPG123.MAXFRAMESIZE)
                    Console.WriteLine("hip: fatal error.  MAXFRAMESIZE not large enough.\n");

            }

            /* the above frame is completely parsed.  start looking for next frame */
            mp.fsizeold = mp.framesize;
            mp.old_free_format = mp.free_format;
            mp.framesize = 0;
            mp.header_parsed = false;
            mp.side_parsed = false;
            mp.data_parsed = false;

            return iret;
        }

        internal virtual int decodeMP3<T>(
            MPGLib.mpstr_tag mp,
            byte[] @in,
            int bufferPos,
            int isize,
            T[] @out,
            int osize,
            MPGLib.ProcessedBytes done,
            Decode.Factory<T> tFactory)
        {
            if (osize < 2304)
            {
                Console.WriteLine("hip: Insufficient memory for decoding buffer %d\n", osize);
                return MPGLib.MP3_ERR;
            }

            /* passing pointers to the functions which clip the samples */
            ISynth synth = new ISynthAnonymousInnerClass<T>(this, mp, @in, @out, tFactory);
            return decodeMP3_clipchoice(mp, @in, bufferPos, isize, @out, done, synth, tFactory);
        }

        internal virtual int decodeMP3_unclipped<T>(
            MPGLib.mpstr_tag mp,
            byte[] @in,
            int bufferPos,
            int isize,
            T[] @out,
            int osize,
            MPGLib.ProcessedBytes done,
            Decode.Factory<T> tFactory)
        {
            /* we forbid input with more than 1152 samples per channel for output in unclipped mode */
            if (osize < 1152 * 2)
            {
                Console.WriteLine("hip: out space too small for unclipped mode\n");
                return MPGLib.MP3_ERR;
            }

            ISynth synth = new ISynthAnonymousInnerClass2<T>(this, mp, @in, @out, tFactory);
            /* passing pointers to the functions which don't clip the samples */
            return decodeMP3_clipchoice(mp, @in, bufferPos, isize, @out, done, synth, tFactory);
        }
    }

}