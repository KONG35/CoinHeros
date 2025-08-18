using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Image Bar;

    void Start()
    {
        Bar.fillAmount = 0f;
        SceneManager.Instance.StartLoading();
    }

    public void Set(float progress)
    {
        Bar.fillAmount = progress;
        
    }   
}

