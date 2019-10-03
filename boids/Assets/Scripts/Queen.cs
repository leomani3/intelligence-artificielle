using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : MonoBehaviour
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



    private float currentSpeed;

    private bool togglePlayerInput = false;
    // Start is called before the first frame update
    void Start()
    {
        neighbors = new List<GameObject>();
        obstacles = new List<GameObject>();
        currentSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {


        ManageOutOfBounds();
        FindNeihbors();
        FindObstacles();

        Vector3 dir = transform.up;
        if (!togglePlayerInput)
        {
            currentSpeed = speed;
            if (neighbors.Count > 0)
            {
                Vector3 repulse = Repulse() * 10;
                dir += repulse;
            }
            if (obstacles.Count > 0)
            {
                Vector3 avoid = Avoid() * 100;
                dir += avoid;
            }

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, angle), 5f);

            RandomDir();
            Forward();
        }
        else
        {
            currentSpeed = speed + 2f;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0, 0, 5));
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0, 0, -5));
            }

            Forward();
        }




    }

    public void SetTogglePlayerInput(bool b)
    {
        Debug.Log(b);
        togglePlayerInput = b;
    }

    public void FindNeihbors()
    {
        neighbors.Clear();
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), neighborsDetectionRadius))
        {
            if (collider.tag == "bird" || (collider.tag == "queen" && collider.name != gameObject.name))
            {
                neighbors.Add(collider.gameObject);
            }
        }
    }

    public void FindObstacles()
    {
        obstacles.Clear();
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), neighborsDetectionRadius))
        {
            if (collider.tag == "mur")
            {
                obstacles.Add(collider.gameObject);
            }
        }
    }

    public Vector3 Avoid()
    {

        Vector3 res = new Vector3();
        if (obstacles.Count > 0)
        {
            if (Vector3.Distance(gameObject.transform.position, obstacles[NearestObstacle()].transform.position) < obstacleDetectionRadius)
            {
                res = gameObject.transform.position - obstacles[NearestObstacle()].transform.position;
            }
        }

        return res.normalized;
    }

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

    public void RandomDir()
    {
        gameObject.transform.eulerAngles += new Vector3(0, 0, Random.Range(-wanderRandom, wanderRandom));
    }

    /// <summary>
    /// Fait avancer le boid
    /// </summary>
    public void Forward()
    {
        gameObject.transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

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
