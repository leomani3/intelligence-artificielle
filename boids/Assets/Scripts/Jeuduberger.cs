using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jeuduberger : MonoBehaviour
{

    public int nbBoids;
    public GameObject boid;

    public int nbQueen;
    public GameObject queen;

    public BoxCollider2D zone;

    private List<GameObject> spawnedBoids;
    private List<GameObject> spawnedQueen;


    private BoxCollider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        spawnedBoids = new List<GameObject>();
        spawnedQueen = new List<GameObject>();
        collider = GetComponent<BoxCollider2D>();

        for (int i = 0; i < nbBoids; i++)
        {
            GameObject sboid = Instantiate(boid, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            sboid.GetComponent<Boids>().speed = 3f;
            spawnedBoids.Add(sboid);
        }

        for (int i = 0; i < nbQueen; i++)
        {
            spawnedQueen.Add(Instantiate(queen, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
        }

        spawnedQueen[0].GetComponent<Queen>().SetTogglePlayerInput(true);
    }

    private void Update()
    {

    }
}
