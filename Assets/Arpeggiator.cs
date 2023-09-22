using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arpeggiator
{
    public float Amplitude;
    public float Interval;
    public float Gate;
    
    public List<float> Frequencies { get; } = new ();
    private int _nextFrequencyIndex;

    public void SetFrequencies(List<float> frequencies)
    {
        Frequencies.Clear();
        foreach (var f in frequencies)  Frequencies.Add(f);
    }

    public void Play()
    {
        
    }
    
    public static IEnumerator PlaySequence(List<float> frequencies, float amplitude, Synthesizer.Interval interval, float gate,
        int repetitions, SampleGenerator.Instrument instrument, List<SoundEffects.SoundEffectType> soundEffects)
    {
        var timer = Time.realtimeSinceStartup;
        
        for (var i = 0; i < repetitions; i++)
        {
            foreach (var frequency in frequencies)
            {
                var note = new Synthesizer.Note()
                {
                    Frequency = frequency,
                    Amplitude = amplitude,
                    Interval = interval * gate
                };
                
                SampleGenerator.PlayNote(note, instrument, 0.0f, soundEffects);
                yield return new WaitForSeconds(interval.TimeDuration - (timer - Time.realtimeSinceStartup));
                timer = Time.realtimeSinceStartup;
            }
        }
    }
}
