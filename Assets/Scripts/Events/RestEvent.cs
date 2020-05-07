using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RestEvent : Event
{
    [SerializeField]
    private bool isDone = false;

    public override IEnumerator Perform(GameplayManager.ClimbResult climbResult)
    {
        while (isDone == false)
        {
            yield return null;
        }
    }

}