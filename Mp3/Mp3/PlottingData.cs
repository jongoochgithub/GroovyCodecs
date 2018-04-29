//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

/*
 *      GTK plotting routines source file
 *
 *      Copyright (c) 1999 Mark Taylor
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */
using GroovyCodecs.Mp3.Common;

namespace GroovyCodecs.Mp3.Mp3
{
    /// <summary>
    ///     used by the frame analyzer
    /// </summary>
    internal class PlottingData
    {

        internal int[][] big_values = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int[][] blocktype = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int crc, padding;

        internal double[][][] en = Arrays.ReturnRectangularArray<double>(2, 4, Encoder.SBMAX_l);

        internal double[][][] en_s = Arrays.ReturnRectangularArray<double>(2, 4, 3 * Encoder.SBMAX_s);

        internal double[][][] energy = Arrays.ReturnRectangularArray<double>(2, 4, Encoder.BLKSIZE);

        /* L,R, M and S values */

        /// <summary>
        ///     psymodel is one ahead
        /// </summary>
        internal double[][] energy_save = Arrays.ReturnRectangularArray<double>(4, Encoder.BLKSIZE);

        internal double[][] ers = Arrays.ReturnRectangularArray<double>(2, 4);

        /// <summary>
        ///     psymodel is one ahead
        /// </summary>
        internal double[] ers_save = new double[4];

        internal int framesize, stereo, js, ms_stereo, i_stereo, emph, bitrate, sampfreq, maindata;

        internal int[][] LAMEmainbits = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int[][] LAMEqss = Arrays.ReturnRectangularArray<int>(2, 2);

        internal double[][][] LAMEsfb = Arrays.ReturnRectangularArray<double>(2, 2, Encoder.SBMAX_l);

        internal double[][][] LAMEsfb_s = Arrays.ReturnRectangularArray<double>(2, 2, 3 * Encoder.SBMAX_s);

        internal int[][] LAMEsfbits = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int[][] mainbits = Arrays.ReturnRectangularArray<int>(2, 2);

        internal double[][] max_noise = Arrays.ReturnRectangularArray<double>(2, 2);

        internal int mean_bits;

        internal int[][] mixed = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int[][] mpg123blocktype = Arrays.ReturnRectangularArray<int>(2, 2);

        internal double[][][] mpg123xr = Arrays.ReturnRectangularArray<double>(2, 2, 576);

        internal double[] ms_ener_ratio = new double[2];

        internal double[] ms_ratio = new double[2];

        internal int[][] over = Arrays.ReturnRectangularArray<int>(2, 2);

        internal double[][] over_noise = Arrays.ReturnRectangularArray<double>(2, 2);

        internal int[][] over_SSD = Arrays.ReturnRectangularArray<int>(2, 2);

        internal double[][] pcmdata = Arrays.ReturnRectangularArray<double>(2, 1600);

        internal double[][] pcmdata2 =
            Arrays.ReturnRectangularArray<double>(2, 1152 + 1152 - Encoder.DECDELAY);

        internal double[][] pe = Arrays.ReturnRectangularArray<double>(2, 4);

        internal int[][] preflag = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int[][] qss = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int resvsize;

        internal int[][] scalefac_scale = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int[] scfsi = new int[2];

        internal double[][][] sfb = Arrays.ReturnRectangularArray<double>(2, 2, Encoder.SBMAX_l);

        internal double[][][] sfb_s = Arrays.ReturnRectangularArray<double>(2, 2, 3 * Encoder.SBMAX_s);

        internal int[][] sfbits = Arrays.ReturnRectangularArray<int>(2, 2);

        internal int[][][] sub_gain = Arrays.ReturnRectangularArray<int>(2, 2, 3);

        internal double[][][] thr = Arrays.ReturnRectangularArray<double>(2, 4, Encoder.SBMAX_l);

        internal double[][][] thr_s = Arrays.ReturnRectangularArray<double>(2, 4, 3 * Encoder.SBMAX_s);

        internal double[][] tot_noise = Arrays.ReturnRectangularArray<double>(2, 2);

        internal int totbits;

        internal double[][][] xfsf = Arrays.ReturnRectangularArray<double>(2, 2, Encoder.SBMAX_l);

        internal double[][][] xfsf_s = Arrays.ReturnRectangularArray<double>(2, 2, 3 * Encoder.SBMAX_s);

        internal double[][][] xr = Arrays.ReturnRectangularArray<double>(2, 2, 576);
    }
}