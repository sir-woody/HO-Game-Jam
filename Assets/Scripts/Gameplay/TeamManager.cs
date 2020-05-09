using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TeamManager : Singleton<TeamManager>
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

    public bool TeamDied() => characters.All(x => x.IsDead());

    public float GetTeamSpeed() => characters.Average(x => x.GetCurrentSpeed());

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
