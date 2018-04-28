using System;
using System.Text;
using GroovyMp3.Types;
using GroovyMp3.Types;

/*  * id3tag.c -- Write ID3 version 1 and 2 tags.  *
 * Copyright (C) 2000 Don Melton.  *
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Library General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.	 See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

/*  * HISTORY: This source file is part of LAME (see http://www.mp3dev.org)
 * and was originally adapted by Conrad Sanderson <c.sanderson@me.gu.edu.au>
 * from mp3info by Ricardo Cerqueira <rmc@rccn.net> to write only ID3 version 1
 * tags.  Don Melton <don@blivet.com> COMPLETELY rewrote it to support version
 * 2 tags and be more conformant to other standards while remaining flexible.
 *
 * NOTE: See http://id3.org/ for more information about ID3 tag formats.
*/
namespace GroovyMp3.Codec.Mp3
{
    internal class ID3Tag
    {

        private static readonly int ADD_V2_FLAG = 1 << 1;

        private static readonly int CHANGED_FLAG = 1 << 0;

        private static readonly int[] genre_alpha_map =
        {
            123,
            34,
            74,
            73,
            99,
            20,
            40,
            26,
            145,
            90,
            116,
            41,
            135,
            85,
            96,
            138,
            89,
            0,
            107,
            132,
            65,
            88,
            104,
            102,
            97,
            136,
            61,
            141,
            32,
            1,
            112,
            128,
            57,
            140,
            2,
            139,
            58,
            3,
            125,
            50,
            22,
            4,
            55,
            127,
            122,
            120,
            98,
            52,
            48,
            54,
            124,
            25,
            84,
            80,
            115,
            81,
            119,
            5,
            30,
            36,
            59,
            126,
            38,
            49,
            91,
            6,
            129,
            79,
            137,
            7,
            35,
            100,
            131,
            19,
            33,
            46,
            47,
            8,
            29,
            146,
            63,
            86,
            71,
            45,
            142,
            9,
            77,
            82,
            64,
            133,
            10,
            66,
            39,
            11,
            103,
            12,
            75,
            134,
            13,
            53,
            62,
            109,
            117,
            23,
            108,
            92,
            67,
            93,
            43,
            121,
            15,
            68,
            14,
            16,
            76,
            87,
            118,
            17,
            78,
            143,
            114,
            110,
            69,
            21,
            111,
            95,
            105,
            42,
            37,
            24,
            56,
            44,
            101,
            83,
            94,
            106,
            147,
            113,
            18,
            51,
            130,
            144,
            60,
            70,
            31,
            72,
            27,
            28
        };

        private static readonly string[] genre_names =
        {
            "Blues",
            "Classic Rock",
            "Country",
            "Dance",
            "Disco",
            "Funk",
            "Grunge",
            "Hip-Hop",
            "Jazz",
            "Metal",
            "New Age",
            "Oldies",
            "Other",
            "Pop",
            "R&B",
            "Rap",
            "Reggae",
            "Rock",
            "Techno",
            "Industrial",
            "Alternative",
            "Ska",
            "Death Metal",
            "Pranks",
            "Soundtrack",
            "Euro-Techno",
            "Ambient",
            "Trip-Hop",
            "Vocal",
            "Jazz+Funk",
            "Fusion",
            "Trance",
            "Classical",
            "Instrumental",
            "Acid",
            "House",
            "Game",
            "Sound Clip",
            "Gospel",
            "Noise",
            "Alternative Rock",
            "Bass",
            "Soul",
            "Punk",
            "Space",
            "Meditative",
            "Instrumental Pop",
            "Instrumental Rock",
            "Ethnic",
            "Gothic",
            "Darkwave",
            "Techno-Industrial",
            "Electronic",
            "Pop-Folk",
            "Eurodance",
            "Dream",
            "Southern Rock",
            "Comedy",
            "Cult",
            "Gangsta",
            "Top 40",
            "Christian Rap",
            "Pop/Funk",
            "Jungle",
            "Native US",
            "Cabaret",
            "New Wave",
            "Psychedelic",
            "Rave",
            "Showtunes",
            "Trailer",
            "Lo-Fi",
            "Tribal",
            "Acid Punk",
            "Acid Jazz",
            "Polka",
            "Retro",
            "Musical",
            "Rock & Roll",
            "Hard Rock",
            "Folk",
            "Folk-Rock",
            "National Folk",
            "Swing",
            "Fast Fusion",
            "Bebob",
            "Latin",
            "Revival",
            "Celtic",
            "Bluegrass",
            "Avantgarde",
            "Gothic Rock",
            "Progressive Rock",
            "Psychedelic Rock",
            "Symphonic Rock",
            "Slow Rock",
            "Big Band",
            "Chorus",
            "Easy Listening",
            "Acoustic",
            "Humour",
            "Speech",
            "Chanson",
            "Opera",
            "Chamber Music",
            "Sonata",
            "Symphony",
            "Booty Bass",
            "Primus",
            "Porn Groove",
            "Satire",
            "Slow Jam",
            "Club",
            "Tango",
            "Samba",
            "Folklore",
            "Ballad",
            "Power Ballad",
            "Rhythmic Soul",
            "Freestyle",
            "Duet",
            "Punk Rock",
            "Drum Solo",
            "A Cappella",
            "Euro-House",
            "Dance Hall",
            "Goa",
            "Drum & Bass",
            "Club-House",
            "Hardcore",
            "Terror",
            "Indie",
            "BritPop",
            "Negerpunk",
            "Polsk Punk",
            "Beat",
            "Christian Gangsta",
            "Heavy Metal",
            "Black Metal",
            "Crossover",
            "Contemporary Christian",
            "Christian Rock",
            "Merengue",
            "Salsa",
            "Thrash Metal",
            "Anime",
            "JPop",
            "SynthPop"
        };

