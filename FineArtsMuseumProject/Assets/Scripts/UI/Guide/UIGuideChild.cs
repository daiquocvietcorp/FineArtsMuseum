using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGuideChild : MonoBehaviour
{
    public Image guideImage;
    public Sprite guideSprite;
    
    public Button CloseButton;
    public Button PreviousButton;
    public Button NextButton;

    public TextMeshProUGUI indexText;
    
    public int currentIndex;

    private UIGuide _parent;
    
    // Start is called before the first frame update
    public void Initialize()
    {
        
        CloseButton.onClick.AddListener(CloseButtonClicked);
        PreviousButton.onClick.AddListener(()=>PreviousButtonClicked(currentIndex-1));
        NextButton.onClick.AddListener(()=>NextButtonClicked(currentIndex+1));

        guideImage.sprite = guideSprite;
        
        indexText.text = (currentIndex + 1).ToString() + "<color=#808080>/" + _parent.listGuide.Count+"</color>";
        
        if(currentIndex <= 0 ) PreviousButton.interactable = false;
        if(currentIndex >= _parent.listGuide.Count - 1) NextButton.interactable = false;
    }

    public void CloseButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void RegisterClickClose(Action onClick)
    {
        CloseButton.onClick.AddListener(() => onClick?.Invoke());
    }
    
    public void PreviousButtonClicked(int currentIndex)
    {
        Debug.Log("PreviousButtonClicked:"+currentIndex);
        _parent.ShowGuide(currentIndex);
    }
    
    public void NextButtonClicked(int currentIndex)
    {
        Debug.Log("NextButtonClicked:"+currentIndex);
        _parent.ShowGuide(currentIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterUIGuide(UIGuide guide)
    {
        _parent = guide;
    }
}
