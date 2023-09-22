using System;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyGenerator
{
    public float A4Frequency = 440.0f;

    public static readonly int[] MajorScaleOffsets = { 0, 2, 4, 5, 7, 9, 11 };

    public List<float> GenerateScale(Pitch key, int lowestOctave, int highestOctave = int.MinValue)
    {
        var list = new List<float>();
        var highestOctaveReal = Mathf.Max(lowestOctave, highestOctave);

        for (var o = lowestOctave; o <= highestOctaveReal; o++)
        {
            foreach (var offset in MajorScaleOffsets)
            {
                list.Add(GetFrequency(key + offset, o));
            }
        }
        
        list.Add(GetFrequency(key + 12, highestOctaveReal));

        return list;
    }

    public List<float> GenerateChord(Pitch rootPitch, int octave, ChordType chordType,
        Intonation intonation = Intonation.EqualTemperament)
    {
        var list = new List<float>();

        switch (intonation)
        {
            case Intonation.EqualTemperament:
                GenerateEqualTemperamentChord(rootPitch, octave, chordType, ref list);
                break;
            
            case Intonation.JustIntonation:
                GenerateJustIntonationChord(GetFrequency(rootPitch, octave), chordType, ref list);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(intonation), intonation, null);
        }

        return list;
    }

    private const float MinorThird = 6f / 5f;
    private const float MajorThird = 5f / 4f;
    private const float MajorFifth = 3f / 2f;
    private const float HarmonicSeventh = 9f / 5f;
    private const float MajorSeventh = 15f / 8f;
    private const float MajorFourth = 4f / 3f;
    private const float DiminishedFifth = 64f / 45f;
    private const float DiminishedThird = 256f / 225f;
    private const float AugmentedFifth = 25f / 16f;

    private static void GenerateJustIntonationChord(float rootFrequency, ChordType chordType, ref List<float> list)
    {
        switch (chordType)
        {
            case ChordType.Major:
                list.Add(rootFrequency);
                list.Add(rootFrequency * MajorThird);
                list.Add(rootFrequency * MajorFifth);
                break;
            
            case ChordType.Minor:
                list.Add(rootFrequency);
                list.Add(rootFrequency * MinorThird);
                list.Add(rootFrequency * MajorFifth);
                break;

            case ChordType.Seventh:
                list.Add(rootFrequency);
                list.Add(rootFrequency * MinorThird);
                list.Add(rootFrequency * MajorFifth);
                list.Add(rootFrequency * HarmonicSeventh);
                break;
            
            case ChordType.MajorSeventh:
                list.Add(rootFrequency);
                list.Add(rootFrequency * MinorThird);
                list.Add(rootFrequency * MajorFifth);
                list.Add(rootFrequency * MajorSeventh);
                break;
            
            case ChordType.Augmented:
                list.Add(rootFrequency);
                list.Add(rootFrequency * MajorThird);
                list.Add(rootFrequency * MajorThird * MajorThird);
                break;
            
            case ChordType.Diminished:
                list.Add(rootFrequency);
                list.Add(rootFrequency * DiminishedThird);
                list.Add(rootFrequency * DiminishedFifth);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(chordType), chordType, null);
        }
    }

    private void GenerateEqualTemperamentChord(Pitch rootPitch, int octave, ChordType chordType, ref List<float> list)
    {
        switch (chordType)
        {
            case ChordType.Major:
                list.Add(GetFrequency(rootPitch, octave));
                list.Add(GetFrequency(rootPitch + 4, octave));
                list.Add(GetFrequency(rootPitch + 7, octave));
                break;
            
            case ChordType.Minor:
                list.Add(GetFrequency(rootPitch, octave));
                list.Add(GetFrequency(rootPitch + 3, octave));
                list.Add(GetFrequency(rootPitch + 7, octave));
                break;

            case ChordType.Seventh:
                list.Add(GetFrequency(rootPitch, octave));
                list.Add(GetFrequency(rootPitch + 3, octave));
                list.Add(GetFrequency(rootPitch + 7, octave));
                list.Add(GetFrequency(rootPitch + 10, octave));
                break;
            
            case ChordType.MajorSeventh:
                list.Add(GetFrequency(rootPitch, octave));
                list.Add(GetFrequency(rootPitch + 3, octave));
                list.Add(GetFrequency(rootPitch + 7, octave));
                list.Add(GetFrequency(rootPitch + 11, octave));
                break;
            
            case ChordType.Augmented:
                list.Add(GetFrequency(rootPitch, octave));
                list.Add(GetFrequency(rootPitch + 5, octave));
                list.Add(GetFrequency(rootPitch + 8, octave));
                break;
            
            case ChordType.Diminished:
                list.Add(GetFrequency(rootPitch, octave));
                list.Add(GetFrequency(rootPitch + 3, octave));
                list.Add(GetFrequency(rootPitch + 6, octave));
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(chordType), chordType, null);
        }
    }

    public float GetFrequency(Pitch pitch, int octave)
    {
        return GetFrequency((int)pitch, octave);
    }
    
    public float GetFrequency(int pitch, int octave)
    {
        return A4Frequency * Mathf.Pow(2.0f, (pitch + 2f) / 12f + (octave - 4));
    }

    public enum Intonation
    {
        EqualTemperament = 0,
        JustIntonation = 1,
    }

    public enum ChordType
    {
        Major = 0,
        Minor = 1,
        Seventh = 2,
        MajorSeventh = 3,
        Augmented = 4,
        Diminished = 5,
    }

    public enum Pitch
    {
        C = 0,
        
        CSharp = 1,
        DFlat = 1,
        
        D = 2,
        
        DSharp = 3,
        EFlat = 3,
        
        E = 4,
        
        F = 5,
        
        FSharp = 6,
        GFlat = 6,
        
        G = 7,
        
        GSharp = 8,
        AFlat = 8,
        
        A = 9,
        
        ASharp = 10,
        BFlat = 10,
        
        B = 11
    }
}
