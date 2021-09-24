using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public GameObject cameraView;
    public GameObject cameraPos;
    public float speed;

    private void LateUpdate()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, cameraPos.transform.position, Time.deltaTime*speed);
        gameObject.transform.LookAt(cameraView.transform);
    }
}
