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
/// An <see cref="EventBase"/> will allways happen at the end of a <see cref="Map.Road"/> segment.
/// <see cref="EventBase"/>s might also happen throughout a <see cref="Map.Road"/> segment.
/// </summary>
public abstract class EventBase : MonoBehaviour
{
    public abstract SoundManager.AmbientType AmbientSoundType { get; }
    public abstract IEnumerator Perform(GameplayManager gameplayManager, GameplayManager.ClimbResult climbResult);
    public virtual void PreShow() { }
    public abstract void Show();
    public virtual void PostShow() { }
    public virtual void PreHide() { }
    public abstract void Hide();
    public virtual void PostHide() { }
}