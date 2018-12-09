using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselsSaved : ScriptableObject
{
    public float distance;
    public int selectedIndex;
    public LayerMask vessels;
    public LayerMask map;
}