        private static readonly int PAD_V2_FLAG = 1 << 5;

        private static readonly int SPACE_V1_FLAG = 1 << 4;

        private static readonly int V1_ONLY_FLAG = 1 << 2;

        private static readonly int V2_ONLY_FLAG = 1 << 3;

        private static readonly int ID_TITLE = FRAME_ID('T', 'I', 'T', '2');

        private static readonly int ID_ARTIST = FRAME_ID('T', 'P', 'E', '1');

        private static readonly int ID_ALBUM = FRAME_ID('T', 'A', 'L', 'B');

        private static readonly int ID_GENRE = FRAME_ID('T', 'C', 'O', 'N');

        private static readonly int ID_ENCODER = FRAME_ID('T', 'S', 'S', 'E');

        private static readonly int ID_PLAYLENGTH = FRAME_ID('T', 'L', 'E', 'N');

        private static readonly int ID_COMMENT = FRAME_ID('C', 'O', 'M', 'M');

        private static readonly int ID_DATE = FRAME_ID('T', 'D', 'A', 'T');

        private static readonly int ID_TIME = FRAME_ID('T', 'I', 'M', 'E');

        private static readonly int ID_TPOS = FRAME_ID('T', 'P', 'O', 'S');

        private static readonly int ID_TRACK = FRAME_ID('T', 'R', 'C', 'K');

        private static readonly int ID_YEAR = FRAME_ID('T', 'Y', 'E', 'R');

        private static readonly int ID_TXXX = FRAME_ID('T', 'X', 'X', 'X');

        private static readonly int ID_WXXX = FRAME_ID('W', 'X', 'X', 'X');

        private static readonly int ID_SYLT = FRAME_ID('S', 'Y', 'L', 'T');

        private static readonly int ID_APIC = FRAME_ID('A', 'P', 'I', 'C');

        private static readonly int ID_GEOB = FRAME_ID('G', 'E', 'O', 'B');

        private static readonly int ID_PCNT = FRAME_ID('P', 'C', 'N', 'T');

        private static readonly int ID_AENC = FRAME_ID('A', 'E', 'N', 'C');

        private static readonly int ID_LINK = FRAME_ID('L', 'I', 'N', 'K');

        private static readonly int ID_ENCR = FRAME_ID('E', 'N', 'C', 'R');

        private static readonly int ID_GRID = FRAME_ID('G', 'R', 'I', 'D');

        private static readonly int ID_PRIV = FRAME_ID('P', 'R', 'I', 'V');

        private static readonly string ASCII = "US-ASCII";

        private const int GENRE_INDEX_OTHER = 12;

        private const int GENRE_NUM_UNKNOWN = 255;

        private const string mime_gif = "image/gif";

        private const string mime_jpeg = "image/jpeg";

        private const string mime_png = "image/png";

        internal BitStream bits;

        internal Mp3Version ver;

        internal void setModules(BitStream bits, Mp3Version ver)
        {
            this.bits = bits;
            this.ver = ver;
        }

        private static int FRAME_ID(char a, char b, char c, char d)
        {
            return ((a & 0xff) << 24) | ((b & 0xff) << 16) | ((c & 0xff) << 8) | ((d & 0xff) << 0);
        }

        private void copyV1ToV2(LameGlobalFlags gfp, int frame_id, string s)
        {
            var gfc = gfp.internal_flags;
            var flags = gfc.tag_spec.flags;
            id3v2_add_latin1(gfp, frame_id, null, null, s);
            gfc.tag_spec.flags = flags;
        }

        private void id3v2AddLameVersion(LameGlobalFlags gfp)
        {
            string buffer;
            var b = ver.LameOsBitness;
            var v = ver.LameVersion;
            var u = ver.LameUrl;

            var lenb = b.Length;
            if (lenb > 0)
                buffer = string.Format("LAME {0} version {1} ({2})", b, v, u);
            else
                buffer = string.Format("LAME version {0} ({1})", v, u);

            copyV1ToV2(gfp, ID_ENCODER, buffer);
        }

        private void id3v2AddAudioDuration(LameGlobalFlags gfp)
        {
            if (gfp.num_samples != -1)
            {
                string buffer;
                double max_ulong = int.MaxValue;
                double ms = gfp.num_samples;
                long playlength_ms;
                ms *= 1000;
                ms /= gfp.in_samplerate;
                if (ms > int.MaxValue)
                    playlength_ms = (long)max_ulong;
                else if (ms < 0)
                    playlength_ms = 0;
                else
                    playlength_ms = (long)ms;

                buffer = string.Format("{0:D}", playlength_ms);
                copyV1ToV2(gfp, ID_PLAYLENGTH, buffer);
            }
        }

