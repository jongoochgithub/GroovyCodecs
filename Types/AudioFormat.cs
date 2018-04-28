using System.Collections.Generic;

namespace GroovyMp3.Types
{
    public class AudioFormat
    {
        /// <summary>for buffer estimation</summary>
        public int AverageBytesPerSecond { get; set; }

        public bool BigEndian { get; set; }

        /// <summary>number of bits per sample of mono data</summary>
        public short BitsPerSample { get; set; }

        /// <summary>block size of data</summary>
        public short BlockAlign { get; set; }

        /// <summary>number of channels</summary>
        public short Channels { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        /// <summary>sample rate</summary>
        public int SampleRate { get; set; }

        public bool IsFloatingPoint { get; set; }
    }
}