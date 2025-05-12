using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarPool : MonoBehaviour
{

    [SerializeField] private GameObject[] hpBarPrefab;
    // [SerializeField] private GameObject hpBarPrefab; // 체력바 프리팹
    [SerializeField] private Transform hpBarRoot; // 체력바 붙일 위치

    // 프리팹별로 관리하는 오브젝트 풀 딕셔너리
    private Dictionary<GameObject, Queue<GameObject>> hpBarPools = new();

    // 체력바 요청 시 풀에서 꺼내거나, 새로 생성
    public GameObject GetHPBar(GameObject prefab)
    {
        // 해당 프리팹에 대한 풀 큐강 없으면 새로 생성함
        if (!hpBarPools.ContainsKey(prefab))
            hpBarPools[prefab] = new Queue<GameObject>();

        // 해당 프리팹 큐 가져오기
        var pool = hpBarPools[prefab];

        GameObject hpBar;

        if (pool.Count > 0) // 재사용 가능한 체력바가 있을 경우
        {
            hpBar = pool.Dequeue();
        }
        else // 없으면 새로 생성
        {
            hpBar = Instantiate(prefab, hpBarRoot);
        }

        hpBar.SetActive(true); // 체력바 활성화
        return hpBar;
    }

    // 체력바 반환 시 호출, 다시 풀에 저장
    public void ReturnHPBar(GameObject hpBar)
    {
        // 비활성화 처리
        hpBar.SetActive(false);
        // 어떤 프리팹에서 생성된지 추적
        GameObject prefab = FindMatchingPrefab(hpBar);
        if (prefab != null)
        {
            hpBarPools[prefab].Enqueue(hpBar);
            // 초기화 작업 수행 (이벤트 해제, 텍스트 초기화)
            var hpBarUI = hpBar.GetComponent<HPBarUI>();
            if (hpBarUI != null) hpBarUI.ResetHPBar();
        }
    }
    // 프리팹을 찾아 반환
    private GameObject FindMatchingPrefab(GameObject instance)
    {
        foreach (var prefab in hpBarPrefab)
        {
            // 이름에 프리팹 이름이 포함되어 있는지 확인
            if (instance.name.Contains(prefab.name))
                return prefab;
        }
        return null;
    }
}
