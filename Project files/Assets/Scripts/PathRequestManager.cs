using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestsQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;
    Grid grid;
    OfficeBuilder officeBuilder;

    bool isProcessingPath;
    GameObject[] seekers;
    GameObject target;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
        grid = GetComponent<Grid>();
        officeBuilder = GetComponent<OfficeBuilder>();
    }
    public void WakeAgents()
    {
        if (officeBuilder.ReadyToStart())
        {
            officeBuilder.SetReadyToStart(false);
            seekers = GameObject.FindGameObjectsWithTag("Seeker");
            target = GameObject.FindGameObjectWithTag("Target");
            grid.CreateGrid();
            if (seekers.Length != 0 && target != null)
            {
                foreach (GameObject a in seekers)
                {
                    a.GetComponent<Agent>().Alert(target.GetComponent<Transform>());
                }
            }
            else
            {
                Debug.Log("Must have at least one Seeker and an exit.");
            }
        }
    }
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestsQueue.Enqueue(newRequest);
        instance.TryProccessNext();
    }

    void TryProccessNext()
    {
        if(!isProcessingPath && pathRequestsQueue.Count > 0)
        {
            currentPathRequest = pathRequestsQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProccessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
    public void FinishedPath()
    {
        bool finished = true;
        foreach (GameObject a in seekers)
        {
            if(a.GetComponent<Agent>().GetActive())
            {
                finished = false;
                return; 
            }
        }
        if(finished)
        {
            grid.DisplayResults();
        }
    }
}
