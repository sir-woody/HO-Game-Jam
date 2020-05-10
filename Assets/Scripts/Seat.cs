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
    private Button button = null;

    private Character character = null;
    public Character Character => character;

    public void SetCharacter(Character character)
    {
        this.character = character;

        Sprite spriteIdle = character.GetSprite(Character.SpriteType.Idle);
        Sprite spriteHoover = character.GetSprite(Character.SpriteType.Hoover);

        image.sprite = spriteIdle;
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