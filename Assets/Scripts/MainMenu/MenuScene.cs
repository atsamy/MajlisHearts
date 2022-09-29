using UnityEngine;
using System.Collections;

public class MenuScene : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);
        SFXManager.Instance.PlayClip("Open");
        MenuManager.Instance.CurrentScene = this;
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.ShowMain();
        SFXManager.Instance.PlayClip("Close");
        MenuManager.Instance.CurrentScene = null;
    }
}
