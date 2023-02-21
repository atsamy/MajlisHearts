using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BalootMainPlayer : PlayerBaloot
{
    public event Action WaitOthers;

    public delegate void WaitPassCards();
    public event WaitPassCards OnWaitSelectType;

    public BalootMainPlayer(int index) : base(index)
    {
        isPlayer= false;
    }

    protected override void WaitForOthers()
    {
        WaitOthers?.Invoke();
    }

    public override void CheckGameType()
    {
        OnWaitSelectType?.Invoke();
    }

    internal void ForcePlay()
    {
        OnForcePlay?.Invoke();
    }

    public override void SetTurn(RoundInfo info)
    {
        base.SetTurn(info);
    }
}
