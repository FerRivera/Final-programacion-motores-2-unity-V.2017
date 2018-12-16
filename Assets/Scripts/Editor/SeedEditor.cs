using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Linq;
using System;

[CustomEditor(typeof(Seed))]
public class SeedEditor : Editor
{
    private Seed _target;

    public float buttonWidth = 130;
    public float buttonHeight = 30;

    //int _buttonMinSize = 45;
    //int _buttonMaxSize = 70;
    
    //int _newMapYPos = 20;
    //int _saveNewMapYPos = 60;
    int _deleteLastPathYPos = 140;
    int _deleteLastVesselYPos = 180;
    int _overwriteMapYPos = 100;

     //20, _saveNewMapYPos, buttonWidth, buttonHeight

    public PathConfig pathsSaved;
    SceneButtonsConfig _sceneButtonsConfig;

    public bool newMap = false;
    public bool saveMap = false;

    void OnEnable()
    {
        _target = (Seed)target;

        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        if (pathsSaved == null)
        {
            ScriptableObjectsCreator.CreatePathConfig();
            pathsSaved = (PathConfig)Resources.Load("PathConfig");
        }

        _sceneButtonsConfig = (SceneButtonsConfig)Resources.Load("SceneButtonsConfig");

        if (_sceneButtonsConfig == null)
        {
            ScriptableObjectsCreator.CreateSceneButtonsConfig();
            _sceneButtonsConfig = (SceneButtonsConfig)Resources.Load("SceneButtonsConfig");
        }

        EditorUtility.SetDirty(_sceneButtonsConfig);

        ConfigurateButtonsByDefault();
        //sceneButtonsConfig.saveNewMapRect = new Rect(20, 60, buttonWidth, buttonHeight);
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
        ConfigurateObjects();

        _target.selectedIndex = EditorGUILayout.Popup("Path to create", _target.selectedIndex, pathsSaved.objectsToInstantiate.Select(x => x.name).ToArray());

        ShowPreview();        
    }

    private void FixValues()
    {
        
    }

    void OnSceneGUI()
    {
        Handles.BeginGUI();
        
        var addValue = 30 / Vector3.Distance(Camera.current.transform.position, _target.transform.position);

        if (!VesselsInstantiatorEditor.editMode)
        {
            OpenSaveMapWindow();

            NewMap();

            DeleteLastPath();
            DeleteLastVessel();

            DrawButton("+", _target.transform.position + Camera.current.transform.up * addValue);
            DrawButton("+", _target.transform.position - Camera.current.transform.up * addValue);
            DrawButton("+", _target.transform.position + Camera.current.transform.right * addValue);
            DrawButton("+", _target.transform.position - Camera.current.transform.right * addValue);

            SaveMap();
        }       

        Handles.EndGUI();
    }

    public void OpenSaveMapWindow()
    {
        if (pathsSaved.paths.Count <= 0)
            return;

        if (GUI.Button(_sceneButtonsConfig.saveNewMapRect, "Save new Map"))
        {
            WindowSaveMaps.CreateWindow();
        }
    }

    public void ConfigurateObjects()
    {
        _target.mapItems = Resources.LoadAll("MapItems", typeof(GameObject)).ToList();

        if (pathsSaved.objectsToInstantiate.Count != _target.mapItems.Count)
        {
            pathsSaved.objectsToInstantiate = _target.mapItems;
        }

        _target.vesselsToInstantiate = Resources.LoadAll("Vessels", typeof(GameObject)).ToList();

        if (pathsSaved.vesselsToInstantiate.Count != _target.vesselsToInstantiate.Count)
        {
            pathsSaved.vesselsToInstantiate = _target.vesselsToInstantiate;
        }
    }

