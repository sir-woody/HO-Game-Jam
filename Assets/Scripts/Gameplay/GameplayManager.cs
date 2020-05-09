﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
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
    [SerializeField]
    private FadeManager fade = null;

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

    public HudController HudController => hudController;

    private void Start()
    {
        StartCoroutine(LoopCoroutine());
    }

    private void Update()
    {
        restWasIssued = restWasIssued || Input.GetKeyDown(KeyCode.Space);
        distancePerSecond =
            (Input.GetKey(KeyCode.UpArrow) == true ? defaultSpeed : 0) +
            (Input.GetKey(KeyCode.DownArrow) == true ? -defaultSpeed : 0);
        
        if (distancePerSecond != 0) Team.Instance.StartClimbing(); else Team.Instance.StopClimbing();
    }

    private IEnumerator LoopCoroutine()
    {
        mapManager.InitializeRoad();
        marker.transform.position = mapManager.Move(0);

        yield return StartCoroutine(EventCoroutine(null, characterSelectionEventPrefab, true));

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
                
                enumerator = ClimbCoroutine();
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
            result.markerPosition = mapManager.Move(distancePerSecond);
            result.hasReachedCrossroads = mapManager.HasReachedCrossroad();
            result.hasBacktrackedCrossroads = mapManager.HasBacktrackedCrossroad();
            result.eventPrefab = mapManager.GetUpcommingEvent();
            yield return result;
        }
        while (result.hasReachedCrossroads == false);
    }

    private IEnumerator CharacterSelectionCoroutine(ClimbResult climbResult)
    {
        Debug.Log("Performing character selection");

        mapManager.Hide();
        hudController.Hide();
        EventBase eventObject = Instantiate(characterSelectionEventPrefab, eventParent);
        eventObject.Show();
        yield return StartCoroutine(fade.FadeIn());

        yield return StartCoroutine(eventObject.Perform(this, climbResult));

        yield return StartCoroutine(fade.FadeOut());
        eventObject.Hide();
        Destroy(eventObject.gameObject);
        mapManager.Show();
        hudController.Show();
        yield return StartCoroutine(fade.FadeIn());


        //var i = 0;
        //List<Character> availableCharacters = Team.Instance.GetPrefabs();
        //foreach (Character character in availableCharacters)
        //{
        //    Character characterInstance = Instantiate(character);
        //    Team.Instance.characters.Add(characterInstance);
        //    hudController.BindStats(characterInstance, i++);
        //}

        Debug.Log("Character selection performed");
    }

    private IEnumerator EventCoroutine(ClimbResult climbResult, EventBase eventPrefab, bool skipFirstFade = false)
    {
        Debug.Log("Performing new event");

        if (skipFirstFade == false)
        {
            yield return StartCoroutine(fade.FadeOut());
        }
        mapManager.Hide();
        hudController.Hide();
        EventBase eventObject = Instantiate(eventPrefab, eventParent);
        eventObject.Show();
        yield return StartCoroutine(fade.FadeIn());

        yield return StartCoroutine(eventObject.Perform(this, climbResult));

        yield return StartCoroutine(fade.FadeOut());
        eventObject.Hide();
        Destroy(eventObject.gameObject);
        mapManager.Show();
        hudController.Show();
        yield return StartCoroutine(fade.FadeIn());
        
        Debug.Log("Event performed");
    }

}
