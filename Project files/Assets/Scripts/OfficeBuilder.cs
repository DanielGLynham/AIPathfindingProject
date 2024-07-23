using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeBuilder : MonoBehaviour
{
    public GameObject wall, exit, obstacle, seeker, cursor;
    LayerMask unWalkable;
    Grid grid;
    bool readyToStart = true;
    private int selectedOption = 0; // 0 = wall, 1 = exit, 2 = obs, 3 = seeker, 4 = eraser
    public void SelectOption(int op)
    {
        selectedOption = op;
    }
    private void Start()
    {
        grid = GameObject.Find("A*").GetComponent<Grid>();
        cursor = Instantiate(cursor, Input.mousePosition.normalized, Quaternion.Euler(new Vector3(90, 0, 0)));
        unWalkable = LayerMask.GetMask("unwalkable");
    }
    public void Restart()
    {
        if (grid.GetCanReset())
        {
            grid.RemoveMarkers();
            foreach (GameObject a in GameObject.FindGameObjectsWithTag("Seeker"))
            {
                a.GetComponent<Agent>().Restart();
            }
        }
        readyToStart = true;
    }

    private void Update()
    {
        // if mouse is in grid
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8.5f);
        Vector3 worldCoors = Camera.main.ScreenToWorldPoint(screenPosition);

        if(worldCoors.x >= 0)
            worldCoors.x = (int)(worldCoors.x) + 0.5f;
        else 
            worldCoors.x = (int)(worldCoors.x) - 0.5f;
        if (worldCoors.z >= 0)
            worldCoors.z = (int)(worldCoors.z) + 0.5f;
        else
            worldCoors.z = (int)(worldCoors.z) - 0.5f;
        worldCoors.y = -1;


        if(worldCoors.x >= -18.5f && worldCoors.x <= 17.5f && worldCoors.z >= -10.5f && worldCoors.z <= 13.5f)
        { 
            cursor.transform.position = worldCoors;
            worldCoors.y = 0;
            if (Input.GetMouseButtonDown(0))
            {
                switch (selectedOption)
                {
                    case 0:
                        // Create wall
                        Instantiate(wall, worldCoors, Quaternion.identity);
                        break;
                    case 1:
                        // if no exits yet
                        // Create Exit
                        if(GameObject.FindGameObjectWithTag("Target") == null)
                            Instantiate(exit, worldCoors, Quaternion.identity);
                        break;
                    case 2:
                        // create obstacle
                        Instantiate(obstacle, worldCoors, Quaternion.identity);
                        break;
                    case 3:
                        // create seeker
                        Instantiate(seeker, worldCoors, Quaternion.identity);
                        break;
                    case 4: 

                        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 1000))
                        {
                            if (hit.transform.gameObject.layer == 9 || hit.transform.gameObject.layer == 10)
                            {
                                Destroy(hit.transform.gameObject);
                            }
                        }
                        break;
                }
            }
        }
    }
    public bool ReadyToStart()
    {
        return readyToStart;
    }
    public void SetReadyToStart(bool toggle)
    {
        readyToStart = toggle;
    }
    public void Exit()
    {
        Application.Quit();
    }
}
