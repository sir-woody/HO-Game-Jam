﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
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

    [Header("Canvas")]
    [SerializeField]
    private HudController hudController = null;

    [Header("Events")]
    [SerializeField]
    private RectTransform eventParent = null;
    [SerializeField]
    private RestEvent restEventPrefab = null;
    [SerializeField]
    private CharacterSelectionEvent characterSelectionEventPrefab = null;

    [Header("Map")]
    [SerializeField]
    private MapManager mapManager = null;
    [SerializeField]
    private Transform marker = null;
    [SerializeField]
    private float defaultSpeed = 1;

    private float distancePerSecond = 1;
    private bool restWasIssued;

    public HudController HudController
    {
        get
        {
            return hudController;
        }
    }


    public void StartGame()
    {
        StartCoroutine(LoopCoroutine());
    }


    private IEnumerator LoopCoroutine()
    {
        /// Initialize Map
        mapManager.InitializeRoad();
        marker.transform.position = mapManager.Move(0);


        /// Begin character selection
        yield return StartCoroutine(EventCoroutine(null, characterSelectionEventPrefab, true));

        /// Begin climb
        IEnumerator<ClimbResult> enumerator = ClimbCoroutine();
        while (enumerator.MoveNext() == true)
        {
            ClimbResult climbResult = enumerator.Current;

            /// Update marker's position
            marker.transform.position = climbResult.markerPosition;

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

    private void HandleMapInput()
    {
        restWasIssued = restWasIssued || Input.GetKeyDown(KeyCode.Space);
        distancePerSecond =
            (Input.GetKey(KeyCode.UpArrow) == true ? defaultSpeed : 0) +
            (Input.GetKey(KeyCode.DownArrow) == true ? -defaultSpeed : 0);

        if (distancePerSecond != 0)
        {
            TeamManager.Instance.StartClimbing();
        }
        else
        {
            TeamManager.Instance.StopClimbing();
        }
    }

    private IEnumerator EventCoroutine(ClimbResult climbResult, EventBase eventPrefab, bool skipFirstFade = false)
    {
        Debug.Log("Performing new event");



        if (skipFirstFade == false)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeOut());
            SoundManager.Instance.PlayAmbient(eventPrefab.AmbientSoundType, FadeManager.Instance.FadeDuration * 2);
        }

        TeamManager.Instance.StopClimbing();
        mapManager.Hide();
        hudController.Hide();
        EventBase eventObject = Instantiate(eventPrefab, eventParent);
        eventObject.Show();
        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        yield return StartCoroutine(eventObject.Perform(this, climbResult));


        SoundManager.Instance.PlayAmbient(SoundManager.AmbientType.Outside, FadeManager.Instance.FadeDuration * 2);
        yield return StartCoroutine(FadeManager.Instance.FadeOut());
        eventObject.Hide();
        Destroy(eventObject.gameObject);
        mapManager.Show();
        hudController.Show();
        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        Debug.Log("Event performed");
    }

}
