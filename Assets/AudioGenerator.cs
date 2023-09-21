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
            Duration = 0.3f,
            Envelope = new NoteGenerator.Envelope {Attack = 0.05f, Decay = 0.25f, Sustain = 0.3f, Release = 0.05f},
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

        var intonation = FrequencyGenerator.Intonation.JustIntonation;
        var repetitions = 4;

        var longnote = _noteAttributes;
        longnote.Duration = 9.6f;
        longnote.Amplitude = 0.5f;
        longnote.WaveFormType = NoteGenerator.WaveFormType.SuperSaw;
        longnote.Envelope = NoteGenerator.Envelope.Default;
        
        NoteGenerator.PlayNote(_frequencyGenerator.GetFrequency(FrequencyGenerator.Note.C, 1), 0f, longnote, _soundEffects);
        longnote.Amplitude = 0.15f;
        NoteGenerator.PlayNote(_frequencyGenerator.GetFrequency(FrequencyGenerator.Note.C, 5), -1f, longnote, _soundEffects);
        NoteGenerator.PlayNote(_frequencyGenerator.GetFrequency(FrequencyGenerator.Note.C, 5) * 1.01f, 1f, longnote, _soundEffects);
        
        yield return Arpeggiator.PlaySequence(
            frequencyGenerator.GenerateChord(FrequencyGenerator.Note.C, 3, FrequencyGenerator.ChordType.Minor, intonation)
            , _noteAttributes, _soundEffects, repetitions);
        
        yield return Arpeggiator.PlaySequence(
            frequencyGenerator.GenerateChord(FrequencyGenerator.Note.BFlat, 2, FrequencyGenerator.ChordType.Major, intonation)
            , _noteAttributes, _soundEffects, repetitions);
        
        yield return Arpeggiator.PlaySequence(
            frequencyGenerator.GenerateChord(FrequencyGenerator.Note.AFlat, 2, FrequencyGenerator.ChordType.Major, intonation)
            , _noteAttributes, _soundEffects, repetitions);
        
        yield return Arpeggiator.PlaySequence(
            frequencyGenerator.GenerateChord(FrequencyGenerator.Note.G, 2, FrequencyGenerator.ChordType.Major, intonation)
            , _noteAttributes, _soundEffects, repetitions);

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