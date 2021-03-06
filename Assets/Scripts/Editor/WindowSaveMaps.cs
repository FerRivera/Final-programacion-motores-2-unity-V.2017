﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Threading;
using System;
using System.Linq;
using System.IO;

public class WindowSaveMaps : EditorWindow
{ 

    private bool _groupEnabled;
    private bool _groupBoolExample;
    private string _mapName;
    private string _path;
    private int click;
    public float maxYSize = 800;
    public PathConfig pathsSaved;
    private string _fullPath;
    private Seed _seed;
    string noPathSelected = "No path selected";
    private Vector2 _scrollPosition;

    public static void CreateWindow()
    {
        var window = ((WindowSaveMaps)GetWindow(typeof(WindowSaveMaps)));
        window.titleContent = new GUIContent("Save new map");
        window.Show();
        window.Init();
    }

    public void Init()
    {
        maxSize = new Vector2(501, 525);
        minSize = new Vector2(500, 524);

        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        _seed = GameObject.FindGameObjectWithTag("Seed").GetComponent<Seed>();

        _fullPath = noPathSelected;

        CalculatePolygons();
    }

    void CalculatePolygons()
    {
        if (pathsSaved == null)
            return;

        pathsSaved.totalPolygons = 0;

        foreach (var item in pathsSaved.paths)
        {
            if (item != null && item.GetComponent<MeshFilter>() != null)
            {
                pathsSaved.totalPolygons += item.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
            }
        }

        foreach (var item in pathsSaved.vessels)
        {
            if (item != null && item.GetComponent<MeshFilter>() != null)
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
        
        if (String.IsNullOrEmpty(_mapName))
            EditorGUILayout.HelpBox("Name field is empty!", MessageType.Warning);

        if (CheckIfNameExist(_mapName, _fullPath))
            EditorGUILayout.HelpBox("That name is already in use!", MessageType.Warning);

        EditorGUILayout.LabelField("Path Selected: " + _fullPath);

        if (!CheckPathSelected())
            EditorGUILayout.HelpBox("Select the folder where you wish to save the map!", MessageType.Warning);

        EditorGUILayout.LabelField("Total polygons: " + pathsSaved.totalPolygons);

        if (GUILayout.Button("Select folder"))
        {
            string tempPathSeparated = "";
            _path = EditorUtility.OpenFolderPanel("Select folder", "Assets/", string.Empty);

            tempPathSeparated = _path.Split(new[] { "/" }, StringSplitOptions.None).Last();
            if (tempPathSeparated == "Assets")
                _path = _path + "/";

            _fullPath = _path;
            if (!String.IsNullOrEmpty(_path))
                _path = _path.Split(new[] { "Assets/" }, StringSplitOptions.None)[1];

            Repaint();
        }

        if (GUILayout.Button("Save map"))
        {
            List<string> tempPath = new List<string>();
            if (CheckPathSelected())
            {
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

                                if (!String.IsNullOrEmpty(_path))
                                {
                                    if (path == "Assets/" + _path + "/" + _mapName + ".asset")
                                    {
                                        if (File.Exists(_fullPath + "/" + tempPath.Last()))
                                        {
                                            currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>("Assets/" + _path + "/" + _mapName + ".asset");
                                            _seed.mapNameLoaded = _mapName;
                                            _seed.mapLoaded = true;
                                            _seed.currentMap = currentMap;
                                            _seed.mapLoadedIndex = i;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (path == "Assets/" + _mapName + ".asset")
                                    {
                                        if (File.Exists(_fullPath + "/" + tempPath.Last()))
                                        {
                                            currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>("Assets/" + _mapName + ".asset");
                                            _seed.mapNameLoaded = _mapName;
                                            _seed.mapLoaded = true;
                                            _seed.currentMap = currentMap;
                                            _seed.mapLoadedIndex = i;
                                            break;
                                        }
                                    }
                                }                                
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
                                currentMap.vesselsDistance.AddRange(pathsSaved.vesselsDistance);

                                currentMap.totalPolygons = pathsSaved.totalPolygons;
                                //esto hace que cuando cierro unity y lo vuelvo a abrir no se pierda la info
                                EditorUtility.SetDirty(currentMap);

                                Close();
                            }
                        }
                    }
                }
            }            
        }

        if (pathsSaved.paths.Count <= 0)
            EditorGUILayout.HelpBox("There are no objects created in the map!", MessageType.Warning);        
    }

    public bool CheckPathSelected()
    {
        if (!String.IsNullOrEmpty(_fullPath) && _fullPath != noPathSelected)
            return true;

        return false;
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

    void OnGUI()
    {
        SaveMap();

        EditorGUILayout.HelpBox("After saving the new map, you can overwrite the changes by clicking on the \"Overwrite map\" button on the scene", MessageType.Info);
    }

    private void OnFocus()
    {
        CalculatePolygons();
    }
}