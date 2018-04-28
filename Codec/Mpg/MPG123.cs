//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Codec.Mpg
{
    internal class MPG123
    {

        internal class gr_info_s
        {

            internal int big_values;

            internal int block_type;

            internal int count1table_select;

            internal float[][] full_gain = new float[3][];

            internal int[] full_gainPos = new int[3];

            internal int maxb;

            internal int[] maxband = new int[3];

            internal int maxbandl;

            internal int mixed_block_flag;

            internal int part2_3_length;

            internal float[] pow2gain;

            internal int pow2gainPos;

            internal int preflag;

            internal int region1start;

            internal int region2start;

            internal int scalefac_compress;

            internal int scalefac_scale;

            internal int scfsi;

            internal int[] subblock_gain = new int[3];

            internal int[] table_select = new int[3];
        }

        internal class grT
        {

            internal gr_info_s[] gr = new gr_info_s[2];

            internal grT()
            {
                gr[0] = new gr_info_s();
                gr[1] = new gr_info_s();
            }
        }

        internal class III_sideinfo
        {

            internal grT[] ch = new grT[2];

            internal int main_data_begin;

            internal int private_bits;

            internal III_sideinfo()
            {
                ch[0] = new grT();
                ch[1] = new grT();
            }
        }

        internal const double M_PI = 3.14159265358979323846;

        internal const double M_SQRT2 = 1.41421356237309504880;

        internal const int MAXFRAMESIZE = 2880;

        internal const int MPG_MD_DUAL_CHANNEL = 2;

        internal const int MPG_MD_JOINT_STEREO = 1;

        internal const int MPG_MD_MONO = 3;

        internal const int MPG_MD_STEREO = 0;

        internal const int SBLIMIT = 32;

        /* AF: ADDED FOR LAYER1/LAYER2 */
        internal const int SCALE_BLOCK = 12;

        internal const int SSLIMIT = 18;
    }

}