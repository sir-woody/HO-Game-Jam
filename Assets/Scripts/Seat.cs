using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Seat : MonoBehaviour
{
    [SerializeField]
    private Image image = null;
    [SerializeField]
    private Image maskImage = null;
    [SerializeField]
    private Button button = null;

    public Character Character { get; private set; } = null;

    public void SetCharacter(Character character)
    {
        this.Character = character;

        Sprite spriteIdle = character.GetSprite(Character.SpriteType.Idle);
        Sprite spriteHoover = character.GetSprite(Character.SpriteType.Hoover);

        image.sprite = spriteIdle;
        maskImage.sprite = spriteHoover;
        SpriteState spriteState = button.spriteState;
        /// Replacing pressedSprite and selectedSprite with null for better visual effect
        //spriteState.selectedSprite = spriteHoover;
        //spriteState.pressedSprite = spriteHoover;
        spriteState.highlightedSprite = spriteHoover;
        spriteState.selectedSprite = null;
        spriteState.pressedSprite = null;
        button.spriteState = spriteState;

        button.onClick.AddListener(character.OnCharacterClicked);

    }

}