using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestEvent : EventBase
{

    [SerializeField]
    private Seat.SeatSerializedDictionary seats = null;

    [Space]
    [SerializeField]
    private AudioClip tentUnpackSound = null;
    [SerializeField]
    private bool isDone = false;
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
        restIssued = true;
    }

    private IEnumerator PerformActions()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        TeamManager.Instance.characters.ForEach(x => x.Rest());
        isRested = true;

        yield return StartCoroutine(FadeManager.Instance.FadeIn());
    }

}