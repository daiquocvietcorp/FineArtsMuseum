using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using InputController;
using UI;
using UnityEngine;

public class UIGuide : UIBasic
{
    public List<UIGuideChild> listGuide;
    private Action _onBackButtonClicked;
    
    public void ShowGuide(int index)
    {
        for (int i = 0; i < listGuide.Count; i++)
        {
            listGuide[i].gameObject.SetActive(i == index);
            if (i == index)
            {
                listGuide[i].Initialize(); // Phải gọi lại Initialize() mỗi khi active
            }
        }
    }

    public override void DisableUI()
    {
        base.DisableUI();
        _onBackButtonClicked?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < listGuide.Count; i++)
        {
            listGuide[i].RegisterUIGuide(this);
            listGuide[i].Initialize();
        }
        ShowGuide(0);
    }

    public override void SetData(IUIData data)
    {
        base.SetData(data);
        
        if (data is UIGuideData guideData)
        {
            foreach(var guide in listGuide)
            {
                guide.RegisterClickClose(guideData.OnBackButtonClicked);
            }
            
            _onBackButtonClicked = guideData.OnBackButtonClicked;
        }
    }

    public override void ActionUI(Action action = null)
    {
        base.ActionUI(action);
        ShowGuide(0);
    }

    public override void EnableUI()
    {
        base.EnableUI();
        InputManager.Instance.DisableJoystick();
        InputManager.Instance.DisableJoystickRotation();
    }
    
    
}

public class UIGuideData : IUIData
{
    public Action OnBackButtonClicked;
}