        internal void id3tag_genre_list(GenreListHandler handler)
        {
            if (handler != null)
                for (var i = 0; i < genre_names.Length; ++i)
                    if (i < genre_alpha_map.Length)
                    {
                        var j = genre_alpha_map[i];
                        handler.genre_list_handler(j, genre_names[j]);
                    }
        }

        internal void id3tag_init(LameGlobalFlags gfp)
        {
            var gfc = gfp.internal_flags;
            gfc.tag_spec = new ID3TagSpec();
            gfc.tag_spec.genre_id3v1 = GENRE_NUM_UNKNOWN;
            gfc.tag_spec.padding_size = 128;
            id3v2AddLameVersion(gfp);
        }

        internal void id3tag_add_v2(LameGlobalFlags gfp)
        {
            var gfc = gfp.internal_flags;
            gfc.tag_spec.flags &= ~V1_ONLY_FLAG;
            gfc.tag_spec.flags |= ADD_V2_FLAG;
        }

        internal void id3tag_v1_only(LameGlobalFlags gfp)
        {
            var gfc = gfp.internal_flags;
            gfc.tag_spec.flags &= ~(ADD_V2_FLAG | V2_ONLY_FLAG);
            gfc.tag_spec.flags |= V1_ONLY_FLAG;
        }

        internal void id3tag_v2_only(LameGlobalFlags gfp)
        {
            var gfc = gfp.internal_flags;
            gfc.tag_spec.flags &= ~V1_ONLY_FLAG;
            gfc.tag_spec.flags |= V2_ONLY_FLAG;
        }

        internal void id3tag_space_v1(LameGlobalFlags gfp)
        {
            var gfc = gfp.internal_flags;
            gfc.tag_spec.flags &= ~V2_ONLY_FLAG;
            gfc.tag_spec.flags |= SPACE_V1_FLAG;
        }

        internal void id3tag_pad_v2(LameGlobalFlags gfp)
        {
            id3tag_set_pad(gfp, 128);
        }

        internal void id3tag_set_pad(LameGlobalFlags gfp, int n)
        {
            var gfc = gfp.internal_flags;
            gfc.tag_spec.flags &= ~V1_ONLY_FLAG;
            gfc.tag_spec.flags |= PAD_V2_FLAG;
            gfc.tag_spec.flags |= ADD_V2_FLAG;
            gfc.tag_spec.padding_size = n;
        }

        internal bool id3tag_set_albumart(LameGlobalFlags gfp, byte[] image, int size)
        {
            var mimetype = MimeType.MIMETYPE_NONE;
            var data = image;
            var gfc = gfp.internal_flags;
            if (Lame.LAME_MAXALBUMART < size)
                return false;

            if (2 < size && data[0] == unchecked((sbyte)0xFF) && data[1] == unchecked((sbyte)0xD8))
                mimetype = MimeType.MIMETYPE_JPEG;
            else if (4 < size && data[0] == unchecked((sbyte)0x89) &&
                     Encoding.GetEncoding(ASCII).GetString(data, 1, 3).StartsWith("PNG", StringComparison.Ordinal))
                mimetype = MimeType.MIMETYPE_PNG;
            else if (4 < size && Encoding.GetEncoding(ASCII).GetString(data, 1, 3).StartsWith(
                         "GIF8",
                         StringComparison.Ordinal))
                mimetype = MimeType.MIMETYPE_GIF;
            else
                return false;

            if (gfc.tag_spec.albumart != null)
            {
                gfc.tag_spec.albumart = null;
                gfc.tag_spec.albumart_size = 0;
                gfc.tag_spec.albumart_mimetype = MimeType.MIMETYPE_NONE;
            }

            if (size < 1)
                return true;

            gfc.tag_spec.albumart = new sbyte[size];
            if (gfc.tag_spec.albumart != null)
            {
                Array.Copy(image, 0, gfc.tag_spec.albumart, 0, size);
                gfc.tag_spec.albumart_size = size;
                gfc.tag_spec.albumart_mimetype = mimetype;
                gfc.tag_spec.flags |= CHANGED_FLAG;
                id3tag_add_v2(gfp);
            }

            return true;
        }

        private int set_4_byte_value(sbyte[] bytes, int bytesPos, int value)
        {
            int i;
            for (i = 3; i >= 0; --i)
            {
                bytes[bytesPos + i] = unchecked((sbyte)(value & 0xff));
                value >>= 8;
            }

            return bytesPos + 4;
        }

        private int toID3v2TagId(string s)
        {
            int i, x = 0;
            if (ReferenceEquals(s, null))
                return 0;

            for (i = 0; i < 4 && i < s.Length; ++i)
            {
                var c = s[i];
                var u = 0x0ff & c;
                x <<= 8;
                x |= u;
                if (c < 'A' || 'Z' < c)
                    if (c < '0' || '9' < c)
                        return 0;
            }

            return x;
        }

        private bool isNumericString(int frame_id)
        {
            if (frame_id == ID_DATE || frame_id == ID_TIME || frame_id == ID_TPOS || frame_id == ID_TRACK ||
                frame_id == ID_YEAR)
                return true;

            return false;
        }

