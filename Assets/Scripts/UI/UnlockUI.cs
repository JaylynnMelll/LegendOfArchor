using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText; // 안내 텍스트
    [SerializeField] private Button confirmButton; // 확인 버튼
    [SerializeField] private Button cancelButton; // 취소 버튼

    private Action onConfirm; // 전달받은 외부 함수 저장 변수

    public void Open(int price, Action action)
    {
        gameObject.SetActive(true); // UI 오브젝트 활성화
        messageText.text = $"{price}Gold로 해금하시겠습니까?"; // 안내 메시지 출력
        onConfirm = action; // 해금 시 실행할 동작 저장
    }

    private void Awake()
    {
        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke(); // onConfirm에 저장된 함수가 nul이 아니면 실행
            Close(); // UI 비활성화
        });

        cancelButton.onClick.AddListener(Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        onConfirm = null;
    }
}
