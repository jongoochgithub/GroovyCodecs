using System;
using System.IO;
using GroovyCodecs.Types;

namespace GroovyCodecs.WavFile
{
	public class WavReader
	{
		FileStream _fs;
		BinaryReader _reader;
		private int chunkID;
		private int fileSize;
		private int riffType;
		private int fmtID;
		private int fmtSize;
		private short fmtCode;
		private short channels;
		private int sampleRate;
		private int byteRate;
		private short fmtBlockAlign;
		private short bitDepth;
		private short fmtExtraSize;
		private int dataID;
		private int bytes;

		public bool OpenFile(string filename)
		{
			if (!File.Exists(filename))
			{
				return false;
			}

			try
			{
				_fs = File.Open(filename, FileMode.Open);
				_reader = new BinaryReader(_fs);

				// chunk 0
				chunkID = _reader.ReadInt32();
				fileSize = _reader.ReadInt32();
				riffType = _reader.ReadInt32();


				// chunk 1
				fmtID = _reader.ReadInt32();
				fmtSize = _reader.ReadInt32(); // bytes for this chunk
				fmtCode = _reader.ReadInt16();
				channels = _reader.ReadInt16();
				sampleRate = _reader.ReadInt32();
				byteRate = _reader.ReadInt32();
				fmtBlockAlign = _reader.ReadInt16();
				bitDepth = _reader.ReadInt16();

				if (fmtSize == 18)
				{
					// Read any extra values
					fmtExtraSize = _reader.ReadInt16();
					_reader.ReadBytes(fmtExtraSize);
				}

				// chunk 2
				dataID = _reader.ReadInt32();
				bytes = _reader.ReadInt32();

				while (dataID != 0x61746164)
				{
					byte[] byteArray = _reader.ReadBytes(bytes);
					dataID = _reader.ReadInt32();
                    bytes = _reader.ReadInt32();
				}
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}

		public AudioFormat GetFormat()
		{
			return new AudioFormat
			{
				BitsPerSample = bitDepth,

				BlockAlign = fmtBlockAlign,

				Channels = channels,

				SampleRate = sampleRate,

				IsFloatingPoint = fmtCode == 3,
			};
		}


		public byte[] readWav()
		{
			byte[] byteArray = _reader.ReadBytes(bytes);
			_reader.Close();
			_fs.Dispose();

			return byteArray;
		}
	}
}