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
    float max_repulsive_value = 0.5f;
    float max_rep_distance = 2;
    float obst_value;
    float dest_value;
    float ped_value;
    public float travel_cost;

    public bool isNode = false;
    public bool invalidNode = false;
    public bool isEdge = false;
    public bool isNodeAlready = false;
    
    public float traveltime = Mathf.Infinity; // Ankunftszeit der Welle/Flutung => relevant für FMM
    public bool activated = false;

    public GameObject[] neighbors;

    public bool is_occupied;

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
        position2d = new Vector2Int(x_pos, y_pos);

        Init_Neighbors();
        set_Obst_Value();
    }

    private void Init_Neighbors()
    {
        neighbors = new GameObject[8];
        neighbors[0] = NullOrCell(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y));
        neighbors[1] = NullOrCell(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y - 1));
        neighbors[2] = NullOrCell(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y - 1));
        neighbors[3] = NullOrCell(Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y - 1));
        neighbors[4] = NullOrCell(Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y));
        neighbors[5] = NullOrCell(Mathf.FloorToInt(transform.position.x - 1), Mathf.FloorToInt(transform.position.y + 1));
        neighbors[6] = NullOrCell(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y + 1));
        neighbors[7] = NullOrCell(Mathf.FloorToInt(transform.position.x + 1), Mathf.FloorToInt(transform.position.y + 1));
    }

    // Update is called once per frame
    void Update()
    {
        //ges_value = obstacle_value + dest_value + ped_value;
        //sprite.color = new Color(1f, get_Value() * 0.085f, 1f, 1f);
        set_Value();
        if (isNodeAlready == true)
        {
            sprite.color = new Color(0.75f, 0.2f, 0f, 1f);
        } else if (object_type != 2)
        
        {
            sprite.color = new Color(1f, travel_cost * 0.050f, 1f, 1f);
        }
    }

    public void set_Obst_Value()
    {
        float sum_cell_value = 0;

        foreach (Vector2Int o in cmap.obstacles)
        {
            sum_cell_value += Mathf.Max(0, max_repulsive_value * (1 - (((Vector2Int.Distance(this.position2d, o))) / max_rep_distance)));
        }

        obstacle_value = sum_cell_value;
    }

    public void set_Value()
    {
        travel_cost = obstacle_value + traveltime;
    }

    public void set_Ped_Value()
    {

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

    public GameObject NullOrCell(int x, int y)
    {
        if (x >= 0 && x < cmap.x_range && y >= 0 && y < cmap.y_range)
        {
            return cmap.cell_array[x,y];
        } else
        {
            return null;
        }
    }
}
