using System;
using System.Collections.Generic;
using UnityEngine;

public static class SampleGenerator
{
    private const float SuperSawDetune = 15f / 1200f;
    private const float StringsDetune = 10f / 1200f;

    private static float[,] _samples = new float[6000000, 2];

    public static void PlayNote(Synthesizer.Note note, Instrument instrument, float pan, List<SoundEffects.SoundEffectType> soundEffects = null)
    {
        var clipDuration = (note.Interval.TimeDuration + instrument.Envelope.Release) + 0.0f;

        var sampleCount = (int)(clipDuration * AudioSettings.outputSampleRate);
        var sampleRate = (float)AudioSettings.outputSampleRate;

        //var samples = new float[sampleCount, 2];

        var frequency = note.Frequency;
        var amplitude = note.Amplitude;
        var timeDuration = note.Interval.TimeDuration;

        // generate samples
        for (var i = 0; i < sampleCount; i++)
        {
            var t = i / sampleRate;
            var sample = GetSample(t, frequency, amplitude, timeDuration, instrument);

            _samples[i, 0] = pan > 0f ? (1f - pan) * sample : sample;
            _samples[i, 1] = pan < 0f ? (1f + pan) * sample : sample;
        }

        // apply effects
        if (soundEffects != null)
        {
            foreach (var soundEffect in soundEffects)
            {
                SoundEffects.ApplyEffect(ref _samples, sampleRate, sampleCount, soundEffect);
            }
        }

        // Set the samples to the audio clip
        var samplesInterleaved = new float[sampleCount * 2];
        for (var i = 0; i < sampleCount; i++)
        {
            samplesInterleaved[i * 2 + 0] = _samples[i, 0];
            samplesInterleaved[i * 2 + 1] = _samples[i, 1];
        }
        
        var audioClip = AudioClip.Create("GeneratedAudio", sampleCount * 2, 2, AudioSettings.outputSampleRate, false);
        audioClip.SetData(samplesInterleaved, 0);
        
        AudioSourcePool.Instance.PlayAudioClip(audioClip);
    }

    public struct Instrument
    {
        public WaveFormType WaveFormType;
        public Envelope Envelope;
    }
    
    public enum WaveFormType
    {
        SawTooth = 0,
        Sine = 1,
        Square = 2,
        Triangle = 3,
        SuperSaw = 4,
        Strings = 5,
    }
    
    private static float GetSample(float t, float frequency, float amplitude, float timeDuration, Instrument instrument)
    {
        var sample = instrument.WaveFormType switch
        {
            WaveFormType.SawTooth => SawtoothWave(t, frequency, amplitude),
            WaveFormType.Sine => SineWave(t, frequency, amplitude),
            WaveFormType.Square => SquareWave(t, frequency, amplitude),
            WaveFormType.Triangle => TriangleWave(t, frequency, amplitude),
            WaveFormType.SuperSaw => SuperSawWave(t, frequency, amplitude),
            WaveFormType.Strings => StringsWave(t, frequency, amplitude),
            _ => throw new ArgumentOutOfRangeException(nameof(instrument.WaveFormType), instrument.WaveFormType, null)
        };

        return sample * instrument.Envelope.CalculateValue(t, timeDuration);
    }

    public readonly struct Envelope
    {
        public readonly float Attack;
        public readonly float Decay;
        public readonly float Sustain;
        public readonly float Release;

        private readonly float _oneOverAttack;
        private readonly float _oneOverDecay;
        private readonly float _oneOverRelease;

        public Envelope(float attack, float decay, float sustain, float release)
        {
            Attack = attack;
            Decay = decay;
            Sustain = sustain;
            Release = release;

            _oneOverAttack = 1.0f / attack;
            _oneOverDecay = 1.0f / decay;
            _oneOverRelease = 1.0f / release;
        }

        private float CalculateVolume(float t)
        {
            if (t < Attack) return t * _oneOverAttack;

            var T = t - Attack;
            if (T < Decay) return 1f + T * (Sustain - 1f) * _oneOverDecay;

            return Sustain;
        }

        public float CalculateValue(float t, float duration)
        {
            if (t >= duration + Release) return 0;
            var volume = CalculateVolume(t);
            if (t < duration) return volume;

            var T = t - duration;
            return volume * (1f - T  * _oneOverRelease);
        }
    }
    
    private static float SawtoothWave(float t, float f, float A)
    {
        var tf = t * f;
        var normalizedT = tf - (float)Math.Floor(tf);
        return 2.0f * A * (normalizedT - 0.5f);
    }

    private static float SuperSawWave(float t, float f, float A)
    {
        var wave1 = SawtoothWave(t, f, A);
        var wave2 = SawtoothWave(t, f * Mathf.Pow(2.0f, SuperSawDetune), A);
        var wave3 = SawtoothWave(t, f * Mathf.Pow(2.0f, -SuperSawDetune), A);

        return (wave1 + wave2 + wave3) * 0.33333333f;
    }
    
    private static float StringsWave(float t, float f, float A)
    {
        var wave1 = SawtoothWave(t, f, A);
        var wave2 = SawtoothWave(t, f * Mathf.Pow(2.0f, StringsDetune), A);

        return (wave1 + wave2) * 0.5f;
    }

    private static float SineWave(float t, float f, float A)
    {
        return Mathf.Sin(2.0f * Mathf.PI * f * t) * A;
    }
    
    private static float SquareWave(float t, float f, float A)
    {
        var T = 1.0f / f; // Calculate the period based on the frequency
        var normalizedT = (t / T) - Mathf.Floor(t / T);
    
        return (normalizedT < 0.5f) ? A : -A;
    }


    private static float TriangleWave(float t, float f, float A)
    {
        var T = 1.0f / f; // Calculate the period based on the frequency
        var halfT = T / 2.0f; // Half of the period

        // Calculate the triangle wave
        var normalizedT = (t / T) - Mathf.Floor(t / T);
        
        return normalizedT switch
        {
            < 0.25f => A * normalizedT * 4.0f,
            < 0.75f => A * (1.0f - (normalizedT - 0.25f) * 4.0f),
            _ => A * (normalizedT - 1.0f) * 4.0f
        };
    }
}
