using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementsHolder : MonoBehaviour
{
    public Text DebugText;
    public PausePanel PausePanel;
    public LevelPanel LevelPanel;
    public WaitingScript WaitingPanel;
    public Image TableTop;
    public GameObject ScoresHolder;
    public Popup HostLeftPopup;
    public GameObject GamePanel;
    public Transform DragCardHolder;

    [Space]
    [Header("Emojis Elements")]
    [Space]
    public Image[] EmojiImages;
    public Sprite[] Emojes;
    public GameObject EmojiButton;
    public GameObject EmojiPanel;

    public Sprite CardBack;

    public void PauseGame()
    {
        PausePanel.Show();
        GameSFXManager.Instance.PlayClip("Click");
    }
}
