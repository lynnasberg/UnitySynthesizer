using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arpeggiator
{
    public static IEnumerator PlaySequence(List<float> notes, NoteGenerator.NoteAttributes noteAttributes, List<SoundEffects.SoundEffectType> soundEffects, int repetitions)
    {
        var timer = Time.realtimeSinceStartup;
        
        for (var i = 0; i < repetitions; i++)
        {
            foreach (var note in notes)
            {
                NoteGenerator.PlayNote(note, 0.0f, noteAttributes, soundEffects);
                yield return new WaitForSeconds(0.2f - (timer - Time.realtimeSinceStartup));
                timer = Time.realtimeSinceStartup;
            }
        }
    }
}
