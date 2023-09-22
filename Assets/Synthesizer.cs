using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{
    public static float Tempo = 120f;
    private float _sampleRate;

    public FrequencyGenerator FrequencyGenerator { get; private set; }

    private void Awake()
    {
        FrequencyGenerator = new FrequencyGenerator();
    }

    private void Start()
    {
        StartCoroutine(PlayStuff());
    }

    private IEnumerator PlayStuff()
    {
        yield return new WaitForSeconds(1.0f);
        
        var instrument = new SampleGenerator.Instrument()
        {
            Envelope = new SampleGenerator.Envelope {Attack = 0.05f, Decay = 0.25f, Sustain = 0.3f, Release = 0.05f},
            WaveFormType = SampleGenerator.WaveFormType.SawTooth
        };
        
        var soundEffects = new List<SoundEffects.SoundEffectType>()
        {
            SoundEffects.SoundEffectType.StereoEcho
        };

        var intonation = FrequencyGenerator.Intonation.JustIntonation;
        var repetitions = 4;
        var interval = Interval.Sixteenth;
        var gate = 0.5f;

        while (true)
        {
            yield return Arpeggiator.PlaySequence(FrequencyGenerator.GenerateChord(
                FrequencyGenerator.Pitch.C, 3, FrequencyGenerator.ChordType.Minor, intonation),
                0.5f, interval, gate, repetitions, instrument, soundEffects);
            
            yield return Arpeggiator.PlaySequence(FrequencyGenerator.GenerateChord(
                    FrequencyGenerator.Pitch.BFlat, 2, FrequencyGenerator.ChordType.Major, intonation),
                0.5f, interval, gate, repetitions, instrument, soundEffects);
            
            yield return Arpeggiator.PlaySequence(FrequencyGenerator.GenerateChord(
                    FrequencyGenerator.Pitch.AFlat, 2, FrequencyGenerator.ChordType.Major, intonation),
                0.5f, interval, gate, repetitions, instrument, soundEffects);
            
            yield return Arpeggiator.PlaySequence(FrequencyGenerator.GenerateChord(
                    FrequencyGenerator.Pitch.G, 2, FrequencyGenerator.ChordType.Major, intonation),
                0.5f, interval, gate, repetitions, instrument, soundEffects);
        }
        

        /*foreach (var offset in FrequencyGenerator.MajorScaleOffsets)
        {
            var frequencyGenerator = new FrequencyGenerator();
            var scale = frequencyGenerator.GenerateScale(FrequencyGenerator.Note.C + offset * 2, 2);

            var lowest = scale[0];
            var highest = scale[^1];
        
            scale.Shuffle();

            var noteAttributes = new NoteGenerator.NoteAttributes()
            {
                Amplitude = 0.4f,
                Duration = 0.5f,
                Envelope = new NoteGenerator.Envelope {Attack = 0.01f, Decay = 0.3f, Sustain = 0.05f, Release = 0.1f},
                WaveFormType = (NoteGenerator.WaveFormType)(_scc % 4)
            };

            var soundEffects = new List<SoundEffects.SoundEffectType>()
            {
                SoundEffects.SoundEffectType.StereoEcho
            };

            foreach (var t in scale)
            {
                NoteGenerator.PlayNote(t, Mathf.Lerp(-0.5f, 0.5f, Mathf.InverseLerp(lowest, highest, t)), noteAttributes, soundEffects);
                yield return new WaitForSeconds(0.15f);
            }
        }*/
    }
    
    public struct Note
    {
        public float Frequency;
        public float Amplitude;
        public Interval Interval;
    }

    public struct Interval
    {
        public float Duration { get; private set; }
        public float TimeDuration => Duration * 4f * (60f / Tempo);
        
        public static Interval operator +(Interval x, Interval y) {
            return new Interval { Duration = x.Duration + y.Duration };
        }
        
        public static Interval operator -(Interval x, Interval y) {
            return new Interval { Duration = x.Duration - y.Duration };
        }
        
        public static Interval operator *(int i, Interval interval)
        {
            if (i > 0) return new Interval { Duration = i * interval.Duration };
            
            Debug.LogError("Cannot multiply interval by negative or zero integer");
            return new Interval();
        }
        
        public static Interval operator *(Interval interval, int i)
        {
            return i * interval;
        }
        
        public static Interval operator *(float f, Interval interval)
        {
            if (f > 0) return new Interval { Duration = f * interval.Duration };
            
            Debug.LogError("Cannot multiply interval by negative or zero float");
            return new Interval();
        }
        
        public static Interval operator *(Interval interval, float f)
        {
            return f * interval;
        }

        public static Interval Whole = new() { Duration = 1.0f };
        public static Interval Half = new() { Duration = 0.5f };
        public static Interval Quarter = new() { Duration = 0.25f };
        public static Interval Eighth = new() { Duration = 0.125f };
        public static Interval Sixteenth = new() { Duration = 0.0625f };
        public static Interval ThirtySecond = new() { Duration = 0.03125f };
        public static Interval SixtyFourth = new() { Duration = 0.015625f };
        public static Interval Triplet = new() { Duration = 1f / 3f };
        public static Interval Sextuplet = new() { Duration = 1f / 6f };
    }

    public enum IntervalType
    {
        Whole = 0,
        Half = 1,
        Quarter = 2,
        Eighth = 3,
        Sixteenth = 4,
        ThirtySecond = 5,
        SixtyFourth = 6,
        Triplet = 7,
        Sextuplet = 8
    }
}