//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using GroovyMp3.Types;

namespace GroovyMp3.Codec.Mp3
{
    internal class IIISideInfo
    {

        internal int main_data_begin;

        internal int private_bits;

        internal int resvDrain_post;

        internal int resvDrain_pre;

        internal int[][] scfsi = Arrays.ReturnRectangularArray<int>(2, 4);

        internal GrInfo[][] tt = Arrays.ReturnRectangularArray<GrInfo>(2, 2);

        internal IIISideInfo()
        {
            for (var gr = 0; gr < 2; gr++)
            for (var ch = 0; ch < 2; ch++)
                tt[gr][ch] = new GrInfo();
        }
    }
}