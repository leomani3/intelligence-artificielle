using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [Header("Déplacement")]
    public float speed;
    public float wanderRandom;

    [Header("Détection")]
    public float neighborsDetectionRadius;
    public float tooNear;
    public float obstacleDetectionRadius;

    [Header("Line rendering")]
    public Material cohereMat;
    public Material repulseMat;
    public Material alignMat;

    public GameObject repulseRenderer;
    public GameObject cohereRenderer;
    public GameObject alignRenderer;
    public GameObject followRenderer;
    public GameObject avoidRenderer;




    private List<GameObject> neighbors;
    private List<GameObject> obstacles;
    private GameObject queen = null;
    public GameObject chien = null;
    private GameObject zone = null;

    private bool drawRepulse = false;
    private bool drawCohere = false;
    private bool drawAlign = false;
    private bool drawFollowQueen = false;
    private bool drawAvoid = false;
    // Start is called before the first frame update
    void Start()
    {
        repulseRenderer = Instantiate(repulseRenderer, transform.position, Quaternion.identity, transform);
        cohereRenderer = Instantiate(cohereRenderer, transform.position, Quaternion.identity, transform);
        alignRenderer = Instantiate(alignRenderer, transform.position, Quaternion.identity, transform);
        followRenderer = Instantiate(followRenderer, transform.position, Quaternion.identity, transform);
        avoidRenderer = Instantiate(avoidRenderer, transform.position, Quaternion.identity, transform);

        neighbors = new List<GameObject>();
        obstacles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageOutOfBounds();
        FindNeihbors();
        FindObstacles();
        ManageDrawInput();

        Vector3 dir = transform.up;
        Vector3 repulse = new Vector3();
        Vector3 cohere = new Vector3();
        Vector3 align = new Vector3();
        Vector3 followQueen = new Vector3();
        Vector3 avoid = new Vector3();
        Vector3 avoidChien = new Vector3();

        if (neighbors.Count > 0)
        {
            repulse = Repulse();
            cohere = Cohere();
            align = Align();


            if (drawRepulse)
            {
                //Debug.DrawRay(transform.position, repulse, Color.red);
                repulseRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                repulseRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position + repulse);
            }
            else
            {
                repulseRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                repulseRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
            }

            if (drawCohere)
            {
                //Debug.DrawRay(transform.position, cohere, Color.yellow);
                cohereRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                cohereRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position + cohere);
            }
            else
            {
                cohereRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                cohereRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
            }

            if (drawAlign)
            {
                //Debug.DrawRay(transform.position, align, Color.green);
                alignRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                alignRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position + align);
            }
            else
            {
                alignRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                alignRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
            }

        }
        else
        {
            repulseRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            repulseRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
            cohereRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            cohereRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
            alignRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            alignRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }
        if (queen != null)
        {
            followQueen = FollowQueen();
            cohere = new Vector3(0, 0, 0);
            align = new Vector3(0, 0, 0);

            if (drawFollowQueen)
            {
                //Debug.DrawRay(transform.position, followQueen, Color.cyan);
                followRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                followRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position + followQueen);
            }
            else
            {
                followRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                followRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
            }

        }
        else
        {
            followRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            followRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }

        if (obstacles.Count > 0)
        {
            avoid = Avoid() * 100;

            if (drawAvoid)
            {
                //Debug.DrawRay(transform.position, avoid, Color.black);
                avoidRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                avoidRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position + avoid);
            }
            else
            {
                avoidRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                avoidRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
            }

        }
        else
        {
            avoidRenderer.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            avoidRenderer.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }
    
        if(zone != null)
        {
            avoid = avoidZone() * 10;
            Debug.DrawRay(transform.position, avoid);
        }

        if (chien != null)
        {
            avoidChien = AvoidChien() * 200;
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

    public void ManageDrawInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            drawRepulse = !drawRepulse;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            drawCohere = !drawCohere;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            drawAlign = !drawAlign;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            drawFollowQueen = !drawFollowQueen;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            drawAvoid = !drawAvoid;
        }

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
        foreach (Collider2D collider  in Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), neighborsDetectionRadius))
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
            else if(Vector3.Distance(transform.position, neighbors[i].transform.position) < distMin)
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
