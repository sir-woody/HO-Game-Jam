using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CrossroadsEvent : EventBase
{
    [SerializeField]
    private int selectedRoad = -1;
    [SerializeField]
    private int roadsCount = 2;

    public override SoundManager.AmbientType AmbientSoundType => SoundManager.AmbientType.Outside;


    public override IEnumerator Perform(GameplayManager gameplayManager, GameplayManager.ClimbResult climbResult)
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

    public void SelectRoad(int road)
    {
        selectedRoad = road;
    }
}