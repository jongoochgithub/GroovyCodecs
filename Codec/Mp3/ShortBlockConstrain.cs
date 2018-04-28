//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Diagnostics;

namespace GroovyMp3.Codec.Mp3
{
    internal sealed class ShortBlockConstrain : VBRQuantize.alloc_sf_f
    {
        /// 
        private readonly VBRQuantize vbrQuantize;

        /// <param name="vbrQuantize"> </param>
        internal ShortBlockConstrain(VBRQuantize vbrQuantize)
        {
            this.vbrQuantize = vbrQuantize;
        }

        /// <summary>
        ///     ****************************************************************
        ///     short block scalefacs
        ///     *****************************************************************
        /// </summary>
        public void alloc(VBRQuantize.algo_t that, int[] vbrsf, int[] vbrsfmin, int vbrmax)
        {

            var cod_info = that.cod_info;

            var gfc = that.gfc;

            var maxminsfb = that.mingain_l;
            int mover, maxover0 = 0, maxover1 = 0, delta = 0;
            int v, v0, v1;
            int sfb;

            var psymax = cod_info.psymax;

            for (sfb = 0; sfb < psymax; ++sfb)
            {
                Debug.Assert(vbrsf[sfb] >= vbrsfmin[sfb]);
                v = vbrmax - vbrsf[sfb];
                if (delta < v)
                    delta = v;

                v0 = v - (4 * 14 + 2 * VBRQuantize.max_range_short[sfb]);
                v1 = v - (4 * 14 + 4 * VBRQuantize.max_range_short[sfb]);
                if (maxover0 < v0)
                    maxover0 = v0;

                if (maxover1 < v1)
                    maxover1 = v1;
            }

            if (gfc.noise_shaping == 2)
                mover = Math.Min(maxover0, maxover1);
            else
                mover = maxover0;

            if (delta > mover)
                delta = mover;

            vbrmax -= delta;
            maxover0 -= mover;
            maxover1 -= mover;

            if (maxover0 == 0)
                cod_info.scalefac_scale = 0;
            else if (maxover1 == 0)
                cod_info.scalefac_scale = 1;

            if (vbrmax < maxminsfb)
                vbrmax = maxminsfb;

            cod_info.global_gain = vbrmax;

            if (cod_info.global_gain < 0)
                cod_info.global_gain = 0;
            else if (cod_info.global_gain > 255)
                cod_info.global_gain = 255;

            {
                var sf_temp = new int[L3Side.SFBMAX];
                for (sfb = 0; sfb < L3Side.SFBMAX; ++sfb)
                    sf_temp[sfb] = vbrsf[sfb] - vbrmax;

                vbrQuantize.set_subblock_gain(cod_info, that.mingain_s, sf_temp);
                vbrQuantize.set_scalefacs(cod_info, vbrsfmin, sf_temp, VBRQuantize.max_range_short);
            }
            Debug.Assert(vbrQuantize.checkScalefactor(cod_info, vbrsfmin));

        }
    }
}