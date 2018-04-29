//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    internal class VBRSeekInfo
    {

        /// <summary>
        ///     Pointer to our bag.
        /// </summary>
        internal int[] bag;

        internal int nBytesWritten;

        internal int nVbrNumFrames;

        /// <summary>
        ///     Actual position in our bag.
        /// </summary>
        internal int pos;

        /// <summary>
        ///     How many frames we have seen in this chunk.
        /// </summary>
        internal int seen;

        /// <summary>
        ///     Size of our bag.
        /// </summary>
        internal int size;

        /// <summary>
        ///     What we have seen so far.
        /// </summary>
        internal int sum;

        /* VBR tag data */
        internal int TotalFrameSize;

        /// <summary>
        ///     How many frames we want to collect into one chunk.
        /// </summary>
        internal int want;
    }
}