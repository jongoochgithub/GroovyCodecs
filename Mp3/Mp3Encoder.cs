using System;
using System.Collections.Generic;
using GroovyCodecs.Mp3.Common;
using GroovyCodecs.Mp3.Mp3;
using GroovyCodecs.Mp3.Mpg;
using GroovyCodecs.Types;

namespace GroovyCodecs.Mp3
{
    /// <summary>
    ///     Wrapper for the jump3r encoder.
    ///     @author Ken Handel
    /// </summary>
    public class Mp3Encoder : IMp3Encoder
    {

        public const int BITRATE_AUTO = -1;

        // channel mode has no influence on mono files.
        public const int CHANNEL_MODE_AUTO = -1;

        public const int CHANNEL_MODE_DUAL_CHANNEL = 2;

        public const int CHANNEL_MODE_JOINT_STEREO = 1;

        public const int CHANNEL_MODE_MONO = 3;

        public const int CHANNEL_MODE_STEREO = 0;

        private static readonly int DEFAULT_QUALITY = QUALITY_MIDDLE;

        private static readonly int DEFAULT_BITRATE = BITRATE_AUTO;

        private static readonly int DEFAULT_CHANNEL_MODE = CHANNEL_MODE_AUTO;

        // suggested maximum buffer size for an mpeg frame
        private const int DEFAULT_PCM_BUFFER_SIZE = 2048 * 16;

        // frame size=576 for MPEG2 and MPEG2.5
        // =576*2 for MPEG1

        // in VBR mode, bitrate is ignored.
        private static readonly bool DEFAULT_VBR = false;

        public const int MPEG_VERSION_1 = 1; // MPEG-1

        // constants from lame.h
        public const int MPEG_VERSION_2 = 0; // MPEG-2

        public const int MPEG_VERSION_2DOT5 = 2; // MPEG-2.5

        public const int NOT_SPECIFIED = -1;

        /// <summary>
        ///     property key to read/set the bitrate: an Integer value. Set to -1 for
        ///     default bitrate.
        /// </summary>
        public const string P_BITRATE = "bitrate";

        /// <summary>
        ///     property key to read/set the channel mode: a String, one of
        ///     &quot;jointstereo&quot;, &quot;dual&quot;, &quot;mono&quot;,
        ///     &quot;auto&quot; (default).
        /// </summary>
        public const string P_CHMODE = "chmode";

        /// <summary>
        ///     property key to read/set the quality: an Integer from 1 (highest) to 9
        ///     (lowest).
        /// </summary>
        public const string P_QUALITY = "quality";

        /*
        public static readonly WaveFormatEncoding MPEG1L3 = new AudioFormat.Encoding("MPEG1L3");
		// Lame converts automagically to MPEG2 or MPEG2.5, if necessary.
		public static readonly WaveFormatEncoding MPEG2L3 = new AudioFormat.Encoding("MPEG2L3");
		public static readonly WaveFormatEncoding MPEG2DOT5L3 = new AudioFormat.Encoding("MPEG2DOT5L3");
        */

        // property constants
        /// <summary>
        ///     property key to read/set the VBR mode: an instance of Boolean (default:
        ///     false)
        /// </summary>
        public const string P_VBR = "vbr";

        public const int QUALITY_HIGH = 2;

        // quality==0 not yet coded in LAME (3.83alpha)
        // high mean bitrate in VBR // mode
        public const int QUALITY_HIGHEST = 1;

        public const int QUALITY_LOW = 7;

        // low mean bitrate in VBR mode
        public const int QUALITY_LOWEST = 9;

        public const int QUALITY_MIDDLE = 5;

        private int bitRate = DEFAULT_BITRATE;

        internal BitStream bs;

        private int chMode = DEFAULT_CHANNEL_MODE;

        internal Mpg.Common common;

        private int effBitRate;

        private int effChMode;

        private int effEncoding;

        // these fields are set upon successful initialization to show effective
        // values.
        private int effQuality;

        private int effSampleRate;

        private int effVbr;

        internal GainAnalysis ga;

        internal GetAudio gaud;

        private LameGlobalFlags gfp;

        internal ID3Tag id3;

        internal Interface intf;

        internal Lame lame;

        internal MPGLib mpg;

        internal Presets p;

        internal Parse parse;

        internal Quantize qu;

        private int quality = DEFAULT_QUALITY;

        internal QuantizePVT qupvt;

        internal Reservoir rv;

        private ByteOrder sourceByteOrder;

        private int sourceChannels;

        // encoding source values
        private AudioFormat sourceFormat;

        private bool sourceIsBigEndian;

