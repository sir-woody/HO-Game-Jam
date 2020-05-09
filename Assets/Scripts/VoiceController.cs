using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class VoiceController : MonoBehaviour
{
    [SerializeField]
    private float minInterval = 5;
    [SerializeField]
    private List<AudioClip> onClickSounds = null;

    private float lastSoundTime;

    private void Awake()
    {
        lastSoundTime = -minInterval;
    }

    public AudioClip GetOnClickSound()
    {
        float time = Time.time;
        if (onClickSounds.Count > 0 && time - lastSoundTime >= minInterval)
        {
            this.lastSoundTime = time;
            int random = UnityEngine.Random.Range(0, onClickSounds.Count);
            return onClickSounds[random];
        }
        return null;
    }
}