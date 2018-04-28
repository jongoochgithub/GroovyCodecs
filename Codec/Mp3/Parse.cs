using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GroovyMp3.Types;
using GroovyMp3.Types;

/*  *      Command line parsing related functions  *
 *      Copyright (c) 1999 Mark Taylor  *
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

/* $Id: Parse.java,v 1.28 2011/05/24 22:17:17 kenchis Exp $ */
namespace GroovyMp3.Codec.Mp3
{

    internal class Parse
    {

        private enum TextEncoding
        {
            TENC_RAW,

            TENC_LATIN1,

            TENC_UCS2
        }

        private enum ID3TAG_MODE
        {
            ID3TAG_MODE_DEFAULT,

            ID3TAG_MODE_V1_ONLY,

            ID3TAG_MODE_V2_ONLY
        }

        internal class NoGap
        {
            internal int num_nogap;
        }

        private class GenreListHandlerAnonymousInnerClass : GenreListHandler
        {
            private readonly Parse outerInstance;

            internal GenreListHandlerAnonymousInnerClass(Parse outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void genre_list_handler(int num, string name)
            {
                Console.Write("{0,3:D} {1}\n", num, name);
            }
        }

        private static readonly bool INTERNAL_OPTS = false;

        internal bool brhist;

        internal bool disable_wav_header;

        internal ID3Tag id3;

        private bool ignore_tag_errors;

        internal int in_bitwidth = 16;

        internal ByteOrder in_endian = ByteOrder.LITTLE_ENDIAN;

        internal bool in_signed = true;

        internal GetAudio.sound_file_format input_format;

        internal int mp3_delay;

        internal bool mp3_delay_set;

        internal MP3Data mp3input_data = new MP3Data();

        internal Presets pre;

        internal bool print_clipping_info;

        internal int silent;

        internal bool swapbytes;

        internal float update_interval;

        internal Mp3Version ver;

        internal void setModules(Mp3Version ver2, ID3Tag id32, Presets pre2)
        {
            ver = ver2;
            id3 = id32;
            pre = pre2;
        }

        private bool set_id3tag(LameGlobalFlags gfp, int type, string str)
        {
            switch (type)
            {
                case 'a':
                    id3.id3tag_set_artist(gfp, str);
                    return false;
                case 't':
                    id3.id3tag_set_title(gfp, str);
                    return false;
                case 'l':
                    id3.id3tag_set_album(gfp, str);
                    return false;
                case 'g':
                    id3.id3tag_set_genre(gfp, str);
                    return false;
                case 'c':
                    id3.id3tag_set_comment(gfp, str);
                    return false;
                case 'n':
                    id3.id3tag_set_track(gfp, str);
                    return false;
                case 'y':
                    id3.id3tag_set_year(gfp, str);
                    return false;
                case 'v':
                    id3.id3tag_set_fieldvalue(gfp, str);
                    return false;
            }

            return false;
        }

        private bool set_id3v2tag(LameGlobalFlags gfp, int type, string str)
        {
            switch (type)
            {
                case 'a':
                    id3.id3tag_set_textinfo_ucs2(gfp, "TPE1", str);
                    return false;
                case 't':
                    id3.id3tag_set_textinfo_ucs2(gfp, "TIT2", str);
                    return false;
                case 'l':
                    id3.id3tag_set_textinfo_ucs2(gfp, "TALB", str);
                    return false;
                case 'g':
                    id3.id3tag_set_textinfo_ucs2(gfp, "TCON", str);
                    return false;
                case 'c':
                    id3.id3tag_set_comment(gfp, null, null, str, 0);
                    return false;
                case 'n':
                    id3.id3tag_set_textinfo_ucs2(gfp, "TRCK", str);
                    return false;
            }

            return false;
        }

        private bool id3_tag(LameGlobalFlags gfp, int type, TextEncoding enc, string str)
        {
            string x = null;
            bool result;
            switch (enc)
            {
                default:
                    x = str;
                    break;
                case TextEncoding.TENC_RAW:
                    x = str;
                    break;
                case TextEncoding.TENC_LATIN1:
                    x = str;
                    break;
                case TextEncoding.TENC_UCS2:
                    x = str;
                    break;
            }

            switch (enc)
            {
                case TextEncoding.TENC_UCS2:
                    result = set_id3v2tag(gfp, type, x);
                    break;
                default:
                case TextEncoding.TENC_RAW:
                case TextEncoding.TENC_LATIN1:
                    result = set_id3tag(gfp, type, x);
                    break;
            }

            return result;
        }

        private void lame_version_print(TextWriter fp)
        {

            var b = ver.LameOsBitness;

            var v = ver.LameVersion;

            var u = ver.LameUrl;

            var lenb = b.Length;

            var lenv = v.Length;

            var lenu = u.Length;
            const int lw = 80;
            const int sw = 16;
            if (lw >= lenb + lenv + lenu + sw || lw < lenu + 2)
            {
                if (lenb > 0)
                    fp.Write("LAME %s version %s (%s)\n\n", b, v, u);
                else
                    fp.Write("LAME version %s (%s)\n\n", v, u);
            }
            else
            {
                if (lenb > 0)
                    fp.Write("LAME %s version %s\n%*s(%s)\n\n", b, v, lw - 2 - lenu, "", u);
                else
                    fp.Write("LAME version %s\n%*s(%s)\n\n", v, lw - 2 - lenu, "", u);
            }
        }

        private void print_license(TextWriter fp)
        {
            lame_version_print(fp);
            fp.GetHashCode();
            fp.Write(
                "1. In your program, you cannot include any source code from LAME, with\n" +
                "   the exception of files whose only purpose is to describe the library\n" +
                "   interface (such as lame.h).\n" + "\n");
            fp.Write(
                "2. Any modifications of LAME must be released under the LGPL.\n" +
                "   The LAME project (www.mp3dev.org) would appreciate being\n" +
                "   notified of any modifications.\n" + "\n");
            fp.Write(
                "3. You must give prominent notice that your program is:\n" +
                "      A. using LAME (including version number)\n" + "      B. LAME is under the LGPL\n" +
                "      C. Provide a copy of the LGPL.  (the file COPYING contains the LGPL)\n" +
                "      D. Provide a copy of LAME source, or a pointer where the LAME\n" +
                "         source can be obtained (such as http://sourceforge.net/projects/jsidplay2/)\n" +
                "   An example of prominent notice would be an \"About the LAME encoding engine\"\n" +
                "   button in some pull down menu within the executable of your program.\n" + "\n");
            fp.Write(
                "4. If you determine that distribution of LAME requires a patent license,\n" +
                "   you must obtain such license.\n" + "\n" + "\n");
            fp.Write(
                "*** IMPORTANT NOTE ***\n" + "\n" +
                "The decoding functions provided in LAME use the mpglib decoding engine which\n" +
                "is under the GPL.  They may not be used by any program not released under the\n" +
                "GPL unless you obtain such permission from the MPG123 project (www.mpg123.de).\n" + "\n");
        }

