//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Codec.Mpg
{

    internal class Frame
    {

        internal L2Tables.al_table2[] alloc;

        internal int bitrate_index;

        internal int copyright;

        internal int down_sample;

        internal int down_sample_sblimit;

        internal int emphasis;

        internal bool error_protection; // 1 = CRC-16 code following header

        internal int extension;

        internal int framesize; // computed framesize

        /* AF: ADDED FOR LAYER1/LAYER2 */
        internal int II_sblimit;

        internal int jsbound;

        internal int lay; // Layer

        internal int lsf; // 0 = MPEG-1, 1 = MPEG-2/2.5

        internal int mode;

        internal int mode_ext;

        internal bool mpeg25; // 1 = MPEG-2.5, 0 = MPEG-1/2

        internal int original;

        internal int padding;

        internal int sampling_frequency; // sample rate of decompressed audio in Hz

        internal int single; // single channel (monophonic)

        internal int stereo;
    }

}