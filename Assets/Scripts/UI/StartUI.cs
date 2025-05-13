using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    // [SerializeField] private Button startButton;
    // [SerializeField] private Button exitButton;
    // 
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("2DTopDownShooting");

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