using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterSelectionEvent : EventBase
{
    public override IEnumerator Perform(GameplayManager gameplayManager, GameplayManager.ClimbResult climbResult)
    {
        List<Character> availableCharacters = Team.Instance.GetPrefabs();
        for (int i = 0; i < availableCharacters.Count; i++)
        {
            Character character = availableCharacters[i];
            Character characterInstance = Instantiate(character);
            Team.Instance.characters.Add(characterInstance);
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

