using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFloorManager : MonoBehaviour
{
    public Button TrailF1Button;
    public Button TrailF2Button;
    public Button TrailF3Button;
    public CanvasGroup ButtonF1CG;
    public CanvasGroup ButtonF2CG;
    public CanvasGroup ButtonF3CG;

    private int currentFloor = 1;
    private bool isOpenFloor = false;
    
    private void Start()
    {
        TrailF1Button.onClick.AddListener(() =>
        {
            ButtonFloorClicked(1);
        });
        
        TrailF2Button.onClick.AddListener(() =>
        {
            ButtonFloorClicked(2);
        });
        
        TrailF3Button.onClick.AddListener(() =>
        {
            ButtonFloorClicked(3);
        });
    }
    
    public void ButtonFloorClicked(int floor)
    {
            if (isOpenFloor)
            {

                if (floor == currentFloor)
                {
                    isOpenFloor = false;
                    TrailEffectManager.Instance.SetCurrentFloor(0);
                    TrailEffectManager.Instance.StopTrails();
                    currentFloor = 0;
                }
                else
                {
                    TrailEffectManager.Instance.StopTrails();
                    
                    TrailEffectManager.Instance.SetCurrentFloor(floor);
                    TrailEffectManager.Instance.StartTrails();
                    isOpenFloor = true;
                    currentFloor = floor;
                }
            }
            else
            {
                currentFloor = floor;
                isOpenFloor = true;
                TrailEffectManager.Instance.SetCurrentFloor(floor);
                TrailEffectManager.Instance.StartTrails();
            }
            SetCurrentFloorButton(floor,isOpenFloor);
    }

    public void SetCurrentFloorButton(int floor,bool isOpenTrail)
    {
        if (isOpenTrail)
        {
            if (floor == 1)
            {
                ButtonF1CG.alpha = 1;
                ButtonF2CG.alpha = .5f;
                ButtonF3CG.alpha = .5f;
            }

            if (floor == 2)
            {
                ButtonF1CG.alpha = .5f;
                ButtonF2CG.alpha = 1;
                ButtonF3CG.alpha = .5f;
            }

            if (floor == 3)
            {
                ButtonF1CG.alpha = .5f;
                ButtonF2CG.alpha = .5f;
                ButtonF3CG.alpha = 1f;
            }
        }
        else
        {
            ButtonF1CG.alpha = 1f;
            ButtonF2CG.alpha = 1f;
            ButtonF3CG.alpha = 1f;
        }
        
    }
}
