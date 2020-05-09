using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapEditor : OdinEditor
{
    private Map map;

    protected override void OnEnable()
    {
        map = target as Map;
        map.FixCurves();
    }
    private void OnSceneGUI()
    {
        if (map != null && map.Nodes != null)
        {
            for (int i = 0; i < map.Nodes.Count; i++)
            {
                DrawNextSegment(i);
            }
        }
    }
    private void DrawNextSegment(int currentNodeIndex)
    {

        Map.Node currentNode = map.Nodes[currentNodeIndex];

        Vector2 newPosition = map.transform.InverseTransformPoint((Vector2)UnityEditor.Handles.PositionHandle((Vector2)map.transform.TransformPoint(currentNode.localPosition) + map.mapOffset, Quaternion.identity) - map.mapOffset);
        if (currentNode.roads != null)
        {
            for (int i = 0; i < currentNode.roads.Count; i++)
            {
                BezierSolution.BezierSpline roadCurve = currentNode.roads[i].roadCurve;

                if (roadCurve != null)
                {
                    if (roadCurve.gizmoColor.value.a == 0)
                    {
                        roadCurve.gizmoColor.value = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
                    }
                    Handles.DrawBezier(
                        (Vector2)roadCurve[0].position + map.mapOffset,
                        (Vector2)roadCurve[1].position + map.mapOffset,
                        (Vector2)roadCurve[0].followingControlPointPosition + map.mapOffset,
                        (Vector2)roadCurve[1].precedingControlPointPosition + map.mapOffset, roadCurve.gizmoColor.value, null, 2f);
                }
            }
        }

        if (newPosition != currentNode.localPosition)
        {
            UnityEditor.Undo.RecordObject(map, "Node position changed");
            currentNode.localPosition = newPosition;
            map.FixCurves();
        }

    }

}
