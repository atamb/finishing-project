using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public bool[] park; // Park dizisi
    public GameObject objePrefab; // Oluşturulacak obje prefabı
    public GameObject busses;

    void Start()
    {
        // Park dizisini 3 elemanlı olarak başlat
        park = new bool[3];
    }

    void Update()
    {
        // 1 tuşuna basıldığında park[0]'ı True yap
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            park[0] = true;
            Debug.Log("Park 1 seçildi.");
            CreateObjectAndPlayAnimation("kadisarj1");
        }

        // 2 tuşuna basıldığında park[1]'i True yap
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            park[1] = true;
            Debug.Log("Park 2 seçildi.");
            CreateObjectAndPlayAnimation("kadisarj2");
        }

        // 3 tuşuna basıldığında park[2]'yi True yap
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            park[2] = true;
            Debug.Log("Park 3 seçildi.");
            CreateObjectAndPlayAnimation("kadisarj3");
        }
    }

    void CreateObjectAndPlayAnimation(string animationName)
    {
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
    }
    }
