using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggles : MonoBehaviour
{
    [SerializeField]
    Image sfxButton;
    [SerializeField]
    Image musicButton;

    SFXManager manager;

    [SerializeField]
    Sprite[] sfxSprites;
    [SerializeField]
    Sprite[] musicSprites;

    private void Start()
    {
        if (SFXManager.Instance == null)
        {
            manager = GameSFXManager.Instance;
        }
        else
        {
            manager = SFXManager.Instance;
        }

        sfxButton.sprite = sfxSprites[manager.SFXState];
        musicButton.sprite = musicSprites[manager.MusicState];
    }

    public void ToggleSfx()
    {
        sfxButton.sprite = sfxSprites[manager.ToggleSFX()];
        manager.PlayClip("Toggle");
    }

    public void ToggleMusic()
    {
        manager.PlayClip("Toggle");
        musicButton.sprite = musicSprites[manager.ToggleMusic()];
    }
}
