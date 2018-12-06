using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; //Siempre que trabajamos con editor usamos UnityEditor
using System.Threading;
using System;
using System.Linq;
using System.IO;

public class WindowSaveMaps : EditorWindow // Tiene que heredar de Editor Window
{ 

    private bool _groupEnabled;
    private bool _groupBoolExample;
    private string _mapName;
    private string _path;
    private int click;
    public PathConfig pathsSaved;
    private string _fullPath;
    private Seed _seed;

    [MenuItem("Level options/Save new map")] // La ubicación dentro del editor de Unity
    static void CreateWindow() // Crea la ventana a mostrar
    {
        var window = ((WindowSaveMaps)GetWindow(typeof(WindowSaveMaps))); //Esta línea va a obtener la ventana o a crearla. Una vez que haga esto, va a mostrarla.
        window.titleContent = new GUIContent("Save new map");
        window.Show();
        window.Init();
    }

    public void Init()
    {    
        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        _seed = GameObject.FindGameObjectWithTag("Seed").GetComponent<Seed>();

        CalculatePolygons();
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

    public void SaveMap()
    {
        if (pathsSaved == null)
            return;

        _mapName = EditorGUILayout.TextField("Map name", _mapName);

        EditorGUI.BeginDisabledGroup(true);
        _fullPath = EditorGUILayout.TextField("Path Selected", _fullPath);
        pathsSaved.totalPolygons = EditorGUILayout.IntField("Total polygons:", pathsSaved.totalPolygons);
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Select folder"))
        {
            _path = EditorUtility.OpenFolderPanel("Select folder", "Assets/", _path);
            _fullPath = _path;
            if (!String.IsNullOrEmpty(_path))
                _path = _path.Split(new[] { "Assets/" }, StringSplitOptions.None)[1];

            Repaint();
        }

        if (GUILayout.Button("Save map"))
        {
            List<string> tempPath = new List<string>();

            if (!String.IsNullOrEmpty(_mapName))
            {
                if (!CheckIfNameExist(_mapName, _fullPath))
                {
                    if (pathsSaved.paths.Count > 0)
                    {
                        ScriptableObjectUtility.CreateAsset<MapsSaved>(_path + "/" + _mapName);

                        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

                        MapsSaved currentMap = null;

                        for (int i = asset.Length - 1; i >= 0; i--)
                        {
                            //obtengo todo el path
                            string path = AssetDatabase.GUIDToAssetPath(asset[i]);
                            //separo las diferentes carpetas por el carcater /
                            tempPath = path.Split('/').ToList();

                            if (path == "Assets/" + _path + "/" +_mapName + ".asset")
                            {
                                if (File.Exists(_fullPath + "/" + tempPath.Last()))
                                {
                                   currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>("Assets/" + _path + "/" + _mapName + ".asset");
                                    _seed.mapNameLoaded = _mapName;
                                    _seed.mapLoaded = true;
                                    break;
                                }
                            }

                            //obtengo la ultima parte, que seria el nombre con la extension y saco la extension
                            //var currentMapName = tempPath.LastOrDefault().Split('.');
                            //si el nombre que obtuve con el que escribi son iguales entonces uso ese scriptable object
                            //if (currentMapName[0] == _mapName)
                            //{
                            //    currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>(path);                                
                            //    _seed.mapNameLoaded = _mapName;
                            //    _seed.mapLoaded = true;
                            //    break;
                            //}
                        }

                        if (currentMap != null)
                        {
                            currentMap.paths.AddRange(pathsSaved.paths);
                            currentMap.objectType.AddRange(pathsSaved.objectType);
                            currentMap.positions.AddRange(pathsSaved.positions);
                            currentMap.rotations.AddRange(pathsSaved.rotations);

                            currentMap.vessels.AddRange(pathsSaved.vessels);
                            currentMap.VesselsType.AddRange(pathsSaved.vesselsType);
                            currentMap.vesselsPositions.AddRange(pathsSaved.vesselsPositions);

                            currentMap.totalPolygons = pathsSaved.totalPolygons;
                            //esto hace que cuando cierro unity y lo vuelvo a abrir no se pierda la info
                            EditorUtility.SetDirty(currentMap);
                        }
                    }
                }
            }
        }

        if(String.IsNullOrEmpty(_mapName))
            EditorGUILayout.HelpBox("Name field is empty!", MessageType.Warning);

        if (pathsSaved.paths.Count <= 0)
            EditorGUILayout.HelpBox("There are no objects created in the map!", MessageType.Warning);

        if (CheckIfNameExist(_mapName, _fullPath))
            EditorGUILayout.HelpBox("That name is already in use!", MessageType.Warning);
    }

    public bool CheckIfNameExist(string fileName, string path)
    {
        List<string> tempPath = new List<string>();
        string assetPath = "";
        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

        for (int i = asset.Length - 1; i >= 0; i--)
        {
            //obtengo todo el path
            assetPath = AssetDatabase.GUIDToAssetPath(asset[i]);

            tempPath = assetPath.Split('/').ToList();

            if (tempPath.Last() == fileName + ".asset")
            {
                if (File.Exists(path + "/" + tempPath.Last()))
                {
                    return true;
                }
            }
        }

        return false;
    }

    //public bool CheckIfNameExist(string fileName, string path)
    //{
    //    List<string> tempPath = new List<string>();

    //    var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

    //    for (int i = asset.Length - 1; i >= 0; i--)
    //    {
    //        //obtengo todo el path
    //        string assetPath = AssetDatabase.GUIDToAssetPath(asset[i]);

    //        //separo las diferentes carpetas por el caracter /
    //        tempPath = assetPath.Split('/').ToList();            

    //        //obtengo la ultima parte, que seria el nombre con la extension y saco la extension
    //        var currentMapName = tempPath.LastOrDefault().Split('.');

    //        //si el nombre que obtuve con el que escribi son iguales entonces uso ese scriptable object
    //        if (currentMapName[0] == fileName)
    //        {                
    //            return true;                
    //        }
    //    }

    //    return false;
    //}

    void OnGUI() // Todo lo que se muestra en la ventana
    {
        SaveMap();        
    }

    private void OnFocus()
    {
        CalculatePolygons();
    }
}