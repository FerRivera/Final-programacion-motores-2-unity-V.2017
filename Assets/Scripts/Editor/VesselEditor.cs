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

        _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");

        if (_vesselsSaved == null)
        {
            ScriptableObjectsCreator.CreateVesselsConfig();
            _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");
        }
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

        _target.currentIndex = EditorGUILayout.Popup("Actual type", _target.currentIndex, _pathsSaved.vesselsToInstantiate.Select(x => x.name).ToArray());

        ShowPreview();        

        //_target.id = EditorGUILayout.IntField("ID", _target.id);

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
            vessel.GetComponent<Vessel>().distanceBetweenVessels = _target.distanceBetweenVessels;

            var temp = _pathsSaved.vessels[_target.id];

            _pathsSaved.vessels[_target.id] = vessel;
            _pathsSaved.vesselsType[_target.id] = vessel.GetComponent<Vessel>().currentIndex;
            _pathsSaved.vesselsPositions[_target.id] = vessel.transform.position;
            _pathsSaved.vesselsDistance[_target.id] = vessel.GetComponent<Vessel>().distanceBetweenVessels;

            DestroyImmediate(temp);

            Selection.activeObject = vessel;
        }
    }

    void OnSceneGUI()
    {
        Handles.BeginGUI();

        _target.distanceBetweenVessels = EditorGUILayout.FloatField("Distance between vessels: ", _target.distanceBetweenVessels, GUILayout.Width(300));

        ChangeDistance();

        DeleteActualVessel();

        Handles.EndGUI();
    }

    public void ChangeDistance()
    {
        if (_target != null && _pathsSaved != null && _pathsSaved.maxVesselDistance < _target.distanceBetweenVessels)
        {
            _pathsSaved.maxVesselDistance = _target.distanceBetweenVessels;            
        }
         
        if(_target != null && _pathsSaved && _pathsSaved.vesselsDistance[_target.id] != _target.distanceBetweenVessels)
            _pathsSaved.vesselsDistance[_target.id] = _target.distanceBetweenVessels;
    }

    void DeleteActualVessel()
    {
        if (GUI.Button(new Rect(20, 50, 130, 30), "Delete vessel"))
        {
            var temp = _pathsSaved.vessels[_target.id];
            var tempID = temp.GetComponent<Vessel>().id;

            _pathsSaved.vessels.RemoveAt(tempID);
            _pathsSaved.vesselsType.RemoveAt(tempID);
            _pathsSaved.vesselsPositions.RemoveAt(tempID);
            _pathsSaved.vesselsDistance.RemoveAt(tempID);

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
