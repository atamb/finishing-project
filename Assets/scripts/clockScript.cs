using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class clockScript : MonoBehaviour
{
    public GameObject[] busses;
    public Vector3 respawnPositionLeft = new Vector3(-238f, 121f, 0f); // Sol spawn konumu
    public Vector3 respawnPositionRight = new Vector3(238f, 180f, 0f); // Sağ spawn konumu
    public TextMeshProUGUI clockText; // Saati ekrana yazdırmak için TMP metin nesnesi
    private float timeMultiplier = 30f; // Yazdırma hızını ayarlamak için çarpan
    private bool checkBool;
    public GameObject parentObject;
    public RectTransform spawnRectTransform;
    public int hour;

    void Start()
    {
        checkBool = false;
    }

    void Update()
    {
        float totalElapsedTime = Time.realtimeSinceStartup;
        float gameTime = (totalElapsedTime + 5*3600 - 2700)* timeMultiplier;
        UpdateClock(gameTime);
    }
    void UpdateClock(float gameTime)
    {
        // Oyun zamanını saat, dakika ve saniyeye dönüştür
        hour = (int)(gameTime / 3600f) % 24; // Saati 24 saat formatına dönüştürmek için mod işlemi uygulandı
        int minute = (int)((gameTime / 60f) % 60);

        // Saati TMP metin nesnesine yazdır
        clockText.text = hour.ToString("00") + ":" + minute.ToString("00");

        if (hour == 7 && minute == 35 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[0], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            checkBool = true;
        }
        if (hour == 9 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            checkBool = false;
        }
        if (hour == 10 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[2], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            checkBool = true;
        }
        if (hour == 12 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[3], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            checkBool = false;
        }
        if (hour == 13 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[4], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            checkBool = true;
        }
        if (hour == 15 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[5], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            checkBool = false;
        }
        if (hour == 16 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[6], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            checkBool = true;
        }
        if (hour == 18 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[7], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            checkBool = false;
        }
        if (hour == 19 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[8], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            checkBool = true;
        }
        if (hour == 21 && minute == 10 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            checkBool = false;
        }
        if (hour == 22 && minute == 30 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[0], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            checkBool = true;
        }
    }
}
