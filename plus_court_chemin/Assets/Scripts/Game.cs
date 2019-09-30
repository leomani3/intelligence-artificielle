using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public int width;
    public int height;
    public Tile whiteTile;
    public Tile redTile;
    public Tile greenTile;
    public Tile blackTile;
    public Tile blueTile;
    public Tile playerTile;
    public Tile bonusTile;
    public Tilemap tilemap;

    [Header("Gameplay settings")]
    public float timeBetweenEnemyMove;
    public float timeBetweenBonusSpawn;

    private Vector3Int enemyPos;
    private Vector3Int playerPos;
    private bool gameStarted = false;
    private List<Vector3Int> chemin;
    private bool playerMoved = false;

    private float timeElapsed = 0;
    private float timeElapsedBonus = 0;
    // Start is called before the first frame update
    void Start()
    {
        chemin = new List<Vector3Int>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), whiteTile);
            }
        }

        enemyPos = new Vector3Int(Random.Range(0, width), Random.Range(0, height), 0);
        tilemap.SetTile(new Vector3Int(enemyPos.x, enemyPos.y, 0), redTile);
        playerPos = new Vector3Int(Random.Range(0, width), Random.Range(0, height), 0);
        tilemap.SetTile(new Vector3Int(playerPos.x, playerPos.y, 0), playerTile);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (playerPos.x < width - 1 && !tilemap.GetTile(new Vector3Int(playerPos.x + 1, playerPos.y, 0)).name.Contains("black"))
            {
                if (tilemap.GetTile(new Vector3Int(playerPos.x + 1, playerPos.y, 0)).name.Contains("BONUS"))
                {
                    timeBetweenEnemyMove += 0.05f;
                    tilemap.SetTile(new Vector3Int(playerPos.x + 1, playerPos.y, 0), whiteTile);
                }
                tilemap.SetTile(playerPos, whiteTile);
                playerPos.x += 1;
                tilemap.SetTile(playerPos, playerTile);
                playerMoved = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (playerPos.x > 0 && !tilemap.GetTile(new Vector3Int(playerPos.x - 1, playerPos.y, 0)).name.Contains("black"))
            {
                if (tilemap.GetTile(new Vector3Int(playerPos.x - 1, playerPos.y, 0)).name.Contains("BONUS"))
                {
                    timeBetweenEnemyMove += 0.05f;
                    tilemap.SetTile(new Vector3Int(playerPos.x - 1, playerPos.y, 0), whiteTile);
                }
                tilemap.SetTile(playerPos, whiteTile);
                playerPos.x -= 1;
                tilemap.SetTile(playerPos, playerTile);
                playerMoved = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (playerPos.y < height - 1 && !tilemap.GetTile(new Vector3Int(playerPos.x, playerPos.y + 1, 0)).name.Contains("black"))
            {
                if (tilemap.GetTile(new Vector3Int(playerPos.x, playerPos.y + 1, 0)).name.Contains("BONUS"))
                {
                    timeBetweenEnemyMove += 0.05f;
                    tilemap.SetTile(new Vector3Int(playerPos.x, playerPos.y + 1, 0), whiteTile);
                }
                tilemap.SetTile(playerPos, whiteTile);
                playerPos.y += 1;
                tilemap.SetTile(playerPos, playerTile);
                playerMoved = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (playerPos.y > 0 && !tilemap.GetTile(new Vector3Int(playerPos.x, playerPos.y - 1, 0)).name.Contains("black"))
            {
                if (tilemap.GetTile(new Vector3Int(playerPos.x, playerPos.y - 1, 0)).name.Contains("BONUS"))
                {
                    timeBetweenEnemyMove += 0.05f;
                    tilemap.SetTile(new Vector3Int(playerPos.x, playerPos.y - 1, 0), whiteTile);
                }
                tilemap.SetTile(playerPos, whiteTile);
                playerPos.y -= 1;
                tilemap.SetTile(playerPos, playerTile);
                playerMoved = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            gameStarted = true;
        }
        if (gameStarted)
        {
            tilemap.SetTile(new Vector3Int(playerPos.x, playerPos.y, 0), playerTile);
            if (playerMoved || chemin.Count == 0)
            {
                chemin = AstarGame(enemyPos, playerPos);
                drawPath();
                playerMoved = false;
            }
            movEnemy();
            spawnBonus();
        }

        /*if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (startPointDefined && endPointDefined)
            {
                StartCoroutine(Astar(currentStartPoint, currentEndPoint, 0.05f));
            }
            else
            {
                Debug.Log("Veuillez définir un point de départ et un point d'arrivée");
            }
        }*/
    }

    public void spawnBonus()
    {
        if (timeElapsedBonus >= timeBetweenBonusSpawn)
        {
            Vector3Int randomPos = new Vector3Int(Random.Range(0, width), Random.Range(0, height), 0);
            while (randomPos == playerPos || randomPos == enemyPos)
            {
                randomPos = new Vector3Int(Random.Range(0, width), Random.Range(0, height), 0);
            }

            tilemap.SetTile(randomPos, bonusTile);
            timeElapsedBonus = 0;
            timeBetweenBonusSpawn = Random.Range(2.0f, 7.0f);
        }
        timeElapsedBonus += Time.deltaTime;
    }

    public void drawPath()
    {
        for (int i = 0; i < chemin.Count-1; i++)
        {
            if (!tilemap.GetTile(chemin[i]).name.Contains("BONUS"))
            {
                tilemap.SetTile(chemin[i], greenTile);
            }
        }
    }

    public void movEnemy()
    {
        if (timeElapsed >= timeBetweenEnemyMove && chemin.Count > 0)
        {
            tilemap.SetTile(enemyPos, whiteTile);
            enemyPos = chemin[0];
            tilemap.SetTile(enemyPos, redTile);
            chemin.RemoveAt(0);
            timeElapsed = 0;
            if (timeBetweenEnemyMove > 0.2f)
            {
                timeBetweenEnemyMove -= 0.05f;
            }
        }
        timeElapsed += Time.deltaTime;

        testWin();
    }

    public void testWin()
    {
        if(enemyPos.x == playerPos.x && enemyPos.y == playerPos.y)
        {
            Debug.Log("VOUS AVEZ PERDU !!!!!!");
            SceneManager.LoadScene(0);
            Time.timeScale = 0.0f;
        }
    }

    public void resetPath()
    {
        for (int i = 0; i < chemin.Count; i++)
        {
            tilemap.SetTile(chemin[i], whiteTile);
        }
        chemin.Clear();
    }


    public List<Vector3Int> AstarGame(Vector3Int startPoint, Vector3Int endPoint)
    {
        //initialisation
        resetPath();

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
            /*tilemap.SetTile(new Vector3Int(startNode.x, startNode.y, 0), greenTile);
            tilemap.SetTile(new Vector3Int(endNode.x, endNode.y, 0), redTile);*/
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
            chemin.Add(new Vector3Int(currentNode.x, currentNode.y, 0));
            currentNode = currentNode.parent;
        }
        chemin.Reverse();
        return chemin;
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

    public static int SortByCost(Node n1, Node n2)
    {
        return n1.cost.CompareTo(n2.cost);
    }

    public int distanceTo(Node currentNode, Node obj)
    {
        return Mathf.Max(Mathf.Abs(currentNode.x - obj.x), Mathf.Abs(currentNode.y - obj.y));
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
            }
        }
    }
}
