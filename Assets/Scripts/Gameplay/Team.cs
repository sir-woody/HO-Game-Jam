using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Team : Singleton<Team>
{
    [SerializeField]
    private List<Character> characterPrefabs = null;

    [HideInInspector]
    public List<Character> characters;

    public void StartClimbing()
    {
        foreach (Character character in characters)
        {
            character.StartClimbing();
        }
    }

    public void StopClimbing()
    {
        foreach (Character character in characters)
        {
            character.StopClimbing();
        }
    }

    public List<Sprite> GetCharacterSprites(Character.SpriteType spriteType)
    {
        List<Sprite> sprites = new List<Sprite>(characters.Count);
        foreach (Character character in characters)
        {
            Sprite sprite = character.GetSprite(spriteType);
            sprites.Add(sprite);
        }
        return sprites;
    }

    public List<Character> GetPrefabs()
    {
        return new List<Character>(characterPrefabs);
    }

}
