using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EventScriptableObject : ScriptableObject
{
    [SerializeField]
    private Event eventPrefab = null;
    /// <summary>
    /// This is a reference to a prefab - call <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/>
    /// to create a new instance of it.
    /// </summary>
    public Event EventPrefab => eventPrefab;
}