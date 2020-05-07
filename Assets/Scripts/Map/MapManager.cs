using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Map map = null;
    [SerializeField]
    private RectTransform visuals = null;

    private Map.Node currentNode;
    private Map.Node nextNode;
    private Map.Road currentRoad;

    private float currentDistance = 0;

    public void InitializeRoad()
    {
        currentNode = map.Nodes[0];
        currentRoad = currentNode.roads[0];
        nextNode = map.Nodes[currentRoad.nextNodeIndex];
    }

    /// <summary>
    /// Moves the cursor along current road.
    /// </summary>
    public Vector2 Move(float distancePerSecond)
    {
        float distance = distancePerSecond * Time.deltaTime;
        currentDistance = Mathf.Min(currentDistance + distance, currentRoad.crossDuration);

        Vector2 currentPoint = GetCurrentCursorPosition();
        return currentPoint;
    }

    /// <summary>
    /// Returns the global position of the cursor in the scene.
    /// </summary>
    public Vector2 GetCurrentCursorPosition()
    {
        return currentRoad.roadCurve.GetPoint(currentDistance / currentRoad.crossDuration);
    }

    /// <summary>
    /// Returns true if the distance traveled is equal to the length of the road.
    /// The road can be traveled using <see cref="Move(float)"/> method.
    /// </summary>
    public bool HasReachedCrossroad()
    {
        return currentDistance == currentRoad.crossDuration;
    }
    /// <summary>
    /// This method returns a <see cref="EventScriptableObject"/> if an event should appear.
    /// Otherwise, returns null.
    /// Should be checked every frame.
    /// </summary>
    public Event GetUpcommingEvent()
    {
        if (currentDistance == currentRoad.crossDuration)
        {
            return currentRoad.eventPrefab;
        }
        else
        {
            /// TODO: add random events throughout the road
            return null;
        }
    }

    /// <summary>
    /// Returns the numer of available roads on the next node.
    /// Use in pair with <see cref="MoveToNextRoad(int)"/>.
    /// </summary>
    public int GetCrossroadsCount()
    {
        return nextNode.roads.Count;
    }

    /// <summary>
    /// Call after finishing an <see cref="Event"/> retrieved from <see cref="GetUpcommingEvent"/>.
    /// To get the cout of available roads, call <see cref="GetCrossroadsCount"/>.
    /// </summary>
    public void MoveToNextRoad(int roadIndex)
    {
        if (roadIndex < 0 || roadIndex >= nextNode.roads.Count)
        {
            Debug.LogError($"Road index out of range (roadIndex: {roadIndex}, node roads count: {currentNode.roads.Count})");
            return;
        }

        currentNode = nextNode;
        currentRoad = nextNode.roads[roadIndex];
        nextNode = map.Nodes[currentRoad.nextNodeIndex];
        currentDistance = 0;
    }

    public void Show()
    {
        visuals.gameObject.SetActive(true);
    }

    public void Hide()
    {
        visuals.gameObject.SetActive(false);
    }
}
