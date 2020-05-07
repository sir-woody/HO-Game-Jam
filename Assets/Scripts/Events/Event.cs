using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// TODO: finish this class.
/// This is the base class for all events occuring throughout the game.
/// An <see cref="Event"/> will allways happen at the end of a <see cref="Map.Road"/> segment.
/// <see cref="Event"/>s might also happen throughout a <see cref="Map.Road"/> segment.
/// </summary>
public abstract class Event : MonoBehaviour
{
    public abstract IEnumerator Perform(GameplayManager.ClimbResult climbResult);
}