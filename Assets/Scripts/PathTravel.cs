using System.Collections.Generic;
using UnityEngine;


public class PathTravel : MonoBehaviour
{
    private TileMapper tileMapper;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the tilemapper object
       if (!GameObject.Find("TileWrapper").TryGetComponent<TileMapper>(out tileMapper))
        {
            Debug.LogError("Could not find TileMapper component on TileWrapper object");
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
