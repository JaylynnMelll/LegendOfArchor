using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public int MaxHP { get; private set; } = 100;
    public int CurrentHP { get; private set; }

    public int Gold { get; private set; }
    public int Exp { get; private set; }
    public int MaxExp { get; private set; } = 30;
    public int Level { get; private set; } = 1;

    private void Awake()
    {
        Instance = this;
        CurrentHP = MaxHP;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        GameManager.instance.UpdateGold();
    }

    public void AddExp(int amount)
    {
        Exp += amount;
        GameManager.instance.UpdateExp();
        CheckLevelUp();
    }
    private void CheckLevelUp()
    {
        if (Exp >= MaxExp)
        {
            Exp -= MaxExp;
            Level++;
            MaxExp = Mathf.RoundToInt(MaxExp * 1.25f);
            GameManager.instance.UpdateExp();
        }
    }
}
