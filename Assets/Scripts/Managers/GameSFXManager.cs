using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSFXManager : SFXManager
{
    //public List<AudioSource> Sources3D;
    //private int curAS3D;

    public new static GameSFXManager Instance;

    public MultiSoundEffect[] MultiSoundEffects;
    // Start is called before the first frame update

    // Update is called once per frame
    protected new void Start()
    {
        base.Start();
        Ambient.mute = Muted;
    }

    void Awake()
    {
        Instance = this;
    }

    public void PlayClipRandom(string code)
    {
        MultiSoundEffect current = MultiSoundEffects.First(a => a.Code == code);
        Sources[curAS].volume = current.Volume;
        Sources[curAS].clip = current.GetClip;
        Sources[curAS].Play();
        curAS = (curAS + 1) % Sources.Count;
    }
}

[System.Serializable]
public class MultiSoundEffect
{
    public string Code;
    public AudioClip[] Clips;
    [Range(0, 1)]
    public float Volume;

    public AudioClip GetClip
    {
        get { return Clips[Random.Range(0, Clips.Length)]; }
    }
}
