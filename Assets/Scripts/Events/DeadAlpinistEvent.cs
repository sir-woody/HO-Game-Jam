using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DeadAlpinistEvent : EventBase
{
    [SerializeField] 
    List<BackpackManager.ItemType> leftItems = null;
    [SerializeField]
    private Backpack backpackPrefab = null;
    [SerializeField]
    private RectTransform backpackSlot = null;
    [SerializeField]
    private GraphicRaycaster raycaster = null;

    private Backpack backpack;

    private void SpawnBackpack(Item[] items)
    {
        backpack = Instantiate(backpackPrefab, backpackSlot, false);
        backpack.Show(items, raycaster, false);
    }

    private bool isDone;

    public override SoundManager.AmbientType AmbientSoundType => SoundManager.AmbientType.Outside;

    public override IEnumerator Perform(GameplayManager gameplayManager, GameplayManager.ClimbResult climbResult)
    {
        int random = UnityEngine.Random.Range(0, SoundManager.Instance.EventSounds.Count);
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Event, SoundManager.Instance.EventSounds[random]);

        var items = leftItems.Select(x => BackpackManager.Instance.SpawnItem(x)).ToArray();
        SpawnBackpack(items);

        while (isDone == false)
        {
            if (backpack.IsEmpty()) Done();
            yield return null;
        }

    }

    public override void Show()
    {
    }
    public override void Hide()
    {
    }
    public void Done()
    {
        isDone = true;
    }
}