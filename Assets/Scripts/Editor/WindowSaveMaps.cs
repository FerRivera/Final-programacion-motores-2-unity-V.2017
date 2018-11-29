using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; //Siempre que trabajamos con editor usamos UnityEditor
using System.Threading;
using System;
using System.Linq;

public class WindowSaveMaps : EditorWindow // Tiene que heredar de Editor Window
{ 

    private bool _groupEnabled;
    private bool _groupBoolExample;
    private string _mapName;
    private string _path;
    private int click;
    public PathConfig pathsSaved;

    [MenuItem("Level options/Save new map")] // La ubicación dentro del editor de Unity
    static void CreateWindow() // Crea la ventana a mostrar
    {
        var window = ((WindowSaveMaps)GetWindow(typeof(WindowSaveMaps))); //Esta línea va a obtener la ventana o a crearla. Una vez que haga esto, va a mostrarla.
        window.Show();
        window.Init();
    }

    public void Init()
    {
        pathsSaved = (PathConfig)Resources.Load("PathConfig");
    }

    public void SaveMap()
    {        
        _mapName = EditorGUILayout.TextField("Map name", _mapName);

        EditorGUI.BeginDisabledGroup(true);
        _path = EditorGUILayout.TextField("Path Selected", _path);
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Select folder"))
        {
            _path = EditorUtility.OpenFolderPanel("Select folder", "Assets/", _path);

            if (!String.IsNullOrEmpty(_path))            
                _path = _path.Split(new[] { "Assets/" }, StringSplitOptions.None)[1];                     

            Repaint();
        }

        if (GUILayout.Button("Save map"))
        {
            List<string> tempPath = new List<string>();

            if (!String.IsNullOrEmpty(_mapName))
            {
                if (!CheckIfNameExist(_mapName))
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
                            //obtengo la ultima parte, que seria el nombre con la extension y saco la extension
                            var currentMapName = tempPath.LastOrDefault().Split('.');
                            //si el nombre que obtuve con el que escribi son iguales entonces uso ese scriptable object
                            if (currentMapName[0] == _mapName)
                            {
                                currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>(path);
                                break;
                            }
                        }

                        if (currentMap != null)
                        {
                            currentMap.paths.AddRange(pathsSaved.paths);
                            currentMap.objectType.AddRange(pathsSaved.objectType);
                            currentMap.positions.AddRange(pathsSaved.positions);

                            currentMap.vessels.AddRange(pathsSaved.vessels);
                            currentMap.VesselsType.AddRange(pathsSaved.vesselsType);
                            currentMap.vesselsPositions.AddRange(pathsSaved.vesselsPositions);

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
            EditorGUILayout.HelpBox("There are no paths created!", MessageType.Warning);

        if (CheckIfNameExist(_mapName))
            EditorGUILayout.HelpBox("That name is already in use!", MessageType.Warning);
    }

    public bool CheckIfNameExist(string fileName)
    {
        List<string> tempPath = new List<string>();

        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

        for (int i = asset.Length - 1; i >= 0; i--)
        {
            //obtengo todo el path
            string path = AssetDatabase.GUIDToAssetPath(asset[i]);
            //separo las diferentes carpetas por el carcater /
            tempPath = path.Split('/').ToList();
            //obtengo la ultima parte, que seria el nombre con la extension y saco la extension
            var currentMapName = tempPath.LastOrDefault().Split('.');
            //si el nombre que obtuve con el que escribi son iguales entonces uso ese scriptable object
            if (currentMapName[0] == fileName)
            {                
                return true;
            }
        }

        return false;
    }

    void OnGUI() // Todo lo que se muestra en la ventana
    {
        SaveMap();

        //_pathAdministrator.Update();
        //for (int i = 0; i < _groupFloat; i++)
        //{
        //    _goToPreview = (GameObject)EditorGUILayout.ObjectField("Object: ", _goToPreview, typeof(GameObject), true);
        //    _preview = AssetPreview.GetAssetPreview(_goToPreview);
        //}

        //for (int i = 0; i < _groupFloat; i++)
        //{
        //    _goToPreview = (GameObject)EditorGUILayout.ObjectField("Object: ", _goToPreview, typeof(GameObject), true);
        //    _preview = AssetPreview.GetAssetPreview(_goToPreview);
        //}

        //for (int i = 0; i < _groupFloat; i++)
        //{
        //    GameObject _goToPreview;
        //}


        /*
        EJEMPLOS DE LABELS:
        
        Por el primer parámetro pasamos el texto, por el segundo el estilo del label.
        */
        //GUILayout.Label("Label", EditorStyles.label);
        //GUILayout.Label("Bold Label", EditorStyles.boldLabel);
        //GUILayout.Label("Mini Label", EditorStyles.miniLabel);

        ///*
        //EJEMPLO GROUP:

        //Para iniciar un grupo se utiliza EditorGUILayout.BeginToggleGroup(Texto del toggle, si se habilita o no).
        //Todo lo que sigue dentro del mismo es un grupo.
        //Para terminar el grupo se usa EditorGUILayout.EndToogleGroup().
        //*/
        //_groupEnabled = EditorGUILayout.BeginToggleGroup("Toggle Group Example", _groupEnabled);
        //_groupBoolExample = EditorGUILayout.Toggle("Bool Example", _groupBoolExample);
        //_groupStringExample = EditorGUILayout.TextField("String Example", _groupStringExample);
        //_groupFloatExample = EditorGUILayout.FloatField("Float Example", _groupFloatExample);
        //EditorGUILayout.EndToggleGroup();

        ///*
        //EJEMPLO DE UN RECT:

        //Para crear un Rect, hacemos EditorGUILayout.BeginHorizontal() y dentro vamos a poner todo el contenido.
        //Para terminarlo, usamos EditorGUILayout.EndHorizontal()
        //*/
        //EditorGUILayout.BeginHorizontal(GUIStyle.none);
        //GUILayout.Label("No hago nada!");
        //EditorGUILayout.EndHorizontal();

        ///*
        //EJEMPLO BOTON:

        //En este ejemplo, tenemos un boton que con cada click suma uno
        //*/
        //Rect rectAdd = EditorGUILayout.BeginHorizontal("Button");
        //if (GUI.Button(rectAdd, GUIContent.none))
        //    _clicks++;
        //GUILayout.Label("+1 Click");
        //EditorGUILayout.EndHorizontal();
        //GUILayout.Label("Total Clicks: " + _clicks);

        ///*
        //EJEMPLO CERRAR VENTANA:

        //Creamos un botón que al hacerle click se cierre la ventana.
        //Como nuestra ventana hereda de EditorWindow, podemos usar la función Close()
        //*/
        //Rect rectClose = EditorGUILayout.BeginHorizontal("Button");
        //if (GUI.Button(rectClose, GUIContent.none))
        //    Close();
        //GUILayout.Label("Cerrar Ventana");
        //EditorGUILayout.EndHorizontal();

        ///*
        //EJEMPLO ABRIR OTRA VENTANA:

        //Creamos un botón que al hacerle click abra otra ventana.
        //*/
        //Rect rectOpenNew = EditorGUILayout.BeginHorizontal("Button");
        //if (GUI.Button(rectOpenNew, GUIContent.none))
        //    //((OpenAnotherWindowExample)GetWindow(typeof(OpenAnotherWindowExample))).Show();
        //GUILayout.Label("Abrir la otra ventana");
        //EditorGUILayout.EndHorizontal();

        ///*
        //EJEMPLO FOCO:

        //Podemos saber si el usuario tiene el foco sobre esta ventana
        //*/
        //if (focusedWindow == this) EditorGUILayout.LabelField("Tengo el foco en esta ventana");

        ///*
        //EJEMPLO MOUSE OVER:

        //Podemos saber si el usuario tiene el mouse sobre la ventana
        //*/
        //if (mouseOverWindow == this) EditorGUILayout.LabelField("Tengo el mouse sobre esta ventana");

        ///*
        //EJEMPLO CAMBIAR TAMAÑO:

        //Podemos cambiar el tamaño de la ventana
        //*/
        //maxSize = new Vector2(300, 500);
        //minSize = new Vector2(300, 500);

        ///*
        //EJEMPLO LEER MOVIMIENTO DEL MOUSE:

        //Podemos detectar los eventos de movimiento del mouse
        //*/
        //wantsMouseMove = true; // Por defecto viene false. Sin esto no podemos detectar los comportamientos del mouse
        //EditorGUILayout.LabelField("Posición del mouse: ", Event.current.mousePosition.ToString());
        //if (Event.current.type == EventType.MouseMove) Repaint();

        ///*
        //EJEMPLO MOSTRAR UN PREVIEW:

        //Podemos mostrar un preview de casi cualquier cosa
        //*/
        //_goToPreview = (GameObject)EditorGUILayout.ObjectField("Objeto: ", _goToPreview, typeof(GameObject), true);
        //_preview = AssetPreview.GetAssetPreview(_goToPreview);
        //if (_preview != null)
        //{
        //    Repaint();
        //    GUILayout.BeginHorizontal();
        //    GUI.DrawTexture(GUILayoutUtility.GetRect(50, 50, 50, 50), _preview, ScaleMode.ScaleToFit);
        //    GUILayout.Label(_goToPreview.name);
        //    GUILayout.Label(AssetDatabase.GetAssetPath(_goToPreview));
        //    GUILayout.EndHorizontal();
        //}
        //else
        //    EditorGUILayout.LabelField("No existe ninguna preview");
    }

    void OnDestroy() { } // Se llama cuando se cierra la ventana
    void OnFocus() { } // Se llama cuando pongo el foco
    void OnLostFocus() { } // Se llama cuando se pierde el foco
    void OnHierarchyChange() { } // Se llama cuando cambia la jeraquía
    void OnInspectorUpdate() { } // Se llama 10 frames por segundo para que se pueda updatear el inspector
    void Update() { } // Se llama varias veces por segundo en todas las ventanas visibles
}