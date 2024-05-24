using UnityEngine;

public class ChargingManager : MonoBehaviour
{
    private const int totalInputs = 96; // Toplam input sayısı
    private const int hours = 16; // Toplam saat sayısı
    private const int halfInputs = totalInputs / 2; // Otobüs numarası ve dakika inputlarının yarısı

    void Start()
    {
        int[] a = new int[totalInputs]; // Otobüs numaralarını ve dakika sürelerini tutacak dizi
        LoadChargingSchedule(a);
    }

    void LoadChargingSchedule(int[] array)
    {
        for (int i = 0; i < halfInputs; i++)
        {
            int busNumber = PlayerPrefs.GetInt("BusNumber_" + i, 0);
            int minute = PlayerPrefs.GetInt("Minute_" + i, 0);

            array[2 * i] = busNumber;
            array[2 * i + 1] = minute;
        }
    }
}
