using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VesselsInstantiator))]
[ExecuteInEditMode]
public class VesselsInstantiatorEditor : Editor
{
    public static bool editMode = false;

    List<string> layers;
    string[] layerNames;
    List<Object> _objects = new List<Object>();
    VesselsSaved _vesselsSaved;

    PathConfig _pathsSaved;

    static bool preInstantiateVessel;
    static int preInstantiateVesselIndex;
    static GameObject preInstantiateVesselGo = null;

    bool _isClose = false;

    //GUIStyle _style = new GUIStyle();
    GUIStyle editModeStyle = new GUIStyle();
    GUIStyle insideVesselLimitStyle = new GUIStyle();

    float _currentTime = 0;
    float _totalTime = 2;

    void OnEnable()
    {
        //_target = (VesselsInstantiator)target;
        _objects = Resources.LoadAll("Vessels", typeof(GameObject)).ToList();
        _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");

        if(_vesselsSaved == null)
        {
            ScriptableObjectsCreator.CreateVesselsConfig();
            _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");            
        }

        EditorUtility.SetDirty(_vesselsSaved);

        _pathsSaved = (PathConfig)Resources.Load("PathConfig");

        if (_pathsSaved == null)
        {
            ScriptableObjectsCreator.CreatePathConfig();
            _pathsSaved = (PathConfig)Resources.Load("PathConfig");
        }

        editModeStyle.fontSize = 20;
        insideVesselLimitStyle.fontSize = 20;
    }

    private void OnDisable()
    {
        editMode = false;
        preInstantiateVessel = false;
        preInstantiateVesselIndex = 0;
        DestroyImmediate(preInstantiateVesselGo);
    }

    public override void OnInspectorGUI()
    {
        ShowValues();

        FixValues();

        Repaint();
    }

    private void ShowValues()
    {
        if (_vesselsSaved.selectedIndex > _pathsSaved.vesselsToInstantiate.Count())
            _vesselsSaved.selectedIndex = 0;

        _vesselsSaved.selectedIndex = EditorGUILayout.Popup("Vessel to create", _vesselsSaved.selectedIndex, _objects.Select(x => x.name).ToArray());

        ShowPreview();

        _vesselsSaved.showHelpBox = EditorGUILayout.Toggle("Show Helpbox", _vesselsSaved.showHelpBox);

        _vesselsSaved.gizmoInstantiatedVessel = EditorGUILayout.ColorField("Color vessel limits", _vesselsSaved.gizmoInstantiatedVessel);

        if (_vesselsSaved.gizmoInstantiatedVessel.a < 1)
            _vesselsSaved.gizmoInstantiatedVessel.a = 1;

        _vesselsSaved.gizmoPredictionVessel = EditorGUILayout.ColorField("Color prediction vessel limits", _vesselsSaved.gizmoPredictionVessel);

        if (_vesselsSaved.gizmoPredictionVessel.a < 1)
            _vesselsSaved.gizmoPredictionVessel.a = 1;

        _vesselsSaved.editModeEnabled = EditorGUILayout.ColorField("Edit mode warning color", _vesselsSaved.editModeEnabled);

        if (_vesselsSaved.editModeEnabled.a < 1)
            _vesselsSaved.editModeEnabled.a = 1;

        _vesselsSaved.insideVesselLimit = EditorGUILayout.ColorField("Vessel limits warning color", _vesselsSaved.insideVesselLimit);

        if (_vesselsSaved.insideVesselLimit.a < 1)
            _vesselsSaved.insideVesselLimit.a = 1;

        _vesselsSaved.distance = EditorGUILayout.FloatField("Distance between vessels", _vesselsSaved.distance);

        if (_vesselsSaved.distance < 0)
            _vesselsSaved.distance = 0;        

        if (_vesselsSaved.showHelpBox)
            EditorGUILayout.HelpBox("This value is the minimium distance that can be between vessels", MessageType.Info);

        _vesselsSaved.vessels = LayerMaskField("Vessels layer", _vesselsSaved.vessels.value);

        if (_vesselsSaved.showHelpBox)
            EditorGUILayout.HelpBox("Layer used to calculate vessels distance", MessageType.Info);

        _vesselsSaved.map = LayerMaskField("Map layer", _vesselsSaved.map.value);

        if (_vesselsSaved.showHelpBox)
            EditorGUILayout.HelpBox("Layer used to place vessels", MessageType.Info);

        if (editMode)
        {
            if (GUILayout.Button("Disable Editing"))
            {
                editMode = false;
                preInstantiateVessel = false;
            }
        }
        else
        {
            if (GUILayout.Button("Enable Editing"))
            {
                editMode = true;
            }
        }
    }

    public static void ExitGUI()
    {
        GUIUtility.ExitGUI();
        throw new ExitGUIException();
    }

    private void FixValues()
    {

    }

