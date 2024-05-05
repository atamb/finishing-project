using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gameManager2 : MonoBehaviour
{
    public TMP_InputField[] inputFields; // Input alanlarını bu dizide saklayacağız
    public static int[] carNumbers = new int[11]; // Araba numaralarını saklamak için bir dizi
    public GameObject toDestroy;
    public GameObject toCreate;

    public void StoreInputs()
    {
        // Input alanlarından gelen sayıları doğrudan diziye aktarın
        for (int i = 0; i < inputFields.Length; i++)
        {
            carNumbers[i] = int.Parse(inputFields[i].text);
            PlayerPrefs.SetInt("CarNumber_" + i, carNumbers[i]);
        }

        // Test amaçlı, doğru girişlerin saklanıp saklanmadığını kontrol etmek için konsola yazdırabiliriz
        Debug.Log("Girilen arabalar: " + string.Join(", ", carNumbers));
        toCreate.SetActive(true);
        toDestroy.SetActive(false);
    }
}
