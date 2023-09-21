using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGenerator : MonoBehaviour
{
    private float _sampleRate;

    private int _scc = -1;

    private FrequencyGenerator _frequencyGenerator;
    private NoteGenerator.NoteAttributes _noteAttributes;
    private List<SoundEffects.SoundEffectType> _soundEffects;

    private void Awake()
    {
        _frequencyGenerator = new FrequencyGenerator();
        
        _noteAttributes = new NoteGenerator.NoteAttributes()
        {
            Amplitude = 0.4f,
            Duration = 2.0f,
            Envelope = new NoteGenerator.Envelope {Attack = 0.5f, Decay = 0.25f, Sustain = 0.5f, Release = 1.0f},
            WaveFormType = NoteGenerator.WaveFormType.SawTooth
        };
        
        _soundEffects = new List<SoundEffects.SoundEffectType>()
        {
            SoundEffects.SoundEffectType.StereoEcho
        };
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.S)) return;
        _scc++;
        
        var w = (NoteGenerator.WaveFormType)(_scc % 4);
        Debug.Log(w);

        StartCoroutine(PlayStuff());
    }

    private IEnumerator PlayStuff()
    {
        var frequencyGenerator = new FrequencyGenerator();
        var chord = frequencyGenerator.GenerateChord(FrequencyGenerator.Note.C, 3, FrequencyGenerator.ChordType.Augmented, FrequencyGenerator.Intonation.EqualTemperament);

        foreach (var note in chord)
        {
            NoteGenerator.PlayNote(note, 0.0f, _noteAttributes, _soundEffects);
        }

        yield break;

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
}