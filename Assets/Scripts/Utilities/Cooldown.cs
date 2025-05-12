using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cooldown
{
    [SerializeField] private float cooldownTime;
    private float _nextActivationTime;

    public bool IsCoolingDown => Time.time < _nextActivationTime;
    public float StartCoolingDown() => _nextActivationTime = Time.time + cooldownTime;
}
