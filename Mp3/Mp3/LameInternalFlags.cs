//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using GroovyCodecs.Mp3.Common;
using GroovyCodecs.Mp3.Mpg;

namespace GroovyCodecs.Mp3.Mp3
{

    internal class LameInternalFlags
    {

        internal class Header
        {

            internal byte[] buf = new byte[MAX_HEADER_LEN];

            internal int ptr;

            internal int write_timing;
        }

        /* BPC = maximum number of filter convolution windows to precompute */
        internal const int BPC = 320;

        internal const int MAX_BITS_PER_CHANNEL = 4095;

        internal const int MAX_BITS_PER_GRANULE = 7680;

        /* variables for BitStream */

        /// <summary>
        ///     <PRE>
        ///         mpeg1: buffer=511 bytes  smallest frame: 96-38(sideinfo)=58
        ///         max number of frames in reservoir:  8
        ///         mpeg2: buffer=255 bytes.  smallest frame: 24-23bytes=1
        ///         with VBR, if you are encoding all silence, it is possible to
        ///         have 8kbs/24khz frames with 1byte of data each, which means we need
        ///         to buffer up to 255 headers!
        ///     </PRE>
        /// </summary>
        /// <summary>
        ///     also, max_header_buf has to be a power of two
        /// </summary>
        internal const int MAX_HEADER_BUF = 256;

        /// <summary>
        ///     max size of header is 38
        /// </summary>
        private const int MAX_HEADER_LEN = 40;

        internal static readonly int MFSIZE = 3 * 1152 + Encoder.ENCDELAY - Encoder.MDCTDELAY;

        internal float[] amp_filter = new float[32];

        internal int ancillary_flag;

        /// <summary>
        ///     all ATH related stuff
        /// </summary>
        internal ATH ATH;

        internal int AudiophileGain;

        /// <summary>
        ///     norm/start/short/stop/mixed(short)/sum
        /// </summary>
        internal int[][] bitrate_blockType_Hist = Arrays.ReturnRectangularArray<int>(16, 4 + 1 + 1);

        internal int bitrate_index;

        /* simple statistics */

        internal int[][] bitrate_stereoMode_Hist = Arrays.ReturnRectangularArray<int>(16, 4 + 1);

        internal float[][] blackfilt = new float[2 * BPC + 1][];

        /// <summary>
        ///     block type
        /// </summary>
        internal int[] blocktype_old = new int[2];

        internal int[] bm_l = new int[Encoder.SBMAX_l];

        internal int[] bm_s = new int[Encoder.SBMAX_s];

        internal int[] bo_l = new int[Encoder.SBMAX_l];

        internal int[] bo_s = new int[Encoder.SBMAX_s];

        internal int[] bv_scf = new int[576];

        /// <summary>
        ///     number of channels in the input data stream (PCM or decoded PCM)
        /// </summary>
        internal int channels_in;

        /// <summary>
        ///     number of channels in the output data stream (not used for decoding)
        /// </summary>
        internal int channels_out;

        /// <summary>
        ///     ******************************************************************
        ///     internal variables NOT set by calling program, and should not be *
        ///     modified by the calling program *
        ///     *******************************************************************
        /// </summary>
        /// <summary>
        ///     Some remarks to the Class_ID field: The Class ID is an Identifier for a
        ///     pointer to this struct. It is very unlikely that a pointer to
        ///     lame_global_flags has the same 32 bits in it's structure (large and other
        ///     special properties, for instance prime).
        ///     To test that the structure is right and initialized, use: if ( gfc .
        ///     Class_ID == LAME_ID ) ... Other remark: If you set a flag to 0 for uninit
        ///     data and 1 for init data, the right test should be "if (flag == 1)" and
        ///     NOT "if (flag)". Unintended modification of this element will be
        ///     otherwise misinterpreted as an init.
        /// </summary>
        internal long Class_ID;

        internal int[] CurrentStep = new int[2];

        internal float decay;

        /* ReplayGain */
        internal bool decode_on_the_fly = true;

        internal III_psy_xmin[] en = new III_psy_xmin[4];

        internal int fill_buffer_resample_init;

