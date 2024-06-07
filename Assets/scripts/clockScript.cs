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
    public bool checkBool;
    public bool[] goingBool = new bool[8];
    public GameObject parentObject;
    public RectTransform spawnRectTransform;
    public int hour;
    private int minute;
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
    public GameObject[] instantiatedBusses;
    public float totalCharge = 0;
    public int speedMultiplier = 1; 
    private float lastSpeedChangeTime;
    private float accumulatedGameTime;
    public float speedCanva;
    public TextMeshProUGUI speedX;

    private float minCarbonValue = float.MaxValue;
    private float minPriceValue = float.MaxValue;
    public float maxAvgBusCharge = float.MinValue;
    public bool[] startChargePlayed = new bool[105]; // 24 saat için başlangıç animasyonu kontrolü
    public bool[] endChargePlayed = new bool[105]; // 24 saat için 3 otobüsün bitiş animasyonu kontrolü
    public bool[] startChargePlayed2 = new bool[60]; // 24 saat için başlangıç animasyonu kontrolü
    public bool[] endChargePlayed2 = new bool[60]; // 24 saat için 3 otobüsün bitiş animasyonu kontrolü
    public GameObject GameOver;
    void Start()
    {
        checkBool = false;
        respawnPositionLeft = new Vector3(-267f, 121f, 0f);
        respawnPositionRight = new Vector3(267f, 180f, 0f);
        instantiatedBusses = new GameObject[6];
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
        for (int i = 0; i < startChargePlayed.Length; i++)
        {
            startChargePlayed[i] = false;
            endChargePlayed[i] = false;
        }
        for (int i = 0; i < startChargePlayed2.Length; i++)
        {
            startChargePlayed2[i] = false;
            endChargePlayed2[i] = false;
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
        speedX.text = speedMultiplier.ToString();
        maxAvgBusCharge = PlayerPrefs.GetFloat("maxAvgBusCharge", float.MinValue);
        if (!PlayerPrefs.HasKey("Firsttt"))
        {
            PlayerPrefs.SetInt("Firsttt", 0);
            PlayerPrefs.SetFloat("maxAvgBusCharge", 0);
            PlayerPrefs.Save();
        }
        else
        {
            maxAvgBusCharge = PlayerPrefs.GetFloat("maxAvgBusCharge");
        }
    }

    void Update()
    {
        float currentTime = Time.realtimeSinceStartup;
        float deltaTime = currentTime - lastSpeedChangeTime;
        accumulatedGameTime += deltaTime * timeMultiplier * speedMultiplier;
        lastSpeedChangeTime = currentTime;
        UpdateClock(accumulatedGameTime);
        busChargeFunc();
        After14();
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
                if(busCharges[0]==0)
                {
                    GameOver.SetActive(true);
                }
            }
            else if(goingBool[1])
            {
                busCharges[1] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[1].text = busCharges[1].ToString();
                if(busCharges[1]==0)
                {
                    GameOver.SetActive(true);
                }
            }
            else if(goingBool[2])
            {
                busCharges[2] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[2].text = busCharges[2].ToString();
                if(busCharges[2]==0)
                {
                    GameOver.SetActive(true);
                }
            }
            else if(goingBool[3])
            {
                busCharges[3] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[3].text = busCharges[3].ToString();
                if(busCharges[3]==0)
                {
                    GameOver.SetActive(true);
                }
            }
            else if(goingBool[4])
            {
                busCharges[4] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[4].text = busCharges[4].ToString();
                if(busCharges[4]==0)
                {
                    GameOver.SetActive(true);
                }
            }
            else if(goingBool[5])
            {
                busCharges[5] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[5].text = busCharges[5].ToString();
                if(busCharges[5]==0)
                {
                    GameOver.SetActive(true);
                }
            }
            else if(goingBool[6])
            {
                busCharges[6] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[6].text = busCharges[6].ToString();
                if(busCharges[6]==0)
                {
                    GameOver.SetActive(true);
                }
            }
            else if(goingBool[7])
            {
                busCharges[7] -= 1;
                timer = 0f; // Zamanlayıcıyı sıfırla
                chargeTexts[7].text = busCharges[7].ToString();
                if(busCharges[7]==0)
                {
                    GameOver.SetActive(true);
                }
            }
        }
        }
    }

    void BusCharge2(int busNum, int whichTimer)
    {
        if(whichTimer == 1)
        {
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
    void BusCharging(int busNum, int whichTimer, int StartCheck)
    {
        if(StartCheck <= 71)
        {
        if(!startChargePlayed[StartCheck])
        {
        startChargePlayed[StartCheck] = true;
        if(whichTimer == 1)
        {
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(1,0);
            if(whereisBus[busNum-1]==2)
                ChargeAnimation(1,2);
            lightning[busNum-1].SetActive(true);
        }
        if(whichTimer == 2)
        {
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(2,0);
            if(whereisBus[busNum-1]==2)
                ChargeAnimation(2,2);
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
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(3,0);
            if(whereisBus[busNum-1]==2)
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
        }
        else if(StartCheck >= 72)
        {
        if(!startChargePlayed2[StartCheck-72])
        {
        startChargePlayed2[StartCheck-72] = true;
        if(whichTimer == 1)
        {
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(1,0);
            if(whereisBus[busNum-1]==2)
                ChargeAnimation(1,2);
            lightning[busNum-1].SetActive(true);
        }
        if(whichTimer == 2)
        {
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(2,0);
            if(whereisBus[busNum-1]==2)
                ChargeAnimation(2,2);
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
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(3,0);
            if(whereisBus[busNum-1]==2)
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
        }
    }

    void ChargeFinished(int busNum, int whichNum, int finishedCheck)
    {
        if(finishedCheck <= 71){
            if(!endChargePlayed[finishedCheck])
        {
            endChargePlayed[finishedCheck] = true;
            lightning[busNum-1].SetActive(false);
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(whichNum,1);
            if(whereisBus[busNum-1]==2)
                ChargeAnimation(whichNum,3);
        }
        }
        if(finishedCheck >= 72){
            if(!endChargePlayed2[finishedCheck-72])
        {
            endChargePlayed2[finishedCheck-72] = true;
            lightning[busNum-1].SetActive(false);
            if(whereisBus[busNum-1]==1)
                ChargeAnimation(whichNum,1);
            if(whereisBus[busNum-1]==2)
                ChargeAnimation(whichNum,3);
        }
        }
        
    }

    void ChargeAnimation(int whichPark, int SabiKadiGirisCikis){
        string isim1="kadisarj";
        string isim2="kadicikis";
        string isim3="sabisarj";
        string isim4="sabicikis";
        if(SabiKadiGirisCikis == 0){
            string animadi = isim3 + whichPark.ToString();
            Debug.Log(whichPark-1+"a girdi");
            CreateObjectAndPlayAnimation(animadi,whichPark-1);
        }
        else if(SabiKadiGirisCikis == 1){
            string animadi = isim4 + whichPark.ToString();
            Debug.Log(whichPark-1+"den çıktı");
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
    void After14()
    {
        if (hour == 14)
        {
            if((a[36] != carNumbers[4]) && (minute <= a[37]) && (a[36] != 0))
            {
                BusCharging(a[36],1,42);
                BusCharge2(a[36],1);
                if(minute == a[37])
                    ChargeFinished(a[36],1,42);
            }
            if((a[38] != carNumbers[4]) && (minute <= a[39]) && (a[38] != 0))
            {
                BusCharging(a[38],2,43);
                BusCharge2(a[38],2);
                if(minute == a[39])
                    ChargeFinished(a[38],2,43);
            }
            if((a[40] != carNumbers[4]) && (minute <= a[41]) && (a[40] != 0))
            {
                BusCharging(a[40],3,44);
                BusCharge2(a[40],3);
                if(minute == a[41])
                    ChargeFinished(a[40],3,44);
            }
        }
        if (hour==15)
        {
            if(minute <= 5)
            {
                if(minute == 5)
                {
                    rawImages[4].color = greenColor;
                    goingBool[carNumbers[4]-1] = false;
                    whereisBus[carNumbers[4]-1] = 2;
                }
                if((a[42] != carNumbers[4]) && (minute <= a[43]) && (a[42] != 0))
                    {BusCharging(a[42],1,45);
                BusCharge2(a[42],1);
                    if(minute == a[43])
                        ChargeFinished(a[42],1,45);}
                if((a[44] != carNumbers[4]) && (minute <= a[45]) && (a[44] != 0))
                    {BusCharging(a[44],2,46);
                BusCharge2(a[44],2);
                    if(minute == a[45])
                        ChargeFinished(a[44],2,46);}
                if((a[46] != carNumbers[4]) && (minute <= a[47]) && (a[46] != 0))
                    {BusCharging(a[46],3,47);
                BusCharge2(a[46],3);
                    if(minute == a[47])
                        ChargeFinished(a[46],3,47);}
            }
            else if((minute > 5)&&(minute<=15))
            {
                if(minute == 15 && checkBool == true)
                {
                    GameObject canvasInstance = Instantiate(busses[carNumbers[5]-1], parentObject.transform);
                    spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
                    spawnRectTransform.anchoredPosition = respawnPositionLeft;
                    rawImages[5].color = yellowColor;
                    goingBool[carNumbers[5]-1] = true;
                    checkBool = false;
                }
                if((a[42] != carNumbers[4]) && (minute <= a[43]) && (a[42] != 0))
                    {BusCharging(a[42],1,48);
                BusCharge2(a[42],1);
                    if(minute == a[43])
                        ChargeFinished(a[42],1,48);}
                else if((a[42] == carNumbers[4]) && (minute <= a[43] + 5) && (a[42] != 0))
                    {BusCharging(a[42],1,48);
                BusCharge2(a[42],1);
                    if(minute == a[43] + 5)
                        ChargeFinished(a[42],1,48);}
                if((a[44] != carNumbers[4]) && (minute <= a[45]) && (a[44] != 0))
                    {BusCharging(a[44],2,49);
                BusCharge2(a[44],2);
                    if(minute == a[45])
                        ChargeFinished(a[44],2,49);}
                else if((a[44] == carNumbers[4]) && (minute <= a[45] + 5) && (a[44] != 0))
                    {BusCharging(a[44],2,49);
                BusCharge2(a[44],2);
                    if(minute == a[45] + 5)
                        ChargeFinished(a[44],2,49);}
                if((a[46] != carNumbers[4]) && (minute <= a[47]) && (a[46] != 0))
                    {BusCharging(a[46],3,50);
                BusCharge2(a[46],3);
                    if(minute == a[47])
                        ChargeFinished(a[46],3,50);}
                else if((a[46] == carNumbers[4]) && (minute <= a[47] + 5) && (a[46] != 0))
                    {BusCharging(a[46],3,50);
                BusCharge2(a[46],3);
                    if(minute == a[47] + 5)
                        ChargeFinished(a[46],3,50);}
            }

            else if(minute > 15)
            {
                if((a[42] == carNumbers[4]) && (minute <= a[43] + 5) && (a[42] != 0))
                    {BusCharging(a[42],1,51);
                BusCharge2(a[42],1);
                    if(minute == a[43] + 5)
                        ChargeFinished(a[42],1,51);}
                else if((a[42] == carNumbers[5]) && (a[42] != 0))
                {
                    ChargeFinished(a[42],1,114);
                }
                else if((a[42] != carNumbers[5]) && (minute <= a[43]) && (a[42] != 0))
                    {BusCharging(a[42],1,51);
                BusCharge2(a[42],1);
                    if(minute == a[43])
                        ChargeFinished(a[42],1,51);}
                if((a[44] == carNumbers[4]) && (minute <= a[45] + 5) && (a[44] != 0))
                    {BusCharging(a[44],2,52);
                BusCharge2(a[44],2);
                    if(minute == a[45] + 5)
                        ChargeFinished(a[44],2,52);}
                else if((a[44] == carNumbers[5]) && (a[44] != 0))
                {
                    ChargeFinished(a[44],2,115);
                }
                else if((a[44] != carNumbers[5]) && (minute <= a[45]) && (a[44] != 0))
                    {BusCharging(a[44],2,52);
                BusCharge2(a[44],2);
                    if(minute == a[45])
                        ChargeFinished(a[44],2,52);}
                if((a[46] == carNumbers[4]) && (minute <= a[47] + 5) && (a[46] != 0))
                    {BusCharging(a[46],3,53);
                BusCharge2(a[46],3);
                    if(minute == a[47] + 5)
                        ChargeFinished(a[46],3,53);}
                else if((a[46] == carNumbers[5]) && (a[46] != 0))
                {
                    ChargeFinished(a[46],3,116);
                }
                else if((a[46] != carNumbers[5]) && (minute <= a[47]) && (a[46] != 0))
                    {BusCharging(a[46],3,53);
                BusCharge2(a[46],3);
                    if(minute == a[47])
                        ChargeFinished(a[46],3,53);}
            }
        }
    }
    void UpdateClock(float gameTime)
    {
        hour = (int)(gameTime / 3600f) % 24; // Saati 24 saat formatına dönüştürmek için mod işlemi uygulandı
        minute = (int)((gameTime / 60f) % 60);

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
                {BusCharging(a[0],1,0);
                BusCharge2(a[0],1);
                if(minute == a[1])
                    ChargeFinished(a[0],1,0);}
            if((a[2] != carNumbers[0]) && (minute <= a[3]) && (a[2] != 0))
                {BusCharging(a[2],2,1);
                BusCharge2(a[2],2);
                if(minute == a[3])
                    ChargeFinished(a[2],2,1);}
            if((a[4] != carNumbers[0]) && (minute <= a[5]) && (a[4] != 0))
                {BusCharging(a[4],3,2);
                BusCharge2(a[4],3);
                if(minute == a[5])
                    ChargeFinished(a[4],3,2);}
        }
        if (hour==9)
        {
            if(minute <= 5)
            {
                if((a[6] != carNumbers[0]) && (minute <= a[7]) && (a[6] != 0))
                    {BusCharging(a[6],1,3);
                     BusCharge2(a[6],1);
                    if(minute == a[7])
                        ChargeFinished(a[6],1,3);}
                if((a[8] != carNumbers[0]) && (minute <= a[9]) && (a[8] != 0))
                    {BusCharging(a[8],2,4);
                    BusCharge2(a[8],2);
                    if(minute == a[9])
                        ChargeFinished(a[8],2,4);}
                if((a[10] != carNumbers[0]) && (minute <= a[11]) && (a[10] != 0))
                    {BusCharging(a[10],3,5);
                    BusCharge2(a[10],3);
                    if(minute == a[11])
                        ChargeFinished(a[10],3,5);}
            }
            else if((minute > 5)&&(minute<=15))
            {
                if((a[6] != carNumbers[0]) && (minute <= a[7]) && (a[6] != 0))
                    {BusCharging(a[6],1,6);
                    BusCharge2(a[6],1);
                    if(minute == a[7])
                        ChargeFinished(a[6],1,6);}
                else if((a[6] == carNumbers[0]) && (minute <= a[7] + 5) && (a[6] != 0))
                    {BusCharging(a[6],1,6);
                    BusCharge2(a[6],1);
                    if(minute == a[7] + 5)
                        ChargeFinished(a[6],1,6);}
                if((a[8] != carNumbers[0]) && (minute <= a[9]) && (a[8] != 0))
                    {BusCharging(a[8],2,7);
                    BusCharge2(a[8],2);
                    if(minute == a[9])
                        ChargeFinished(a[8],2,7);}
                else if((a[8] == carNumbers[0]) && (minute <= a[9] + 5) && (a[8] != 0))
                    {BusCharging(a[8],2,7);
                    BusCharge2(a[8],2);
                    if(minute == a[9] + 5)
                        ChargeFinished(a[8],2,7);}
                if((a[10] != carNumbers[0]) && (minute <= a[11]) && (a[10] != 0))
                    {BusCharging(a[10],3,8);
                    BusCharge2(a[10],3);
                    if(minute == a[11])
                        ChargeFinished(a[10],3,8);}
                else if((a[10] == carNumbers[0]) && (minute <= a[11] + 5) && (a[10] != 0))
                    {BusCharging(a[10],3,8);
                    BusCharge2(a[10],3);
                    if(minute == a[11] + 5)
                        ChargeFinished(a[10],3,8);}
            }
            else if(minute > 15)
            {
                if((a[6] == carNumbers[0]) && (minute <= a[7] + 5) && (a[6] != 0))
                    {BusCharging(a[6],1,9);
                    BusCharge2(a[6],1);
                    if(minute == a[7] + 5)
                        ChargeFinished(a[6],1,9);}
                else if((a[6] == carNumbers[1]) && (a[6] != 0))
                {
                    ChargeFinished(a[6],1,102);
                }
                else if((a[6] != carNumbers[1]) && (minute <= a[7]) && (a[6] != 0))
                    {BusCharging(a[6],1,9);
                    BusCharge2(a[6],1);
                    if(minute == a[7])
                        ChargeFinished(a[6],1,9);}
                if((a[8] == carNumbers[0]) && (minute <= a[9] + 5) && (a[8] != 0))
                    {BusCharging(a[8],2,10);
                    BusCharge2(a[8],2);
                    if(minute == a[9] + 5)
                        ChargeFinished(a[8],2,10);}
                else if((a[8] == carNumbers[1]) && (a[6] != 0))
                {
                    ChargeFinished(a[8],2,103);
                }
                else if((a[8] != carNumbers[1]) && (minute <= a[9]) && (a[8] != 0))
                    {BusCharging(a[8],2,10);
                    BusCharge2(a[8],2);
                    if(minute == a[9])
                        ChargeFinished(a[8],2,10);}
                if((a[10] == carNumbers[0]) && (minute <= a[11] + 5) && (a[10] != 0))
                    {BusCharging(a[10],3,11);
                    BusCharge2(a[10],3);
                    if(minute == a[11] + 5)
                        ChargeFinished(a[10],3,11);}
                else if((a[10] == carNumbers[1]) && (a[6] != 0))
                {
                    ChargeFinished(a[10],3,104);
                }
                else if((a[10] != carNumbers[1]) && (minute <= a[11]) && (a[10] != 0))
                    {BusCharging(a[10],3,11);
                    BusCharge2(a[10],3);
                    if(minute == a[11])
                        ChargeFinished(a[10],3,11);}
            }
        }

        if (hour == 9 && minute == 5)
        {
            rawImages[0].color = greenColor;
            goingBool[carNumbers[0]-1] = false;
            whereisBus[carNumbers[0]-1] = 2;
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
                    {BusCharging(a[12],1,12);
                    BusCharge2(a[12],1);
                    if(minute == a[13])
                        ChargeFinished(a[12],1,12);}
                if((a[14] != carNumbers[1]) && (minute <= a[15]) && (a[14] != 0))
                    {BusCharging(a[14],2,13);
                    BusCharge2(a[14],2);
                    if(minute == a[15])
                        ChargeFinished(a[14],2,13);}
                if((a[16] != carNumbers[1]) && (minute <= a[17]) && (a[16] != 0))
                    {BusCharging(a[16],3,14);
                    BusCharge2(a[16],3);
                    if(minute == a[17])
                        ChargeFinished(a[16],3,14);}
            }
            else if((minute > 35)&&(minute<=45))
            {
                if((a[12] != carNumbers[1]) && (minute <= a[13]) && (a[12] != 0))
                    {BusCharging(a[12],1,15);
                    BusCharge2(a[12],1);
                    if(minute == a[13])
                        ChargeFinished(a[12],1,15);}
                else if((a[12] == carNumbers[1]) && (minute <= a[13] + 35) && (a[12] != 0))
                    {BusCharging(a[12],1,15);
                    BusCharge2(a[12],1);
                    if(minute == a[13] + 35)
                        ChargeFinished(a[12],1,15);}
                if((a[14] != carNumbers[1]) && (minute <= a[15]) && (a[14] != 0))
                    {BusCharging(a[14],2,16);
                    BusCharge2(a[14],2);
                    if(minute == a[15])
                        ChargeFinished(a[14],2,16);}
                else if((a[14] == carNumbers[1]) && (minute <= a[15] + 35) && (a[14] != 0))
                    {BusCharging(a[14],2,16);
                    BusCharge2(a[14],2);
                    if(minute == a[15] + 35)
                        ChargeFinished(a[14],2,16);}
                if((a[16] != carNumbers[1]) && (minute <= a[17]) && (a[16] != 0))
                    {BusCharging(a[16],3,17);
                    BusCharge2(a[16],3);
                    if(minute == a[17])
                        ChargeFinished(a[16],3,17);}
                else if((a[16] == carNumbers[1]) && (minute <= a[17] + 35) && (a[16] != 0))
                    {BusCharging(a[16],3,17);
                    BusCharge2(a[16],3);
                    if(minute == a[17] + 35)
                        ChargeFinished(a[16],3,17);}
            }
            else if(minute > 45)
            {
                if((a[12] == carNumbers[1]) && (minute <= a[13] + 35) && (a[12] != 0))
                    {BusCharging(a[12],1,18);
                    BusCharge2(a[12],1);
                    if(minute == a[13] + 35)
                        ChargeFinished(a[12],1,18);}
                else if((a[12] == carNumbers[2]) && (a[12] != 0))
                {
                    ChargeFinished(a[12],1,105);
                }
                else if((a[12] != carNumbers[2]) && (minute <= a[13]) && (a[12] != 0))
                    {BusCharging(a[12],1,18);
                    BusCharge2(a[12],1);
                    if(minute == a[13])
                        ChargeFinished(a[12],1,18);}
                if((a[14] == carNumbers[1]) && (minute <= a[15] + 35) && (a[14] != 0))
                    {BusCharging(a[14],2,19);
                    BusCharge2(a[14],2);
                    if(minute == a[15] + 35)
                        ChargeFinished(a[14],2,19);}
                else if((a[14] == carNumbers[2]) && (a[14] != 0))
                {
                    ChargeFinished(a[14],2,106);
                }
                else if((a[14] != carNumbers[2]) && (minute <= a[15]) && (a[14] != 0))
                    {BusCharging(a[14],2,19);
                    BusCharge2(a[14],2);
                    if(minute == a[15])
                        ChargeFinished(a[14],2,19);}
                if((a[16] == carNumbers[1]) && (minute <= a[17] + 35) && (a[16] != 0))
                    {BusCharging(a[16],3,20);
                    BusCharge2(a[16],3);
                    if(minute == a[17] + 35)
                        ChargeFinished(a[16],3,20);}
                else if((a[16] == carNumbers[2]) && (a[16] != 0))
                {
                    ChargeFinished(a[16],3,107);
                }
                else if((a[16] != carNumbers[2]) && (minute <= a[17]) && (a[16] != 0))
                    {BusCharging(a[16],3,20);
                    BusCharge2(a[16],3);
                    if(minute == a[17])
                        ChargeFinished(a[16],3,20);}
            }
        }
        if (hour == 10 && minute == 35)
        {
            rawImages[1].color = greenColor;
            goingBool[carNumbers[1]-1] = false;
            whereisBus[carNumbers[1]-1] = 1;
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
            {
                BusCharging(a[18],1,21);
                BusCharge2(a[18],1);
                if(minute == a[19])
                    ChargeFinished(a[18],1,21);
            }
            if((a[20] != carNumbers[2]) && (minute <= a[21]) && (a[20] != 0))
            {
                BusCharging(a[20],2,22);
                BusCharge2(a[20],2);
                if(minute == a[21])
                    ChargeFinished(a[20],2,22);
            }
            if((a[22] != carNumbers[2]) && (minute <= a[23]) && (a[22] != 0))
            {
                BusCharging(a[22],3,23);
                BusCharge2(a[22],3);
                if(minute == a[23])
                    ChargeFinished(a[22],3,23);
            }
        }

        if (hour==12)
        {
            if(minute <= 5)
            {
                if((a[24] != carNumbers[2]) && (minute <= a[25]) && (a[24] != 0))
                    {BusCharging(a[24],1,24);
                BusCharge2(a[24],1);
                    if(minute == a[25])
                        ChargeFinished(a[24],1,24);}
                if((a[26] != carNumbers[2]) && (minute <= a[27]) && (a[26] != 0))
                    {BusCharging(a[26],2,25);
                BusCharge2(a[26],2);
                    if(minute == a[27])
                        ChargeFinished(a[26],2,25);}
                if((a[28] != carNumbers[2]) && (minute <= a[29]) && (a[28] != 0))
                    {BusCharging(a[28],3,26);
                BusCharge2(a[28],3);
                    if(minute == a[29])
                        ChargeFinished(a[28],3,26);}
            }
            else if((minute > 5)&&(minute<=15))
            {
                if((a[24] != carNumbers[2]) && (minute <= a[25]) && (a[24] != 0))
                    {BusCharging(a[24],1,27);
                BusCharge2(a[24],1);
                    if(minute == a[25])
                        ChargeFinished(a[24],1,27);}
                else if((a[24] == carNumbers[2]) && (minute <= a[25] + 5) && (a[24] != 0))
                    {BusCharging(a[24],1,27);
                BusCharge2(a[24],1);
                    if(minute == a[25] + 5)
                        ChargeFinished(a[24],1,27);}
                if((a[26] != carNumbers[2]) && (minute <= a[27]) && (a[26] != 0))
                    {BusCharging(a[26],2,28);
                BusCharge2(a[26],2);
                    if(minute == a[27])
                        ChargeFinished(a[26],2,28);}
                else if((a[26] == carNumbers[2]) && (minute <= a[27] + 5) && (a[26] != 0))
                    {BusCharging(a[26],2,28);
                BusCharge2(a[26],2);
                    if(minute == a[27] + 5)
                        ChargeFinished(a[26],2,28);}
                if((a[28] != carNumbers[2]) && (minute <= a[29]) && (a[28] != 0))
                    {BusCharging(a[28],3,29);
                BusCharge2(a[28],3);
                    if(minute == a[29])
                        ChargeFinished(a[28],3,29);}
                else if((a[28] == carNumbers[2]) && (minute <= a[29] + 5) && (a[28] != 0))
                    {BusCharging(a[28],3,29);
                BusCharge2(a[28],3);
                    if(minute == a[29] + 5)
                        ChargeFinished(a[28],3,29);}
            }
            else if(minute > 15)
            {
                if((a[24] == carNumbers[2]) && (minute <= a[25] + 5) && (a[24] != 0))
                    {BusCharging(a[24],1,30);
                BusCharge2(a[24],1);
                    if(minute == a[25] + 5)
                        ChargeFinished(a[24],1,30);}
                else if((a[24] == carNumbers[3]) && (a[24] != 0))
                {
                    ChargeFinished(a[24],1,108);
                }
                else if((a[24] != carNumbers[3]) && (minute <= a[25]) && (a[24] != 0))
                    {BusCharging(a[24],1,30);
                BusCharge2(a[24],1);
                    if(minute == a[25])
                        ChargeFinished(a[24],1,30);}
                if((a[26] == carNumbers[2]) && (minute <= a[27] + 5) && (a[26] != 0))
                    {BusCharging(a[26],2,31);
                BusCharge2(a[26],2);
                    if(minute == a[27] + 5)
                        ChargeFinished(a[26],2,31);}
                else if((a[26] == carNumbers[3]) && (a[26] != 0))
                {
                    ChargeFinished(a[26],2,109);
                }
                else if((a[26] != carNumbers[3]) && (minute <= a[27]) && (a[26] != 0))
                    {BusCharging(a[26],2,31);
                BusCharge2(a[26],2);
                    if(minute == a[27])
                        ChargeFinished(a[24],2,31);}
                if((a[28] == carNumbers[2]) && (minute <= a[29] + 5) && (a[28] != 0))
                    {BusCharging(a[28],3,32);
                BusCharge2(a[28],3);
                    if(minute == a[29] + 5)
                        ChargeFinished(a[28],3,32);}
                else if((a[28] == carNumbers[3]) && (a[28] != 0))
                {
                    ChargeFinished(a[28],3,110);
                }
                else if((a[28] != carNumbers[3]) && (minute <= a[29]) && (a[28] != 0))
                    {BusCharging(a[28],3,32);
                BusCharge2(a[28],3);
                    if(minute == a[29])
                        ChargeFinished(a[24],3,32);}
                
            }
        }
        if (hour == 12 && minute == 5)
        {
            rawImages[2].color = greenColor;
            goingBool[carNumbers[2]-1] = false;
            whereisBus[carNumbers[2]-1] = 2;
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
                    {BusCharging(a[30],1,33);
                BusCharge2(a[30],1);
                    if(minute == a[31])
                        ChargeFinished(a[30],1,33);}
                if((a[32] != carNumbers[3]) && (minute <= a[33]) && (a[32] != 0))
                    {BusCharging(a[32],2,34);
                BusCharge2(a[32],2);
                    if(minute == a[33])
                        ChargeFinished(a[32],2,34);}
                if((a[34] != carNumbers[3]) && (minute <= a[35]) && (a[34] != 0))
                    {BusCharging(a[34],3,35);
                BusCharge2(a[34],3);
                    if(minute == a[35])
                        ChargeFinished(a[34],3,35);}
            }
            else if((minute > 35)&&(minute<=45))
            {
                if((a[30] != carNumbers[3]) && (minute <= a[31]) && (a[30] != 0))
                    {BusCharging(a[30],1,36);
                BusCharge2(a[30],1);
                    if(minute == a[31])
                        ChargeFinished(a[30],1,36);}
                else if((a[30] == carNumbers[3]) && (minute <= a[31] + 35) && (a[30] != 0))
                    {BusCharging(a[30],1,36);
                BusCharge2(a[30],1);
                    if(minute == a[31] + 35)
                        ChargeFinished(a[30],1,36);}
                if((a[32] != carNumbers[3]) && (minute <= a[33]) && (a[32] != 0))
                    {BusCharging(a[32],2,37);
                BusCharge2(a[32],2);
                    if(minute == a[33])
                        ChargeFinished(a[32],2,37);}
                else if((a[32] == carNumbers[3]) && (minute <= a[33] + 35) && (a[32] != 0))
                    {BusCharging(a[32],2,37);
                BusCharge2(a[32],2);
                    if(minute == a[33] + 35)
                        ChargeFinished(a[32],2,37);}
                if((a[34] != carNumbers[3]) && (minute <= a[35]) && (a[34] != 0))
                    {BusCharging(a[34],3,38);
                BusCharge2(a[34],3);
                    if(minute == a[35])
                        ChargeFinished(a[34],3,38);}
                else if((a[34] == carNumbers[3]) && (minute <= a[35] + 35) && (a[34] != 0))
                    {BusCharging(a[34],3,38);
                BusCharge2(a[34],3);
                    if(minute == a[35] + 35)
                        ChargeFinished(a[34],3,38);}
            }
            else if(minute > 45)
            {
                if((a[30] == carNumbers[3]) && (minute <= a[31] + 35) && (a[30] != 0))
                    {BusCharging(a[30],1,39);
                BusCharge2(a[30],1);
                    if(minute == a[31] + 35)
                        ChargeFinished(a[30],1,39);}
                else if((a[30] == carNumbers[4]) && (a[30] != 0))
                {
                    ChargeFinished(a[30],1,111);
                }
                else if((a[30] != carNumbers[4]) && (minute <= a[31]) && (a[30] != 0))
                    {BusCharging(a[30],1,39);
                BusCharge2(a[30],1);
                    if(minute == a[31])
                        ChargeFinished(a[30],1,39);}
                if((a[32] == carNumbers[3]) && (minute <= a[33] + 35) && (a[32] != 0))
                    {BusCharging(a[32],2,40);
                BusCharge2(a[32],2);
                    if(minute == a[33] + 35)
                        ChargeFinished(a[32],2,40);}
                else if((a[32] == carNumbers[4]) && (a[32] != 0))
                {
                    ChargeFinished(a[32],2,112);
                }
                else if((a[32] != carNumbers[4]) && (minute <= a[33]) && (a[32] != 0))
                    {BusCharging(a[32],2,40);
                BusCharge2(a[32],2);
                    if(minute == a[33])
                        ChargeFinished(a[32],2,40);}
                if((a[34] == carNumbers[3]) && (minute <= a[35] + 35) && (a[34] != 0))
                    {BusCharging(a[34],3,41);
                BusCharge2(a[34],3);
                    if(minute == a[35] + 35)
                        ChargeFinished(a[34],3,41);}
                else if((a[34] == carNumbers[4]) && (a[34] != 0))
                {
                    ChargeFinished(a[34],3,113);
                }
                else if((a[34] != carNumbers[4]) && (minute <= a[35]) && (a[34] != 0))
                    {BusCharging(a[34],3,41);
                BusCharge2(a[34],3);
                    if(minute == a[35])
                        ChargeFinished(a[34],3,41);}
            }
        }
        if (hour == 13 && minute == 35)
        {
            rawImages[3].color = greenColor;
            goingBool[carNumbers[3]-1] = false;
            whereisBus[carNumbers[3]-1] = 1;
        }
        if (hour == 13 && minute == 45 && checkBool == false)
        {
            GameObject canvasInstance = Instantiate(busses[carNumbers[4]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[4].color = yellowColor;
            goingBool[carNumbers[4]-1] = true;
            checkBool = true;
            Debug.Log("Hour: 13, Minute: 45 - Bus spawning");
        }

        if (hour==16)
        {
            if(minute <= 35)
            {
                if((a[48] != carNumbers[5]) && (minute <= a[49]) && (a[48] != 0))
                    {BusCharging(a[48],1,54);
                BusCharge2(a[48],1);
                    if(minute == a[49])
                        ChargeFinished(a[48],1,54);}
                if((a[50] != carNumbers[5]) && (minute <= a[51]) && (a[50] != 0))
                    {BusCharging(a[50],2,55);
                BusCharge2(a[50],2);
                    if(minute == a[51])
                        ChargeFinished(a[50],2,55);}
                if((a[52] != carNumbers[5]) && (minute <= a[53]) && (a[52] != 0))
                    {BusCharging(a[52],3,56);
                BusCharge2(a[52],3);
                    if(minute == a[53])
                        ChargeFinished(a[52],3,56);}
            }
            else if((minute > 35)&&(minute<=45))
            {
                if((a[48] != carNumbers[5]) && (minute <= a[49]) && (a[48] != 0))
                    {BusCharging(a[48],1,57);
                BusCharge2(a[48],1);
                    if(minute == a[49])
                        ChargeFinished(a[48],1,57);}
                else if((a[48] == carNumbers[5]) && (minute <= a[49] + 35) && (a[48] != 0))
                    {BusCharging(a[48],1,57);
                BusCharge2(a[48],1);
                    if(minute == a[49] + 35)
                        ChargeFinished(a[48],1,57);}
                if((a[50] != carNumbers[5]) && (minute <= a[51]) && (a[50] != 0))
                    {BusCharging(a[50],2,58);
                BusCharge2(a[50],2);
                    if(minute == a[51])
                        ChargeFinished(a[50],2,58);}
                else if((a[50] == carNumbers[5]) && (minute <= a[51] + 35) && (a[50] != 0))
                    {BusCharging(a[50],2,58);
                BusCharge2(a[50],2);
                    if(minute == a[51] + 35)
                        ChargeFinished(a[50],2,58);}
                if((a[52] != carNumbers[5]) && (minute <= a[53]) && (a[52] != 0))
                    {BusCharging(a[52],3,59);
                BusCharge2(a[52],3);
                    if(minute == a[53])
                        ChargeFinished(a[52],3,59);}
                else if((a[52] == carNumbers[5]) && (minute <= a[53] + 35) && (a[52] != 0))
                    {BusCharging(a[52],3,59);
                BusCharge2(a[52],3);
                    if(minute == a[53 + 35])
                        ChargeFinished(a[52],3,59);}
            }
            else if(minute > 45)
            {
                if((a[48] == carNumbers[5]) && (minute <= a[49] + 35) && (a[48] != 0))
                    {BusCharging(a[48],1,60);
                BusCharge2(a[48],1);
                    if(minute == a[49 + 35])
                        ChargeFinished(a[48],1,60);}
                else if((a[48] == carNumbers[6]) && (a[48] != 0))
                {
                    ChargeFinished(a[48],1,117);
                }
                else if((a[48] != carNumbers[6]) && (minute <= a[49]) && (a[48] != 0))
                    {BusCharging(a[48],1,60);
                BusCharge2(a[48],1);
                    if(minute == a[49])
                        ChargeFinished(a[48],1,60);}
                if((a[50] == carNumbers[5]) && (minute <= a[51] + 35) && (a[50] != 0))
                    {BusCharging(a[50],2,61);
                BusCharge2(a[50],2);
                    if(minute == a[51] + 35)
                        ChargeFinished(a[50],2,61);}
                else if((a[50] == carNumbers[6]) && (a[50] != 0))
                {
                    ChargeFinished(a[50],2,118);
                }
                else if((a[50] != carNumbers[6]) && (minute <= a[51]) && (a[50] != 0))
                    {BusCharging(a[50],2,61);
                BusCharge2(a[50],2);
                    if(minute == a[51])
                        ChargeFinished(a[50],2,61);}
                if((a[52] == carNumbers[5]) && (minute <= a[53] + 35) && (a[52] != 0))
                    {BusCharging(a[52],3,62);
                BusCharge2(a[52],3);
                    if(minute == a[53 + 35])
                        ChargeFinished(a[52],3,62);}
                else if((a[52] == carNumbers[6]) && (a[52] != 0))
                {
                    ChargeFinished(a[52],3,119);
                }
                else if((a[52] != carNumbers[6]) && (minute <= a[53]) && (a[52] != 0))
                    {BusCharging(a[52],3,62);
                BusCharge2(a[52],3);
                    if(minute == a[53])
                        ChargeFinished(a[52],3,62);}
            }
        }
        if (hour == 16 && minute == 35)
        {
            rawImages[5].color = greenColor;
            goingBool[carNumbers[5]-1] = false;
            whereisBus[carNumbers[5]-1] = 1;
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
            {
                BusCharging(a[54],1,63);
                BusCharge2(a[54],1);
                if(minute == a[55])
                    ChargeFinished(a[54],1,63);
            }
            if((a[56] != carNumbers[6]) && (minute <= a[57]) && (a[56] != 0))
            {
                BusCharging(a[56],2,64);
                BusCharge2(a[56],2);
                if(minute == a[57])
                    ChargeFinished(a[56],2,64);
            }
            if((a[58] != carNumbers[6]) && (minute <= a[59]) && (a[58] != 0))
            {
                BusCharging(a[58],3,65);
                BusCharge2(a[58],3);
                if(minute == a[59])
                    ChargeFinished(a[58],3,65);
            }
        }
        if (hour==18)
        {
            if(minute <= 5)
            {
                if((a[60] != carNumbers[6]) && (minute <= a[61]) && (a[60] != 0))
                    {BusCharging(a[60],1,66);
                BusCharge2(a[60],1);
                    if(minute == a[61])
                        ChargeFinished(a[60],1,66);}
                if((a[62] != carNumbers[6]) && (minute <= a[63]) && (a[62] != 0))
                    {BusCharging(a[62],2,67);
                BusCharge2(a[62],2);
                    if(minute == a[63])
                        ChargeFinished(a[62],2,67);}
                if((a[64] != carNumbers[6]) && (minute <= a[65]) && (a[64] != 0))
                    {BusCharging(a[64],3,68);
                BusCharge2(a[64],3);
                    if(minute == a[65])
                        ChargeFinished(a[64],3,68);}
            }
            else if((minute > 5)&&(minute<=15))
            {
                if((a[60] != carNumbers[6]) && (minute <= a[61]) && (a[60] != 0))
                    {BusCharging(a[60],1,69);
                BusCharge2(a[60],1);
                    if(minute == a[61])
                        ChargeFinished(a[60],1,69);}
                else if((a[60] == carNumbers[6]) && (minute <= a[61] + 5) && (a[60] != 0))
                    {BusCharging(a[60],1,69);
                BusCharge2(a[60],1);
                    if(minute == a[61] + 5)
                        ChargeFinished(a[60],1,69);}
                if((a[62] != carNumbers[6]) && (minute <= a[63]) && (a[62] != 0))
                    {BusCharging(a[62],2,70);
                BusCharge2(a[62],2);
                    if(minute == a[63])
                        ChargeFinished(a[62],2,70);}
                else if((a[62] == carNumbers[6]) && (minute <= a[63] + 5) && (a[62] != 0))
                    {BusCharging(a[62],2,70);
                BusCharge2(a[62],2);
                    if(minute == a[63] + 5)
                        ChargeFinished(a[62],2,70);}
                if((a[64] != carNumbers[6]) && (minute <= a[65]) && (a[64] != 0))
                    {BusCharging(a[64],3,71);
                BusCharge2(a[64],3);
                    if(minute == a[65])
                        ChargeFinished(a[64],3,71);}
                else if((a[64] == carNumbers[6]) && (minute <= a[65] + 5) && (a[64] != 0))
                    {BusCharging(a[64],3,71);
                BusCharge2(a[64],3);
                    if(minute == a[65 + 5])
                        ChargeFinished(a[64],3,71);}
            }
            else if(minute > 15)
            {
                if((a[60] == carNumbers[6]) && (minute <= a[61] + 5) && (a[60] != 0))
                    {BusCharging(a[60],1,72);
                BusCharge2(a[60],1);
                    if(minute == a[61] + 5)
                        ChargeFinished(a[60],1,72);}
                else if((a[60] == carNumbers[7]) && (a[60] != 0))
                {
                    ChargeFinished(a[60],1,120);
                }
                else if((a[60] != carNumbers[7]) && (minute <= a[61]) && (a[60] != 0))
                    {BusCharging(a[60],1,72);
                BusCharge2(a[60],1);
                    if(minute == a[61])
                        ChargeFinished(a[60],1,72);}
                if((a[62] == carNumbers[6]) && (minute <= a[63] + 5) && (a[62] != 0))
                    {BusCharging(a[62],2,73);
                BusCharge2(a[62],2);
                    if(minute == a[63] + 5)
                        ChargeFinished(a[62],2,73);}
                else if((a[62] == carNumbers[7]) && (a[62] != 0))
                {
                    ChargeFinished(a[62],2,121);
                }
                else if((a[62] != carNumbers[7]) && (minute <= a[63]) && (a[62] != 0))
                    {BusCharging(a[62],2,73);
                BusCharge2(a[62],2);
                    if(minute == a[63])
                        ChargeFinished(a[62],2,73);}
                if((a[64] == carNumbers[6]) && (minute <= a[65] + 5) && (a[64] != 0))
                    {BusCharging(a[64],3,74);
                BusCharge2(a[64],3);
                    if(minute == a[65 + 5])
                        ChargeFinished(a[64],3,74);}
                else if((a[64] == carNumbers[7]) && (a[64] != 0))
                {
                    ChargeFinished(a[64],3,122);
                }
                else if((a[64] != carNumbers[7]) && (minute <= a[65]) && (a[64] != 0))
                    {BusCharging(a[64],3,74);
                BusCharge2(a[64],3);
                    if(minute == a[65])
                        ChargeFinished(a[64],3,74);}
            }
        }
        if (hour == 18 && minute == 5)
        {
            rawImages[6].color = greenColor;
            goingBool[carNumbers[6]-1] = false;
            whereisBus[carNumbers[6]-1] = 2;
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
                    {BusCharging(a[66],1,75);
                BusCharge2(a[66],1);
                    if(minute == a[67])
                        ChargeFinished(a[66],1,75);}
                if((a[68] != carNumbers[7]) && (minute <= a[69]) && (a[68] != 0))
                    {BusCharging(a[68],2,76);
                BusCharge2(a[68],2);
                    if(minute == a[69])
                        ChargeFinished(a[68],2,76);}
                if((a[70] != carNumbers[7]) && (minute <= a[71]) && (a[70] != 0))
                    {BusCharging(a[70],3,77);
                BusCharge2(a[70],3);
                    if(minute == a[71])
                        ChargeFinished(a[70],3,77);}
            }
            else if((minute > 35)&&(minute<=45))
            {
                if((a[66] != carNumbers[7]) && (minute <= a[67]) && (a[66] != 0))
                    {BusCharging(a[66],1,78);
                BusCharge2(a[66],1);
                    if(minute == a[67])
                        ChargeFinished(a[66],1,78);}
                else if((a[66] == carNumbers[7]) && (minute <= a[67] + 35) && (a[66] != 0))
                    {BusCharging(a[66],1,78);
                BusCharge2(a[66],1);
                    if(minute == a[67] + 35)
                        ChargeFinished(a[66],1,78);}
                if((a[68] != carNumbers[7]) && (minute <= a[69]) && (a[68] != 0))
                    {BusCharging(a[68],2,79);
                BusCharge2(a[68],2);
                    if(minute == a[69])
                        ChargeFinished(a[68],2,79);}
                else if((a[68] == carNumbers[7]) && (minute <= a[69] + 35) && (a[68] != 0))
                    {BusCharging(a[68],2,79);
                BusCharge2(a[68],2);
                    if(minute == a[69] + 35)
                        ChargeFinished(a[68],2,79);}
                if((a[70] != carNumbers[7]) && (minute <= a[71]) && (a[70] != 0))
                    {BusCharging(a[70],3,80);
                BusCharge2(a[70],3);
                    if(minute == a[71])
                        ChargeFinished(a[70],3,80);}
                else if((a[70] == carNumbers[7]) && (minute <= a[71] + 35) && (a[70] != 0))
                    {BusCharging(a[70],3,80);
                BusCharge2(a[70],3);
                    if(minute == a[71 + 35])
                        ChargeFinished(a[70],3,80);}
            }
            else if(minute > 45)
            {
                if((a[66] == carNumbers[7]) && (minute <= a[67] + 35) && (a[66] != 0))
                    {BusCharging(a[66],1,81);
                BusCharge2(a[66],1);
                    if(minute == a[67] + 35)
                        ChargeFinished(a[66],1,81);}
                else if((a[66] == carNumbers[8]) && (a[66] != 0))
                {
                    ChargeFinished(a[66],1,123);
                }
                else if((a[66] != carNumbers[8]) && (minute <= a[67]) && (a[66] != 0))
                    {BusCharging(a[66],1,81);
                BusCharge2(a[66],1);
                    if(minute == a[67])
                        ChargeFinished(a[66],1,81);}
                if((a[68] == carNumbers[7]) && (minute <= a[69] + 35) && (a[68] != 0))
                    {BusCharging(a[68],2,82);
                BusCharge2(a[68],2);
                    if(minute == a[69] + 35)
                        ChargeFinished(a[68],2,82);}
                else if((a[68] == carNumbers[8]) && (a[68] != 0))
                {
                    ChargeFinished(a[68],2,124);
                }
                else if((a[68] != carNumbers[8]) && (minute <= a[69]) && (a[68] != 0))
                    {BusCharging(a[68],2,82);
                BusCharge2(a[68],2);
                    if(minute == a[69])
                        ChargeFinished(a[68],2,82);}
                if((a[70] == carNumbers[7]) && (minute <= a[71] + 35) && (a[70] != 0))
                    {BusCharging(a[70],3,83);
                BusCharge2(a[70],3);
                    if(minute == a[71 + 35])
                        ChargeFinished(a[70],3,83);}
                else if((a[70] == carNumbers[8]) && (a[70] != 0))
                {
                    ChargeFinished(a[70],3,125);
                }
                else if((a[70] != carNumbers[8]) && (minute <= a[71]) && (a[70] != 0))
                    {BusCharging(a[70],3,83);
                BusCharge2(a[70],3);
                    if(minute == a[71])
                        ChargeFinished(a[70],3,83);}
            }
        }
        if (hour == 19 && minute == 35)
        {
            rawImages[7].color = greenColor;
            goingBool[carNumbers[7]-1] = false;
            whereisBus[carNumbers[7]-1] = 1;
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
            {
                BusCharging(a[72],1,84);
                BusCharge2(a[72],1);
                if(minute == a[73])
                    ChargeFinished(a[72],1,84);
            }
            if((a[74] != carNumbers[8]) && (minute <= a[75]) && (a[74] != 0))
            {
                BusCharging(a[74],2,85);
                BusCharge2(a[74],2);
                if(minute == a[75])
                    ChargeFinished(a[74],2,85);
            }
            if((a[76] != carNumbers[8]) && (minute <= a[77]) && (a[76] != 0))
            {
                BusCharging(a[76],3,86);
                BusCharge2(a[76],3);
                if(minute == a[77])
                    ChargeFinished(a[76],3,86);
            }
        }
        if (hour==21)
        {
            if(minute <= 5)
            {
                if((a[78] != carNumbers[8]) && (minute <= a[79]) && (a[78] != 0))
                    {BusCharging(a[78],1,87);
                BusCharge2(a[78],1);
                    if(minute == a[79])
                        ChargeFinished(a[78],1,87);}
                if((a[80] != carNumbers[8]) && (minute <= a[81]) && (a[80] != 0))
                    {BusCharging(a[80],2,88);
                BusCharge2(a[80],2);
                    if(minute == a[81])
                        ChargeFinished(a[80],2,88);}
                if((a[82] != carNumbers[8]) && (minute <= a[83]) && (a[82] != 0))
                    {BusCharging(a[82],3,89);
                BusCharge2(a[82],3);
                    if(minute == a[83])
                        ChargeFinished(a[82],3,89);}
            }
            else if((minute > 5)&&(minute<=10))
            {
                if((a[78] != carNumbers[8]) && (minute <= a[79]) && (a[78] != 0))
                    {BusCharging(a[78],1,90);
                BusCharge2(a[78],1);
                    if(minute == a[79])
                        ChargeFinished(a[78],1,90);}
                else if((a[78] == carNumbers[8]) && (minute <= a[79] + 5) && (a[78] != 0))
                    {BusCharging(a[78],1,90);
                BusCharge2(a[78],1);
                    if(minute == a[79] + 5)
                        ChargeFinished(a[78],1,90);}
                if((a[80] != carNumbers[8]) && (minute <= a[81]) && (a[80] != 0))
                    {BusCharging(a[80],2,91);
                BusCharge2(a[80],2);
                    if(minute == a[81])
                        ChargeFinished(a[80],2,91);}
                else if((a[80] == carNumbers[8]) && (minute <= a[81] + 5) && (a[80] != 0))
                    {BusCharging(a[80],2,91);
                BusCharge2(a[80],2);
                    if(minute == a[81] + 5)
                        ChargeFinished(a[80],2,91);}
                if((a[82] != carNumbers[8]) && (minute <= a[83]) && (a[82] != 0))
                    {BusCharging(a[82],3,92);
                BusCharge2(a[82],3);
                    if(minute == a[83])
                        ChargeFinished(a[82],3,92);}
                else if((a[82] == carNumbers[8]) && (minute <= a[83] + 5) && (a[82] != 0))
                    {BusCharging(a[82],3,92);
                BusCharge2(a[82],3);
                    if(minute == a[83] + 5)
                        ChargeFinished(a[82],3,92);}
            }
            else if(minute > 10)
            {
                if((a[78] == carNumbers[8]) && (minute <= a[79] + 5) && (a[78] != 0))
                    {BusCharging(a[78],1,93);
                BusCharge2(a[78],1);
                    if(minute == a[79] + 5)
                        ChargeFinished(a[78],1,93);}
                else if((a[78] == carNumbers[9]) && (a[78] != 0))
                {
                    ChargeFinished(a[78],1,126);
                }
                else if((a[78] != carNumbers[9]) && (minute <= a[79]) && (a[78] != 0))
                    {BusCharging(a[78],1,93);
                BusCharge2(a[78],1);
                    if(minute == a[79])
                        ChargeFinished(a[78],1,93);}
                if((a[80] == carNumbers[8]) && (minute <= a[81] + 5) && (a[80] != 0))
                    {BusCharging(a[80],2,94);
                BusCharge2(a[80],2);
                    if(minute == a[81] + 5)
                        ChargeFinished(a[80],2,94);}
                else if((a[80] == carNumbers[9]) && (a[80] != 0))
                {
                    ChargeFinished(a[80],2,127);
                }
                else if((a[80] != carNumbers[9]) && (minute <= a[81]) && (a[80] != 0))
                    {BusCharging(a[80],2,94);
                BusCharge2(a[80],2);
                    if(minute == a[81])
                        ChargeFinished(a[80],2,94);}
                if((a[82] == carNumbers[8]) && (minute <= a[83] + 5) && (a[82] != 0))
                    {BusCharging(a[82],3,95);
                BusCharge2(a[82],3);
                    if(minute == a[83] + 5)
                        ChargeFinished(a[82],3,95);}
                else if((a[82] == carNumbers[9]) && (a[82] != 0))
                {
                    ChargeFinished(a[82],3,128);
                }
                else if((a[82] != carNumbers[9]) && (minute <= a[83]) && (a[82] != 0))
                    {BusCharging(a[82],3,95);
                BusCharge2(a[82],3);
                    if(minute == a[83])
                        ChargeFinished(a[82],3,95);}
            }
        }
        if (hour == 21 && minute == 5)
        {
            rawImages[8].color = greenColor;
            goingBool[carNumbers[8]-1] = false;
            whereisBus[carNumbers[8]-1] = 2;
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
                    {BusCharging(a[84],1,96);
                BusCharge2(a[84],1);
                    if(minute == a[85])
                        ChargeFinished(a[84],1,96);}
                if((a[86] != carNumbers[9]) && (minute <= a[87]) && (a[86] != 0))
                    {BusCharging(a[86],2,97);
                BusCharge2(a[86],2);
                    if(minute == a[87])
                        ChargeFinished(a[86],2,97);}
                if((a[88] != carNumbers[9]) && (minute <= a[89]) && (a[88] != 0))
                    {BusCharging(a[88],3,98);
                BusCharge2(a[88],3);
                    if(minute == a[89])
                        ChargeFinished(a[88],3,98);}
            }
            else if(minute >= 30)
            {
                if((a[84] == carNumbers[9]) && (minute <= 30 + a[85]) && (a[84] != 0))
                    {BusCharging(a[84],1,99);
                BusCharge2(a[84],1);
                    if(minute == 30 + a[85])
                        ChargeFinished(a[84],1,99);}
                else if((a[84] == carNumbers[10]) && (a[84] != 0))
                {
                    ChargeFinished(a[84],1,129);
                }
                else if((a[84] != carNumbers[10]) && (minute <= a[85]) && (a[84] != 0))
                    {BusCharging(a[84],1,99);
                BusCharge2(a[84],1);
                    if(minute == a[85])
                        ChargeFinished(a[84],1,99);}
                if((a[86] == carNumbers[9]) && (minute <= 30 + a[87]) && (a[86] != 0))
                    {BusCharging(a[86],2,100);
                BusCharge2(a[86],2);
                    if(minute == a[87])
                        ChargeFinished(a[86],2,100);}
                else if((a[86] == carNumbers[10]) && (a[86] != 0))
                {
                    ChargeFinished(a[86],2,130);
                }
                else if((a[86] != carNumbers[10]) && (minute <= a[87]) && (a[86] != 0))
                    {BusCharging(a[86],2,100);
                BusCharge2(a[86],2);
                    if(minute == a[87])
                        ChargeFinished(a[86],2,100);}
                if((a[88] == carNumbers[9]) && (minute <= 30 + a[89]) && (a[88] != 0))
                    {BusCharging(a[88],3,101);
                BusCharge2(a[88],3);
                    if(minute == a[89])
                        ChargeFinished(a[88],3,101);}
                else if((a[88] == carNumbers[10]) && (a[88] != 0))
                {
                    ChargeFinished(a[88],3,131);
                }
                else if((a[88] != carNumbers[10]) && (minute <= a[89]) && (a[88] != 0))
                    {BusCharging(a[88],3,101);
                BusCharge2(a[88],3);
                    if(minute == a[89])
                        ChargeFinished(a[88],3,101);}
            }
        }
        if (hour == 22 && minute == 30 && checkBool == false)
        {
            rawImages[9].color = greenColor;
            goingBool[carNumbers[9]-1] = false;
            whereisBus[carNumbers[9]-1] = 1;

            GameObject canvasInstance = Instantiate(busses[carNumbers[10]-1], parentObject.transform);
            spawnRectTransform = canvasInstance.GetComponent<RectTransform>();
            spawnRectTransform.anchoredPosition = respawnPositionRight;
            rawImages[10].color = yellowColor;
            goingBool[carNumbers[10]-1] = true;            
            checkBool = true;
        }
        if (hour == 23 && minute == 50)
        {
            ended.SetActive(true);
            UpdateHighScores();
        }
    }
    
    public void UpdateHighScores()
    {
        carbonText2.text = carbonText.text;
        priceText2.text = priceText.text;
        totalCharge = 0;
        for (int i = 0; i < busCharges.Length; i++)
        {
            totalCharge += busCharges[i];
        }
        totalCharge /= 8;
        avgBusCharge.text = totalCharge.ToString();
        
        // Yeni değerler alınır
        float carbonValue = float.Parse(carbonText2.text);
        float priceValue = float.Parse(priceText2.text);

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
        if (totalCharge > maxAvgBusCharge)
        {
            maxAvgBusCharge = totalCharge;
            avgBusCharge2.text = maxAvgBusCharge.ToString();
            PlayerPrefs.SetFloat("maxAvgBusCharge", maxAvgBusCharge);
            PlayerPrefs.Save();
        }
    }
}