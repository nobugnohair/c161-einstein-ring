using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject particles;

    [HideInInspector]
    public List<GameObject> particleArray;

    private int count;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CreateParticle", 2.0f, 0.3f);
        particleArray = new List<GameObject>();
    }

    void CreateParticle()
    {
        if (count <= 199)
        {
            GameObject cur = Instantiate(particles);
            count += 1;
            particleArray.Add(cur);
        }
    }
}
