using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }

    public int Gold { get; private set; }
    public int Exp { get; private set; }

    public int MaxExp { get; private set; } = 10;
    public int Level { get; private set; } = 1;

    private void Awake()
    {
        Instance = this;
        LoadGold();
    }

    private void Start()
    {
        GameManager.Instance.UpdateGold(Gold);
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        SetGold();
        GameManager.Instance.UpdateGold(Gold);
    }

    public void SetGold()
    {
        SaveGold();
        LoadGold();
    }

    public void SaveGold()
    {
        PlayerPrefs.SetInt("Gold", Gold);
    }

    public void LoadGold()
    {
        Gold = PlayerPrefs.GetInt("Gold", 0);
    }

    public void AddExp(int amount)
    {
        Exp += amount;
        GameManager.Instance.UpdateExp();
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (Exp >= MaxExp)
        {
            Exp -= MaxExp;
            Level++;
            MaxExp = Mathf.RoundToInt(MaxExp * 1.25f);
            GameManager.Instance.UpdateExp();
            GameManager.Instance.LevelUp(Level);
        }
    }
}
