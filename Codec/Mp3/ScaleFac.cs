//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;

namespace GroovyMp3.Codec.Mp3
{
    /// <summary>
    ///     Layer III side information.
    ///     @author Ken
    /// </summary>
    internal sealed class ScaleFac
    {

        internal int[] l = new int[1 + Encoder.SBMAX_l];

        internal int[] psfb12 = new int[1 + Encoder.PSFB12];

        internal int[] psfb21 = new int[1 + Encoder.PSFB21];

        internal int[] s = new int[1 + Encoder.SBMAX_s];

        internal ScaleFac()
        {
        }

        internal ScaleFac(int[] arrL, int[] arrS, int[] arr21, int[] arr12)
        {
            Array.Copy(arrL, 0, l, 0, Math.Min(arrL.Length, l.Length));
            Array.Copy(arrS, 0, s, 0, Math.Min(arrS.Length, s.Length));
            Array.Copy(arr21, 0, psfb21, 0, Math.Min(arr21.Length, psfb21.Length));
            Array.Copy(arr12, 0, psfb12, 0, Math.Min(arr12.Length, psfb12.Length));
        }
    }
}