        private int sourceSampleRate;

        private int sourceSampleSizeInBits;

        private bool sourceIsFloatingPoint;

        internal Takehiro tak;

        private AudioFormat targetFormat;

        internal Util util;

        internal VBRTag vbr;

        private bool vbrMode = DEFAULT_VBR;

        internal Mp3Version ver;

        public Mp3Encoder()
        {

        }

        /// <summary>
        ///     Initializes the encoder with the given source/PCM format. The default mp3
        ///     encoding parameters are used, see DEFAULT_BITRATE, DEFAULT_CHANNEL_MODE,
        ///     DEFAULT_QUALITY, and DEFAULT_VBR.
        /// </summary>
        /// <exception cref="IllegalArgumentException">
        ///     when parameters are not supported by LAME.
        /// </exception>
        public Mp3Encoder(AudioFormat sourceFormat)
        {
            readParams(sourceFormat, null);
            SetFormat(sourceFormat, null);
        }

        /// <summary>
        ///     Initializes the encoder with the given source/PCM format. The mp3
        ///     parameters are read from the targetFormat's properties. For any parameter
        ///     that is not set, global system properties are queried for backwards
        ///     tritonus compatibility. Last, parameters will use the default values
        ///     DEFAULT_BITRATE, DEFAULT_CHANNEL_MODE, DEFAULT_QUALITY, and DEFAULT_VBR.
        /// </summary>
        /// <exception cref="IllegalArgumentException">
        ///     when parameters are not supported by LAME.
        /// </exception>
        public Mp3Encoder(AudioFormat sourceFormat, AudioFormat targetFormat)
        {
            readParams(sourceFormat, targetFormat.Properties);
            SetFormat(sourceFormat, targetFormat);
        }

        /// <summary>
        ///     Initializes the encoder, overriding any parameters set in the audio
        ///     format's properties or in the system properties.
        /// </summary>
        /// <exception cref="IllegalArgumentException">
        ///     when parameters are not supported by LAME.
        /// </exception>
        public Mp3Encoder(AudioFormat sourceFormat, int bitRate, int channelMode, int quality, bool VBR)
        {
            this.bitRate = bitRate;
            chMode = channelMode;
            this.quality = quality;
            vbrMode = VBR;
            SetFormat(sourceFormat, null);
        }

        public virtual AudioFormat SourceFormat
        {
            set => SetFormat(value, null);
            get => sourceFormat;
        }

        public virtual AudioFormat TargetFormat
        {
            set => SetFormat(null, value);
            get => targetFormat;
        }

        public virtual void SetFormat(AudioFormat sourceFormat, AudioFormat targetFormat)
        {
            this.sourceFormat = sourceFormat;
            if (sourceFormat != null)
            {
                sourceSampleSizeInBits = sourceFormat.BitsPerSample;
                sourceByteOrder = sourceFormat.BigEndian
                    ? ByteOrder.BIG_ENDIAN
                    : ByteOrder.LITTLE_ENDIAN;
                sourceChannels = sourceFormat.Channels;
                sourceSampleRate = sourceFormat.SampleRate;
                sourceIsBigEndian = sourceFormat.BigEndian;
                sourceIsFloatingPoint = sourceFormat.IsFloatingPoint;

                // simple check that bitrate is not too high for MPEG2 and MPEG2.5
                // todo: exception ?
                if (sourceFormat.SampleRate < 32000 && bitRate > 160)
                    bitRate = 160;
            }

            //-1 means do not change the sample rate
            var targetSampleRate = -1;
            this.targetFormat = targetFormat;
            if (targetFormat != null)
                targetSampleRate = targetFormat.SampleRate;

            var result = nInitParams(
                sourceChannels,
                sourceSampleRate,
                targetSampleRate,
                bitRate,
                chMode,
                quality,
                vbrMode,
                sourceIsBigEndian);
            if (result < 0)
                throw new ArgumentException("parameters not supported by LAME (returned " + result + ")");
        }

        /// <summary>
        ///     returns -1 if string is too short or returns one of the exception
        ///     constants if everything OK, returns the length of the string
        /// </summary>
        public virtual string EncoderVersion => ver.LameVersion;

        /// <summary>
        ///     Returns the buffer needed pcm buffer size. The passed parameter is a
        ///     wished buffer size. The implementation of the encoder may return a lower
        ///     or higher buffer size. The encoder must be initalized (i.e. not closed)
        ///     at this point. A return value of <0 denotes an error.
        /// </summary>
        public virtual int PCMBufferSize => DEFAULT_PCM_BUFFER_SIZE;

