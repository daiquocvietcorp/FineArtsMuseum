using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [SerializeField]  private List<Material> materials;

    private void Awake()
    {
        foreach(var material in materials)
        {
            //warm up
            material.SetPass(0);
        }
    }
}
