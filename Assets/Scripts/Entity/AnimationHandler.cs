using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsDamaged = Animator.StringToHash("IsDamaged");

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Move(Vector2 direction)
    {
        animator.SetBool(IsMoving, direction.magnitude > 0.5f);
    }

    public void Damage()
    {
        animator.SetBool(IsDamaged, true);
    }

    public void InvinvibilityEnd()
    {
        animator.SetBool(IsDamaged, false);
    }
}
