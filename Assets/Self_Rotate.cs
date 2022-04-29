using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Self_Rotate : MonoBehaviour
{
    public float speed;
    public float direction;

    void Update()
    {
        transform.Rotate(direction *(Vector3.up * speed *Time.deltaTime), Space.Self);    
    }
}
