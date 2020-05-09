using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RestEvent : EventBase
{
    [SerializeField]
    private Button seatBottomLeft = null;
    [SerializeField]
    private Button seatBottomRight = null;
    [SerializeField]
    private Button seatTopLeft = null;
    [SerializeField]
    private Button seatTopRight = null;

    [Space]
    [SerializeField]
    private AudioClip tentUnpackSound = null;
    [SerializeField]
    private bool isDone = false;

    public override SoundManager.AmbientType AmbientSoundType => SoundManager.AmbientType.Inside;


    public override IEnumerator Perform(GameplayManager gameplayManager, GameplayManager.ClimbResult climbResult)
    {

        while (isDone == false)
        {
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
        var restHudController = GetComponentInChildren<HudController>();
        List<Sprite> spritesIdle = new List<Sprite>(TeamManager.Instance.GetCharacterSprites(Character.SpriteType.Idle));
        List<Sprite> spritesHoover = new List<Sprite>(TeamManager.Instance.GetCharacterSprites(Character.SpriteType.Hoover));
        List<Character> characters = new List<Character>(TeamManager.Instance.characters);
        List<Button> seats = new List<Button>()
        {
            seatBottomLeft,
            seatBottomRight,
            seatTopLeft,
            seatTopRight,
        };
        var seatsCopy = new List<Button>(seats);

        while (characters.Count > 0)
        {
            int characterIndex = characters.RemoveRandom(out Character character);
            Sprite spriteIdle = spritesIdle[characterIndex];
            Sprite spriteHoover = spritesHoover[characterIndex];
            spritesIdle.RemoveAt(characterIndex);
            spritesHoover.RemoveAt(characterIndex);

            seats.RemoveRandom(out Button seat);
            restHudController.BindStats(character, seatsCopy.IndexOf(seat));

            (seat.targetGraphic as Image).sprite = spriteIdle;
            SpriteState spriteState = seat.spriteState;
            spriteState.highlightedSprite = spriteHoover;
            /// Replacing pressedSprite and selectedSprite with null for better visual effect
            //spriteState.selectedSprite = spriteHoover;
            //spriteState.pressedSprite = spriteHoover;
            spriteState.selectedSprite = null;
            spriteState.pressedSprite = null;
            seat.spriteState = spriteState;

            seat.onClick.AddListener(character.OnCharacterClicked);
        }

        for (int i = 0; i < seats.Count; i++)
        {
            seats[i].gameObject.SetActive(false);
        }
    }
    public override void Hide()
    {

    }


    public void FinishRest()
    {
        isDone = true;
    }

    public void PerformActions()
    {
        Debug.LogError($"{nameof(PerformActions)} not implemented");
    }

}