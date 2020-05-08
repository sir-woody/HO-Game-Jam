using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RestEvent : Event
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
    private bool isDone = false;


    public override IEnumerator Perform(GameplayManager.ClimbResult climbResult)
    {

        while (isDone == false)
        {
            yield return null;
        }
    }

    public override void Show()
    {
        List<Sprite> spritesIdle = new List<Sprite>(Team.Instance.GetCharacterSprites(Character.SpriteType.Idle));
        List<Sprite> spritesHoover = new List<Sprite>(Team.Instance.GetCharacterSprites(Character.SpriteType.Hoover));
        List<Character> characters = new List<Character>(Team.Instance.characters);
        List<Button> seats = new List<Button>()
        {
            seatBottomLeft,
            seatBottomRight,
            seatTopLeft,
            seatTopRight,
        };

        while (characters.Count > 0)
        {
            int characterIndex = RemoveRandom(characters, out Character character);
            Sprite spriteIdle = spritesIdle[characterIndex];
            Sprite spriteHoover = spritesHoover[characterIndex];
            spritesIdle.RemoveAt(characterIndex);
            spritesHoover.RemoveAt(characterIndex);

            RemoveRandom(seats, out Button seat);

            (seat.targetGraphic as Image).sprite = spriteIdle;
            SpriteState spriteState = seat.spriteState;
            spriteState.highlightedSprite = spriteHoover;
            spriteState.selectedSprite = spriteHoover;
            /// Replacing pressedSprite with null for better visual effect
            //spriteState.pressedSprite = spriteHoover;
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

    private int RemoveRandom<T>(List<T> list, out T element)
    {
        if (list.Count == 0)
        {
            Debug.LogError("Trying to retrieve an element from an empty list");
            element = default;
            return -1;
        }
        int random = UnityEngine.Random.Range(0, list.Count);
        element = list[random];
        list.RemoveAt(random);
        return random;
    }

}