    private void OnSceneGUI()
    {       

        PreInstantiateVessel();

        if(preInstantiateVesselGo != null)
        {
            Handles.color = _vesselsSaved.gizmoPredictionVessel;

            Handles.RadiusHandle(preInstantiateVesselGo.transform.rotation, preInstantiateVesselGo.transform.position, _vesselsSaved.distance);
        }            

        if (editMode)
        {
            Handles.BeginGUI();
            editModeStyle.fontSize = 20;
            if(_vesselsSaved != null)
                editModeStyle.normal.textColor = _vesselsSaved.editModeEnabled;
            Vector3 pos = preInstantiateVesselGo.transform.position;
            Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos).normalized;
            GUI.Label(new Rect(Camera.current.pixelWidth / 2 - 100, pos2D.y, 100, 100), "Edit mode activated!", editModeStyle);
            Handles.EndGUI();

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
        
        if (_isClose && preInstantiateVesselGo != null && _currentTime <= _totalTime)
        {
            _currentTime += Time.deltaTime;
            insideVesselLimitStyle.fontSize = 20;

            Handles.BeginGUI();
            insideVesselLimitStyle.fontSize = 20;
            if (_vesselsSaved != null)
                insideVesselLimitStyle.normal.textColor = _vesselsSaved.insideVesselLimit;
            Vector3 pos = preInstantiateVesselGo.transform.position;
            Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos).normalized;
            GUI.Label(new Rect(Camera.current.pixelWidth/2-100, pos2D.y+40, 100, 100), "Inside vessel limit!", insideVesselLimitStyle);
            Handles.EndGUI();
        }
        else if(_currentTime > _totalTime)
        {
            _currentTime = 0;
            _isClose = false;
            insideVesselLimitStyle.fontSize = 0;
        }
            
    }

    public void PreInstantiateVessel()
    {
        if (!editMode)
            DestroyImmediate(preInstantiateVesselGo);

        if (preInstantiateVessel && editMode && _vesselsSaved.selectedIndex != preInstantiateVesselIndex)
        {
            preInstantiateVessel = false;
            preInstantiateVesselIndex = 0;
            DestroyImmediate(preInstantiateVesselGo);
        }            

        if (editMode && !preInstantiateVessel && preInstantiateVesselGo == null)
        {
            preInstantiateVessel = true;
            preInstantiateVesselGo = (GameObject)Instantiate(_objects[_vesselsSaved.selectedIndex]);            
            preInstantiateVesselIndex = _vesselsSaved.selectedIndex;
            DestroyImmediate(preInstantiateVesselGo.GetComponent<Collider>());
        }        

        if (preInstantiateVesselGo != null)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(worldRay, out hitInfo, float.MaxValue, _vesselsSaved.map))
            {
                float y = preInstantiateVesselGo.GetComponent<Renderer>().bounds.size.y / 2;
                var dir = hitInfo.point;

                Vector3 pos = new Vector3(dir.x, dir.y + y, dir.z);
                preInstantiateVesselGo.transform.position = pos;
            }                
        }
    }

    void CreateVessel(Ray MouseRay, RaycastHit MousePosHit)
    {
        Vector3 pos = Vector3.zero;        

        if (Physics.Raycast(MouseRay, out MousePosHit, float.MaxValue, _vesselsSaved.map))
        {
            var dir = MousePosHit.point;

            GameObject vessel = (GameObject)Instantiate(_objects[_vesselsSaved.selectedIndex]);

            if (vessel.GetComponent<Vessel>() == null)
            {
                vessel.AddComponent<Vessel>().lastIndex = _vesselsSaved.selectedIndex;
                var temp = vessel.GetComponent<Vessel>();
                temp.currentIndex = _vesselsSaved.selectedIndex;
                temp.id = _pathsSaved.vessels.Count;
                temp.distanceBetweenVessels = _vesselsSaved.distance;

                if (_pathsSaved.maxVesselDistance < _vesselsSaved.distance)
                    _pathsSaved.maxVesselDistance = _vesselsSaved.distance;                
            }            

            float y = vessel.GetComponent<Renderer>().bounds.size.y / 2;

            pos = new Vector3(dir.x, dir.y + y, dir.z);
            vessel.transform.position = pos;            

            _isClose = CheckCloserVessels(vessel.transform.position,vessel);                           

            if (vessel != null)
            {
                _pathsSaved.vessels.Add(vessel);
                _pathsSaved.vesselsPositions.Add(vessel.transform.position);
                _pathsSaved.vesselsType.Add(_vesselsSaved.selectedIndex);
                _pathsSaved.vesselsDistance.Add(_vesselsSaved.distance);
            }
        }
    }

    bool CheckCloserVessels(Vector3 position,GameObject go)
    {
        var temp = Physics.OverlapSphere(position, _pathsSaved.maxVesselDistance, _vesselsSaved.vessels).ToList();

        temp.Remove(go.GetComponent<Collider>());

        foreach (var item in temp)
        {
            float distance = item.gameObject.GetComponent<Vessel>().distanceBetweenVessels;
            var vessels = Physics.OverlapSphere(item.transform.position, distance, _vesselsSaved.vessels).ToList();           

            if (go != null && vessels.Contains(go.GetComponent<Collider>()))
            {
                DestroyImmediate(go);
                return true;
            }
        }

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

    void ShowPreview()
    {
        var _preview = AssetPreview.GetAssetPreview(_pathsSaved.vesselsToInstantiate[_vesselsSaved.selectedIndex]);

        if (_preview != null)
        {
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(_pathsSaved.vesselsToInstantiate[_vesselsSaved.selectedIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(_pathsSaved.vesselsToInstantiate[_vesselsSaved.selectedIndex]));
            GUILayout.EndHorizontal();
        }
    }
}
