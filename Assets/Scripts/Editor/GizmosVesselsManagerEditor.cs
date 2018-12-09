using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GizmosVesselsManager), true)]
public class GizmosVesselsManagerEditor : Editor
{
    static PathConfig _pathsSaved;

    //static GizmosVesselsManager _target;

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    public static void DrawHandles(Vessel item, GizmoType gizmoType)
    {
        if (_pathsSaved != null)
        {
            //foreach (var item in pathsSaved.vessels)
            //{
            Handles.RadiusHandle(item.transform.rotation, item.transform.position, item.distanceBetweenVessels);
            //Handles.CubeHandleCap(0,item.transform.position, item.transform.rotation, item.distanceBetweenVessels,EventType);
            //HandleUtility.Repaint();
            //SceneView.RepaintAll();
            //}
        }
    }

    private void OnEnable()
    {
        //_target = (GizmosVesselsManager)target;

        _pathsSaved = (PathConfig)Resources.Load("PathConfig");
    }

    private void OnSceneGUI()
    {

    }

    //private void DrawHandles()
    //{
    //    if (_pathsSaved == null)
    //        return;

    //    foreach (var item in _pathsSaved.vessels)
    //    {
    //        Handles.RadiusHandle(item.transform.rotation,item.transform.position, item.GetComponent<Vessel>().distanceBetweenVessels);
    //    }
    //}

    //[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    //private static void OnScene(SceneView sceneview)
    //{
    //    if (_pathsSaved == null)
    //        return;

    //    foreach (var item in _pathsSaved.vessels)
    //    {
    //        Handles.RadiusHandle(item.transform.rotation, item.transform.position, item.GetComponent<Vessel>().distanceBetweenVessels);
    //    }        

    //    //HandleUtility.Repaint();

    //    //sceneview.Repaint();
    //}

    //[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    //static void DrawHandles(GizmoType gizmoType)
    //{
    //    if (_pathsSaved == null)
    //        return;

    //    foreach (var item in _pathsSaved.vessels)
    //    {
    //        Handles.RadiusHandle(item.transform.rotation, item.transform.position, item.GetComponent<Vessel>().distanceBetweenVessels);
    //    }

    //    //GUIStyle style = new GUIStyle(); // This is optional
    //    //style.normal.textColor = Color.yellow;
    //    //Handles.RadiusHandle(spawnPoint.transform.position, spawnPoint.name + "(ItemSpawnPoint)"); // you can remove the "style" if you don't want it
    //}
}
