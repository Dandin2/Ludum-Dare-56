using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioMixer Mixer;

    public Sprite SoundImage;
    public Sprite SoundMutedImage;
    public Image SoundImageHolder;

    private bool musicMuted;
    private bool sfxMuted;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        sfxMuted = WorldManager.instance.SfxMuted;
        musicMuted = WorldManager.instance.MusicMuted;
        if (sfxMuted)
        {
            SoundImageHolder.sprite = SoundMutedImage;
            Mixer.SetFloat("SFX", -80);
        }
        else
        {
            SoundImageHolder.sprite = SoundImage;
            Mixer.SetFloat("SFX", 0);
        }

        if (musicMuted)
        {
            Mixer.SetFloat("Music", -80);
        }
        else
        {
            Mixer.SetFloat("Music", 0);
        }
    }

    public void ToggleMuteSound()
    {
        sfxMuted = !sfxMuted;
        if (sfxMuted)
        {
            SoundImageHolder.sprite = SoundMutedImage;
            Mixer.SetFloat("SFX", -80);
        }
        else
        {
            SoundImageHolder.sprite = SoundImage;
            Mixer.SetFloat("SFX", 0);
        }
        WorldManager.instance.SfxMuted = sfxMuted;
    }

    public void ToggleMuteMusic()
    {
        musicMuted = !musicMuted;
        if (musicMuted)
        {
            Mixer.SetFloat("Music", -80);
        }
        else
        {
            Mixer.SetFloat("Music", 0);
        }
        WorldManager.instance.MusicMuted = musicMuted;
    }
}
