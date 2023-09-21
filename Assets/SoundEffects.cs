using System;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public enum SoundEffectType
    {
        StereoEcho,
        Reverb
    }

    public static void ApplyEffect(ref float[,] samples, float sampleRate, float sampleCount, SoundEffectType soundEffectType)
    {
        switch (soundEffectType)
        {
            case SoundEffectType.StereoEcho:
                var delay = 0;
                var amplitude = 0.5f;
                
                for (var reflection = 0; reflection < 5; reflection++)
                {
                    delay += (int)(0.3f * sampleRate);
                    amplitude *= 0.5f;

                    for (var i = 0; i < sampleCount; i++)
                    {
                        if (i + delay >= sampleCount) break;
                        var channel = reflection % 2;
                        samples[i + delay, channel] += amplitude * samples[i, channel];
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
