using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsSaved : ScriptableObject
{
    //[HideInInspector]
    public List<GameObject> paths = new List<GameObject>();
    //[HideInInspector]
    public List<Vector3> positions = new List<Vector3>();
    //[HideInInspector]
    public List<int> objectType = new List<int>();
    //[HideInInspector]
    public List<Quaternion> rotations = new List<Quaternion>();
    //[HideInInspector]
    public List<GameObject> vessels = new List<GameObject>();
    //[HideInInspector]
    public List<Vector3> vesselsPositions = new List<Vector3>();
    //[HideInInspector]
    public List<int> VesselsType = new List<int>();
    //[HideInInspector]
    public int totalPolygons;
    //[HideInInspector]
    public List<float> vesselsDistance = new List<float>();
}
