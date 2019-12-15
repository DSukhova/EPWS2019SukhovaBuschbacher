using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    float height;
    float width;

    public Vector2Int position2d;

    public int x_pos;
    public int y_pos;

    public int object_type; // 0 = tile, 1 = blocking, 2 = Ziel, 3 = Quelle
    float max_repulsive_value = 10;
    float max_rep_distance = 2;
    float obst_value;
    float dest_value;
    float ges_value;
    float ped_value;

    public float traveltime = Mathf.Infinity; // Ankunftszeit der Welle/Flutung => relevant für FMM
    public bool activated = false;

    public GameObject[] neighbors;
    
    // Neighbor Tiles Nummerierung
    //  5 6 7
    //  4 X 0   
    //  3 2 1

    Cell_Map cmap;
    SpriteRenderer sprite;

    float obstacle_value;

    // Start is called before the first frame update
    void Start()
    { 
        cmap = GetComponentInParent<Cell_Map>();
        sprite = GetComponent<SpriteRenderer>();
        transform.position = new Vector3(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);

        x_pos = Mathf.FloorToInt(transform.position.x);
        y_pos = Mathf.FloorToInt(transform.position.y);
        position2d = new Vector2Int(x_pos,y_pos);

        neighbors = new GameObject[8];
        neighbors[0] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x + 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y, 0, cmap.y_range - 1))];
        neighbors[1] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x + 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y - 1, 0, cmap.y_range - 1))];
        neighbors[2] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y - 1, 0, cmap.y_range - 1))];
        neighbors[3] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x - 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y - 1, 0, cmap.y_range - 1))];
        neighbors[4] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x - 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y, 0, cmap.y_range - 1))];
        neighbors[5] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x - 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y + 1, 0, cmap.y_range - 1))];
        neighbors[6] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y + 1, 0, cmap.y_range - 1))];
        neighbors[7] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x + 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y + 1, 0, cmap.y_range - 1))];

        // Alle Zellen werden als weit entfernt markiert
        status = 0;
        
        if (transform.position.x == cmap.x_range || transform.position.y == cmap.y_range)
        {
            activated = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //ges_value = obstacle_value + dest_value + ped_value;
        
            sprite.color = new Color(1f, traveltime * 0.085f, 1f, 1f);
        
    }

    float get_Obst_Value()
    {
        float sum_cell_value = 0;

        foreach (GameObject o in cmap.obstacles)
        {
            sum_cell_value += Mathf.Max(0, max_repulsive_value * (1 - (((Vector3.Distance(transform.position, o.transform.position))) / max_rep_distance)));
        }

        return sum_cell_value;
    }

    public float get_Distance_Value()
    {
        float sum_cell_value = 0;

            foreach (GameObject o in cmap.destinations)
            {
                sum_cell_value += (Mathf.Floor(Vector3.Distance(transform.position, o.transform.position)));
            }

        return sum_cell_value;
    }

    public float get_Value()
    {
        return get_Obst_Value() + get_Distance_Value();
    }

    GameObject[] getNeighbors()
    {
        GameObject[] cell_array = new GameObject[8];

            cell_array[0] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x + 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y, 0, cmap.y_range - 1))];
            cell_array[1] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x + 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y - 1, 0, cmap.y_range - 1))];
            cell_array[2] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y - 1, 0, cmap.y_range - 1))];
            cell_array[3] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x - 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y - 1, 0,  cmap.y_range - 1))];
            cell_array[4] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x - 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y, 0, cmap.y_range - 1))];
            cell_array[5] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x - 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y + 1, 0, cmap.y_range - 1))];
            cell_array[6] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y + 1, 0, cmap.y_range - 1))];
            cell_array[7] = cmap.cell_array[Mathf.FloorToInt(Mathf.Clamp(transform.position.x + 1, 0, cmap.x_range - 1)), Mathf.FloorToInt(Mathf.Clamp(transform.position.y + 1, 0, cmap.y_range - 1))];

        return cell_array;
    }

}
