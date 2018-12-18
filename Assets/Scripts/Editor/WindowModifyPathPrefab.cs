using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WindowModifyPathPrefab : EditorWindow
{
    int _pathCurrentIndex;
    int _materialCurrentIndex;
    int _initialMaterialIndex;
    PathConfig _pathsSaved;
    GameObject _selectedObject;
    List<Material> _materials = new List<Material>();
    List<string> _materialsPath = new List<string>();
    Texture2D _preview;
    GUIStyle _guiStyle = new GUIStyle();
    List<MonoScript> _scripts = new List<MonoScript>();
    List<MonoScript> _scriptsFinal = new List<MonoScript>();
    List<MonoScript> _scriptsTemp = new List<MonoScript>();
    Vector2 _scrollComponentsPosition;
    Vector2 _scrollScriptsPosition;

    public static void CreateWindow()
    {
        var window = ((WindowModifyPathPrefab)GetWindow(typeof(WindowModifyPathPrefab), false, "Modify prefab"));
        window.Show();
        window.Init();
    }

    public void Init()
    {
        maxSize = new Vector2(501, 525);
        minSize = new Vector2(500, 524);

        _pathsSaved = (PathConfig)Resources.Load("PathConfig");

        if (_pathsSaved == null)
        {
            ScriptableObjectsCreator.CreateVesselsConfig();
            _pathsSaved = (PathConfig)Resources.Load("PathConfig");
        }

        _pathCurrentIndex = _pathsSaved.pathTypeSelected;

        StartDropListWithCurrentMaterial();
        ShowPreview();

        _guiStyle.fontSize = 15;
        _guiStyle.fontStyle = FontStyle.Bold;

        GetAllScripts();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Material:", _guiStyle);

        EditorGUILayout.Space();

        _pathCurrentIndex = EditorGUILayout.Popup("Prefab type", _pathCurrentIndex, _pathsSaved.objectsToInstantiate.Select(x => x.name).ToArray());

        ShowPreview();

        EditorGUILayout.HelpBox("Changing the material will directly affect the prefab!", MessageType.Warning);

        GetAllMaterials();

        _selectedObject = (GameObject)_pathsSaved.objectsToInstantiate[_pathCurrentIndex];

        if (_selectedObject.GetComponent<Renderer>() != null)
            EditorGUILayout.LabelField("Prefab material:" + _selectedObject.GetComponent<Renderer>().sharedMaterial.name);

        //_selectedObject.GetComponent<Renderer>().sharedMaterial = (Material)EditorGUILayout.ObjectField("Prefab material", _selectedObject.GetComponent<Renderer>().sharedMaterial, typeof(Material), true);

        _materialCurrentIndex = EditorGUILayout.Popup("Materials", _materialCurrentIndex, _materials.Select(x => x.name).ToArray());

        if (_selectedObject.GetComponent<Renderer>() != null)
            _selectedObject.GetComponent<Renderer>().sharedMaterial = _materials[_materialCurrentIndex];        

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Components:", _guiStyle);

        EditorGUILayout.Space();

        GetAllComponents();        

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Add scripts:", _guiStyle);

        DrawScriptsArea();

        UpdateAllPathsMaterial();
        UpdateAllPathsComponents();

        EditorGUILayout.HelpBox("You are modificating the prefab, to update instantiated objects click the buttons or reload the map", MessageType.Info);
    }

    public void GetAllScripts()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths().ToList();

        var itemToRemove = assetPaths.Where(x => x.Contains(GetType().Name)).FirstOrDefault();

        assetPaths.Remove(itemToRemove);

        foreach (string assetPath in assetPaths)
        {
            if (assetPath.EndsWith(".cs"))
            {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                string temp = script.ToString();

                if (temp.Contains("MonoBehaviour"))
                    _scripts.Add(script);
            }
        }        

        if(_selectedObject != null)
        {
            _scriptsFinal = _scripts;
            _scriptsTemp = _scriptsFinal;

            var scriptsAttachedToObject = _selectedObject.gameObject.GetComponents<MonoBehaviour>().ToList();

            foreach (var scriptAttached in scriptsAttachedToObject)
            {
                foreach (var scriptTemp in _scriptsFinal)
                {
                    if (scriptAttached.GetType().Name == scriptTemp.name)
                        _scriptsTemp.Remove(scriptTemp);
                }
            }

            _scriptsFinal = _scriptsTemp;
        }        
    }

    public void DrawScriptsArea()
    {
        EditorGUILayout.BeginVertical();
        _scrollScriptsPosition = EditorGUILayout.BeginScrollView(_scrollScriptsPosition, GUILayout.Width(maxSize.x), GUILayout.Height(100));

        foreach (var item in _scriptsFinal)
        {
            Rect rect = EditorGUILayout.BeginVertical();
            rect.width = 150;
            rect.height = 20;
            rect.x = 230;
            EditorGUILayout.LabelField("Script:" + item.GetClass().Name);

            if (GUI.Button(rect, "Add Script"))
            {

            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public void GetAllComponents()
    {
        var components = _selectedObject.GetComponents<Component>().ToList();

        components.Remove(_selectedObject.GetComponent<Transform>());
        components.Remove(_selectedObject.GetComponent<Renderer>());

        EditorGUILayout.BeginVertical();
        _scrollComponentsPosition = EditorGUILayout.BeginScrollView(_scrollComponentsPosition,GUILayout.Width(maxSize.x), GUILayout.Height(100));

        foreach (var item in components)
        {
            Rect rect = EditorGUILayout.BeginVertical();
            rect.width = 150;
            rect.height = 20;
            rect.x = 230;
            EditorGUILayout.LabelField("Component:" + item.GetType().Name);
            if (GUI.Button(rect, "Delete component"))
            {
                DestroyImmediate(item, true);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public void GetAllMaterials()
    {
        _materialsPath = AssetDatabase.FindAssets("t:Material").ToList();

        if (_materialsPath.Count != _materials.Count)
        {
            for (int i = 0; i < _materialsPath.Count(); i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(_materialsPath[i]);

                var material = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (!_materials.Contains(material))
                    _materials.Add(material);
            }
        }
    }

    public void UpdateAllPathsMaterial()
    {
        if(GUI.Button(new Rect(5, maxSize.y - 70, 160,50),"Update paths material"))
        {
            foreach (var item in _pathsSaved.paths)
            {
                int itemCurrentIndex = item.gameObject.GetComponent<Path>().currentIndex;
                if (itemCurrentIndex == _pathCurrentIndex)
                    item.GetComponent<Renderer>().sharedMaterial = _materials[_materialCurrentIndex];
            }
        }
    }

    public void UpdateAllPathsComponents()
    {
        if (GUI.Button(new Rect(205, maxSize.y - 70, 160, 50), "Update paths components"))
        {
            foreach (var item in _pathsSaved.paths)
            {
                int itemCurrentIndex = item.gameObject.GetComponent<Path>().currentIndex;
                if (itemCurrentIndex == _pathCurrentIndex)
                    item.GetComponent<Renderer>().sharedMaterial = _materials[_materialCurrentIndex];
            }
        }
    }

    public void StartDropListWithCurrentMaterial()
    {
        _selectedObject = (GameObject)_pathsSaved.objectsToInstantiate[_pathCurrentIndex];

        //_selectedObject.GetComponent<Renderer>().sharedMaterial = (Material)EditorGUILayout.ObjectField("Object material", _selectedObject.GetComponent<Renderer>().sharedMaterial, typeof(Material), true);

        _materialsPath = AssetDatabase.FindAssets("t:Material").ToList();

        for (int i = 0; i < _materialsPath.Count(); i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(_materialsPath[i]);

            var material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (!_materials.Contains(material))
                _materials.Add(material);
        }

        for (int i = 0; i < _materials.Count(); i++)
        {
            if (_materials[i] == _selectedObject.GetComponent<Renderer>().sharedMaterial)
                _materialCurrentIndex = i;
        }
    }

    void ShowPreview()
    {
        _preview = null;
        if (_selectedObject.GetComponent<Renderer>() == null)
            return;

        _preview = AssetPreview.GetAssetPreview(_selectedObject.GetComponent<Renderer>().sharedMaterial);

        if (_preview != null)
        {
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(50, 50, 50, 50), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(_preview.name);
            GUILayout.Label(AssetDatabase.GetAssetPath(_selectedObject));
            GUILayout.EndHorizontal();
        }
    }

    private void OnFocus()
    {
        GetAllScripts();
    }

    private void Update()
    {
        //SceneView.RepaintAll();
    }
}
