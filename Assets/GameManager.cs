using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// naive for loop
// 299 particiles causes significant frame drop

// constant block implementation
// block size 2
// around 599 particiles without significant frame drop

// block size 0.5
// around 2499 particles with occasional frame drop

// block size 0.3
// around 2999 particle

// block size 0.3
// neighbor force update for each particle
// around 1999 particles pretty frequent frame drop

// block size 0.3
// neighbor force update grid pre calculated in game manager
// around 2499 particles can be supported
// see fewer particles attached to the surface of saturn

public class GameManager : MonoBehaviour
{
    public GameObject particles;

    [HideInInspector]
    public List<GameObject> particleArray;
    public Dictionary<int, LinkedList<int>> octantMap;
    public Dictionary<int, float> massMap;
    public Dictionary<int, Vector3> forceMap;

    private int count;
    private float maxV;
    private float minV;
    private float blockSize;

    private float grav_const = 6.67f * (float)Math.Pow(10, -8);

    // Start is called before the first frame update
    void Start()
    {
        // InvokeRepeating("CreateParticle", 2.0f, 0.05f);
        maxV = 10f;
        minV = -10f;
        blockSize = 0.3f;
        particleArray = new List<GameObject>();
        octantMap = new Dictionary<int, LinkedList<int>>();
        massMap = new Dictionary<int, float>();
        forceMap = new Dictionary<int, Vector3>();
    }

    void CreateParticle()
    {
        if (count <= 2499)
        {
            GameObject cur = Instantiate(particles);
            count += 1;
            particleArray.Add(cur);
        }
    }

    void FixedUpdate()
    {
        CreateParticle();
        
        octantMap = new Dictionary<int, LinkedList<int>>();
        massMap = new Dictionary<int, float>();
        forceMap = new Dictionary<int, Vector3>();

        for (int i=0; i<particleArray.Count; i++)
        {
            int oct = calcOctant(particleArray[i].transform);
            if (!octantMap.ContainsKey(oct))
            {
                octantMap[oct] = new LinkedList<int>();
                massMap[oct] = 0f;
            }
            octantMap[oct].AddLast(i);
            massMap[oct] += particleArray[i].GetComponent<Rigidbody>().mass;
        }

        foreach(var oct in massMap.Keys)
        {
            Vector3 digits = reverseOctant(oct);
            int x = (int) digits.x;
            int y = (int) digits.y;
            int z = (int) digits.z;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        if (!forceMap.ContainsKey(oct))
                        {
                            forceMap[oct] = new Vector3(0, 0, 0);
                        }

                        int dim = getDim();
                        if (x + i >= 0 && x + i < dim && y + j >= 0 && y + j < dim && z + k >= 0 && z + k < dim)
                        {
                            int neighborOct = (x + i) * dim * dim + (y + j) * dim + z + k;
                            if (massMap.ContainsKey(neighborOct))
                            {
                                Vector3 particl_block_diff = blockPosition(x, y, z) - blockPosition(x + i, y + j, z + k);
                                float dist = (float)Math.Pow(particl_block_diff.magnitude, 2);
                                if (dist > 0)
                                {
                                   forceMap[oct] += -particl_block_diff.normalized * grav_const * massMap[neighborOct] / dist;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public int getDim()
    {
        int dim = (int)((maxV - minV) / blockSize);
        return dim;
    }

    public Vector3 reverseOctant(int o)
    {
        int dim = getDim();
        int z = o % dim;
        int y = (o / dim) % dim;
        int x = (o / dim / dim) % dim;
        return new Vector3(x, y, z);
    }

    public int calcOctant(Transform t)
    {
        int dim = getDim();
        return calcDigit(t.position.x) * dim * dim + calcDigit(t.position.y) * dim + calcDigit(t.position.z);
    }

    public int calcDigit(float v)
    {
        // max V 10
        // min V 10
        // blockSize 2
        int dim = getDim();

        if (v > maxV)
        {
            return dim - 1;
        }
        else if (v < -10)
        {
            return 0;
        }
        return (int)((v - minV) / blockSize);
    }

    public Vector3 blockPosition(int x, int y, int z)
    {
        return new Vector3(minV + x * blockSize, minV + y * blockSize, minV + z * blockSize);
    }
}
