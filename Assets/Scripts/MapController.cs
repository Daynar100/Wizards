using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject genericCell;
    private GameObject[,] terrainGrid = new GameObject[17,17];
    private static MapController map;
    void Awake() {
        map = this;
        for(int y = 0; y < 17; ++y)
        for(int x = 0; x < 17; ++x) {
            terrainGrid[x,y] = Instantiate(genericCell,new Vector3((float)x - 8,(float)-y,0),Quaternion.identity);
        }
    }
    public static void Dig(int x, int y) {
        if (!map) return;
        if (y <= 1)
        if (map.terrainGrid[x + 8,-y + 1] != null)
            Destroy(map.terrainGrid[x + 8,-y + 1]);
    }
}