        private bool isMultiFrame(int frame_id)
        {
            if (frame_id == ID_TXXX || frame_id == ID_WXXX || frame_id == ID_COMMENT || frame_id == ID_SYLT ||
                frame_id == ID_APIC || frame_id == ID_GEOB || frame_id == ID_PCNT || frame_id == ID_AENC ||
                frame_id == ID_LINK || frame_id == ID_ENCR || frame_id == ID_GRID || frame_id == ID_PRIV)
                return true;

            return false;
        }

        private bool hasUcs2ByteOrderMarker(char bom)
        {
            if (bom == (char)0xFFFE || bom == (char)0xFEFF)
                return true;

            return false;
        }

        private FrameDataNode findNode(ID3TagSpec tag, int frame_id, FrameDataNode last)
        {
            var node = last != null ? last.nxt : tag.v2_head;
            while (node != null)
            {
                if (node.fid == frame_id)
                    return node;

                node = node.nxt;
            }

            return null;
        }

        private void appendNode(ID3TagSpec tag, FrameDataNode node)
        {
            if (tag.v2_tail == null || tag.v2_head == null)
            {
                tag.v2_head = node;
                tag.v2_tail = node;
            }
            else
            {
                tag.v2_tail.nxt = node;
                tag.v2_tail = node;
            }
        }

        private string setLang(string src)
        {
            int i;
            if (ReferenceEquals(src, null) || src.Length == 0)
                return "XXX";

            var dst = new StringBuilder();
            if (!ReferenceEquals(src, null))
                dst.Append(src.Substring(0, 3));

            for (i = dst.Length; i < 3; ++i)
                dst.Append(' ');

            return dst.ToString();
        }

        private bool isSameLang(string l1, string l2)
        {
            var d = setLang(l2);
            for (var i = 0; i < 3; ++i)
            {
                var a = char.ToLower(l1[i]);
                var b = char.ToLower(d[i]);
                if (a < ' ')
                    a = ' ';

                if (b < ' ')
                    b = ' ';

                if (a != b)
                    return false;
            }

            return true;
        }

        private bool isSameDescriptor(FrameDataNode node, string dsc)
        {
            if (node.dsc.enc == 1 && node.dsc.dim > 0)
                return false;

            for (var i = 0; i < node.dsc.dim; ++i)
                if (null == dsc || node.dsc.l[i] != dsc[i])
                    return false;

            return true;
        }

        private bool isSameDescriptorUcs2(FrameDataNode node, string dsc)
        {
            if (node.dsc.enc != 1 && node.dsc.dim > 0)
                return false;

            for (var i = 0; i < node.dsc.dim; ++i)
                if (null == dsc || node.dsc.l[i] != dsc[i])
                    return false;

            return true;
        }

        private void id3v2_add_ucs2(LameGlobalFlags gfp, int frame_id, string lang, string desc, string text)
        {
            var gfc = gfp.internal_flags;
            if (gfc != null)
            {
                var node = findNode(gfc.tag_spec, frame_id, null);
                if (isMultiFrame(frame_id))
                    while (node != null)
                    {
                        if (isSameLang(node.lng, lang))
                            if (isSameDescriptorUcs2(node, desc))
                                break;

                        node = findNode(gfc.tag_spec, frame_id, node);
                    }

                if (node == null)
                {
                    node = new FrameDataNode();
                    appendNode(gfc.tag_spec, node);
                }

                node.fid = frame_id;
                node.lng = setLang(lang);
                node.dsc.l = desc;
                node.dsc.dim = !ReferenceEquals(desc, null) ? desc.Length : 0;
                node.dsc.enc = 1;
                node.txt.l = text;
                node.txt.dim = !ReferenceEquals(text, null) ? text.Length : 0;
                node.txt.enc = 1;
                gfc.tag_spec.flags |= CHANGED_FLAG | ADD_V2_FLAG;
            }
        }

        private void id3v2_add_latin1(LameGlobalFlags gfp, int frame_id, string lang, string desc, string text)
        {
            var gfc = gfp.internal_flags;
            if (gfc != null)
            {
                FrameDataNode node;
                node = findNode(gfc.tag_spec, frame_id, null);
                if (isMultiFrame(frame_id))
                    while (node != null)
                    {
                        if (isSameLang(node.lng, lang))
                            if (isSameDescriptor(node, desc))
                                break;

                        node = findNode(gfc.tag_spec, frame_id, node);
                    }

                if (node == null)
                {
                    node = new FrameDataNode();
                    appendNode(gfc.tag_spec, node);
                }

                node.fid = frame_id;
                node.lng = setLang(lang);
                node.dsc.l = desc;
                node.dsc.dim = !ReferenceEquals(desc, null) ? desc.Length : 0;
                node.dsc.enc = 0;
                node.txt.l = text;
                node.txt.dim = !ReferenceEquals(text, null) ? text.Length : 0;
                node.txt.enc = 0;
                gfc.tag_spec.flags |= CHANGED_FLAG | ADD_V2_FLAG;
            }
        }

        internal int id3tag_set_textinfo_ucs2(LameGlobalFlags gfp, string id, string text)
        {
            long t_mask = FRAME_ID('T', (char)0, (char)0, (char)0);
            var frame_id = toID3v2TagId(id);
            if (frame_id == 0)
                return -1;

            if ((frame_id & t_mask) == t_mask)
            {
                if (isNumericString(frame_id))
                    return -2;

                if (ReferenceEquals(text, null))
                    return 0;

                if (!hasUcs2ByteOrderMarker(text[0]))
                    return -3;

                if (gfp != null)
                {
                    id3v2_add_ucs2(gfp, frame_id, null, null, text);
                    return 0;
                }
            }

            return -255;
        }

