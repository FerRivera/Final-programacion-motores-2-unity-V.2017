using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seed : MonoBehaviour
{
    public int selectedIndex;
    public List<Object> mapItems;
    public List<Object> vesselsToInstantiate;
    public bool mapLoaded;
    public string mapNameLoaded;
    public MapsSaved currentMap;
    public int mapLoadedIndex;
}
