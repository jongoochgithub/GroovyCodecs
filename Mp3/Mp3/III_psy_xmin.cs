//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using GroovyCodecs.Mp3.Common;

namespace GroovyCodecs.Mp3.Mp3
{
    internal sealed class III_psy_xmin
    {
        internal float[] l = new float[Encoder.SBMAX_l];

        internal float[][] s = Arrays.ReturnRectangularArray<float>(Encoder.SBMAX_s, 3);

        internal void assign(III_psy_xmin iii_psy_xmin)
        {
            Array.Copy(iii_psy_xmin.l, 0, l, 0, Encoder.SBMAX_l);
            for (var i = 0; i < Encoder.SBMAX_s; i++)
            for (var j = 0; j < 3; j++)
                s[i][j] = iii_psy_xmin.s[i][j];
        }
    }
}