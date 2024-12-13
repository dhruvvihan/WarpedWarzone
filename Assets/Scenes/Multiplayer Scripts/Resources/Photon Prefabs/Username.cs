using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Username : MonoBehaviour
{
    Camera mainCam;
    void Update()
    {
        if (mainCam == null)
        {
            mainCam = FindObjectOfType<Camera>();
        }

        if (mainCam == null)
            return;
        transform.LookAt(mainCam.transform);
        transform.Rotate(Vector3.up * 180);

    }
}
