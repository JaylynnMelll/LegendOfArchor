using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int Gold {  get; private set; }
    public int Exp { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"��� + {amount}, ���� : {Gold}");
    }

    public void AddExp(int amount)
    {
        Exp += amount;
        Debug.Log($"����ġ + {amount}, ���� : {Exp}");
    }
}
