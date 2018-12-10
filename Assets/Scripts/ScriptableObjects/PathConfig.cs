using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConfig : ScriptableObject 
{
    [HideInInspector]
    public List<GameObject> paths = new List<GameObject>();
    [HideInInspector]
    public List<Vector3> positions = new List<Vector3>();
    [HideInInspector]
    public List<int> objectType = new List<int>();
    [HideInInspector]
    public List<Quaternion> rotations = new List<Quaternion>();
    [HideInInspector]
    public List<Object> objectsToInstantiate = new List<Object>();
    [HideInInspector]
    public int angleToRotate;
    [HideInInspector]
    public int totalPolygons;
    [HideInInspector]
    public float maxVesselDistance;
    [HideInInspector]
    public List<GameObject> vessels = new List<GameObject>();
    [HideInInspector]
    public List<Vector3> vesselsPositions = new List<Vector3>();
    [HideInInspector]
    public List<int> vesselsType = new List<int>();
    [HideInInspector]
    public List<float> vesselsDistance = new List<float>();
    [HideInInspector]
    public List<Object> vesselsToInstantiate = new List<Object>();
}
