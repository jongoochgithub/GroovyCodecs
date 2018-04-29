//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System.Collections.Generic;
using GroovyCodecs.Types;

namespace GroovyCodecs.Mp3.Mp3
{

    internal sealed class ID3TagSpec
    {

        internal string album;

        internal sbyte[] albumart;

        internal MimeType albumart_mimetype;

        internal int albumart_size;

        internal string artist;

        internal string comment;

        internal int flags;

        internal int genre_id3v1;

        internal int num_values;

        internal int padding_size;

        internal string title;

        internal int track_id3v1;

        internal FrameDataNode v2_head, v2_tail;

        internal List<string> values = new List<string>();

        internal int year;
    }
}