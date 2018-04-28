//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Codec.Mp3
{
    internal class MP3Data
    {

        /// <summary>
        ///     bitrate
        /// </summary>
        internal int bitrate;

        /* this data is not currently computed by the mpglib routines */

        /// <summary>
        ///     frames decoded counter
        /// </summary>
        internal int framenum;

        /// <summary>
        ///     number of samples per mp3 frame
        /// </summary>
        internal int framesize;

        /// <summary>
        ///     true if header was parsed and following data was computed
        /// </summary>
        internal bool header_parsed;

        /// <summary>
        ///     mp3 frame type
        /// </summary>
        internal int mode;

        /// <summary>
        ///     mp3 frame type
        /// </summary>
        internal int mode_ext;

        /* this data is only computed if mpglib detects a Xing VBR header */

        /// <summary>
        ///     number of samples in mp3 file.
        /// </summary>
        internal int nsamp;

        /// <summary>
        ///     sample rate
        /// </summary>
        internal int samplerate;

        /// <summary>
        ///     number of channels
        /// </summary>
        internal int stereo;

        /// <summary>
        ///     total number of frames in mp3 file
        /// </summary>
        internal int totalframes;
    }
}