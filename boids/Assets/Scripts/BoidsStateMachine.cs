using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsStateMachine : MonoBehaviour
{
    [Header("Déplacement")]
    public float speed;
    public float wanderRandom;

    [Header("Détection")]
    public float neighborsDetectionRadius;
    public float tooNear;
    public float obstacleDetectionRadius;

    private List<GameObject> neighbors;
    private List<GameObject> obstacles;
    private GameObject queen = null;
    public GameObject chien = null;
    private GameObject zone = null;

    private Animator animator;

    enum State { Wander, FollowNeighbors, FollowQueen, Repulse, Avoid };


    private State currentState;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("wander", true);
        currentState = State.Wander;

        neighbors = new List<GameObject>();
        obstacles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageOutOfBounds();
        FindNeihbors();
        FindObstacles();

        Vector3 dir = transform.up;
        Vector3 repulse = new Vector3();
        Vector3 cohere = new Vector3();
        Vector3 align = new Vector3();
        Vector3 followQueen = new Vector3();
        Vector3 avoid = new Vector3();
        Vector3 avoidChien = new Vector3();

        animator.SetBool("followNeighbors", false);
        animator.SetBool("followQueen", false);
        animator.SetBool("wander", false);
        animator.SetBool("avoid", false);


        switch (currentState)
        {
            case State.Wander:
                animator.SetBool("wander", true);
                RandomDir();
                Forward();

                if (obstacles.Count > 0)
                    currentState = State.Avoid;
                else if (queen != null)
                    currentState = State.FollowQueen;
                else if (neighbors.Count > 0)
                    currentState = State.FollowNeighbors;

                break;

            case State.FollowNeighbors:
                animator.SetBool("followNeighbors", true);
                repulse = Repulse();
                cohere = Cohere();
                align = Align();

                if (obstacles.Count > 0)
                    currentState = State.Avoid;
                else if (queen != null)
                    currentState = State.FollowQueen;
                break;

            case State.FollowQueen:
                animator.SetBool("followQueen", true);
                if (obstacles.Count > 0)
                {
                    currentState = State.Avoid;
                    break;
                }
                else if (queen == null)
                {
                    if (neighbors.Count > 0)
                    {
                        currentState = State.FollowNeighbors;
                    }
                    else
                    {
                        currentState = State.Wander;
                    }
                    break;
                }

                followQueen = FollowQueen();
                cohere = new Vector3(0, 0, 0);
                align = new Vector3(0, 0, 0);

                break;

            case State.Avoid:
                animator.SetBool("avoid", true);
                avoid = Avoid() * 100;

                if (queen != null)
                {
                    currentState = State.FollowQueen;
                }
                else if (neighbors.Count > 0)
                {
                    currentState = State.FollowNeighbors;
                }
                else
                {
                    currentState = State.Wander;
                }
                break;
        }

        dir += (repulse * 10) + cohere + align + (followQueen * 10) + avoid + avoidChien;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, angle), 5f);

        RandomDir();
        Forward();
    }

    public Vector3 AvoidChien()
    {
        Vector3 res = new Vector3();
        if (Vector3.Distance(gameObject.transform.position, chien.transform.position) < tooNear)
        {
            res = (gameObject.transform.position - chien.transform.position);
        }
        return res.normalized;
    }

    public Vector3 FollowQueen()
    {
        return (queen.transform.position - transform.position).normalized;
    }

    /// <summary>
    /// retourne tous les voisins d'un boid
    /// </summary>
    public void FindNeihbors()
    {
        queen = null;
        chien = null;
        neighbors.Clear();
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), neighborsDetectionRadius))
        {
            if (collider.tag == "bird" && collider.gameObject.transform.position != transform.position)
            {
                neighbors.Add(collider.gameObject);
            }
            if (collider.tag == "queen")
            {
                queen = collider.gameObject;
            }
            if (collider.tag == "chien")
            {
                chien = collider.gameObject;
            }
        }
    }

    public Vector3 avoidZone()
    {
        Vector3 res = new Vector3();
        if (Vector3.Distance(gameObject.transform.position, zone.transform.position) < tooNear)
        {
            res = (gameObject.transform.position - zone.transform.position);
        }
        return res.normalized;
    }

    public Vector3 Cohere()
    {
        Vector3 res = new Vector3();
        if (neighbors.Count > 0)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                res += neighbors[i].transform.position;
            }
            res = res / neighbors.Count;
        }

        return (res - gameObject.transform.position).normalized;
    }

    public Vector3 Avoid()
    {

        Vector3 res = new Vector3();
        if (obstacles.Count > 0)
        {
            if (Vector3.Distance(gameObject.transform.position, obstacles[NearestObstacle()].transform.position) < tooNear)
            {
                res = gameObject.transform.position - obstacles[NearestObstacle()].transform.position;
            }
        }
        return res.normalized;
    }

    public void FindObstacles()
    {
        obstacles.Clear();
        zone = null;
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), neighborsDetectionRadius))
        {
            if (collider.tag == "mur")
            {
                obstacles.Add(collider.gameObject);
            }
            if (collider.tag == "zone")
            {
                zone = collider.gameObject;
            }
        }
    }

    /// <summary>
    /// Permet au boid d'esquiver les obstacles et les autres boids
    /// </summary>
    /// <returns></returns>
    public Vector3 Repulse()
    {
        Vector3 res = new Vector3();
        if (neighbors.Count > 0)
        {
            if (Vector3.Distance(gameObject.transform.position, neighbors[NearestNeighbor()].transform.position) < tooNear)
            {
                res = gameObject.transform.position - neighbors[NearestNeighbor()].transform.position;
            }
        }
        return res.normalized;
    }

    /// <summary>
    /// Permet d'aligner le boid avec ses voisins
    /// </summary>
    /// <returns>Le vecteur3 correspond à la direction moyenne des voisins</returns>
    public Vector3 Align()
    {
        Vector3 res = new Vector3();
        for (int i = 0; i < neighbors.Count; i++)
        {
            res += neighbors[i].transform.up;
        }
        res = res / neighbors.Count;

        return res.normalized;
    }

    public int NearestNeighbor()
    {
        int res = 0;
        float distMin = 0;
        bool isSet = false;
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (!isSet)
            {
                distMin = Vector3.Distance(transform.position, neighbors[i].transform.position);
                res = i;
                isSet = true;
            }
            else if (Vector3.Distance(transform.position, neighbors[i].transform.position) < distMin)
            {
                distMin = Vector3.Distance(transform.position, neighbors[i].transform.position);
                res = i;
            }
        }
        return res;
    }

    public int NearestObstacle()
    {
        int res = 0;
        float distMin = 0;
        bool isSet = false;
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (!isSet)
            {
                distMin = Vector3.Distance(transform.position, obstacles[i].transform.position);
                res = i;
                isSet = true;
            }
            else if (Vector3.Distance(transform.position, obstacles[i].transform.position) < distMin)
            {
                distMin = Vector3.Distance(transform.position, obstacles[i].transform.position);
                res = i;
            }
        }
        return res;
    }

    /// <summary>
    /// Fait avancer l'oiseaux vers l'avant avec un peu d'aléatoire
    /// </summary>
    /// <returns>Vecteur de déplacement</returns>
    public void RandomDir()
    {
        gameObject.transform.eulerAngles += new Vector3(0, 0, Random.Range(-wanderRandom, wanderRandom));
    }

    /// <summary>
    /// Fait avancer le boid
    /// </summary>
    public void Forward()
    {
        gameObject.transform.position += transform.up * speed * Time.deltaTime;
    }

    /// <summary>
    /// gère le fait que si les boids sortent de l'écran il reviennent de l'autre côté
    /// </summary>
    public void ManageOutOfBounds()
    {
        //s'il dépasse par la droite
        if (Camera.main.WorldToScreenPoint(gameObject.transform.position).x > Screen.width)
        {
            transform.position = new Vector3(transform.position.x * (-1) + 0.5f, transform.position.y, transform.position.z);
        }
        //s'il dépasse par la gauche
        if (Camera.main.WorldToScreenPoint(gameObject.transform.position).x < 0)
        {
            transform.position = new Vector3(transform.position.x * (-1) - 0.5f, transform.position.y, transform.position.z);
        }
        //s'il dépasse par le bas
        if (Camera.main.WorldToScreenPoint(gameObject.transform.position).y < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y * (-1) - 0.5f, transform.position.z);
        }
        //s'il dépasse pas le haut
        if (Camera.main.WorldToScreenPoint(gameObject.transform.position).y > Screen.height)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y * (-1) + 0.5f, transform.position.z);
        }
    }
}
