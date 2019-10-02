using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int nbBoids;
    public GameObject boid;

    public int nbQueen;
    public GameObject queen;
    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        for (int i = 0; i < nbBoids; i++)
        {
            Instantiate(boid, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        }

        for (int i = 0; i < nbQueen; i++)
        {
            Instantiate(queen, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        }
    }
}