        public virtual int MP3BufferSize => PCMBufferSize / 2 + 1024;

        public virtual int InputBufferSize => PCMBufferSize;

        public virtual int OutputBufferSize => MP3BufferSize;

        /// <summary>
        ///     Encode a block of data. Throws IllegalArgumentException when parameters
        ///     are wrong. When the <code>encoded</code> array is too small, an
        ///     ArrayIndexOutOfBoundsException is thrown. <code>length</code> should be
        ///     the value returned by getPCMBufferSize.
        /// </summary>
        /// <returns> the number of bytes written to <code>encoded</code>. May be 0. </returns>
        public virtual int EncodeBuffer(byte[] pcm, int offset, int length, byte[] encoded)
        {
            if (length < 0 || offset + length > pcm.Length)
                throw new ArgumentException("inconsistent parameters");

            var result = doEncodeBuffer(pcm, offset, length, encoded);
            if (result < 0)
            {
                if (result == -1)
                    throw new IndexOutOfRangeException("Encode buffer too small");

                throw new Exception("crucial error in encodeBuffer.");
            }

            return result;
        }

        public virtual int EncodeFinish(byte[] encoded)
        {
            return lame.lame_encode_flush(gfp, encoded, 0, encoded.Length);
        }

        public virtual void Close()
        {
            lame.lame_close(gfp);
        }

        /// <summary>
        ///     Return the audioformat representing the encoded mp3 stream. The format
        ///     object will have the following properties:
        ///     <ul>
        ///         <li>
        ///             quality: an Integer, 1 (highest) to 9 (lowest)
        ///             <li>
        ///                 bitrate: an Integer, 32...320 kbit/s
        ///                 <li>
        ///                     chmode: channel mode, a String, one of &quot;jointstereo&quot;,
        ///                     &quot;dual&quot;, &quot;mono&quot;, &quot;auto&quot; (default).
        ///                     <li>
        ///                         vbr: a Boolean
        ///                         <li>
        ///                             encoder.version: a string with the version of the encoder
        ///                             <li>encoder.name: a string with the name of the encoder
        ///     </ul>
        /// </summary>
        public virtual AudioFormat EffectiveFormat
        {
            get
            {
                // first gather properties
                var map = new Dictionary<string, object>();
                map[P_QUALITY] = EffectiveQuality;
                map[P_BITRATE] = EffectiveBitRate;
                map[P_CHMODE] = chmode2string(EffectiveChannelMode);
                map[P_VBR] = EffectiveVBR;

                // map.put(P_SAMPLERATE, getEffectiveSampleRate());
                // map.put(P_ENCODING,getEffectiveEncoding());
                map["encoder.name"] = "LAME";
                map["encoder.version"] = EncoderVersion;
                short channels = 2;
                if (chMode == CHANNEL_MODE_MONO)
                    channels = 1;

                return new AudioFormat
                {
                    SampleRate = EffectiveSampleRate,
                    BitsPerSample = NOT_SPECIFIED,
                    Channels = channels,
                    BigEndian = false,
                    IsFloatingPoint = false,
                    Properties = map
                };
            }
        }

        public virtual int EffectiveQuality
        {
            get
            {
                if (effQuality >= QUALITY_LOWEST)
                    return QUALITY_LOWEST;
                if (effQuality >= QUALITY_LOW)
                    return QUALITY_LOW;
                if (effQuality >= QUALITY_MIDDLE)
                    return QUALITY_MIDDLE;
                if (effQuality >= QUALITY_HIGH)
                    return QUALITY_HIGH;

                return QUALITY_HIGHEST;
            }
        }

        public virtual int EffectiveBitRate => effBitRate;

        public virtual int EffectiveChannelMode => effChMode;

        public virtual bool EffectiveVBR => effVbr != 0;

        public virtual int EffectiveSampleRate => effSampleRate;

        public float ConvertByteArrayToFloat(byte[] bytes, int offset, ByteOrder byteOrder)
        {
            var byte0 = bytes[offset + 0];
            var byte1 = bytes[offset + 1];
            var byte2 = bytes[offset + 2];
            var byte3 = bytes[offset + 3];
            int bits;
            if (byteOrder == ByteOrder.BIG_ENDIAN) //big endian
                bits = ((0xff & byte0) << 24) | ((0xff & byte1) << 16) | ((0xff & byte2) << 8) | ((0xff & byte3) << 0);
            else
                bits = ((0xff & byte3) << 24) | ((0xff & byte2) << 16) | ((0xff & byte1) << 8) | ((0xff & byte0) << 0);

            var bytebits = BitConverter.GetBytes(bits);
            var result = BitConverter.ToSingle(bytebits, 0);
            return result;
        }

