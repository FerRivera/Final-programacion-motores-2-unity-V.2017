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

        StartDropListWithCurrentMaterial();
        ShowPreview();
    }

    void OnGUI()
    {
        _pathCurrentIndex = EditorGUILayout.Popup("Object type", _pathCurrentIndex, _pathsSaved.objectsToInstantiate.Select(x => x.name).ToArray());

        ShowPreview();        

        _materialsPath = AssetDatabase.FindAssets("t:Material").ToList();

        if(_materialsPath.Count != _materials.Count)
        {
            for (int i = 0; i < _materialsPath.Count(); i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(_materialsPath[i]);

                var material = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (!_materials.Contains(material))
                    _materials.Add(material);
            }
        }

        _selectedObject = (GameObject)_pathsSaved.objectsToInstantiate[_pathCurrentIndex];

        EditorGUILayout.LabelField("Object material:" + _selectedObject.GetComponent<Renderer>().sharedMaterial.name);

        _selectedObject.GetComponent<Renderer>().sharedMaterial = (Material)EditorGUILayout.ObjectField("Object material", _selectedObject.GetComponent<Renderer>().sharedMaterial, typeof(Material), true);

        _materialCurrentIndex = EditorGUILayout.Popup("Materials", _materialCurrentIndex, _materials.Select(x => x.name).ToArray());

        _selectedObject.GetComponent<Renderer>().sharedMaterial = _materials[_materialCurrentIndex];

        EditorGUILayout.HelpBox("You are modificating the prefab, to update instantiated objects click the button or reload the map",MessageType.Info);

        UpdateAllPathsMaterial();

        for (int i = 0; i < 10; i++)
        {
            EditorGUILayout.Space();
        }

        var components = _selectedObject.GetComponents<Component>().ToList();

        components.Remove(_selectedObject.GetComponent<Transform>());

        foreach (var item in components)
        {
            
            Rect rect = EditorGUILayout.BeginHorizontal();
            rect.width = 150;
            rect.height = 20;
            rect.x = 200;
            EditorGUILayout.LabelField("Component:" + item.GetType().Name);
            if (GUI.Button(rect, "Delete component"))
            {
                DestroyImmediate(item,true);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
        }
    }

    public void UpdateAllPathsMaterial()
    {
        if(GUI.Button(new Rect(5,170,150,50),"Update paths material"))
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

    private void Update()
    {
        //SceneView.RepaintAll();
    }
}
