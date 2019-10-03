using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int nbBoids;
    public GameObject boid;

    public int nbQueen;
    public GameObject queen;

    public GameObject obstacle;

    public TMP_Text repulseText;
    public TMP_Text cohereText;
    public TMP_Text alignText;
    public TMP_Text followText;
    public TMP_Text avoidText;

    private bool drawRepulse = false;
    private bool drawCohere = false;
    private bool drawAlign = false;
    private bool drawFollowQueen = false;
    private bool drawAvoid = false;

    private List<GameObject> spawnedBoids;
    private List<GameObject> spawnedQueen;

    private bool togglePlayerInput = false;

    private BoxCollider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        spawnedBoids = new List<GameObject>();
        spawnedQueen = new List<GameObject>();
        collider = GetComponent<BoxCollider2D>();

        for (int i = 0; i < nbBoids; i++)
        {
            spawnedBoids.Add(Instantiate(boid, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
        }

        for (int i = 0; i < nbQueen; i++)
        {
            spawnedQueen.Add(Instantiate(queen, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            drawRepulse = !drawRepulse;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            drawCohere = !drawCohere;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            drawAlign = !drawAlign;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            drawFollowQueen = !drawFollowQueen;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            drawAvoid = !drawAvoid;

        if (drawRepulse)
            repulseText.color = new Color(repulseText.color.r, repulseText.color.g, repulseText.color.b, 1f);
        else
            repulseText.color = new Color(repulseText.color.r, repulseText.color.g, repulseText.color.b, 0.3f);

        if (drawCohere)
            cohereText.color = new Color(cohereText.color.r, cohereText.color.g, cohereText.color.b, 1f);
        else
            cohereText.color = new Color(cohereText.color.r, cohereText.color.g, cohereText.color.b, 0.3f);

        if (drawAlign)
            alignText.color = new Color(alignText.color.r, alignText.color.g, alignText.color.b, 1f);
        else
            alignText.color = new Color(alignText.color.r, alignText.color.g, alignText.color.b, 0.3f);

        if (drawFollowQueen)
            followText.color = new Color(followText.color.r, followText.color.g, followText.color.b, 1f);
        else
            followText.color = new Color(followText.color.r, followText.color.g, followText.color.b, 0.3f);

        if (drawAvoid)
            avoidText.color = new Color(avoidText.color.r, avoidText.color.g, avoidText.color.b, 1f);
        else
            avoidText.color = new Color(avoidText.color.r, avoidText.color.g, avoidText.color.b, 0.3f);

        Camera.main.orthographicSize -= Input.mouseScrollDelta.y;

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(obstacle, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            togglePlayerInput = !togglePlayerInput;
            spawnedQueen[0].GetComponent<Queen>().SetTogglePlayerInput(togglePlayerInput);
            if (togglePlayerInput)
            {
                spawnedQueen[0].GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                spawnedQueen[0].GetComponent<SpriteRenderer>().color = new Color32(255, 151, 0, 255);
            }
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            spawnedBoids.Add(Instantiate(boid, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
        }
        if (Input.GetKeyDown(KeyCode.U) && spawnedBoids.Count > 0)
        {
            Destroy(spawnedBoids[spawnedBoids.Count - 1]);
            spawnedBoids.RemoveAt(spawnedBoids.Count - 1);
        }
        if (Input.GetKey(KeyCode.I))
        {
            spawnedBoids.Add(Instantiate(boid, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            spawnedQueen.Add(Instantiate(queen, new Vector3(Random.Range(0, collider.size.x / 2), Random.Range(0, collider.size.y / 2), 0), Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
        }
        if (Input.GetKeyDown(KeyCode.J) && spawnedQueen.Count > 0)
        {
            Destroy(spawnedQueen[spawnedQueen.Count - 1]);
            spawnedQueen.RemoveAt(spawnedQueen.Count - 1);
        }

    }
}

