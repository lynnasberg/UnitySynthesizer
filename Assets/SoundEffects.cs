using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public enum SoundEffectType
    {
        StereoEcho,
        StereoChorus
    }

    public static void ApplyEffect(ref float[,] samples, float sampleRate, int sampleCount, SoundEffectType soundEffectType)
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
            
            case SoundEffectType.StereoChorus:
                var chorusDelay = 100;
                for (var i = sampleCount - chorusDelay - 1; i >= 0; i--)
                {
                    samples[i + chorusDelay, 0] = samples[i, 0];
                }
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(soundEffectType), soundEffectType, null);
        }
    }
}
