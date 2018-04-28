using GroovyMp3.Types;

namespace GroovyMp3.Codec
{
    public interface ILameEncoder
    {
        int EffectiveBitRate { get; }

        int EffectiveChannelMode { get; }

        AudioFormat EffectiveFormat { get; }

        int EffectiveQuality { get; }

        int EffectiveSampleRate { get; }

        bool EffectiveVBR { get; }

        string EncoderVersion { get; }

        int InputBufferSize { get; }

        int MP3BufferSize { get; }

        int OutputBufferSize { get; }

        int PCMBufferSize { get; }

        AudioFormat SourceFormat { get; set; }

        AudioFormat TargetFormat { get; set; }

        void Close();

        float ConvertByteArrayToFloat(byte[] bytes, int offset, ByteOrder byteOrder);

        int EncodeBuffer(byte[] pcm, int offset, int length, byte[] encoded);

        int EncodeFinish(byte[] encoded);

        void SetFormat(AudioFormat sourceFormat, AudioFormat targetFormat);
    }
}