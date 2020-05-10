using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    [Serializable]
    public class Road
    {
        public EventBase eventPrefab;
        public int nextNodeIndex;
        public bool performEventOnBacktrack = false;
        public BezierSolution.BezierSpline roadCurve;
        public ColorRef color;
    }

    [Serializable]
    public class Node
    {

        public Vector2 localPosition;
        public List<Road> roads;

#if UNITY_EDITOR
        [Button]
        private void AddNewNodeAsChild()
        {
            Map map = FindObjectOfType<Map>();
            UnityEditor.Undo.RecordObject(map.gameObject, "New road added");
            Vector2 nodePosition = this.localPosition + Vector2.up;
            Node node = new Node
            {
                localPosition = nodePosition,
                roads = new List<Road>() { },
            };

            Road road = new Road()
            {
                nextNodeIndex = map.Nodes.Count,
                color = new ColorRef()
                {
                    value = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1),
                }
            };
            this.roads.Add(road);
            map.Nodes.Add(node);
            map.FixCurves();
            UnityEditor.Undo.RegisterCreatedObjectUndo(road.roadCurve, "Curve added");
        }
#endif
    }

    [SerializeField]
    private List<Node> nodes = null;

    public Vector2 mapOffset;

    public List<Node> Nodes => nodes;


#if UNITY_EDITOR

    public void FixCurves()
    {
        if (Application.isPlaying == true)
        {
            return;
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            Node beg = nodes[i];
            if (beg.roads == null || beg.roads.Count == 0)
            {
                continue;
            }
            for (int j = 0; j < beg.roads.Count; j++)
            {
                Road road = beg.roads[j];
                Node end = nodes[road.nextNodeIndex];
                if (road.roadCurve == null || road.roadCurve.Count < 2)
                {
                    if (road.roadCurve != null)
                    {
                        DestroyImmediate(road.roadCurve.gameObject);
                    }
                    string curveName = $"Curve {i}:{road.nextNodeIndex}";
                    foreach (Transform child in this.transform)
                    {
                        if (child.name == curveName)
                        {
                            UnityEditor.Undo.RecordObject(gameObject, "Setting bezier curve");
                            road.roadCurve = child.GetComponent<BezierSolution.BezierSpline>();
                            break;
                        }
                    }
                    if (road.roadCurve == null)
                    {
                        road.roadCurve = new GameObject(curveName).AddComponent<BezierSolution.BezierSpline>();
                        road.roadCurve.gizmoColor = road.color;
                        road.roadCurve.transform.SetParent(this.transform, false);
                        road.roadCurve[0].localPosition = (Vector3)beg.localPosition;
                        road.roadCurve[1].localPosition = (Vector3)end.localPosition;
                        road.roadCurve[0].followingControlPointLocalPosition = (road.roadCurve[1].localPosition - road.roadCurve[0].localPosition) * 0.3f;
                        road.roadCurve[1].followingControlPointLocalPosition = (road.roadCurve[1].localPosition - road.roadCurve[0].localPosition) * 0.3f;
                        UnityEditor.Undo.RegisterCreatedObjectUndo(road.roadCurve, $"Undo adding bezier curve {road}");
                    }
                }
                if (j < 0 || j >= nodes.Count)
                {
                    Debug.LogError($"Node [{i}] pointing to a index out of range (index: {road}, nodes.Length: {nodes.Count})");
                    continue;
                }
                road.roadCurve.name = $"Curve {i}:{road.nextNodeIndex}";
                road.roadCurve[0].localPosition = (Vector3)beg.localPosition;
                road.roadCurve[1].localPosition = (Vector3)end.localPosition;
                road.roadCurve.transform.localPosition = Vector3.zero;
            }
        }
    }
#endif


}
