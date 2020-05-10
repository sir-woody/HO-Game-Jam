using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private CanvasGroup menuVisuals = null;
    [SerializeField]
    private Button startGameButton = null;


    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);

        /// Initialize sounds
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Music, SoundManager.Instance.MainTheme);
        SoundManager.Instance.PlayAmbient(SoundManager.AmbientType.Inside, 1);
        
        /// Set screen to black, then fade in
        FadeManager.Instance.SetFade(1);
        StartCoroutine(FadeManager.Instance.FadeIn());
    }

    public void OnStartGameButtonClicked()
    {
        menuVisuals.blocksRaycasts = false;
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        int random = UnityEngine.Random.Range(0, SoundManager.Instance.EventSounds.Count);
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Event, SoundManager.Instance.EventSounds[random]);
        yield return StartCoroutine(FadeManager.Instance.FadeOut(2));
        menuVisuals.gameObject.SetActive(false);
        GameplayManager.Instance.StartGame();
    }
}
