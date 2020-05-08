﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CrossroadsEvent : Event
{
    [SerializeField]
    private int selectedRoad = -1;
    [SerializeField]
    private int roadsCount = 2;

    public override IEnumerator Perform(GameplayManager.ClimbResult climbResult)
    {
        while (selectedRoad < 0 || selectedRoad >= roadsCount)
        {
            yield return null;
        }
        climbResult.selectedCrossroad = selectedRoad;
    }

    public override void Show()
    {
    }
    public override void Hide()
    {
    }

}