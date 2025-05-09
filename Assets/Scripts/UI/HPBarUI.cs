using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarUI : BaseUI
{
    // [SerializeField] private Slider delayedSlider;
    // [SerializeField] private Slider immediateSlider;
    [SerializeField] private Slider hpSlider; // 체력바
    [SerializeField] private Slider hpDelayedSlider; // 흰색(잔상) 체력바
    [SerializeField] private Image fillImage; // 체력바 색 설정할 수 있게 이미지 분리
    [SerializeField] private TextMeshProUGUI hpText;


    private float targetHP = 1f; // 체력비율 0.0f ~ 1.0f
    public float delaySpeed = 5f; // 흰색 체력바가 따라가는 속도

    void Update()
    {
        // 흰색 체력바는 잔상처리 함
        hpDelayedSlider.value = Mathf.Lerp(hpDelayedSlider.value,
        targetHP, Time.deltaTime * delaySpeed);
    }


    public void UpdateHP(float currentHP, float maxHP)
    {
        // 
        targetHP = currentHP / maxHP;
        hpText.text = $"{targetHP * maxHP}";
        // 체력바 즉시 적용
        hpSlider.value = targetHP;
    }

    // 체력바 색 설정
    public void SetFillColor(Color color)
    {
        fillImage.color = color;
    }

    // UI 상태
    protected override UIState GetUIState()
    {
        return UIState.Game;
    }
}