        internal void usage(TextWriter fp, string ProgramName)
        {
            lame_version_print(fp);
            fp.Write(
                "usage: %s [options] <infile> [outfile]\n" + "\n" +
                "    <infile> and/or <outfile> can be \"-\", which means stdin/stdout.\n" + "\n" + "Try:\n" +
                "     \"%s --help\"           for general usage information\n" + " or:\n" +
                "     \"%s --preset help\"    for information on suggested predefined settings\n" + " or:\n" +
                "     \"%s --longhelp\"\n" + "  or \"%s -?\"              for a complete options list\n\n",
                ProgramName,
                ProgramName,
                ProgramName,
                ProgramName,
                ProgramName);
        }

        private void short_help(LameGlobalFlags gfp, TextWriter fp, string ProgramName)
        {
            lame_version_print(fp);
            fp.Write(
                "usage: %s [options] <infile> [outfile]\n" + "\n" +
                "    <infile> and/or <outfile> can be \"-\", which means stdin/stdout.\n" + "\n" + "RECOMMENDED:\n" +
                "    lame -V 2 input.wav output.mp3\n" + "\n",
                ProgramName);
            fp.Write(
                "OPTIONS:\n" + "    -b bitrate      set the bitrate, default 128 kbps\n" +
                "    -h              higher quality, but a little slower.  Recommended.\n" +
                "    -f              fast mode (lower quality)\n" +
                "    -V n            quality setting for VBR.  default n=%d\n" +
                "                    0=high quality,bigger files. 9=smaller files\n",
                gfp.VBR_q);
            fp.Write(
                "    --preset type   type must be \"medium\", \"standard\", \"extreme\", \"insane\",\n" +
                "                    or a value for an average desired bitrate and depending\n" +
                "                    on the value specified, appropriate quality settings will\n" +
                "                    be used.\n" + "                    \"--preset help\" gives more info on these\n" +
                "\n");
            fp.Write(
                "    --longhelp      full list of options\n" + "\n" +
                "    --license       print License information\n\n");
        }

        private void long_help(LameGlobalFlags gfp, TextWriter fp, string ProgramName, int lessmode)
        {
            lame_version_print(fp);
            fp.Write(
                "usage: %s [options] <infile> [outfile]\n" + "\n" +
                "    <infile> and/or <outfile> can be \"-\", which means stdin/stdout.\n" + "\n" + "RECOMMENDED:\n" +
                "    lame -V2 input.wav output.mp3\n" + "\n",
                ProgramName);
            fp.Write(
                "OPTIONS:\n" + "  Input options:\n" + "    --scale <arg>   scale input (multiply PCM data) by <arg>\n" +
                "    --scale-l <arg> scale channel 0 (left) input (multiply PCM data) by <arg>\n" +
                "    --scale-r <arg> scale channel 1 (right) input (multiply PCM data) by <arg>\n" +
                "    --mp1input      input file is a MPEG Layer I   file\n" +
                "    --mp2input      input file is a MPEG Layer II  file\n" +
                "    --mp3input      input file is a MPEG Layer III file\n" + "    --nogap <file1> <file2> <...>\n" +
                "                    gapless encoding for a set of contiguous files\n" + "    --nogapout <dir>\n" +
                "                    output dir for gapless encoding (must precede --nogap)\n" +
                "    --nogaptags     allow the use of VBR tags in gapless encoding\n");
            fp.Write(
                "\n" + "  Input options for RAW PCM:\n" + "    -r              input is raw pcm\n" +
                "    -x              force byte-swapping of input\n" +
                "    -s sfreq        sampling frequency of input file (kHz) - default 44.1 kHz\n" +
                "    --bitwidth w    input bit width is w (default 16)\n" +
                "    --signed        input is signed (default)\n" + "    --unsigned      input is unsigned\n" +
                "    --little-endian input is little-endian (default)\n" + "    --big-endian    input is big-endian\n");
            fp.Write(
                "  Operational options:\n" +
                "    -a              downmix from stereo to mono file for mono encoding\n" +
                "    -m <mode>       (j)oint, (s)imple, (f)orce, (d)dual-mono, (m)ono\n" +
                "                    default is (j) or (s) depending on bitrate\n" +
                "                    joint  = joins the best possible of MS and LR stereo\n" +
                "                    simple = force LR stereo on all frames\n" +
                "                    force  = force MS stereo on all frames.\n" +
                "    --preset type   type must be \"medium\", \"standard\", \"extreme\", \"insane\",\n" +
                "                    or a value for an average desired bitrate and depending\n" +
                "                    on the value specified, appropriate quality settings will\n" +
                "                    be used.\n" + "                    \"--preset help\" gives more info on these\n" +
                "    --comp  <arg>   choose bitrate to achive a compression ratio of <arg>\n");
            fp.Write(
                "    --replaygain-fast   compute RG fast but slightly inaccurately (default)\n" +
                "    --replaygain-accurate   compute RG more accurately and find the peak sample\n" +
                "    --noreplaygain  disable ReplayGain analysis\n" +
                "    --clipdetect    enable --replaygain-accurate and print a message whether\n" +
                "                    clipping occurs and how far the waveform is from full scale\n");
            fp.Write(
                "    --freeformat    produce a free format bitstream\n" +
                "    --decode        input=mp3 file, output=wav\n" +
                "    -t              disable writing wav header when using --decode\n");
            fp.Write(
                "  Verbosity:\n" + "    --disptime <arg>print progress report every arg seconds\n" +
                "    -S              don't print progress report, VBR histograms\n" +
                "    --nohist        disable VBR histogram display\n" +
                "    --silent        don't print anything on screen\n" +
                "    --quiet         don't print anything on screen\n" +
                "    --brief         print more useful information\n" +
                "    --verbose       print a lot of useful information\n" + "\n");
            fp.Write(
                "  Noise shaping & psycho acoustic algorithms:\n" +
                "    -q <arg>        <arg> = 0...9.  Default  -q 5 \n" +
                "                    -q 0:  Highest quality, very slow \n" +
                "                    -q 9:  Poor quality, but fast \n" +
                "    -h              Same as -q 2.   Recommended.\n" +
                "    -f              Same as -q 7.   Fast, ok quality\n");
            fp.Write(
                "  CBR (constant bitrate, the default) options:\n" +
                "    -b <bitrate>    set the bitrate in kbps, default 128 kbps\n" +
                "    --cbr           enforce use of constant bitrate\n" + "\n" + "  ABR options:\n" +
                "    --abr <bitrate> specify average bitrate desired (instead of quality)\n" + "\n");
            fp.Write(
                "  VBR options:\n" + "    -V n            quality setting for VBR.  default n=%d\n" +
                "                    0=high quality,bigger files. 9=smaller files\n" +
                "    -v              the same as -V 4\n" +
                "    --vbr-old       use old variable bitrate (VBR) routine\n" +
                "    --vbr-new       use new variable bitrate (VBR) routine (default)\n",
                gfp.VBR_q);
            fp.Write(
                "    -b <bitrate>    specify minimum allowed bitrate, default  32 kbps\n" +
                "    -B <bitrate>    specify maximum allowed bitrate, default 320 kbps\n" +
                "    -F              strictly enforce the -b option, for use with players that\n" +
                "                    do not support low bitrate mp3\n" +
                "    -t              disable writing LAME Tag\n" +
                "    -T              enable and force writing LAME Tag\n");
            fp.Write(
                "  ATH related:\n" + "    --noath         turns ATH down to a flat noise floor\n" +
                "    --athshort      ignore GPSYCHO for short blocks, use ATH only\n" +
                "    --athonly       ignore GPSYCHO completely, use ATH only\n" +
                "    --athtype n     selects between different ATH types [0-4]\n" +
                "    --athlower x    lowers ATH by x dB\n");
            fp.Write(
                "    --athaa-type n  ATH auto adjust: 0 'no' else 'loudness based'\n" +
                "    --athaa-sensitivity x  activation offset in -/+ dB for ATH auto-adjustment\n" + "\n");
            fp.Write(
                "  PSY related:\n" + "    --short         use short blocks when appropriate\n" +
                "    --noshort       do not use short blocks\n" + "    --allshort      use only short blocks\n");
            fp.Write(
                "    --temporal-masking x   x=0 disables, x=1 enables temporal masking effect\n" +
                "    --nssafejoint   M/S switching criterion\n" +
                "    --nsmsfix <arg> M/S switching tuning [effective 0-3.5]\n" +
                "    --interch x     adjust inter-channel masking ratio\n" +
                "    --ns-bass x     adjust masking for sfbs  0 -  6 (long)  0 -  5 (short)\n" +
                "    --ns-alto x     adjust masking for sfbs  7 - 13 (long)  6 - 10 (short)\n" +
                "    --ns-treble x   adjust masking for sfbs 14 - 21 (long) 11 - 12 (short)\n");
            fp.Write(
                "    --ns-sfb21 x    change ns-treble by x dB for sfb21\n" +
                "    --shortthreshold x,y  short block switching threshold,\n" +
                "                          x for L/R/M channel, y for S channel\n" + "  Noise Shaping related:\n" +
                "    --substep n     use pseudo substep noise shaping method types 0-2\n");
            fp.Write(
                "  experimental switches:\n" + "    -X n[,m]        selects between different noise measurements\n" +
                "                    n for long block, m for short. if m is omitted, m = n\n" +
                "    -Y              lets LAME ignore noise in sfb21, like in CBR\n" +
                "    -Z [n]          currently no effects\n");
            fp.Write(
                "  MP3 header/stream options:\n" + "    -e <emp>        de-emphasis n/5/c  (obsolete)\n" +
                "    -c              mark as copyright\n" + "    -o              mark as non-original\n" +
                "    -p              error protection.  adds 16 bit checksum to every frame\n" +
                "                    (the checksum is computed correctly)\n" +
                "    --nores         disable the bit reservoir\n" +
                "    --strictly-enforce-ISO   comply as much as possible to ISO MPEG spec\n" + "\n");
            fp.Write(
                "  Filter options:\n" + "  --lowpass <freq>        frequency(kHz), lowpass filter cutoff above freq\n" +
                "  --lowpass-width <freq>  frequency(kHz) - default 15%% of lowpass freq\n" +
                "  --highpass <freq>       frequency(kHz), highpass filter cutoff below freq\n" +
                "  --highpass-width <freq> frequency(kHz) - default 15%% of highpass freq\n");
            fp.Write("  --resample <sfreq>  sampling frequency of output file(kHz)- default=automatic\n");
            fp.Write(
                "  ID3 tag options:\n" + "    --tt <title>    audio/song title (max 30 chars for version 1 tag)\n" +
                "    --ta <artist>   audio/song artist (max 30 chars for version 1 tag)\n" +
                "    --tl <album>    audio/song album (max 30 chars for version 1 tag)\n" +
                "    --ty <year>     audio/song year of issue (1 to 9999)\n" +
                "    --tc <comment>  user-defined text (max 30 chars for v1 tag, 28 for v1.1)\n" +
                "    --tn <track[/total]>   audio/song track number and (optionally) the total\n" +
                "                           number of tracks on the original recording. uniquetempvar.\n" +
                "    --tg <genre>    audio/song genre (name or number in list)\n" +
                "    --ti <file>     audio/song albumArt (jpeg/png/gif file, 128KB max, v2.3)\n" +
                "    --tv <id=value> user-defined frame specified by id and value (v2.3 tag)\n");
            fp.Write(
                "    --add-id3v2     force addition of version 2 tag\n" +
                "    --id3v1-only    add only a version 1 tag\n" + "    --id3v2-only    add only a version 2 tag\n" +
                "    --space-id3v1   pad version 1 tag with spaces instead of nulls\n" +
                "    --pad-id3v2     same as '--pad-id3v2-size 128'\n" +
                "    --pad-id3v2-size <value> adds version 2 tag, pad with extra <value> bytes\n" +
                "    --genre-list    print alphabetically sorted ID3 genre list and exit\n" +
                "    --ignore-tag-errors  ignore errors in values passed for tags\n" + "\n");
            fp.Write(
                "    Note: A version 2 tag will NOT be added unless one of the input fields\n" +
                "    won't fit in a version 1 tag (e.g. the title string is longer than 30\n" +
                "    characters), or the '--add-id3v2' or '--id3v2-only' options are used,\n" +
                "    or output is redirected to stdout.\n" +
                "\nMisc:\n    --license       print License information\n\n");
            display_bitrates(fp);
        }

