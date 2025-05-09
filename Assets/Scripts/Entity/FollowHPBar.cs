using UnityEngine;
using UnityEngine.UI;

public class FollowHPBar : MonoBehaviour
{
    private Transform target; // 체력바가 따라갈 대상
    private RectTransform rectTransform; // 체력바의 RectTransform 컴포넌트
    private Canvas canvas; // 체력바 부모 Canvas
    private Camera cam;
    private Vector3 offset = new Vector3(0, 1.0f, 0); // 캐릭터 머리위로 띄우기 위한 offset

    void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // 체력바 RectTransform 참조
        canvas = GetComponentInParent<Canvas>(); // 체력바 부모 Canvas 참조
        cam = Camera.main; // 메인 카메라 참조(좌표 변환 기준 카메라)
    }

    void Update()
    {
        if (target == null) return; // 대상없을시 실행 X

        // 대상의 월드 위치 + offset -> 스크린 좌표로 변환
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
        // 체력바의 실제 위치를 스크린 좌표로 설정
        rectTransform.position = screenPos;
    }

    // 외부에서 대상 설정
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
