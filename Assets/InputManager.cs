using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InputManager : MonoBehaviour
{
    public TMP_InputField[] busNumberInputs; // Otobüs numarası inputları
    public TMP_InputField[] minuteInputs; // Dakika inputları

    private const int totalInputs = 90; // Toplam input sayısı

    void Start()
    {
        // PlayerPrefs'ten kaydedilen değerleri yükle
        LoadInputs();
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
        for (int i = 0; i < totalInputs / 2; i++)
        {
            Debug.Log("Bus " + i + ": " + busNumberInputs[i].text);
            Debug.Log("Minute " + i + ": " + minuteInputs[i].text);
        }
        SceneManager.LoadScene(2);
    }
}