        internal void display_bitrates(TextWriter fp)
        {
            display_bitrate(fp, "1", 1, 1);
            display_bitrate(fp, "2", 2, 0);
            display_bitrate(fp, "2.5", 4, 0);
            fp.WriteLine();
        }

        private void display_bitrate(TextWriter fp, string version, int d, int indx)
        {
            var nBitrates = 14;
            if (d == 4)
                nBitrates = 8;

            fp.Write(
                "\nMPEG-%-3s layer III sample frequencies (kHz):  %2d  %2d  %g\n" + "bitrates (kbps):",
                version,
                32 / d,
                48 / d,
                44.1 / d);
            for (var i = 1; i <= nBitrates; i++)
                fp.Write(" %2d", Tables.bitrate_table[indx][i]);

            fp.WriteLine();
        }

        private void presets_longinfo_dm(TextWriter msgfp)
        {
            msgfp.Write("\n" + "The --preset switches are aliases over LAME settings.\n" + "\n" + "\n");
            msgfp.Write(
                "To activate these presets:\n" + "\n" + "   For VBR modes (generally highest quality):\n" + "\n");
            msgfp.Write(
                "     \"--preset medium\" This preset should provide near transparency\n" +
                "                             to most people on most music.\n" + "\n" +
                "     \"--preset standard\" This preset should generally be transparent\n" +
                "                             to most people on most music and is already\n" +
                "                             quite high in quality.\n" + "\n");
            msgfp.Write(
                "     \"--preset extreme\" If you have extremely good hearing and similar\n" +
                "                             equipment, this preset will generally provide\n" +
                "                             slightly higher quality than the \"standard\"\n" +
                "                             mode.\n" + "\n");
            msgfp.Write(
                "   For CBR 320kbps (highest quality possible from the --preset switches):\n" + "\n" +
                "     \"--preset insane\"  This preset will usually be overkill for most\n" +
                "                             people and most situations, but if you must\n" +
                "                             have the absolute highest quality with no\n" +
                "                             regard to filesize, this is the way to go.\n" + "\n");
            msgfp.Write(
                "   For ABR modes (high quality per given bitrate but not as high as VBR):\n" + "\n" +
                "     \"--preset <kbps>\"  Using this preset will usually give you good\n" +
                "                             quality at a specified bitrate. Depending on the\n" +
                "                             bitrate entered, this preset will determine the\n");
            msgfp.Write(
                "                             optimal settings for that particular situation.\n" +
                "                             While this approach works, it is not nearly as\n" +
                "                             flexible as VBR, and usually will not attain the\n" +
                "                             same level of quality as VBR at higher bitrates.\n" + "\n");
            msgfp.Write(
                "The following options are also available for the corresponding profiles:\n" + "\n" +
                "   <fast>        standard\n" + "   <fast>        extreme\n" + "                 insane\n" +
                "   <cbr> (ABR Mode) - The ABR Mode is implied. To use it,\n" +
                "                      simply specify a bitrate. For example:\n" +
                "                      \"--preset 185\" activates this\n" +
                "                      preset and uses 185 as an average kbps.\n" + "\n");
            msgfp.Write("   \"fast\" - Enables the fast VBR mode for a particular profile.\n" + "\n");
            msgfp.Write(
                "   \"cbr\"  - If you use the ABR mode (read above) with a significant\n" +
                "            bitrate such as 80, 96, 112, 128, 160, 192, 224, 256, 320,\n" +
                "            you can use the \"cbr\" option to force CBR mode encoding\n" +
                "            instead of the standard abr mode. ABR does provide higher\n" +
                "            quality but CBR may be useful in situations such as when\n" +
                "            streaming an mp3 over the internet may be important.\n" + "\n");
            msgfp.Write(
                "    For example:\n" + "\n" + "    \"--preset fast standard <input file> <output file>\"\n" +
                " or \"--preset cbr 192 <input file> <output file>\"\n" +
                " or \"--preset 172 <input file> <output file>\"\n" +
                " or \"--preset extreme <input file> <output file>\"\n" + "\n" + "\n");
            msgfp.Write(
                "A few aliases are also available for ABR mode:\n" +
                "phone => 16kbps/mono        phon+/lw/mw-eu/sw => 24kbps/mono\n" +
                "mw-us => 40kbps/mono        voice => 56kbps/mono\n" + "fm/radio/tape => 112kbps    hifi => 160kbps\n" +
                "cd => 192kbps               studio => 256kbps\n");
        }

