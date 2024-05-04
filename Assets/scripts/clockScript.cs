using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class clockScript : MonoBehaviour
{
    public GameObject[] busses;
    public Vector3 respawnPositionLeft = new Vector3(-238f, 121f, 0f); // Sol spawn konumu
    public Vector3 respawnPositionRight = new Vector3(238f, 180f, 0f); // Sağ spawn konumu
    public TextMeshProUGUI clockText; // Saati ekrana yazdırmak için TMP metin nesnesi
    private float timeMultiplier = 30f; // Yazdırma hızını ayarlamak için çarpan
    private bool checkBool;
    public bool[] goingBool = new bool[8];
    public GameObject parentObject;
    public RectTransform spawnRectTransform;
    public int hour;
    public RawImage[] rawImages; // RawImage bileşeni
    Color greenColor = new Color(0f, 1f, 0f); // R:0, G:1, B:0
    Color yellowColor = new Color(1f, 1f, 0f);
    public int[] busCharges = new int[8];
    private float timer = 0f;
    public TMP_Text[] chargeTexts;

    void Start()
    {
        checkBool = false;
        respawnPositionLeft = new Vector3(-267f, 121f, 0f);
        respawnPositionRight = new Vector3(267f, 180f, 0f);
        for (int i = 0; i < busCharges.Length; i++)
        {
            busCharges[i] = 100;
            goingBool[i] = false;
        }
    }

    void Update()
    {
        float totalElapsedTime = Time.realtimeSinceStartup;
        float gameTime = (totalElapsedTime + 5*3600 - 2700)* timeMultiplier;
        UpdateClock(gameTime);
        busChargeFunc();
    }
    void busChargeFunc()
    {
        timer += Time.deltaTime;

        // Belirli bir süre geçtikten sonra busCharges[1]'i azalt
        if (timer >= 1f)
        {
            if (goingBool[0])
            {
                busCharges[0] -= 1;
                timer = 0f;
                chargeTexts[0].text = busCharges[0].ToString();
            }
            else if(goingBool[1])
            {
                busCharges[1] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[1].text = busCharges[1].ToString();
            }
            else if(goingBool[2])
            {
                busCharges[2] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[2].text = busCharges[2].ToString();
            }
            else if(goingBool[3])
            {
                busCharges[3] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[3].text = busCharges[3].ToString();
            }
            else if(goingBool[4])
            {
                busCharges[4] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[4].text = busCharges[4].ToString();
            }
            else if(goingBool[5])
            {
                busCharges[5] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[5].text = busCharges[5].ToString();
            }
            else if(goingBool[6])
            {
                busCharges[6] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[6].text = busCharges[6].ToString();
            }
            else if(goingBool[7])
            {
                busCharges[7] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[7].text = busCharges[7].ToString();
            }
        }
    }

    void UpdateClock(float gameTime)
    {
        // Oyun zamanını saat, dakika ve saniyeye dönüştür
        hour = (int)(gameTime / 3600f) % 24; // Saati 24 saat formatına dönüştürmek için mod işlemi uygulandı
        int minute = (int)((gameTime / 60f) % 60);

        // Saati TMP metin nesnesine yazdır
        clockText.text = hour.ToString("00") + ":" + minute.ToString("00");

        if (hour == 7 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[0], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[0].color = yellowColor;
            goingBool[0] = true;
            checkBool = true;
        }
        if (hour == 9 && minute == 5)
        {
            rawImages[0].color = greenColor;
            goingBool[0] = false;
        }
        if (hour == 9 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[1].color = yellowColor;
            goingBool[1] = true;
            checkBool = false;
        }
        if (hour == 10 && minute == 35)
        {
            rawImages[1].color = greenColor;
            goingBool[1] = false;
        }
        if (hour == 10 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[2], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[2].color = yellowColor;
            goingBool[2] = true;
            checkBool = true;
        }
        if (hour == 12 && minute == 5)
        {
            rawImages[2].color = greenColor;
            goingBool[2] = false;
        }
        if (hour == 12 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[3], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[3].color = yellowColor;
            goingBool[3] = true;
            checkBool = false;
        }
        if (hour == 13 && minute == 35)
        {
            rawImages[3].color = greenColor;
            goingBool[3] = false;
        }
        if (hour == 13 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[4], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[4].color = yellowColor;
            goingBool[4] = true;
            checkBool = true;
        }
        if (hour == 15 && minute == 05)
        {
            rawImages[4].color = greenColor;
            goingBool[4] = false;
        }
        if (hour == 15 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[5], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[5].color = yellowColor;
            goingBool[5] = true;
            checkBool = false;
        }
        if (hour == 16 && minute == 35)
        {
            rawImages[5].color = greenColor;
            goingBool[5] = false;
        }
        if (hour == 16 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[6], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[6].color = yellowColor;
            goingBool[6] = true;
            checkBool = true;
        }
        if (hour == 18 && minute == 5)
        {
            rawImages[6].color = greenColor;
            goingBool[6] = false;
        }
        if (hour == 18 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[7], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[7].color = yellowColor;
            goingBool[7] = true;
            checkBool = false;
        }
        if (hour == 19 && minute == 35)
        {
            rawImages[7].color = greenColor;
            goingBool[7] = false;
        }
        if (hour == 19 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[3], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[8].color = yellowColor;
            goingBool[8] = true;
            checkBool = true;
        }
        if (hour == 21 && minute == 5)
        {
            rawImages[8].color = greenColor;
            goingBool[8] = false;
        }
        if (hour == 21 && minute == 10 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[9].color = yellowColor;
            goingBool[9] = true;
            checkBool = false;
        }
        if (hour == 22 && minute == 30 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[0], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[10].color = yellowColor;
            goingBool[10] = true;
            rawImages[9].color = greenColor;
            goingBool[9] = false;
            checkBool = true;
        }
        if (hour == 23 && minute == 50)
        {
            rawImages[10].color = greenColor;
            goingBool[10] = false;
        }
    }
}
