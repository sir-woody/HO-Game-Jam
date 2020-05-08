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

        Vector2 newPosition = map.transform.InverseTransformPoint(UnityEditor.Handles.PositionHandle((Vector3) map.transform.TransformPoint(currentNode.localPosition) + map.MapOffset, Quaternion.identity) - map.MapOffset);
        if (currentNode.roads != null)
        {
            for (int i = 0; i < currentNode.roads.Count; i++)
            {
                BezierCurve roadCurve = currentNode.roads[i].roadCurve;

                if (roadCurve != null)
                {
                    if (roadCurve.color.value.a == 0)
                    {
                        roadCurve.color.value = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
                    }
                    Handles.DrawBezier(
                        map.transform.TransformPoint(roadCurve.points[0]) + map.MapOffset,
                        map.transform.TransformPoint(roadCurve.points[3]) + map.MapOffset,
                        map.transform.TransformPoint(roadCurve.points[1]) + map.MapOffset,
                        map.transform.TransformPoint(roadCurve.points[2]) + map.MapOffset, roadCurve.color.value, null, 2f);
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