        private int presets_set(LameGlobalFlags gfp, int fast, int cbr, string preset_name, string ProgramName)
        {
            var mono = 0;
            if (preset_name.Equals("help") && fast < 1 && cbr < 1)
            {
                lame_version_print(Console.Out);
                presets_longinfo_dm(Console.Out);
                return -1;
            }

            if (preset_name.Equals("phone"))
            {
                preset_name = "16";
                mono = 1;
            }

            if (preset_name.Equals("phon+") || preset_name.Equals("lw") || preset_name.Equals("mw-eu") ||
                preset_name.Equals("sw"))
            {
                preset_name = "24";
                mono = 1;
            }

            if (preset_name.Equals("mw-us"))
            {
                preset_name = "40";
                mono = 1;
            }

            if (preset_name.Equals("voice"))
            {
                preset_name = "56";
                mono = 1;
            }

            if (preset_name.Equals("fm"))
                preset_name = "112";

            if (preset_name.Equals("radio") || preset_name.Equals("tape"))
                preset_name = "112";

            if (preset_name.Equals("hifi"))
                preset_name = "160";

            if (preset_name.Equals("cd"))
                preset_name = "192";

            if (preset_name.Equals("studio"))
                preset_name = "256";

            if (preset_name.Equals("medium"))
            {
                pre.lame_set_VBR_q(gfp, 4);
                if (fast > 0)
                    gfp.VBR = VbrMode.vbr_mtrh;
                else
                    gfp.VBR = VbrMode.vbr_rh;

                return 0;
            }

            if (preset_name.Equals("standard"))
            {
                pre.lame_set_VBR_q(gfp, 2);
                if (fast > 0)
                    gfp.VBR = VbrMode.vbr_mtrh;
                else
                    gfp.VBR = VbrMode.vbr_rh;

                return 0;
            }

            if (preset_name.Equals("extreme"))
            {
                pre.lame_set_VBR_q(gfp, 0);
                if (fast > 0)
                    gfp.VBR = VbrMode.vbr_mtrh;
                else
                    gfp.VBR = VbrMode.vbr_rh;

                return 0;
            }

            if (preset_name.Equals("insane") && fast < 1)
            {
                gfp.preset = Lame.INSANE;
                pre.apply_preset(gfp, Lame.INSANE, 1);
                return 0;
            }

            if (Convert.ToInt32(preset_name) > 0 && fast < 1)
                if (Convert.ToInt32(preset_name) >= 8 && Convert.ToInt32(preset_name) <= 320)
                {
                    gfp.preset = Convert.ToInt32(preset_name);
                    pre.apply_preset(gfp, Convert.ToInt32(preset_name), 1);
                    if (cbr == 1)
                        gfp.VBR = VbrMode.vbr_off;

                    if (mono == 1)
                        gfp.mode = MPEGMode.MONO;

                    return 0;
                }
                else
                {
                    lame_version_print(Console.Error);
                    Console.Write(
                        "Error: The bitrate specified is out of the valid range for this preset\n" + "\n" +
                        "When using this mode you must enter a value between \"32\" and \"320\"\n" + "\n" +
                        "For further information try: \"%s --preset help\"\n",
                        ProgramName);
                    return -1;
                }

            lame_version_print(Console.Error);
            Console.Write(
                "Error: You did not enter a valid profile and/or options with --preset\n" + "\n" +
                "Available profiles are:\n" + "\n" + "   <fast>        medium\n" + "   <fast>        standard\n" +
                "   <fast>        extreme\n" + "                 insane\n" +
                "          <cbr> (ABR Mode) - The ABR Mode is implied. To use it,\n" +
                "                             simply specify a bitrate. For example:\n" +
                "                             \"--preset 185\" activates this\n" +
                "                             preset and uses 185 as an average kbps.\n" + "\n");
            Console.Write(
                "    Some examples:\n" + "\n" + " or \"%s --preset fast standard <input file> <output file>\"\n" +
                " or \"%s --preset cbr 192 <input file> <output file>\"\n" +
                " or \"%s --preset 172 <input file> <output file>\"\n" +
                " or \"%s --preset extreme <input file> <output file>\"\n" + "\n" +
                "For further information try: \"%s --preset help\"\n",
                ProgramName,
                ProgramName,
                ProgramName,
                ProgramName,
                ProgramName);
            return -1;
        }

