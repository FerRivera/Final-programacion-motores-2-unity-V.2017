using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class PathsAdministrator
{
    private List<GameObject> _previewsList = new List<GameObject>();
    private PathConfig scriptableObj;// = new ObjectsSave();
    private float _groupFloat;
    private Texture2D _preview;
    private int selectedIndex;
    private List<Object> mapItems = new List<Object>();

    public void Update()
    {
        //scriptableObj = (ObjectsSave)Resources.Load("ObjectsSave");
        SavePaths();        
    }

    public void SavePaths()
    {
        
        //mapItems = Resources.LoadAll("MapItems").ToList();        

        //selectedIndex = EditorGUILayout.Popup(selectedIndex,mapItems.Select(x => x.name).ToArray());

        #region window
        //_groupFloat = EditorGUILayout.FloatField("Amount of objects", _groupFloat);
        //_previewsList.Clear();

        //for (int i = 0; i < _groupFloat; i++)
        //{
        //    //if (_previewsList.Count <= i || _previewsList[i] != null)
        //    //{

        //    //}

        //    //creo un objeto null para que la lista por default tenga algo
        //    //GameObject temp = null;

        //    //if(temp != null)
        //    //    _previewsList.Add(temp);

        //    //_previewsList[i] = (GameObject)EditorGUILayout.ObjectField("Object: " + (i + 1), _previewsList[i], typeof(GameObject), true);
        //    //_preview = AssetPreview.GetAssetPreview(_previewsList[i]);

        //    //scriptableObj.paths.Add(_previewsList[i]);


        //    //if (_previewsList.Count < _groupFloat)
        //    //if (scriptableObj.maxAmount != _groupFloat)
        //    //{
        //    //    scriptableObj.paths.Clear();
        //    //    _previewsList.Add(temp);
        //    //    _previewsList[i] = (GameObject)EditorGUILayout.ObjectField("Object: " + (i + 1), _previewsList[i], typeof(GameObject), true);
        //    //    _preview = AssetPreview.GetAssetPreview(_previewsList[i]);
        //    //    scriptableObj.paths.Add(_previewsList[i]);
        //    //    Debug.Log(_previewsList.Count);
        //    //}

        //    //if (_groupFloat < _previewsList.Count)
        //    //    _previewsList.RemoveAt(_previewsList.Count);

        //}
        //scriptableObj.maxAmount = _groupFloat;

        //if (Event.current.type == EventType.Layout /*Event.current.Equals(Event.KeyboardEvent("return")*/)
        //{
        //    for (int i = 0; i < _groupFloat; i++)
        //    {
        //        //if (_previewsList.Count <= i || _previewsList[i] != null)
        //        //{

        //        //}
        //        //creo un objeto null para que la lista por default tenga algo
        //        GameObject temp = null;
        //        if (_previewsList.Count < _groupFloat)
        //            _previewsList.Add(temp);
        //        //_previewsList[i] = temp;
        //        _previewsList[i] = (GameObject)EditorGUILayout.ObjectField("Object: " + (i + 1), _previewsList[i], typeof(GameObject), true);
        //        _preview = AssetPreview.GetAssetPreview(_previewsList[i]);
        //        scriptableObj.paths.Add(_previewsList[i]);
        //        Debug.Log(_previewsList.Count);
        //        //if (_groupFloat < _previewsList.Count)
        //        //    _previewsList.RemoveAt(_previewsList.Count);
        //    }
        //    //Debug.Log(paths.paths.Count + "cantidad de items guardados");
        //}
        #endregion
    }
}