        private void readParams(AudioFormat sourceFormat, IDictionary<string, object> props)
        {
            if (props != null)
                readProps(props);
        }

        /// <summary>
        ///     Initializes the lame encoder. Throws IllegalArgumentException when
        ///     parameters are not supported by LAME.
        /// </summary>
        private int nInitParams(
            int channels,
            int inSampleRate,
            int outSampleRate,
            int bitrate,
            int mode,
            int quality,
            bool VBR,
            bool bigEndian)
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
            gfp.num_channels = channels;
            gfp.in_samplerate = inSampleRate;
            if (outSampleRate >= 0)
                gfp.out_samplerate = outSampleRate;

            if (mode != CHANNEL_MODE_AUTO)
                Enum.TryParse(chmode2string(mode), out gfp.mode);

            if (VBR)
            {
                gfp.VBR = VbrMode.vbr_default;
                gfp.VBR_q = quality;
            }
            else
            {
                if (bitrate != BITRATE_AUTO)
                    gfp.brate = bitrate;
            }

            gfp.quality = quality;

            id3.id3tag_init(gfp);
            /*
             * turn off automatic writing of ID3 tag data into mp3 stream we have to
             * call it before 'lame_init_params', because that function would spit
             * out ID3v2 tag data.
             */
            gfp.write_id3tag_automatic = false;
            gfp.findReplayGain = true;

            var rc = lame.lame_init_params(gfp);

            // return effective values
            effSampleRate = gfp.out_samplerate;
            effBitRate = gfp.brate;
            effChMode = (int)gfp.mode;
            effVbr = (int)gfp.VBR;
            effQuality = VBR ? gfp.VBR_q : gfp.quality;
            return rc;
        }

        private int doEncodeBuffer(byte[] pcm, int offset, int length, byte[] encoded)
        {
            var bytes_per_sample = sourceSampleSizeInBits >> 3;
            var samples_read = length / bytes_per_sample;

            var sample_buffer = new int[samples_read];

            var sample_index = samples_read;
            if (!sourceIsFloatingPoint)
            {
                if (sourceByteOrder == ByteOrder.LITTLE_ENDIAN)
                {
                    if (bytes_per_sample == 1)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] = (pcm[offset + i] & 0xff) << 24;

                    if (bytes_per_sample == 2)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] =
                                ((pcm[offset + i] & 0xff) << 16) | ((pcm[offset + i + 1] & 0xff) << 24);

                    if (bytes_per_sample == 3)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] =
                                ((pcm[offset + i] & 0xff) << 8) | ((pcm[offset + i + 1] & 0xff) << 16) |
                                ((pcm[offset + i + 2] & 0xff) << 24);

