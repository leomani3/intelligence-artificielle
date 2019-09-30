using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int nbBoids;
    public GameObject boid;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < nbBoids; i++)
        {
            Instantiate(boid, new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        }
    }
}
