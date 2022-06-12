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

    [SerializeField]
    Color offColor;

    private void Awake()
    {
        sfxButton.color = SFXManager.Instance.SFXState == 0 ? offColor : Color.white;
        musicButton.color = SFXManager.Instance.SFXState == 0 ? offColor : Color.white;
    }

    public void ToggleSfx()
    {
        sfxButton.color = SFXManager.Instance.ToggleSFX() ? offColor : Color.white;
    }

    public void ToggleMusic()
    {
        musicButton.color = SFXManager.Instance.ToggleMusic() ? offColor : Color.white;
    }
}
