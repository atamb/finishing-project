using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class oneGetir : MonoBehaviour
{
    void Start()
    {
        // Eğer bir UI elemanı ise
        if (GetComponent<Graphic>() != null)
        {
            // Graphic bileşenini al
            Graphic graphic = GetComponent<Graphic>();

            // Sorting Order değerini artır (örneğin, 1000)
            graphic.canvas.sortingOrder = 1000;
        }
    }
}
