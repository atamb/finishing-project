using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InputManager : MonoBehaviour
{
    public TMP_InputField[] busNumberInputs; // Otobüs numarası inputları
    public TMP_InputField[] minuteInputs; // Dakika inputları

    private const int totalInputs = 90; // Toplam input sayısı
    public const float Cost = 0;
    public float[] electricPrices = {1.6f,2.8f,2.6f,2.4f,5.5f,2.8f,3.0f,4.0f,6.0f,4.3f,4.2f,6.6f,6.9f,5.1f,5.0f};
    public float[] carbonEmission = new float[15];

    void Start()
    {
        LoadInputs();
        electricPrices[0] = 1.6f;
        electricPrices[1] = 2.8f;
        electricPrices[2] = 2.6f;
        electricPrices[3] = 2.4f;
        electricPrices[4] = 5.5f;
        electricPrices[5] = 2.8f;
        electricPrices[6] = 3.0f;
        electricPrices[7] = 4.0f;
        electricPrices[8] = 6.0f;
        electricPrices[9] = 4.3f;
        electricPrices[10] = 4.2f;
        electricPrices[11] = 6.6f;
        electricPrices[12] = 6.9f;
        electricPrices[13] = 5.1f;
        electricPrices[14] = 5.0f;
        carbonEmission[0] = 334;
        carbonEmission[1] = 268;
        carbonEmission[2] = 191;
        carbonEmission[3] = 152;
        carbonEmission[4] = 140;
        carbonEmission[5] = 137;
        carbonEmission[6] = 143;
        carbonEmission[7] = 156;
        carbonEmission[8] = 188;
        carbonEmission[9] = 227;
        carbonEmission[10] = 262;
        carbonEmission[11] = 319;
        carbonEmission[12] = 317;
        carbonEmission[13] = 344;
        carbonEmission[14] = 400;

    }

    void LoadInputs()
    {
        for (int i = 0; i < totalInputs / 2; i++)
        {
            busNumberInputs[i].text = "0";
            minuteInputs[i].text = "0";

            PlayerPrefs.DeleteKey("BusNumber_" + i);
            PlayerPrefs.DeleteKey("Minute_" + i);
        }

        // PlayerPrefs'teki değişiklikleri hemen uygula
        PlayerPrefs.Save();
    }

    void CalculatePrices()
    {
        float totalCost = 0;
        float totalEmission = 0;

        for (int i = 0; i < minuteInputs.Length/3; i++)
        {
            totalCost += electricPrices[i] * float.Parse(minuteInputs[3*i].text) / 60;
            totalCost += electricPrices[i] * float.Parse(minuteInputs[3*i + 1].text) / 60;
            totalCost += electricPrices[i] * float.Parse(minuteInputs[3*i + 2].text) / 60;
            totalEmission += carbonEmission[i] * float.Parse(minuteInputs[3*i].text) / 60000;
            totalEmission += carbonEmission[i] * float.Parse(minuteInputs[3*i + 1].text) / 60000;
            totalEmission += carbonEmission[i] * float.Parse(minuteInputs[3*i + 2].text) / 60000;
        }

        PlayerPrefs.SetFloat("ElectricPrice", totalCost);
        PlayerPrefs.SetFloat("CarbonEmission", totalEmission);
        PlayerPrefs.Save();
    }

    public void SaveInputs()
    {
        // Otobüs numarası ve dakika inputlarındaki değerleri PlayerPrefs'e kaydet
        for (int i = 0; i < totalInputs / 2; i++)
        {
            int busNumber = int.Parse(busNumberInputs[i].text);
            int minute = int.Parse(minuteInputs[i].text);

            PlayerPrefs.SetInt("BusNumber_" + i, busNumber);
            PlayerPrefs.SetInt("Minute_" + i, minute);
        }

        // PlayerPrefs'e kaydedilen değerleri hemen uygula
        PlayerPrefs.Save();
        SceneManager.LoadScene(2);
        CalculatePrices();
    }
}
