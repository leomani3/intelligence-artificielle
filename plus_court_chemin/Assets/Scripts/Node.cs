using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public int cost;
    public int distance;
    public Node parent;

    public Node(int posx, int posy, int c, int d)
    {
        x = posx;
        y = posy;
        cost = c;
        distance = d;
        parent = null;
    }

    public bool Equal(Node n2)
    {
        return (this.x == n2.x && this.y == n2.y);
    }
}
