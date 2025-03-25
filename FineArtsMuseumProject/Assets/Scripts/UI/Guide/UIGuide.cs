using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class UIGuide : MonoSingleton<UIGuide>
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
        ShowGuide(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
