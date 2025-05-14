using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSplitEffectScaler : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystemToScale;

    [Tooltip("각 분열 단계마다 이만큼 크기 감소")]
    [SerializeField] private float scaleStep = 0.2f;

    [Tooltip("최소 크기 배율")]
    [SerializeField] private float minScale = 0.3f;

    private void Awake()
    {
        if (particleSystemToScale == null)
            particleSystemToScale = GetComponent<ParticleSystem>();
    }

    public void ApplyScaleBySplitCount(int splitCount)
    {
        float scale = Mathf.Max(1f - splitCount * scaleStep, minScale);

        // 전체 파티클 크기 조절
        transform.localScale = Vector3.one * scale;

        // Particle Start Size도 조절
        var main = particleSystemToScale.main;
        main.startSizeMultiplier = scale;
    }
}
