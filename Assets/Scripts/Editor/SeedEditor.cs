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

    int _buttonMinSize = 45;
    int _buttonMaxSize = 70;

    public PathConfig pathsSaved;

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

        OpenSaveMapWindow();

        NewMap();

        DeleteLastPath();
        DeleteLastVessel();
        
        DrawButton("+", _target.transform.position + Camera.current.transform.up * addValue);
        DrawButton("+", _target.transform.position - Camera.current.transform.up * addValue);
        DrawButton("+", _target.transform.position + Camera.current.transform.right * addValue);
        DrawButton("+", _target.transform.position - Camera.current.transform.right * addValue);        

        SaveMap();

        Handles.EndGUI();
    }

    public void OpenSaveMapWindow()
    {
        if (!_target.mapLoaded)
        {
            if (GUI.Button(new Rect(20, 140, buttonWidth, buttonHeight), "Save new Map"))
            {
                WindowSaveMaps.CreateWindow();
            }
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
        if (!newMap && GUI.Button(new Rect(20, 20, buttonWidth, buttonHeight), "New Map"))
        {                
            newMap = true;
        }

        if(newMap && GUI.Button(new Rect(20, 20, buttonWidth, buttonHeight), "No"))
        {
            newMap = false;
        }
        if (newMap && GUI.Button(new Rect(160, 20, buttonWidth, buttonHeight), "Yes"))
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
    }

    void DeleteLastPath()
    {
        if (GUI.Button(new Rect(20, 60, buttonWidth, buttonHeight), "Delete Last Path"))
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
        if (GUI.Button(new Rect(20, 100, buttonWidth, buttonHeight), "Delete Last Vessel"))
        {
            if (pathsSaved.vessels.Count > 0)
            {
                var lastObject = pathsSaved.vessels.LastOrDefault();

                pathsSaved.vesselsType.RemoveAt(pathsSaved.vessels.Count - 1);
                pathsSaved.vesselsPositions.RemoveAt(pathsSaved.vessels.Count - 1);
                pathsSaved.vessels.Remove(lastObject);
                pathsSaved.vesselsDistance.Remove(pathsSaved.vesselsDistance.Count - 1);

                DestroyImmediate(lastObject);
            }            
        }
    }

    private void DrawButton(string text, Vector3 position)
    {
        var p = Camera.current.WorldToScreenPoint(position);

        var size = 700 / Vector3.Distance(Camera.current.transform.position, position);
        size = Mathf.Clamp(size, _buttonMinSize, _buttonMaxSize);

        var r = new Rect(p.x - size / 2, Screen.height - p.y - size, size, size / 2);

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
        if (_target.mapLoaded)
        {
            if (!saveMap && GUI.Button(new Rect(20, 140, buttonWidth, buttonHeight), "Save Map"))
            {
                saveMap = true;
            }

            if (saveMap && GUI.Button(new Rect(20, 140, buttonWidth, buttonHeight), "No"))
            {
                saveMap = false;
            }
            if (saveMap && GUI.Button(new Rect(160, 140, buttonWidth, buttonHeight), "Yes"))
            {
                //List<string> tempPath = new List<string>();

                //var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

                //MapsSaved _targetcurrentMap = null;

                //for (int i = asset.Length - 1; i >= 0; i--)
                //{
                    //string path = AssetDatabase.GUIDToAssetPath(asset[i]);
                    ////separo las diferentes carpetas por el carcater /
                    //tempPath = path.Split('/').ToList();

                    //if (path == "Assets/" + _path + "/" + _mapName + ".asset")
                    //{
                    //    if (File.Exists(_fullPath + "/" + tempPath.Last()))
                    //    {
                    //        currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>("Assets/" + _path + "/" + _mapName + ".asset");
                    //        _seed.mapNameLoaded = _mapName;
                    //        _seed.mapLoaded = true;
                    //        break;
                    //    }
                    //}

                    ////obtengo todo el path
                    //string path = AssetDatabase.GUIDToAssetPath(asset[i]);
                    ////separo las diferentes carpetas por el carcater /
                    //tempPath = path.Split('/').ToList();
                    ////obtengo la ultima parte, que seria el nombre con la extension y saco la extension
                    //var currentMapName = tempPath.LastOrDefault().Split('.');
                    ////si el nombre que obtuve con el que escribi son iguales entonces uso ese scriptable object
                    //if (currentMapName[0] == _target.mapNameLoaded)
                    //{
                    //    currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>(path);
                    //    break;
                    //}
                //}

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

                    //esto hace que cuando cierro unity y lo vuelvo a abrir no se pierda la info
                    EditorUtility.SetDirty(_target.currentMap);
                    saveMap = false;
                }
            }
        }
    }

    Vector3 GetNextMove(GameObject go, Direction direction)
    {
        Vector3 DistanceToReturn = new Vector3(0, 0, 0);
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
