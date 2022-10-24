using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0, 1)]
    public float gameVolume;

    public Sound[] sounds;

    private void Awake()
    {
        foreach (Sound s in sounds)//making audio sources for a sounds
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * gameVolume;
            s.source.pitch = s.pitch;
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.soundName == name);
        if (s != null) s.source.Play();
    }
}