using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneButtonsConfig : ScriptableObject
{
    [HideInInspector]
    public Rect saveNewMapRect;
    [HideInInspector]
    public Rect newMapRect;
    [HideInInspector]
    public Rect overwriteMapRect;
    [HideInInspector]
    public Rect deleteLastPathRect;
    [HideInInspector]
    public Rect deleteLastVesselRect;
    [HideInInspector]
    public Rect bringSeedRect;
    [HideInInspector]
    public Rect deletePathRect;
    [HideInInspector]
    public Rect deleteCloserVesselsRect;
    [HideInInspector]
    public Rect deleteVesselRect;
    [HideInInspector]
    public bool seedButton;
    [HideInInspector]
    public bool pathButton;
    [HideInInspector]
    public bool vesselButton;
}
