using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using Player;
using UnityEngine;

public class TeleportTriggerPoint : MonoBehaviour
{
    [SerializeField] private int floor = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CharacterManager.Instance.SetFloorForCharacter(floor);
            CameraManager.Instance.SetCameraRotationByFloor(floor);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