        private int id3tag_set_textinfo_latin1(LameGlobalFlags gfp, string id, string text)
        {
            long t_mask = FRAME_ID('T', (char)0, (char)0, (char)0);
            var frame_id = toID3v2TagId(id);
            if (frame_id == 0)
                return -1;

            if ((frame_id & t_mask) == t_mask)
            {
                if (ReferenceEquals(text, null))
                    return 0;

                if (gfp != null)
                {
                    id3v2_add_latin1(gfp, frame_id, null, null, text);
                    return 0;
                }
            }

            return -255;
        }

        internal int id3tag_set_comment(LameGlobalFlags gfp, string lang, string desc, string text, int textPos)
        {
            if (gfp != null)
            {
                id3v2_add_latin1(gfp, ID_COMMENT, lang, desc, text);
                return 0;
            }

            return -255;
        }

        internal void id3tag_set_title(LameGlobalFlags gfp, string title)
        {
            var gfc = gfp.internal_flags;
            if (!ReferenceEquals(title, null) && title.Length != 0)
            {
                gfc.tag_spec.title = title;
                gfc.tag_spec.flags |= CHANGED_FLAG;
                copyV1ToV2(gfp, ID_TITLE, title);
            }
        }

        internal void id3tag_set_artist(LameGlobalFlags gfp, string artist)
        {
            var gfc = gfp.internal_flags;
            if (!ReferenceEquals(artist, null) && artist.Length != 0)
            {
                gfc.tag_spec.artist = artist;
                gfc.tag_spec.flags |= CHANGED_FLAG;
                copyV1ToV2(gfp, ID_ARTIST, artist);
            }
        }

        internal void id3tag_set_album(LameGlobalFlags gfp, string album)
        {
            var gfc = gfp.internal_flags;
            if (!ReferenceEquals(album, null) && album.Length != 0)
            {
                gfc.tag_spec.album = album;
                gfc.tag_spec.flags |= CHANGED_FLAG;
                copyV1ToV2(gfp, ID_ALBUM, album);
            }
        }

        internal void id3tag_set_year(LameGlobalFlags gfp, string year)
        {
            var gfc = gfp.internal_flags;
            if (!ReferenceEquals(year, null) && year.Length != 0)
            {
                var num = Convert.ToInt32(year);
                if (num < 0)
                    num = 0;

                if (num > 9999)
                    num = 9999;

                if (num != 0)
                {
                    gfc.tag_spec.year = num;
                    gfc.tag_spec.flags |= CHANGED_FLAG;
                }

                copyV1ToV2(gfp, ID_YEAR, year);
            }
        }

        internal void id3tag_set_comment(LameGlobalFlags gfp, string comment)
        {
            var gfc = gfp.internal_flags;
            if (!ReferenceEquals(comment, null) && comment.Length != 0)
            {
                gfc.tag_spec.comment = comment;
                gfc.tag_spec.flags |= CHANGED_FLAG;
                {
                    var flags = gfc.tag_spec.flags;
                    id3v2_add_latin1(gfp, ID_COMMENT, "XXX", "", comment);
                    gfc.tag_spec.flags = flags;
                }
            }
        }

        internal int id3tag_set_track(LameGlobalFlags gfp, string track)
        {
            var gfc = gfp.internal_flags;
            var ret = 0;
            if (!ReferenceEquals(track, null) && track.Length != 0)
            {
                var trackcount = track.IndexOf('/');
                int num;
                if (trackcount != -1)
                    num = int.Parse(track.Substring(0, trackcount));
                else
                    num = int.Parse(track);

                if (num < 1 || num > 255)
                {
                    num = 0;
                    ret = -1;
                    gfc.tag_spec.flags |= CHANGED_FLAG | ADD_V2_FLAG;
                }

                if (num != 0)
                {
                    gfc.tag_spec.track_id3v1 = num;
                    gfc.tag_spec.flags |= CHANGED_FLAG;
                }

                if (trackcount != -1)
                    gfc.tag_spec.flags |= CHANGED_FLAG | ADD_V2_FLAG;

                copyV1ToV2(gfp, ID_TRACK, track);
            }

            return ret;
        }

        private int nextUpperAlpha(string p, int pPos, char x)
        {
            for (var c = char.ToUpper(p[pPos]); pPos < p.Length; c = char.ToUpper(p[pPos++]))
                if ('A' <= c && c <= 'Z')
                    if (c != x)
                        return pPos;

            return pPos;
        }

        private bool sloppyCompared(string p, string q)
        {
            var pPos = nextUpperAlpha(p, 0, (char)0);
            var qPos = nextUpperAlpha(q, 0, (char)0);
            var cp = pPos < p.Length ? char.ToUpper(p[pPos]) : (char)0;
            var cq = char.ToUpper(q[qPos]);
            while (cp == cq)
            {
                if (cp == (char)0)
                    return true;

                if (p[1] == '.')
                    while (qPos < q.Length && q[qPos++] != ' ')
                    {
                    }

                pPos = nextUpperAlpha(p, pPos, cp);
                qPos = nextUpperAlpha(q, qPos, cq);
                cp = pPos < p.Length ? char.ToUpper(p[pPos]) : (char)0;
                cq = char.ToUpper(q[qPos]);
            }

            return false;
        }

