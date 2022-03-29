using UnityEngine;
using System.Collections;

public abstract class MenuScene : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);

        SFXManager.Instance.PlayClip("Click");
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);

        SFXManager.Instance.PlayClip("Close");
    }

    //public MenuScene PreviousMenu;
}
