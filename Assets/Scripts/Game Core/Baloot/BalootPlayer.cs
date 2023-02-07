using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootPlayer : PlayerBase
{
    public delegate void TypeSelected(int playerIndex, BalootGameType type);
    public TypeSelected OnTypeSelected;

    public BalootPlayer(int index) : base(index)
    {

    }

    public virtual void CheckGameType()
    {
        
    }

    public virtual void SelectType(BalootGameType type)
    {
        OnTypeSelected?.Invoke(index, type);
    }
}
