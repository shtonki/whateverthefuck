namespace whateverthefuck.src.util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using OpenTK;
    using OpenTK.Audio;
    using OpenTK.Audio.OpenAL;

    public static class BoomBoxSetterUpper
    {
        public static void SetupBoombox()
        {
            System.Threading.Thread t = new Thread(() =>
            {
                Boombox.Init();
                Logging.Log("Initialized sounds.");
                var audioInfo = Boombox.Play(Boombox.Songs.Africa, 0f, 0);
                audioInfo.SetPosition(0f, 0);
                audioInfo.SetVolume(2);
            });
            t.Start();
        }
    }

    public static class Boombox
    {
        private static readonly Dictionary<SoundEffects, AudioInfo> SoundEffectToSourceIdDictionary = new Dictionary<SoundEffects, AudioInfo>();
        private static Dictionary<Songs, string> songsToResourceNameDictionary = new Dictionary<Songs, string>();
        private static Dictionary<AudioInfo, Songs> currentlyPlayingSongs = new Dictionary<AudioInfo, Songs>(); // will probably need to make this public later on but style cop is very anger

        public enum SoundEffects
        {
            SoundEffect1,
            SoundEffect2,
        }

        public enum Songs
        {
            Song1,
            Song2,
            Africa,
        }

        private static IList<string> AvailableDevices => AudioContext.AvailableDevices;

        public static void Init()
        {
            IntPtr device = Alc.OpenDevice(null); // My default device.
            ContextHandle context = Alc.CreateContext(device, new[] { -1 });
            Alc.MakeContextCurrent(context);
            currentlyPlayingSongs = new Dictionary<AudioInfo, Songs>();

            AL.DistanceModel(ALDistanceModel.InverseDistanceClamped);

            AL.Listener(ALListener3f.Position, 0, 0, 1); // position listener in a Z-direction away from sources for 3d sound not to switch directions instantly.

            var soundEffectResources = SetupSoundEffectResourceDictionary();
            songsToResourceNameDictionary = SetupSongResourceDictionary();
            LoadSoundEffects(soundEffectResources);

            // Play(SoundEffects.soundeffect1);
            // Play(Songs.Africa);
        }

        public static void SetSourcePosition(AudioInfo audioInfo, float x, float y)
        {
            AL.Source(audioInfo.SourceId, ALSource3f.Position, x, y, 0);
        }

        public static void SetListenerPosition(float x, float y)
        {
            AL.Listener(ALListener3f.Position,  x,  y, 1); // position listener in a Z-direction away from sources for 3d sound not to switch directions instantly.
        }

        public static AudioInfo Play(SoundEffects soundEffect)
        {
            try
            {
                AL.SourcePlay(SoundEffectToSourceIdDictionary[soundEffect].SourceId);
                return SoundEffectToSourceIdDictionary[soundEffect];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static AudioInfo Play(Songs song, float x = 0, float y = 0)
        {
            try
            {
                // Should probably already be .wav when it gets in here.
                string path = Path.Combine(System.Reflection.Assembly.GetCallingAssembly().Location, System.IO.Path.GetFullPath(@"..\..\Resources\")) + songsToResourceNameDictionary[song] + ".wav";
                WaveInfo waveInfo = ReadWavFile(path);

                AL.GenBuffers(1, out int bufferID);
                AL.GenSources(1, out int sourceId);

                AL.BufferData(bufferID, ALFormat.Mono16, waveInfo.ByteData, waveInfo.ByteData.Length, waveInfo.SampleRate);

                AL.Source(sourceId, ALSourcei.Buffer, bufferID); // Bind source to buffer

                AL.Source(sourceId, ALSource3f.Position, x, y, 0);

                AL.Source(sourceId, ALSourcef.ReferenceDistance, 0f); // songs will play at same volume no matter what position listener is located.

                // AL.DistanceModel(ALDistanceModel.ExponentDistanceClamped);

                // Exponent distance: gain = (distance / referenceDistance) ** (- rollOffFactor)
                // Clamped distance: max(referenceDistance, min(distance, maxDistance))
                // sound to be dead when listener 2f away from source (one screen)
                // gain = 0.01 = ((distance/referenceDistance)^(-rollOffFactor)), and we clamp 0.01 ->
                // // clamp: max(referenceDistance, min(distance, maxDistance)) less than 0.01
                // Look at that! No real solutions exist :)) Todo: deal with this headache
                // https://www.desmos.com/calculator/xzqjyqhvzb
                /* AL.DistanceModel(ALDistanceModel.InverseDistanceClamped);
                AL.Source(sourceId, ALSourcef.MaxDistance, 0.2f);
                AL.Source(sourceId, ALSourcef.ReferenceDistance, 1.2f);
                AL.Source(sourceId, ALSourcef.RolloffFactor, 10f);
                */
                AL.DistanceModel(ALDistanceModel.LinearDistanceClamped);
                AL.Source(sourceId, ALSourcef.MaxDistance, 2.236f); // sqrt 5 (1z, 2x -> 1^2+2^2 hypotenuse)
                AL.Source(sourceId, ALSourcef.ReferenceDistance, 2.236f / 10f); // half attentuation by 10 steps until 0
                AL.Source(sourceId, ALSourcef.RolloffFactor, 1); // default roll off

                System.Timers.Timer disposeTimer = new System.Timers.Timer(waveInfo.NumberOfSeconds * 1000);

                var audioInfo = new AudioInfo(sourceId, bufferID);

                disposeTimer.Elapsed += (sender, args) =>
                {
                    DisposeBuffer(bufferID, sourceId);
                    currentlyPlayingSongs.Remove(audioInfo);
                    disposeTimer.Dispose();
                };
                currentlyPlayingSongs.Add(audioInfo, song);
                AL.SourcePlay(sourceId);
                disposeTimer.Start();
                return audioInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SetVolume(int sourceID, int percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                Logging.Log("Give number between 0 and 100 including.", Logging.LoggingLevel.Info);
                if (percentage < 0)
                {
                    percentage = 0;
                }
                else
                {
                    percentage = 100;
                }
            }
            Logging.Log("Setting volume of source to: " + percentage, Logging.LoggingLevel.Info);
            AL.Source(sourceID, ALSourcef.Gain, (float)percentage / 100);
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

        private static void LoadSoundEffects(Dictionary<SoundEffects, string> resourceDictionary)
        {
            foreach (var resource in resourceDictionary)
            {
                // Should probably already be .wav when it gets in here.
                string path = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, System.IO.Path.GetFullPath(@"..\..\Resources\")) + resource.Value + ".wav";
                WaveInfo waveInfo = ReadWavFile(path);
                Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, System.IO.Path.GetFullPath(@"..\..\"));

                AL.GenBuffers(1, out int bufferId);
                AL.GenSources(1, out int sourceId);

                AL.BufferData(bufferId, ALFormat.Stereo16, waveInfo.ByteData, waveInfo.ByteData.Length, waveInfo.SampleRate);

                AL.Source(sourceId, ALSourcei.Buffer, bufferId); // Bind source to buffer

                SoundEffectToSourceIdDictionary[resource.Key] = new AudioInfo(sourceId, bufferId);
            }
        }

        private static void DisposeBuffer(int bufferID, int sourceID)
        {
            Logging.Log("Song finished playing.", Logging.LoggingLevel.Info);
            AL.DeleteSource(sourceID);
            AL.DeleteBuffer(bufferID);
        }

        private static Dictionary<SoundEffects, string> SetupSoundEffectResourceDictionary()
        {
            return new Dictionary<SoundEffects, string>()
            {
                { SoundEffects.SoundEffect1, nameof(Properties.Resources.soundeffect1) },
                { SoundEffects.SoundEffect2, nameof(Properties.Resources.soundeffect3) },
            };
        }

        private static Dictionary<Songs, string> SetupSongResourceDictionary()
        {
            return new Dictionary<Songs, string>()
            {
                { Songs.Song1, nameof(Properties.Resources.piano) },
                { Songs.Africa, nameof(Properties.Resources.africatoto) },
            };
        }

        private static WaveInfo ReadWavFile(string filename)
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

                    // chunk 2
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
                            var asInt16 = new short[samps];
                            System.Buffer.BlockCopy(waveInfo.ByteData, 0, asInt16, 0, bytes);
                            asFloat = Array.ConvertAll(asInt16, e => e / (float)short.MaxValue);
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

        public class AudioInfo
        {
            public AudioInfo(int sid, int bid)
            {
                this.SourceId = sid;
                this.BufferId = bid;
            }

            public int SourceId { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local, to be needed.
            private int BufferId { get; set; }

            public void SetPosition(float x, float y)
            {
                Boombox.SetSourcePosition(this, x, y);
            }

            public void SetVolume(int percentage)
            {
                Boombox.SetVolume(this.SourceId, percentage);
            }
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

            public int NumberOfSeconds => (int)Math.Ceiling((double)this.ByteData.Length / (this.SampleRate * (this.BitsPerSample / 8)));

            private int BitsPerSample => this.ByteRate / this.SampleRate * 8;
        }
    }
}
