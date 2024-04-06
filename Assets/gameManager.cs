using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public bool[] park; // Park dizisi
    public GameObject objePrefab; // Oluşturulacak obje prefabı
    public GameObject busses; // Otobüslerin instantiate edileceği ana obje
    private GameObject[] instantiatedBusses; // Oluşturulmuş otobüsleri tutmak için dizi

    void Start()
    {
        // Park dizisini 3 elemanlı olarak başlat
        park = new bool[3];
        instantiatedBusses = new GameObject[3]; // Oluşturulmuş otobüsleri tutmak için dizi
    }

    void Update()
    {
        // 1 tuşuna basıldığında park[0]'ı toggle yap
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleParkStatus(0, "kadisarj1", "kadicikis1");
        }

        // 2 tuşuna basıldığında park[1]'i toggle yap
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleParkStatus(1, "kadisarj2", "kadicikis2");
        }

        // 3 tuşuna basıldığında park[2]'yi toggle yap
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleParkStatus(2, "kadisarj3", "kadicikis3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene(0);
        }
    }

    void ToggleParkStatus(int index, string animName, string reverseAnimName)
    {
        if (park[index])
        {
            park[index] = false;
            Debug.Log("Park " + (index + 1) + "den çıkıldı.");
            CreateObjectAndPlayAnimation(reverseAnimName, index);
        }
        else
        {
            park[index] = true;
            Debug.Log("Park " + (index + 1) + " seçildi.");
            CreateObjectAndPlayAnimation(animName, index);
        }
    }

    void CreateObjectAndPlayAnimation(string animationName, int index)
    {
        // Eğer zaten bir otobüs instantiate edilmişse, yok et
        if (instantiatedBusses[index] != null)
        {
            Destroy(instantiatedBusses[index]);
        }

        // Otobüslerin instantiate edileceği konumu belirle
        Vector3 position = busses.transform.position;

        // Objeyi oluştur
        GameObject obje = Instantiate(objePrefab, position, Quaternion.identity);

        // Oluşturulan objenin ebeveynini "busses" objesi olarak ayarla
        obje.transform.parent = busses.transform;

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

        // Oluşturulan objeyi diziye kaydet
        instantiatedBusses[index] = obje;
    }
    

    }
