using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private float speed = 3f;
    public GameObject player;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            player.transform.Rotate(0f, -Input.GetAxis("Mouse X") * speed, 0f, Space.World);
            player.transform.Rotate(-Input.GetAxis("Mouse Y") * speed, 0f, 0f);
        }
    }
}
