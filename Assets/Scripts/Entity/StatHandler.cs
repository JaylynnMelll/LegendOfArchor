using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Range(1, 100)][SerializeField] private int health = 10;
    public int Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, 100);
    }

    [Range(1f, 20f)][SerializeField] private float speed = 3f;
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0f, 20f);
    }

    [Range(1, 100)][SerializeField] private int rewardGold = 10;
    public int RewardGold
    {
        get => rewardGold;
        set => rewardGold = Mathf.Clamp(value, 0, 100);
    }

    [Range(1, 100)][SerializeField] private int rewardExp = 5;
    public int RewardExp
    {
        get => rewardExp;
        set => rewardExp = Mathf.Clamp(value, 0, 100);
    }
}
