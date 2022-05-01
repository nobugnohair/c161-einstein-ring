using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public Transform gravityTarget;
    Rigidbody rb;

    private GameManager gm;

    private float init_velocity =  2f;
    private float saturn_mass = 5.683f * (float)Math.Pow(10, 8);
    private float grav_const = 6.67f * (float) Math.Pow(10, -8);
    
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        Vector3 diff = transform.position - gravityTarget.position;
        rb = GetComponent<Rigidbody>();
        Vector3 tangent = Vector3.Cross(diff, new Vector3(0, -1, 0));
        rb.velocity = tangent.normalized * init_velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessGravity();
    }

    void ProcessGravity()
    {
        // Calculate Saturn Force
        Vector3 diff = transform.position - gravityTarget.position;
        rb.AddForce(-diff.normalized * saturn_mass * grav_const / (float) Math.Pow(diff.magnitude, 2));

       
        // constant oct block implementation
        List<GameObject> pArray = gm.particleArray;
        LinkedList<int> block = gm.octantMap[gm.calcOctant(this.transform)];
        LinkedList<int>.Enumerator em = block.GetEnumerator();

        while (em.MoveNext())
        {
            int pIndex = em.Current;
            Vector3 particle_diff = transform.position - pArray[pIndex].transform.position;
            float dist = (float)Math.Pow(particle_diff.magnitude, 2);
            if (dist > 0)
            {
                rb.AddForce(-particle_diff.normalized * grav_const / dist);
            }
        }


        // naive for loop implementation
        /*
        List<GameObject> pArray = gm.particleArray;
        for (int i = 0; i < pArray.Count; i++)
        {
            Vector3 particle_diff = transform.position - pArray[i].transform.position;
            float dist = (float)Math.Pow(particle_diff.magnitude, 2);
            if (dist > 0)
            {
                rb.AddForce(-particle_diff.normalized * grav_const / dist);
            }
        }
        */
    }
}