        private int sloppySearchGenre(string genre)
        {
            for (var i = 0; i < genre_names.Length; ++i)
                if (sloppyCompared(genre, genre_names[i]))
                    return i;

            return genre_names.Length;
        }

        private int searchGenre(string genre)
        {
            for (var i = 0; i < genre_names.Length; ++i)
                if (genre_names[i].Equals(genre))
                    return i;

            return genre_names.Length;
        }

        internal int id3tag_set_genre(LameGlobalFlags gfp, string genre)
        {
            var gfc = gfp.internal_flags;
            var ret = 0;
            if (!ReferenceEquals(genre, null) && genre.Length != 0)
            {
                int num;
                try
                {
                    num = int.Parse(genre);
                    if (num < 0 || num >= genre_names.Length)
                        return -1;

                    genre = genre_names[num];
                }
                catch (FormatException)
                {
                    num = searchGenre(genre);
                    if (num == genre_names.Length)
                        num = sloppySearchGenre(genre);

                    if (num == genre_names.Length)
                    {
                        num = GENRE_INDEX_OTHER;
                        ret = -2;
                    }
                    else
                    {
                        genre = genre_names[num];
                    }
                }

                gfc.tag_spec.genre_id3v1 = num;
                gfc.tag_spec.flags |= CHANGED_FLAG;
                if (ret != 0)
                    gfc.tag_spec.flags |= ADD_V2_FLAG;

                copyV1ToV2(gfp, ID_GENRE, genre);
            }

            return ret;
        }

        private int set_frame_custom(sbyte[] frame, int framePos, char[] fieldvalue)
        {
            if (fieldvalue != null && fieldvalue[0] != (char)0)
            {
                var value = 5;
                var length = new string(fieldvalue, value, fieldvalue.Length - value).Length;
                frame[framePos++] = (sbyte)fieldvalue[0];
                frame[framePos++] = (sbyte)fieldvalue[1];
                frame[framePos++] = (sbyte)fieldvalue[2];
                frame[framePos++] = (sbyte)fieldvalue[3];
                framePos = set_4_byte_value(
                    frame,
                    value,
                    new string(fieldvalue, value, fieldvalue.Length - value).Length + 1);
                frame[framePos++] = 0;
                frame[framePos++] = 0;
                frame[framePos++] = 0;
                while (length-- != 0)
                    frame[framePos++] = (sbyte)fieldvalue[value++];
            }

            return framePos;
        }

        private int sizeOfNode(FrameDataNode node)
        {
            var n = 0;
            if (node != null)
            {
                n = 10;
                n += 1;
                switch (node.txt.enc)
                {
                    default:
                        goto case 0;
                    case 0:
                        n += node.txt.dim;
                        break;
                    case 1:
                        n += node.txt.dim * 2;
                        break;
                }
            }

            return n;
        }

        private int sizeOfCommentNode(FrameDataNode node)
        {
            var n = 0;
            if (node != null)
            {
                n = 10;
                n += 1;
                n += 3;
                switch (node.dsc.enc)
                {
                    default:
                        goto case 0;
                    case 0:
                        n += 1 + node.dsc.dim;
                        break;
                    case 1:
                        n += 2 + node.dsc.dim * 2;
                        break;
                }

                switch (node.txt.enc)
                {
                    default:
                        goto case 0;
                    case 0:
                        n += node.txt.dim;
                        break;
                    case 1:
                        n += node.txt.dim * 2;
                        break;
                }
            }

            return n;
        }

        private int writeChars(sbyte[] frame, int framePos, string str, int strPos, int n)
        {
            while (n-- != 0)
                frame[framePos++] = (sbyte)str[strPos++];

            return framePos;
        }

        private int writeUcs2s(sbyte[] frame, int framePos, string str, int strPos, int n)
        {
            while (n-- != 0)
            {
                frame[framePos++] = unchecked((sbyte)(0xff & (str[strPos] >> 8)));
                frame[framePos++] = unchecked((sbyte)(0xff & str[strPos++]));
            }

            return framePos;
        }

        private int set_frame_comment(sbyte[] frame, int framePos, FrameDataNode node)
        {
            var n = sizeOfCommentNode(node);
            if (n > 10)
            {
                framePos = set_4_byte_value(frame, framePos, ID_COMMENT);
                framePos = set_4_byte_value(frame, framePos, n - 10);
                frame[framePos++] = 0;
                frame[framePos++] = 0;
                frame[framePos++] = node.txt.enc == 1 ? (sbyte)1 : (sbyte)0;
                frame[framePos++] = (sbyte)node.lng[0];
                frame[framePos++] = (sbyte)node.lng[1];
                frame[framePos++] = (sbyte)node.lng[2];
                if (node.dsc.enc != 1)
                {
                    framePos = writeChars(frame, framePos, node.dsc.l, 0, node.dsc.dim);
                    frame[framePos++] = 0;
                }
                else
                {
                    framePos = writeUcs2s(frame, framePos, node.dsc.l, 0, node.dsc.dim);
                    frame[framePos++] = 0;
                    frame[framePos++] = 0;
                }

                if (node.txt.enc != 1)
                    framePos = writeChars(frame, framePos, node.txt.l, 0, node.txt.dim);
                else
                    framePos = writeUcs2s(frame, framePos, node.txt.l, 0, node.txt.dim);
            }

            return framePos;
        }

