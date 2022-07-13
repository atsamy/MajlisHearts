using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager : MonoBehaviour
{
    [HideInInspector]
    public Sprite playerAvatar;

    public static AvatarManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerAvatar(string name)
    {
        playerAvatar = Resources.Load<Sprite>("Avatar/Face/" + name);
    }

    public Sprite GetAvatarSprite(string name)
    {
        return Resources.Load<Sprite>("Avatar/Face" + name);
    }

    public Sprite GetAvatarBodySprite(string name)
    {
        return Resources.Load<Sprite>("Avatar/Body" + name);
    }

}
