//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Diagnostics;

namespace GroovyMp3.Codec.Mp3
{

    internal sealed class LongBlockConstrain : VBRQuantize.alloc_sf_f
    {
        /// 
        private readonly VBRQuantize vbrQuantize;

        /// <param name="vbrQuantize"> </param>
        internal LongBlockConstrain(VBRQuantize vbrQuantize)
        {
            this.vbrQuantize = vbrQuantize;
        }

        /// <summary>
        ///     ****************************************************************
        ///     long block scalefacs
        ///     *****************************************************************
        /// </summary>
        public void alloc(VBRQuantize.algo_t that, int[] vbrsf, int[] vbrsfmin, int vbrmax)
        {

            var cod_info = that.cod_info;

            var gfc = that.gfc;
            int[] max_rangep;

            var maxminsfb = that.mingain_l;
            int sfb;
            int maxover0, maxover1, maxover0p, maxover1p, mover, delta = 0;
            int v, v0, v1, v0p, v1p, vm0p = 1, vm1p = 1;

            var psymax = cod_info.psymax;

            max_rangep = gfc.mode_gr == 2 ? VBRQuantize.max_range_long : VBRQuantize.max_range_long_lsf_pretab;

            maxover0 = 0;
            maxover1 = 0;
            maxover0p = 0; // pretab
            maxover1p = 0; // pretab

            for (sfb = 0; sfb < psymax; ++sfb)
            {
                Debug.Assert(vbrsf[sfb] >= vbrsfmin[sfb]);
                v = vbrmax - vbrsf[sfb];
                if (delta < v)
                    delta = v;

                v0 = v - 2 * VBRQuantize.max_range_long[sfb];
                v1 = v - 4 * VBRQuantize.max_range_long[sfb];
                v0p = v - 2 * (max_rangep[sfb] + vbrQuantize.qupvt.pretab[sfb]);
                v1p = v - 4 * (max_rangep[sfb] + vbrQuantize.qupvt.pretab[sfb]);
                if (maxover0 < v0)
                    maxover0 = v0;

                if (maxover1 < v1)
                    maxover1 = v1;

                if (maxover0p < v0p)
                    maxover0p = v0p;

                if (maxover1p < v1p)
                    maxover1p = v1p;
            }

            if (vm0p == 1)
            {
                var gain = vbrmax - maxover0p;
                if (gain < maxminsfb)
                    gain = maxminsfb;

                for (sfb = 0; sfb < psymax; ++sfb)
                {

                    var a = gain - vbrsfmin[sfb] - 2 * vbrQuantize.qupvt.pretab[sfb];
                    if (a <= 0)
                    {
                        vm0p = 0;
                        vm1p = 0;
                        break;
                    }
                }
            }

            if (vm1p == 1)
            {
                var gain = vbrmax - maxover1p;
                if (gain < maxminsfb)
                    gain = maxminsfb;

                for (sfb = 0; sfb < psymax; ++sfb)
                {

                    var b = gain - vbrsfmin[sfb] - 4 * vbrQuantize.qupvt.pretab[sfb];
                    if (b <= 0)
                    {
                        vm1p = 0;
                        break;
                    }
                }
            }

            if (vm0p == 0)
                maxover0p = maxover0;

            if (vm1p == 0)
                maxover1p = maxover1;

            if (gfc.noise_shaping != 2)
            {
                maxover1 = maxover0;
                maxover1p = maxover0p;
            }

            mover = Math.Min(maxover0, maxover0p);
            mover = Math.Min(mover, maxover1);
            mover = Math.Min(mover, maxover1p);

            if (delta > mover)
                delta = mover;

            vbrmax -= delta;
            if (vbrmax < maxminsfb)
                vbrmax = maxminsfb;

            maxover0 -= mover;
            maxover0p -= mover;
            maxover1 -= mover;
            maxover1p -= mover;

            if (maxover0 == 0)
            {
                cod_info.scalefac_scale = 0;
                cod_info.preflag = 0;
                max_rangep = VBRQuantize.max_range_long;
            }
            else if (maxover0p == 0)
            {
                cod_info.scalefac_scale = 0;
                cod_info.preflag = 1;
            }
            else if (maxover1 == 0)
            {
                cod_info.scalefac_scale = 1;
                cod_info.preflag = 0;
                max_rangep = VBRQuantize.max_range_long;
            }
            else if (maxover1p == 0)
            {
                cod_info.scalefac_scale = 1;
                cod_info.preflag = 1;
            }
            else
            {
                Debug.Assert(false); // this should not happen
            }

            cod_info.global_gain = vbrmax;
            if (cod_info.global_gain < 0)
                cod_info.global_gain = 0;
            else if (cod_info.global_gain > 255)
                cod_info.global_gain = 255;

            {
                var sf_temp = new int[L3Side.SFBMAX];
                for (sfb = 0; sfb < L3Side.SFBMAX; ++sfb)
                    sf_temp[sfb] = vbrsf[sfb] - vbrmax;

                vbrQuantize.set_scalefacs(cod_info, vbrsfmin, sf_temp, max_rangep);
            }
            Debug.Assert(vbrQuantize.checkScalefactor(cod_info, vbrsfmin));

        }
    }
}