        internal bool findPeakSample = true;

        internal bool findReplayGain = true;

        internal int frac_SpF;

        /// <summary>
        ///     0 = stop early after 0 distortion found. 1 = full search
        /// </summary>
        internal int full_outer_loop;

        internal int h_ptr;

        internal Header[] header = new Header[MAX_HEADER_BUF];

        /// <summary>
        ///     normalized frequency bounds of passband
        /// </summary>
        internal float highpass1, highpass2;

        internal MPGLib.mpstr_tag hip;

        internal float[] in_buffer_0;

        internal float[] in_buffer_1;

        internal int in_buffer_nsamples;

        internal float[][] inbuf_old = new float[2][];

        internal int iteration_init_init;

        internal IIterationLoop iteration_loop;

        internal double[] itime = new double[2];

        internal IIISideInfo l3_side = new IIISideInfo();

        internal int lame_encode_frame_init;

        /* loudness calculation (for adaptive threshold of hearing) */
        /// <summary>
        ///     loudness^2 approx. per granule and channel
        /// </summary>
        internal float[][] loudness_sq = Arrays.ReturnRectangularArray<float>(2, 2);

        /// <summary>
        ///     account for granule delay of L3psycho_anal
        /// </summary>
        internal float[] loudness_sq_save = new float[2];

        /* lowpass and highpass filter control */
        /// <summary>
        ///     normalized frequency bounds of passband
        /// </summary>
        internal float lowpass1, lowpass2;

        internal float masking_lower;

        internal int mf_samples_to_encode;

        internal int mf_size;

        internal float[][] mfbuf = Arrays.ReturnRectangularArray<float>(2, MFSIZE);

        /* daa from PsyModel */
        /* The static variables "r", "phi_sav", "new", "old" and "oldest" have */
        /* to be remembered for the unpredictability measure. For "r" and */
        /* "phi_sav", the first index from the left is the channel select and */
        /* the second index is the "age" of the data. */
        internal float[] minval_l = new float[Encoder.CBANDS];

        internal float[] minval_s = new float[Encoder.CBANDS];

        internal float[] mld_cb_l = new float[Encoder.CBANDS];

        internal float[] mld_cb_s = new float[Encoder.CBANDS];

        /// <summary>
        ///     Scale Factor Bands
        /// </summary>
        internal float[] mld_l = new float[Encoder.SBMAX_l];

        internal float[] mld_s = new float[Encoder.SBMAX_s];

        internal int mode_ext;

        /// <summary>
        ///     granules per frame
        /// </summary>
        internal int mode_gr;

        internal float ms_ener_ratio_old;

        internal float[] ms_ratio = new float[2];

        internal float ms_ratio_s_old, ms_ratio_l_old;

        internal float[][] nb_1 = Arrays.ReturnRectangularArray<float>(4, Encoder.CBANDS);

        internal float[][] nb_2 = Arrays.ReturnRectangularArray<float>(4, Encoder.CBANDS);

        internal float[][] nb_s1 = Arrays.ReturnRectangularArray<float>(4, Encoder.CBANDS);

        internal float[][] nb_s2 = Arrays.ReturnRectangularArray<float>(4, Encoder.CBANDS);

        internal int nMusicCRC;

        /// <summary>
        ///     gain change required for preventing clipping
        /// </summary>
        internal int noclipGainChange;

        /// <summary>
        ///     user-specified scale factor required for preventing clipping
        /// </summary>
        internal float noclipScale;

        internal int nogap_current;

        internal int nogap_total;

        /// <summary>
        ///     0 = none 1 = ISO AAC model 2 = allow scalefac_select=1
        /// </summary>
        internal int noise_shaping;

        /// <summary>
        ///     0 = ISO model: amplify all distorted bands
        ///     <BR>
        ///         1 = amplify within 50% of max (on db scale)
        ///         <BR>
        ///             2 = amplify only most distorted band
        ///             <BR>
        ///                 3 = method 1 and refine with method 2<BR>
        /// </summary>
        internal int noise_shaping_amp;

