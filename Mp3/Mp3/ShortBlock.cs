//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    internal enum ShortBlock
    {
        /// <summary>
        ///     LAME may use them, even different block types for L/R.
        /// </summary>
        short_block_allowed,

        /// <summary>
        ///     LAME may use them, but always same block types in L/R.
        /// </summary>
        short_block_coupled,

        /// <summary>
        ///     LAME will not use short blocks, long blocks only.
        /// </summary>
        short_block_dispensed,

        /// <summary>
        ///     LAME will not use long blocks, short blocks only.
        /// </summary>
        short_block_forced
    }

}