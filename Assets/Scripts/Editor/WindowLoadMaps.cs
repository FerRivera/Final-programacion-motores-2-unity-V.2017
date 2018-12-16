using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WindowLoadMaps : EditorWindow
{
    List<bool> wantToDeleteList = new List<bool>();
    private Vector2 _scrollPosition;
    public float maxYSize = 500;
    public PathConfig pathsSaved;
    private Seed _seed;
    string _searchMap;

    [MenuItem("Tool options/Load map")]
    static void CreateWindow()
    {
        var window = ((WindowLoadMaps)GetWindow(typeof(WindowLoadMaps),false, "Load map"));
        //window.titleContent = new GUIContent("Load map");
        window.Show();
        window.Init();        
    }

    public void Init()
    {
        maxSize = new Vector2(501, 525);
        minSize = new Vector2(500, 524);

        _seed = GameObject.FindGameObjectWithTag("Seed").GetComponent<Seed>();

        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        //maxSize = new Vector2(maxXSize, maxYSize);

        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

        for (int i = 0; i < asset.Length; i++)
        {
            bool wantToDelete = false;
            wantToDeleteList.Add(wantToDelete);
        }
    }

    public void LoadMaps()
    {       
        List<string> tempPath = new List<string>();

        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);                

        _searchMap = EditorGUILayout.TextField("Search map:", _searchMap);        

        EditorGUILayout.Space();

        if (asset.Length <= 0)
            EditorGUILayout.HelpBox("There are no maps saved! You have to create a new one before", MessageType.Info);

        EditorGUILayout.HelpBox("Last map loaded is written with bold letters", MessageType.Info);

        EditorGUILayout.BeginVertical(GUILayout.Height(maxYSize));
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, true, true);        

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

            if (!string.IsNullOrEmpty(_searchMap) && !currentMapName[0].ToLower().Contains(_searchMap.ToLower()))
                continue;

            //EditorGUI.BeginDisabledGroup(true);
            //currentMapName[0] = EditorGUILayout.TextField("Map name", currentMapName[0]);


            if(_seed.mapLoadedIndex == i)
            {
                EditorGUILayout.LabelField("Map name: " + currentMapName[0], EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Total polygons: " + AssetDatabase.LoadAssetAtPath<MapsSaved>(path).totalPolygons, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Path: " + path, EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("Map name: " + currentMapName[0]);
                EditorGUILayout.LabelField("Total polygons: " + AssetDatabase.LoadAssetAtPath<MapsSaved>(path).totalPolygons);
                EditorGUILayout.LabelField("Path: " + path);
            }           

            //EditorGUI.EndDisabledGroup();

            if (!wantToDeleteList[i])
            {
                if (!wantToDeleteList[i] && GUILayout.Button("Delete map"))
                {
                    wantToDeleteList[i] = true;                    
                }
            }
            else
            {
                if (GUILayout.Button("No") && wantToDeleteList[i])
                {
                    wantToDeleteList[i] = false;
                }
                if (GUILayout.Button("Yes") && wantToDeleteList[i])
                {
                    wantToDeleteList[i] = false;
                    AssetDatabase.DeleteAsset(path);                    
                    if (_seed.currentMap == currentMap)
                    {
                        _seed.mapLoaded = false;
                        _seed.mapLoadedIndex = -1;
                    }                    
                }
            }

            if(GUILayout.Button("Load map"))
            {
                _seed.mapNameLoaded = currentMapName[0];
                currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>(path);
                _seed.currentMap = currentMap;
                _seed.mapLoadedIndex = i;
                LoadMapOnScene(currentMap);
            }

            EditorGUILayout.Space();
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public void LoadMapOnScene(MapsSaved map)
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
        pathsSaved.vesselsType.Clear();
        pathsSaved.vesselsPositions.Clear();
        pathsSaved.vesselsDistance.Clear();

        int count = map.paths.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject path = (GameObject)Instantiate(pathsSaved.objectsToInstantiate[map.objectType[i]]);
            path.transform.position = map.positions[i];
            path.AddComponent<Path>().currentIndex = map.objectType[i];
            path.GetComponent<Path>().lastIndex = map.objectType[i];
            path.GetComponent<Path>().id = i;
            path.transform.rotation = map.rotations[i];

            pathsSaved.paths.Add(path);
            pathsSaved.objectType.Add(map.objectType[i]);
            pathsSaved.positions.Add(path.transform.position);
            pathsSaved.rotations.Add(path.transform.rotation);
        }

        _seed.transform.position = map.positions[count - 1];       

        count = map.vessels.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject vessel = (GameObject)Instantiate(pathsSaved.vesselsToInstantiate[map.VesselsType[i]]);
            vessel.transform.position = map.vesselsPositions[i];
            vessel.AddComponent<Vessel>().currentIndex = map.VesselsType[i];
            vessel.GetComponent<Vessel>().lastIndex = map.VesselsType[i];
            vessel.GetComponent<Vessel>().id = i;
            vessel.GetComponent<Vessel>().distanceBetweenVessels = map.vesselsDistance[i];

            pathsSaved.vessels.Add(vessel);
            pathsSaved.vesselsType.Add(map.VesselsType[i]);
            pathsSaved.vesselsPositions.Add(vessel.transform.position);
            pathsSaved.vesselsDistance.Add(vessel.GetComponent<Vessel>().distanceBetweenVessels);
        }

        _seed.mapLoaded = true;
    }

    void OnGUI()
    {
        LoadMaps();
    }

}
