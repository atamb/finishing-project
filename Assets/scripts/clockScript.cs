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
    private float timer2 = 0f;
    private float timer3 = 0f;
    private float timer4 = 0f;
    public TMP_Text[] chargeTexts;
    public static int[] carNumbers = new int[11];
    private float StartTime;
    private const int totalInputs = 96; // Toplam input sayısı
    private const int hours = 16; // Toplam saat sayısı
    private const int halfInputs = totalInputs / 2; // Otobüs numarası ve dakika inputlarının yarısı
    public int[] a = new int[96];

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
        for (int i = 0; i < carNumbers.Length; i++)
        {
            carNumbers[i] = PlayerPrefs.GetInt("CarNumber_" + i);
        }
        StartTime = Time.realtimeSinceStartup;
        LoadChargingSchedule();
    }

    void Update()
    {
        float totalElapsedTime = Time.realtimeSinceStartup - StartTime;
        float gameTime = (totalElapsedTime * timeMultiplier + 7*3600+2400);
        UpdateClock(gameTime);
        busChargeFunc();
    }
    void LoadChargingSchedule()
    {
        for (int i = 0; i < halfInputs; i++)
        {
            int busNumber = PlayerPrefs.GetInt("BusNumber_" + i, 0);
            int minute = PlayerPrefs.GetInt("Minute_" + i, 0);

            a[2 * i] = busNumber;
            a[2 * i + 1] = minute;
        }
    }
    void busChargeFunc()
    {
        if(goingBool[0]||goingBool[1]||goingBool[2]||goingBool[3]||goingBool[4]||goingBool[5]||goingBool[6]||goingBool[7])
        {
        timer += Time.deltaTime;
        if (timer >= 8.62f)
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
    }

    void BusCharging(int busNum, int whichTimer)
    {
        if(whichTimer == 1)
        {
            timer2 += Time.deltaTime;
            if(timer2 >= 4f)
            {
                if(busCharges[busNum-1] < 100)
                {
                    busCharges[busNum-1] += 1;
                    chargeTexts[busNum-1].text = busCharges[busNum-1].ToString();
                }
                timer2 = 0;
            }
        }
        if(whichTimer == 2)
        {
            timer3 += Time.deltaTime;
            if(timer3 >= 4f)
            {
                if(busCharges[busNum-1] < 100)
                {
                    busCharges[busNum-1] += 1;
                    chargeTexts[busNum-1].text = busCharges[busNum-1].ToString();
                }
                timer3 = 0;
            }
        }

        if(whichTimer == 3)
        {
            timer4 += Time.deltaTime;
            if(timer4 >= 4f)
            {
                if(busCharges[busNum-1] < 100)
                {
                    busCharges[busNum-1] += 1;
                    chargeTexts[busNum-1].text = busCharges[busNum-1].ToString();
                }
                timer4 = 0;
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
            GameObject canvasInstance = Instantiate(busses[carNumbers[0]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[0].color = yellowColor;
            goingBool[carNumbers[0]-1] = true;
            checkBool = true;
        }
        if (hour==8)
        {
            if((a[0] != carNumbers[0]) && (minute <= a[1]) && (a[0] != 0))
                BusCharging(a[0],1);
            if((a[2] != carNumbers[0]) && (minute <= a[3]) && (a[2] != 0))
                BusCharging(a[2],2);
            if((a[4] != carNumbers[0]) && (minute <= a[5]) && (a[4] != 0))
                BusCharging(a[4],3);
        }
        if (hour==9)
        {
            if(minute <= 5)
            {
                if((a[6] != carNumbers[0]) && (minute <= a[7]) && (a[6] != 0))
                    BusCharging(a[6],1);
                if((a[8] != carNumbers[0]) && (minute <= a[9]) && (a[8] != 0))
                    BusCharging(a[8],2);
                if((a[10] != carNumbers[0]) && (minute <= a[11]) && (a[10] != 0))
                    BusCharging(a[10],3);
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[6] != carNumbers[0]) && (minute <= a[7]) && (a[6] != 0))
                    BusCharging(a[6],1);
                else if((a[6] == carNumbers[0]) && (minute <= a[7] + 5) && (a[6] != 0))
                    BusCharging(a[6],1);
                if((a[8] != carNumbers[0]) && (minute <= a[9]) && (a[8] != 0))
                    BusCharging(a[8],2);
                else if((a[8] == carNumbers[0]) && (minute <= a[9] + 5) && (a[8] != 0))
                    BusCharging(a[8],2);
                if((a[10] != carNumbers[0]) && (minute <= a[11]) && (a[10] != 0))
                    BusCharging(a[10],3);
                else if((a[10] == carNumbers[0]) && (minute <= a[11] + 5) && (a[10] != 0))
                    BusCharging(a[10],3);
            }
            else if(minute >= 15)
            {
                if((a[6] != carNumbers[1]) && (minute <= 15 + a[7]) && (a[6] != 0))
                    BusCharging(a[6],1);
                if((a[8] != carNumbers[1]) && (minute <= 15 + a[9]) && (a[8] != 0))
                    BusCharging(a[8],2);
                if((a[10] != carNumbers[1]) && (minute <= 15 + a[11]) && (a[10] != 0))
                    BusCharging(a[10],3);
            }
        }

        if (hour == 9 && minute == 5)
        {
            rawImages[0].color = greenColor;
            goingBool[carNumbers[0]-1] = false;
        }
        if (hour == 9 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[1]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[1].color = yellowColor;
            goingBool[carNumbers[1]-1] = true;
            checkBool = false;
        }
        if (hour==10)
        {
            if(minute <= 35)
            {
                if((a[12] != carNumbers[1]) && (minute <= a[13]) && (a[12] != 0))
                    BusCharging(a[12],1);
                if((a[14] != carNumbers[1]) && (minute <= a[15]) && (a[14] != 0))
                    BusCharging(a[14],2);
                if((a[16] != carNumbers[1]) && (minute <= a[17]) && (a[16] != 0))
                    BusCharging(a[16],3);
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[12] != carNumbers[1]) && (minute <= a[13]) && (a[12] != 0))
                    BusCharging(a[12],1);
                else if((a[12] == carNumbers[1]) && (minute <= a[13] + 35) && (a[12] != 0))
                    BusCharging(a[12],1);
                if((a[14] != carNumbers[1]) && (minute <= a[15]) && (a[14] != 0))
                    BusCharging(a[14],2);
                else if((a[14] == carNumbers[1]) && (minute <= a[15] + 35) && (a[14] != 0))
                    BusCharging(a[14],2);
                if((a[16] != carNumbers[1]) && (minute <= a[17]) && (a[16] != 0))
                    BusCharging(a[16],3);
                else if((a[16] == carNumbers[1]) && (minute <= a[17] + 35) && (a[16] != 0))
                    BusCharging(a[16],3);
            }
            else if(minute >= 45)
            {
                if((a[12] != carNumbers[2]) && (minute <= 45 + a[13]) && (a[12] != 0))
                    BusCharging(a[12],1);
                if((a[14] != carNumbers[2]) && (minute <= 45 + a[15]) && (a[14] != 0))
                    BusCharging(a[14],2);
                if((a[16] != carNumbers[2]) && (minute <= 45 + a[17]) && (a[16] != 0))
                    BusCharging(a[16],3);
            }
        }
        if (hour == 10 && minute == 35)
        {
            rawImages[1].color = greenColor;
            goingBool[carNumbers[1]-1] = false;
        }
        if (hour == 10 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[2]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[2].color = yellowColor;
            goingBool[carNumbers[2]-1] = true;
            checkBool = true;
        }
        if (hour==11)
        {
            if((a[18] != carNumbers[2]) && (minute <= a[19]) && (a[18] != 0))
                BusCharging(a[18],1);
            if((a[20] != carNumbers[2]) && (minute <= a[21]) && (a[20] != 0))
                BusCharging(a[20],2);
            if((a[22] != carNumbers[2]) && (minute <= a[23]) && (a[22] != 0))
                BusCharging(a[22],3);
        }
        if (hour==12)
        {
            if(minute <= 5)
            {
                if((a[24] != carNumbers[2]) && (minute <= a[25]) && (a[24] != 0))
                    BusCharging(a[24],1);
                if((a[26] != carNumbers[2]) && (minute <= a[27]) && (a[26] != 0))
                    BusCharging(a[26],2);
                if((a[28] != carNumbers[2]) && (minute <= a[29]) && (a[28] != 0))
                    BusCharging(a[28],3);
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[24] != carNumbers[2]) && (minute <= a[25]) && (a[24] != 0))
                    BusCharging(a[24],1);
                else if((a[24] == carNumbers[2]) && (minute <= a[25] + 5) && (a[24] != 0))
                    BusCharging(a[24],1);
                if((a[26] != carNumbers[2]) && (minute <= a[27]) && (a[26] != 0))
                    BusCharging(a[26],2);
                else if((a[26] == carNumbers[2]) && (minute <= a[27] + 5) && (a[26] != 0))
                    BusCharging(a[26],2);
                if((a[28] != carNumbers[2]) && (minute <= a[29]) && (a[28] != 0))
                    BusCharging(a[28],3);
                else if((a[28] == carNumbers[2]) && (minute <= a[29] + 5) && (a[28] != 0))
                    BusCharging(a[28],3);
            }
            else if(minute >= 15)
            {
                if((a[24] != carNumbers[3]) && (minute <= 15 + a[25]) && (a[24] != 0))
                    BusCharging(a[24],1);
                if((a[26] != carNumbers[3]) && (minute <= 15 + a[27]) && (a[26] != 0))
                    BusCharging(a[26],2);
                if((a[28] != carNumbers[3]) && (minute <= 15 + a[29]) && (a[28] != 0))
                    BusCharging(a[28],3);
            }
        }
        if (hour == 12 && minute == 5)
        {
            rawImages[2].color = greenColor;
            goingBool[carNumbers[2]-1] = false;
        }
        if (hour == 12 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[3]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[3].color = yellowColor;
            goingBool[carNumbers[3]-1] = true;
            checkBool = false;
        }
        if (hour==13)
        {
            if(minute <= 35)
            {
                if((a[30] != carNumbers[3]) && (minute <= a[31]) && (a[30] != 0))
                    BusCharging(a[30],1);
                if((a[32] != carNumbers[3]) && (minute <= a[33]) && (a[32] != 0))
                    BusCharging(a[32],2);
                if((a[34] != carNumbers[3]) && (minute <= a[35]) && (a[34] != 0))
                    BusCharging(a[34],3);
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[30] != carNumbers[3]) && (minute <= a[31]) && (a[30] != 0))
                    BusCharging(a[30],1);
                else if((a[30] == carNumbers[3]) && (minute <= a[31] + 35) && (a[30] != 0))
                    BusCharging(a[30],1);
                if((a[32] != carNumbers[3]) && (minute <= a[33]) && (a[32] != 0))
                    BusCharging(a[32],2);
                else if((a[32] == carNumbers[3]) && (minute <= a[33] + 35) && (a[32] != 0))
                    BusCharging(a[32],2);
                if((a[34] != carNumbers[3]) && (minute <= a[35]) && (a[34] != 0))
                    BusCharging(a[34],3);
                else if((a[34] == carNumbers[3]) && (minute <= a[35] + 35) && (a[34] != 0))
                    BusCharging(a[34],3);
            }
            else if(minute >= 45)
            {
                if((a[30] != carNumbers[4]) && (minute <= 45 + a[31]) && (a[30] != 0))
                    BusCharging(a[30],1);
                if((a[32] != carNumbers[4]) && (minute <= 45 + a[33]) && (a[32] != 0))
                    BusCharging(a[32],2);
                if((a[34] != carNumbers[4]) && (minute <= 45 + a[35]) && (a[34] != 0))
                    BusCharging(a[34],3);
            }
        }
        if (hour == 13 && minute == 35)
        {
            rawImages[3].color = greenColor;
            goingBool[carNumbers[3]-1] = false;
        }
        if (hour == 13 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[4]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[4].color = yellowColor;
            goingBool[carNumbers[4]-1] = true;
            checkBool = true;
        }
        if (hour==14)
        {
            if((a[36] != carNumbers[4]) && (minute <= a[36]) && (a[36] != 0))
                BusCharging(a[36],1);
            if((a[38] != carNumbers[4]) && (minute <= a[38]) && (a[38] != 0))
                BusCharging(a[38],2);
            if((a[40] != carNumbers[4]) && (minute <= a[40]) && (a[40] != 0))
                BusCharging(a[40],3);
        }
        if (hour==15)
        {
            if(minute <= 5)
            {
                if((a[42] != carNumbers[4]) && (minute <= a[43]) && (a[42] != 0))
                    BusCharging(a[24],1);
                if((a[44] != carNumbers[4]) && (minute <= a[45]) && (a[44] != 0))
                    BusCharging(a[26],2);
                if((a[46] != carNumbers[4]) && (minute <= a[47]) && (a[46] != 0))
                    BusCharging(a[28],3);
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[42] != carNumbers[3]) && (minute <= a[43]) && (a[42] != 0))
                    BusCharging(a[42],1);
                else if((a[42] == carNumbers[3]) && (minute <= a[43] + 5) && (a[42] != 0))
                    BusCharging(a[42],1);
                if((a[44] != carNumbers[3]) && (minute <= a[45]) && (a[44] != 0))
                    BusCharging(a[44],2);
                else if((a[44] == carNumbers[3]) && (minute <= a[45] + 5) && (a[44] != 0))
                    BusCharging(a[44],2);
                if((a[46] != carNumbers[3]) && (minute <= a[47]) && (a[46] != 0))
                    BusCharging(a[46],3);
                else if((a[46] == carNumbers[3]) && (minute <= a[47] + 5) && (a[46] != 0))
                    BusCharging(a[46],3);
            }
            else if(minute >= 15)
            {
                if((a[42] != carNumbers[5]) && (minute <= 15 + a[43]) && (a[42] != 0))
                    BusCharging(a[24],1);
                if((a[44] != carNumbers[5]) && (minute <= 15 + a[45]) && (a[44] != 0))
                    BusCharging(a[26],2);
                if((a[46] != carNumbers[5]) && (minute <= 15 + a[47]) && (a[46] != 0))
                    BusCharging(a[28],3);
            }
        }
        if (hour == 15 && minute == 05)
        {
            rawImages[4].color = greenColor;
            goingBool[carNumbers[4]-1] = false;
        }
        if (hour == 15 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[5]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[5].color = yellowColor;
            goingBool[carNumbers[5]-1] = true;
            checkBool = false;
        }
        if (hour==16)
        {
            if(minute <= 35)
            {
                if((a[48] != carNumbers[5]) && (minute <= a[49]) && (a[48] != 0))
                    BusCharging(a[48],1);
                if((a[50] != carNumbers[5]) && (minute <= a[51]) && (a[50] != 0))
                    BusCharging(a[50],2);
                if((a[52] != carNumbers[5]) && (minute <= a[53]) && (a[52] != 0))
                    BusCharging(a[52],3);
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[48] != carNumbers[5]) && (minute <= a[49]) && (a[48] != 0))
                    BusCharging(a[48],1);
                else if((a[48] == carNumbers[5]) && (minute <= a[49] + 35) && (a[48] != 0))
                    BusCharging(a[48],1);
                if((a[50] != carNumbers[5]) && (minute <= a[51]) && (a[50] != 0))
                    BusCharging(a[50],2);
                else if((a[50] == carNumbers[5]) && (minute <= a[51] + 35) && (a[50] != 0))
                    BusCharging(a[50],2);
                if((a[52] != carNumbers[5]) && (minute <= a[53]) && (a[52] != 0))
                    BusCharging(a[52],3);
                else if((a[52] == carNumbers[5]) && (minute <= a[53] + 35) && (a[52] != 0))
                    BusCharging(a[52],3);
            }
            else if(minute >= 45)
            {
                if((a[48] != carNumbers[6]) && (minute <= 45 + a[49]) && (a[48] != 0))
                    BusCharging(a[48],1);
                if((a[50] != carNumbers[6]) && (minute <= 45 + a[51]) && (a[50] != 0))
                    BusCharging(a[50],2);
                if((a[52] != carNumbers[6]) && (minute <= 45 + a[53]) && (a[52] != 0))
                    BusCharging(a[52],3);
            }
        }
        if (hour == 16 && minute == 35)
        {
            rawImages[5].color = greenColor;
            goingBool[carNumbers[5]-1] = false;
        }
        if (hour == 16 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[6]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[6].color = yellowColor;
            goingBool[carNumbers[6]-1] = true;
            checkBool = true;
        }
        if (hour==17)
        {
            if((a[54] != carNumbers[6]) && (minute <= a[55]) && (a[54] != 0))
                BusCharging(a[54],1);
            if((a[56] != carNumbers[6]) && (minute <= a[57]) && (a[56] != 0))
                BusCharging(a[56],2);
            if((a[58] != carNumbers[6]) && (minute <= a[59]) && (a[58] != 0))
                BusCharging(a[58],3);
        }
        if (hour==18)
        {
            if(minute <= 5)
            {
                if((a[60] != carNumbers[6]) && (minute <= a[61]) && (a[60] != 0))
                    BusCharging(a[60],1);
                if((a[62] != carNumbers[6]) && (minute <= a[63]) && (a[62] != 0))
                    BusCharging(a[62],2);
                if((a[64] != carNumbers[6]) && (minute <= a[65]) && (a[64] != 0))
                    BusCharging(a[64],3);
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[60] != carNumbers[6]) && (minute <= a[61]) && (a[60] != 0))
                    BusCharging(a[60],1);
                else if((a[60] == carNumbers[6]) && (minute <= a[61] + 5) && (a[60] != 0))
                    BusCharging(a[60],1);
                if((a[62] != carNumbers[6]) && (minute <= a[63]) && (a[62] != 0))
                    BusCharging(a[62],2);
                else if((a[62] == carNumbers[6]) && (minute <= a[63] + 5) && (a[62] != 0))
                    BusCharging(a[62],2);
                if((a[64] != carNumbers[6]) && (minute <= a[65]) && (a[64] != 0))
                    BusCharging(a[64],3);
                else if((a[64] == carNumbers[6]) && (minute <= a[65] + 5) && (a[64] != 0))
                    BusCharging(a[64],3);
            }
            else if(minute >= 15)
            {
                if((a[60] != carNumbers[7]) && (minute <= 15 + a[61]) && (a[60] != 0))
                    BusCharging(a[60],1);
                if((a[62] != carNumbers[7]) && (minute <= 15 + a[63]) && (a[62] != 0))
                    BusCharging(a[62],2);
                if((a[64] != carNumbers[7]) && (minute <= 15 + a[65]) && (a[64] != 0))
                    BusCharging(a[64],3);
            }
        }
        if (hour == 18 && minute == 5)
        {
            rawImages[6].color = greenColor;
            goingBool[carNumbers[6]-1] = false;
        }
        if (hour == 18 && minute == 15 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[7]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[7].color = yellowColor;
            goingBool[carNumbers[7]-1] = true;
            checkBool = false;
        }
        if (hour==19)
        {
            if(minute <= 35)
            {
                if((a[66] != carNumbers[7]) && (minute <= a[67]) && (a[66] != 0))
                    BusCharging(a[66],1);
                if((a[68] != carNumbers[7]) && (minute <= a[69]) && (a[68] != 0))
                    BusCharging(a[68],2);
                if((a[70] != carNumbers[7]) && (minute <= a[71]) && (a[70] != 0))
                    BusCharging(a[70],3);
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[66] != carNumbers[7]) && (minute <= a[67]) && (a[66] != 0))
                    BusCharging(a[66],1);
                else if((a[66] == carNumbers[7]) && (minute <= a[67] + 35) && (a[66] != 0))
                    BusCharging(a[66],1);
                if((a[68] != carNumbers[7]) && (minute <= a[69]) && (a[68] != 0))
                    BusCharging(a[68],2);
                else if((a[68] == carNumbers[7]) && (minute <= a[69] + 35) && (a[68] != 0))
                    BusCharging(a[68],2);
                if((a[70] != carNumbers[7]) && (minute <= a[71]) && (a[70] != 0))
                    BusCharging(a[70],3);
                else if((a[70] == carNumbers[7]) && (minute <= a[71] + 35) && (a[70] != 0))
                    BusCharging(a[70],3);
            }
            else if(minute >= 45)
            {
                if((a[66] != carNumbers[8]) && (minute <= 45 + a[67]) && (a[66] != 0))
                    BusCharging(a[66],1);
                if((a[68] != carNumbers[8]) && (minute <= 45 + a[69]) && (a[68] != 0))
                    BusCharging(a[68],2);
                if((a[70] != carNumbers[8]) && (minute <= 45 + a[71]) && (a[70] != 0))
                    BusCharging(a[70],3);
            }
        }
        if (hour == 19 && minute == 35)
        {
            rawImages[7].color = greenColor;
            goingBool[carNumbers[7]-1] = false;
        }
        if (hour == 19 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[8]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[8].color = yellowColor;
            goingBool[carNumbers[8]-1] = true;
            checkBool = true;
        }
        if (hour==20)
        {
            if((a[72] != carNumbers[8]) && (minute <= a[73]) && (a[72] != 0))
                BusCharging(a[72],1);
            if((a[74] != carNumbers[8]) && (minute <= a[75]) && (a[74] != 0))
                BusCharging(a[74],2);
            if((a[76] != carNumbers[8]) && (minute <= a[77]) && (a[76] != 0))
                BusCharging(a[76],3);
        }
        if (hour==21)
        {
            if(minute <= 5)
            {
                if((a[78] != carNumbers[8]) && (minute <= a[79]) && (a[78] != 0))
                    BusCharging(a[78],1);
                if((a[80] != carNumbers[8]) && (minute <= a[81]) && (a[80] != 0))
                    BusCharging(a[80],2);
                if((a[82] != carNumbers[8]) && (minute <= a[83]) && (a[82] != 0))
                    BusCharging(a[82],3);
            }
            else if((minute > 5)&&(minute<10))
            {
                if((a[78] != carNumbers[8]) && (minute <= a[79]) && (a[78] != 0))
                    BusCharging(a[78],1);
                else if((a[78] == carNumbers[8]) && (minute <= a[79] + 5) && (a[78] != 0))
                    BusCharging(a[78],1);
                if((a[80] != carNumbers[8]) && (minute <= a[81]) && (a[80] != 0))
                    BusCharging(a[80],2);
                else if((a[80] == carNumbers[8]) && (minute <= a[81] + 5) && (a[80] != 0))
                    BusCharging(a[80],2);
                if((a[82] != carNumbers[8]) && (minute <= a[83]) && (a[82] != 0))
                    BusCharging(a[82],3);
                else if((a[82] == carNumbers[8]) && (minute <= a[83] + 5) && (a[82] != 0))
                    BusCharging(a[82],3);
            }
            else if(minute >= 10)
            {
                if((a[78] != carNumbers[9]) && (minute <= 15 + a[79]) && (a[78] != 0))
                    BusCharging(a[78],1);
                if((a[80] != carNumbers[9]) && (minute <= 15 + a[81]) && (a[80] != 0))
                    BusCharging(a[80],2);
                if((a[82] != carNumbers[9]) && (minute <= 15 + a[83]) && (a[82] != 0))
                    BusCharging(a[82],3);
            }
        }
        if (hour == 21 && minute == 5)
        {
            rawImages[8].color = greenColor;
            goingBool[carNumbers[8]-1] = false;
        }
        if (hour == 21 && minute == 10 && checkBool == true)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[9]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionLeft;
            rawImages[9].color = yellowColor;
            goingBool[carNumbers[9]-1] = true;
            checkBool = false;
        }
        if (hour==22)
        {
            if(minute <= 30)
            {
                if((a[84] != carNumbers[9]) && (minute <= a[85]) && (a[84] != 0))
                    BusCharging(a[84],1);
                if((a[86] != carNumbers[9]) && (minute <= a[87]) && (a[86] != 0))
                    BusCharging(a[86],2);
                if((a[88] != carNumbers[9]) && (minute <= a[89]) && (a[88] != 0))
                    BusCharging(a[88],3);
            }
            else if(minute >= 30)
            {
                if((a[84] != carNumbers[10]) && (minute <= 30 + a[85]) && (a[84] != 0))
                    BusCharging(a[84],1);
                if((a[86] != carNumbers[10]) && (minute <= 30 + a[87]) && (a[86] != 0))
                    BusCharging(a[86],2);
                if((a[88] != carNumbers[10]) && (minute <= 30 + a[89]) && (a[88] != 0))
                    BusCharging(a[88],3);
            }
        }
        if (hour == 22 && minute == 30 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[10]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[10].color = yellowColor;
            goingBool[carNumbers[10]-1] = true;
            rawImages[9].color = greenColor;
            goingBool[carNumbers[9]-1] = false;
            checkBool = true;
        }
        if (hour == 23 && minute == 50)
        {
            rawImages[10].color = greenColor;
            goingBool[carNumbers[10]-1] = false;
        }
    }
}
