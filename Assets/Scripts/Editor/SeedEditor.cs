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

    public bool restartMap = false;
    public bool saveMap = false;

    void OnEnable()
    {
        _target = (Seed)target;
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
        pathsSaved = (PathConfig)Resources.Load("PathConfig");

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

        RestartMap();

        DeleteLastPath();
        DeleteLastVessel();

        DrawButton("+", _target.transform.position + Camera.current.transform.up * addValue);
        DrawButton("+", _target.transform.position - Camera.current.transform.up * addValue);
        DrawButton("+", _target.transform.position + Camera.current.transform.right * addValue);
        DrawButton("+", _target.transform.position - Camera.current.transform.right * addValue);        

        SaveMap();
        Handles.EndGUI();
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

    

    void RestartMap()
    {
        if (!restartMap && GUI.Button(new Rect(20, 20, buttonWidth, buttonHeight), "Restart Map"))
        {                
            restartMap = true;
        }

        if(restartMap && GUI.Button(new Rect(20, 20, buttonWidth, buttonHeight), "No"))
        {
            restartMap = false;
        }
        if (restartMap && GUI.Button(new Rect(160, 20, buttonWidth, buttonHeight), "Yes"))
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

            _target.transform.position = new Vector3(0, 0, 0);

            restartMap = false;
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
                List<string> tempPath = new List<string>();

                var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

                MapsSaved currentMap = null;

                for (int i = asset.Length - 1; i >= 0; i--)
                {
                    //obtengo todo el path
                    string path = AssetDatabase.GUIDToAssetPath(asset[i]);
                    //separo las diferentes carpetas por el carcater /
                    tempPath = path.Split('/').ToList();
                    //obtengo la ultima parte, que seria el nombre con la extension y saco la extension
                    var currentMapName = tempPath.LastOrDefault().Split('.');
                    //si el nombre que obtuve con el que escribi son iguales entonces uso ese scriptable object
                    if (currentMapName[0] == _target.mapNameLoaded)
                    {
                        currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>(path);
                        break;
                    }
                }

                if (currentMap != null)
                {
                    currentMap.paths.Clear();
                    currentMap.objectType.Clear();
                    currentMap.positions.Clear();
                    currentMap.rotations.Clear();

                    currentMap.vessels.Clear();
                    currentMap.VesselsType.Clear();
                    currentMap.vesselsPositions.Clear();

                    currentMap.paths.AddRange(pathsSaved.paths);
                    currentMap.objectType.AddRange(pathsSaved.objectType);
                    currentMap.positions.AddRange(pathsSaved.positions);
                    currentMap.rotations.AddRange(pathsSaved.rotations);

                    currentMap.vessels.AddRange(pathsSaved.vessels);
                    currentMap.VesselsType.AddRange(pathsSaved.vesselsType);
                    currentMap.vesselsPositions.AddRange(pathsSaved.vesselsPositions);

                    //esto hace que cuando cierro unity y lo vuelvo a abrir no se pierda la info
                    EditorUtility.SetDirty(currentMap);
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