        /// <summary>
        ///     0 = stop at over=0, all scalefacs amplified or
        ///     <BR>
        ///         a scalefac has reached max value
        ///         <BR>
        ///             1 = stop when all scalefacs amplified or a scalefac has reached max value
        ///             <BR>
        ///                 2 = stop when all scalefacs amplified
        /// </summary>
        internal int noise_shaping_stop;

        internal int npart_l, npart_s;

        /// <summary>
        ///     variables used for --nspsytune
        /// </summary>
        internal NsPsy nsPsy = new NsPsy();

        internal int[] numlines_l = new int[Encoder.CBANDS];

        internal int numlines_l_num1;

        internal int[] numlines_s = new int[Encoder.CBANDS];

        internal int numlines_s_num1;

        /* variables used by Quantize */
        internal int[] OldValue = new int[2];

        /* used for padding */
        /// <summary>
        ///     padding for the current frame?
        /// </summary>
        internal int padding;

        /* ratios */
        internal float[] pe = new float[4];

        internal float PeakSample;

        internal PlottingData pinfo;

        internal int[] pseudohalf = new int[L3Side.SFBMAX];

        internal PSY PSY;

        /// <summary>
        ///     1 = gpsycho. 0 = none
        /// </summary>
        internal int psymodel;

        internal int RadioGain;

        /// <summary>
        ///     input_samp_rate/output_samp_rate
        /// </summary>
        internal double resample_ratio;

        /// <summary>
        ///     in bits
        /// </summary>
        internal int ResvMax;

        /* variables for Reservoir */
        /// <summary>
        ///     in bits
        /// </summary>
        internal int ResvSize;

        internal ReplayGain rgdata;

        internal float[] rnumlines_l = new float[Encoder.CBANDS];

        internal float[] s3_ll;

        internal float[] s3_ss;

        internal int[][] s3ind = Arrays.ReturnRectangularArray<int>(Encoder.CBANDS, 2);

        internal int[][] s3ind_s = Arrays.ReturnRectangularArray<int>(Encoder.CBANDS, 2);

        internal int samplerate_index;

        /* variables for newmdct.c */

        internal float[][][][] sb_sample = Arrays.ReturnRectangularArray<float>(2, 2, 18, Encoder.SBLIMIT);

        internal ScaleFac scalefac_band = new ScaleFac();

        /// <summary>
        ///     will be set in lame_init_params
        /// </summary>
        internal bool sfb21_extra;

        internal int sideinfo_len;

        internal int slot_lag;

        /// <summary>
        ///     0 = no, 1 = yes
        /// </summary>
        internal int subblock_gain;

        /// <summary>
        ///     0 = no substep
        ///     <BR>
        ///         1 = use substep shaping at last step(VBR only)
        ///         <BR>
        ///             (not implemented yet)
        ///             <BR>
        ///                 2 = use substep inside loop
        ///                 <BR>
        ///                     3 = use substep inside loop and last step<BR>
        /// </summary>
        internal int substep_shaping;

        /// <summary>
        ///     optional ID3 tags
        /// </summary>
        internal ID3TagSpec tag_spec;

        internal III_psy_xmin[] thm = new III_psy_xmin[4];

        /// <summary>
        ///     fft and energy calculation
        /// </summary>
        internal float[] tot_ener = new float[4];

        /// <summary>
        ///     0 = no. 1=outside loop 2=inside loop(slow)
        /// </summary>
        internal int use_best_huffman;

        /// <summary>
        ///     max bitrate index
        /// </summary>
        internal int VBR_max_bitrate;

        /// <summary>
        ///     min bitrate index
        /// </summary>
        internal int VBR_min_bitrate;

        /// <summary>
        ///     used for Xing VBR header
        /// </summary>
        internal VBRSeekInfo VBR_seek_table = new VBRSeekInfo();

        internal int w_ptr;

        internal LameInternalFlags()
        {
            for (var i = 0; i < en.Length; i++)
                en[i] = new III_psy_xmin();

            for (var i = 0; i < thm.Length; i++)
                thm[i] = new III_psy_xmin();

            for (var i = 0; i < header.Length; i++)
                header[i] = new Header();
        }
    }

}