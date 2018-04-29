//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    /// <summary>
    ///     Control Parameters set by User. These parameters are here for backwards
    ///     compatibility with the old, non-shared lib API. Please use the
    ///     lame_set_variablename() functions below
    ///     @author Ken
    /// </summary>
    internal class LameGlobalFlags
    {

        /* general control params */
        /// <summary>
        ///     collect data for a MP3 frame analyzer?
        /// </summary>
        internal bool analysis;

        /// <summary>
        ///     select ATH auto-adjust loudness calc
        /// </summary>
        internal int athaa_loudapprox;

        /// <summary>
        ///     dB, tune active region of auto-level
        /// </summary>
        internal float athaa_sensitivity;

        /// <summary>
        ///     select ATH auto-adjust scheme
        /// </summary>
        internal int athaa_type;

        /// <summary>
        ///     change ATH formula 4 shape
        /// </summary>
        internal float ATHcurve;

        /// <summary>
        ///     lower ATH by this many db
        /// </summary>
        internal float ATHlower;

        /// <summary>
        ///     only use ATH
        /// </summary>
        internal bool ATHonly;

        /// <summary>
        ///     only use ATH for short blocks
        /// </summary>
        internal bool ATHshort;

        /// <summary>
        ///     select ATH formula
        /// </summary>
        internal int ATHtype;

        /*
         * set either brate>0 or compression_ratio>0, LAME will compute the value of
         * the variable not set. Default is compression_ratio = 11.025
         */
        /// <summary>
        ///     bitrate
        /// </summary>
        internal int brate;

        /// <summary>
        ///     add Xing VBR tag?
        /// </summary>
        internal bool bWriteVbrTag;

        internal long class_id;

        /// <summary>
        ///     sizeof(wav file)/sizeof(mp3 file)
        /// </summary>
        internal float compression_ratio;

        /* frame params */
        /// <summary>
        ///     mark as copyright. default=0
        /// </summary>
        internal int copyright;

        /// <summary>
        ///     decode on the fly? default=0
        /// </summary>
        internal bool decode_on_the_fly;

        /// <summary>
        ///     use lame/mpglib to convert mp3 to wav
        /// </summary>
        internal bool decode_only;

        /// <summary>
        ///     use bit reservoir?
        /// </summary>
        internal bool disable_reservoir;

        /// <summary>
        ///     Input PCM is emphased PCM (for instance from one of the rarely emphased
        ///     CDs), it is STRONGLY not recommended to use this, because psycho does not
        ///     take it into account, and last but not least many decoders don't care
        ///     about these bits
        /// </summary>
        internal int emphasis;

        internal int encoder_delay;

        /// <summary>
        ///     number of samples of padding appended to input
        /// </summary>
        internal int encoder_padding;

        /// <summary>
        ///     use 2 bytes per frame for a CRC checksum. default=0
        /// </summary>
        internal bool error_protection;

        internal int exp_nspsytune;

        internal bool experimentalY;

        internal int experimentalZ;

        /// <summary>
        ///     the MP3 'private extension' bit. Meaningless
        /// </summary>
        internal int extension;

        /// <summary>
        ///     find the RG value? default=0
        /// </summary>
        internal bool findReplayGain;

        /// <summary>
        ///     force M/S mode. requires mode=1
        /// </summary>
        internal bool force_ms;

        /// <summary>
        ///     number of frames encoded
        /// </summary>
        internal int frameNum;

        internal int framesize;

        /// <summary>
        ///     use free format? default=0
        /// </summary>
        internal bool free_format;

        /// <summary>
        ///     freq in Hz. 0=lame choses. -1=no filter
        /// </summary>
        internal int highpassfreq;

        /// <summary>
        ///     freq width of filter, in Hz (default=15%)
        /// </summary>
        internal int highpasswidth;

        /// <summary>
        ///     input_samp_rate in Hz. default=44.1 kHz
        /// </summary>
        internal int in_samplerate;

        internal float interChRatio;

        /// <summary>
        ///     ***********************************************************************
        /// </summary>
        /// <summary>
        ///     ***********************************************************************
        /// </summary>
        internal LameInternalFlags internal_flags;

        /// <summary>
        ///     is this struct owned by calling program or lame?
        /// </summary>
        internal int lame_allocated_gfp;

        /* resampling and filtering */

        /// <summary>
        ///     freq in Hz. 0=lame choses. -1=no filter
        /// </summary>
        internal int lowpassfreq;

        /// <summary>
        ///     freq width of filter, in Hz (default=15%)
        /// </summary>
        internal int lowpasswidth;

        /*
         * psycho acoustics and other arguments which you should not change unless
         * you know what you are doing
         */

        internal float maskingadjust;

        internal float maskingadjust_short;

        /// <summary>
        ///     see enum default = LAME picks best value
        /// </summary>
        internal MPEGMode mode = MPEGMode.STEREO;

        /// <summary>
        ///     Naoki's adjustment of Mid/Side maskings
        /// </summary>
        internal float msfix;

        /// <summary>
        ///     disable ATH
        /// </summary>
        internal bool noATH;

        /// <summary>
        ///     input number of channels. default=2
        /// </summary>
        internal int num_channels;

        /* input description */

        /// <summary>
        ///     number of samples. default=-1
        /// </summary>
        internal int num_samples;

        /// <summary>
        ///     mark as original. default=1
        /// </summary>
        internal int original;

        /// <summary>
        ///     output_samp_rate. default: LAME picks best value at least not used for
        ///     MP3 decoding: Remember 44.1 kHz MP3s and AC97
        /// </summary>
        internal int out_samplerate;

        internal int preset;

        /// <summary>
        ///     quality setting 0=best, 9=worst default=5
        /// </summary>
        internal int quality;

        /* quantization/noise shaping */
        internal int quant_comp;

        internal int quant_comp_short;

        /// <summary>
        ///     scale input by this amount before encoding at least not used for MP3
        ///     decoding
        /// </summary>
        internal float scale;

        /// <summary>
        ///     scale input of channel 0 (left) by this amount before encoding
        /// </summary>
        internal float scale_left;

        /// <summary>
        ///     scale input of channel 1 (right) by this amount before encoding
        /// </summary>
        internal float scale_right;

        internal ShortBlock? short_blocks;

        /// <summary>
        ///     enforce ISO spec as much as possible
        /// </summary>
        internal bool strict_ISO;

        /// <summary>
        ///     0 off, 1 on
        /// </summary>
        internal bool tune;

        /// <summary>
        ///     used to pass values for debugging and stuff
        /// </summary>
        internal float tune_value_a;

        /// <summary>
        ///     use temporal masking effect
        /// </summary>
        internal bool? useTemporal;

        /* VBR control */
        internal VbrMode VBR;

        /// <summary>
        ///     strictly enforce VBR_min_bitrate normaly, it will be violated for analog
        ///     silence
        /// </summary>
        internal int VBR_hard_min;

        internal int VBR_max_bitrate_kbps;

        internal int VBR_mean_bitrate_kbps;

        internal int VBR_min_bitrate_kbps;

        /// <summary>
        ///     Range [0,...,9]
        /// </summary>
        internal int VBR_q;

        /// <summary>
        ///     Range [0,...,1[
        /// </summary>
        internal float VBR_q_frac;

        /// <summary>
        ///     *********************************************************************
        /// </summary>
        /// <summary>
        ///     *********************************************************************
        /// </summary>
        /// <summary>
        ///     0=MPEG-2/2.5 1=MPEG-1
        /// </summary>
        internal int version;

        /// <summary>
        ///     1 (default) writes ID3 tags, 0 not
        /// </summary>
        internal bool write_id3tag_automatic;
    }

}