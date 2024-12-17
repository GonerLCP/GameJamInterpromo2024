using NUnit.Framework;
using UnityEditor.TerrainTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CadreScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("enter");
    }
}
