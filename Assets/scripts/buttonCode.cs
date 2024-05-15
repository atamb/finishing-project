using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonCode : MonoBehaviour
{
    public bool Sabiha;
    public GameObject Cam2;
    public GameObject busService;

    void Start()
    {
        Sabiha = true;
    }
    public void Button()
    {
        busService.SetActive(false);
    }
    public void changeStation()
    {
        Sabiha = !Sabiha;
        if(Sabiha)
            Cam2.SetActive(false);
        else
            Cam2.SetActive(true);
    }
    public void BusServices()
    {
        busService.SetActive(true);
    }
}
