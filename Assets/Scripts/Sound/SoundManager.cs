using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Serializable]
    private class SoundModelDictionary : SerializedDictionary<AudioType, SoundModel> { }

    public enum AudioType
    {
        Inside,
        Outside,
        WalkOnSnow,

        Music         = 0x0000100,
        Voice         = 0x0000200,
        Event         = 0x0000300,
    }
    public enum AmbientType
    {
        Undefined = -1,
        Inside = AudioType.Inside,
        Outside = AudioType.Outside,
        WalkOnSnow = AudioType.WalkOnSnow,
    }
    public enum SoundType
    {
        Music = AudioType.Music,
        Voice = AudioType.Voice,
        Event = AudioType.Event,
    }

    [Serializable]
    public struct SoundModel
    {
        public AudioSource source;
        public float maxVolume;
        [HideInInspector]
        public float targetVolume;
        [HideInInspector]
        public float currentVolume;
        public float fadeDuration;
        [HideInInspector]
        public float currentFadeDuration;
    }

    [SerializeField]
    [Range(0, 1)]
    private float masterVolume = 1;
    [SerializeField]
    private AudioClip mainTheme = null;
    [SerializeField]
    private List<AudioClip> eventSounds = null;

    [Space]
    [SerializeField]
    private SoundModelDictionary soundModelDictionary = null;

    private List<SoundModel> playingSounds = new List<SoundModel>();

    public AudioClip MainTheme => mainTheme;
    public IReadOnlyList<AudioClip> EventSounds => eventSounds;

    //private AmbientType currentAmbientType = AmbientType.Undefined;
    private Dictionary<AmbientType, SoundModel> playingAmbients = new Dictionary<AmbientType, SoundModel>();


    private void Update()
    {
        List<AudioType> list = soundModelDictionary.Keys.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            AudioType type = list[i];
            SoundModel model = soundModelDictionary[type];
            if (model.currentFadeDuration <= 0)
            {
                model.currentVolume = model.targetVolume * this.masterVolume * model.maxVolume;
            }
            else
            {
                model.currentVolume = Mathf.MoveTowards(model.currentVolume, model.targetVolume * this.masterVolume * model.maxVolume, Time.deltaTime / model.currentFadeDuration);
            }
            model.source.volume = model.currentVolume;
            soundModelDictionary[type] = model;
            if (playingAmbients.ContainsKey((AmbientType)type) == true)
            {
                playingAmbients[(AmbientType)type] = model;
            }
        }
        for (int i = 0; i < playingSounds.Count; i++)
        {
            SoundModel model = playingSounds[i];
            if (model.currentFadeDuration <= 0)
            {
                model.currentVolume = model.targetVolume * masterVolume * model.maxVolume;
            }
            else
            {
                model.currentVolume = Mathf.MoveTowards(model.currentVolume, model.targetVolume * masterVolume * model.maxVolume, Time.deltaTime / model.currentFadeDuration);
            }
            model.source.volume = model.currentVolume;
            playingSounds[i] = model;
        }
    }

    public void PlayAmbient(AmbientType ambientType, float fadeDuration = -1)
    {
        if (ambientType == AmbientType.Undefined)
        {
            Debug.LogError($"Trying to play AmbientType.Undefined");
            return;
        }
        if (playingAmbients.TryGetValue(ambientType, out SoundModel existingModel) == true)
        {
            Debug.Log($"Playing ambients: (unchanged)");
            return;
        }
        if (soundModelDictionary.TryGetValue((AudioType)ambientType, out SoundModel model) == true)
        {
            if (fadeDuration >= 0)
            {
                model.currentFadeDuration = fadeDuration;
            }
            else
            {
                model.currentFadeDuration = model.fadeDuration;
            }
            model.targetVolume = 1;
            playingAmbients.Add(ambientType, model);
            soundModelDictionary[(AudioType)ambientType] = model;
            Debug.Log($"Playing ambient: {ambientType}");
        }
        else
        {
            Debug.LogError($"Sound {ambientType} not found in SoundManager");
        }
    }
    public void StopAmbient(AmbientType ambientType, float fadeDuration = -1)
    {
        if (playingAmbients.TryGetValue(ambientType, out SoundModel model) == false)
        {
            Debug.Log($"Trying to stop an ambient of type {ambientType} which is not currently playing.");
            return;
        }
        if (fadeDuration >= 0)
        {
            model.currentFadeDuration = fadeDuration;
        }
        else
        {
            model.currentFadeDuration = model.fadeDuration;
        }
        model.targetVolume = 0;
        soundModelDictionary[(AudioType)ambientType] = model;
        playingAmbients.Remove(ambientType);
        Debug.Log($"Stopping ambient: {ambientType}");
    }

    public SoundModel PlaySound(SoundType soundType, AudioClip clip)
    {
        if (clip == null)
        {
            Debug.Log("Trying to play a null clip");
            return default;
        }
        if (soundModelDictionary.TryGetValue((AudioType)soundType, out SoundModel model) == true)
        {
            model.source = Instantiate(model.source, model.source.transform, false).GetComponent<AudioSource>();
            model.source.clip = clip;
            model.source.Play();
            model.targetVolume = 1;
            model.currentFadeDuration = model.fadeDuration;
            model.currentVolume = 1;
            playingSounds.Add(model);
            StartCoroutine(OnSoundFinished(model));
            return model;
        }
        else
        {
            Debug.Log($"Sound AudioType.Sound not found in SoundManager");
            return default;
        }
    }

    private IEnumerator OnSoundFinished(SoundModel model)
    {
        if (model.source.loop == true)
        {
            yield break;
        }
        while (model.source.isPlaying == true)
        {
            yield return null;
        }
        int index = playingSounds.FindIndex(x => x.source == model.source);
        playingSounds.RemoveAt(index);
        Destroy(model.source.gameObject);
    }

}