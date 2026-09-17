using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject comojugarpopupWindow;
    public GameObject creditsPopupWindow;

    void Start()
    {
        comojugarpopupWindow.SetActive(false);
        creditsPopupWindow.SetActive(false);
    }
    public void PlayScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(); 
        #endif
    }

    public void OpenComoJugarPopup()
    {
        if (comojugarpopupWindow != null)
        {
            comojugarpopupWindow.SetActive(true);
        }
    }

    public void CloseComoJugarPopup()
    {
        if (comojugarpopupWindow != null)
        {
            comojugarpopupWindow.SetActive(false);
        }
    }

    public void OpenCreditsPopup()
    {
        if (creditsPopupWindow != null)
        {
            creditsPopupWindow.SetActive(true);
        }
    }

    public void CloseCreditsPopup()
    {
        if (creditsPopupWindow != null)
        {
            creditsPopupWindow.SetActive(false);
        }
    }
}
