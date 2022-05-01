using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// naive for loop
// 299 particiles causes significant frame drop

// constant block implementation
// block size 2
// around 599 particiles without significant frame drop

// block size 0.5
// around 2499 particles with occasional frame drop

public class GameManager : MonoBehaviour
{
    public GameObject particles;

    [HideInInspector]
    public List<GameObject> particleArray;
    public Dictionary<int, LinkedList<int>> octantMap;

    private int count;
    private float maxV;
    private float minV;
    private float blockSize;
    // Start is called before the first frame update
    void Start()
    {
        // InvokeRepeating("CreateParticle", 2.0f, 0.05f);
        maxV = 10f;
        minV = -10f;
        blockSize = 0.3f;
        particleArray = new List<GameObject>();
        octantMap = new Dictionary<int, LinkedList<int>>();
    }

    void CreateParticle()
    {
        if (count <= 2999)
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
        for (int i=0; i<particleArray.Count; i++)
        {
            int oct = calcOctant(particleArray[i].transform);
            if (!octantMap.ContainsKey(oct))
            {
                octantMap[oct] = new LinkedList<int>();
            }
            octantMap[oct].AddLast(i);
        }
        
    }

    public int calcOctant(Transform t)
    {
        int dim = (int) ((maxV - minV) / blockSize);
        return calcDigit(t.position.x) * dim * dim + calcDigit(t.position.y) * dim + calcDigit(t.position.z);
    }

    int calcDigit(float v)
    {
        // max V 10
        // min V 10
        // blockSize 2
        int dim = (int)((maxV - minV) / blockSize);

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
}