        private int set_frame_custom2(sbyte[] frame, int framePos, FrameDataNode node)
        {
            var n = sizeOfNode(node);
            if (n > 10)
            {
                framePos = set_4_byte_value(frame, framePos, node.fid);
                framePos = set_4_byte_value(frame, framePos, n - 10);
                frame[framePos++] = 0;
                frame[framePos++] = 0;
                frame[framePos++] = node.txt.enc == 1 ? (sbyte)1 : (sbyte)0;
                if (node.txt.enc != 1)
                    framePos = writeChars(frame, framePos, node.txt.l, 0, node.txt.dim);
                else
                    framePos = writeUcs2s(frame, framePos, node.txt.l, 0, node.txt.dim);
            }

            return framePos;
        }

        private int set_frame_apic(sbyte[] frame, int framePos, char[] mimetype, sbyte[] data, int size)
        {
            if (mimetype != null && data != null && size != 0)
            {
                framePos = set_4_byte_value(frame, framePos, FRAME_ID('A', 'P', 'I', 'C'));
                framePos = set_4_byte_value(frame, framePos, 4 + mimetype.Length + size);
                frame[framePos++] = 0;
                frame[framePos++] = 0;
                frame[framePos++] = 0;
                var mimetypePos = 0;
                while (mimetypePos < mimetype.Length)
                    frame[framePos++] = (sbyte)mimetype[mimetypePos++];

                frame[framePos++] = 0;
                frame[framePos++] = 0;
                frame[framePos++] = 0;
                var dataPos = 0;
                while (size-- != 0)
                    frame[framePos++] = data[dataPos++];
            }

            return framePos;
        }

        internal int id3tag_set_fieldvalue(LameGlobalFlags gfp, string fieldvalue)
        {
            var gfc = gfp.internal_flags;
            if (!ReferenceEquals(fieldvalue, null) && fieldvalue.Length != 0)
            {
                var frame_id = toID3v2TagId(fieldvalue);
                if (fieldvalue.Length < 5 || fieldvalue[4] != '=')
                    return -1;

                if (frame_id != 0)
                    if (id3tag_set_textinfo_latin1(gfp, fieldvalue, fieldvalue.Substring(5)) != 0)
                    {
                        gfc.tag_spec.values.Add(fieldvalue);
                        gfc.tag_spec.num_values++;
                    }

                gfc.tag_spec.flags |= CHANGED_FLAG;
            }

            id3tag_add_v2(gfp);
            return 0;
        }

        internal int lame_get_id3v2_tag(LameGlobalFlags gfp, sbyte[] buffer, int size)
        {
            LameInternalFlags gfc;
            if (gfp == null)
                return 0;

            gfc = gfp.internal_flags;
            if (gfc == null)
                return 0;

            if ((gfc.tag_spec.flags & V1_ONLY_FLAG) != 0)
                return 0;

            {
                var title_length = gfc.tag_spec.title != null ? gfc.tag_spec.title.Length : 0;
                var artist_length = gfc.tag_spec.artist != null ? gfc.tag_spec.artist.Length : 0;
                var album_length = gfc.tag_spec.album != null ? gfc.tag_spec.album.Length : 0;
                var comment_length = gfc.tag_spec.comment != null ? gfc.tag_spec.comment.Length : 0;
                if ((gfc.tag_spec.flags & (ADD_V2_FLAG | V2_ONLY_FLAG)) != 0 || title_length > 30 ||
                    artist_length > 30 || album_length > 30 || comment_length > 30 ||
                    gfc.tag_spec.track_id3v1 != 0 && comment_length > 28)
                {
                    int tag_size;
                    int p;
                    int adjusted_tag_size;
                    int i;
                    string albumart_mime = null;
                    id3v2AddAudioDuration(gfp);
                    tag_size = 10;
                    for (i = 0; i < gfc.tag_spec.num_values; ++i)
                        tag_size += 6 + gfc.tag_spec.values[i].Length;

                    if (gfc.tag_spec.albumart != null && gfc.tag_spec.albumart_size != 0)
                    {
                        switch (gfc.tag_spec.albumart_mimetype)
                        {
                            case MimeType.MIMETYPE_JPEG:
                                albumart_mime = mime_jpeg;
                                break;
                            case MimeType.MIMETYPE_PNG:
                                albumart_mime = mime_png;
                                break;
                            case MimeType.MIMETYPE_GIF:
                                albumart_mime = mime_gif;
                                break;
                        }

                        if (!ReferenceEquals(albumart_mime, null))
                            tag_size += 10 + 4 + albumart_mime.Length + gfc.tag_spec.albumart_size;
                    }

                    {
                        var tag = gfc.tag_spec;
                        if (tag.v2_head != null)
                        {
                            FrameDataNode node;
                            for (node = tag.v2_head; node != null; node = node.nxt)
                                if (node.fid == ID_COMMENT)
                                    tag_size += sizeOfCommentNode(node);
                                else
                                    tag_size += sizeOfNode(node);
                        }
                    }
                    if ((gfc.tag_spec.flags & PAD_V2_FLAG) != 0)
                        tag_size += gfc.tag_spec.padding_size;

                    if (size < tag_size)
                        return tag_size;

                    if (buffer == null)
                        return 0;

                    p = 0;
                    buffer[p++] = (sbyte)'I';
                    buffer[p++] = (sbyte)'D';
                    buffer[p++] = (sbyte)'3';
                    buffer[p++] = 3;
                    buffer[p++] = 0;
                    buffer[p++] = 0;
                    adjusted_tag_size = tag_size - 10;
                    buffer[p++] = (sbyte)((adjusted_tag_size >> 21) & 0x7f);
                    buffer[p++] = (sbyte)((adjusted_tag_size >> 14) & 0x7f);
                    buffer[p++] = (sbyte)((adjusted_tag_size >> 7) & 0x7f);
                    buffer[p++] = (sbyte)(adjusted_tag_size & 0x7f);
                    {
                        var tag = gfc.tag_spec;
                        if (tag.v2_head != null)
                        {
                            FrameDataNode node;
                            for (node = tag.v2_head; node != null; node = node.nxt)
                                if (node.fid == ID_COMMENT)
                                    p = set_frame_comment(buffer, p, node);
                                else
                                    p = set_frame_custom2(buffer, p, node);
                        }
                    }
                    for (i = 0; i < gfc.tag_spec.num_values; ++i)
                        p = set_frame_custom(buffer, p, gfc.tag_spec.values[i].ToCharArray());

                    if (!ReferenceEquals(albumart_mime, null))
                        p = set_frame_apic(
                            buffer,
                            p,
                            albumart_mime.ToCharArray(),
                            gfc.tag_spec.albumart,
                            gfc.tag_spec.albumart_size);

                    Arrays.Fill(buffer, p, tag_size, (sbyte)0);
                    return tag_size;
                }
            }
            return 0;
        }

