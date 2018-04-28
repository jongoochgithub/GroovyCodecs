//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyMp3.Types
{
    public class Mp3Version
    {

        /// <summary>
        ///     Major version number.
        /// </summary>
        private const int LAME_MAJOR_VERSION = 3;

        /// <summary>
        ///     Minor version number.
        /// </summary>
        private const int LAME_MINOR_VERSION = 98;

        /// <summary>
        ///     Patch level.
        /// </summary>
        private const int LAME_PATCH_VERSION = 4;

        /// <summary>
        ///     URL for the LAME website.
        /// </summary>
        private const string LAME_URL = "http://www.mp3dev.org/";

        /// <summary>
        ///     Major version number.
        /// </summary>
        private const int PSY_MAJOR_VERSION = 0;

        /// <summary>
        ///     Minor version number.
        /// </summary>
        private const int PSY_MINOR_VERSION = 93;

        /// <summary>
        ///     A string which describes the version of LAME.
        /// </summary>
        /// <returns> string which describes the version of LAME </returns>
        public string LameVersion => LAME_MAJOR_VERSION + "." + LAME_MINOR_VERSION + "." + LAME_PATCH_VERSION;

        /// <summary>
        ///     The short version of the LAME version string.
        /// </summary>
        /// <returns> short version of the LAME version string </returns>
        public string LameShortVersion => LAME_MAJOR_VERSION + "." + LAME_MINOR_VERSION + "." + LAME_PATCH_VERSION;

        /// <summary>
        ///     The shortest version of the LAME version string.
        /// </summary>
        /// <returns> shortest version of the LAME version string </returns>
        public string LameVeryShortVersion => "LAME" + LAME_MAJOR_VERSION + "." + LAME_MINOR_VERSION + "r";

        /// <summary>
        ///     String which describes the version of GPSYCHO
        /// </summary>
        /// <returns> string which describes the version of GPSYCHO </returns>
        public string PsyVersion => PSY_MAJOR_VERSION + "." + PSY_MINOR_VERSION;

        /// <summary>
        ///     String which is a URL for the LAME website.
        /// </summary>
        /// <returns> string which is a URL for the LAME website </returns>
        public string LameUrl => LAME_URL;

        /// <summary>
        ///     Quite useless for a java version, however we are compatible ;-)
        /// </summary>
        /// <returns> "32bits" </returns>
        public string LameOsBitness => "32bits";
    }

}