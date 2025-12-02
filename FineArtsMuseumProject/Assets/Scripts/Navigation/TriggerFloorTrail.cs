using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFloorTrail : MonoBehaviour
{
    public int floorNumber;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("TriggerFloorTrail: " + floorNumber);
            TrailEffectManager.Instance.StartTrailsByFloor(floorNumber);
            //TrailEffectManager.Instance.StartTrails();
        }
    }
}
