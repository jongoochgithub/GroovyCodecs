//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    internal class HuffCodeTab
    {

        /// <summary>
        ///     pointer to array[xlen][ylen]
        /// </summary>
        internal readonly int[] hlen;

        /// <summary>
        ///     max number to be stored in linbits
        /// </summary>
        internal readonly int linmax;

        /// <summary>
        ///     pointer to array[xlen][ylen]
        /// </summary>
        internal readonly int[] table;

        /// <summary>
        ///     max. x-index+
        /// </summary>
        internal readonly int xlen;

        internal HuffCodeTab(int len, int max, int[] tab, int[] hl)
        {
            xlen = len;
            linmax = max;
            table = tab;
            hlen = hl;
        }
    }
}