using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneButtonsConfig : ScriptableObject
{
    public Rect saveNewMapRect;
    public Rect newMapRect;
    public Rect overwriteMapRect;
    public Rect deleteLastPathRect;
    public Rect deleteLastVesselRect;
    public Rect bringSeedRect;
    public Rect deletePathRect;
    public Rect deleteCloserVesselsRect;
    public Rect deleteVesselRect;
    public bool seedButton;
    public bool pathButton;
    public bool vesselButton;
}
