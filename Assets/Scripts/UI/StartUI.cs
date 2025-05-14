using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject selectWeaponUI;
    public void OnClickStartButton()
    {
        selectWeaponUI.SetActive(true);

    }

    public void OnClickCancelPanelButton()
    {
        selectWeaponUI.SetActive(false);
    }

    public void OnClickExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}