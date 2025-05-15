using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponItemUI : MonoBehaviour
{
    [SerializeField] private Image weaponIcon; // 무기 아이콘 이미지
    [SerializeField] private GameObject weaponIconObject;  // 무기 아이콘 오브젝트
    [SerializeField] private GameObject lockIcon;    // 잠금 아이콘 오브젝트

    private int weaponIndex; // 무기 인덱스 번호
    private Action<int> onClickWeaponSelect; // 무기 선택 시 호출될 외부 함수 (OnClickWeaponItem)

    // 무기 데이터를 받아 UI 초기화
    public void Init(Sprite icon, int index, Action<int> onClick, bool isUnlocked)
    {
        weaponIndex = index; // 무기 인덱스 저장
        onClickWeaponSelect = onClick; // 클릭 시 실행할 함수 저장

        if (weaponIcon != null)
        {
            weaponIcon.gameObject.SetActive(isUnlocked); // 잠금 상태에 따라 아이콘 표시
            weaponIcon.sprite = icon; // 아이콘 이미지 설정
        }

        lockIcon.SetActive(!isUnlocked); // 잠금 상태에 따라 아이콘 표시

        GetComponent<Button>().onClick.AddListener(() =>
        {
            onClickWeaponSelect?.Invoke(weaponIndex); // 전달받은 함수 실행
        });
    }

    // 무기 해금될 때 쓰는 함수
    public void SetUnlocked()
    {
        lockIcon.SetActive(false); // 잠금 아이콘 비활성화
        weaponIconObject.SetActive(true); // 무기 아이콘 활성화
    }
}
