using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour 
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public GameObject markerPrefab;

    private List<GameObject> markers = new List<GameObject>();
    private bool canReset = false;
    int black = 0, red = 0, yellow = 0, green = 0;
    public Text txtBlack, txtRed, txtYellow, txtGreen;
    

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x<=1; x++)
            for (int y = -1; y<=1; y++)
                {
                    if(x == 0 && y == 0)
                    {
                        continue;
                    }
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    if(checkX >= 0 && checkX < gridSizeX && checkY >=0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
    public void DisplayResults()
    {

        canReset = true;
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                // create some blocks of colour dependant on node busyfactor
                GameObject temp;
                switch (grid[x,y].GetBusyFactor())
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                        temp = Instantiate(markerPrefab, worldPoint, Quaternion.Euler(new Vector3(90, 0, 0)));
                        temp.GetComponent<SpriteRenderer>().color = Color.green;
                        markers.Add(temp);
                        ++green;
                        break;
                    case 3:
                    case 4:
                        temp = Instantiate(markerPrefab, worldPoint, Quaternion.Euler(new Vector3(90, 0, 0)));
                        temp.GetComponent<SpriteRenderer>().color = Color.yellow;
                        markers.Add(temp);
                        ++yellow;
                        break;
                    case 5:
                        temp = Instantiate(markerPrefab, worldPoint, Quaternion.Euler(new Vector3(90, 0, 0)));
                        temp.GetComponent<SpriteRenderer>().color = Color.red;
                        markers.Add(temp);
                        ++red;
                        break;                    
                    default:
                        temp = Instantiate(markerPrefab, worldPoint, Quaternion.Euler(new Vector3(90, 0, 0)));
                        temp.GetComponent<SpriteRenderer>().color = Color.black;
                        markers.Add(temp);
                        ++black;
                        break;
                }
                grid[x,y].Restart();
            }
        }
        // display results
        txtBlack.text = "" + black;
        txtRed.text = "" + red;
        txtYellow.text = "" + yellow;
        txtGreen.text = "" + green;
        // reset for next
        black = 0;
        red = 0;
        yellow = 0;
        green = 0;
    }
    public void RemoveMarkers()
    {
        if(canReset)
            foreach(GameObject g in markers)
            {
                if(g != null)
                    Destroy(g);
            }
        canReset = false;
    }
    public bool GetCanReset()
    {
        return canReset;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid!= null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }

        }
    }
}
