using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectGroup : MonoBehaviour
{
    public string GroupID;

    [SerializeField]
    Button[] buttons;

    [SerializeField]
    bool saveSelection;

    public int GroupIndex { get; private set; } = 0;

    private void Start()
    {
        if (saveSelection)
            GroupIndex = PlayerPrefs.GetInt(GroupID, 0);

        buttons[GroupIndex].interactable = false;
    }

    public void Select(int index)
    {
        if (saveSelection)
            PlayerPrefs.SetInt(GroupID, index);

        buttons[GroupIndex].interactable = true;
        buttons[index].interactable = false;

        GroupIndex = index;
    }
}
