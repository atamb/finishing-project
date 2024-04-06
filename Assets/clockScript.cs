using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class clockScript : MonoBehaviour
{
    public static clockScript instance; // Clock nesnesine her yerden erişmek için bir örnek oluşturuyoruz
    public TextMeshProUGUI clockText; // Saati ekrana yazdırmak için TMP metin nesnesi
    private float timeMultiplier = 100f; // Yazdırma hızını ayarlamak için çarpan

    void Awake()
    {
        // Eğer Clock nesnesi daha önce oluşturulmadıysa
        if (instance == null)
        {
            // Bu Clock nesnesini instance olarak ayarla ve yok etme
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Eğer zaten başka bir Clock nesnesi varsa, bu nesneyi yok et
            Destroy(gameObject);
        }
    }

    void Update()
    {
         float realTime = Time.realtimeSinceStartup;

        // Yazdırma hızını artırarak oyun zamanını elde et
        float gameTime = realTime * timeMultiplier;

        // Oyun zamanını saat, dakika ve saniyeye dönüştür
        int hour = (int)(gameTime / 3600f);
        int minute = (int)((gameTime / 60f) % 60);

        // Saati TMP metin nesnesine yazdır
        clockText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }
}
