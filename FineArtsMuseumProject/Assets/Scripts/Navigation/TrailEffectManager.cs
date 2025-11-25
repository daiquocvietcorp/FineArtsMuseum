using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffectManager : MonoBehaviour
{
    
    public static TrailEffectManager Instance { get; set; }

    public List<MoveThroughPointsBySpeed> trailEffects;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (MoveThroughPointsBySpeed mover in trailEffects)
            {
                mover.ResetPath();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (MoveThroughPointsBySpeed mover in trailEffects)
            {
                mover.StopMoving();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (MoveThroughPointsBySpeed mover in trailEffects)
            {
                mover.StartMoving();
                mover.ResetPath();
            }
        }
    }
}