        private GetAudio.sound_file_format filename_to_type(string FileName)
        {
            var len = FileName.Length;
            if (len < 4)
                return GetAudio.sound_file_format.sf_unknown;

            FileName = FileName.Substring(len - 4);
            if (FileName.Equals(".mpg", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_mp123;

            if (FileName.Equals(".mp1", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_mp123;

            if (FileName.Equals(".mp2", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_mp123;

            if (FileName.Equals(".mp3", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_mp123;

            if (FileName.Equals(".wav", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_wave;

            if (FileName.Equals(".aif", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_aiff;

            if (FileName.Equals(".raw", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_raw;

            if (FileName.Equals(".ogg", StringComparison.CurrentCultureIgnoreCase))
                return GetAudio.sound_file_format.sf_ogg;

            return GetAudio.sound_file_format.sf_unknown;
        }

        private int resample_rate(double freq)
        {
            if (freq >= 1000)
                freq = freq * 0.001;

            switch ((int)freq)
            {
                case 8:
                    return 8000;
                case 11:
                    return 11025;
                case 12:
                    return 12000;
                case 16:
                    return 16000;
                case 22:
                    return 22050;
                case 24:
                    return 24000;
                case 32:
                    return 32000;
                case 44:
                    return 44100;
                case 48:
                    return 48000;
                default:
                    Console.Write("Illegal resample frequency: %.3f kHz\n", freq);
                    return 0;
            }
        }

        private int set_id3_albumart(LameGlobalFlags gfp, string file_name)
        {
            var ret = -1;
            FileStream fpi = null;
            if (ReferenceEquals(file_name, null))
                return 0;

            try
            {
                fpi = File.OpenRead(file_name);
                try
                {
                    var size = (int)(fpi.Length & int.MaxValue);
                    var albumart = new byte[size];
                    fpi.Read(albumart, 0, size);

                    ret = id3.id3tag_set_albumart(gfp, albumart, size) ? 0 : 4;
                }
                catch (IOException)
                {
                    ret = 3;
                }
                finally
                {
                    try
                    {
                        fpi.Close();
                    }
                    catch (IOException e)
                    {
                        Console.Write(e.ToString());
                        Console.Write(e.StackTrace);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                ret = 1;
            }

            switch (ret)
            {
                case 1:
                    Console.Write("Could not find: '%s'.\n", file_name);
                    break;
                case 2:
                    Console.Write("Insufficient memory for reading the albumart.\n");
                    break;
                case 3:
                    Console.Write("Read error: '%s'.\n", file_name);
                    break;
                case 4:
                    Console.Write("Unsupported image: '%s'.\nSpecify JPEG/PNG/GIF image (128KB maximum)\n", file_name);
                    break;
                default:
                    break;
            }

            return ret;
        }

        internal virtual int parse_args(
            LameGlobalFlags gfp,
            List<string> argv,
            StringBuilder inPath,
            StringBuilder outPath,
            string[] nogap_inPath,
            NoGap ng)
        {
            var input_file = 0;
            var autoconvert = 0;
            double val;
            var nogap = 0;
            var nogap_tags = 0;
            const string ProgramName = "lame";
            var count_nogap = 0;
            var noreplaygain = 0;
            var id3tag_mode = ID3TAG_MODE.ID3TAG_MODE_DEFAULT;
            inPath.Length = 0;
            outPath.Length = 0;
            silent = 0;
            ignore_tag_errors = false;
            brhist = true;
            mp3_delay = 0;
            mp3_delay_set = false;
            print_clipping_info = false;
            disable_wav_header = false;
            id3.id3tag_init(gfp);
            for (var i = 0; i < argv.Count; i++)
            {
                char c;
                string token;
                var tokenPos = 0;
                string arg;
                string nextArg;
                int argUsed;
                token = argv[i];
                if (token[tokenPos++] == '-')
                {
                    argUsed = 0;
                    nextArg = i + 1 < argv.Count ? argv[i + 1] : "";
                    if (token.Length - tokenPos == 0)
                    {
                        input_file = 1;
                        if (inPath.Length == 0)
                        {
                            inPath.Length = 0;
                            inPath.Append(argv[i]);
                        }
                        else if (outPath.Length == 0)
                        {
                            outPath.Length = 0;
                            outPath.Append(argv[i]);
                        }
                    }

                    if (token[tokenPos] == '-')
                    {
                        tokenPos++;
                        if (token.Substring(tokenPos).Equals("resample", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.out_samplerate = resample_rate(double.Parse(nextArg));
                        }
                        else if (token.Substring(tokenPos).Equals("vbr-old", StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.VBR = VbrMode.vbr_rh;
                        }
                        else if (token.Substring(tokenPos).Equals("vbr-new", StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.VBR = VbrMode.vbr_mtrh;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "vbr-mtrh",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.VBR = VbrMode.vbr_mtrh;
                        }
                        else if (token.Substring(tokenPos).Equals("cbr", StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.VBR = VbrMode.vbr_off;
                        }
                        else if (token.Substring(tokenPos).Equals("abr", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.VBR = VbrMode.vbr_abr;
                            gfp.VBR_mean_bitrate_kbps = Convert.ToInt32(nextArg);
                            if (gfp.VBR_mean_bitrate_kbps >= 8000)
                                gfp.VBR_mean_bitrate_kbps = (gfp.VBR_mean_bitrate_kbps + 500) / 1000;

                            gfp.VBR_mean_bitrate_kbps = Math.Min(gfp.VBR_mean_bitrate_kbps, 320);
                            gfp.VBR_mean_bitrate_kbps = Math.Max(gfp.VBR_mean_bitrate_kbps, 8);
                        }
                        else if (token.Substring(tokenPos).Equals("r3mix", StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.preset = Lame.R3MIX;
                            pre.apply_preset(gfp, Lame.R3MIX, 1);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "bitwidth",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            in_bitwidth = Convert.ToInt32(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("signed", StringComparison.CurrentCultureIgnoreCase))
                        {
                            in_signed = true;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "unsigned",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            in_signed = false;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "little-endian",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            in_endian = ByteOrder.LITTLE_ENDIAN;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "big-endian",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            in_endian = ByteOrder.BIG_ENDIAN;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "mp1input",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            input_format = GetAudio.sound_file_format.sf_mp1;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "mp2input",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            input_format = GetAudio.sound_file_format.sf_mp2;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "mp3input",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            input_format = GetAudio.sound_file_format.sf_mp3;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "ogginput",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            Console.Write("sorry, vorbis support in LAME is deprecated.\n");
                            return -1;
                        }
                        else if (token.Substring(tokenPos).Equals("phone", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (presets_set(gfp, 0, 0, token, ProgramName) < 0)
                                return -1;

                            Console.Write("Warning: --phone is deprecated, use --preset phone instead!");
                        }
                        else if (token.Substring(tokenPos).Equals("voice", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (presets_set(gfp, 0, 0, token, ProgramName) < 0)
                                return -1;

                            Console.Write("Warning: --voice is deprecated, use --preset voice instead!");
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "noshort",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.short_blocks = ShortBlock.short_block_dispensed;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "short",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.short_blocks = ShortBlock.short_block_allowed;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "allshort",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.short_blocks = ShortBlock.short_block_forced;
                        }
                        else if (token.Substring(tokenPos).Equals("decode", StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.decode_only = true;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "decode-mp3delay",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            mp3_delay = Convert.ToInt32(nextArg);
                            mp3_delay_set = true;
                            argUsed = 1;
                        }
                        else if (token.Substring(tokenPos).Equals("nores", StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.disable_reservoir = true;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "strictly-enforce-ISO",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.strict_ISO = true;
                        }
                        else if (token.Substring(tokenPos).Equals("scale", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.scale = (float)double.Parse(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("scale-l", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.scale_left = (float)double.Parse(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("scale-r", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.scale_right = (float)double.Parse(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "freeformat",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.free_format = true;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "replaygain-fast",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.findReplayGain = true;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "replaygain-accurate",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.decode_on_the_fly = true;
                            gfp.findReplayGain = true;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "noreplaygain",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            noreplaygain = 1;
                            gfp.findReplayGain = false;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "clipdetect",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            print_clipping_info = true;
                            gfp.decode_on_the_fly = true;
                        }
                        else if (token.Substring(tokenPos).Equals("nohist", StringComparison.CurrentCultureIgnoreCase))
                        {
                            brhist = false;
                        }
                        else if (token.Substring(tokenPos).Equals("tt", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            id3_tag(gfp, 't', TextEncoding.TENC_RAW, nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("ta", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            id3_tag(gfp, 'a', TextEncoding.TENC_RAW, nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("tl", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            id3_tag(gfp, 'l', TextEncoding.TENC_RAW, nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("ty", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            id3_tag(gfp, 'y', TextEncoding.TENC_RAW, nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("tc", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            id3_tag(gfp, 'c', TextEncoding.TENC_RAW, nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("tn", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var ret = id3_tag(gfp, 'n', TextEncoding.TENC_RAW, nextArg);
                            argUsed = 1;
                            if (ret)
                                if (!ignore_tag_errors)
                                    if (id3tag_mode == ID3TAG_MODE.ID3TAG_MODE_V1_ONLY)
                                    {
                                        Console.Write("The track number has to be between 1 and 255 for ID3v1.\n");
                                        return -1;
                                    }
                                    else if (id3tag_mode == ID3TAG_MODE.ID3TAG_MODE_V2_ONLY)
                                    {
                                    }
                                    else
                                    {
                                        if (silent < 10)
                                            Console.Write(
                                                "The track number has to be between 1 and 255 for ID3v1, ignored for ID3v1.\n");
                                    }
                        }
                        else if (token.Substring(tokenPos).Equals("tg", StringComparison.CurrentCultureIgnoreCase))
                        {
                            id3_tag(gfp, 'g', TextEncoding.TENC_RAW, nextArg);
                            argUsed = 1;
                        }
                        else if (token.Substring(tokenPos).Equals("tv", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            if (id3_tag(gfp, 'v', TextEncoding.TENC_RAW, nextArg))
                                if (silent < 10)
                                    Console.Write("Invalid field value: '%s'. Ignored\n", nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("ti", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            if (set_id3_albumart(gfp, nextArg) != 0)
                                if (!ignore_tag_errors)
                                    return -1;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "ignore-tag-errors",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            ignore_tag_errors = true;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "add-id3v2",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            id3.id3tag_add_v2(gfp);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "id3v1-only",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            id3.id3tag_v1_only(gfp);
                            id3tag_mode = ID3TAG_MODE.ID3TAG_MODE_V1_ONLY;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "id3v2-only",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            id3.id3tag_v2_only(gfp);
                            id3tag_mode = ID3TAG_MODE.ID3TAG_MODE_V2_ONLY;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "space-id3v1",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            id3.id3tag_space_v1(gfp);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "pad-id3v2",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            id3.id3tag_pad_v2(gfp);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "pad-id3v2-size",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            var n = Convert.ToInt32(nextArg);
                            n = n <= 128000 ? n : 128000;
                            n = n >= 0 ? n : 0;
                            id3.id3tag_set_pad(gfp, n);
                            argUsed = 1;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "genre-list",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            id3.id3tag_genre_list(new GenreListHandlerAnonymousInnerClass(this));
                            return -2;
                        }
                        else if (token.Substring(tokenPos).Equals("lowpass", StringComparison.CurrentCultureIgnoreCase))
                        {
                            val = double.Parse(nextArg);
                            argUsed = 1;
                            if (val < 0)
                            {
                                gfp.lowpassfreq = -1;
                            }
                            else
                            {
                                if (val < 0.001 || val > 50000.0)
                                {
                                    Console.Write("Must specify lowpass with --lowpass freq, freq >= 0.001 kHz\n");
                                    return -1;
                                }

                                gfp.lowpassfreq = (int)(val * (val < 50.0 ? 1000 : 1) + 0.5);
                            }
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "lowpass-width",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            val = double.Parse(nextArg);
                            argUsed = 1;
                            if (val < 0.001 || val > 50000.0)
                            {
                                Console.Write(
                                    "Must specify lowpass width with --lowpass-width freq, freq >= 0.001 kHz\n");
                                return -1;
                            }

                            gfp.lowpassfreq = (int)(val * (val < 16.0 ? 1000 : 1) + 0.5);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "highpass",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            val = double.Parse(nextArg);
                            argUsed = 1;
                            if (val < 0.0)
                            {
                                gfp.highpassfreq = -1;
                            }
                            else
                            {
                                if (val < 0.001 || val > 50000.0)
                                {
                                    Console.Write("Must specify highpass with --highpass freq, freq >= 0.001 kHz\n");
                                    return -1;
                                }

                                gfp.highpassfreq = (int)(val * (val < 16.0 ? 1000 : 1) + 0.5);
                            }
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "highpass-width",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            val = double.Parse(nextArg);
                            argUsed = 1;
                            if (val < 0.001 || val > 50000.0)
                            {
                                Console.Write(
                                    "Must specify highpass width with --highpass-width freq, freq >= 0.001 kHz\n");
                                return -1;
                            }

                            gfp.highpasswidth = (int)val;
                        }
                        else if (token.Substring(tokenPos).Equals("comp", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            val = double.Parse(nextArg);
                            if (val < 1.0)
                            {
                                Console.Write("Must specify compression ratio >= 1.0\n");
                                return -1;
                            }

                            gfp.compression_ratio = (float)val;
                        }
                        else if (token.Substring(tokenPos).Equals("notemp", StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.useTemporal = false;
                        }
                        else if (token.Substring(tokenPos).Equals("interch", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.interChRatio = (float)double.Parse(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "temporal-masking",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.useTemporal = Convert.ToInt32(nextArg) != 0;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "nssafejoint",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.exp_nspsytune = gfp.exp_nspsytune | 2;
                        }
                        else if (token.Substring(tokenPos).Equals("nsmsfix", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.msfix = (float)double.Parse(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals("ns-bass", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            {
                                double d;
                                int k;
                                d = double.Parse(nextArg);
                                k = (int)(d * 4);
                                if (k < -32)
                                    k = -32;

                                if (k > 31)
                                    k = 31;

                                if (k < 0)
                                    k += 64;

                                gfp.exp_nspsytune = gfp.exp_nspsytune | (k << 2);
                            }
                        }
                        else if (token.Substring(tokenPos).Equals("ns-alto", StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            {
                                double d;
                                int k;
                                d = double.Parse(nextArg);
                                k = (int)(d * 4);
                                if (k < -32)
                                    k = -32;

                                if (k > 31)
                                    k = 31;

                                if (k < 0)
                                    k += 64;

                                gfp.exp_nspsytune = gfp.exp_nspsytune | (k << 8);
                            }
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "ns-treble",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            {
                                double d;
                                int k;
                                d = double.Parse(nextArg);
                                k = (int)(d * 4);
                                if (k < -32)
                                    k = -32;

                                if (k > 31)
                                    k = 31;

                                if (k < 0)
                                    k += 64;

                                gfp.exp_nspsytune = gfp.exp_nspsytune | (k << 14);
                            }
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "ns-sfb21",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            {
                                double d;
                                int k;
                                d = double.Parse(nextArg);
                                k = (int)(d * 4);
                                if (k < -32)
                                    k = -32;

                                if (k > 31)
                                    k = 31;

                                if (k < 0)
                                    k += 64;

                                gfp.exp_nspsytune = gfp.exp_nspsytune | (k << 20);
                            }
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "nspsytune2",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                        }
                        else if (token.Substring(tokenPos).Equals("quiet", StringComparison.CurrentCultureIgnoreCase) ||
                                 token.Substring(tokenPos).Equals("silent", StringComparison.CurrentCultureIgnoreCase))
                        {
                            silent = 10;
                        }
                        else if (token.Substring(tokenPos).Equals("brief", StringComparison.CurrentCultureIgnoreCase))
                        {
                            silent = -5;
                        }
                        else if (token.Substring(tokenPos).Equals("verbose", StringComparison.CurrentCultureIgnoreCase))
                        {
                            silent = -10;
                        }
                        else if (token.Substring(tokenPos).Equals(
                                     "version",
                                     StringComparison.CurrentCultureIgnoreCase) || token
                                                                                   .Substring(tokenPos).Equals(
                                                                                       "license",
                                                                                       StringComparison
                                                                                           .CurrentCultureIgnoreCase)
                        )
                        {
                            print_license(Console.Out);
                            return -2;
                        }
                        else if (token.Substring(tokenPos).Equals("help", StringComparison.CurrentCultureIgnoreCase) ||
                                 token.Substring(tokenPos).Equals("usage", StringComparison.CurrentCultureIgnoreCase))
                        {
                            short_help(gfp, Console.Out, ProgramName);
                            return -2;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "longhelp",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            long_help(gfp, Console.Out, ProgramName, 0);
                            return -2;
                        }
                        else if (token.Substring(tokenPos).Equals("?", StringComparison.CurrentCultureIgnoreCase))
                        {
                            long_help(gfp, Console.Out, ProgramName, 1);
                            return -2;
                        }
                        else if (token.Substring(tokenPos).Equals(
                                     "preset",
                                     StringComparison.CurrentCultureIgnoreCase) || token
                                                                                   .Substring(tokenPos).Equals(
                                                                                       "alt-preset",
                                                                                       StringComparison
                                                                                           .CurrentCultureIgnoreCase)
                        )
                        {
                            argUsed = 1;
                            {
                                int fast = 0, cbr = 0;
                                while (nextArg.Equals("fast") || nextArg.Equals("cbr"))
                                {
                                    if (nextArg.Equals("fast") && fast < 1)
                                        fast = 1;

                                    if (nextArg.Equals("cbr") && cbr < 1)
                                        cbr = 1;

                                    argUsed++;
                                    nextArg = i + argUsed < argv.Count ? argv[i + argUsed] : "";
                                }

                                if (presets_set(gfp, fast, cbr, nextArg, ProgramName) < 0)
                                    return -1;
                            }
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "disptime",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            update_interval = (float)double.Parse(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "nogaptags",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            nogap_tags = 1;
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "nogapout",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            outPath.Length = 0;
                            outPath.Append(nextArg);
                            argUsed = 1;
                        }
                        else if (token.Substring(tokenPos).Equals("nogap", StringComparison.CurrentCultureIgnoreCase))
                        {
                            nogap = 1;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "tune",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            {
                                gfp.tune_value_a = (float)double.Parse(nextArg);
                                gfp.tune = true;
                            }
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "shortthreshold",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            {
                                float x, y;

                                var sc = nextArg.Split(' ');
                                float.TryParse(sc[0], out x);

                                // Scanner sc = new Scanner(nextArg);
                                // x = sc.nextFloat();
                                //if (!sc.hasNext())
                                if (sc.Length == 1)
                                    y = x;
                                else
                                    float.TryParse(sc[1], out y);

                                argUsed = 1;
                                gfp.internal_flags.nsPsy.attackthre = x;
                                gfp.internal_flags.nsPsy.attackthre_s = y;
                            }
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "maskingadjust",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.maskingadjust = (float)double.Parse(nextArg);
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "maskingadjustshort",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.maskingadjust_short = (float)double.Parse(nextArg);
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "athcurve",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.ATHcurve = (float)double.Parse(nextArg);
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "no-preset-tune",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "substep",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.internal_flags.substep_shaping = Convert.ToInt32(nextArg);
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "sbgain",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.internal_flags.subblock_gain = Convert.ToInt32(nextArg);
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "sfscale",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.internal_flags.noise_shaping = 2;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "noath",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.noATH = true;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "athonly",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.ATHonly = true;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "athshort",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            gfp.ATHshort = true;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "athlower",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.ATHlower = -(float)double.Parse(nextArg) / 10.0f;
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "athtype",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.ATHtype = Convert.ToInt32(nextArg);
                        }
                        else if (INTERNAL_OPTS && token.Substring(tokenPos).Equals(
                                     "athaa-type",
                                     StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.athaa_type = Convert.ToInt32(nextArg);
                        }
                        else if (token.Substring(tokenPos).Equals(
                            "athaa-sensitivity",
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            argUsed = 1;
                            gfp.athaa_sensitivity = (float)double.Parse(nextArg);
                        }
                        else
                        {
                            {
                                Console.Write("%s: unrecognized option --%s\n", ProgramName, token);
                                return -1;
                            }
                        }

                        i += argUsed;
                    }
                    else
                    {
                        while (tokenPos < token.Length)
                        {
                            c = token[tokenPos++];
                            arg = tokenPos < token.Length ? token : nextArg;
                            switch (c)
                            {
                                case 'm':
                                    argUsed = 1;
                                    switch (arg[0])
                                    {
                                        case 's':
                                            gfp.mode = MPEGMode.STEREO;
                                            break;
                                        case 'd':
                                            gfp.mode = MPEGMode.DUAL_CHANNEL;
                                            break;
                                        case 'f':
                                            gfp.force_ms = true;
                                            goto case 'j';
                                        case 'j':
                                            gfp.mode = MPEGMode.JOINT_STEREO;
                                            break;
                                        case 'm':
                                            gfp.mode = MPEGMode.MONO;
                                            break;
                                        case 'a':
                                            gfp.mode = MPEGMode.JOINT_STEREO;
                                            break;
                                        default:
                                            Console.Write("%s: -m mode must be s/d/j/f/m not %s\n", ProgramName, arg);
                                            return -1;
                                    }

                                    break;
                                case 'V':
                                    argUsed = 1;
                                    if (gfp.VBR == VbrMode.vbr_off)
                                    {
                                        gfp.VBR_q = (int)VbrMode.vbr_default;
                                        gfp.VBR_q_frac = 0;
                                    }

                                    gfp.VBR_q = (int)(float)double.Parse(arg);
                                    gfp.VBR_q_frac = (float)double.Parse(arg) - gfp.VBR_q;
                                    break;
                                case 'v':
                                    if (gfp.VBR == VbrMode.vbr_off)
                                        gfp.VBR = VbrMode.vbr_mtrh;

                                    break;
                                case 'q':
                                    argUsed = 1;
                                {
                                    var tmp_quality = Convert.ToInt32(arg);
                                    if (tmp_quality < 0)
                                        tmp_quality = 0;

                                    if (tmp_quality > 9)
                                        tmp_quality = 9;

                                    gfp.quality = tmp_quality;
                                }
                                    break;
                                case 'f':
                                    gfp.quality = 7;
                                    break;
                                case 'h':
                                    gfp.quality = 2;
                                    break;
                                case 's':
                                    argUsed = 1;
                                    val = double.Parse(arg);
                                    gfp.in_samplerate = (int)(val * (val <= 192 ? 1000 : 1) + 0.5);
                                    break;
                                case 'b':
                                    argUsed = 1;
                                    gfp.brate = Convert.ToInt32(arg);
                                    if (gfp.brate > 320)
                                        gfp.disable_reservoir = true;

                                    gfp.VBR_min_bitrate_kbps = gfp.brate;
                                    break;
                                case 'B':
                                    argUsed = 1;
                                    gfp.VBR_max_bitrate_kbps = Convert.ToInt32(arg);
                                    break;
                                case 'F':
                                    gfp.VBR_hard_min = 1;
                                    break;
                                case 't':
                                    gfp.bWriteVbrTag = false;
                                    disable_wav_header = true;
                                    break;
                                case 'T':
                                    gfp.bWriteVbrTag = true;
                                    nogap_tags = 1;
                                    disable_wav_header = false;
                                    break;
                                case 'r':
                                    input_format = GetAudio.sound_file_format.sf_raw;
                                    break;
                                case 'x':
                                    swapbytes = true;
                                    break;
                                case 'p':
                                    gfp.error_protection = true;
                                    break;
                                case 'a':
                                    autoconvert = 1;
                                    gfp.mode = MPEGMode.MONO;
                                    break;
                                case 'S':
                                    silent = 10;
                                    break;
                                case 'X':
                                {
                                    int x, y;
                                    var sc = nextArg.Split(' ');
                                    int.TryParse(sc[0], out x);

                                    // Scanner sc = new Scanner(arg);
                                    // x = sc.nextFloat();
                                    //if (!sc.hasNext())
                                    if (sc.Length == 1)
                                        y = x;
                                    else
                                        int.TryParse(sc[1], out y);

                                    argUsed = 1;
                                    if (INTERNAL_OPTS)
                                    {
                                        gfp.quant_comp = x;
                                        gfp.quant_comp_short = y;
                                    }
                                }
                                    break;
                                case 'Y':
                                    gfp.experimentalY = true;
                                    break;
                                case 'Z':
                                {
                                    var n = 1;
                                    var sc = nextArg.Split(' ');

                                    // Scanner sc = new Scanner(arg);
                                    int.TryParse(sc[0], out n);
                                    if (INTERNAL_OPTS)
                                        gfp.experimentalZ = n;
                                }
                                    break;
                                case 'e':
                                    argUsed = 1;
                                    switch (arg[0])
                                    {
                                        case 'n':
                                            gfp.emphasis = 0;
                                            break;
                                        case '5':
                                            gfp.emphasis = 1;
                                            break;
                                        case 'c':
                                            gfp.emphasis = 3;
                                            break;
                                        default:
                                            Console.Write("%s: -e emp must be n/5/c not %s\n", ProgramName, arg);
                                            return -1;
                                    }

                                    break;
                                case 'c':
                                    gfp.copyright = 1;
                                    break;
                                case 'o':
                                    gfp.original = 0;
                                    break;
                                case '?':
                                    long_help(gfp, Console.Out, ProgramName, 0);
                                    return -1;
                                default:
                                    Console.Write("%s: unrecognized option -%c\n", ProgramName, c);
                                    return -1;
                            }

                            if (argUsed != 0)
                            {
                                if (ReferenceEquals(arg, token))
                                    token = "";
                                else
                                    ++i;

                                arg = "";
                                argUsed = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (nogap != 0)
                    {
                        if (ng != null && count_nogap < ng.num_nogap)
                        {
                            nogap_inPath[count_nogap++] = argv[i];
                            input_file = 1;
                        }
                        else
                        {
                            Console.Write(
                                "Error: 'nogap option'.  Calling program does not allow nogap option, or\n" +
                                "you have exceeded maximum number of input files for the nogap option\n");
                            ng.num_nogap = -1;
                            return -1;
                        }
                    }
                    else
                    {
                        if (inPath.Length == 0)
                        {
                            inPath.Length = 0;
                            inPath.Append(argv[i]);
                            input_file = 1;
                        }
                        else
                        {
                            if (outPath.Length == 0)
                            {
                                outPath.Length = 0;
                                outPath.Append(argv[i]);
                            }
                            else
                            {
                                Console.Write("%s: excess arg %s\n", ProgramName, argv[i]);
                                return -1;
                            }
                        }
                    }
                }
            }

            if (0 == input_file)
            {
                usage(Console.Out, ProgramName);
                return -1;
            }

            if (inPath.ToString()[0] == '-')
                silent = silent <= 1 ? 1 : silent;

            if (outPath.Length == 0 && count_nogap == 0)
            {
                outPath.Length = 0;
                outPath.Append(inPath.ToString().Substring(0, inPath.Length - 4));
                if (gfp.decode_only)
                    outPath.Append(".mp3.wav");
                else
                    outPath.Append(".mp3");
            }

            if (0 == noreplaygain)
                gfp.findReplayGain = true;

            if (nogap != 0 && gfp.bWriteVbrTag && nogap_tags == 0)
            {
                Console.Write("Note: Disabling VBR Xing/Info tag since it interferes with --nogap\n");
                gfp.bWriteVbrTag = false;
            }

            if (outPath.ToString()[0] == '-')
                gfp.bWriteVbrTag = false;

            if (input_format == GetAudio.sound_file_format.sf_unknown)
                input_format = filename_to_type(inPath.ToString());

            if (input_format == GetAudio.sound_file_format.sf_ogg)
            {
                Console.Write("sorry, vorbis support in LAME is deprecated.\n");
                return -1;
            }

            if (autoconvert != 0)
                gfp.num_channels = 2;
            else if (MPEGMode.MONO == gfp.mode)
                gfp.num_channels = 1;
            else
                gfp.num_channels = 2;

            if (gfp.free_format)
                if (gfp.brate < 8 || gfp.brate > 640)
                {
                    Console.Write("For free format, specify a bitrate between 8 and 640 kbps\n");
                    Console.Write("with the -b <bitrate> option\n");
                    return -1;
                }

            if (ng != null)
                ng.num_nogap = count_nogap;

            return 0;
        }
    }
}