using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class busCanvaMovement : MonoBehaviour
{
    public float speed = 1.98f; // Hareket hızı
    public float leftBoundary = -270f; // Sol sınır
    public float rightBoundary = 270f; // Sağ sınır
    public Vector3 respawnPositionLeft = new Vector3(-263f, 121f, 0f); // Sol spawn konumu
    public Vector3 respawnPositionRight = new Vector3(267f, 180f, 0f); // Sağ spawn konumu
    public Vector3 respawnPositionMid = new Vector3(120f, 180f, 0f); // Sağ spawn konumu
    public bool isMovingRight = false; // Sağa mı gidiyor?
    public RectTransform rectTransform;
    public float movement;
    private clockScript clockScript;

    public Image imageComponent; // SpriteRenderer bileşeni

    void Start()
    {
        imageComponent = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform.anchoredPosition.y >= 150f)
        {
            isMovingRight = false;
        }
        else if (rectTransform.anchoredPosition.y < 150f)
        {
            isMovingRight = true;
            rectTransform.Rotate(new Vector3(0f, 180f, 0f));
            foreach (Transform child in transform)
        {
            // Alt objenin adını kontrol et (Adı "Text (TMP)" olanı bul)
            if (child.name == "Text (TMP)")
            {
                // Alt objenin TextMeshPro (TMP) bileşenini al
                TMP_Text textComponent = child.GetComponent<TMP_Text>();
                
                // Alt objenin TextMeshPro (TMP) bileşeni bulunduysa işlem yap
                if (textComponent != null)
                {
                    // Yeni rotasyon
                    Quaternion newRotationTMP = Quaternion.Euler(0f, 0f, 0f);
                    // TMP bileşeninin rotasyonunu değiştir
                    textComponent.rectTransform.rotation = newRotationTMP;
                    
                    // İstenirse döngüyü sonlandırabilir
                    break;
                }
            }
        }
        }
        GameObject clockObject = GameObject.Find("Clock");
        if (clockObject != null) {
            clockScript = clockObject.GetComponent<clockScript>();
        }
        leftBoundary = -265f;
        rightBoundary = 265f;
    }

    void Update()
    {
        speed = clockScript.speedCanva;
        if (imageComponent == null)
            return;

        movement = isMovingRight ? speed : -speed;
        rectTransform.anchoredPosition += new Vector2(movement * Time.deltaTime, 0f);
        if ((isMovingRight && rectTransform.anchoredPosition.x >= 270f) ||
            (!isMovingRight && rectTransform.anchoredPosition.x <= -270f))
        {
            Destroy(gameObject);
            // 10 saniye sonra tekrar spawn
        }
    }
}
