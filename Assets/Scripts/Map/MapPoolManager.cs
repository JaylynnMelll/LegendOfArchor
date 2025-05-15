using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 방 타입을 일반, 보스로 나눈다
public enum RoomType { Normal, Boss }

public class MapPoolManager : MonoBehaviour
{
    [Header("프리팹 목록")]
    [SerializeField] private GameObject[] normalRooms;
    [SerializeField] private GameObject[] bossRooms;

    // 각 프리팹 기준으로 재사용 큐를 생성
    private Dictionary<GameObject, Queue<GameObject>> roomPools = new ();

    // 방 꺼내기
    public GameObject GetRoom(RoomType type)
    {
        // 타입에 따라 소스 배열 선택
        GameObject[] sourceArray = (type == RoomType.Normal) ? normalRooms : bossRooms;

        // 무작위 프리팹 선택
        GameObject prefab = sourceArray[Random.Range(0, sourceArray.Length)];

        // 해당 프리팹 큐가 없다면 생성
        if(!roomPools.ContainsKey(prefab))
            roomPools[prefab] = new Queue<GameObject>();

        Queue<GameObject> pool = roomPools[prefab];

        // 큐에 재사용 가능한 방이 있으면 꺼내고, 없다면 새로 생성
        if(pool.Count > 0)
        {
            GameObject room = pool.Dequeue();
            room.SetActive(true);
            return room;
        }

        else
        {
            return Instantiate(prefab);
        }
    }

    // 방 반환
    public void ReturnRoom(GameObject room)
    {
        room.SetActive(false);
        room.transform.SetParent(transform);

        //// 내부 EnemyController는 제거(풀링x)
        //foreach(Transform child in room.transform)
        //{
        //    if (child.TryGetComponent<EnemyController>(out var enemy))
        //        Destroy(child.gameObject);
        //}

        // 반환할 프리팹 큐에 넣음
        GameObject prefab = FindMatchingPrefab(room);
        if(prefab != null)
        {
            roomPools[prefab].Enqueue(room);
        }
    }

    // 인스턴스 이름으로 프리팹 찾기 (이름 포함 여부로 찾기)
    private GameObject FindMatchingPrefab(GameObject roomInstance)
    {
        foreach(var prefab in normalRooms.Concat(bossRooms))
        {
            if (roomInstance.name.Contains(prefab.name))
                return prefab;
        }
        return null;
    }
}
