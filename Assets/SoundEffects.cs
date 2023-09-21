using System;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public enum SoundEffectType
    {
        StereoEcho,
        Reverb
    }

    public static void ApplyEffect(ref float[] samples, float sampleRate, SoundEffectType soundEffectType)
    {
        switch (soundEffectType)
        {
            case SoundEffectType.StereoEcho:
                var delay = 0;
                var amplitude = 0.5f;
                
                for (var reflection = 0; reflection < 5; reflection++)
                {
                    delay += (int)(0.8f * sampleRate);
                    amplitude *= 0.3f;
                    
                    for (var i = reflection % 2; i < samples.Length; i += 2)
                    {
                        if (i + delay >= samples.Length) break;
                        samples[i + delay] += amplitude * samples[i];
                    }
                }

                break;
            
            case SoundEffectType.Reverb:
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(soundEffectType), soundEffectType, null);
        }
    }
}
