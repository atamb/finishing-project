using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public bool[] park; // Park dizisi
    public GameObject[] busPark;
    public Canvas canvas;
    public GameObject objePrefab; // Oluşturulacak obje prefabı
    public GameObject busses; // Otobüslerin instantiate edileceği ana obje
    public GameObject busses2; // Otobüslerin instantiate edileceği ana obje
    private GameObject[] instantiatedBusses;
    public bool sabigiris;
    public bool kadigiris;
    public bool kadicikis;
    private string isim1="kadisarj";
    private string isim2="kadicikis";
    private string isim3="sabisarj";
    private string isim4="sabicikis";

    void Start()
    {
        park = new bool[6];
        instantiatedBusses = new GameObject[6];
        sabigiris = false;
        kadigiris = false;
        kadicikis = false;
    }

    void Update()
    {
        if(kadigiris)
        {
            kadigiris = false;
            for(int i=0;i<3;i++)
            {
                if(!park[i])
                {
                    string parkAdi = isim1 + (i + 1).ToString();
                    string parkAdi2 = isim2 + (i + 1).ToString();
                    ToggleParkStatus(i, parkAdi, parkAdi2);
                    break;
                }
            }
        }    
        if(kadicikis)
        {
            kadicikis = false;
            for(int i=0;i<3;i++)
            {
                if(park[i])
                {
                    string parkAdi = isim1 + (i + 1).ToString();
                    string parkAdi2 = isim2 + (i + 1).ToString();
                    ToggleParkStatus(i, parkAdi, parkAdi2);
                    break;
                }
            }
        }
        
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
        // 2 tuşuna basıldığında park[3]'i toggle yap
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ToggleParkStatus2(3, "sabisarj1", "sabicikis1");
        }

        // 2 tuşuna basıldığında park[4]'i toggle yap
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ToggleParkStatus2(4, "sabisarj2", "sabicikis2");
        }

        // 3 tuşuna basıldığında park[5]'yi toggle yap
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ToggleParkStatus2(5, "sabisarj3", "sabicikis3");
        }
    }

    void ToggleParkStatus(int index, string animName, string reverseAnimName)
    {
        if (park[index])
        {
            park[index] = false;
            Debug.Log("Park " + (index + 1) + "den çıkıldı.");
            CreateObjectAndPlayAnimation(reverseAnimName, index);
            busPark[index].SetActive(false);           
        }
        else
        {
            park[index] = true;
            Debug.Log("Park " + (index + 1) + " seçildi.");
            CreateObjectAndPlayAnimation(animName, index);
            busPark[index].SetActive(true);
        }
    }

    void ToggleParkStatus2(int index, string animName, string reverseAnimName)
    {
        if (park[index])
        {
            park[index] = false;
            Debug.Log("Park " + (index + 1) + "den çıkıldı.");
            CreateObjectAndPlayAnimation2(reverseAnimName, index);
        }
        else
        {
            park[index] = true;
            Debug.Log("Park " + (index + 1) + " seçildi.");
            CreateObjectAndPlayAnimation2(animName, index);
        }
    }

    void CreateObjectAndPlayAnimation(string animationName, int index)
    {
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

}
