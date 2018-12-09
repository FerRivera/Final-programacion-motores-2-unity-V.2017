﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Vessel))]
public class VesselEditor : Editor
{
    PathConfig _pathsSaved;
    Vessel _target;
    VesselsSaved _vesselsSaved;

    void OnEnable()
    {
        _target = (Vessel)target;
        //_objects = Resources.LoadAll("Vessels", typeof(GameObject)).ToList();
        _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");
        //SceneView.onSceneGUIDelegate += OnScene;
        //OnScene(SceneView.currentDrawingSceneView);
    }

    public override void OnInspectorGUI()
    {
        ShowValues();

        FixValues();

        Repaint();
    }

    private void ShowValues()
    {
        _pathsSaved = (PathConfig)Resources.Load("PathConfig");

        //_seed = GameObject.FindGameObjectWithTag("Seed").GetComponent<Seed>();

        _target.distanceBetweenVessels = _vesselsSaved.distance;

        _target.currentIndex = EditorGUILayout.Popup("Actual type", _target.currentIndex, _pathsSaved.vesselsToInstantiate.Select(x => x.name).ToArray());

        ShowPreview();
        _target.id = EditorGUILayout.IntField("ID", _target.id);

        SwitchType();
    }

    void ShowPreview()
    {
        var _preview = AssetPreview.GetAssetPreview(_pathsSaved.vesselsToInstantiate[_target.currentIndex]);

        if (_preview != null)
        {
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(_pathsSaved.vesselsToInstantiate[_target.currentIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(_pathsSaved.vesselsToInstantiate[_target.currentIndex]));
            GUILayout.EndHorizontal();
        }
    }

    void SwitchType()
    {
        if (_target.lastIndex != _target.currentIndex)
        {
            GameObject vessel = (GameObject)Instantiate(_pathsSaved.vesselsToInstantiate[_target.currentIndex]);
            vessel.transform.position = _pathsSaved.vessels[_target.id].transform.position;

            vessel.AddComponent<Vessel>().currentIndex = _target.currentIndex;
            vessel.GetComponent<Vessel>().lastIndex = _target.currentIndex;
            vessel.GetComponent<Vessel>().id = _target.id;

            var temp = _pathsSaved.vessels[_target.id];

            _pathsSaved.vessels[_target.id] = vessel;
            _pathsSaved.vesselsType[_target.id] = vessel.GetComponent<Vessel>().currentIndex;
            _pathsSaved.vesselsPositions[_target.id] = vessel.transform.position;

            DestroyImmediate(temp);

            //_pathsSaved.vessels.Insert(_target.id, vessel);
            //_pathsSaved.vesselsType.Insert(_target.id, vessel.GetComponent<Vessel>().currentIndex);
            //_pathsSaved.vesselsPositions.Insert(_target.id, vessel.transform.position);

            Selection.activeObject = vessel;
        }
    }

    void OnSceneGUI()
    {
        //DrawHandles(_target,GizmoType.NotInSelectionHierarchy);

        Handles.BeginGUI();        

        DeleteActualVessel();

        Handles.EndGUI();
    }   

    void DeleteActualVessel()
    {
        if (GUI.Button(new Rect(20, 20, 130, 30), "Delete vessel"))
        {
            var temp = _pathsSaved.vessels[_target.id];
            var tempID = temp.GetComponent<Vessel>().id;

            _pathsSaved.vessels.RemoveAt(tempID);
            _pathsSaved.vesselsType.RemoveAt(tempID);
            _pathsSaved.vesselsPositions.RemoveAt(tempID);

            //pathsSaved.paths.Remove(temp);
            //pathsSaved.objectType.Remove(pathsSaved.objectType[tempID]);
            //pathsSaved.positions.Remove(pathsSaved.positions[tempID]);
            //pathsSaved.rotations.Remove(pathsSaved.rotations[tempID]);

            for (int i = _pathsSaved.vessels.Count - 1; i >= _target.id; i--)
            {
                _pathsSaved.vessels[i].GetComponent<Vessel>().id--;
            }

            DestroyImmediate(temp);
        }
    }

    private void FixValues()
    {

    }
        
}
