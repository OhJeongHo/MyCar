using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform Target;
    public float height = 10;
    public float distance = -20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(Target.position.x, Target.position.y + height, Target.position.z + distance);
        transform.Rotate(Vector3.down, Target.rotation.y);
    }
}
