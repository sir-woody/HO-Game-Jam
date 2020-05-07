using System;
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
        public EventScriptableObject eventScriptableObject;
        public bool hasReachedCrossroads;
        public Vector2 markerPosition;
        public int crossroadsCount;
        public int selectedCrossroad;
    }

    [SerializeField]
    private MapManager mapManager = null;
    [SerializeField]
    private float distancePerSecond = 1;
    [SerializeField]
    private Transform marker = null;

    private bool restWasIssued;

    private void Start()
    {
        StartCoroutine(LoopCoroutine());
    }

    private void Update()
    {
        restWasIssued = restWasIssued || Input.GetKeyDown(KeyCode.Space);
    }

    private IEnumerator LoopCoroutine()
    {
        mapManager.InitializeRoad();
        IEnumerator<ClimbResult> enumerator = ClimbCoroutine();

        while (enumerator.MoveNext() == true)
        {
            ClimbResult climbResult = enumerator.Current;

            /// Update marker's position
            marker.transform.position = climbResult.markerPosition;

            if (climbResult.hasReachedCrossroads == false && climbResult.eventScriptableObject == null)
            {
                /// We are in the middle of the road, no event nor crossroads are close.
                /// Checking whether the user has issued Rest order.
                if (restWasIssued == true)
                {
                    /// Rest order was issued, performing Rest action.
                    yield return StartCoroutine(RestCoroutine());
                    restWasIssued = false;
                }
                else
                {
                    /// Wait a frame.
                    yield return null;
                }
                continue;
            }

            if (climbResult.eventScriptableObject != null)
            {
                /// We have encountered an event
                yield return StartCoroutine(EventCoroutine(climbResult));
            }

            if (climbResult.hasReachedCrossroads == false)
            {
                /// We have encountered an event in the middle of the road,
                /// but we haven't reached the crossroad yet.
                /// Continue moving along the road.
                continue;
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
        do
        {
            result.hasReachedCrossroads = mapManager.HasReachedCrossroad();
            result.markerPosition = mapManager.Move(distancePerSecond);
            result.eventScriptableObject = mapManager.GetUpcommingEvent();
            yield return result;
        }
        while (result.hasReachedCrossroads == false);
    }

    private IEnumerator RestCoroutine()
    {
        Debug.Log("Performing rest");
        yield return new WaitForSeconds(1);
        Debug.Log("Rest performed");
    }

    private IEnumerator EventCoroutine(ClimbResult climbResult)
    {
        Debug.Log("Performing new event");
        yield return new WaitForSeconds(1);
        climbResult.selectedCrossroad = UnityEngine.Random.Range(0, climbResult.crossroadsCount);
        Debug.Log("Event performed");
    }

}
