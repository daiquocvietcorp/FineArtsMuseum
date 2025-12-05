using System;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class TrailEffectManager : MonoBehaviour
{
    public static TrailEffectManager Instance { get; private set; }

    public List<MoveThroughPointsBySpeed> trailEffects;

    public Transform player;
    public GameObject trailF1;
    public GameObject trailF2;
    public GameObject trailF3;
    public GameObject trailHam;

    private int currentFloor = -1;
    public bool isTrailOn;
    
    private void Awake()
    {
        Instance = this;
        isTrailOn = false;
    }

    private void Start()
    {
        ResetTrails();
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
            {
                mover.ResetPath();
                mover.StopMoving();
            }
        }
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;   
    }

    // public void StartTrails()
    // {
    //     foreach (var mover in trailEffects)
    //     {
    //         if (mover != null)
    //         {
    //             mover.ResetPath();   // reset trước
    //             mover.StartMoving(); // chạy sau
    //         }
    //     }
    // }
    public void StartTrails()
    {
        StartTrailsByFloor(currentFloor);
    }
    
    public void StartTrailsByFloor(int floor)
    {
        if (floor == 0 && trailHam !=null)
        {
            trailF1.SetActive(false);
            trailF2.SetActive(false);
            trailF3.SetActive(false);
            trailHam.SetActive(true);
        }
        
        if (floor == 1)
        {
            trailF1.SetActive(true);
            trailF2.SetActive(false);
            trailF3.SetActive(false);
            if(trailHam !=null)
            trailHam.SetActive(false);

        }
        else if(floor == 2)
        {
            trailF1.SetActive(false);
            trailF2.SetActive(true);
            trailF3.SetActive(false);
            if(trailHam !=null)
            trailHam.SetActive(false);

        }
        else if(floor == 3)
        {
            trailF1.SetActive(false);
            trailF2.SetActive(false);
            trailF3.SetActive(true);
            if(trailHam !=null)
            trailHam.SetActive(false);
        }
        else
        {
            trailF1.SetActive(true);
            trailF2.SetActive(true);
            trailF3.SetActive(true);
            if(trailHam != null)
                trailHam.SetActive(true);
        }
        
        
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