    void ShowPreview()
    {
        var _preview = AssetPreview.GetAssetPreview(pathsSaved.objectsToInstantiate[_target.selectedIndex]);

        if (_preview != null)
        {
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(pathsSaved.objectsToInstantiate[_target.selectedIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(pathsSaved.objectsToInstantiate[_target.selectedIndex]));
            GUILayout.EndHorizontal();
        }
    }    

    void NewMap()
    {
        if (!newMap && GUI.Button(_sceneButtonsConfig.newMapRect, "New Map"))
        {                
            newMap = true;
        }

        if(newMap && EditorUtility.DisplayDialog("Start a new map?","Are you sure you want to start a new map?", "Yes", "No"))
        {
            foreach (var item in pathsSaved.paths)
            {
                DestroyImmediate(item);
            }

            foreach (var item in pathsSaved.vessels)
            {
                DestroyImmediate(item);
            }

            pathsSaved.paths.Clear();
            pathsSaved.objectType.Clear();
            pathsSaved.positions.Clear();
            pathsSaved.rotations.Clear();

            pathsSaved.vessels.Clear();
            pathsSaved.vesselsPositions.Clear();
            pathsSaved.vesselsType.Clear();
            pathsSaved.vesselsDistance.Clear();

            pathsSaved.maxVesselDistance = 0;
            pathsSaved.totalPolygons = 0;

            _target.transform.position = new Vector3(0, 0, 0);

            newMap = false;
            _target.mapLoaded = false;
        }
        else
            newMap = false;
    }

    void DeleteLastPath()
    {
        if (pathsSaved.paths.Count <= 0)
            return;

        if (GUI.Button(_sceneButtonsConfig.deleteLastPathRect, "Delete Last Path"))
        {
            if(pathsSaved.paths.Count > 0)
            {
                var lastObject = pathsSaved.paths.LastOrDefault();

                pathsSaved.objectType.RemoveAt(pathsSaved.objectType.Count-1);
                pathsSaved.positions.RemoveAt(pathsSaved.positions.Count-1);
                pathsSaved.rotations.RemoveAt(pathsSaved.rotations.Count-1);
                pathsSaved.paths.Remove(lastObject);

                if (pathsSaved.paths.LastOrDefault() != null)
                    _target.transform.position = pathsSaved.paths.LastOrDefault().transform.position;

                if (pathsSaved.paths.Count() <= 0)
                    _target.transform.position = new Vector3(0, 0, 0);

                DestroyImmediate(lastObject);
            }
            else
            {
                _target.transform.position = new Vector3(0, 0, 0);
            }
        }
    }

    void DeleteLastVessel()
    {
        if (pathsSaved.vessels.Count <= 0)
            return;

        if (GUI.Button(_sceneButtonsConfig.deleteLastVesselRect, "Delete Last Vessel"))
        {
            if (pathsSaved.vessels.Count > 0)
            {
                var lastObject = pathsSaved.vessels.LastOrDefault();

                pathsSaved.vesselsType.RemoveAt(pathsSaved.vessels.Count - 1);
                pathsSaved.vesselsPositions.RemoveAt(pathsSaved.vessels.Count - 1);
                pathsSaved.vessels.Remove(lastObject);
                pathsSaved.vesselsDistance.RemoveAt(pathsSaved.vesselsDistance.Count - 1);

                DestroyImmediate(lastObject);
            }            
        }
    }

    private void DrawButton(string text, Vector3 position)
    {
        var p = Camera.current.WorldToScreenPoint(position);

        var size = 700 / Vector3.Distance(Camera.current.transform.position, position);
        //size = Mathf.Clamp(size, _buttonMinSize, _buttonMaxSize);

        var r = new Rect(p.x - size / 2, Screen.height - p.y - size, size, size / 2);
        //screenPos.x - buttonPlusWidth / 2, Camera.current.pixelHeight - screenPos.y - 100, buttonPlusWidth, buttonHeight

        var dirTest = new Vector3(position.x, 0, position.z) - new Vector3(_target.transform.position.x, 0, _target.transform.position.z);

        dirTest = dirTest.normalized;

        var posArray = new Vector3[] { _target.transform.forward, -_target.transform.forward, _target.transform.right, -_target.transform.right };

        var disArray = posArray.OrderBy(x => Vector3.Distance(x, dirTest));

        dirTest = disArray.First();

        Direction dir;

        RaycastHit rch5;

        if (Physics.Raycast(_target.transform.position, dirTest, out rch5, 1))
        {
        }
        if (rch5.collider != null)
            return;

        if (GUI.Button(r, text))
        {
            if (dirTest.x >= 1)
                dir = Direction.Right;
            else if (dirTest.x <= -1)
                dir = Direction.Left;
            else if (dirTest.z >= 1)
                dir = Direction.Forward;
            else
                dir = Direction.Backward;

            CreatePath(_target.transform.position + dirTest, dir);
        }
    }

    void CreatePath(Vector3 dir, Direction direction)
    {
        GameObject lastObject = null;
        GameObject path = (GameObject)Instantiate(pathsSaved.objectsToInstantiate[_target.selectedIndex]);
        path.transform.position = new Vector3(0, 0, 0);

        if (path.GetComponent<Path>() == null)
        {
            path.AddComponent<Path>().lastIndex = _target.selectedIndex;
            var temp = path.GetComponent<Path>();
            temp.currentIndex = _target.selectedIndex;
            temp.id = pathsSaved.paths.Count;
        }

        if (pathsSaved.paths.Count > 0)
            lastObject = pathsSaved.paths[pathsSaved.paths.Count - 1];
        else
            lastObject = path;

        _target.transform.position = GetNextMove(lastObject, direction);

        path.transform.position = GetPathPosition(lastObject, direction);

        _target.transform.position = path.transform.position;

        pathsSaved.paths.Add(path);
        pathsSaved.objectType.Add(_target.selectedIndex);
        pathsSaved.positions.Add(path.transform.position);
        pathsSaved.rotations.Add(path.transform.rotation);
    }    

    public void SaveMap()
    {
        if (_target.currentMap == null)
            _target.mapLoaded = false;

        if (_target.mapLoaded)
        {
            if (!saveMap && GUI.Button(_sceneButtonsConfig.overwriteMapRect, "Overwrite Map"))
            {
                saveMap = true;
            }

            if (saveMap && EditorUtility.DisplayDialog("Overwrite current map?", "Are you sure you want to overwrite the current map?", "Yes", "No"))
            {
                if (_target.currentMap != null)
                {
                    _target.currentMap.paths.Clear();
                    _target.currentMap.objectType.Clear();
                    _target.currentMap.positions.Clear();
                    _target.currentMap.rotations.Clear();

                    _target.currentMap.vessels.Clear();
                    _target.currentMap.VesselsType.Clear();
                    _target.currentMap.vesselsPositions.Clear();
                    _target.currentMap.vesselsDistance.Clear();

                    _target.currentMap.paths.AddRange(pathsSaved.paths);
                    _target.currentMap.objectType.AddRange(pathsSaved.objectType);
                    _target.currentMap.positions.AddRange(pathsSaved.positions);
                    _target.currentMap.rotations.AddRange(pathsSaved.rotations);

                    _target.currentMap.vessels.AddRange(pathsSaved.vessels);
                    _target.currentMap.VesselsType.AddRange(pathsSaved.vesselsType);
                    _target.currentMap.vesselsPositions.AddRange(pathsSaved.vesselsPositions);
                    _target.currentMap.vesselsDistance.AddRange(pathsSaved.vesselsDistance);

                    CalculatePolygons();

                    _target.currentMap.totalPolygons = pathsSaved.totalPolygons;
                    //esto hace que cuando cierro unity y lo vuelvo a abrir no se pierda la info
                    EditorUtility.SetDirty(_target.currentMap);
                    saveMap = false;
                }
            }
            else
                saveMap = false;
        }
    }

    void CalculatePolygons()
    {
        if (pathsSaved == null)
            return;

        pathsSaved.totalPolygons = 0;

        foreach (var item in pathsSaved.paths)
        {
            if (item.GetComponent<MeshFilter>() != null)
            {
                pathsSaved.totalPolygons += item.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
            }
        }

        foreach (var item in pathsSaved.vessels)
        {
            if (item.GetComponent<MeshFilter>() != null)
            {
                pathsSaved.totalPolygons += item.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
            }
        }
    }

    Vector3 GetNextMove(GameObject go, Direction direction)
    {
        Vector3 DistanceToReturn = new Vector3(0, 0, 0);

        if (go == null)
            return default(Vector3);

        switch (direction)
        {
            case Direction.Forward:
                DistanceToReturn = new Vector3(go.transform.position.x, 0, go.transform.position.z + go.GetComponent<Renderer>().bounds.size.z / 2);
                return DistanceToReturn;
            case Direction.Backward:
                DistanceToReturn = new Vector3(go.transform.position.x, 0, go.transform.position.z - go.GetComponent<Renderer>().bounds.size.z / 2);
                return DistanceToReturn;
            case Direction.Left:
                DistanceToReturn = new Vector3(go.transform.position.x - go.GetComponent<Renderer>().bounds.size.x / 2, 0, go.transform.position.z);
                return DistanceToReturn;
            case Direction.Right:
                DistanceToReturn = new Vector3(go.transform.position.x + go.GetComponent<Renderer>().bounds.size.x / 2, 0, go.transform.position.z);
                return DistanceToReturn;
        }

        return default(Vector3);
    }

    Vector3 GetPathPosition(GameObject go, Direction direction)
    {
        Vector3 DistanceToReturn = new Vector3(0, 0, 0);

        if (_target == null || go == null)
            return default(Vector3);

        switch (direction)
        {
            case Direction.Forward:
                    DistanceToReturn = new Vector3(_target.transform.position.x, 0, _target.transform.position.z + go.GetComponent<Renderer>().bounds.size.z / 2);
                    return DistanceToReturn;
            case Direction.Backward:
                    DistanceToReturn = new Vector3(_target.transform.position.x, 0, _target.transform.position.z - go.GetComponent<Renderer>().bounds.size.z / 2);
                    return DistanceToReturn;
            case Direction.Left:
                    DistanceToReturn = new Vector3(_target.transform.position.x - go.GetComponent<Renderer>().bounds.size.x / 2, 0, _target.transform.position.z);
                    return DistanceToReturn;
            case Direction.Right:
                    DistanceToReturn = new Vector3(_target.transform.position.x + go.GetComponent<Renderer>().bounds.size.x / 2, 0, _target.transform.position.z);
                    return DistanceToReturn;
        }

        return default(Vector3);
    }

    public void ConfigurateButtonsByDefault()
    {
        _sceneButtonsConfig.newMapRect.width = 130;
        _sceneButtonsConfig.newMapRect.height = 30;

        _sceneButtonsConfig.saveNewMapRect.width = 130;
        _sceneButtonsConfig.saveNewMapRect.height = 30;

        _sceneButtonsConfig.overwriteMapRect.width = 130;
        _sceneButtonsConfig.overwriteMapRect.height = 30;

        _sceneButtonsConfig.deleteLastPathRect.width = 130;
        _sceneButtonsConfig.deleteLastPathRect.height = 30;

        _sceneButtonsConfig.deleteLastVesselRect.width = 130;
        _sceneButtonsConfig.deleteLastVesselRect.height = 30;

        _sceneButtonsConfig.bringSeedRect.width = 130;
        _sceneButtonsConfig.bringSeedRect.height = 30;

        _sceneButtonsConfig.deletePathRect.width = 130;
        _sceneButtonsConfig.deletePathRect.height = 30;

        _sceneButtonsConfig.deleteVesselRect.width = 130;
        _sceneButtonsConfig.deleteVesselRect.height = 30;

        _sceneButtonsConfig.deleteCloserVesselsRect.width = 130;
        _sceneButtonsConfig.deleteCloserVesselsRect.height = 30;

        if (_sceneButtonsConfig.newMapRect.x == 0 && _sceneButtonsConfig.newMapRect.y == 0
            && _sceneButtonsConfig.saveNewMapRect.x == 0 && _sceneButtonsConfig.saveNewMapRect.y == 0
            && _sceneButtonsConfig.overwriteMapRect.x == 0 && _sceneButtonsConfig.overwriteMapRect.y == 0
            && _sceneButtonsConfig.deleteLastPathRect.x == 0 && _sceneButtonsConfig.deleteLastPathRect.y == 0
            && _sceneButtonsConfig.deleteLastVesselRect.x == 0 && _sceneButtonsConfig.deleteLastVesselRect.y == 0
            && _sceneButtonsConfig.bringSeedRect.x == 0 && _sceneButtonsConfig.bringSeedRect.y == 0
            && _sceneButtonsConfig.deletePathRect.x == 0 && _sceneButtonsConfig.deletePathRect.y == 0
            && _sceneButtonsConfig.deleteVesselRect.x == 0 && _sceneButtonsConfig.deleteVesselRect.y == 0
            && _sceneButtonsConfig.deleteCloserVesselsRect.x == 0 && _sceneButtonsConfig.deleteCloserVesselsRect.y == 0)
        {
            _sceneButtonsConfig.newMapRect.x = 20;
            _sceneButtonsConfig.newMapRect.y = 20;

            _sceneButtonsConfig.saveNewMapRect.x = 20;
            _sceneButtonsConfig.saveNewMapRect.y = 60;

            _sceneButtonsConfig.overwriteMapRect.x = 20;
            _sceneButtonsConfig.overwriteMapRect.y = 100;

            _sceneButtonsConfig.deleteLastPathRect.x = 20;
            _sceneButtonsConfig.deleteLastPathRect.y = 140;

            _sceneButtonsConfig.deleteLastVesselRect.x = 20;
            _sceneButtonsConfig.deleteLastVesselRect.y = 180;

            _sceneButtonsConfig.bringSeedRect.x = 20;
            _sceneButtonsConfig.bringSeedRect.y = 40;

            _sceneButtonsConfig.deletePathRect.x = 20;
            _sceneButtonsConfig.deletePathRect.y = 80;

            _sceneButtonsConfig.deleteVesselRect.x = 20;
            _sceneButtonsConfig.deleteVesselRect.y = 40;

            _sceneButtonsConfig.deleteCloserVesselsRect.x = 20;
            _sceneButtonsConfig.deleteCloserVesselsRect.y = 80;
        }
    }

}

public enum Direction
{
    Forward,
    Backward,
    Left,
    Right
}

public enum ButtonType
{
    Add,
    ChangeType
}
