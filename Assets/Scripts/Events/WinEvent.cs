﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WinEvent : Event
{
    public override IEnumerator Perform(GameplayManager.ClimbResult climbResult)
    {
        Debug.Log("You win!");
        while (true)
        {
            yield return null;
        }
    }

    public override void Show()
    {

    }
    public override void Hide()
    {

    }
}
