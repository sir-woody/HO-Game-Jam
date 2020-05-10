using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RestEvent : EventBase
{

    [SerializeField]
    private Seat.SeatSerializedDictionary seats = null;

    [Space]
    [SerializeField]
    private AudioClip tentUnpackSound = null;
    [SerializeField]
    private bool isDone = false;
    [SerializeField]
    private Button performButton = null;
    [SerializeField]
    private DeadAlpinistEvent scoutResultEventPrefab = null;

    private bool shouldPerformAction = false;
    private bool restActionPerformed = false;
    private Character[] scouts;

    public override SoundManager.AmbientType AmbientSoundType
    {
        get
        {
            return SoundManager.AmbientType.Inside;
        }
    }

    public override IEnumerator Perform(GameplayManager gameplayManager, GameplayManager.ClimbResult climbResult)
    {
        scouts = new[] { TeamManager.Instance.characters[0] };
        while (isDone == false)
        {
            if(!restActionPerformed && shouldPerformAction)
            {
                yield return StartCoroutine(PerformActions());
            }
            yield return null;
        }
    }

    public override void PreShow()
    {
        SoundManager.SoundModel soundModel = SoundManager.Instance.PlaySound(SoundManager.SoundType.Event, tentUnpackSound);
        soundModel.source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
    }

    public override void Show()
    {
        StatManager.Instance.Show();
        List<Character> characters = new List<Character>(TeamManager.Instance.characters);

        foreach (KeyValuePair<Seat.SeatPosition, Seat> pair in seats)
        {
            pair.Value.gameObject.SetActive(false);
        }

        while (characters.Count > 0)
        {
            characters.RemoveRandom(out Character character);
            if (character.IsDead() == true)
            {
                continue;
            }
            Seat seat = seats[character.seatPosition];
            seat.gameObject.SetActive(true);

            seat.SetCharacter(character);
        }

    }
    public override void Hide()
    {
        StatManager.Instance.Hide();
        BackpackManager.Instance.HideBackpack();
    }


    public void Done()
    {
        isDone = true;
    }

    public void Rest()
    {
        shouldPerformAction = true;
    }

    private IEnumerator PerformActions()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        List<BackpackManager.ItemType> foundItems = new List<BackpackManager.ItemType>();
        foreach (Character character in TeamManager.Instance.characters)
        {
            if (character.IsRestActionSleep() == true)
            {
                character.Rest();
            }
            else
            {
                IEnumerable<BackpackManager.ItemType> scoutedItems = character.Scout();
                foundItems.AddRange(scoutedItems);
            }
        }

        if (foundItems.Count > 0)
        {
            DeadAlpinistEvent foundItemsEvent = Instantiate(scoutResultEventPrefab, GameplayManager.Instance.eventParent, false);
            foundItemsEvent.AddLeftItems(foundItems);
            yield return StartCoroutine(FadeManager.Instance.FadeIn());
            yield return StartCoroutine(foundItemsEvent.Perform(GameplayManager.Instance, null));
            yield return StartCoroutine(FadeManager.Instance.FadeOut());
            Destroy(foundItemsEvent.gameObject);
        }

        restActionPerformed = true;
        performButton.interactable = false;

        yield return StartCoroutine(FadeManager.Instance.FadeIn());
    }

}