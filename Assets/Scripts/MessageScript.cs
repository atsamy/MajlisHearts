using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using ArabicSupport;

public class MessageScript : MonoBehaviour
{
    [SerializeField]
    Text playerName;
    [SerializeField]
    Text message;
    [SerializeField]
    Image avatar;

    public void Set(string name,string message)
    {
        playerName.text = ArabicFixer.Fix(name);
        this.message.text = ArabicFixer.Fix(message);
        this.avatar.sprite = AvatarManager.Instance.GetPlayerAvatar(name);

        if (LanguageManager.Instance.CurrentLanguage == Language.Arabic)
        {
            this.message.alignment = TextAnchor.UpperRight;
        }
    }
}
