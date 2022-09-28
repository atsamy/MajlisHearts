using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ArabicSupport;

public class MessageScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI message;
    [SerializeField]
    Image avatar;

    public void Set(string name,string message,bool toggleMessageColor)
    {
        print(message);
        playerName.text = ArabicFixer.Fix(name,false,false);
        this.message.text = ArabicFixer.Fix(message);

        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(name);
        GetComponent<Image>().enabled = toggleMessageColor;

        if (LanguageManager.Instance.CurrentLanguage == Language.Arabic)
        {
            this.message.alignment = TextAlignmentOptions.TopRight;
        }
    }
}
