using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vessel : MonoBehaviour
{
    public int lastIndex;
    public int currentIndex;
    public int selectedIndex;
    public int id;
    public float distanceBetweenVessels;

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    public void Delete(PathConfig _pathsSaved)
    {
        var temp = _pathsSaved.vessels[id];
        var tempID = temp.GetComponent<Vessel>().id;

        _pathsSaved.vessels.RemoveAt(tempID);
        _pathsSaved.vesselsType.RemoveAt(tempID);
        _pathsSaved.vesselsPositions.RemoveAt(tempID);
        _pathsSaved.vesselsDistance.RemoveAt(tempID);

        for (int i = _pathsSaved.vessels.Count - 1; i >= id; i--)
        {
            _pathsSaved.vessels[i].GetComponent<Vessel>().id--;
        }

        DestroyImmediate(temp);
    }
}
