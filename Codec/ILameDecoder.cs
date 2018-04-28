using System.IO;

namespace GroovyMp3.Codec
{
    public interface ILameDecoder
    {
        void close();

        void decode(MemoryStream sampleBuffer, bool playOriginal);
    }
}