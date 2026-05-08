using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public string stat = "";
    public TextMeshPro text;

    private void Start()
    {
        text.text = stat;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(UnityEngine.Camera.main.transform);
    }
}
