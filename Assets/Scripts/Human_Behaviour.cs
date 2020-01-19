using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Human_Behaviour : MonoBehaviour
{

    public GameObject cmap_object;
    Cell_Map cmap;

    public int x, y;
    // Neighbor Tiles Nummerierung
    //  5 6 7
    //  4 X 0   
    //  3 2 1

    // Weg finden die Bewegung kontinuierlich zu gestalten

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);
        cmap = cmap_object.GetComponent<Cell_Map>();
        x = Mathf.FloorToInt(transform.position.x);
        y = Mathf.FloorToInt(transform.position.y);
        cmap.cell_array[x, y].GetComponent<Cell>().is_occupied = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Move_Methode
            Move();
        }
    }

    public void Move()
    {
        // Get Direction bildet die Bewegungsrichtung der Agenten aus der Durchschnittsrichtung der möglichen Kandidaten der Zellen: Kontinuierlich
        // Move; Flags der Zelle werden auf false gesetzt, damit sie wieder reservierbar und begehbar sind; Bewegung wird durchgeführt; Nächste Zelle wird auf besetzt gesetzt;
        int i = getDirection();

        if (i >= 0 && cmap.cell_array[x, y].GetComponent<Cell>().object_type != 2)
        {
            cmap.cell_array[x, y].GetComponent<Cell>().is_occupied = false;
            transform.position = NeighborToCell(i);
            x = Mathf.FloorToInt(transform.position.x);
            y = Mathf.FloorToInt(transform.position.y);
            cmap.cell_array[x, y].GetComponent<Cell>().is_occupied = true;
        }
        else if (cmap.cell_array[x, y].GetComponent<Cell>().object_type == 2)
        {
            cmap.cell_array[x, y].GetComponent<Cell>().is_occupied = false;
            Destroy(gameObject);
        }
    }

    GameObject[] getNeighbors()
    {
        GameObject[] cell_array = new GameObject[8];

        cell_array[0] = NullOrCell(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y));
        cell_array[1] = NullOrCell(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y - 1));
        cell_array[2] = NullOrCell(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y - 1));
        cell_array[3] = NullOrCell(Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y - 1));
        cell_array[4] = NullOrCell(Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y));
        cell_array[5] = NullOrCell(Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y + 1));
        cell_array[6] = NullOrCell(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y + 1));
        cell_array[7] = NullOrCell(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y + 1));

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
            if (is_Walkable(i))
            {
                if (lowest > getNeighbors()[i].GetComponent<Cell>().travel_cost)
                {
                    lowest = getNeighbors()[i].GetComponent<Cell>().travel_cost;
                    direction = i;
                }
            }
        }

        for (int j = 0; j < getNeighbors().Length; j++)
        {
            if (is_Walkable(j))
            {
                if (lowest == getNeighbors()[j].GetComponent<Cell>().travel_cost)
                {
                    possible_directions[max_pos_neighbors] = j;
                    max_pos_neighbors++;
                }
            }
        }

        int k = -1;

        if (max_pos_neighbors > 0)
        {
            k = possible_directions[Random.Range(0, max_pos_neighbors)];
        }

        return k;
    }

    Vector3 NeighborToCell(int i)
    {
        return cmap.GetComponent<Cell_Map>().cell_array[Mathf.FloorToInt(getNeighbors()[i].transform.position.x), Mathf.FloorToInt(getNeighbors()[i].transform.position.y)].transform.position;
    }

    public bool is_Walkable(int i)
    {
        return (getNeighbors()[i] != null && getNeighbors()[i].GetComponent<Cell>().is_occupied != true && getNeighbors()[i].GetComponent<Cell>().object_type != 1 && getNeighbors()[i].GetComponent<Cell>().is_occupied != true);
    }

    public GameObject NullOrCell(int x, int y)
    {
        if (x >= 0 && x < cmap.x_range && y >= 0 && y < cmap.y_range)
        {
            return cmap.cell_array[x, y];
        }
        else
        {
            return null;
        }
    }
}
