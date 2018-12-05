using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsSaved : ScriptableObject
{
    public List<GameObject> paths = new List<GameObject>();
    public List<Vector3> positions = new List<Vector3>();
    public List<int> objectType = new List<int>();
    public List<Quaternion> rotations = new List<Quaternion>();
    public List<GameObject> vessels = new List<GameObject>();
    public List<Vector3> vesselsPositions = new List<Vector3>();
    public List<int> VesselsType = new List<int>();
    public int totalPolygons;
}
