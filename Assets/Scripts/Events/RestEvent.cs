using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RestEvent : EventBase
{
    [SerializeField]
    private Seat seatBottomLeft = null;
    [SerializeField]
    private Seat seatBottomRight = null;
    [SerializeField]
    private Seat seatTopLeft = null;
    [SerializeField]
    private Seat seatTopRight = null;

    [Space]
    [SerializeField]
    private AudioClip tentUnpackSound = null;
    [SerializeField]
    private bool isDone = false;
    [SerializeField]
    private DeadAlpinistEvent scoutResultEventPrefab = null;
    private bool restIssued = false;
    private bool isRested = false;
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
            if(!isRested && restIssued)
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
        HudController restHudController = GetComponentInChildren<HudController>();
        List<Character> characters = new List<Character>(TeamManager.Instance.characters);
        List<Seat> seats = new List<Seat>()
        {
            seatBottomLeft,
            seatBottomRight,
            seatTopLeft,
            seatTopRight,
        };
        List<Seat> seatsCopy = new List<Seat>(seats);

        while (characters.Count > 0)
        {
            characters.RemoveRandom(out Character character);
            if (character.IsDead() == true)
            {
                continue;
            }
            
            seats.RemoveRandom(out Seat seat);
            restHudController.BindStats(character, seatsCopy.IndexOf(seat));
            seat.SetCharacter(character);
        }

        for (int i = 0; i < seats.Count; i++)
        {
            seats[i].gameObject.SetActive(false);
        }
    }
    public override void Hide()
    {
        BackpackManager.Instance.HideBackpack();
    }


    public void Done()
    {
        isDone = true;
    }

    public void Rest()
    {
        restIssued = true;
    }

    private IEnumerator PerformActions()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        TeamManager.Instance.characters.Except(scouts).ToList().ForEach(x => x.Rest());

        var foundItems = scouts.SelectMany(x => x.Scout());

        var foundItemsEvent = Instantiate(scoutResultEventPrefab, GameplayManager.Instance.eventParent);
        foundItemsEvent.AddLeftItems(foundItems);
        yield return StartCoroutine(foundItemsEvent.Perform(GameplayManager.Instance, null));
        isRested = true;

        yield return StartCoroutine(FadeManager.Instance.FadeIn());
    }

}