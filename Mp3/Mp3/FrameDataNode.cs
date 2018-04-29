//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    internal sealed class FrameDataNode
    {

        internal Inf dsc = new Inf(), txt = new Inf();

        /// <summary>
        ///     Frame Identifier
        /// </summary>
        internal int fid;

        /// <summary>
        ///     3-character language descriptor
        /// </summary>
        internal string lng;

        internal FrameDataNode nxt;
    }
}