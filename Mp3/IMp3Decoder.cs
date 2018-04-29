using System.IO;

namespace GroovyCodecs.Mp3
{
    public interface IMp3Decoder
    {
        void close();

        void decode(MemoryStream sampleBuffer, bool playOriginal);
    }
}