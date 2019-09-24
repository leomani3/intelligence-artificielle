using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            wipe();
            endPointDefined = false;
            startPointDefined = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(startPointDefined && endPointDefined)
            {
                StartCoroutine(Dijsktra(currentStartPoint, currentEndPoint, 0.05f));
            }
            else
            {
                Debug.Log("Veuillez définir un point de départ et un point d'arrivée");
            }
        }
    }

    public IEnumerator Dijsktra(Vector3Int startPoint, Vector3Int endPoint, float frameTime)
    {
        //initialisation
        Node startNode = new Node(startPoint.x, startPoint.y, 0, 0);
        Node endNode = new Node(endPoint.x, endPoint.y, 0, 0);

        List<Node> explored = new List<Node>();
        List<Node> chosen = new List<Node>();

        Node currentNode = startNode;

        Node[,] graph = new Node[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tilemap.GetTile(new Vector3Int(i, j, 0)).name.Contains("black"))
                {
                    graph[i, j] = new Node(i, j, -2, 1);
                }
                else
                {
                    graph[i, j] = new Node(i, j, -1, 1);
                }
            }
        }
        graph[startPoint.x, startPoint.y] = startNode;

        while (!currentNode.Equal(endNode))
        {
            tilemap.SetTile(new Vector3Int(startNode.x, startNode.y, 0), greenTile);
            //on étend aux voisins
            exploreNeighbors(currentNode, graph, explored);
            //on tri explored dans l'ordre croissant des couts
            explored.Sort(SortByCost);
            //choisir le plus bas cout parmis les noeuds explorés. par défaut explored[0] grace au tri.
            currentNode = explored[0];
            tilemap.SetTile(new Vector3Int(explored[0].x, explored[0].y, 0), whiteTile);
            explored.RemoveAt(0);
            yield return new WaitForSeconds(frameTime);
        }
        //exploration terminée. Il faut maintenant chercher le chemin !
        while (!currentNode.Equal(startNode))
        {
            tilemap.SetTile(new Vector3Int(currentNode.x, currentNode.y, 0), greenTile);
            currentNode = currentNode.parent;
            yield return new WaitForSeconds(frameTime);
        }

    }

    public void wipe()
    {
        for (int i = 0; i < tilemap.size.x; i++)
        {
            for (int j = 0; j < tilemap.size.y; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), whiteTile);
            }
        }
    }

    static int SortByCost(Node n1, Node n2)
    {
        return n1.cost.CompareTo(n2.cost);
    }

    public void addNeighbor(Node currentNode, int neighborX, int neighborY, Node[,] graph, List<Node> explored)
    {
        if (graph[neighborX, neighborY].cost == -1 || (graph[neighborX, neighborY].cost != -2 && graph[neighborX, neighborY].cost != -1 && currentNode.cost + graph[neighborX, neighborY].distance < graph[neighborX, neighborY].cost))
        {
            graph[neighborX, neighborY].cost = currentNode.cost + graph[neighborX, neighborY].distance;
            graph[neighborX, neighborY].parent = currentNode;
            explored.Add(graph[neighborX, neighborY]);
            //Debug.Log("current node : " + currentNode.x + " " + currentNode.y + "voisin ajouté : " + neighborX + " " + neighborY + "avec un nouveau cout de : " + graph[neighborX, neighborY].cost);
        }
        tilemap.SetTile(new Vector3Int(currentNode.x, currentNode.y, 0), blueTile);
    }

    public void exploreNeighbors(Node currentNode, Node[,] graph, List<Node> explored)
    {
        if (currentNode.x > 0)
        {
            addNeighbor(currentNode, currentNode.x - 1, currentNode.y, graph, explored);
            if (currentNode.y > 0)
            {
                addNeighbor(currentNode, currentNode.x - 1, currentNode.y-1, graph, explored);
                addNeighbor(currentNode, currentNode.x, currentNode.y - 1, graph, explored);
            }
            if (currentNode.y < height - 1)
            {
                addNeighbor(currentNode, currentNode.x - 1, currentNode.y + 1, graph, explored);
                addNeighbor(currentNode, currentNode.x, currentNode.y + 1, graph, explored);
            }
        }

        if (currentNode.x < width - 1)
        {
            addNeighbor(currentNode, currentNode.x + 1, currentNode.y, graph, explored);
            if (currentNode.y > 0)
            {
                addNeighbor(currentNode, currentNode.x + 1, currentNode.y - 1, graph, explored);
                addNeighbor(currentNode, currentNode.x, currentNode.y - 1, graph, explored);
            }
            if (currentNode.y < height - 1)
            {
                addNeighbor(currentNode, currentNode.x + 1, currentNode.y + 1, graph, explored);
                addNeighbor(currentNode, currentNode.x, currentNode.y + 1, graph, explored);
            }
        }
    }
}
