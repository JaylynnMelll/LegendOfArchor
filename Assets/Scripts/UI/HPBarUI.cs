using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarUI : BaseUI
{
    [SerializeField] private Slider hpSlider; // 체력바
    [SerializeField] private Slider hpDelayedSlider; // 흰색(잔상) 체력바
    [SerializeField] private Image fillImage; // 체력바 색 설정할 수 있게 이미지 분리
    [SerializeField] private TextMeshProUGUI hpText;
    private ResourceController resource;


    private float targetHP = 1f; // 체력비율 0.0f ~ 1.0f
    public float delaySpeed = 5f; // 흰색 체력바가 따라가는 속도

    private void Awake()
    {
        // 초기화 (풀링 대비)
        hpSlider.value = 1f;
        hpDelayedSlider.value = 1f;
        hpText.text = string.Empty;
    }
    public void Init(ResourceController resource)
    {
        this.resource = resource;
        resource.AddHealthChangeEvent(UpdateHP);
        UpdateHP(resource.CurrentHealth, resource.MaxHealth); // 초기 표시
    }

    private void Update()
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

    public void ResetHPBar()
    {
        // 리소스 언바인딩
        if (resource != null)
        {
            resource.RemoveHealthChangeEvent(UpdateHP);
            resource = null;
        }

        // 상태 초기화
        hpSlider.value = 1f;
        hpDelayedSlider.value = 1f;
        targetHP = 1f;
        hpText.text = string.Empty;
    }

    // UI 상태
    protected override UIState GetUIState()
    {
        return UIState.Game;
    }
}
