using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage; // 무기 아이콘 이미지

    private int weaponIndex; // 무기 인덱스 번호
    private Action<int> onClickWeaponSelect; // 무기 선택 시 호출될 외부 함수 (OnClickWeaponItem)

    // 무기 데이터를 받아 UI 초기화
    public void Init(Sprite icon, int index, System.Action<int> onClick)
    {
        iconImage.sprite = icon;
        weaponIndex = index;
        onClickWeaponSelect = onClick;

        // 버튼을 눌렀을 때 인덱스 전달하면서 실행
        GetComponent<Button>().onClick.AddListener(() => onClickWeaponSelect?.Invoke(weaponIndex));
    }
}
