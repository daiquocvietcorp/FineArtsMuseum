using System;
using System.Collections;
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

    public Image timerFillImage; // Thêm image để hiển thị tiến trình đếm ngược

    private UIGuide _parent;
    
    public int timerDuration = 10;

    private Coroutine _autoNextCoroutine;
    
    private Action _closeAction;

    public void Initialize()
    {
        CloseButton.onClick.AddListener(CloseButtonClicked);
        PreviousButton.onClick.AddListener(() => PreviousButtonClicked(currentIndex - 1));
        NextButton.onClick.AddListener(() => NextButtonClicked(currentIndex + 1));

        guideImage.sprite = guideSprite;

        indexText.text = (currentIndex + 1).ToString() + "<color=#808080>/" + _parent.listGuide.Count + "</color>";

        PreviousButton.interactable = currentIndex > 0;
        NextButton.interactable = currentIndex < _parent.listGuide.Count - 1;

        if (_autoNextCoroutine != null)
            StopCoroutine(_autoNextCoroutine);
        _autoNextCoroutine = StartCoroutine(AutoNextTimer(timerDuration));
    }

    IEnumerator AutoNextTimer(int seconds)
    {
        float elapsed = 0f;
        timerFillImage.fillAmount = 0f;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            timerFillImage.fillAmount = elapsed / seconds;
            yield return null;
        }

        if (NextButton.interactable)
        {
            NextButtonClicked(currentIndex + 1);
        }
        else
        {
            CloseButtonClicked();
            _closeAction?.Invoke();
        }
    }

    public void CloseButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void RegisterClickClose(Action onClick)
    {
        _closeAction = onClick;
        CloseButton.onClick.AddListener(() => onClick?.Invoke());
    }

    public void PreviousButtonClicked(int index)
    {
        _parent.ShowGuide(index);
    }

    public void NextButtonClicked(int index)
    {
        _parent.ShowGuide(index);
    }

    public void RegisterUIGuide(UIGuide guide)
    {
        _parent = guide;
    }
}