        internal int id3tag_write_v2(LameGlobalFlags gfp)
        {
            var gfc = gfp.internal_flags;
            if ((gfc.tag_spec.flags & CHANGED_FLAG) != 0 && 0 == (gfc.tag_spec.flags & V1_ONLY_FLAG))
            {
                sbyte[] tag = null;
                int tag_size, n;
                n = lame_get_id3v2_tag(gfp, null, 0);
                tag = new sbyte[n];
                tag_size = lame_get_id3v2_tag(gfp, tag, n);
                if (tag_size > n)
                    return -1;
                for (var i = 0; i < tag_size; ++i)
                    bits.add_dummy_byte(gfp, tag[i] & 0xff, 1);

                return tag_size;
            }

            return 0;
        }

        private int set_text_field(sbyte[] field, int fieldPos, string text, int size, int pad)
        {
            var textPos = 0;
            while (size-- != 0)
                if (!ReferenceEquals(text, null) && textPos < text.Length)
                    field[fieldPos++] = (sbyte)text[textPos++];
                else
                    field[fieldPos++] = (sbyte)pad;

            return fieldPos;
        }

        internal int lame_get_id3v1_tag(LameGlobalFlags gfp, sbyte[] buffer, int size)
        {
            var tag_size = 128;
            LameInternalFlags gfc;
            if (gfp == null)
                return 0;

            if (size < tag_size)
                return tag_size;

            gfc = gfp.internal_flags;
            if (gfc == null)
                return 0;

            if (buffer == null)
                return 0;

            if ((gfc.tag_spec.flags & CHANGED_FLAG) != 0 && 0 == (gfc.tag_spec.flags & V2_ONLY_FLAG))
            {
                var p = 0;
                var pad = (gfc.tag_spec.flags & SPACE_V1_FLAG) != 0 ? ' ' : 0;
                string year;
                buffer[p++] = (sbyte)'T';
                buffer[p++] = (sbyte)'A';
                buffer[p++] = (sbyte)'G';
                p = set_text_field(buffer, p, gfc.tag_spec.title, 30, pad);
                p = set_text_field(buffer, p, gfc.tag_spec.artist, 30, pad);
                p = set_text_field(buffer, p, gfc.tag_spec.album, 30, pad);
                year = string.Format("{0:D}", Convert.ToInt32(gfc.tag_spec.year));
                p = set_text_field(buffer, p, gfc.tag_spec.year != 0 ? year : null, 4, pad);
                p = set_text_field(buffer, p, gfc.tag_spec.comment, gfc.tag_spec.track_id3v1 != 0 ? 28 : 30, pad);
                if (gfc.tag_spec.track_id3v1 != 0)
                {
                    buffer[p++] = 0;
                    buffer[p++] = (sbyte)gfc.tag_spec.track_id3v1;
                }

                buffer[p++] = (sbyte)gfc.tag_spec.genre_id3v1;
                return tag_size;
            }

            return 0;
        }

        internal int id3tag_write_v1(LameGlobalFlags gfp)
        {
            var tag = new sbyte[128];
            var m = tag.Length;
            var n = lame_get_id3v1_tag(gfp, tag, m);
            if (n > m)
                return 0;

            for (var i = 0; i < n; ++i)
                bits.add_dummy_byte(gfp, tag[i] & 0xff, 1);

            return n;
        }
    }
}