using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectTab : MonoBehaviour
{
    [SerializeField]
    Image avatarFrame;

    [SerializeField]
    Sprite[] avatarFrameSprites;


    Button button; 

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Pressed(bool value)
    {
        print(name + " " + value);
        button.interactable = !value;
        //avatarFrame.sprite = avatarFrameSprites[value?1:0];
    }
}
