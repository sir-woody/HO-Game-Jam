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

        Music       = 0x000100,
        Ui          = 0x000200,
        Voice       = 0x000300,
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
        Ui = AudioType.Ui,
        Voice = AudioType.Voice,
        Music = AudioType.Music,
    }

    [Serializable]
    private class SoundModel
    {
        public AudioSource source = null;
        public float maxVolume = 1;
        [HideInInspector]
        public float targetVolume = 0;
        [HideInInspector]
        public float currentVolume = 0;
        public float fadeDuration = 0.5f;
    }


    [SerializeField]
    private SoundModelDictionary soundModelDictionary = null;
    [SerializeField]
    [Range(0, 1)]
    private float masterVolume = 1;
    private Queue<AudioSource> sources = new Queue<AudioSource>();
    private AmbientType currentAmbientType = AmbientType.Undefined;


    private void Update()
    {
        foreach (KeyValuePair<AudioType, SoundModel> model in soundModelDictionary)
        {
            if (model.Value.fadeDuration <= 0)
            {
                model.Value.currentVolume = model.Value.targetVolume * masterVolume * model.Value.maxVolume;
            }
            else
            {
                model.Value.currentVolume = Mathf.MoveTowards(model.Value.currentVolume, model.Value.targetVolume, Time.deltaTime / model.Value.fadeDuration) * masterVolume * model.Value.maxVolume;
            }
            model.Value.source.volume = model.Value.currentVolume;
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
        }
        else
        {
            Debug.Log($"Sound {ambientType} not found in SoundManager");
        }
    }

    public void PlaySound(SoundType soundType)
    {
        if (soundModelDictionary.TryGetValue((AudioType)soundType, out SoundModel model) == true)
        {
            AudioSource source = Instantiate(model.source, model.source.transform, false).GetComponent<AudioSource>();
            source.PlayOneShot(model.source.clip);
            source.volume = 1;
            StartCoroutine(OnSoundFinished(source));
        }
        else
        {
            Debug.Log($"Sound {soundType} not found in SoundManager");
        }
    }

    private IEnumerator OnSoundFinished(AudioSource source)
    {
        while (source.isPlaying == true)
        {
            yield return null;
        }
        Destroy(source.gameObject);
    }

}