using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Human_Behaviour : MonoBehaviour
{

    public GameObject cmap_object;
    Cell_Map cmap;

    float ped_value;

    // Neighbor Tiles Nummerierung
    //  5 6 7
    //  4 X 0   
    //  3 2 1

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);
        cmap = cmap_object.GetComponent<Cell_Map>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position = NeighborToCell(getDirection());
        }

    }

    GameObject[] getNeighbors()
    {
        GameObject[] cell_array = new GameObject[8];

        cell_array[0] = cmap.cell_array[Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y)];
        cell_array[1] = cmap.cell_array[Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y - 1)];
        cell_array[2] = cmap.cell_array[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y - 1)];
        cell_array[3] = cmap.cell_array[Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y - 1)];
        cell_array[4] = cmap.cell_array[Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y)];
        cell_array[5] = cmap.cell_array[Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y + 1)];
        cell_array[6] = cmap.cell_array[Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y + 1)];
        cell_array[7] = cmap.cell_array[Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y + 1)];

        return cell_array;
    }

    int getDirection()
    {
        // get lowest cell_value
        float lowest = Mathf.Infinity;
        int direction = 0;
        int[] possible_directions = new int[8];
        int max_pos_neighbors = 0;
        
        for (int i = 0; i < getNeighbors().Length; i++)
        {
            if (getNeighbors()[i] != null)
            {
                if (lowest > getNeighbors()[i].GetComponent<Cell>().traveltime)
                {
                    lowest = getNeighbors()[i].GetComponent<Cell>().traveltime;
                    direction = i;
                }
            }
        }

        for (int j = 0; j < getNeighbors().Length; j++)
        {
            if (getNeighbors()[j] != null)
            {
                if (lowest == getNeighbors()[j].GetComponent<Cell>().traveltime)
                {
                    possible_directions[max_pos_neighbors] = j;
                    max_pos_neighbors++;
                }
            }
        }

        return possible_directions[Random.Range(0, max_pos_neighbors)];
    }

    Vector3 NeighborToCell(int i)
    {
        return cmap.GetComponent<Cell_Map>().cell_array[Mathf.FloorToInt(getNeighbors()[i].transform.position.x), Mathf.FloorToInt(getNeighbors()[i].transform.position.y)].transform.position;
    }
}
