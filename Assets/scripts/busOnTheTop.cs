using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class busOnTheTop : MonoBehaviour
{
    public float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        float posX = PlayerPrefs.GetFloat("MovingObjectX");
        float posY = PlayerPrefs.GetFloat("MovingObjectY");
        float posZ = PlayerPrefs.GetFloat("MovingObjectZ");
        transform.position = new Vector3(posX, posY, posZ);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerPrefs.SetFloat("MovingObjectX", transform.position.x);
            PlayerPrefs.SetFloat("MovingObjectY", transform.position.y);
            PlayerPrefs.SetFloat("MovingObjectZ", transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayerPrefs.SetFloat("MovingObjectX", transform.position.x);
            PlayerPrefs.SetFloat("MovingObjectY", transform.position.y);
            PlayerPrefs.SetFloat("MovingObjectZ", transform.position.z);
        }
    }
}
