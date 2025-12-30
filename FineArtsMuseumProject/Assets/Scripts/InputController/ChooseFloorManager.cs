using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public Image capsuleButton1;
    public Image capsuleButton2;
    public Image capsuleButton3;
    
    public Color highLightColor;
    
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;

    private int currentFloor = 1;
    private bool isOpenFloor = false;
    
    private void Start()
    {
        text1.text = "Đo đạc tầng 1";
        text2.text = "Đo đạc tầng 2";
        text3.text = "Đo đạc tầng 3";
        
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
        
        capsuleButton1.color = Color.white;
        capsuleButton2.color = Color.white;
        capsuleButton3.color = Color.white;
        
        text1.text = "Đo đạc tầng 1";
        text2.text = "Đo đạc tầng 2";
        text3.text = "Đo đạc tầng 3";
        
        text1.color = Color.black;
        text2.color = Color.black;
        text3.color = Color.black;
       
        if (isOpenTrail)
        {
            if (floor == 1)
            {
                ButtonF1CG.alpha = 1;
                ButtonF2CG.alpha = .5f;
                ButtonF3CG.alpha = .5f;
                
                capsuleButton1.color = highLightColor;
                text1.text = "Tắt đo đạc tầng 1";
                text1.color = Color.white;
            }

            if (floor == 2)
            {
                ButtonF1CG.alpha = .5f;
                ButtonF2CG.alpha = 1;
                ButtonF3CG.alpha = .5f;
                
                capsuleButton2.color = highLightColor;
                text2.text = "Tắt đo đạc tầng 2";
                text2.color = Color.white;
            }

            if (floor == 3)
            {
                ButtonF1CG.alpha = .5f;
                ButtonF2CG.alpha = .5f;
                ButtonF3CG.alpha = 1f;
                
                capsuleButton3.color = highLightColor;
                text3.text = "Tắt đo đạc tầng 3";
                text3.color = Color.white;
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
