using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class TrailEffectManager : MonoBehaviour
{
    public static TrailEffectManager Instance { get; private set; }

    public List<MoveThroughPointsBySpeed> trailEffects;

    public Transform player;
    public GameObject trailF1;
    public GameObject trailF2;
    public GameObject trailF3;

    private int currentFloor = 0;
    public bool isTrailOn;
    
    private void Awake()
    {
        Instance = this;
        isTrailOn = false;
    }

    private void Start()
    {
        StopTrails();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        //     ResetTrails();
        //
        // if (Input.GetKeyDown(KeyCode.S))
        //     StopTrails();
        //
        // if (Input.GetKeyDown(KeyCode.Space))
        //     StartTrails();
    }

    public void ResetTrails()
    {
        if (player.position.y < 4)
        {
            trailF1.SetActive(true);
            trailF2.SetActive(false);
            trailF3.SetActive(false);
        }
        else if(player.position.y >= 4 && player.position.y < 8)
        {
            trailF1.SetActive(false);
            trailF2.SetActive(true);
            trailF3.SetActive(false);
        }
        else
        {
            trailF1.SetActive(false);
            trailF2.SetActive(false);
            trailF3.SetActive(true);
        }
        
        foreach (var mover in trailEffects)
        {
            if (mover != null)
                mover.ResetPath();
        }
    }

    public void StopTrails()
    {
        foreach (var mover in trailEffects)
        {
            if (mover != null)
                mover.StopMoving();
        }
    }

    public void StartTrails()
    {
        // if (player.position.y < 4)
        // {
        //     trailF1.SetActive(true);
        //     trailF2.SetActive(false);
        //     trailF3.SetActive(false);
        //     currentFloor = 0;
        // }
        // else if(player.position.y >= 4 && player.position.y < 8)
        // {
        //     trailF1.SetActive(false);
        //     trailF2.SetActive(true);
        //     trailF3.SetActive(false);
        //     currentFloor = 1;
        // }
        // else
        // {
        //     trailF1.SetActive(false);
        //     trailF2.SetActive(false);
        //     trailF3.SetActive(true);
        //     currentFloor = 2;
        // }
        
        foreach (var mover in trailEffects)
        {
            if (mover != null)
            {
                mover.ResetPath();   // reset trước
                mover.StartMoving(); // chạy sau
            }
        }
    }
    
    public void StartTrailsByFloor(int floor)
    {
        if(!isTrailOn) return;
        if (player.position.y < 4)
        {
            trailF1.SetActive(true);
            trailF2.SetActive(false);
            trailF3.SetActive(false);
            currentFloor = 0;
        }
        else if(player.position.y >= 4 && player.position.y < 8)
        {
            trailF1.SetActive(false);
            trailF2.SetActive(true);
            trailF3.SetActive(false);
            currentFloor = 1;
        }
        else
        {
            trailF1.SetActive(false);
            trailF2.SetActive(false);
            trailF3.SetActive(true);
            currentFloor = 2;
        }
        
        //StopTrails();
        
        UnityEngine.Debug.Log("isTrailOn:"+isTrailOn);

        if(floor == currentFloor)
            return;
        UnityEngine.Debug.Log("isTrailOn:"+isTrailOn);
        
        
        
        foreach (var mover in trailEffects)
        {
            if (mover != null)
            {
                mover.ResetPath();   // reset trước
                mover.StartMoving(); // chạy sau
            }
        }
    }
}