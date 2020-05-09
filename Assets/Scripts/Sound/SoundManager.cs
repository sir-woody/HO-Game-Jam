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

        Sound_Music         = 0x0000100,
        Sound_Voice         = 0x0000200,
        Sound_Event         = 0x0000300,
    }
    public enum AmbientType
    {
        Undefined = -1,
        Unchanged = -2,
        Inside = AudioType.Inside,
        Outside = AudioType.Outside,
    }
    public enum SoundType
    {
        Music = AudioType.Sound_Music,
        Voice = AudioType.Sound_Voice,
        Event = AudioType.Sound_Event,
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

    private AmbientType currentAmbientType = AmbientType.Undefined;


    private void Update()
    {
        List<AudioType> list = soundModelDictionary.Keys.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            AudioType type = list[i];
            SoundModel model = soundModelDictionary[type];
            if (model.fadeDuration <= 0)
            {
                model.currentVolume = model.targetVolume * this.masterVolume * model.maxVolume;
            }
            else
            {
                model.currentVolume = Mathf.MoveTowards(model.currentVolume, model.targetVolume * this.masterVolume * model.maxVolume, Time.deltaTime / model.fadeDuration);
            }
            soundModelDictionary[type] = model;
            model.source.volume = model.currentVolume;
        }
        for (int i = 0; i < playingSounds.Count; i++)
        {
            SoundModel model = playingSounds[i];
            if (model.fadeDuration <= 0)
            {
                model.currentVolume = model.targetVolume * masterVolume * model.maxVolume;
            }
            else
            {
                model.currentVolume = Mathf.MoveTowards(model.currentVolume, model.targetVolume * masterVolume * model.maxVolume, Time.deltaTime / model.fadeDuration);
            }
            playingSounds[i] = model;
            model.source.volume = model.currentVolume;
        }
    }

    public void PlayAmbient(AmbientType ambientType, float fadeDuration = -1)
    {
        if (ambientType == AmbientType.Undefined)
        {
            Debug.LogError($"Trying to play AmbientType.Undefined");
            return;
        }
        if (ambientType == AmbientType.Unchanged || ambientType == currentAmbientType)
        {
            Debug.Log($"Playing ambient: {currentAmbientType} (unchanged)");
            return;
        }
        if (soundModelDictionary.TryGetValue((AudioType)ambientType, out SoundModel model) == true)
        {
            if (fadeDuration != -1)
            {
                model.fadeDuration = fadeDuration;
            }
            model.targetVolume = 1;
            AmbientType lastAmbientType = currentAmbientType;
            if (currentAmbientType != AmbientType.Undefined)
            {
                StopAmbient(currentAmbientType, fadeDuration);
            }
            currentAmbientType = ambientType;
            soundModelDictionary[(AudioType)ambientType] = model;
            Debug.Log($"Playing ambient: {currentAmbientType} previously: {lastAmbientType}");
        }
        else
        {
            Debug.LogError($"Sound {ambientType} not found in SoundManager");
        }
    }
    private void StopAmbient(AmbientType ambientType, float fadeDuration = -1)
    {
        if (soundModelDictionary.TryGetValue((AudioType)ambientType, out SoundModel model) == true)
        {
            if (fadeDuration != -1)
            {
                model.fadeDuration = fadeDuration;
            }
            model.targetVolume = 0;
            soundModelDictionary[(AudioType)ambientType] = model;
        }
        else
        {
            Debug.Log($"Sound {ambientType} not found in SoundManager");
        }
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
            SoundModel newModel = model;
            newModel.source = Instantiate(model.source, model.source.transform, false).GetComponent<AudioSource>();
            newModel.source.clip = clip;
            newModel.source.Play();
            newModel.targetVolume = 1;
            newModel.currentVolume = 1;
            playingSounds.Add(newModel);
            StartCoroutine(OnSoundFinished(newModel));
            return newModel;
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