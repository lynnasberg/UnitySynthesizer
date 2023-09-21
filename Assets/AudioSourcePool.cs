using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    public static AudioSourcePool Instance; // Singleton instance

    public GameObject audioSourcePrefab; // Prefab for the AudioSource GameObject
    public int poolSize = 10; // Number of AudioSources in the pool

    private readonly Queue<AudioSource> _availableAudioSources = new();

    private void Awake()
    {
        // Singleton pattern to ensure there's only one instance of the pool
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create and initialize the pool
        for (var i = 0; i < poolSize; i++)
        {
            SpawnInstance();
        }
    }

    private void SpawnInstance()
    {
        var obj = Instantiate(audioSourcePrefab, transform, true);
        var audioSource = obj.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        _availableAudioSources.Enqueue(audioSource);
    }

    // Play an audio clip using a pooled AudioSource
    public void PlayAudioClip(AudioClip clip)
    {
        if (_availableAudioSources.Count == 0) SpawnInstance();

        var audioSource = _availableAudioSources.Dequeue();
        audioSource.clip = clip;
        audioSource.Play();

        // Start a coroutine to check when the audio finishes playing
        StartCoroutine(WaitForAudioFinish(audioSource));
    }

    // Coroutine to wait for the audio to finish and return the AudioSource to the pool
    private IEnumerator WaitForAudioFinish(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.Stop();
        audioSource.clip = null;
        _availableAudioSources.Enqueue(audioSource);
    }
}
