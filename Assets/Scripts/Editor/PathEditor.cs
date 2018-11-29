using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path _target;

    public PathConfig pathsSaved;

    private Seed _seed;

    int _buttonMinSize = 45;
    int _buttonMaxSize = 70;
    int _angleToRotate;

    void OnEnable()
    {
        _target = (Path)target;        
    }

    public override void OnInspectorGUI()
    {
        ShowValues();

        FixValues();

        Repaint();
    }

    void OnSceneGUI()
    {
        Handles.BeginGUI();        

        SetAsActualPath();

        if(pathsSaved != null)
            pathsSaved.angleToRotate = EditorGUILayout.IntField("Angle to rotate", pathsSaved.angleToRotate, GUILayout.Width(300));

        var addValue = 30 / Vector3.Distance(Camera.current.transform.position, _target.transform.position);

        DrawButton("↻", _target.transform.position + Camera.current.transform.up * addValue ,Direction.Left);
        DrawButton("↺", _target.transform.position - Camera.current.transform.up * addValue ,Direction.Right);

        Handles.EndGUI();
    }

    private void DrawButton(string text, Vector3 position , Direction dir)
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

        if (GUI.Button(r, text))
        {            
            if (dir == Direction.Left)            
                _target.transform.Rotate(new Vector3(0, pathsSaved.angleToRotate, 0));            
            else
                _target.transform.Rotate(new Vector3(0,-pathsSaved.angleToRotate, 0));
        }

    }
    private void ShowValues()
    {
        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        _seed = GameObject.FindGameObjectWithTag("Seed").GetComponent<Seed>();

        _target.currentIndex = EditorGUILayout.Popup("Actual type", _target.currentIndex, pathsSaved.objectsToInstantiate.Select(x => x.name).ToArray());

        ShowPreview();
        //_target.id = EditorGUILayout.IntField("ID", _target.id);

        SwitchType();
    }

    void ShowPreview()
    {
        var _preview = AssetPreview.GetAssetPreview(pathsSaved.objectsToInstantiate[_target.currentIndex]);

        if (_preview != null)
        {
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(pathsSaved.objectsToInstantiate[_target.currentIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(pathsSaved.objectsToInstantiate[_target.currentIndex]));
            GUILayout.EndHorizontal();
        }
    }

    void SwitchType()
    {
        if(_target.lastIndex != _target.currentIndex)
        {            
            GameObject path = (GameObject)Instantiate(pathsSaved.objectsToInstantiate[_target.currentIndex]);
            path.transform.position = pathsSaved.paths[_target.id].transform.position;

            path.AddComponent<Path>().currentIndex = _target.currentIndex;
            path.GetComponent<Path>().lastIndex = _target.currentIndex;
            path.GetComponent<Path>().id = _target.id;

            DestroyImmediate(pathsSaved.paths[_target.id]);

            pathsSaved.paths.Remove(pathsSaved.paths[_target.id]);
            pathsSaved.objectType.Remove(pathsSaved.objectType[_target.id]);
            pathsSaved.positions.Remove(pathsSaved.positions[_target.id]);

            pathsSaved.paths.Insert(_target.id, path);
            pathsSaved.objectType.Insert(_target.id, path.GetComponent<Path>().currentIndex);
            pathsSaved.positions.Insert(_target.id, path.transform.position);

            Selection.activeObject = path;
        }
    }

    void SetAsActualPath()
    {
        if (GUI.Button(new Rect(20, 50, 130, 30), "Bring seed"))
        {
            pathsSaved.paths[pathsSaved.paths.Count - 1].GetComponent<Path>().id = _target.id;

            _seed.transform.position = _target.transform.position;
            Swap(pathsSaved.paths, _target.id, pathsSaved.paths.Count-1);
            Swap(pathsSaved.positions, _target.id, pathsSaved.positions.Count-1);
            Swap(pathsSaved.objectType, _target.id, pathsSaved.objectType.Count-1);

            _target.id = pathsSaved.paths.Count-1;

            Selection.activeGameObject = _seed.gameObject;
        }
    }

    public void Swap<T>(IList<T> list, int itemToMove, int placeLast)
    {
        T tmp = list[itemToMove];
        list[itemToMove] = list[placeLast];
        list[placeLast] = tmp;
    }

    private void FixValues()
    {

    }
}
