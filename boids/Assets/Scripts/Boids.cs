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




    private List<GameObject> neighbors;

    // Start is called before the first frame update
    void Start()
    {
        neighbors = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageOutOfBounds();
        FindNeihbors();

        if(neighbors.Count > 0)
        {
            Vector3 dir = Align();
            Vector3 look = dir;

            float rot = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot - 90);

            Forward();
        }
        else
        {
            gameObject.transform.position += Wander();
        }
    }

    /// <summary>
    /// retourne tous les voisins d'un boid
    /// </summary>
    public void FindNeihbors()
    {
        neighbors.Clear();
        foreach (Collider2D collider  in Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), neighborsDetectionRadius))
        {
            if (collider.tag == "bird" && collider.name != gameObject.name)
            {
                neighbors.Add(collider.gameObject);
            }
        }
    }

    /// <summary>
    /// Permet d'aligner le boid avec ses voisins
    /// </summary>
    /// <returns>Le vecteur3 correspond à la direction moyenne des voisins</returns>
    public Vector3 Align()
    {
        Vector3 res = Vector3.zero;
        if (neighbors.Count > 0)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                res += neighbors[i].transform.up;
            }
            res = res / neighbors.Count;

            return res;
        }
        else
        {
            return gameObject.transform.up;
        }

        
        
    }

    /// <summary>
    /// Fait avancer l'oiseaux vers l'avant avec un peu d'aléatoire
    /// </summary>
    /// <returns>Vecteur de déplacement</returns>
    public Vector3 Wander()
    {
        gameObject.transform.eulerAngles += new Vector3(0, 0, Random.Range(-wanderRandom, wanderRandom));
        return transform.up * speed * Time.deltaTime;
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
        //s'il dépasse par la droite ou gauche
        if (Camera.main.WorldToScreenPoint(gameObject.transform.position).x < 0 || Camera.main.WorldToScreenPoint(gameObject.transform.position).x > Screen.width)
        {
            transform.localPosition = new Vector3(transform.position.x * (-1), transform.position.y, transform.position.z);
        }
        //s'il dépasse par le haut ou le bas
        if (Camera.main.WorldToScreenPoint(gameObject.transform.position).y < 0 || Camera.main.WorldToScreenPoint(gameObject.transform.position).y > Screen.height)
        {
            transform.localPosition = new Vector3(transform.position.x, transform.position.y * (-1), transform.position.z);
        }
    }
}
