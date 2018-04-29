//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    /// <summary>
    ///     Structure to receive extracted header (toc may be null).
    ///     @author Ken
    /// </summary>
    internal class VBRTagData
    {

        /// <summary>
        ///     Total bit stream bytes from Vbr header data.
        /// </summary>
        protected internal int bytes;

        /// <summary>
        ///     Encoder delay.
        /// </summary>
        internal int encDelay;

        /// <summary>
        ///     Encoder padding added at end of stream.
        /// </summary>
        internal int encPadding;

        /// <summary>
        ///     From Vbr header data.
        /// </summary>
        protected internal int flags;

        /// <summary>
        ///     Total bit stream frames from Vbr header data.
        /// </summary>
        internal int frames;

        /// <summary>
        ///     Size of VBR header, in bytes.
        /// </summary>
        internal int headersize;

        /// <summary>
        ///     From MPEG header 0=MPEG2, 1=MPEG1.
        /// </summary>
        protected internal int hId;

        /// <summary>
        ///     Sample rate determined from MPEG header.
        /// </summary>
        protected internal int samprate;

        /// <summary>
        ///     May be null if toc not desired.
        /// </summary>
        protected internal byte[] toc = new byte[VBRTag.NUMTOCENTRIES];

        /// <summary>
        ///     Encoded vbr scale from Vbr header data.
        /// </summary>
        protected internal int vbrScale;
    }
}