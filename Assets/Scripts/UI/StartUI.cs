using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject selectWeaponPanel;
    public void OnClickStartButton()
    {
        selectWeaponPanel.SetActive(true);

    }

    public void OnClickCancelPanelButton()
    {
        selectWeaponPanel.SetActive(false);
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