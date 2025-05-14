using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSplitEffectScaler : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystemToScale;

    [Tooltip("�� �п� �ܰ踶�� �̸�ŭ ũ�� ����")]
    [SerializeField] private float scaleStep = 0.2f;

    [Tooltip("�ּ� ũ�� ����")]
    [SerializeField] private float minScale = 0.3f;

    private void Awake()
    {
        if (particleSystemToScale == null)
            particleSystemToScale = GetComponent<ParticleSystem>();
    }

    public void ApplyScaleBySplitCount(int splitCount)
    {
        float scale = Mathf.Max(1f - splitCount * scaleStep, minScale);

        // ��ü ��ƼŬ ũ�� ����
        transform.localScale = Vector3.one * scale;

        // Particle Start Size�� ����
        var main = particleSystemToScale.main;
        main.startSizeMultiplier = scale;
    }
}
