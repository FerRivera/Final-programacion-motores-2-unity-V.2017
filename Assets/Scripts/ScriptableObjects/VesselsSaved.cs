using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VesselsSaved : ScriptableObject
{
    //[HideInInspector]
    public float distance;
    //[HideInInspector]
    public int selectedIndex;
    //[HideInInspector]
    public LayerMask vessels;
    //[HideInInspector]
    public LayerMask map;
    //[HideInInspector]
    public bool showHelpBox;
    //[HideInInspector]
    public bool showVesselsLimits;
    //[HideInInspector]
    public Color gizmoInstantiatedVessel;
    //[HideInInspector]
    public Color gizmoPredictionVessel;
    //[HideInInspector]
    public Color editModeEnabled;
    //[HideInInspector]
    public Color insideVesselLimit;
}
