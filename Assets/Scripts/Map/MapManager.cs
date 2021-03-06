﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [SerializeField]
    private Map map = null;
    [SerializeField]
    private RectTransform visuals = null;

    private Map.Node currentNode;
    private Map.Road currentRoad;
    private List<Map.Road> currentPath;

    private float currentDistance = 0;

    private void Awake()
    {
        Hide();
    }

    public void InitializeRoad()
    {
        currentNode = map.Nodes[0];
        currentRoad = currentNode.roads[0];
        currentPath = new List<Map.Road>()
        {
            currentRoad,
        };
    }

    /// <summary>
    /// Moves the cursor along current road.
    /// </summary>
    public Vector2 Move(float distancePerSecond)
    {
        float distance = distancePerSecond * Time.deltaTime;
        float previousDistance = currentDistance;
        Vector2 currentPoint = (Vector2)currentRoad.roadCurve.MoveAlongSpline(ref currentDistance, distance) + map.mapOffset;
        //if (currentDistance - previousDistance > 2 * distance)
        //{
        //    Debug.LogWarning("Crossed distance was greater than twice the distance it should. Hotfixing.");
        //    currentDistance = 2 * distance;
        //}

        if (currentNode == map.Nodes[0])
        {
            /// We can't go back if we are on the first node (root node)
            currentDistance = Mathf.Max(currentDistance, 0);
        }

        //Vector2 currentPoint = GetCurrentCursorPosition();
        return currentPoint;
    }

    /// <summary>
    /// Returns the global position of the cursor in the scene.
    /// </summary>
    public Vector2 GetCurrentCursorPosition()
    {
        float clampedDistance = Mathf.Clamp(currentDistance, 0, 1);
        Vector2 point = this.currentRoad.roadCurve.GetPoint(clampedDistance);
        return point;
    }

    /// <summary>
    /// Returns true if the distance traveled is equal to the length of the road.
    /// The road can be traveled using <see cref="Move(float)"/> method.
    /// </summary>
    public bool HasReachedCrossroad()
    {
        return currentDistance > 1;
    }
    public bool HasBacktrackedCrossroad()
    {
        return currentDistance < 0;
    }
    /// <summary>
    /// This method returns a <see cref="EventScriptableObject"/> if an event should appear.
    /// Otherwise, returns null.
    /// Should be checked every frame.
    /// </summary>
    public EventBase GetUpcommingEvent()
    {
        if (currentDistance > 1)
        {
            if (currentRoad != currentPath[currentPath.Count - 1])
            {
                /// We have backtracked.
                /// Events on backtracked roads don't occure again unless <see cref="Map.Road.performEventOnBacktrack"/> is true.
                return currentRoad.performEventOnBacktrack == true ? currentRoad.eventPrefab : null;
            }
            else
            {
                return currentRoad.eventPrefab;
            }
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
        return map.Nodes[currentRoad.nextNodeIndex].roads.Count;
    }

    /// <summary>
    /// Call after finishing an <see cref="EventBase"/> retrieved from <see cref="GetUpcommingEvent"/>.
    /// To get the cout of available roads, call <see cref="GetCrossroadsCount"/>.
    /// </summary>
    public void MoveToNextRoad(int roadIndex)
    {
        if (currentPath[currentPath.Count - 1] != currentRoad)
        {
            /// Our team has backtracked and is currently passing a crossroad they have already passed in the past.
            /// No event will happen this time, team cannot change the once chosen path
            int currentRoadIndex = currentPath.IndexOf(currentRoad);
            currentNode = map.Nodes[currentRoad.nextNodeIndex];
            if (roadIndex == -1)
            {
                currentRoad = currentPath[currentRoadIndex + 1];
            }
            else
            {
                int patchSize = currentPath.Count - 1;
                for (int i = currentRoadIndex; i < patchSize; i++)
                {
                    currentPath.RemoveAt(currentRoadIndex + 1);
                }
                currentRoad = currentNode.roads[roadIndex];
                currentPath.Add(currentRoad);
            }
            currentDistance = 0;
            /// Not setting the value of nextNode because it is always set to the furthest node on our current path, event after we backtrack.
        }
        else
        {
            /// Entering a new crossroads
            if (map.Nodes[currentRoad.nextNodeIndex].roads.Count == 1)
            {
                /// Param <param name="roadIndex"/> was set to -1 meaning no path was chosen
                /// The crossroad has only a single path - this path will be chosen
                roadIndex = 0;
            }
            else if (roadIndex < 0 || roadIndex >= map.Nodes[currentRoad.nextNodeIndex].roads.Count)
            {
                Debug.LogError($"Road index out of range (roadIndex: {roadIndex}, node roads count: {currentNode.roads.Count})");
                return;
            }

            currentNode = map.Nodes[currentRoad.nextNodeIndex];
            currentRoad = currentNode.roads[roadIndex];
            currentPath.Add(currentRoad);
            currentDistance = 0;
        }
    }
    /// <summary>
    /// Should be called if <see cref="HasBacktrackedCrossroad"/> is true.
    /// </summary>
    public void MoveToPreviousRoad()
    {
        int currentRoadIndex = currentPath.IndexOf(currentRoad);
        if (currentRoadIndex == 0)
        {
            Debug.LogError($"Trying to go back past the first node of the map (the root node)");
            return;
        }
        currentRoad = currentPath[currentRoadIndex - 1];
        if (currentRoadIndex == 1)
        {
            currentNode = map.Nodes[0];
        }
        else
        {
            currentNode = map.Nodes[currentPath[currentRoadIndex - 2].nextNodeIndex];
        }
        currentDistance = 1;
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
