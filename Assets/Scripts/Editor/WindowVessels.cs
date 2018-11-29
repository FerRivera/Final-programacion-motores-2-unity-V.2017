using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[ExecuteInEditMode]
public class WindowVessels : EditorWindow
{
    List<Object> _objects = new List<Object>();
    VesselsSaved _vesselsSaved;

    List<string> layers;
    string[] layerNames;

    [MenuItem("Level options/Create Vessels")]
    static void CreateWindow()
    {
        var window = ((WindowVessels)GetWindow(typeof(WindowVessels)));
        window.Show();
        window.Init();
    }

    public void Init()
    {
        _objects = Resources.LoadAll("Vessels", typeof(GameObject)).ToList();
        _vesselsSaved = (VesselsSaved)Resources.Load("VesselsConfig");
    }

    void OnGUI()
    {
        _vesselsSaved.selectedIndex = EditorGUILayout.Popup("Vessel to create", _vesselsSaved.selectedIndex, _objects.Select(x => x.name).ToArray());

        _vesselsSaved.distance = EditorGUILayout.FloatField("Distance between vessels", _vesselsSaved.distance);

        _vesselsSaved.vessels = LayerMaskField("Vessels layer", _vesselsSaved.vessels.value);

        _vesselsSaved.map = LayerMaskField("Map layer", _vesselsSaved.map.value);

        var _preview = AssetPreview.GetAssetPreview(_objects[_vesselsSaved.selectedIndex]);

        if (_preview != null)
        {
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(_objects[_vesselsSaved.selectedIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(_objects[_vesselsSaved.selectedIndex]));
            GUILayout.EndHorizontal();
        }
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

    void CreateVessel()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        RaycastHit MousePosHit;
        Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(MouseRay, out MousePosHit, float.MaxValue, _vesselsSaved.map))
        {            
            var dir = MousePosHit.point + (Camera.main.transform.position - MousePosHit.point).normalized;

            if (!CloserVessels(dir, _vesselsSaved.distance))
            {
                GameObject path = (GameObject)Instantiate(_objects[_vesselsSaved.selectedIndex]);

                if (path.GetComponent<Vessel>() == null)
                    path.AddComponent<Vessel>();

                Vector3 pos = new Vector3(dir.x, path.GetComponent<Renderer>().bounds.size.y, dir.z);
                path.transform.position = pos;
            }            
        }
    }

    bool CloserVessels(Vector3 position,float radius)
    {
        var temp = Physics.OverlapSphere(position, radius, _vesselsSaved.vessels);

        if (temp.Count() > 0)
            return true;

        return false;
    }  

    private void Update()
    {
        //CreateVessel();
    }
}