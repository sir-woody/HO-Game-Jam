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
    private float distanceBetweenNodes = 10;

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
        currentDistance = Mathf.Min(currentDistance + distance, distanceBetweenNodes);

        Vector2 currentPoint = GetCurrentCursorPosition();
        return currentPoint;
    }

    /// <summary>
    /// Returns the global position of the cursor in the scene.
    /// </summary>
    public Vector2 GetCurrentCursorPosition()
    {
        return currentRoad.roadCurve.GetPoint(currentDistance / distanceBetweenNodes);
    }

    /// <summary>
    /// This method returns a <see cref="EventScriptableObject"/> if an event should appear.
    /// Otherwise, returns null.
    /// Should be checked every frame.
    /// </summary>
    public EventScriptableObject GetUpcommingEvent()
    {
        if (currentDistance == distanceBetweenNodes)
        {
            return currentRoad.eventScriptableObject;
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
    public int GetAvailableRoadCount()
    {
        return nextNode.roads.Count;
    }

    /// <summary>
    /// Call after finishing an <see cref="Event"/> retrieved from <see cref="GetUpcommingEvent"/>.
    /// To get the cout of available roads, call <see cref="GetAvailableRoadCount"/>.
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
    }
}
