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
    public bool sabicikis;
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
        AnimPlayer();
    }
    void AnimPlayer()
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
        if(sabigiris)
        {
            sabigiris = false;
            for(int i=0;i<3;i++)
            {
                if(!park[i])
                {
                    string parkAdi = isim3 + (i + 1).ToString();
                    string parkAdi2 = isim4 + (i + 1).ToString();
                    ToggleParkStatus2(i+3, parkAdi, parkAdi2);
                    break;
                }
            }
        }
        if(sabicikis)
        {
            sabicikis = false;
            for(int i=0;i<3;i++)
            {
                if(park[i])
                {
                    string parkAdi = isim3 + (i + 1).ToString();
                    string parkAdi2 = isim4 + (i + 1).ToString();
                    ToggleParkStatus2(i+3, parkAdi, parkAdi2);
                    break;
                }
            }
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
            busPark[index].SetActive(false); 
        }
        else
        {
            park[index] = true;
            Debug.Log("Park " + (index + 1) + " seçildi.");
            CreateObjectAndPlayAnimation2(animName, index);
            busPark[index].SetActive(true);
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
