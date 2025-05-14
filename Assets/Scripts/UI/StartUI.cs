using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject selectWeapon;
    public void OnClickStartButton()
    {
        selectWeapon.SetActive(true);

    }

    public void OnClickCancelPanelButton()
    {
        selectWeapon.SetActive(false);
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