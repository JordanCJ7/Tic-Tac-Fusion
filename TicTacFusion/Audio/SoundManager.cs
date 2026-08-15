using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace TicTacFusion.Audio
{
    public class SoundManager
    {
        private static SoundManager? _instance;
        public static SoundManager Instance => _instance ??= new SoundManager();

        public bool IsMuted { get; set; } = false;

        private byte[]? _clickSound;
        private byte[]? _placeXSound;
        private byte[]? _placeOSound;
        private byte[]? _timerTickSound;
        private byte[]? _winSound;
        private byte[]? _loseSound;
        private byte[]? _drawSound;

        private SoundManager()
        {
            PreGenerateSounds();
        }

        private void PreGenerateSounds()
        {
            _clickSound = GenerateToneWav(frequency: 880, durationMs: 40, attackMs: 5, decayMs: 35, waveType: 0); // Crisp UI click
            _placeXSound = GenerateSweepWav(startFreq: 440, endFreq: 880, durationMs: 120, waveType: 0); // High-tech swoosh for X
            _placeOSound = GenerateSweepWav(startFreq: 700, endFreq: 400, durationMs: 140, waveType: 1); // Resonant tone for O
            _timerTickSound = GenerateToneWav(frequency: 1200, durationMs: 35, attackMs: 2, decayMs: 33, waveType: 0); // Short countdown tick
            _winSound = GenerateFanfareWav(); // Triumphant victory fanfare
            _loseSound = GenerateSweepWav(startFreq: 400, endFreq: 180, durationMs: 350, waveType: 1); // Sci-fi defeat
            _drawSound = GenerateChordWav(new[] { 523.25, 659.25, 783.99 }, durationMs: 250); // Major chord for draw
        }

        public void PlayClick() => PlaySoundBytes(_clickSound);
        public void PlayPlaceX() => PlaySoundBytes(_placeXSound);
        public void PlayPlaceO() => PlaySoundBytes(_placeOSound);
        public void PlayTimerTick() => PlaySoundBytes(_timerTickSound);
        public void PlayWin() => PlaySoundBytes(_winSound);
        public void PlayLose() => PlaySoundBytes(_loseSound);
        public void PlayDraw() => PlaySoundBytes(_drawSound);

        private void PlaySoundBytes(byte[]? wavData)
        {
            if (IsMuted || wavData == null) return;
            Task.Run(() =>
            {
                try
                {
                    using var ms = new MemoryStream(wavData);
                    using var player = new SoundPlayer(ms);
                    player.Play();
                }
                catch
                {
                    // Audio fallback - ignore if audio device is unavailable
                }
            });
        }

        private static byte[] GenerateToneWav(double frequency, int durationMs, int attackMs, int decayMs, int waveType)
        {
            int sampleRate = 44100;
            int totalSamples = (int)(sampleRate * (durationMs / 1000.0));
            int attackSamples = (int)(sampleRate * (attackMs / 1000.0));
            int decaySamples = (int)(sampleRate * (decayMs / 1000.0));
            
            short[] samples = new short[totalSamples];
            for (int i = 0; i < totalSamples; i++)
            {
                double t = (double)i / sampleRate;
                double val = waveType switch
                {
                    0 => Math.Sin(2 * Math.PI * frequency * t), // Sine
                    1 => Math.Sin(2 * Math.PI * frequency * t) * 0.7 + Math.Sin(4 * Math.PI * frequency * t) * 0.3, // Harmonic sine
                    _ => Math.Sin(2 * Math.PI * frequency * t)
                };

                // ADSR Envelope
                double env = 1.0;
                if (i < attackSamples && attackSamples > 0)
                    env = (double)i / attackSamples;
                else if (i > totalSamples - decaySamples && decaySamples > 0)
                    env = (double)(totalSamples - i) / decaySamples;

                samples[i] = (short)(val * env * 24000);
            }

            return CreateWavHeader(samples, sampleRate);
        }

        private static byte[] GenerateSweepWav(double startFreq, double endFreq, int durationMs, int waveType)
        {
            int sampleRate = 44100;
            int totalSamples = (int)(sampleRate * (durationMs / 1000.0));
            short[] samples = new short[totalSamples];

            double phase = 0;
            for (int i = 0; i < totalSamples; i++)
            {
                double progress = (double)i / totalSamples;
                double currentFreq = startFreq + (endFreq - startFreq) * progress;
                phase += 2 * Math.PI * currentFreq / sampleRate;

                double val = waveType == 1 
                    ? Math.Sin(phase) * 0.8 + Math.Sin(phase * 2) * 0.2
                    : Math.Sin(phase);

                // Smooth fade out
                double env = Math.Sin(Math.PI * progress);
                samples[i] = (short)(val * env * 24000);
            }

            return CreateWavHeader(samples, sampleRate);
        }

        private static byte[] GenerateChordWav(double[] freqs, int durationMs)
        {
            int sampleRate = 44100;
            int totalSamples = (int)(sampleRate * (durationMs / 1000.0));
            short[] samples = new short[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                double t = (double)i / sampleRate;
                double val = 0;
                foreach (var f in freqs)
                {
                    val += Math.Sin(2 * Math.PI * f * t);
                }
                val /= freqs.Length;

                double env = 1.0 - ((double)i / totalSamples);
                samples[i] = (short)(val * env * 24000);
            }

            return CreateWavHeader(samples, sampleRate);
        }

        private static byte[] GenerateFanfareWav()
        {
            int sampleRate = 44100;
            // Fanfare notes: C5 (523.25), E5 (659.25), G5 (783.99), C6 (1046.50)
            double[] notes = { 523.25, 659.25, 783.99, 1046.50 };
            int noteDurationMs = 120;
            int finalNoteMs = 350;
            int totalMs = (notes.Length - 1) * noteDurationMs + finalNoteMs;
            int totalSamples = (int)(sampleRate * (totalMs / 1000.0));
            short[] samples = new short[totalSamples];

            int offset = 0;
            for (int n = 0; n < notes.Length; n++)
            {
                int dur = (n == notes.Length - 1) ? finalNoteMs : noteDurationMs;
                int count = (int)(sampleRate * (dur / 1000.0));
                double freq = notes[n];

                for (int i = 0; i < count && (offset + i) < totalSamples; i++)
                {
                    double t = (double)i / sampleRate;
                    double val = Math.Sin(2 * Math.PI * freq * t) * 0.8 + Math.Sin(4 * Math.PI * freq * t) * 0.2;
                    double env = 1.0 - ((double)i / count) * 0.4;
                    if (n == notes.Length - 1)
                        env = 1.0 - ((double)i / count);
                    samples[offset + i] = (short)(val * env * 24000);
                }
                offset += count;
            }

            return CreateWavHeader(samples, sampleRate);
        }

        private static byte[] CreateWavHeader(short[] samples, int sampleRate)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            int byteRate = sampleRate * 1 * 2; // 1 channel, 16 bit (2 bytes)
            int dataSize = samples.Length * 2;

            // RIFF header
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataSize);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // fmt subchunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // Subchunk1Size for PCM
            writer.Write((short)1); // AudioFormat (1 = PCM)
            writer.Write((short)1); // NumChannels (1 = Mono)
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)2); // BlockAlign
            writer.Write((short)16); // BitsPerSample

            // data subchunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(dataSize);

            foreach (var sample in samples)
            {
                writer.Write(sample);
            }

            return stream.ToArray();
        }
    }
}

