using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public int width;
    public int height;
    public Tile whiteTile;
    public Tile redTile;
    public Tile greenTile;
    public Tile blackTile;
    public Tile blueTile;
    public Tilemap tilemap;

    private Vector3Int startPoint;
    private Vector3Int endPoint;
    private bool startPointDefined;
    private bool endPointDefined;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), whiteTile);
            }
        }

        startPoint = new Vector3Int(0, 0, 0);
        endPoint = new Vector3Int(width - 1, height - 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("game");
    }

    public IEnumerator game()
    {
        List<Node> chemin = AstarGame(startPoint, endPoint);
        Debug.Log("1");
        tilemap.SetTile(startPoint, whiteTile);
        Debug.Log("2");
        startPoint = new Vector3Int(chemin[0].x, chemin[0].y, 0);
        Debug.Log("3");
        tilemap.SetTile(startPoint, redTile);
        Debug.Log("4");

        tilemap.SetTile(endPoint, whiteTile);
        Debug.Log("5");
        int x=0, y=0;
        if (endPoint.x == 0)
        {
            do
            {
                x = Random.Range(-1, 2);
            } while (x == -1);
        }
        if (endPoint.x == width-1)
        {
            do
            {
                x = Random.Range(-1, 2);
            } while (x == 1);
        }

        if (endPoint.y == 0)
        {
            do
            {
                y = Random.Range(-1, 2);
            } while (y == -1);
        }
        if (endPoint.y == height - 1)
        {
            do
            {
                y = Random.Range(-1, 2);
            } while (y == 1);
        }
        Debug.Log("6");
        endPoint += new Vector3Int(x, y, 0);
        Debug.Log("7");
        tilemap.SetTile(endPoint, greenTile);
        Debug.Log("8");
        yield return new WaitForSeconds(2);
        Debug.Log("9");
    }

    public List<Node> AstarGame(Vector3Int startPoint, Vector3Int endPoint)
    {
        List<Node> chemin = new List<Node>();
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
                if (tilemap.GetTile(new Vector3Int(i, j, 0)).name.Contains("green"))
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
            exploreNeighbors(currentNode, graph, explored, 1, startNode, endNode);
            //on tri explored dans l'ordre croissant des couts
            explored.Sort(SortByCost);
            //choisir le plus bas cout parmis les noeuds explorés. par défaut explored[0] grace au tri.
            currentNode = explored[0];
            explored.RemoveAt(0);
        }
        //exploration terminée. Il faut maintenant chercher le chemin !
        while (!currentNode.Equal(startNode))
        {
            tilemap.SetTile(new Vector3Int(currentNode.x, currentNode.y, 0), greenTile);
            chemin.Add(currentNode);
            currentNode = currentNode.parent;
        }
        chemin.Reverse();
        return chemin;
    }

    static int SortByCost(Node n1, Node n2)
    {
        return n1.cost.CompareTo(n2.cost);
    }

    public void addNeighbor(Node currentNode, int neighborX, int neighborY, Node[,] graph, List<Node> explored, int algo, Node startNode, Node endNode)
    {
        if (algo == 0)
        {
            if (graph[neighborX, neighborY].cost == -1 || (graph[neighborX, neighborY].cost != -2 && graph[neighborX, neighborY].cost != -1 && currentNode.cost + graph[neighborX, neighborY].distance < graph[neighborX, neighborY].cost))
            {
                graph[neighborX, neighborY].cost = currentNode.cost + graph[neighborX, neighborY].distance;
                graph[neighborX, neighborY].parent = currentNode;
                explored.Add(graph[neighborX, neighborY]);
                //Debug.Log("current node : " + currentNode.x + " " + currentNode.y + "voisin ajouté : " + neighborX + " " + neighborY + "avec un nouveau cout de : " + graph[neighborX, neighborY].cost);
                tilemap.SetTile(new Vector3Int(neighborX, neighborY, 0), blueTile);
            }
            //tilemap.SetTile(new Vector3Int(currentNode.x, currentNode.y, 0), blueTile);
        }
        else if (algo == 1)
        {
            if (graph[neighborX, neighborY].cost == -1)
            {
                graph[neighborX, neighborY].cost = graph[neighborX, neighborY].distance + distanceTo(new Node(neighborX, neighborY, 0, 0), endNode);
                graph[neighborX, neighborY].parent = currentNode;
                explored.Add(graph[neighborX, neighborY]);
                //Debug.Log("current node : " + currentNode.x + " " + currentNode.y + "voisin ajouté : " + neighborX + " " + neighborY + "avec un nouveau cout de : " + graph[neighborX, neighborY].cost+" distance : "+ distanceTo(new Node(neighborX, neighborY, 0, 0), endNode));
                tilemap.SetTile(new Vector3Int(neighborX, neighborY, 0), blueTile);
            }
        }
    }

    public int distanceTo(Node currentNode, Node obj)
    {
        return Mathf.Max(Mathf.Abs(currentNode.x - obj.x), Mathf.Abs(currentNode.y - obj.y));
    }


    public void exploreNeighbors(Node currentNode, Node[,] graph, List<Node> explored, int algo, Node startNode, Node endNode)
    {
        if (currentNode.x > 0)
        {
            addNeighbor(currentNode, currentNode.x - 1, currentNode.y, graph, explored, algo, startNode, endNode);
            if (currentNode.y > 0)
            {
                addNeighbor(currentNode, currentNode.x - 1, currentNode.y - 1, graph, explored, algo, startNode, endNode);
                addNeighbor(currentNode, currentNode.x, currentNode.y - 1, graph, explored, algo, startNode, endNode);
            }
            if (currentNode.y < height - 1)
            {
                addNeighbor(currentNode, currentNode.x - 1, currentNode.y + 1, graph, explored, algo, startNode, endNode);
                addNeighbor(currentNode, currentNode.x, currentNode.y + 1, graph, explored, algo, startNode, endNode);
            }
        }

        if (currentNode.x < width - 1)
        {
            addNeighbor(currentNode, currentNode.x + 1, currentNode.y, graph, explored, algo, startNode, endNode);
            if (currentNode.y > 0)
            {
                addNeighbor(currentNode, currentNode.x + 1, currentNode.y - 1, graph, explored, algo, startNode, endNode);
                addNeighbor(currentNode, currentNode.x, currentNode.y - 1, graph, explored, algo, startNode, endNode);
            }
            if (currentNode.y < height - 1)
            {
                addNeighbor(currentNode, currentNode.x + 1, currentNode.y + 1, graph, explored, algo, startNode, endNode);
                addNeighbor(currentNode, currentNode.x, currentNode.y + 1, graph, explored, algo, startNode, endNode);
            }
        }
    }
}
