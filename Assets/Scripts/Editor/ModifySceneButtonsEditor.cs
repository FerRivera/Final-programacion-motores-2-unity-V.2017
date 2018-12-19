using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModifySceneButtonsEditor : EditorWindow
{
    Seed _seed;
    SceneButtonsConfig _sceneButtonsConfig;
    Rect _cameraRect;
    PathConfig _pathsSaved;

    [MenuItem("Tool options/Modify scene buttons")]
    static void CreateWindow()
    {
        var window = ((ModifySceneButtonsEditor)GetWindow(typeof(ModifySceneButtonsEditor), false, "Modify buttons"));
        window.Show();
        window.Init();
    }

    public void Init()
    {
        maxSize = new Vector2(501, 525);
        minSize = new Vector2(500, 524);

        _seed = GameObject.FindGameObjectWithTag("Seed").GetComponent<Seed>();

        _sceneButtonsConfig = (SceneButtonsConfig)Resources.Load("SceneButtonsConfig");

        if (_sceneButtonsConfig == null)
        {
            ScriptableObjectsCreator.CreateSceneButtonsConfig();
            _sceneButtonsConfig = (SceneButtonsConfig)Resources.Load("SceneButtonsConfig");
        }

        EditorUtility.SetDirty(_sceneButtonsConfig);

        _pathsSaved = (PathConfig)Resources.Load("PathConfig");

        if (_pathsSaved == null)
        {
            ScriptableObjectsCreator.CreatePathConfig();
            _pathsSaved = (PathConfig)Resources.Load("PathConfig");
        }

        _cameraRect = GetWindow<SceneView>().camera.pixelRect;

        ConfigurateButtonsByDefault();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(4, 10, 100, 40), "Seed buttons"))
        {
            _sceneButtonsConfig.pathButton = false;
            _sceneButtonsConfig.vesselButton = false;
            _sceneButtonsConfig.seedButton = true;
        }            

        if (_pathsSaved.paths.Count > 0 && GUI.Button(new Rect(144, 10, 100, 40), "Path buttons"))
        {
            _sceneButtonsConfig.seedButton = false;
            _sceneButtonsConfig.vesselButton = false;
            _sceneButtonsConfig.pathButton = true;
        }

        if (_pathsSaved.vessels.Count > 0 && GUI.Button(new Rect(288, 10, 100, 40), "Vessels buttons"))
        {
            _sceneButtonsConfig.seedButton = false;
            _sceneButtonsConfig.pathButton = false;
            _sceneButtonsConfig.vesselButton = true;
        }

        if (_sceneButtonsConfig.seedButton)
            ModifySeedGUI();

        if (_sceneButtonsConfig.pathButton)
            ModifyPathGUI();

        if (_sceneButtonsConfig.vesselButton)
            ModifyVesselGUI();
    }

    public void ModifyVesselGUI()
    {
        for (int i = 0; i < 8; i++)
        {
            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField("Delete vessel button");

        EditorGUILayout.Space();

        _sceneButtonsConfig.deleteVesselRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.deleteVesselRect.x);
        _sceneButtonsConfig.deleteVesselRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.deleteVesselRect.y);

        if (_sceneButtonsConfig.deleteVesselRect.x <= 0)
            _sceneButtonsConfig.deleteVesselRect.x = 0;

        if (_sceneButtonsConfig.deleteVesselRect.x >= _cameraRect.width - _sceneButtonsConfig.deleteVesselRect.width)
            _sceneButtonsConfig.deleteVesselRect.x = _cameraRect.width - _sceneButtonsConfig.deleteVesselRect.width;

        if (_sceneButtonsConfig.deleteVesselRect.y <= 0)
            _sceneButtonsConfig.deleteVesselRect.y = 0;

        if (_sceneButtonsConfig.deleteVesselRect.y >= _cameraRect.height - _sceneButtonsConfig.deleteVesselRect.height)
            _sceneButtonsConfig.deleteVesselRect.y = _cameraRect.height - _sceneButtonsConfig.deleteVesselRect.height;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Delete closer vessels button");

        EditorGUILayout.Space();

        _sceneButtonsConfig.deleteCloserVesselsRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.deleteCloserVesselsRect.x);
        _sceneButtonsConfig.deleteCloserVesselsRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.deleteCloserVesselsRect.y);

        if (_sceneButtonsConfig.deleteCloserVesselsRect.x <= 0)
            _sceneButtonsConfig.deleteCloserVesselsRect.x = 0;

        if (_sceneButtonsConfig.deleteCloserVesselsRect.x >= _cameraRect.width - _sceneButtonsConfig.deleteCloserVesselsRect.width)
            _sceneButtonsConfig.deleteCloserVesselsRect.x = _cameraRect.width - _sceneButtonsConfig.deleteCloserVesselsRect.width;

        if (_sceneButtonsConfig.deleteCloserVesselsRect.y <= 0)
            _sceneButtonsConfig.deleteCloserVesselsRect.y = 0;

        if (_sceneButtonsConfig.deleteCloserVesselsRect.y >= _cameraRect.height - _sceneButtonsConfig.deleteCloserVesselsRect.height)
            _sceneButtonsConfig.deleteCloserVesselsRect.y = _cameraRect.height - _sceneButtonsConfig.deleteCloserVesselsRect.height;
    }

    public void ModifyPathGUI()
    {
        for (int i = 0; i < 8; i++)
        {
            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField("Bring seed button");

        EditorGUILayout.Space();

        _sceneButtonsConfig.bringSeedRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.bringSeedRect.x);
        _sceneButtonsConfig.bringSeedRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.bringSeedRect.y);

        if (_sceneButtonsConfig.bringSeedRect.x <= 0)
            _sceneButtonsConfig.bringSeedRect.x = 0;

        if (_sceneButtonsConfig.bringSeedRect.x >= _cameraRect.width - _sceneButtonsConfig.bringSeedRect.width)
            _sceneButtonsConfig.bringSeedRect.x = _cameraRect.width - _sceneButtonsConfig.bringSeedRect.width;

        if (_sceneButtonsConfig.bringSeedRect.y <= 0)
            _sceneButtonsConfig.bringSeedRect.y = 0;

        if (_sceneButtonsConfig.bringSeedRect.y >= _cameraRect.height - _sceneButtonsConfig.bringSeedRect.height)
            _sceneButtonsConfig.bringSeedRect.y = _cameraRect.height - _sceneButtonsConfig.bringSeedRect.height;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Delete path button");

        EditorGUILayout.Space();

        _sceneButtonsConfig.deletePathRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.deletePathRect.x);
        _sceneButtonsConfig.deletePathRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.deletePathRect.y);

        if (_sceneButtonsConfig.deletePathRect.x <= 0)
            _sceneButtonsConfig.deletePathRect.x = 0;

        if (_sceneButtonsConfig.deletePathRect.x >= _cameraRect.width - _sceneButtonsConfig.deletePathRect.width)
            _sceneButtonsConfig.deletePathRect.x = _cameraRect.width - _sceneButtonsConfig.deletePathRect.width;

        if (_sceneButtonsConfig.deletePathRect.y <= 0)
            _sceneButtonsConfig.deletePathRect.y = 0;

        if (_sceneButtonsConfig.deletePathRect.y >= _cameraRect.height - _sceneButtonsConfig.deletePathRect.height)
            _sceneButtonsConfig.deletePathRect.y = _cameraRect.height - _sceneButtonsConfig.deletePathRect.height;

    }

    public void ModifySeedGUI()
    {
        for (int i = 0; i < 8; i++)
        {
            EditorGUILayout.Space();
        }

        EditorGUILayout.LabelField("New map button");

        EditorGUILayout.Space();

        _sceneButtonsConfig.newMapRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.newMapRect.x);
        _sceneButtonsConfig.newMapRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.newMapRect.y);

        if (_sceneButtonsConfig.newMapRect.x <= 0)
            _sceneButtonsConfig.newMapRect.x = 0;

        if (_sceneButtonsConfig.newMapRect.x >= _cameraRect.width - _sceneButtonsConfig.newMapRect.width)
            _sceneButtonsConfig.newMapRect.x = _cameraRect.width - _sceneButtonsConfig.newMapRect.width;

        if (_sceneButtonsConfig.newMapRect.y <= 0)
            _sceneButtonsConfig.newMapRect.y = 0;

        if (_sceneButtonsConfig.newMapRect.y >= _cameraRect.height - _sceneButtonsConfig.newMapRect.height)
            _sceneButtonsConfig.newMapRect.y = _cameraRect.height - _sceneButtonsConfig.newMapRect.height;

        EditorGUILayout.Space();

        if (_pathsSaved.paths.Count > 0)
        {
            EditorGUILayout.LabelField("Save new map button");

            EditorGUILayout.Space();

            _sceneButtonsConfig.saveNewMapRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.saveNewMapRect.x);
            _sceneButtonsConfig.saveNewMapRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.saveNewMapRect.y);

            if (_sceneButtonsConfig.saveNewMapRect.x <= 0)
                _sceneButtonsConfig.saveNewMapRect.x = 0;

            if (_sceneButtonsConfig.saveNewMapRect.x >= _cameraRect.width - _sceneButtonsConfig.saveNewMapRect.width)
                _sceneButtonsConfig.saveNewMapRect.x = _cameraRect.width - _sceneButtonsConfig.saveNewMapRect.width;

            if (_sceneButtonsConfig.saveNewMapRect.y <= 0)
                _sceneButtonsConfig.saveNewMapRect.y = 0;

            if (_sceneButtonsConfig.saveNewMapRect.y >= _cameraRect.height - _sceneButtonsConfig.saveNewMapRect.height)
                _sceneButtonsConfig.saveNewMapRect.y = _cameraRect.height - _sceneButtonsConfig.saveNewMapRect.height;

            EditorGUILayout.Space();
        }

        if (_seed.mapLoaded)
        {
            EditorGUILayout.LabelField("Overwrite map button");

            EditorGUILayout.Space();

            _sceneButtonsConfig.overwriteMapRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.overwriteMapRect.x);
            _sceneButtonsConfig.overwriteMapRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.overwriteMapRect.y);

            if (_sceneButtonsConfig.overwriteMapRect.x <= 0)
                _sceneButtonsConfig.overwriteMapRect.x = 0;

            if (_sceneButtonsConfig.overwriteMapRect.x >= _cameraRect.width - _sceneButtonsConfig.overwriteMapRect.width)
                _sceneButtonsConfig.overwriteMapRect.x = _cameraRect.width - _sceneButtonsConfig.overwriteMapRect.width;

            if (_sceneButtonsConfig.overwriteMapRect.y <= 0)
                _sceneButtonsConfig.overwriteMapRect.y = 0;

            if (_sceneButtonsConfig.overwriteMapRect.y >= _cameraRect.height - _sceneButtonsConfig.overwriteMapRect.height)
                _sceneButtonsConfig.overwriteMapRect.y = _cameraRect.height - _sceneButtonsConfig.overwriteMapRect.height;

            EditorGUILayout.Space();
        }

        if (_pathsSaved.paths.Count > 0)
        {
            EditorGUILayout.LabelField("Delete last path button");

            EditorGUILayout.Space();

            _sceneButtonsConfig.deleteLastPathRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.deleteLastPathRect.x);
            _sceneButtonsConfig.deleteLastPathRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.deleteLastPathRect.y);

            if (_sceneButtonsConfig.deleteLastPathRect.x <= 0)
                _sceneButtonsConfig.deleteLastPathRect.x = 0;

            if (_sceneButtonsConfig.deleteLastPathRect.x >= _cameraRect.width - _sceneButtonsConfig.deleteLastPathRect.width)
                _sceneButtonsConfig.deleteLastPathRect.x = _cameraRect.width - _sceneButtonsConfig.deleteLastPathRect.width;

            if (_sceneButtonsConfig.deleteLastPathRect.y <= 0)
                _sceneButtonsConfig.deleteLastPathRect.y = 0;

            if (_sceneButtonsConfig.deleteLastPathRect.y >= _cameraRect.height - _sceneButtonsConfig.deleteLastPathRect.height)
                _sceneButtonsConfig.deleteLastPathRect.y = _cameraRect.height - _sceneButtonsConfig.deleteLastPathRect.height;

            EditorGUILayout.Space();
        }

        if (_pathsSaved.vessels.Count > 0)
        {
            EditorGUILayout.LabelField("Delete last vessel button");

            EditorGUILayout.Space();

            _sceneButtonsConfig.deleteLastVesselRect.x = EditorGUILayout.FloatField("X position:", _sceneButtonsConfig.deleteLastVesselRect.x);
            _sceneButtonsConfig.deleteLastVesselRect.y = EditorGUILayout.FloatField("Y position:", _sceneButtonsConfig.deleteLastVesselRect.y);

            if (_sceneButtonsConfig.deleteLastVesselRect.x <= 0)
                _sceneButtonsConfig.deleteLastVesselRect.x = 0;

            if (_sceneButtonsConfig.deleteLastVesselRect.x >= _cameraRect.width - _sceneButtonsConfig.deleteLastVesselRect.width)
                _sceneButtonsConfig.deleteLastVesselRect.x = _cameraRect.width - _sceneButtonsConfig.deleteLastVesselRect.width;

            if (_sceneButtonsConfig.deleteLastVesselRect.y <= 0)
                _sceneButtonsConfig.deleteLastVesselRect.y = 0;

            if (_sceneButtonsConfig.deleteLastVesselRect.y >= _cameraRect.height - _sceneButtonsConfig.deleteLastVesselRect.height)
                _sceneButtonsConfig.deleteLastVesselRect.y = _cameraRect.height - _sceneButtonsConfig.deleteLastVesselRect.height;

            EditorGUILayout.Space();
        }
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
