using System.Collections;
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

        EditorUtility.SetDirty(_vesselsSaved);
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
        if (Event.current != null && Event.current.isKey && Event.current.type.Equals(EventType.KeyDown) && Event.current.keyCode == KeyCode.Delete)
        {
            _target.Delete(_pathsSaved);
        }

        Handles.BeginGUI();
        
        _target.distanceBetweenVessels = EditorGUILayout.FloatField("Distance between vessels: ", _target.distanceBetweenVessels, GUILayout.Width(300));

        if (_target.distanceBetweenVessels < 0.1)
            _target.distanceBetweenVessels = 0.1f;

        ChangeDistance();

        DeleteActualVessel();

        DeleteVesselsInsideLimit();

        Handles.EndGUI();
    }

    public void DeleteVesselsInsideLimit()
    {
        if (GUI.Button(new Rect(20, 90, 130, 30), "Delete closer vessels"))
        {
            if(EditorUtility.DisplayDialog("Delete close vessels?", "Are you sure you want to delete all vessels inside the limits?", "Yes", "No"))
            {
                var temp = Physics.OverlapSphere(_target.transform.position, _target.distanceBetweenVessels, _vesselsSaved.vessels).ToList();

                temp.Remove(_target.GetComponent<Collider>());

                foreach (var item in temp)
                {
                    item.gameObject.GetComponent<Vessel>().Delete(_pathsSaved);
                }
            }
        }
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

    public void DeleteActualVessel()
    {
        if (GUI.Button(new Rect(20, 50, 130, 30), "Delete vessel"))
        {
            _target.Delete(_pathsSaved);
        }
    }

    private void FixValues()
    {

    }
        
}
