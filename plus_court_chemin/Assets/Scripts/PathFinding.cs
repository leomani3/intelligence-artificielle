using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    public int width;
    public int height;
    public Tile whiteTile;
    public Tile redTile;
    public Tile greenTile;
    public Tile blackTile;
    public Tile blueTile;
    public Tilemap tilemap;

    private Vector3Int currentStartPoint;
    private Vector3Int currentEndPoint;
    private bool startPointDefined;
    private bool endPointDefined;
    // Start is called before the first frame update
    void Start()
    {
        startPointDefined = false;
        endPointDefined = false;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), whiteTile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //click gauche pour le point de départ
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.down);
            if (hit != null && hit.collider != null)
            {
                Debug.Log("DRGFDGDF");
                if (startPointDefined)
                {
                    tilemap.SetTile(currentStartPoint, whiteTile);
                }
                Vector3Int mousePos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                mousePos.z = 0;
                tilemap.SetTile(mousePos, greenTile);
                currentStartPoint = new Vector3Int(mousePos.x, mousePos.y, 0);
                startPointDefined = true;

            }
        }
        //click molette pour le point d'arrivée
        if (Input.GetMouseButton(2))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.down);
            if (hit != null && hit.collider != null)
            {
                if (endPointDefined)
                {
                    tilemap.SetTile(currentEndPoint, whiteTile);
                }
                Vector3Int mousePos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                mousePos.z = 0;
                tilemap.SetTile(mousePos, redTile);
                currentEndPoint = new Vector3Int(mousePos.x, mousePos.y, 0);
                endPointDefined = true;
            }
        }
        //click droit pour les murs
        if (Input.GetMouseButton(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.down);
            if (hit != null && hit.collider != null)
            {
                Vector3Int mousePos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                mousePos.z = 0;
                tilemap.SetTile(mousePos, blackTile);

            }
        }
        //delete pour reset la tilemap
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            for (int i = 0; i < tilemap.size.x; i++)
            {
                for (int j = 0; j < tilemap.size.y; j++)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), whiteTile);
                }
            }
            endPointDefined = false;
            startPointDefined = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(startPointDefined && endPointDefined)
            {
                Dijsktra(currentStartPoint, currentEndPoint);
            }
            else
            {
                Debug.Log("Veuillez définir un point de départ et un point d'arrivée");
            }
        }
    }

    public void Dijsktra(Vector3Int startPoint, Vector3Int endPoint)
    {
        //initialisation
        Node currentNode = new Node(startPoint, 0, 0);
        List<Node> explored = new List<Node>();
        explored.Add(currentNode);

        //construction du graph
        List<Node> graph = new List<Node>();
        for (int i = 0; i < tilemap.size.x; i++)
        {
            for (int j = 0; j < tilemap.size.y; j++)
            {
                if (i == startPoint.x && j == startPoint.y)
                {
                    graph.Add(new Node(new Vector3Int(i, j, 0), 0, 0));
                }
                else
                {
                    graph.Add(new Node(new Vector3Int(i, j, 0), -1, 0));
                }
            }
        }

        //on étend aux voisins

        //on écrit
    }
}
