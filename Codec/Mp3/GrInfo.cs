//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;

namespace GroovyMp3.Codec.Mp3
{
    internal sealed class GrInfo
    {

        internal int big_values;

        internal int block_type;

        internal int count1;

        internal int count1bits;

        internal int count1table_select;

        internal int global_gain;

        internal int[] l3_enc = new int[576];

        internal int max_nonzero_coeff;

        internal int mixed_block_flag;

        internal int part2_3_length;

        internal int part2_length;

        internal int preflag;

        internal int psy_lmax;

        internal int psymax;

        internal int region0_count;

        internal int region1_count;

        internal int[] scalefac = new int[L3Side.SFBMAX];

        internal int scalefac_compress;

        internal int scalefac_scale;

        internal int sfb_lmax;

        /// <summary>
        ///     added for LSF
        /// </summary>
		internal int[] sfb_partition_table;

        internal int sfb_smin;

        internal int sfbdivide;

        internal int sfbmax;

        internal int[] slen = new int[4];

        internal int[] subblock_gain = new int[3 + 1];

        internal int[] table_select = new int[3];

        internal int[] width = new int[L3Side.SFBMAX];

        internal int[] window = new int[L3Side.SFBMAX];

        internal float[] xr = new float[576];

        internal float xrpow_max;

        internal void assign(GrInfo other)
        {
            Array.Copy(other.xr, xr, other.xr.Length);
            Array.Copy(other.l3_enc, l3_enc, other.l3_enc.Length);
            Array.Copy(other.scalefac, scalefac, other.scalefac.Length);
            xrpow_max = other.xrpow_max;

            part2_3_length = other.part2_3_length;
            big_values = other.big_values;
            count1 = other.count1;
            global_gain = other.global_gain;
            scalefac_compress = other.scalefac_compress;
            block_type = other.block_type;
            mixed_block_flag = other.mixed_block_flag;
            Array.Copy(other.table_select, table_select, other.table_select.Length);
            Array.Copy(other.subblock_gain, subblock_gain, other.subblock_gain.Length);
            region0_count = other.region0_count;
            region1_count = other.region1_count;
            preflag = other.preflag;
            scalefac_scale = other.scalefac_scale;
            count1table_select = other.count1table_select;

            part2_length = other.part2_length;
            sfb_lmax = other.sfb_lmax;
            sfb_smin = other.sfb_smin;
            psy_lmax = other.psy_lmax;
            sfbmax = other.sfbmax;
            psymax = other.psymax;
            sfbdivide = other.sfbdivide;
            Array.Copy(other.width, width, other.width.Length);
            Array.Copy(other.window, window, other.window.Length);
            count1bits = other.count1bits;

			sfb_partition_table = new int[other.sfb_partition_table.Length];
            Array.Copy(other.sfb_partition_table, sfb_partition_table, other.sfb_partition_table.Length);
            Array.Copy(other.slen, slen, other.slen.Length);
            max_nonzero_coeff = other.max_nonzero_coeff;
        }
    }
}