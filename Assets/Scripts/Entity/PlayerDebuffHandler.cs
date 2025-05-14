using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffHandler : MonoBehaviour
{
    private bool isSlowed = false;

    public void ApplySlow(StatHandler statHandler, float multiplier, float duration)
    {
        if (isSlowed) return;

        StartCoroutine(SlowCoroutine(statHandler, multiplier, duration));
    }

    private IEnumerator SlowCoroutine(StatHandler statHandler, float multiplier, float duration)
    {
        isSlowed = true;

        float originalSpeed = statHandler.Speed;
        statHandler.Speed = originalSpeed * multiplier;

        // �ð��� ����Ʈ�� UI ���� �߰� ����

        yield return new WaitForSeconds(duration);

        statHandler.Speed = originalSpeed;
        isSlowed = false;
    }
}
