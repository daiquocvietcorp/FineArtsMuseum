using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UI;
using UnityEngine;

public class UIGuide : UIBasic
{
    public List<UIGuideChild> listGuide;
    
    public void ShowGuide(int index)
    {
        
        Debug.Log(index);
        // if (index < 0 || index > listGuide.Count) return;
        
        for (int i = 0; i < listGuide.Count; i++)
        {
            listGuide[i].gameObject.SetActive(i == index);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < listGuide.Count; i++)
        {
            listGuide[i].RegisterUIGuide(this);
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
        }
    }

    public override void ActionUI(Action action = null)
    {
        base.ActionUI(action);
        ShowGuide(0);
    }
}

public class UIGuideData : IUIData
{
    public Action OnBackButtonClicked;
}
