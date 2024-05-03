using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonCode : MonoBehaviour
{
    public GameObject busService;
    public void Button()
    {
        busService.SetActive(false);
    }
    public void BusServices()
    {
        busService.SetActive(true);
    }
}
