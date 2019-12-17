namespace whateverthefuck.src.util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using OpenTK;
    using OpenTK.Audio;
    using OpenTK.Audio.OpenAL;

    public static class Boombox
    {
        public enum SoundEffects
        {
            soundeffect1,
            soundeffect2,
            soundeffect3,
            soundeffect4,
            soundeffect5,
            soundeffect6,
        }

        public enum Songs
        {
            Song1,
            Song2,
            Africa,
        }

        static Dictionary<SoundEffects, AudioInfo> SoundEffectToSourceIDDictionary = new Dictionary<SoundEffects, AudioInfo>();
        static Dictionary<Songs, string> SongsToResourceNameDictionary = new Dictionary<Songs, string>();
        public static Dictionary<AudioInfo, Songs> CurrentlyPlayingSongs = new Dictionary<AudioInfo, Songs>();

        public static void Init()
        {
            IntPtr device = Alc.OpenDevice(AvailableDevices[0]); // My default device. 
            ContextHandle context = Alc.CreateContext(device, new[] {-1});
            Alc.MakeContextCurrent(context);
            CurrentlyPlayingSongs = new Dictionary<AudioInfo, Songs>();

            AL.Listener(ALListener3f.Position, 0, 0, 1); //position listener in a Z-direction away from sources for 3d sound not to switch directions instantly.

            var soundEffectResources = SetupSoundEffectResourceDictionary();
            SongsToResourceNameDictionary = SetupSongResourceDictionary();
            LoadSoundEffects(soundEffectResources);

            //Play(SoundEffects.soundeffect1);
            //Play(Songs.Africa);
        }

        public static void Play(SoundEffects soundEffect)
        {
            try
            {
                AL.SourcePlay(SoundEffectToSourceIDDictionary[soundEffect].SourceId);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Play(Songs song, float x = 0, float y = 0)
        {
            try
            {
                WaveInfo waveInfo = ReadWavFile(Path.Combine(System.IO.Path.GetFullPath(@"..\..\"), "Resources\\") + SongsToResourceNameDictionary[song] + ".wav"); // Should probably already be .wav when it gets in here.

                AL.GenBuffers(1, out int bufferID);
                AL.GenSources(1, out int sourceId);

                AL.BufferData(bufferID, ALFormat.Mono16, waveInfo.ByteData, waveInfo.ByteData.Length, waveInfo.SampleRate);

                AL.Source(sourceId, ALSourcei.Buffer, bufferID); // Bind source to buffer

                AL.Source(sourceId, ALSource3f.Position, x, y, 0);

                System.Timers.Timer disposeTimer = new System.Timers.Timer(waveInfo.NumberOfSeconds * 1000);

                var audioInfo = new AudioInfo(sourceId, bufferID);

                disposeTimer.Elapsed += (sender, args) =>
                {
                    DisposeBuffer(bufferID, sourceId);
                    CurrentlyPlayingSongs.Remove(audioInfo);
                    disposeTimer.Dispose();
                };
                CurrentlyPlayingSongs.Add(audioInfo, song);
                AL.SourcePlay(sourceId);
                disposeTimer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void LoadSoundEffects(Dictionary<SoundEffects, string> resourceDictionary)
        {
            foreach (var resource in resourceDictionary)
            {

                WaveInfo waveInfo = ReadWavFile(Path.Combine(System.IO.Path.GetFullPath(@"..\..\"), "Resources\\") + resource.Value + ".wav"); // Should probably already be .wav when it gets in here.

                AL.GenBuffers(1, out int bufferId);
                AL.GenSources(1, out int sourceId);

                AL.BufferData(bufferId, ALFormat.Stereo16, waveInfo.ByteData, waveInfo.ByteData.Length, waveInfo.SampleRate);

                AL.Source(sourceId, ALSourcei.Buffer, bufferId); // Bind source to buffer

                SoundEffectToSourceIDDictionary[resource.Key] = new AudioInfo(sourceId, bufferId);

                /*System.Timers.Timer disposeTimer = new System.Timers.Timer(1000);
                disposeTimer.Elapsed += (sender, args) =>
                {
                    DisposeBuffer(resource.Key);
                    disposeTimer.Dispose();
                };
                disposeTimer.Start();*/

            }
        }


        public static void SetListenerPosition(float x, float y)
        {
            AL.Listener(ALListener3f.Position,  x,  y, 1); //position listener in a Z-direction away from sources for 3d sound not to switch directions instantly.
        }

        private static void DisposeBuffer(int bufferID, int sourceID)
        {
            Console.WriteLine("Song finished playing.");
            AL.DeleteSource(sourceID);
            AL.DeleteBuffer(bufferID);
        }

        private static void DisposeBuffer(SoundEffects soundEffect)
        {
            Console.WriteLine("Sound effect finished playing.");
            throw new NotImplementedException();
        }

        private static Dictionary<SoundEffects, string> SetupSoundEffectResourceDictionary()
        {
            return new Dictionary<SoundEffects, string>()
            {
                { SoundEffects.soundeffect1, nameof(Properties.Resources.soundeffect1) },
                { SoundEffects.soundeffect2, nameof(Properties.Resources.soundeffect3) },
            };
        }

        private static Dictionary<Songs, string> SetupSongResourceDictionary()
        {
            return new Dictionary<Songs, string>()
            {
                { Songs.Song1, nameof(Properties.Resources.piano) },
                { Songs.Africa, nameof(Properties.Resources.africatoto)},
            };
        }


        public static void SetVolume(int sourceID, int percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                Console.WriteLine("Percentage is not percentage but I got you!");
                if (percentage < 0) percentage = 0;
                else percentage = 100;
            }
            Console.WriteLine("Setting volume of source to: "+ percentage);
            AL.Source(sourceID, ALSourcef.Gain, (float)percentage/100);
        }

        public static void FadeOut()
        {
            throw new NotImplementedException();
        }

        public static void FadeIn()
        {
            throw new NotImplementedException();
            /*float initialGain = 0;
            AL.Source(source, ALSourcef.Gain, 0.01f);
            AL.GetSource(source, ALSourcef.Gain, out initialGain);
            Console.WriteLine(initialGain);
            Console.WriteLine(initialGain);
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(100);
                initialGain += 0.01f;
                AL.Source(source, ALSourcef.Gain, initialGain);
                Console.WriteLine(initialGain);
            }*/
        }

        public static WaveInfo ReadWavFile(string filename)
        {
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    WaveInfo waveInfo = new WaveInfo();

                    waveInfo.ChunkId = reader.ReadInt32();
                    waveInfo.FileSize = reader.ReadInt32();
                    waveInfo.RiffType = reader.ReadInt32();
                    waveInfo.FmtId = reader.ReadInt32();
                    waveInfo.FmtSize = reader.ReadInt32();
                    waveInfo.FmtCode = reader.ReadInt16();
                    waveInfo.Channels = reader.ReadInt16();
                    waveInfo.SampleRate = reader.ReadInt32();
                    waveInfo.ByteRate = reader.ReadInt32();
                    waveInfo.FmtBlockAlign = reader.ReadInt16();
                    waveInfo.BitDepth = reader.ReadInt16();

                    if (waveInfo.FmtSize == 18)
                    {
                        // Read any extra values
                        int fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }


                    // chunk 2 -- HERE'S THE NEW STUFF (ignore these subchunks, I don't know what they are!)
                    int bytes;
                    while (new string(reader.ReadChars(4)) != "data")
                    {
                        bytes = reader.ReadInt32();
                        reader.ReadBytes(bytes);
                    }

                    // DATA!
                    bytes = reader.ReadInt32();
                    waveInfo.ByteData = reader.ReadBytes(bytes);

                    int bytesForSamp = waveInfo.BitDepth / 8;
                    int samps = bytes / bytesForSamp;

                    float[] asFloat = null;
                    switch (waveInfo.BitDepth)
                    {
                        case 64:
                            double[] asDouble = new double[samps];
                            Buffer.BlockCopy(waveInfo.ByteData, 0, asDouble, 0, bytes);
                            asFloat = Array.ConvertAll(asDouble, e => (float)e);
                            break;
                        case 32:
                            asFloat = new float[samps];
                            Buffer.BlockCopy(waveInfo.ByteData, 0, asFloat, 0, bytes);
                            break;
                        case 16:
                            Int16[] asInt16 = new Int16[samps];
                            System.Buffer.BlockCopy(waveInfo.ByteData, 0, asInt16, 0, bytes);
                            asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
                            break;
                        default:
                            return waveInfo;
                    }

                    return waveInfo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Some big errors reading file.");
                return null;
            }
        }
        private static IList<string> AvailableDevices => AudioContext.AvailableDevices;

        public class AudioInfo
        {
            public AudioInfo(int sid, int bid)
            {
                SourceId = sid;
                BufferId = bid;
            }

            public int SourceId { get; set; }

            public int BufferId { get; set; }
        }

        public class WaveInfo
        {
            public int ChunkId { get; set; }

            public int FileSize { get; set; }

            public int RiffType { get; set; }

            public int FmtId { get; set; }

            public int FmtSize { get; set; }

            public int FmtCode { get; set; }

            public int Channels { get; set; }

            public int SampleRate { get; set; }

            public int ByteRate { get; set; }

            public int FmtBlockAlign { get; set; }

            public int BitDepth { get; set; }

            public byte[] ByteData { get; set; }

            private int BitsPerSample => this.ByteRate / this.SampleRate * 8;

            public int NumberOfSeconds => (int)Math.Ceiling((double)this.ByteData.Length / (this.SampleRate * (this.BitsPerSample / 8)));
        }
    }
}
