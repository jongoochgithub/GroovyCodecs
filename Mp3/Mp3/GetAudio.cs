using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GroovyCodecs.Mp3.Common;
using GroovyCodecs.Mp3.Mpg;
using GroovyCodecs.Types;

/*  *	Get Audio routines source file  *
 *	Copyright (c) 1999 Albert L Faber  *
 *
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
/* $Id: GetAudio.java,v 1.26 2011/08/27 18:57:12 kenchis Exp $  */

namespace GroovyCodecs.Mp3.Mp3
{

    internal class GetAudio
    {

        internal enum sound_file_format
        {
            sf_unknown,

            sf_raw,

            sf_wave,

            sf_aiff,

            sf_mp1,

            sf_mp2,

            sf_mp3,

            sf_mp123,

            sf_ogg
        }

        protected internal sealed class BlockAlign
        {

            internal int blockSize;

            internal int offset;
        }

        protected internal sealed class IFF_AIFF
        {

            internal BlockAlign blkAlgn = new BlockAlign();

            internal short numChannels;

            internal int numSampleFrames;

            internal double sampleRate;

            internal short sampleSize;

            internal int sampleType;
        }

        private static readonly char[] abl2 =
        {
            (char)0,
            (char)7,
            (char)7,
            (char)7,
            (char)0,
            (char)7,
            (char)0,
            (char)0,
            (char)0,
            (char)0,
            (char)0,
            (char)8,
            (char)8,
            (char)8,
            (char)8,
            (char)8
        };

        private const int IFF_ID_2CBE = 0x74776f73;

        private const int IFF_ID_2CLE = 0x736f7774;

        private const int IFF_ID_AIFC = 0x41494643;

        private const int IFF_ID_AIFF = 0x41494646;

        private const int IFF_ID_COMM = 0x434f4d4d;

        private const int IFF_ID_FORM = 0x464f524d;

        private const int IFF_ID_NONE = 0x4e4f4e45;

        private const int IFF_ID_SSND = 0x53534e44;

        private static readonly string ISO_8859_1 = "ISO-8859-1";

        private const string type_name = "MP3 file";

        private const int WAV_ID_DATA = 0x64617461;

        private const int WAV_ID_FMT = 0x666d7420;

        private const int WAV_ID_RIFF = 0x52494646;

        private const int WAV_ID_WAVE = 0x57415645;

        private static readonly short WAVE_FORMAT_EXTENSIBLE = unchecked((short)0xFFFE);

        private const short WAVE_FORMAT_PCM = 0x0001;

        private bool count_samples_carefully;

        private MPGLib.mpstr_tag hip;

        internal MPGLib mpg;

        private FileStream musicin;

        private int num_samples_read;

        internal Parse parse;

        private bool pcm_is_unsigned_8bit;

        private int pcmbitwidth;

        private bool pcmswapbytes;

        internal virtual void setModules(Parse parse2, MPGLib mpg2)
        {
            parse = parse2;
            mpg = mpg2;
        }

        internal virtual FileStream init_outfile(string outPath)
        {
            FileStream outf;
            try
            {
                if (Directory.Exists(outPath)) Directory.Delete(outPath, true);
                else File.Delete(outPath);
                outf = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                return null;
            }

            return outf;
        }

        internal void init_infile(LameGlobalFlags gfp, string inPath, Enc enc)
        {
            count_samples_carefully = false;
            num_samples_read = 0;
            pcmbitwidth = parse.in_bitwidth;
            pcmswapbytes = parse.swapbytes;
            pcm_is_unsigned_8bit = !parse.in_signed;
            musicin = OpenSndFile(gfp, inPath, enc);
        }

        internal void close_infile()
        {
            closeSndFile(parse.input_format, musicin);
        }

        internal int get_audio(LameGlobalFlags gfp, int[][] buffer)
        {
            return get_audio_common(gfp, buffer, null);
        }

        internal int get_audio16(LameGlobalFlags gfp, short[][] buffer)
        {
            return get_audio_common(gfp, null, buffer);
        }

