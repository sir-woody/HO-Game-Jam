using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterSelectionEvent : EventBase
{
    [SerializeField]
    private int maxTeamSize = 4;

    public override SoundManager.AmbientType AmbientSoundType => SoundManager.AmbientType.Inside;

    public override IEnumerator Perform(GameplayManager gameplayManager, GameplayManager.ClimbResult climbResult)
    {
        List<Character> availableCharacters = TeamManager.Instance.GetPrefabs();
        for (int i = 0; i < Mathf.Min(availableCharacters.Count, maxTeamSize); i++)
        {
            Character character = availableCharacters[i];
            Character characterInstance = Instantiate(character);
            TeamManager.Instance.characters.Add(characterInstance);
            gameplayManager.HudController.BindStats(characterInstance, i);
        }

        Debug.Log("TODO: add character selection implementation");
        yield return new WaitForSeconds(1);
    }

    public override void Show()
    {
    }

    public override void Hide()
    {
    }

}

