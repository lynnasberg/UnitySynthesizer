using System.Collections.Generic;
using UnityEngine;

public class FrequencyGenerator
{
    public float A4Frequency = 440.0f;

    public static readonly int[] MajorScaleOffsets = { 0, 2, 4, 5, 7, 9, 11 };

    public List<float> GenerateScale(Note key, int lowestOctave, int highestOctave = int.MinValue)
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

    public float GetFrequency(Note note, int octave)
    {
        return A4Frequency * Mathf.Pow(2.0f, ((float)(note + 2)) / 12f + (octave - 4));
    }

    public enum Note
    {
        C = 0,
        D = 1,
        E = 2,
        F = 3,
        G = 4,
        A = 5,
        B = 6,
    }
}
