using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class clockScript : MonoBehaviour
{
    public GameObject[] busses;
    public GameObject ended;
    public Vector3 respawnPositionLeft = new Vector3(-238f, 121f, 0f); // Sol spawn konumu
    public Vector3 respawnPositionRight = new Vector3(238f, 180f, 0f); // Sağ spawn konumu
    public TextMeshProUGUI clockText; // Saati ekrana yazdırmak için TMP metin nesnesi
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI carbonText;
    public TextMeshProUGUI avgBusCharge;
    public TextMeshProUGUI priceText2;
    public TextMeshProUGUI carbonText2;
    public TextMeshProUGUI avgBusCharge2;
    public TextMeshProUGUI priceText3;
    public TextMeshProUGUI carbonText3;
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
    public static float totalCost = 0;
    public static float totalEmission = 0;
    private float StartTime;
    private const int totalInputs = 96; // Toplam input sayısı
    private const int hours = 16; // Toplam saat sayısı
    private const int halfInputs = totalInputs / 2; // Otobüs numarası ve dakika inputlarının yarısı
    public int[] a = new int[96];
    public GameObject[] lightning;
    public int[] whereisBus = new int[8];
    public GameObject bussesSabiha; // Otobüslerin instantiate edileceği ana obje
    public GameObject busses2; // Otobüslerin instantiate edileceği ana obje
    public GameObject objePrefab;
    private GameObject[] instantiatedBusses;
    private float totalCharge = 0;
    public int speedMultiplier = 1; 
    private float lastSpeedChangeTime;
    private float accumulatedGameTime;
    public float speedCanva;
    public TextMeshProUGUI speedX;

    private float minCarbonValue = float.MaxValue;
    private float minPriceValue = float.MaxValue;
    private float minAvgBusCharge = float.MaxValue;
    void Start()
    {
        checkBool = false;
        respawnPositionLeft = new Vector3(-267f, 121f, 0f);
        respawnPositionRight = new Vector3(267f, 180f, 0f);
        instantiatedBusses = new GameObject[7];
        speedCanva = 3.3f;
        for (int i = 0; i < busCharges.Length; i++)
        {
            busCharges[i] = 100;
            goingBool[i] = false;
        }
        for (int i = 0; i < carNumbers.Length; i++)
        {
            carNumbers[i] = PlayerPrefs.GetInt("CarNumber_" + i);
            Debug.Log(carNumbers[i]);
        }
        for (int i = 0; i < carNumbers.Length; i++)
        {
            if(i % 2 == 0 && whereisBus[carNumbers[i]-1]==0)
                whereisBus[carNumbers[i]-1] = 1;//1 ise sabiha
            if(i % 2 == 1 && whereisBus[carNumbers[i]-1]==0)
                whereisBus[carNumbers[i]-1] = 2;//2 ise kadikoy
        }
        for (int i = 0; i < whereisBus.Length; i++)
        {
            if(whereisBus[i] == 0)
                whereisBus[i] = 1;
        }
        StartTime = Time.realtimeSinceStartup;
        lastSpeedChangeTime = StartTime;
        accumulatedGameTime = 7 * 3600 + 1800;
        LoadChargingSchedule();
        float electricityPrice = PlayerPrefs.GetFloat("ElectricPrice", totalCost);
        float carbonTotal = PlayerPrefs.GetFloat("CarbonEmission", totalEmission);
        carbonText.text = carbonTotal.ToString();
        priceText.text = electricityPrice.ToString();
        carbonText2.text = carbonTotal.ToString();
        priceText2.text = electricityPrice.ToString();
        for (int i = 0; i < busCharges.Length; i++)
        {
            totalCharge += busCharges[i];
        }
        totalCharge /= 8;
        avgBusCharge.text = totalCharge.ToString();
        
        minCarbonValue = PlayerPrefs.GetFloat("MinCarbonValue", float.MaxValue);
        minPriceValue = PlayerPrefs.GetFloat("MinPriceValue", float.MaxValue);
        minAvgBusCharge = PlayerPrefs.GetFloat("MinAvgBusCharge", float.MaxValue);
        carbonText3.text = minCarbonValue.ToString();
        priceText3.text = minPriceValue.ToString();
        avgBusCharge2.text = minAvgBusCharge.ToString();
        speedX.text = speedMultiplier.ToString();
    }

    void Update()
    {
        float currentTime = Time.realtimeSinceStartup;
        float deltaTime = currentTime - lastSpeedChangeTime;
    // Geçen zamanı hız çarpanı ile birlikte oyun zamanına ekle
        accumulatedGameTime += deltaTime * timeMultiplier * speedMultiplier;
    // Güncellemeler için lastSpeedChangeTime zamanını güncelle
        lastSpeedChangeTime = currentTime;
        UpdateClock(accumulatedGameTime);
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
   public void speedControlPlus() {
        if(speedMultiplier < 16) {
            UpdateAccumulatedGameTime();
            speedMultiplier *= 2;
            speedCanva *= 2;
            speedX.text = speedMultiplier.ToString();
        }
    }

    public void speedControlMinus() {
        if(speedMultiplier > 1) {
            UpdateAccumulatedGameTime();
            speedMultiplier /= 2;
            speedCanva /= 2;
            speedX.text = speedMultiplier.ToString();
        }
    }
    private void UpdateAccumulatedGameTime() {
    float currentTime = Time.realtimeSinceStartup;
    float deltaTime = currentTime - lastSpeedChangeTime;
    
    // Geçen zamanı hız çarpanı ile birlikte oyun zamanına ekle
    accumulatedGameTime += deltaTime * timeMultiplier * speedMultiplier;
    
    // Güncellemeler için lastSpeedChangeTime zamanını güncelle
    lastSpeedChangeTime = currentTime;
}
    void busChargeFunc()
    {
        if(goingBool[0]||goingBool[1]||goingBool[2]||goingBool[3]||goingBool[4]||goingBool[5]||goingBool[6]||goingBool[7])
        {
        timer += Time.deltaTime * speedMultiplier;
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
            if((whereisBus[busNum-1]==1)&&(Input.GetKeyDown(KeyCode.Alpha1)))
                ChargeAnimation(1,0);
            if((whereisBus[busNum-1]==2)&&(Input.GetKeyDown(KeyCode.Alpha1)))
                ChargeAnimation(1,2);
            lightning[busNum-1].SetActive(true);
            timer2 += Time.deltaTime * speedMultiplier;
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
            if((whereisBus[busNum-1]==1)&&(Input.GetKeyDown(KeyCode.Alpha1)))
                ChargeAnimation(2,0);
            if((whereisBus[busNum-1]==2)&&(Input.GetKeyDown(KeyCode.Alpha1)))
                ChargeAnimation(1,2);
            lightning[busNum-1].SetActive(true);
            timer3 += Time.deltaTime * speedMultiplier;
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
            if((whereisBus[busNum-1]==1)&&(Input.GetKeyDown(KeyCode.Alpha1)))
                ChargeAnimation(3,0);
            if((whereisBus[busNum-1]==2)&&(Input.GetKeyDown(KeyCode.Alpha1)))
                ChargeAnimation(3,2);
            lightning[busNum-1].SetActive(true);
            timer4 += Time.deltaTime * speedMultiplier;
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

    void ChargeFinished(int busNum, int whichNum)
    {
        lightning[busNum-1].SetActive(false);
        if(whereisBus[busNum-1]==1)
            ChargeAnimation(whichNum,1);
        if(whereisBus[busNum-1]==2)
            ChargeAnimation(whichNum,3);
    }

    void ChargeAnimation(int whichPark, int SabiKadiGirisCikis){
        string isim1="kadisarj";
        string isim2="kadicikis";
        string isim3="sabisarj";
        string isim4="sabicikis";
        if(SabiKadiGirisCikis == 0){
            string animadi = isim3 + whichPark.ToString();
            CreateObjectAndPlayAnimation(animadi,whichPark-1);
        }
        else if(SabiKadiGirisCikis == 1){
            string animadi = isim4 + whichPark.ToString();
            CreateObjectAndPlayAnimation(animadi,whichPark-1);
        }
        else if(SabiKadiGirisCikis == 2){
            string animadi = isim1 + whichPark.ToString();
            CreateObjectAndPlayAnimation2(animadi,whichPark+2);
        }
        else if(SabiKadiGirisCikis == 3){
            string animadi = isim2 + whichPark.ToString();
            CreateObjectAndPlayAnimation2(animadi,whichPark+2);
        }
    }
    void CreateObjectAndPlayAnimation(string animationName, int index)
    {
        if (instantiatedBusses[index] != null)
        {
            Destroy(instantiatedBusses[index]);
        }

        // Otobüslerin instantiate edileceği konumu belirle
        Vector3 position = bussesSabiha.transform.position;

        // Objeyi oluştur
        GameObject obje = Instantiate(objePrefab, position, Quaternion.identity);

        // Oluşturulan objenin ebeveynini "busses" objesi olarak ayarla
        obje.transform.parent = bussesSabiha.transform;

        // Oluşturulan objenin içerdiği Animator bileşenini al
        Animator animator = obje.GetComponent<Animator>();

        // Eğer Animator bileşeni varsa ve belirtilen animasyon mevcutsa, oynat
        if (animator != null && animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName);
        }
        else
        {
            Debug.LogWarning("Animator bileşeni veya belirtilen animasyon bulunamadı.");
        }
        instantiatedBusses[index] = obje;

    }
    void CreateObjectAndPlayAnimation2(string animationName, int index)
    {
        if (instantiatedBusses[index] != null)
        {
            Destroy(instantiatedBusses[index]);
        }

        // Otobüslerin instantiate edileceği konumu belirle
        Vector3 position = busses2.transform.position;

        // Objeyi oluştur
        GameObject obje = Instantiate(objePrefab, position, Quaternion.identity);

        // Oluşturulan objenin ebeveynini "busses" objesi olarak ayarla
        obje.transform.parent = busses2.transform;

        // Oluşturulan objenin içerdiği Animator bileşenini al
        Animator animator = obje.GetComponent<Animator>();

        // Eğer Animator bileşeni varsa ve belirtilen animasyon mevcutsa, oynat
        if (animator != null && animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName);
        }
        else
        {
            Debug.LogWarning("Animator bileşeni veya belirtilen animasyon bulunamadı.");
        }
        instantiatedBusses[index] = obje;

    }

    void UpdateClock(float gameTime)
    {
        int hour = (int)(gameTime / 3600f) % 24; // Saati 24 saat formatına dönüştürmek için mod işlemi uygulandı
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
            if(whereisBus[carNumbers[0]-1] == 1)
                whereisBus[carNumbers[0]-1] = 2;
            if(whereisBus[carNumbers[0]-1] == 2)
                whereisBus[carNumbers[0]-1] = 1;
            checkBool = true;
        }
        if (hour==8)
        {
            if((a[0] != carNumbers[0]) && (minute <= a[1]) && (a[0] != 0))
                {BusCharging(a[0],1);
                if((minute == a[1])&&(Input.GetKeyDown(KeyCode.Alpha2)))
                    ChargeFinished(a[0],1);}
            if((a[2] != carNumbers[0]) && (minute <= a[3]) && (a[2] != 0))
                {BusCharging(a[2],2);
                if((minute == a[3])&&(Input.GetKeyDown(KeyCode.Alpha2)))
                    ChargeFinished(a[2],2);}
            if((a[4] != carNumbers[0]) && (minute <= a[5]) && (a[4] != 0))
                {BusCharging(a[4],3);
                if((minute == a[5])&&(Input.GetKeyDown(KeyCode.Alpha2)))
                    ChargeFinished(a[4],3);}
        }
        if (hour==9)
        {
            if(minute <= 5)
            {
                if((a[6] != carNumbers[0]) && (minute <= a[7]) && (a[6] != 0))
                    {BusCharging(a[6],1);
                    if(minute == a[7])
                        lightning[a[6]-1].SetActive(false);}
                if((a[8] != carNumbers[0]) && (minute <= a[9]) && (a[8] != 0))
                    {BusCharging(a[8],2);
                    if(minute == a[9])
                        lightning[a[8]-1].SetActive(false);}
                if((a[10] != carNumbers[0]) && (minute <= a[11]) && (a[10] != 0))
                    {BusCharging(a[10],3);
                    if(minute == a[11])
                        lightning[a[10]-1].SetActive(false);}
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[6] != carNumbers[0]) && (minute <= a[7]) && (a[6] != 0))
                    {BusCharging(a[6],1);
                    if(minute == a[7])
                        lightning[a[6]-1].SetActive(false);}
                else if((a[6] == carNumbers[0]) && (minute <= a[7] + 5) && (a[6] != 0))
                    {BusCharging(a[6],1);
                    if(minute == a[7] + 5)
                        lightning[a[6]-1].SetActive(false);}
                if((a[8] != carNumbers[0]) && (minute <= a[9]) && (a[8] != 0))
                    {BusCharging(a[8],2);
                    if(minute == a[9])
                        lightning[a[8]-1].SetActive(false);}
                else if((a[8] == carNumbers[0]) && (minute <= a[9] + 5) && (a[8] != 0))
                    {BusCharging(a[8],2);
                    if(minute == a[9] + 5)
                        lightning[a[8]-1].SetActive(false);}
                if((a[10] != carNumbers[0]) && (minute <= a[11]) && (a[10] != 0))
                    {BusCharging(a[10],3);
                    if(minute == a[11])
                        lightning[a[10]-1].SetActive(false);}
                else if((a[10] == carNumbers[0]) && (minute <= a[11] + 5) && (a[10] != 0))
                    {BusCharging(a[10],3);
                    if(minute == a[11] + 5)
                        lightning[a[10]-1].SetActive(false);}
            }
            else if(minute >= 15)
            {
                if((a[6] != carNumbers[1]) && (minute <= 15 + a[7]) && (a[6] != 0))
                    {BusCharging(a[6],1);
                    if(minute == 15 + a[7])
                        lightning[a[6]-1].SetActive(false);}
                if((a[8] != carNumbers[1]) && (minute <= 15 + a[9]) && (a[8] != 0))
                    {BusCharging(a[8],2);
                    if(minute == 15 + a[9])
                        lightning[a[8]-1].SetActive(false);}
                if((a[10] != carNumbers[1]) && (minute <= 15 + a[11]) && (a[10] != 0))
                    {BusCharging(a[10],3);
                    if(minute == 15 + a[11])
                        lightning[a[10]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[1]-1] == 1)
                whereisBus[carNumbers[1]-1] = 2;
            if(whereisBus[carNumbers[1]-1] == 2)
                whereisBus[carNumbers[1]-1] = 1;
            checkBool = false;
        }
        if (hour==10)
        {
            if(minute <= 35)
            {
                if((a[12] != carNumbers[1]) && (minute <= a[13]) && (a[12] != 0))
                    {BusCharging(a[12],1);
                    if(minute == a[13])
                        lightning[a[12]-1].SetActive(false);}
                if((a[14] != carNumbers[1]) && (minute <= a[15]) && (a[14] != 0))
                    {BusCharging(a[14],2);
                    if(minute == a[15])
                        lightning[a[14]-1].SetActive(false);}
                if((a[16] != carNumbers[1]) && (minute <= a[17]) && (a[16] != 0))
                    {BusCharging(a[16],3);
                    if(minute == a[17])
                        lightning[a[16]-1].SetActive(false);}
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[12] != carNumbers[1]) && (minute <= a[13]) && (a[12] != 0))
                    {BusCharging(a[12],1);
                    if(minute == a[13])
                        lightning[a[12]-1].SetActive(false);}
                else if((a[12] == carNumbers[1]) && (minute <= a[13] + 35) && (a[12] != 0))
                    {BusCharging(a[12],1);
                    if(minute == a[13] + 35)
                        lightning[a[12]-1].SetActive(false);}
                if((a[14] != carNumbers[1]) && (minute <= a[15]) && (a[14] != 0))
                    {BusCharging(a[14],2);
                    if(minute == a[15])
                        lightning[a[14]-1].SetActive(false);}
                else if((a[14] == carNumbers[1]) && (minute <= a[15] + 35) && (a[14] != 0))
                    {BusCharging(a[14],2);
                    if(minute == a[15] + 35)
                        lightning[a[14]-1].SetActive(false);}
                if((a[16] != carNumbers[1]) && (minute <= a[17]) && (a[16] != 0))
                    {BusCharging(a[16],3);
                    if(minute == a[17])
                        lightning[a[16]-1].SetActive(false);}
                else if((a[16] == carNumbers[1]) && (minute <= a[17] + 35) && (a[16] != 0))
                    {BusCharging(a[16],3);
                    if(minute == a[17] + 35)
                        lightning[a[16]-1].SetActive(false);}
            }
            else if(minute >= 45)
            {
                if((a[12] != carNumbers[2]) && (minute <= 45 + a[13]) && (a[12] != 0))
                    {BusCharging(a[12],1);
                    if(minute == 45 + a[13])
                        lightning[a[12]-1].SetActive(false);}
                if((a[14] != carNumbers[2]) && (minute <= 45 + a[15]) && (a[14] != 0))
                    {BusCharging(a[14],2);
                    if(minute == 45 + a[15])
                        lightning[a[14]-1].SetActive(false);}
                if((a[16] != carNumbers[2]) && (minute <= 45 + a[17]) && (a[16] != 0))
                    {BusCharging(a[16],3);
                    if(minute == 45 + a[17])
                        lightning[a[16]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[2]-1] == 1)
                whereisBus[carNumbers[2]-1] = 2;
            if(whereisBus[carNumbers[2]-1] == 2)
                whereisBus[carNumbers[2]-1] = 1;
            checkBool = true;
        }
        if (hour==11)
        {
            if((a[18] != carNumbers[2]) && (minute <= a[19]) && (a[18] != 0))
                {BusCharging(a[18],1);
                if(minute == a[19])
                        lightning[a[18]-1].SetActive(false);}
            if((a[20] != carNumbers[2]) && (minute <= a[21]) && (a[20] != 0))
                {BusCharging(a[20],2);
                if(minute == a[21])
                        lightning[a[20]-1].SetActive(false);}
            if((a[22] != carNumbers[2]) && (minute <= a[23]) && (a[22] != 0))
                {BusCharging(a[22],3);
                if(minute == a[23])
                        lightning[a[22]-1].SetActive(false);}
        }
        if (hour==12)
        {
            if(minute <= 5)
            {
                if((a[24] != carNumbers[2]) && (minute <= a[25]) && (a[24] != 0))
                    {BusCharging(a[24],1);
                    if(minute == a[25])
                        lightning[a[24]-1].SetActive(false);}
                if((a[26] != carNumbers[2]) && (minute <= a[27]) && (a[26] != 0))
                    {BusCharging(a[26],2);
                    if(minute == a[27])
                        lightning[a[26]-1].SetActive(false);}
                if((a[28] != carNumbers[2]) && (minute <= a[29]) && (a[28] != 0))
                    {BusCharging(a[28],3);
                    if(minute == a[29])
                        lightning[a[28]-1].SetActive(false);}
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[24] != carNumbers[2]) && (minute <= a[25]) && (a[24] != 0))
                    {BusCharging(a[24],1);
                    if(minute == a[25])
                        lightning[a[24]-1].SetActive(false);}
                else if((a[24] == carNumbers[2]) && (minute <= a[25] + 5) && (a[24] != 0))
                    {BusCharging(a[24],1);
                    if(minute == a[25] + 5)
                        lightning[a[24]-1].SetActive(false);}
                if((a[26] != carNumbers[2]) && (minute <= a[27]) && (a[26] != 0))
                    {BusCharging(a[26],2);
                    if(minute == a[27])
                        lightning[a[26]-1].SetActive(false);}
                else if((a[26] == carNumbers[2]) && (minute <= a[27] + 5) && (a[26] != 0))
                    {BusCharging(a[26],2);
                    if(minute == a[27] + 5)
                        lightning[a[26]-1].SetActive(false);}
                if((a[28] != carNumbers[2]) && (minute <= a[29]) && (a[28] != 0))
                    {BusCharging(a[28],3);
                    if(minute == a[29])
                        lightning[a[28]-1].SetActive(false);}
                else if((a[28] == carNumbers[2]) && (minute <= a[29] + 5) && (a[28] != 0))
                    {BusCharging(a[28],3);
                    if(minute == a[29] + 5)
                        lightning[a[28]-1].SetActive(false);}
            }
            else if(minute >= 15)
            {
                if((a[24] != carNumbers[3]) && (minute <= 15 + a[25]) && (a[24] != 0))
                    {BusCharging(a[24],1);
                    if(minute == 15 + a[25])
                        lightning[a[24]-1].SetActive(false);}
                if((a[26] != carNumbers[3]) && (minute <= 15 + a[27]) && (a[26] != 0))
                    {BusCharging(a[26],2);
                    if(minute == 15 + a[27])
                        lightning[a[26]-1].SetActive(false);}
                if((a[28] != carNumbers[3]) && (minute <= 15 + a[29]) && (a[28] != 0))
                    {BusCharging(a[28],3);
                    if(minute == 15 + a[29])
                        lightning[a[28]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[3]-1] == 1)
                whereisBus[carNumbers[3]-1] = 2;
            if(whereisBus[carNumbers[3]-1] == 2)
                whereisBus[carNumbers[3]-1] = 1;
            checkBool = false;
        }
        if (hour==13)
        {
            if(minute <= 35)
            {
                if((a[30] != carNumbers[3]) && (minute <= a[31]) && (a[30] != 0))
                    {BusCharging(a[30],1);
                    if(minute == a[31])
                        lightning[a[30]-1].SetActive(false);}
                if((a[32] != carNumbers[3]) && (minute <= a[33]) && (a[32] != 0))
                    {BusCharging(a[32],2);
                    if(minute == a[33])
                        lightning[a[32]-1].SetActive(false);}
                if((a[34] != carNumbers[3]) && (minute <= a[35]) && (a[34] != 0))
                    {BusCharging(a[34],3);
                    if(minute == a[35])
                        lightning[a[34]-1].SetActive(false);}
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[30] != carNumbers[3]) && (minute <= a[31]) && (a[30] != 0))
                    {BusCharging(a[30],1);
                    if(minute == a[31])
                        lightning[a[30]-1].SetActive(false);}
                else if((a[30] == carNumbers[3]) && (minute <= a[31] + 35) && (a[30] != 0))
                    {BusCharging(a[30],1);
                    if(minute == a[31] + 35)
                        lightning[a[30]-1].SetActive(false);}
                if((a[32] != carNumbers[3]) && (minute <= a[33]) && (a[32] != 0))
                    {BusCharging(a[32],2);
                    if(minute == a[33])
                        lightning[a[32]-1].SetActive(false);}
                else if((a[32] == carNumbers[3]) && (minute <= a[33] + 35) && (a[32] != 0))
                    {BusCharging(a[32],2);
                    if(minute == a[33] + 35)
                        lightning[a[32]-1].SetActive(false);}
                if((a[34] != carNumbers[3]) && (minute <= a[35]) && (a[34] != 0))
                    {BusCharging(a[34],3);
                    if(minute == a[35])
                        lightning[a[34]-1].SetActive(false);}
                else if((a[34] == carNumbers[3]) && (minute <= a[35] + 35) && (a[34] != 0))
                    {BusCharging(a[34],3);
                    if(minute == a[35] + 35)
                        lightning[a[34]-1].SetActive(false);}
            }
            else if(minute >= 45)
            {
                if((a[30] != carNumbers[4]) && (minute <= 45 + a[31]) && (a[30] != 0))
                    {BusCharging(a[30],1);
                    if(minute == 45 + a[31])
                        lightning[a[30]-1].SetActive(false);}
                if((a[32] != carNumbers[4]) && (minute <= 45 + a[33]) && (a[32] != 0))
                    {BusCharging(a[32],2);
                    if(minute == 45 + a[33])
                        lightning[a[32]-1].SetActive(false);}
                if((a[34] != carNumbers[4]) && (minute <= 45 + a[35]) && (a[34] != 0))
                    {BusCharging(a[34],3);
                    if(minute == 45 + a[35])
                        lightning[a[34]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[4]-1] == 1)
                whereisBus[carNumbers[4]-1] = 2;
            if(whereisBus[carNumbers[4]-1] == 2)
                whereisBus[carNumbers[4]-1] = 1;
            checkBool = true;
        }
        if (hour==14)
        {
            if((a[36] != carNumbers[4]) && (minute <= a[37]) && (a[36] != 0))
                {BusCharging(a[36],1);
                if(minute == a[37])
                        lightning[a[36]-1].SetActive(false);}
            if((a[38] != carNumbers[4]) && (minute <= a[39]) && (a[38] != 0))
                {BusCharging(a[38],2);
                if(minute == a[39])
                        lightning[a[38]-1].SetActive(false);}
            if((a[40] != carNumbers[4]) && (minute <= a[41]) && (a[40] != 0))
                {BusCharging(a[40],3);
                if(minute == a[41])
                        lightning[a[40]-1].SetActive(false);}
        }
        if (hour==15)
        {
            if(minute <= 5)
            {
                if((a[42] != carNumbers[4]) && (minute <= a[43]) && (a[42] != 0))
                    {BusCharging(a[42],1);
                    if(minute == a[43])
                        lightning[a[42]-1].SetActive(false);}
                if((a[44] != carNumbers[4]) && (minute <= a[45]) && (a[44] != 0))
                    {BusCharging(a[44],1);
                    if(minute == a[45])
                        lightning[a[44]-1].SetActive(false);}
                if((a[46] != carNumbers[4]) && (minute <= a[47]) && (a[46] != 0))
                    {BusCharging(a[46],1);
                    if(minute == a[47])
                        lightning[a[46]-1].SetActive(false);}
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[42] != carNumbers[3]) && (minute <= a[43]) && (a[42] != 0))
                    {BusCharging(a[42],1);
                    if(minute == a[43])
                        lightning[a[42]-1].SetActive(false);}
                else if((a[42] == carNumbers[3]) && (minute <= a[43] + 5) && (a[42] != 0))
                    {BusCharging(a[42],1);
                    if(minute == a[43] + 5)
                        lightning[a[42]-1].SetActive(false);}
                if((a[44] != carNumbers[3]) && (minute <= a[45]) && (a[44] != 0))
                    {BusCharging(a[44],1);
                    if(minute == a[45])
                        lightning[a[44]-1].SetActive(false);}
                else if((a[44] == carNumbers[3]) && (minute <= a[45] + 5) && (a[44] != 0))
                    {BusCharging(a[44],1);
                    if(minute == a[45] + 5)
                        lightning[a[44]-1].SetActive(false);}
                if((a[46] != carNumbers[3]) && (minute <= a[47]) && (a[46] != 0))
                    {BusCharging(a[46],1);
                    if(minute == a[47])
                        lightning[a[46]-1].SetActive(false);}
                else if((a[46] == carNumbers[3]) && (minute <= a[47] + 5) && (a[46] != 0))
                    {BusCharging(a[46],1);
                    if(minute == a[47] + 5)
                        lightning[a[46]-1].SetActive(false);}
            }
            else if(minute >= 15)
            {
                if((a[42] != carNumbers[5]) && (minute <= 15 + a[43]) && (a[42] != 0))
                    {BusCharging(a[42],1);
                    if(minute == 15 + a[43])
                        lightning[a[42]-1].SetActive(false);}
                if((a[44] != carNumbers[5]) && (minute <= 15 + a[45]) && (a[44] != 0))
                    {BusCharging(a[44],1);
                    if(minute == 15 + a[45])
                        lightning[a[44]-1].SetActive(false);}
                if((a[46] != carNumbers[5]) && (minute <= 15 + a[47]) && (a[46] != 0))
                    {BusCharging(a[46],1);
                    if(minute == 15 + a[47])
                        lightning[a[46]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[5]-1] == 1)
                whereisBus[carNumbers[5]-1] = 2;
            if(whereisBus[carNumbers[5]-1] == 2)
                whereisBus[carNumbers[5]-1] = 1;
            checkBool = false;
        }
        if (hour==16)
        {
            if(minute <= 35)
            {
                if((a[48] != carNumbers[5]) && (minute <= a[49]) && (a[48] != 0))
                    {BusCharging(a[48],1);
                    if(minute == a[49])
                        lightning[a[48]-1].SetActive(false);}
                if((a[50] != carNumbers[5]) && (minute <= a[51]) && (a[50] != 0))
                    {BusCharging(a[50],2);
                    if(minute == a[51])
                        lightning[a[50]-1].SetActive(false);}
                if((a[52] != carNumbers[5]) && (minute <= a[53]) && (a[52] != 0))
                    {BusCharging(a[52],3);
                    if(minute == a[53])
                        lightning[a[52]-1].SetActive(false);}
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[48] != carNumbers[5]) && (minute <= a[49]) && (a[48] != 0))
                    {BusCharging(a[48],1);
                    if(minute == a[49])
                        lightning[a[48]-1].SetActive(false);}
                else if((a[48] == carNumbers[5]) && (minute <= a[49] + 35) && (a[48] != 0))
                    {BusCharging(a[48],1);
                    if(minute == a[49] + 35)
                        lightning[a[48]-1].SetActive(false);}
                if((a[50] != carNumbers[5]) && (minute <= a[51]) && (a[50] != 0))
                    {BusCharging(a[50],2);
                    if(minute == a[51])
                        lightning[a[50]-1].SetActive(false);}
                else if((a[50] == carNumbers[5]) && (minute <= a[51] + 35) && (a[50] != 0))
                    {BusCharging(a[50],2);
                    if(minute == a[51] + 35)
                        lightning[a[50]-1].SetActive(false);}
                if((a[52] != carNumbers[5]) && (minute <= a[53]) && (a[52] != 0))
                    {BusCharging(a[52],3);
                    if(minute == a[53])
                        lightning[a[52]-1].SetActive(false);}
                else if((a[52] == carNumbers[5]) && (minute <= a[53] + 35) && (a[52] != 0))
                    {BusCharging(a[52],3);
                    if(minute == a[53 + 35])
                        lightning[a[52]-1].SetActive(false);}
            }
            else if(minute >= 45)
            {
                if((a[48] != carNumbers[6]) && (minute <= 45 + a[49]) && (a[48] != 0))
                    {BusCharging(a[48],1);
                    if(minute == 45 + a[49])
                        lightning[a[48]-1].SetActive(false);}
                if((a[50] != carNumbers[6]) && (minute <= 45 + a[51]) && (a[50] != 0))
                    {BusCharging(a[50],2);
                    if(minute == 45 + a[51])
                        lightning[a[50]-1].SetActive(false);}
                if((a[52] != carNumbers[6]) && (minute <= 45 + a[53]) && (a[52] != 0))
                    {BusCharging(a[52],3);
                    if(minute == 45 + a[53])
                        lightning[a[52]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[6]-1] == 1)
                whereisBus[carNumbers[6]-1] = 2;
            if(whereisBus[carNumbers[6]-1] == 2)
                whereisBus[carNumbers[6]-1] = 1;
            checkBool = true;
        }
        if (hour==17)
        {
            if((a[54] != carNumbers[6]) && (minute <= a[55]) && (a[54] != 0))
                {BusCharging(a[54],1);
                if(minute == a[55])
                        lightning[a[54]-1].SetActive(false);}
            if((a[56] != carNumbers[6]) && (minute <= a[57]) && (a[56] != 0))
                {BusCharging(a[56],2);
                if(minute == a[57])
                        lightning[a[56]-1].SetActive(false);}
            if((a[58] != carNumbers[6]) && (minute <= a[59]) && (a[58] != 0))
                {BusCharging(a[58],3);
                if(minute == a[59])
                        lightning[a[58]-1].SetActive(false);}
        }
        if (hour==18)
        {
            if(minute <= 5)
            {
                if((a[60] != carNumbers[6]) && (minute <= a[61]) && (a[60] != 0))
                    {BusCharging(a[60],1);
                    if(minute == a[61])
                        lightning[a[60]-1].SetActive(false);}
                if((a[62] != carNumbers[6]) && (minute <= a[63]) && (a[62] != 0))
                    {BusCharging(a[62],2);
                    if(minute == a[63])
                        lightning[a[62]-1].SetActive(false);}
                if((a[64] != carNumbers[6]) && (minute <= a[65]) && (a[64] != 0))
                    {BusCharging(a[64],3);
                    if(minute == a[65])
                        lightning[a[64]-1].SetActive(false);}
            }
            else if((minute > 5)&&(minute<15))
            {
                if((a[60] != carNumbers[6]) && (minute <= a[61]) && (a[60] != 0))
                    {BusCharging(a[60],1);
                    if(minute == a[61])
                        lightning[a[60]-1].SetActive(false);}
                else if((a[60] == carNumbers[6]) && (minute <= a[61] + 5) && (a[60] != 0))
                    {BusCharging(a[60],1);
                    if(minute == a[61] + 5)
                        lightning[a[60]-1].SetActive(false);}
                if((a[62] != carNumbers[6]) && (minute <= a[63]) && (a[62] != 0))
                    {BusCharging(a[62],2);
                    if(minute == a[63])
                        lightning[a[62]-1].SetActive(false);}
                else if((a[62] == carNumbers[6]) && (minute <= a[63] + 5) && (a[62] != 0))
                    {BusCharging(a[62],2);
                    if(minute == a[63] + 5)
                        lightning[a[62]-1].SetActive(false);}
                if((a[64] != carNumbers[6]) && (minute <= a[65]) && (a[64] != 0))
                    {BusCharging(a[64],3);
                    if(minute == a[65])
                        lightning[a[64]-1].SetActive(false);}
                else if((a[64] == carNumbers[6]) && (minute <= a[65] + 5) && (a[64] != 0))
                    {BusCharging(a[64],3);
                    if(minute == a[65 + 5])
                        lightning[a[64]-1].SetActive(false);}
            }
            else if(minute >= 15)
            {
                if((a[60] != carNumbers[7]) && (minute <= 15 + a[61]) && (a[60] != 0))
                    {BusCharging(a[60],1);
                    if(minute == 15 + a[61])
                        lightning[a[60]-1].SetActive(false);}
                if((a[62] != carNumbers[7]) && (minute <= 15 + a[63]) && (a[62] != 0))
                    {BusCharging(a[62],2);
                    if(minute == 15 + a[63])
                        lightning[a[62]-1].SetActive(false);}
                if((a[64] != carNumbers[7]) && (minute <= 15 + a[65]) && (a[64] != 0))
                    {BusCharging(a[64],3);
                    if(minute == 15 + a[65])
                        lightning[a[64]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[7]-1] == 1)
                whereisBus[carNumbers[7]-1] = 2;
            if(whereisBus[carNumbers[7]-1] == 2)
                whereisBus[carNumbers[7]-1] = 1;
            checkBool = false;
        }
        if (hour==19)
        {
            if(minute <= 35)
            {
                if((a[66] != carNumbers[7]) && (minute <= a[67]) && (a[66] != 0))
                    {BusCharging(a[66],1);
                    if(minute == a[67])
                        lightning[a[66]-1].SetActive(false);}
                if((a[68] != carNumbers[7]) && (minute <= a[69]) && (a[68] != 0))
                    {BusCharging(a[68],1);
                    if(minute == a[69])
                        lightning[a[68]-1].SetActive(false);}
                if((a[70] != carNumbers[7]) && (minute <= a[71]) && (a[70] != 0))
                    {BusCharging(a[70],3);
                    if(minute == a[71])
                        lightning[a[70]-1].SetActive(false);}
            }
            else if((minute > 35)&&(minute<45))
            {
                if((a[66] != carNumbers[7]) && (minute <= a[67]) && (a[66] != 0))
                    {BusCharging(a[66],1);
                    if(minute == a[67])
                        lightning[a[66]-1].SetActive(false);}
                else if((a[66] == carNumbers[7]) && (minute <= a[67] + 35) && (a[66] != 0))
                    {BusCharging(a[66],1);
                    if(minute == a[67] + 35)
                        lightning[a[66]-1].SetActive(false);}
                if((a[68] != carNumbers[7]) && (minute <= a[69]) && (a[68] != 0))
                    {BusCharging(a[68],1);
                    if(minute == a[69])
                        lightning[a[68]-1].SetActive(false);}
                else if((a[68] == carNumbers[7]) && (minute <= a[69] + 35) && (a[68] != 0))
                    {BusCharging(a[68],1);
                    if(minute == a[69] + 35)
                        lightning[a[68]-1].SetActive(false);}
                if((a[70] != carNumbers[7]) && (minute <= a[71]) && (a[70] != 0))
                    {BusCharging(a[70],3);
                    if(minute == a[71])
                        lightning[a[70]-1].SetActive(false);}
                else if((a[70] == carNumbers[7]) && (minute <= a[71] + 35) && (a[70] != 0))
                    {BusCharging(a[70],3);
                    if(minute == a[71 + 35])
                        lightning[a[70]-1].SetActive(false);}
            }
            else if(minute >= 45)
            {
                if((a[66] != carNumbers[8]) && (minute <= 45 + a[67]) && (a[66] != 0))
                    {BusCharging(a[66],1);
                    if(minute == 45 + a[67])
                        lightning[a[66]-1].SetActive(false);}
                if((a[68] != carNumbers[8]) && (minute <= 45 + a[69]) && (a[68] != 0))
                    {BusCharging(a[68],1);
                    if(minute == 45 + a[69])
                        lightning[a[68]-1].SetActive(false);}
                if((a[70] != carNumbers[8]) && (minute <= 45 + a[71]) && (a[70] != 0))
                    {BusCharging(a[70],3);
                    if(minute == 45 + a[71])
                        lightning[a[70]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[8]-1] == 1)
                whereisBus[carNumbers[8]-1] = 2;
            if(whereisBus[carNumbers[8]-1] == 2)
                whereisBus[carNumbers[8]-1] = 1;
            checkBool = true;
        }
        if (hour==20)
        {
            if((a[72] != carNumbers[8]) && (minute <= a[73]) && (a[72] != 0))
                {BusCharging(a[72],1);
                if(minute == a[73])
                        lightning[a[72]-1].SetActive(false);}
            if((a[74] != carNumbers[8]) && (minute <= a[75]) && (a[74] != 0))
                {BusCharging(a[74],2);
                if(minute == a[75])
                        lightning[a[74]-1].SetActive(false);}
            if((a[76] != carNumbers[8]) && (minute <= a[77]) && (a[76] != 0))
                {BusCharging(a[76],3);
                if(minute == a[77])
                        lightning[a[76]-1].SetActive(false);}
        }
        if (hour==21)
        {
            if(minute <= 5)
            {
                if((a[78] != carNumbers[8]) && (minute <= a[79]) && (a[78] != 0))
                    {BusCharging(a[78],1);
                    if(minute == a[79])
                        lightning[a[78]-1].SetActive(false);}
                if((a[80] != carNumbers[8]) && (minute <= a[81]) && (a[80] != 0))
                    {BusCharging(a[80],2);
                    if(minute == a[81])
                        lightning[a[80]-1].SetActive(false);}
                if((a[82] != carNumbers[8]) && (minute <= a[83]) && (a[82] != 0))
                    {BusCharging(a[82],3);
                    if(minute == a[83])
                        lightning[a[82]-1].SetActive(false);}
            }
            else if((minute > 5)&&(minute<10))
            {
                if((a[78] != carNumbers[8]) && (minute <= a[79]) && (a[78] != 0))
                    {BusCharging(a[78],1);
                    if(minute == a[79])
                        lightning[a[78]-1].SetActive(false);}
                else if((a[78] == carNumbers[8]) && (minute <= a[79] + 5) && (a[78] != 0))
                    {BusCharging(a[78],1);
                    if(minute == a[79] + 5)
                        lightning[a[78]-1].SetActive(false);}
                if((a[80] != carNumbers[8]) && (minute <= a[81]) && (a[80] != 0))
                    {BusCharging(a[80],2);
                    if(minute == a[81])
                        lightning[a[80]-1].SetActive(false);}
                else if((a[80] == carNumbers[8]) && (minute <= a[81] + 5) && (a[80] != 0))
                    {BusCharging(a[80],2);
                    if(minute == a[81] + 5)
                        lightning[a[80]-1].SetActive(false);}
                if((a[82] != carNumbers[8]) && (minute <= a[83]) && (a[82] != 0))
                    {BusCharging(a[82],3);
                    if(minute == a[83])
                        lightning[a[82]-1].SetActive(false);}
                else if((a[82] == carNumbers[8]) && (minute <= a[83] + 5) && (a[82] != 0))
                    {BusCharging(a[82],3);
                    if(minute == a[83] + 5)
                        lightning[a[82]-1].SetActive(false);}
            }
            else if(minute >= 10)
            {
                if((a[78] != carNumbers[9]) && (minute <= 10 + a[79]) && (a[78] != 0))
                    {BusCharging(a[78],1);
                    if(minute == 10 + a[79])
                        lightning[a[78]-1].SetActive(false);}
                if((a[80] != carNumbers[9]) && (minute <= 10 + a[81]) && (a[80] != 0))
                    {BusCharging(a[80],2);
                    if(minute == 10 + a[81])
                        lightning[a[80]-1].SetActive(false);}
                if((a[82] != carNumbers[9]) && (minute <= 10 + a[83]) && (a[82] != 0))
                    {BusCharging(a[82],3);
                    if(minute == 10 + a[83])
                        lightning[a[82]-1].SetActive(false);}
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
            if(whereisBus[carNumbers[9]-1] == 1)
                whereisBus[carNumbers[9]-1] = 2;
            if(whereisBus[carNumbers[9]-1] == 2)
                whereisBus[carNumbers[9]-1] = 1;
            checkBool = false;
        }
        if (hour==22)
        {
            if(minute <= 30)
            {
                if((a[84] != carNumbers[9]) && (minute <= a[85]) && (a[84] != 0))
                    {BusCharging(a[84],1);
                    if(minute == a[85])
                        lightning[a[84]-1].SetActive(false);}
                if((a[86] != carNumbers[9]) && (minute <= a[87]) && (a[86] != 0))
                    {BusCharging(a[86],2);
                    if(minute == a[87])
                        lightning[a[86]-1].SetActive(false);}
                if((a[88] != carNumbers[9]) && (minute <= a[89]) && (a[88] != 0))
                    {BusCharging(a[88],3);
                    if(minute == a[89])
                        lightning[a[88]-1].SetActive(false);}
            }
            else if(minute >= 30)
            {
                if((a[84] != carNumbers[10]) && (minute <= 30 + a[85]) && (a[84] != 0))
                    {BusCharging(a[84],1);
                    if(minute == 30 + a[85])
                        lightning[a[84]-1].SetActive(false);}
                if((a[86] != carNumbers[10]) && (minute <= 30 + a[87]) && (a[86] != 0))
                    {BusCharging(a[86],2);
                    if(minute == 30 + a[87])
                        lightning[a[86]-1].SetActive(false);}
                if((a[88] != carNumbers[10]) && (minute <= 30 + a[89]) && (a[88] != 0))
                    {BusCharging(a[88],3);
                    if(minute == 30 + a[89])
                        lightning[a[88]-1].SetActive(false);}
            }
        }
        if (hour == 22 && minute == 30 && checkBool == false)
        {
            rawImages[9].color = greenColor;
            goingBool[carNumbers[9]-1] = false;

            GameObject canvasInstance = Instantiate(busses[carNumbers[10]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[10].color = yellowColor;
            goingBool[carNumbers[10]-1] = true;
            if(whereisBus[carNumbers[10]-1] == 1)
                whereisBus[carNumbers[10]-1] = 2;
            if(whereisBus[carNumbers[10]-1] == 2)
                whereisBus[carNumbers[10]-1] = 1;
            
            checkBool = true;
        }
        if (hour == 23 && minute == 50)
        {
            rawImages[10].color = greenColor;
            goingBool[carNumbers[10]-1] = false;
            ended.SetActive(true);
            carbonText2.text = carbonText.text;
            priceText2.text = priceText.text;
            for (int i = 0; i < busCharges.Length; i++)
            {
                totalCharge += busCharges[i];
            }
            totalCharge /= 8;
            avgBusCharge.text = totalCharge.ToString();
            UpdateHighScores();
        }
    }
    public void UpdateHighScores()
    {
        // Yeni değerler alınır
        float carbonValue = float.Parse(carbonText2.text);
        float priceValue = float.Parse(priceText2.text);
        float avgCharge = float.Parse(avgBusCharge.text);

        // Karbon değeri kontrol edilir
        if (carbonValue < minCarbonValue)
        {
            minCarbonValue = carbonValue;
            carbonText3.text = minCarbonValue.ToString();
            PlayerPrefs.SetFloat("MinCarbonValue", minCarbonValue);
        }

        // Fiyat değeri kontrol edilir
        if (priceValue < minPriceValue)
        {
            minPriceValue = priceValue;
            priceText3.text = minPriceValue.ToString();
            PlayerPrefs.SetFloat("MinPriceValue", minPriceValue);
        }

        // Ortalama şarj değeri kontrol edilir
        if (avgCharge > minAvgBusCharge)
        {
            minAvgBusCharge = avgCharge;
            avgBusCharge2.text = minAvgBusCharge.ToString();
            PlayerPrefs.SetFloat("MinAvgBusCharge", minAvgBusCharge);
        }
    }
}
