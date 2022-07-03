using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioSource Music;
    public AudioSource Ambient;

    public List<AudioSource> Sources;
    public SoundEffect[] SoundEffects;
    public AudioClip[] Tracks;

    public bool Muted { get { return PlayerPrefs.GetInt("SFX", 1) == 0; } }
    protected int curAS;

    public int SFXState { get { return PlayerPrefs.GetInt("SFX", 1); } }
    public int MusicState { get { return PlayerPrefs.GetInt("Music", 1); } }

    public delegate void SFXToggled(bool IsOn);
    public delegate void MusicToggled(bool IsOn);

    public SFXToggled OnSFXToggled;

    void Awake()
    {
        Instance = this;
    }
    protected void Start()
    {
        Mute(PlayerPrefs.GetInt("SFX", 1) == 0);

        //if (Tracks.Length > 0)
        //{
        //    Music.clip = Tracks[UnityEngine.Random.Range(0, Tracks.Length)];
            //Music.Play();
        //}

        Music.mute = PlayerPrefs.GetInt("Music", 1) == 0;
    }

    IEnumerator PlaySoundTime(string code, float time)
    {
        yield return new WaitForSeconds(time);
        PlayClip(code);
    }

    public void FadeMusic()
    {
        StartCoroutine(FadeMusicRoutine());
    }

    public void FadeInMusic()
    {
        StartCoroutine(FadeMusicInRoutine());
    }

    IEnumerator FadeMusicRoutine()
    {
        while (Music.volume > 0f)
        {
            Music.volume -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator FadeMusicInRoutine()
    {
        while (Music.volume < 0.2f)
        {
            Music.volume += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    public void PlayClipOneShot(string code)
    {
        SoundEffect current = SoundEffects.First(a => a.Code == code);
        Sources[curAS].volume = current.Volume;
        Sources[curAS].PlayOneShot(current.Clip);
    }
    public void PlayClip(string code)
    {
        SoundEffect current = SoundEffects.First(a => a.Code == code);
        Sources[curAS].volume = current.Volume;
        Sources[curAS].clip = current.Clip;
        Sources[curAS].Play();
        curAS = (curAS + 1) % Sources.Count;
    }

    public void PlayClip(string code, float time)
    {
        StartCoroutine(PlaySoundTime(code, time));
    }

    public void PlayClip(string code, bool loop)
    {
        Sources[curAS].loop = loop;
        PlayClip(code);
    }

    internal void PauseAllAudio()
    {
        foreach (var item in Sources)
        {
            item.Pause();
        }
    }

    public void Stop()
    {
        foreach (var item in Sources)
        {
            if (item.loop)
            {
                item.Stop();
                item.loop = false;
            }
        }
    }



    public void Mute(bool value)
    {
        foreach (AudioSource audioSource in Sources)
        {
            audioSource.mute = value;
        }
        Ambient.mute = value;
    }

    internal void UnPauseAllAudio()
    {
        foreach (var item in Sources)
        {
            item.UnPause();
        }
    }

    public int ToggleSFX()
    {
        int value = Mathf.Abs(SFXState - 1);
        PlayerPrefs.SetInt("SFX", value);
        Mute(Muted);

        if (OnSFXToggled != null)
            OnSFXToggled(value == 1);

        return value;
    }

    public void MuteMusic(bool value)
    {
        Music.mute = !value;
    }

    public int ToggleMusic()
    {
        int value = Mathf.Abs(MusicState - 1);
        PlayerPrefs.SetInt("Music", value);
        Music.mute = (value == 0);

        return value;
    }
}

[Serializable]
public class SoundEffect
{
    public string Code;
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume;

}
