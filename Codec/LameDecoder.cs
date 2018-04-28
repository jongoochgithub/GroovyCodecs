using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GroovyMp3.Types;
using GroovyMp3.Codec.Mp3;
using GroovyMp3.Codec.Mpg;
using GroovyMp3.Types;

namespace GroovyMp3.Codec
{

    public class LameDecoder : ILameDecoder
    {

        private readonly BitStream bs;

        private readonly short[][] buffer = Arrays.ReturnRectangularArray<short>(2, 1152);

        private readonly Mpg.Common common;

        private readonly GainAnalysis ga;

        private readonly GetAudio gaud;

        // private DataOutput outf;
        private readonly LameGlobalFlags gfp;

        private readonly ID3Tag id3;

        private readonly Interface intf;

        private readonly Lame lame;

        private readonly MPGLib mpg;

        private readonly Presets p;

        private readonly Parse parse;

        private readonly Quantize qu;

        private readonly QuantizePVT qupvt;

        private readonly Reservoir rv;

        private readonly Takehiro tak;

        private readonly VBRTag vbr;

        private readonly Mp3Version ver;

        private int wavsize;

        public LameDecoder(string mp3File)
        {
            // encoder modules
            lame = new Lame();
            gaud = new GetAudio();
            ga = new GainAnalysis();
            bs = new BitStream();
            p = new Presets();
            qupvt = new QuantizePVT();
            qu = new Quantize();
            vbr = new VBRTag();
            ver = new Mp3Version();
            id3 = new ID3Tag();
            rv = new Reservoir();
            tak = new Takehiro();
            parse = new Parse();

            mpg = new MPGLib();
            intf = new Interface();
            common = new Mpg.Common();

            lame.setModules(ga, bs, p, qupvt, qu, vbr, ver, id3, mpg);
            bs.setModules(ga, mpg, ver, vbr);
            id3.setModules(bs, ver);
            p.Modules = lame;
            qu.setModules(bs, rv, qupvt, tak);
            qupvt.setModules(tak, rv, lame.enc.psy);
            rv.Modules = bs;
            tak.Modules = qupvt;
            vbr.setModules(lame, bs, ver);
            gaud.setModules(parse, mpg);
            parse.setModules(ver, id3, p);

            // decoder modules
            mpg.setModules(intf, common);
            intf.setModules(vbr, common);

            gfp = lame.lame_init();

            /*
             * turn off automatic writing of ID3 tag data into mp3 stream we have to
             * call it before 'lame_init_params', because that function would spit
             * out ID3v2 tag data.
             */
            gfp.write_id3tag_automatic = false;

            /*
             * Now that all the options are set, lame needs to analyze them and set
             * some more internal options and check for problems
             */
            lame.lame_init_params(gfp);

            parse.input_format = GetAudio.sound_file_format.sf_mp3;

            var inPath = new StringBuilder(mp3File);
            var enc = new Enc();

            gaud.init_infile(gfp, inPath.ToString(), enc);

            var skip_start = 0;
            var skip_end = 0;

            if (parse.silent < 10)
                Console.Write(
                    "\rinput:  {0}{1}({2:g} kHz, {3:D} channel{4}, ",
                    inPath,
                    inPath.Length > 26 ? "\n\t" : "  ",
                    gfp.in_samplerate / 1000,
                    gfp.num_channels,
                    gfp.num_channels != 1 ? "s" : "");

            if (enc.enc_delay > -1 || enc.enc_padding > -1)
            {
                if (enc.enc_delay > -1)
                    skip_start = enc.enc_delay + 528 + 1;

                if (enc.enc_padding > -1)
                    skip_end = enc.enc_padding - (528 + 1);
            }
            else
            {
                skip_start = gfp.encoder_delay + 528 + 1;
            }

            Console.Write("MPEG-{0:D}{1} Layer {2}", 2 - gfp.version, gfp.out_samplerate < 16000 ? ".5" : "", "III");

            Console.Write(")\noutput: (16 bit, Microsoft WAVE)\n");

            if (skip_start > 0)
                Console.Write("skipping initial {0:D} samples (encoder+decoder delay)\n", skip_start);

            if (skip_end > 0)
                Console.Write("skipping final {0:D} samples (encoder padding-decoder delay)\n", skip_end);

            wavsize = -(skip_start + skip_end);
            parse.mp3input_data.totalframes = parse.mp3input_data.nsamp / parse.mp3input_data.framesize;

            Debug.Assert(gfp.num_channels >= 1 && gfp.num_channels <= 2);
        }

        public virtual void decode(MemoryStream sampleBuffer, bool playOriginal)
        {
            var iread = gaud.get_audio16(gfp, buffer);
            if (iread >= 0)
            {
                parse.mp3input_data.framenum += iread / parse.mp3input_data.framesize;
                wavsize += iread;

                for (var i = 0; i < iread; i++)
                {
                    if (playOriginal)
                    {
                        // We put mp3 data into the sample buffer here!
                        sampleBuffer.WriteByte(unchecked((byte)(buffer[0][i] & 0xff)));
                        sampleBuffer.WriteByte(unchecked((byte)(((buffer[0][i] & 0xffff) >> 8) & 0xff)));
                    }

                    if (gfp.num_channels == 2)
                    {
                        // gaud.write16BitsLowHigh(outf, buffer[1][i] & 0xffff);
                        // TODO two channels?
                    }
                }
            }

        }

        public virtual void close()
        {
            lame.lame_close(gfp);
        }
    }
}