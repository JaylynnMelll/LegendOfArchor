using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private float moveUpSpeed = 30f;
    private float lifetime = 1.0f;

    public void Init(int damage)
    {
        if (text == null) return;
        text.text = $"- {damage}";
        text.color = Color.red;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
    }
}