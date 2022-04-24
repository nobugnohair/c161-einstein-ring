using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Transform gravityTarget;
    public float gravity;
    public float velocity;
    Rigidbody rb;

    private GameManager gm;
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        Vector3 diff = transform.position - gravityTarget.position;
        rb = GetComponent<Rigidbody>();
        Vector3 tangent = Vector3.Cross(diff, new Vector3(0, -1, 0));
        rb.velocity = tangent.normalized * velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessGravity();
    }

    void ProcessGravity()
    {
        Vector3 diff = transform.position - gravityTarget.position;
        rb.AddForce(-diff.normalized * gravity * (rb.mass));

        List<GameObject> pArray = gm.particleArray;

        for (int i = 0; i < pArray.Count; i++)
        {
            Vector3 particle_diff = transform.position - pArray[i].transform.position;
            float dist = (float) Math.Pow(particle_diff.magnitude, 2);
            if (dist > 0)
            {
                rb.AddForce(-particle_diff.normalized * 0.001f / dist);

            }
        }
    }
}
