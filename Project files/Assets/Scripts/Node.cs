using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;
    private List<Agent> Currentque = new List<Agent>();
    private bool isFull = false;
    private int busyFactor = 0;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
    public int fCost
    {
        get { return gCost + hCost; }
    }
    public void AddToQue(Agent agent)
    {
        isFull = true;
        Currentque.Add(agent);
        if(Currentque.Count == 1)
        {
            Currentque[0].CanMove();
            busyFactor++;
        }
    }
    public void RemoveFromQue()
    {
        if(Currentque.Count > 0)
        {
            Currentque.RemoveAt(0);
        }
        if (Currentque.Count != 0)
        {
            Currentque[0].CanMove();
            busyFactor++;
        }
        else
        {
            isFull = false;
        }
    }
    public bool GetIsFull()
    {
        return isFull;
    }
    public int GetBusyFactor()
    {
        return busyFactor;
    }
    public void Restart()
    {
        busyFactor = 0;
        Currentque.Clear();
        isFull = false;
    }

}
