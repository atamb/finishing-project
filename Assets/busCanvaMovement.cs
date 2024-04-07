using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class busCanvaMovement : MonoBehaviour
{
    public float speed = 5f; // Hareket hızı
    public float leftBoundary = -270f; // Sol sınır
    public float rightBoundary = 270f; // Sağ sınır
    public Vector3 respawnPositionLeft = new Vector3(-238f, 121f, 0f); // Sol spawn konumu
    public Vector3 respawnPositionRight = new Vector3(238f, 180f, 0f); // Sağ spawn konumu
    public bool isMovingRight = false; // Sağa mı gidiyor?
    public RectTransform rectTransform;
    public float movement;

    private Image imageComponent; // SpriteRenderer bileşeni

    void Start()
    {
        // SpriteRenderer bileşenini al
        imageComponent = GetComponent<Image>();
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Eğer SpriteRenderer null ise (bileşen yoksa) update'i geç
        if (imageComponent == null)
            return;

        movement = isMovingRight ? speed : -speed;
        rectTransform.anchoredPosition += new Vector2(movement * Time.deltaTime, 0f);


        // Obje sol sınırı geçtiğinde veya sağ sınırı geçtiğinde
        if ((isMovingRight && rectTransform.anchoredPosition.x >= 270f) ||
            (!isMovingRight && rectTransform.anchoredPosition.x <= -270f))
        {
            // imageComponent'ı devre dışı bırak
            imageComponent.enabled = false;
            // 10 saniye sonra tekrar spawn
            RespawnObject();
        }
    }

    // Yeni objenin spawnlanması
    void RespawnObject()
    {
        // Sağa doğru mu gidiyoruz?
        if (isMovingRight)
        {
            // Yeni spawn konumu ve yön
            rectTransform.anchoredPosition = respawnPositionRight;
            isMovingRight = !isMovingRight;
        }
        else
        {
            // Yeni spawn konumu ve yön
            rectTransform.anchoredPosition = respawnPositionLeft;
            isMovingRight = !isMovingRight;
        }
        // imageComponent'ı tekrar etkinleştir
        imageComponent.enabled = true;
    }
}
