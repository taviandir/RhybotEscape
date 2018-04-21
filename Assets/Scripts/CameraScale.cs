using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{
    public float floatable = 100.0f;
    //This can be PixelsPerUnit, or you can change it during runtime to alter the camera.

    private void Awake()
    {
        GetComponent<Camera>().orthographicSize = Screen.height * gameObject.GetComponent<Camera>().rect.height / floatable / 2.0f; //- 0.1f;
    }
}
