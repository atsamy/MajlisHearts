using UnityEngine;
using System.Collections;

public abstract class MenuScene : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);
        MenuManager.Instance.HideMain(false,false);
        SFXManager.Instance.PlayClip("Open");
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.ShowMain();
        SFXManager.Instance.PlayClip("Close");
    }

    //public MenuScene PreviousMenu;
}
