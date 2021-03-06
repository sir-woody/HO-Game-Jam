﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    public class ClimbResult
    {
        public EventBase eventPrefab;
        public bool hasReachedCrossroads;
        public bool hasBacktrackedCrossroads;
        public Vector2 markerPosition;
        public int crossroadsCount;
        public int selectedCrossroad;
    }

    [Header("Events")]
    [SerializeField]
    internal RectTransform tooltipParent = null;
    [SerializeField]
    internal RectTransform eventParent = null;
    [SerializeField]
    private RestEvent restEventPrefab = null;
    [SerializeField]
    private WinEvent winEventPrefab = null;
    [SerializeField]
    private GameOverEvent gameOverEventPrefab = null;
    [SerializeField]
    private CharacterSelectionEvent characterSelectionEventPrefab = null;

    [Header("Map")]
    [SerializeField]
    private MapManager mapManager = null;
    [SerializeField]
    private Transform marker = null;
    [SerializeField]
    private float markerLerp = 0.1f;
    [SerializeField]
    private MovementArrow upArrow = null;
    [SerializeField]
    private MovementArrow downArrow = null;

    private float distancePerSecond = 1;
    private bool restWasIssued;




    public void StartGame()
    {
        StartCoroutine(LoopCoroutine());
    }


    private IEnumerator LoopCoroutine()
    {
        /// Initialize Map
        mapManager.Show();
        mapManager.InitializeRoad();
        marker.transform.position = mapManager.Move(0);
        mapManager.Hide();

        /// Begin character selection
        yield return StartCoroutine(EventCoroutine(null, characterSelectionEventPrefab, true));

        mapManager.Show();

        /// Begin climb
        IEnumerator<ClimbResult> enumerator = ClimbCoroutine();
        while (enumerator.MoveNext() == true)
        {
            ClimbResult climbResult = enumerator.Current;

            /// Update marker's position
            marker.transform.position = Vector3.Lerp(marker.transform.position, climbResult.markerPosition, markerLerp);



            if (climbResult.hasReachedCrossroads == false && climbResult.hasBacktrackedCrossroads == false && climbResult.eventPrefab == null)
            {
                /// We are in the middle of the road, no event nor crossroads are close.
                /// Checking whether the user has issued Rest order.
                if (restWasIssued == true)
                {
                    /// Rest order was issued, performing Rest action.
                    yield return StartCoroutine(EventCoroutine(climbResult, restEventPrefab));
                    restWasIssued = false;
                }
                else if (TeamManager.Instance.TeamDied())
                {
                    /// Everyone died game is lost
                    /// Perform it, then clear
                    yield return StartCoroutine(EventCoroutine(climbResult, gameOverEventPrefab));
                    break;
                }
                else
                {
                    /// Wait a frame.
                    yield return null;
                }
                continue;
            }

            if (climbResult.eventPrefab != null)
            {
                /// We have encountered an event
                /// Perform it, then clear
                yield return StartCoroutine(EventCoroutine(climbResult, climbResult.eventPrefab));
                climbResult.eventPrefab = null;
            }

            if (climbResult.hasReachedCrossroads == false && climbResult.hasBacktrackedCrossroads == false)
            {
                /// We have encountered an event in the middle of the road,
                /// but we haven't reached the crossroad yet.
                /// Continue moving along the road.
                continue;
            }
            else if (climbResult.hasBacktrackedCrossroads == true)
            {
                /// We have backtracked.
                /// Moving to the previous road.
                mapManager.MoveToPreviousRoad();
                climbResult.selectedCrossroad = -1;
            }
            else if (mapManager.GetCrossroadsCount() == 0)
            {
                /// We've reached the end of the game
                yield return StartCoroutine(EventCoroutine(climbResult, winEventPrefab));
                break;
            }
            else
            {
                /// We've reached the end of the road.
                /// Passing onto the next road.
                mapManager.MoveToNextRoad(climbResult.selectedCrossroad);
                climbResult.selectedCrossroad = -1;
            }
        }

    }

    private IEnumerator<ClimbResult> ClimbCoroutine()
    {
        ClimbResult result = new ClimbResult();
        result.crossroadsCount = mapManager.GetCrossroadsCount();
        /// Setting the <see cref="ClimbResult.selectedCrossroad"/> to -1 because this value indicated that
        /// no crossroad has been selected yet or there is no change in previous selection (this is important in backtracking)
        result.selectedCrossroad = -1;
        do
        {
            HandleMapInput();

            result.markerPosition = mapManager.Move(distancePerSecond);
            result.hasReachedCrossroads = mapManager.HasReachedCrossroad();
            result.hasBacktrackedCrossroads = mapManager.HasBacktrackedCrossroad();
            result.eventPrefab = mapManager.GetUpcommingEvent();

            yield return result;
        }
        while (true);
    }

    public void IssueRest()
    {
        restWasIssued = true;
    }

    private void HandleMapInput()
    {
        if (restWasIssued == false && Input.GetKeyDown(KeyCode.Space))
        {
            restWasIssued = true;
        }

        float previousDistance = distancePerSecond;
        float teamSpeed = TeamManager.Instance.GetTeamSpeed();

        distancePerSecond =
            ((Input.GetKey(KeyCode.UpArrow) == true || Input.GetKey(KeyCode.W) == true || upArrow.IsPressed == true) ? teamSpeed : 0) +
            ((Input.GetKey(KeyCode.DownArrow) == true || Input.GetKey(KeyCode.S) == true || downArrow.IsPressed == true) ? -teamSpeed : 0);

        if (previousDistance == 0 && distancePerSecond != 0)
        {
            SoundManager.Instance.PlayAmbient(SoundManager.AmbientType.WalkOnSnow);
            TeamManager.Instance.StartClimbing();
        }
        else if (previousDistance != 0 && distancePerSecond == 0)
        {
            SoundManager.Instance.StopAmbient(SoundManager.AmbientType.WalkOnSnow);
            TeamManager.Instance.StopClimbing();
        }
    }

    private IEnumerator EventCoroutine(ClimbResult climbResult, EventBase eventPrefab, bool skipFirstFade = false)
    {
        Debug.Log("Performing new event");

                ClearTooltips();

        EventBase eventObject = Instantiate(eventPrefab, eventParent);
        eventObject.gameObject.SetActive(false);
        eventObject.PreShow();

        /// Stop movement if it was in progress
        if (distancePerSecond != 0)
        {
            distancePerSecond = 0;
            SoundManager.Instance.StopAmbient(SoundManager.AmbientType.WalkOnSnow);
            TeamManager.Instance.StopClimbing();
        }

        if (skipFirstFade == false)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeOut());
            SoundManager.Instance.StopAmbient(SoundManager.AmbientType.Outside, FadeManager.Instance.FadeDuration * 2);
            SoundManager.Instance.PlayAmbient(eventPrefab.AmbientSoundType, FadeManager.Instance.FadeDuration * 2);
        }
        ClearTooltips();
        mapManager.Hide();
        StatManager.Instance.Hide();
        eventObject.gameObject.SetActive(true);
        eventObject.Show();
        yield return StartCoroutine(FadeManager.Instance.FadeIn());
        
        eventObject.PostShow();

        yield return StartCoroutine(eventObject.Perform(this, climbResult));

        ClearTooltips();

        SoundManager.Instance.StopAmbient(eventPrefab.AmbientSoundType, FadeManager.Instance.FadeDuration * 2);
        SoundManager.Instance.PlayAmbient(SoundManager.AmbientType.Outside, FadeManager.Instance.FadeDuration * 2);
        eventObject.PreHide();

        yield return StartCoroutine(FadeManager.Instance.FadeOut());
        eventObject.Hide();
        eventObject.gameObject.SetActive(false);
        mapManager.Show();
        StatManager.Instance.Show();
        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        eventObject.PostHide();
        Destroy(eventObject.gameObject);
        ClearTooltips();
        
        Debug.Log("Event performed");
    }

    private void ClearTooltips()
    {
        if (tooltipParent)
        {
            foreach (Transform child in tooltipParent)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
