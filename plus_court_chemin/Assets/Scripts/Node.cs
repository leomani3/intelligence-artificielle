using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3Int pos;
    public int cost;
    public int costMalus;

    public Node(Vector3Int p, int c, int cm)
    {
        pos = p;
        cost = c;
        costMalus = cm;
    }
}
