using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabScript : MonoBehaviour
{
    public Color[] TxtColors;
    //public Sprite[] BtnSprites;
    // Start is called before the first frame update
    //Image image;
    //[SerializeField]
    MaskableGraphic text;
    Button button;

    void Awake()
    {
        //image = GetComponent<Image>();
        if ((text = GetComponentInChildren<TextMeshProUGUI>()) == null)
            text = GetComponentInChildren<Text>();

        button = GetComponent<Button>();
    }

    public void Pressed(bool value)
    {
        //image.sprite = BtnSprites[index];
        text.color = TxtColors[value ? 1 : 0];
        button.interactable = !value;

        if (value)
            SFXManager.Instance.PlayClip("Tab");
    }
}
