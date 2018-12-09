using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConfig : ScriptableObject 
{
    public List<GameObject> paths = new List<GameObject>();
    public List<Vector3> positions = new List<Vector3>();
    public List<int> objectType = new List<int>();
    public List<Quaternion> rotations = new List<Quaternion>();
    public List<Object> objectsToInstantiate = new List<Object>();
    public int angleToRotate;
    public int totalPolygons;
    public List<GameObject> vessels = new List<GameObject>();
    public List<Vector3> vesselsPositions = new List<Vector3>();
    public List<int> vesselsType = new List<int>();
    public List<Object> vesselsToInstantiate = new List<Object>();
}
