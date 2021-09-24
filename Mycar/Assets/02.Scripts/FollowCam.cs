using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform carTransform;
    Rigidbody rb;
    public float distance = 6.4f;
    public float height = 20f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;
    public float zoomRatio = 0.5f;
    public float defaultFOV = 60f;

    private Vector3 rotationVector = new Vector3();
    private void Start()
    {
        rb = carTransform.GetComponent<Rigidbody>();
    }
    private void LateUpdate()
    {
        float wantedAngle = rotationVector.y;
        float wantedHeight = carTransform.position.y + height;
        float camAngle = transform.eulerAngles.y;
        float camHeight = transform.position.y;
        camAngle = Mathf.LerpAngle(camAngle, wantedAngle, rotationDamping * Time.deltaTime);
        camHeight = Mathf.Lerp(camHeight, wantedHeight, heightDamping * Time.deltaTime);
        Quaternion qt = Quaternion.Euler(0, camAngle, 0);
        transform.position = carTransform.position;
        transform.position -= qt * Vector3.forward * distance;
        transform.position += qt * Vector3.up * camHeight;
        transform.Rotate(Vector3.up * rotationVector.y);
        transform.LookAt(carTransform);
    }
    private void FixedUpdate()
    {
        Vector3 localVelocity = carTransform.InverseTransformDirection(rb.velocity);
        if (localVelocity.z < -0.5f)
        {
            rotationVector.y = carTransform.eulerAngles.y + 180;
        }
        else
        {
            rotationVector.y = carTransform.eulerAngles.y;
        }
        float acc = rb.velocity.magnitude;
        Camera.main.fieldOfView = defaultFOV + acc * zoomRatio;
    }
}