        private int get_audio_common(LameGlobalFlags gfp, int[][] buffer, short[][] buffer16)
        {
            var num_channels = gfp.num_channels;
            var insamp = new int[2 * 1152];

            var buf_tmp16 = Arrays.ReturnRectangularArray<short>(2, 1152);
            int samples_read;
            int framesize;
            int samples_to_read;
            int remaining, tmp_num_samples;
            samples_to_read = framesize = gfp.framesize;
            Debug.Assert(framesize <= 1152);
            tmp_num_samples = gfp.num_samples;
            if (count_samples_carefully)
            {
                remaining = tmp_num_samples - Math.Min(tmp_num_samples, num_samples_read);
                if (remaining < framesize && 0 != tmp_num_samples)
                    samples_to_read = remaining;
            }

            if (is_mpeg_file_format(parse.input_format))
            {
                if (buffer != null)
                    samples_read = read_samples_mp3(gfp, musicin, buf_tmp16);
                else
                    samples_read = read_samples_mp3(gfp, musicin, buffer16);

                if (samples_read < 0)
                    return samples_read;
            }
            else
            {
                samples_read = read_samples_pcm(musicin, insamp, num_channels * samples_to_read);
                if (samples_read < 0)
                    return samples_read;

                var p = samples_read;
                samples_read /= num_channels;
                if (buffer != null)
                {
                    if (num_channels == 2)
                    {
                        for (var i = samples_read; --i >= 0;)
                        {
                            buffer[1][i] = insamp[--p];
                            buffer[0][i] = insamp[--p];
                        }
                    }
                    else if (num_channels == 1)
                    {
                        Arrays.Fill(buffer[1], 0, samples_read, 0);
                        for (var i = samples_read; --i >= 0;)
                            buffer[0][i] = insamp[--p];
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                else
                {
                    if (num_channels == 2)
                    {
                        for (var i = samples_read; --i >= 0;)
                        {
                            buffer16[1][i] = unchecked((short)((insamp[--p] >> 16) & 0xffff));
                            buffer16[0][i] = unchecked((short)((insamp[--p] >> 16) & 0xffff));
                        }
                    }
                    else if (num_channels == 1)
                    {
                        Arrays.Fill(buffer16[1], 0, samples_read, (short)0);
                        for (var i = samples_read; --i >= 0;)
                            buffer16[0][i] = unchecked((short)((insamp[--p] >> 16) & 0xffff));
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
            }

            if (is_mpeg_file_format(parse.input_format))
                if (buffer != null)
                {
                    for (var i = samples_read; --i >= 0;)
                        buffer[0][i] = (buf_tmp16[0][i] & 0xffff) << 16;

                    if (num_channels == 2)
                        for (var i = samples_read; --i >= 0;)
                            buffer[1][i] = (buf_tmp16[1][i] & 0xffff) << 16;
                    else if (num_channels == 1)
                        Arrays.Fill(buffer[1], 0, samples_read, 0);
                    else
                        Debug.Assert(false);
                }

            if (tmp_num_samples != int.MaxValue)
                num_samples_read += samples_read;

            return samples_read;
        }

        internal virtual int read_samples_mp3(LameGlobalFlags gfp, FileStream musicin, short[][] mpg123pcm)
        {
            int @out;
            @out = lame_decode_fromfile(musicin, mpg123pcm[0], mpg123pcm[1], parse.mp3input_data);
            if (@out < 0)
            {
                Arrays.Fill(mpg123pcm[0], (short)0);
                Arrays.Fill(mpg123pcm[1], (short)0);
                return 0;
            }

            if (gfp.num_channels != parse.mp3input_data.stereo)
            {
                if (parse.silent < 10)
                    Console.WriteLine("Error: number of channels has changed in %s - not supported\n", type_name);

                @out = -1;
            }

            if (gfp.in_samplerate != parse.mp3input_data.samplerate)
            {
                if (parse.silent < 10)
                    Console.WriteLine("Error: sample frequency has changed in %s - not supported\n", type_name);

                @out = -1;
            }

            return @out;
        }

        internal int WriteWaveHeader(FileStream fp, int pcmbytes, int freq, int channels, int bits)
        {
            try
            {
                var bytes = (bits + 7) / 8;
                var riff = Encoding.UTF8.GetBytes("RIFF");
                fp.Write(riff, 0, riff.Length);
                write32BitsLowHigh(fp, pcmbytes + 44 - 8);
                var wavefmt = Encoding.UTF8.GetBytes("WAVEfmt ");
                fp.Write(wavefmt, 0, wavefmt.Length);
                write32BitsLowHigh(fp, 2 + 2 + 4 + 4 + 2 + 2);
                write16BitsLowHigh(fp, 1);
                write16BitsLowHigh(fp, channels);
                write32BitsLowHigh(fp, freq);
                write32BitsLowHigh(fp, freq * channels * bytes);
                write16BitsLowHigh(fp, channels * bytes);
                write16BitsLowHigh(fp, bits);
                var data = Encoding.UTF8.GetBytes("data ");
                fp.Write(data, 0, data.Length);
                write32BitsLowHigh(fp, pcmbytes);
            }
            catch (IOException)
            {
                return -1;
            }

            return 0;
        }

        private int unpack_read_samples(
            int samples_to_read,
            int bytes_per_sample,
            bool swap_order,
            int[] sample_buffer,
            FileStream pcm_in)
        {
            var bytes = new byte[bytes_per_sample * samples_to_read];
            pcm_in.Read(bytes, 0, bytes.Length);
            var samples_read = samples_to_read;
            var op = samples_read;
            if (!swap_order)
            {
                if (bytes_per_sample == 1)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = (bytes[i] & 0xff) << 24;

                if (bytes_per_sample == 2)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = ((bytes[i] & 0xff) << 16) | ((bytes[i + 1] & 0xff) << 24);

                if (bytes_per_sample == 3)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = ((bytes[i] & 0xff) << 8) | ((bytes[i + 1] & 0xff) << 16) |
                                              ((bytes[i + 2] & 0xff) << 24);

                if (bytes_per_sample == 4)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = (bytes[i] & 0xff) | ((bytes[i + 1] & 0xff) << 8) |
                                              ((bytes[i + 2] & 0xff) << 16) | ((bytes[i + 3] & 0xff) << 24);
            }
            else
            {
                if (bytes_per_sample == 1)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = (((bytes[i] ^ 0x80) & 0xff) << 24) | (0x7f << 16);

                if (bytes_per_sample == 2)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = ((bytes[i] & 0xff) << 24) | ((bytes[i + 1] & 0xff) << 16);

                if (bytes_per_sample == 3)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = ((bytes[i] & 0xff) << 24) | ((bytes[i + 1] & 0xff) << 16) |
                                              ((bytes[i + 2] & 0xff) << 8);

                if (bytes_per_sample == 4)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                        sample_buffer[--op] = ((bytes[i] & 0xff) << 24) | ((bytes[i + 1] & 0xff) << 16) |
                                              ((bytes[i + 2] & 0xff) << 8) | (bytes[i + 3] & 0xff);
            }

            return samples_read;
        }

        private int read_samples_pcm(FileStream musicin, int[] sample_buffer, int samples_to_read)
        {
            var samples_read = 0;
            bool swap_byte_order;
            try
            {
                switch (pcmbitwidth)
                {
                    case 32:
                    case 24:
                    case 16:
                        if (!parse.in_signed)
                            throw new Exception("Unsigned input only supported with bitwidth 8");

                    {
                        swap_byte_order = parse.in_endian != ByteOrder.LITTLE_ENDIAN;
                        if (pcmswapbytes)
                            swap_byte_order = !swap_byte_order;

                        samples_read = unpack_read_samples(
                            samples_to_read,
                            pcmbitwidth / 8,
                            swap_byte_order,
                            sample_buffer,
                            musicin);
                    }
                        break;
                    case 8:
                    {
                        samples_read = unpack_read_samples(
                            samples_to_read,
                            1,
                            pcm_is_unsigned_8bit,
                            sample_buffer,
                            musicin);
                    }
                        break;
                    default:
                    {
                        throw new Exception("Only 8, 16, 24 and 32 bit input files supported");
                    }
                }
            }
            catch (IOException e)
            {
                throw new Exception("Error reading input file", e);
            }

            return samples_read;
        }

        private int parse_wave_header(LameGlobalFlags gfp, FileStream sf)
        {
            var format_tag = 0;
            var channels = 0;
            var bits_per_sample = 0;
            var samples_per_sec = 0;
            var is_wav = false;
            int data_length = 0, subSize = 0;
            var loop_sanity = 0;
            Read32BitsHighLow(sf);
            if (Read32BitsHighLow(sf) != WAV_ID_WAVE)
                return -1;

            for (loop_sanity = 0; loop_sanity < 20; ++loop_sanity)
            {
                var type = Read32BitsHighLow(sf);
                if (type == WAV_ID_FMT)
                {
                    subSize = Read32BitsLowHigh(sf);
                    if (subSize < 16)
                        return -1;

                    format_tag = Read16BitsLowHigh(sf);
                    subSize -= 2;
                    channels = Read16BitsLowHigh(sf);
                    subSize -= 2;
                    samples_per_sec = Read32BitsLowHigh(sf);
                    subSize -= 4;
                    Read32BitsLowHigh(sf);
                    subSize -= 4;
                    Read16BitsLowHigh(sf);
                    subSize -= 2;
                    bits_per_sample = Read16BitsLowHigh(sf);
                    subSize -= 2;
                    if (subSize > 9 && format_tag == WAVE_FORMAT_EXTENSIBLE)
                    {
                        Read16BitsLowHigh(sf);
                        Read16BitsLowHigh(sf);
                        Read32BitsLowHigh(sf);
                        format_tag = Read16BitsLowHigh(sf);
                        subSize -= 10;
                    }

                    if (subSize > 0)
                        try
                        {
                            sf.Seek(subSize, SeekOrigin.Current);
                        }
                        catch (IOException)
                        {
                            return -1;
                        }
                }
                else if (type == WAV_ID_DATA)
                {
                    subSize = Read32BitsLowHigh(sf);
                    data_length = subSize;
                    is_wav = true;
                    break;
                }
                else
                {
                    subSize = Read32BitsLowHigh(sf);
                    try
                    {
                        sf.Seek(subSize, SeekOrigin.Current);
                    }
                    catch (IOException)
                    {
                        return -1;
                    }
                }
            }

            if (is_wav)
            {
                if (format_tag != WAVE_FORMAT_PCM)
                {
                    if (parse.silent < 10)
                        Console.WriteLine("Unsupported data format: 0x%04X\n", format_tag);

                    return 0;
                }

                if (-1 == (gfp.num_channels = channels))
                {
                    if (parse.silent < 10)
                        Console.WriteLine("Unsupported number of channels: %d\n", channels);

                    return 0;
                }

                gfp.in_samplerate = samples_per_sec;
                pcmbitwidth = bits_per_sample;
                pcm_is_unsigned_8bit = true;
                gfp.num_samples = data_length / (channels * ((bits_per_sample + 7) / 8));
                return 1;
            }

            return -1;
        }

        private int aiff_check2(IFF_AIFF pcm_aiff_data)
        {
            if (pcm_aiff_data.sampleType != IFF_ID_SSND)
            {
                if (parse.silent < 10)
                    Console.WriteLine("ERROR: input sound data is not PCM\n");

                return 1;
            }

            switch (pcm_aiff_data.sampleSize)
            {
                case 32:
                case 24:
                case 16:
                case 8:
                    break;
                default:
                    if (parse.silent < 10)
                        Console.WriteLine("ERROR: input sound data is not 8, 16, 24 or 32 bits\n");

                    return 1;
            }

            if (pcm_aiff_data.numChannels != 1 && pcm_aiff_data.numChannels != 2)
            {
                if (parse.silent < 10)
                    Console.WriteLine("ERROR: input sound data is not mono or stereo\n");

                return 1;
            }

            if (pcm_aiff_data.blkAlgn.blockSize != 0)
            {
                if (parse.silent < 10)
                    Console.WriteLine("ERROR: block size of input sound data is not 0 bytes\n");

                return 1;
            }

            return 0;
        }

        private long make_even_number_of_bytes_in_length(int x)
        {
            if ((x & 0x01) != 0)
                return x + 1;

            return x;
        }

        private int parse_aiff_header(LameGlobalFlags gfp, FileStream sf)
        {
            int subSize = 0, dataType = IFF_ID_NONE;
            var aiff_info = new IFF_AIFF();
            int seen_comm_chunk = 0, seen_ssnd_chunk = 0;
            long pcm_data_pos = -1;
            var chunkSize = Read32BitsHighLow(sf);
            var typeID = Read32BitsHighLow(sf);
            if (typeID != IFF_ID_AIFF && typeID != IFF_ID_AIFC)
                return -1;

            while (chunkSize > 0)
            {
                long ckSize;
                var type = Read32BitsHighLow(sf);
                chunkSize -= 4;
                if (type == IFF_ID_COMM)
                {
                    seen_comm_chunk = seen_ssnd_chunk + 1;
                    subSize = Read32BitsHighLow(sf);
                    ckSize = make_even_number_of_bytes_in_length(subSize);
                    chunkSize -= (int)ckSize;
                    aiff_info.numChannels = (short)Read16BitsHighLow(sf);
                    ckSize -= 2;
                    aiff_info.numSampleFrames = Read32BitsHighLow(sf);
                    ckSize -= 4;
                    aiff_info.sampleSize = (short)Read16BitsHighLow(sf);
                    ckSize -= 2;
                    try
                    {
                        aiff_info.sampleRate = readIeeeExtendedHighLow(sf);
                    }
                    catch (IOException)
                    {
                        return -1;
                    }

                    ckSize -= 10;
                    if (typeID == IFF_ID_AIFC)
                    {
                        dataType = Read32BitsHighLow(sf);
                        ckSize -= 4;
                    }

                    try
                    {
                        sf.Seek((int)ckSize, SeekOrigin.Current);
                    }
                    catch (IOException)
                    {
                        return -1;
                    }
                }
                else if (type == IFF_ID_SSND)
                {
                    seen_ssnd_chunk = 1;
                    subSize = Read32BitsHighLow(sf);
                    ckSize = make_even_number_of_bytes_in_length(subSize);
                    chunkSize -= (int)ckSize;
                    aiff_info.blkAlgn.offset = Read32BitsHighLow(sf);
                    ckSize -= 4;
                    aiff_info.blkAlgn.blockSize = Read32BitsHighLow(sf);
                    ckSize -= 4;
                    aiff_info.sampleType = IFF_ID_SSND;
                    if (seen_comm_chunk > 0)
                    {
                        try
                        {
                            sf.Seek(aiff_info.blkAlgn.offset, SeekOrigin.Current);
                        }
                        catch (IOException)
                        {
                            return -1;
                        }

                        break;
                    }

                    try
                    {
                        pcm_data_pos = sf.Position;
                    }
                    catch (IOException)
                    {
                        return -1;
                    }

                    if (pcm_data_pos >= 0)
                        pcm_data_pos += aiff_info.blkAlgn.offset;

                    try
                    {
                        sf.Seek((int)ckSize, SeekOrigin.Current);
                    }
                    catch (IOException)
                    {
                        return -1;
                    }
                }
                else
                {
                    subSize = Read32BitsHighLow(sf);
                    ckSize = make_even_number_of_bytes_in_length(subSize);
                    chunkSize -= (int)ckSize;
                    try
                    {
                        sf.Seek((int)ckSize, SeekOrigin.Current);
                    }
                    catch (IOException)
                    {
                        return -1;
                    }
                }
            }

            if (dataType == IFF_ID_2CLE)
                pcmswapbytes = parse.swapbytes;
            else if (dataType == IFF_ID_2CBE)
                pcmswapbytes = !parse.swapbytes;
            else if (dataType == IFF_ID_NONE)
                pcmswapbytes = !parse.swapbytes;
            else
                return -1;

            if (seen_comm_chunk != 0 && (seen_ssnd_chunk > 0 || aiff_info.numSampleFrames == 0))
            {
                if (0 != aiff_check2(aiff_info))
                    return 0;

                if (-1 == (gfp.num_channels = aiff_info.numChannels))
                {
                    if (parse.silent < 10)
                        Console.WriteLine("Unsupported number of channels: %u\n", aiff_info.numChannels);

                    return 0;
                }

                gfp.in_samplerate = (int)aiff_info.sampleRate;
                gfp.num_samples = aiff_info.numSampleFrames;
                pcmbitwidth = aiff_info.sampleSize;
                pcm_is_unsigned_8bit = false;
                if (pcm_data_pos >= 0)
                    try
                    {
                        sf.Seek(pcm_data_pos, SeekOrigin.Begin);
                    }
                    catch (IOException)
                    {
                        if (parse.silent < 10)
                            Console.WriteLine("Can't rewind stream to audio data position\n");

                        return 0;
                    }

                return 1;
            }

            return -1;
        }

        private sound_file_format parse_file_header(LameGlobalFlags gfp, FileStream sf)
        {
            var type = Read32BitsHighLow(sf);
            count_samples_carefully = false;
            pcm_is_unsigned_8bit = !parse.in_signed;
            if (type == WAV_ID_RIFF)
            {
                var ret = parse_wave_header(gfp, sf);
                if (ret > 0)
                {
                    count_samples_carefully = true;
                    return sound_file_format.sf_wave;
                }

                if (ret < 0)
                    if (parse.silent < 10)
                        Console.Error.WriteLine("Warning: corrupt or unsupported WAVE format");
            }
            else if (type == IFF_ID_FORM)
            {
                var ret = parse_aiff_header(gfp, sf);
                if (ret > 0)
                {
                    count_samples_carefully = true;
                    return sound_file_format.sf_aiff;
                }

                if (ret < 0)
                    if (parse.silent < 10)
                        Console.WriteLine("Warning: corrupt or unsupported AIFF format\n");
            }
            else
            {
                if (parse.silent < 10)
                    Console.Error.WriteLine("Warning: unsupported audio format\n");
            }

            return sound_file_format.sf_unknown;
        }

        private void closeSndFile(sound_file_format input, FileStream musicin)
        {
            if (musicin != null)
                try
                {
                    musicin.Close();
                }
                catch (IOException e)
                {
                    throw new Exception("Could not close sound file", e);
                }
        }

        private FileStream OpenSndFile(LameGlobalFlags gfp, string inPath, Enc enc)
        {
            gfp.num_samples = -1;
            try
            {
                musicin = new FileStream(inPath, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException e)
            {
                throw new Exception(string.Format("Could not find \"{0}\".", inPath), e);
            }

            if (is_mpeg_file_format(parse.input_format))
            {
                if (-1 == lame_decode_initfile(musicin, parse.mp3input_data, enc))
                    throw new Exception(string.Format("Error reading headers in mp3 input file {0}.", inPath));

                gfp.num_channels = parse.mp3input_data.stereo;
                gfp.in_samplerate = parse.mp3input_data.samplerate;
                gfp.num_samples = parse.mp3input_data.nsamp;
            }
            else if (parse.input_format == sound_file_format.sf_ogg)
            {
                throw new Exception("sorry, vorbis support in LAME is deprecated.");
            }
            else if (parse.input_format == sound_file_format.sf_raw)
            {
                if (parse.silent < 10)
                {
                    Console.WriteLine("Assuming raw pcm input file");
                    if (parse.swapbytes)
                        Console.Write(" : Forcing byte-swapping\n");
                    else
                        Console.Write("\n");
                }

                pcmswapbytes = parse.swapbytes;
            }
            else
            {
                parse.input_format = parse_file_header(gfp, musicin);
            }

            if (parse.input_format == sound_file_format.sf_unknown)
                throw new Exception("Unknown sound format!");

            if (gfp.num_samples == -1)
            {
                double flen = musicin.Length;
                if (flen >= 0)
                    if (is_mpeg_file_format(parse.input_format))
                    {
                        if (parse.mp3input_data.bitrate > 0)
                        {
                            var totalseconds = flen * 8.0 / (1000.0 * parse.mp3input_data.bitrate);
                            var tmp_num_samples = (int)(totalseconds * gfp.in_samplerate);
                            gfp.num_samples = tmp_num_samples;
                            parse.mp3input_data.nsamp = tmp_num_samples;
                        }
                    }
                    else
                    {
                        gfp.num_samples = (int)(flen / (2 * gfp.num_channels));
                    }
            }

            return musicin;
        }

        private bool check_aid(byte[] header)
        {
            return Encoding.GetEncoding(ISO_8859_1).GetString(header, 0, header.Length).StartsWith("AiD\x0001", StringComparison.Ordinal);
        }

        private bool is_syncword_mp123(byte[] headerptr)
        {
            var p = 0;
            if ((headerptr[p + 0] & 0xFF) != 0xFF)
                return false;

            if ((headerptr[p + 1] & 0xE0) != 0xE0)
                return false;

            if ((headerptr[p + 1] & 0x18) == 0x08)
                return false;

            switch (headerptr[p + 1] & 0x06)
            {
                default:
                    goto case 0x00;
                case 0x00:
                    return false;
                case 0x02:
                    if (parse.input_format != sound_file_format.sf_mp3 &&
                        parse.input_format != sound_file_format.sf_mp123)
                        return false;

                    parse.input_format = sound_file_format.sf_mp3;
                    break;
                case 0x04:
                    if (parse.input_format != sound_file_format.sf_mp2 &&
                        parse.input_format != sound_file_format.sf_mp123)
                        return false;

                    parse.input_format = sound_file_format.sf_mp2;
                    break;
                case 0x06:
                    if (parse.input_format != sound_file_format.sf_mp1 &&
                        parse.input_format != sound_file_format.sf_mp123)
                        return false;

                    parse.input_format = sound_file_format.sf_mp1;
                    break;
            }

            if ((headerptr[p + 1] & 0x06) == 0x00)
                return false;

            if ((headerptr[p + 2] & 0xF0) == 0xF0)
                return false;

            if ((headerptr[p + 2] & 0x0C) == 0x0C)
                return false;

            if ((headerptr[p + 1] & 0x18) == 0x18 && (headerptr[p + 1] & 0x06) == 0x04 &&
                (abl2[(headerptr[p + 2] & 0xff) >> 4] & (1 << ((headerptr[p + 3] & 0xff) >> 6))) != 0)
                return false;

            if ((headerptr[p + 3] & 3) == 2)
                return false;

            return true;
        }

        private int lame_decode_initfile(FileStream fd, MP3Data mp3data, Enc enc)
        {
            var buf = new byte[100];
            var pcm_l = new short[1152];
            var pcm_r = new short[1152];
            var freeformat = false;
            if (hip != null)
                mpg.hip_decode_exit(hip);

            hip = mpg.hip_decode_init();
            var len = 4;
            try
            {
                fd.Read(buf, 0, len);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                return -1;
            }

            if (buf[0] == (sbyte)'I' && buf[1] == (sbyte)'D' && buf[2] == (sbyte)'3')
            {
                if (parse.silent < 10)
                    Console.WriteLine(
                        "ID3v2 found. " + "Be aware that the ID3 tag is currently lost when transcoding.");

                len = 6;
                try
                {
                    fd.Read(buf, 0, len);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }

                buf[2] &= 127;
                buf[3] &= 127;
                buf[4] &= 127;
                buf[5] &= 127;
                len = (((((buf[2] << 7) + buf[3]) << 7) + buf[4]) << 7) + buf[5];
                try
                {
                    fd.Seek(len, SeekOrigin.Current);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }

                len = 4;
                try
                {
                    fd.Read(buf, 0, len);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }
            }

            if (check_aid(buf))
            {
                try
                {
                    fd.Read(buf, 0, 2);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }

                var aid_header = (buf[0] & 0xff) + 256 * (buf[1] & 0xff);
                if (parse.silent < 10)
                    Console.Write("Album ID found.  length={0:D} \n", aid_header);

                try
                {
                    fd.Seek(aid_header - 6, SeekOrigin.Current);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }

                try
                {
                    fd.Read(buf, 0, len);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }
            }

            len = 4;
            while (!is_syncword_mp123(buf))
            {
                int i;
                for (i = 0; i < len - 1; i++)
                    buf[i] = buf[i + 1];

                try
                {
                    fd.Read(buf, len - 1, 1);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }
            }

            if ((buf[2] & 0xf0) == 0)
            {
                if (parse.silent < 10)
                    Console.WriteLine("Input file is freeformat.");

                freeformat = true;
            }

            var ret = mpg.hip_decode1_headersB(hip, buf, len, pcm_l, pcm_r, mp3data, enc);
            if (-1 == ret)
                return -1;

            while (!mp3data.header_parsed)
            {
                try
                {
                    fd.Read(buf, 0, 1024);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }

                ret = mpg.hip_decode1_headersB(hip, buf, buf.Length, pcm_l, pcm_r, mp3data, enc);
                if (-1 == ret)
                    return -1;
            }

            if (mp3data.bitrate == 0 && !freeformat)
            {
                if (parse.silent < 10)
                    Console.Error.WriteLine("fail to sync...");

                return lame_decode_initfile(fd, mp3data, enc);
            }

            if (mp3data.totalframes > 0)
            {
            }
            else
            {
                mp3data.nsamp = -1;
            }

            return 0;
        }

        private int lame_decode_fromfile(FileStream fd, short[] pcm_l, short[] pcm_r, MP3Data mp3data)
        {
            var ret = 0;
            var len = 0;
            var buf = new byte[1024];
            ret = -1;
            ret = mpg.hip_decode1_headers(hip, buf, len, pcm_l, pcm_r, mp3data);
            if (ret != 0)
                return ret;

            while (true)
            {
                try
                {
                    len = fd.Read(buf, 0, buf.Length);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    return -1;
                }

                if (len <= 0)
                {
                    ret = mpg.hip_decode1_headers(hip, buf, 0, pcm_l, pcm_r, mp3data);
                    if (ret <= 0)
                    {
                        mpg.hip_decode_exit(hip);
                        hip = null;
                        return -1;
                    }

                    break;
                }

                ret = mpg.hip_decode1_headers(hip, buf, len, pcm_l, pcm_r, mp3data);
                if (ret == -1)
                {
                    mpg.hip_decode_exit(hip);
                    hip = null;
                    return -1;
                }

                if (ret > 0)
                    break;
            }

            return ret;
        }

        private bool is_mpeg_file_format(sound_file_format input_file_format)
        {
            switch (input_file_format)
            {
                case sound_file_format.sf_mp1:
                case sound_file_format.sf_mp2:
                case sound_file_format.sf_mp3:
                case sound_file_format.sf_mp123:
                    return true;
                default:
                    break;
            }

            return false;
        }

        private int Read32BitsLowHigh(FileStream fp)
        {
            var first = 0xffff & Read16BitsLowHigh(fp);
            var second = 0xffff & Read16BitsLowHigh(fp);
            var result = (second << 16) + first;
            return result;
        }

        private int Read16BitsLowHigh(FileStream fp)
        {
            try
            {
                var first = 0xff & fp.ReadByte();
                var second = 0xff & fp.ReadByte();
                var result = (second << 8) + first;
                return result;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                return 0;
            }
        }

        private int Read16BitsHighLow(FileStream fp)
        {
            try
            {
                var high = fp.ReadByte();
                var low = fp.ReadByte();
                return (high << 8) | low;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                return 0;
            }
        }

        private int Read32BitsHighLow(FileStream fp)
        {
            var first = 0xffff & Read16BitsHighLow(fp);
            var second = 0xffff & Read16BitsHighLow(fp);
            var result = (first << 16) + second;
            return result;
        }

        private double unsignedToFloat(double u)
        {
            return (long)(u - 2147483647L - 1) + 2147483648.0;
        }

        private double ldexp(double x, double exp)
        {
            return x * Math.Pow(2, exp);
        }

        private double convertFromIeeeExtended(byte[] bytes)
        {
            double f;
            long expon = ((bytes[0] & 0x7F) << 8) | (bytes[1] & 0xFF);
            var hiMant = ((long)(bytes[2] & 0xFF) << 24) | ((long)(bytes[3] & 0xFF) << 16) |
                         ((long)(bytes[4] & 0xFF) << 8) | (bytes[5] & 0xFF);
            var loMant = ((long)(bytes[6] & 0xFF) << 24) | ((long)(bytes[7] & 0xFF) << 16) |
                         ((long)(bytes[8] & 0xFF) << 8) | (bytes[9] & 0xFF);
            if (expon == 0 && hiMant == 0 && loMant == 0)
            {
                f = 0;
            }
            else
            {
                if (expon == 0x7FFF)
                {
                    f = double.PositiveInfinity;
                }
                else
                {
                    expon -= 16383;
                    f = (long)ldexp(unsignedToFloat(hiMant), (int)(expon -= 31));
                    f += (long)ldexp(unsignedToFloat(loMant), (int)(expon -= 32));
                }
            }

            if ((bytes[0] & 0x80) != 0)
                return -f;
            return f;
        }

        private double readIeeeExtendedHighLow(FileStream fp)
        {
            var bytes = new byte[10];
            var len = fp.Read(bytes, 0, bytes.Length);
            return convertFromIeeeExtended(bytes);
        }

        private void write32BitsLowHigh(FileStream fp, int i)
        {
            write16BitsLowHigh(fp, (int)(i & 0xffffL));
            write16BitsLowHigh(fp, (int)((i >> 16) & 0xffffL));
        }

        internal void write16BitsLowHigh(FileStream fp, int i)
        {
            fp.WriteByte((byte)(i & 0xff));
            fp.WriteByte((byte)((i >> 8) & 0xff));
        }
    }
}