                    if (bytes_per_sample == 4)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] =
                                (pcm[offset + i] & 0xff) | ((pcm[offset + i + 1] & 0xff) << 8) |
                                ((pcm[offset + i + 2] & 0xff) << 16) | ((pcm[offset + i + 3] & 0xff) << 24);
                }
                else
                {
                    if (bytes_per_sample == 1)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] = (((pcm[offset + i] & 0xff) ^ 0x80) << 24) | (0x7f << 16);

                    if (bytes_per_sample == 2)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] =
                                ((pcm[offset + i] & 0xff) << 24) | ((pcm[offset + i + 1] & 0xff) << 16);

                    if (bytes_per_sample == 3)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] =
                                ((pcm[offset + i] & 0xff) << 24) | ((pcm[offset + i + 1] & 0xff) << 16) |
                                ((pcm[offset + i + 2] & 0xff) << 8);

                    if (bytes_per_sample == 4)
                        for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                            sample_buffer[--sample_index] =
                                ((pcm[offset + i] & 0xff) << 24) | ((pcm[offset + i + 1] & 0xff) << 16) |
                                ((pcm[offset + i + 2] & 0xff) << 8) | (pcm[offset + i + 3] & 0xff);
                }
            }
            else
            {
                if (bytes_per_sample == 4)
                    for (var i = samples_read * bytes_per_sample; (i -= bytes_per_sample) >= 0;)
                    {
                        var sample = new byte[4];
                        sample[0] = pcm[offset + i];
                        sample[1] = pcm[offset + i + 1];
                        sample[2] = pcm[offset + i + 2];
                        sample[3] = pcm[offset + i + 3];
                        var amlitude = ConvertByteArrayToFloat(sample, 0, sourceByteOrder);
                        if (Math.Abs(amlitude) >= 1.0)
                            continue;

                        var sampleInt = (int)Math.Round(int.MaxValue * amlitude);
                        sample_buffer[--sample_index] = sampleInt;
                    }
            }

            var p = samples_read;
            samples_read /= gfp.num_channels;

            var buffer = Arrays.ReturnRectangularArray<int>(2, samples_read);
            if (gfp.num_channels == 2)
            {
                for (var i = samples_read; --i >= 0;)
                {
                    buffer[1][i] = sample_buffer[--p];
                    buffer[0][i] = sample_buffer[--p];
                }
            }
            else if (gfp.num_channels == 1)
            {
                Arrays.Fill(buffer[1], 0, samples_read, 0);
                for (var i = samples_read; --i >= 0;)
                    buffer[0][i] = buffer[1][i] = sample_buffer[--p];
            }

            var res = lame.lame_encode_buffer_int(gfp, buffer[0], buffer[1], samples_read, encoded, 0, encoded.Length);
            return res;
        }

        // properties
        private void readProps(IDictionary<string, object> props)
        {
            var q = props[P_QUALITY];
            if (q is string)
                quality = string2quality(((string)q).ToLower(), quality);
            else if (q is int)
                quality = (int)q;
            else if (q != null)
                throw new ArgumentException("illegal type of quality property: " + q);

            q = props[P_BITRATE];
            if (q is string)
                bitRate = int.Parse((string)q);
            else if (q is int?)
                bitRate = (int)q;
            else if (q != null)
                throw new ArgumentException("illegal type of bitrate property: " + q);

            q = props[P_CHMODE];
            if (q is string)
                chMode = string2chmode(((string)q).ToLower(), chMode);
            else if (q != null)
                throw new ArgumentException("illegal type of chmode property: " + q);

            q = props[P_VBR];
            if (q is string)
                vbrMode = string2bool((string)q);
            else if (q is bool)
                vbrMode = (bool)q;
            else if (q != null)
                throw new ArgumentException("illegal type of vbr property: " + q);
        }

        /*
		public virtual WaveFormatEncoding EffectiveEncoding
		{
			get
			{
				if (effEncoding == MPEG_VERSION_2)
				{
					if (EffectiveSampleRate < 16000)
					{
						return MPEG2DOT5L3;
					}
					return MPEG2L3;
				}
				else if (effEncoding == MPEG_VERSION_2DOT5)
				{
					return MPEG2DOT5L3;
				}
				// default
				return MPEG1L3;
			}
		}
        */

        private int string2quality(string quality, int def)
        {
            if (quality.Equals("lowest"))
                return QUALITY_LOWEST;
            if (quality.Equals("low"))
                return QUALITY_LOW;
            if (quality.Equals("middle"))
                return QUALITY_MIDDLE;
            if (quality.Equals("high"))
                return QUALITY_HIGH;
            if (quality.Equals("highest"))
                return QUALITY_HIGHEST;

            return def;
        }

        private string chmode2string(int chmode)
        {
            if (chmode == CHANNEL_MODE_STEREO)
                return "stereo";
            if (chmode == CHANNEL_MODE_JOINT_STEREO)
                return "jointstereo";
            if (chmode == CHANNEL_MODE_DUAL_CHANNEL)
                return "dual";
            if (chmode == CHANNEL_MODE_MONO)
                return "mono";
            if (chmode == CHANNEL_MODE_AUTO)
                return "auto";

            return "auto";
        }

        private int string2chmode(string chmode, int def)
        {
            if (chmode.Equals("stereo"))
                return CHANNEL_MODE_STEREO;
            if (chmode.Equals("jointstereo"))
                return CHANNEL_MODE_JOINT_STEREO;
            if (chmode.Equals("dual"))
                return CHANNEL_MODE_DUAL_CHANNEL;
            if (chmode.Equals("mono"))
                return CHANNEL_MODE_MONO;
            if (chmode.Equals("auto"))
                return CHANNEL_MODE_AUTO;

            return def;
        }

        /// <returns>
        ///     true if val is starts with t or y or on, false if val starts with
        ///     f or n or off.
        /// </returns>
        /// <exception cref="IllegalArgumentException">
        ///     if val is neither true nor false
        /// </exception>
        private static bool string2bool(string val)
        {
            if (val.Length > 0)
            {
                if (val[0] == 'f' || val[0] == 'n' || val.Equals("off"))
                    return false;

                if (val[0] == 't' || val[0] == 'y' || val.Equals("on"))
                    return true;
            }

            throw new ArgumentException("wrong string for boolean property: " + val);
        }

        /// <summary>
        /// * Lame.java ** </summary>
    }

}