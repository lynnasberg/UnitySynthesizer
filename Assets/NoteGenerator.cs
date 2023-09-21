using System;
using System.Collections.Generic;
using UnityEngine;

public static class NoteGenerator
{
    private const float SuperSawDetuneCents = 15f;
    
    public static void PlayNote(float frequency, float pan, NoteAttributes noteAttributes, List<SoundEffects.SoundEffectType> soundEffects = null)
    {
        var clipDuration = (noteAttributes.Duration + noteAttributes.Envelope.Release) + 0.0f;

        var sampleCount = (int)(clipDuration * AudioSettings.outputSampleRate);
        var sampleRate = (float)AudioSettings.outputSampleRate;

        var samples = new float[sampleCount, 2];

        // generate samples
        for (var i = 0; i < sampleCount; i++)
        {
            var t = i / sampleRate;
            var sample = GetSample(t, frequency, noteAttributes);
            samples[i, 0] = Mathf.Lerp(1f, 0f, pan) * sample;
            samples[i, 1] = Mathf.Lerp(1f, 0f, -pan) * sample;
        }

        // apply effects
        if (soundEffects != null)
        {
            foreach (var soundEffect in soundEffects)
            {
                SoundEffects.ApplyEffect(ref samples, sampleRate, sampleCount, soundEffect);
            }
        }

        // Set the samples to the audio clip
        var samplesInterleaved = new float[sampleCount * 2];
        for (var i = 0; i < sampleCount; i++)
        {
            samplesInterleaved[i * 2 + 0] = samples[i, 0];
            samplesInterleaved[i * 2 + 1] = samples[i, 1];
        }
        
        var audioClip = AudioClip.Create("GeneratedAudio", sampleCount * 2, 2, AudioSettings.outputSampleRate, false);
        audioClip.SetData(samplesInterleaved, 0);
        
        AudioSourcePool.Instance.PlayAudioClip(audioClip);
    }

    public struct NoteAttributes
    {
        public WaveFormType WaveFormType;
        public float Amplitude;
        public float Duration;
        public Envelope Envelope;
    }
    
    public enum WaveFormType
    {
        SawTooth = 0,
        Sine = 1,
        Square = 2,
        Triangle = 3,
        SuperSaw = 4,
    }
    
    private static float GetSample(float t, float f, NoteAttributes noteAttributes)
    {
        var sample = noteAttributes.WaveFormType switch
        {
            WaveFormType.SawTooth => SawtoothWave(t, f, noteAttributes.Amplitude),
            WaveFormType.Sine => SineWave(t, f, noteAttributes.Amplitude),
            WaveFormType.Square => SquareWave(t, f, noteAttributes.Amplitude),
            WaveFormType.Triangle => TriangleWave(t, f, noteAttributes.Amplitude),
            WaveFormType.SuperSaw => SuperSawWave(t, f, noteAttributes.Amplitude),
            _ => throw new ArgumentOutOfRangeException(nameof(noteAttributes.WaveFormType), noteAttributes.WaveFormType, null)
        };

        return sample * noteAttributes.Envelope.CalculateValue(t, noteAttributes.Duration);
    }

    public struct Envelope
    {
        public float Attack;
        public float Decay;
        public float Sustain;
        public float Release;

        public Envelope(float attack, float decay, float sustain, float release)
        {
            Attack = attack;
            Decay = decay;
            Sustain = sustain;
            Release = release;
        }

        public float CalculateValue(float t, float duration)
        {
            // basic envelope
            var e = 1f;
        
            if (t < Attack)
            {
                e = t / Attack;
            }
            else if (t < Attack + Decay)
            {
                e = Mathf.Lerp(1.0f, Sustain, (t - Attack) / Decay);
            }
            else
            {
                e = Sustain;
            }
        
            // add release
            if (t > duration + Release)
            {
                e = 0;
            }
            else if (t > duration)
            {
                e = Mathf.Lerp(e, 0f, (t - duration) / Release);
            }

            return e;
        }

        public static Envelope Default = new () { Attack = 0.05f, Decay = 0.0f, Sustain = 1.0f, Release = 0.05f };
    }
    
    private static float SawtoothWave(float t, float f, float A)
    {
        var T = 1.0f / f; // Calculate the period based on the frequency
        var normalizedT = (t / T) - Mathf.Floor(t / T);
    
        return (2.0f * A) * (normalizedT - 0.5f);
    }

    private static float SuperSawWave(float t, float f, float A)
    {
        var wave1 = SawtoothWave(t, f, A);
        var wave2 = SawtoothWave(t, f * Mathf.Pow(2.0f, SuperSawDetuneCents / 1200f), A);
        var wave3 = SawtoothWave(t, f * Mathf.Pow(2.0f, -SuperSawDetuneCents / 1200f), A);

        return (wave1 + wave2 + wave3) * 0.33333333f;
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
