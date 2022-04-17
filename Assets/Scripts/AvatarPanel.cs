using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AvatarPanel : MonoBehaviour
{
    Action<int> selected;
    public void SelectAvatar(int index)
    {
        PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>()
        {
            { "Avatar", "Avatar" + index.ToString() }
        });

        selected?.Invoke(index);

        GameManager.Instance.MyPlayer.Avatar = "Avatar" + index;
        gameObject.SetActive(false);
    }

    public void Open(Action<int> OnSelected)
    {
        selected = OnSelected;
        gameObject.SetActive(true);
    }

}
