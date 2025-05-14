using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 무기 선택 UI 제어
public class SelectWeaponUI : MonoBehaviour
{
    [SerializeField] private GameObject selectWeapon; // 무기 선택 UI 패널
    [SerializeField] private Transform weaponsRoot; // 무기 버튼들이 들어갈 부모 오브젝트 (Weapons 오브젝트)
    [SerializeField] private GameObject weaponItemPrefab; // 무기아이콘, 텍스트 보여주는 버튼 프리팹

    [SerializeField] private Image selectedWeaponIcon; // 선택한 무기 아이콘
    [SerializeField] private GameObject[] powerIcons; // 선택된 무기의 파워 표기
    [SerializeField] private List<GameObject> weaponPrefabs; // 무기 프리팹들

    [SerializeField] private TextMeshProUGUI warningText; // 경고 텍스트

    private Coroutine warningCoroutine; // 코루틴 저장 변수

    private int selectedWeaponIndex = -1; // 선택되지 않은 상태로 나타나기 위해서 초기값 -1

    private void Start()
    {
        // 시작할 때 무기파워 표기 초기화
        foreach (var star in powerIcons)
            star.SetActive(false);
        for (int i = 0; i < weaponPrefabs.Count; i++)
        {
            // 무기 프리팹 복제해서 Weapons에 넣음
            GameObject item = Instantiate(weaponItemPrefab, weaponsRoot);
            // 무기 프리팹에서 아이콘 스프라이트 가져옴
            GameObject prefab = weaponPrefabs[i];
            Sprite icon = prefab.GetComponentInChildren<SpriteRenderer>().sprite;

            // WeaponItemUI에 무기 프리팹, 클릭함수 전달, 초기화
            WeaponItemUI itemUI = item.GetComponent<WeaponItemUI>();
            itemUI.Init(icon, i, OnClickWeaponItem);
        }
    }

    // 무기 버튼 클릭시 호출되는 함수
    private void OnClickWeaponItem(int index)
    {
        selectedWeaponIndex = index;

        // 선택된 무기 UI에 표시
        GameObject prefab = weaponPrefabs[index];
        selectedWeaponIcon.sprite = prefab.GetComponentInChildren<SpriteRenderer>().sprite;
        // 무기의 공격력을 아이콘으로 UI 표기
        int powerCount = GetWeaponPower(prefab);
        for (int i = 0; i < powerIcons.Length; i++)
        {
            powerIcons[i].SetActive(i < powerCount);
        }
    }
    private int GetWeaponPower(GameObject prefab)
    {
        // 무기 데이터 받아오기
        var melee = prefab.GetComponent<MeleeWeaponHandler>();
        if (melee == null) return 0;

        float power = melee.WeaponPower;
        // 공격력에 따른 아이콘 수 설정
        if (power >= 5f) return 5;
        if (power >= 4f) return 4;
        if (power >= 3f) return 3;
        if (power >= 2f) return 2;
        return 1;
    }

    // 선택 완료 버튼 클릭 시 호출되는 함수
    public void OnClickSelectButton()
    {
        if (selectedWeaponIndex < 0)
        {
            // 무기 선택 안하면 UI 표시 하고 함수종료
            ShowWarning("무기를 선택해주세요!");
            return;
        }

        // 선택 완료된 무기 프리팹 이름으로 PlayerPrefs에 저장
        string prefabName = weaponPrefabs[selectedWeaponIndex].name;
        PlayerPrefs.SetString("SelectedWeaponPrefabName", prefabName);
        // 저장된 이름 확인용
        string name = PlayerPrefs.GetString("SelectedWeaponPrefabName", prefabName);
        Debug.Log(name);
        SceneManager.LoadScene("2DTopDownShooting");
        Time.timeScale = 1;
    }

    private void ShowWarning(string message)
    {
        // 기존 경고 메시지 중복 방지
        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);
        // 경고 메시지 표시 코루틴 실행
        warningCoroutine = StartCoroutine(ShowWarningRoutine(message));
    }

    private IEnumerator ShowWarningRoutine(string message)
    {
        warningText.text = message; // 텍스트 표시
        warningText.gameObject.SetActive(true); // 텍스트 활성화

        yield return new WaitForSeconds(2f);
        warningText.gameObject.SetActive(false); // 텍스트 비활성화
        warningText.text = ""; // 텍스트 내용 초기화
    }

    // 취소 버튼 클릭 시 호출되는 함수
    public void OnClickCancelButton()
    {
        // 선택 초기화
        selectedWeaponIndex = -1;

        // 미리보기 초기화
        selectedWeaponIcon.sprite = null;

        foreach (var star in powerIcons)
            star.SetActive(false);
        // 경고 메시지 숨기기
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        warningText.gameObject.SetActive(false);
        warningText.text = "";

        // 선택창 닫기
        selectWeapon.SetActive(false);
    }
}
