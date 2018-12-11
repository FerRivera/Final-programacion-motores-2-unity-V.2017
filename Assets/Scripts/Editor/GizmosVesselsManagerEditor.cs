using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GizmosVesselsManager), true)]
public class GizmosVesselsManagerEditor : Editor
{
    PathConfig _pathsSaved;
    static VesselsSaved _vesselsSaved;

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(Vessel item, GizmoType gizmoType)
    {
        if (_vesselsSaved != null && !_vesselsSaved.showVesselsLimits)
            return;

        Handles.RadiusHandle(item.transform.rotation, item.transform.position, item.distanceBetweenVessels);
    }

    private void OnEnable()
    {
        _pathsSaved = (PathConfig)Resources.Load("PathConfig");

        if (_pathsSaved == null)
        {
            ScriptableObjectsCreator.CreatePathConfig();
            _pathsSaved = (PathConfig)Resources.Load("PathConfig");
        }

        _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");

        if (_vesselsSaved == null)
        {
            ScriptableObjectsCreator.CreateVesselsConfig();
            _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");            
        }

        EditorUtility.SetDirty(_vesselsSaved);
    }

    private void OnSceneGUI()
    {
        DrawHandlde();
    }

    public void DrawHandlde()
    {
        if (!_vesselsSaved.showVesselsLimits)
            return;

        foreach (var item in _pathsSaved.vessels)
        {
            if(item != null)
                Handles.RadiusHandle(item.transform.rotation, item.transform.position, item.GetComponent<Vessel>().distanceBetweenVessels);
        }
    }

    public override void OnInspectorGUI()
    {
        _vesselsSaved.showVesselsLimits = EditorGUILayout.Toggle("Show vessels limits ", _vesselsSaved.showVesselsLimits);
    }
}
