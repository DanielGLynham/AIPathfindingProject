using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Transform target;
    float speed = 5;
    Vector3[] path;
    int targetIndex;
    private bool canMoveNext = true;
    private Pathfinding pathfinding;
    Node currentNode, previousNode;
    private bool active = false;
    private Vector3 originalPos;

    private void Start()
    {
        pathfinding = GameObject.Find("A*").GetComponent<Pathfinding>();
    }
    public void Alert(Transform target)
    {
        originalPos = this.transform.position;
        this.target = target;
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        active = true;
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while(true && active)
        {
            if(transform.position == currentWaypoint)
            {
                previousNode = currentNode;
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    active = false;
                    previousNode.RemoveFromQue();
                    previousNode = null;
                    pathfinding.Finished();
                    yield break;
                }
                currentWaypoint = path[targetIndex];
                currentNode = pathfinding.GetNodeFromCoords(currentWaypoint);
                canMoveNext = false;
                currentNode.AddToQue(this);
            }
            if (canMoveNext)
            {
                if (previousNode != null)
                {
                    previousNode.RemoveFromQue();
                    previousNode = null;
                }
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            }
            yield return null;
        }
    }
    public void CanMove()
    {
        canMoveNext = true;
    }
    public bool GetActive()
    {
        return active;
    }
    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for(int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
    public void Restart()
    {
        active = false;
        canMoveNext = true;
        targetIndex = 0;
        this.transform.position = originalPos;
    }
}
