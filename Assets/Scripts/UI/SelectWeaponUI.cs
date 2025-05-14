using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 무기 선택 UI 제어
public class SelectWeaponUI : MonoBehaviour
{
    [SerializeField] private Transform weaponsRoot; // 무기 버튼들이 들어갈 부모 오브젝트 (Weapons 오브젝트)

    [SerializeField] private GameObject weaponItemPrefab; // 무기아이콘, 텍스트 보여주는 버튼 프리팹

    [SerializeField] private Image selectedWeaponIcon; // 선택한 무기 아이콘

    [SerializeField] private GameObject[] powerIcons; // 선택된 무기의 파워 표기

    [SerializeField] private List<GameObject> weaponPrefabs; // 무기 프리팹들

    [SerializeField] private TextMeshProUGUI warningText; // 경고 텍스트

    [SerializeField] private UnlockUI unlockUI; // 잠금해제 UI

    [SerializeField] private TextMeshProUGUI goldText;

    private const string GoldKey = "Gold"; // 오타, 실수방지용 변수

    private const string SelectedWeaponKey = "SelectedWeaponPrefabName"; // 오타, 실수방지용 변수

    private string GetUnlockKey(string weaponName) => $"Unlocked_{weaponName}"; // 오타, 실수방지용 변수

    private Coroutine warningCoroutine; // 코루틴 저장 변수

    private int selectedWeaponIndex = -1; // 선택되지 않은 상태로 나타나기 위해서 초기값 -1

    private int Gold;

    private void Awake()
    {
        FirstWeaponUnlocked(); // 기본 무기 지급
        InitGold(); // 초기 골드 지급 (기본값 200)
        LoadGold(); // 골드 불러오기
    }

    private void Start()
    {
        InitWeaponUI(); // 무기 UI 초기화
    }

    private void InitWeaponUI()
    {
        // 첫 실행시 파워 아이콘 비활성화
        foreach (var power in powerIcons)
            power.SetActive(false);
        for (int i = 0; i < weaponPrefabs.Count; i++)
        {
            GameObject prefab = weaponPrefabs[i];
            var weapon = GetWeaponHandler(prefab); // 무기 핸들러 컴포넌트 가져오기
            if (weapon == null) continue;

            Sprite icon = prefab.GetComponentInChildren<SpriteRenderer>().sprite; // 아이콘 이미지
            string weaponName = prefab.name;
            bool isUnlocked = IsUnlocked(weaponName); // 무기 해금 여부

            GameObject item = Instantiate(weaponItemPrefab, weaponsRoot); // UI 버튼 생성
            WeaponItemUI itemUI = item.GetComponent<WeaponItemUI>();
            itemUI.Init(icon, i, OnClickWeaponItem, isUnlocked); // UI 버튼 초기화
        }
    }

    // 기본 무기 항상 해금된 상태로 설정
    private void FirstWeaponUnlocked()
    {
        if (weaponPrefabs.Count == 0) return;
        if (PlayerPrefs.HasKey($"Unlocked_{weaponPrefabs[0].name}")) return;

        PlayerPrefs.SetInt($"Unlocked_{weaponPrefabs[0].name}", 1);
        PlayerPrefs.Save();
    }

    // 무기 버튼 클릭시 호출되는 함수
    private void OnClickWeaponItem(int index)
    {
        GameObject prefab = weaponPrefabs[index];
        string weaponName = prefab.name;

        var weapon = GetWeaponHandler(prefab);
        if (weapon == null) return;

        int price = weapon.Price;

        if (!IsUnlocked(weaponName))
        {
            unlockUI.Open(price, () =>
            {
                if (SpendGold(price)) // 골드 충분한지 확인
                {
                    Unlock(weaponName);
                    ShowWarning("무기 해금 완료!");
                    UpdateWeaponUI(); // UI 갱신
                }
                else
                {
                    ShowWarning("골드가 부족합니다!");
                }
            });
            return;
        }

        // 무기 선택 처리
        selectedWeaponIndex = index;
        selectedWeaponIcon.sprite = prefab.GetComponentInChildren<SpriteRenderer>().sprite;

        // 무기 파워에 따라서 아이콘 표시
        int powerCount = GetWeaponPower(prefab);
        for (int i = 0; i < powerIcons.Length; i++)
        {
            powerIcons[i].SetActive(i < powerCount);
        }
    }

    // UI 갱신
    private void UpdateWeaponUI()
    {
        foreach (Transform child in weaponsRoot)
        {
            Destroy(child.gameObject);
        }
        InitWeaponUI();
    }

    // PlayerPrefs에 골드가 저장되지 않은 경우에만 초기 골드 지급
    private void InitGold()
    {
        if (!PlayerPrefs.HasKey(GoldKey))
        {
            PlayerPrefs.SetInt(GoldKey, 200);
            PlayerPrefs.Save();
        }
    }

    // 지불할 골드 충분한지 확인 후 성공 여부 반환
    private bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            SetGold();
            return true;
        }
        return false;
    }

    // 골드 저장 및 갱신
    private void SetGold()
    {
        SaveGold();
        LoadGold();
    }

    // 현재 골드 저장
    public void SaveGold()
    {
        PlayerPrefs.SetInt(GoldKey, Gold);
    }

    // 저장된 골드 불러오기
    public void LoadGold()
    {
        Gold = PlayerPrefs.GetInt(GoldKey, 0);
        goldText.text = $"{Gold}"; // UI 텍스트에 골드 표시
    }

    // 해당 무기 해금되어 있는지 확인
    public bool IsUnlocked(string weaponName)
    {
        return PlayerPrefs.GetInt(GetUnlockKey(weaponName), 0) == 1;
    }

    // 해당 무기를 해금 상태로 설정
    public void Unlock(string weaponName)
    {
        PlayerPrefs.SetInt(GetUnlockKey(weaponName), 1);
    }

    // 무기 프리팹에서 WeaponHandler 컴포넌트를 가져옴
    private WeaponHandler GetWeaponHandler(GameObject prefab)
    {
        return prefab.GetComponent<WeaponHandler>();
    }

    // 무기 파워 수치를 확인해서 숫자 반환
    private int GetWeaponPower(GameObject prefab)
    {
        var weapon = GetWeaponHandler(prefab);
        if (weapon == null) return 0;

        float power = weapon.WeaponPower;

        if (power >= 5f) return 5;
        if (power >= 4f) return 4;
        if (power >= 3f) return 3;
        if (power >= 2f) return 2;
        if (power > 0f) return 1;
        return 0;
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
        PlayerPrefs.SetString(SelectedWeaponKey, prefabName);
        // 저장된 이름 확인용
        string name = PlayerPrefs.GetString(SelectedWeaponKey, prefabName);
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
        gameObject.SetActive(false);
    }
}
