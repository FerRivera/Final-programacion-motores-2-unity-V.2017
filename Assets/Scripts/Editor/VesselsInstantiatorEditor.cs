using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VesselsInstantiator))]
public class VesselsInstantiatorEditor : Editor
{
    //private VesselsInstantiator _target;

    static bool _editMode = false;

    List<string> layers;
    string[] layerNames;
    List<Object> _objects = new List<Object>();
    VesselsSaved _vesselsSaved;

    PathConfig _pathsSaved;

    void OnEnable()
    {
        //_target = (VesselsInstantiator)target;
        _objects = Resources.LoadAll("Vessels", typeof(GameObject)).ToList();
        _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");
        _pathsSaved = (PathConfig)Resources.Load("PathConfig");
    }

    public override void OnInspectorGUI()
    {
        //Primero mostramos los valores
        ShowValues();

        //Luego arreglamos los valores que tengamos que arreglar
        FixValues();

        //DrawDefaultInspector(); //Dibuja el inspector como lo hariamos normalmente. Sirve por si no queremos rehacher todo el inspector y solamente queremos agregar un par de funcionalidades.

        Repaint(); //Redibuja el inspector
    }

    private void ShowValues()
    {
        _vesselsSaved.selectedIndex = EditorGUILayout.Popup("Vessel to create", _vesselsSaved.selectedIndex, _objects.Select(x => x.name).ToArray());

        _vesselsSaved.distance = EditorGUILayout.FloatField("Distance between vessels", _vesselsSaved.distance);

        _vesselsSaved.vessels = LayerMaskField("Vessels layer", _vesselsSaved.vessels.value);

        _vesselsSaved.map = LayerMaskField("Map layer", _vesselsSaved.map.value);

        if (_editMode)
        {
            if (GUILayout.Button("Disable Editing"))
            {
                _editMode = false;
            }
        }
        else
        {
            if (GUILayout.Button("Enable Editing"))
            {
                _editMode = true;
            }
        }
    }

    private void FixValues()
    {

    }

    private void OnSceneGUI()
    {
        if (_editMode)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(worldRay, out hitInfo))
                {
                    CreateVessel(worldRay, hitInfo);
                }

                Event.current.Use();
            }            
        }
    }

    void CreateVessel(Ray MouseRay, RaycastHit MousePosHit)
    {
        if (Physics.Raycast(MouseRay, out MousePosHit, float.MaxValue, _vesselsSaved.map))
        {
            //var dir = MousePosHit.point + (Camera.main.transform.position - MousePosHit.point).normalized;
            var dir = MousePosHit.point;

            if (!CloserVessels(dir, _vesselsSaved.distance))
            {
                GameObject vessel = (GameObject)Instantiate(_objects[_vesselsSaved.selectedIndex]);

                if (vessel.GetComponent<Vessel>() == null)
                {
                    vessel.AddComponent<Vessel>().lastIndex = _vesselsSaved.selectedIndex;
                    var temp = vessel.GetComponent<Vessel>();
                    temp.currentIndex = _vesselsSaved.selectedIndex;
                    temp.id = _pathsSaved.vessels.Count;
                }

                float y = vessel.GetComponent<Renderer>().bounds.size.y/2;

                Vector3 pos = new Vector3(dir.x, dir.y+y , dir.z);
                vessel.transform.position = pos;

                _pathsSaved.vessels.Add(vessel);
                _pathsSaved.vesselsPositions.Add(vessel.transform.position);
                _pathsSaved.vesselsType.Add(_vesselsSaved.selectedIndex);
            }
        }
    }

    bool CloserVessels(Vector3 position, float radius)
    {
        var temp = Physics.OverlapSphere(position, radius, _vesselsSaved.vessels);

        if (temp.Count() > 0)
            return true;

        return false;
    }

    public LayerMask LayerMaskField(string label, LayerMask selected)
    {

        if (layers == null)
        {
            layers = new List<string>();
            layerNames = new string[4];
        }
        else
        {
            layers.Clear();
        }

        int emptyLayers = 0;
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {

                for (; emptyLayers > 0; emptyLayers--) layers.Add("Layer " + (i - emptyLayers));
                layers.Add(layerName);
            }
            else
            {
                emptyLayers++;
            }
        }

        if (layerNames.Length != layers.Count)
        {
            layerNames = new string[layers.Count];
        }
        for (int i = 0; i < layerNames.Length; i++) layerNames[i] = layers[i];

        selected.value = EditorGUILayout.MaskField(label, selected.value, layerNames);

        return selected;
    }
}
