using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell_Map : MonoBehaviour
{

    public int x_range, y_range;
    public float x_gap, y_gap;
    public GameObject[,] cell_array;
    public List<GameObject> obstacles;
    public List<GameObject> destinations;

    public GameObject blocking;
    public GameObject tile;
    public GameObject destination;
    public GameObject source;

    public float[,] Distance_Map;
    public bool[,] state;

    // Start is called before the first frame update
    void Start()
    {
        Distance_Map = new float[x_range, y_range];
        state = new bool[x_range, y_range];

        generateTilemap();
        Destroy(cell_array[10, 10]);
        Destroy(cell_array[10, 11]);
        Destroy(cell_array[10, 12]);
        Destroy(cell_array[10, 13]);
        Destroy(cell_array[10, 9]);
        Destroy(cell_array[10, 8]);
        Destroy(cell_array[10, 7]);
        Destroy(cell_array[10, 6]);
        //Destroy(cell_array[15, 12]);
        Destroy(cell_array[13, 12]);

        Destroy(cell_array[6, 13]);
        Destroy(cell_array[7, 13]);
        Destroy(cell_array[8, 13]);
        Destroy(cell_array[9, 13]);
        Destroy(cell_array[6, 6]);
        Destroy(cell_array[7, 6]);
        Destroy(cell_array[8, 6]);
        Destroy(cell_array[9, 6]);

        //cell_array[15, 12] = Instantiate(destination, new Vector3(15,12, 0), Quaternion.identity, this.transform);
        cell_array[13, 12] = Instantiate(destination, new Vector3(13, 12, 0), Quaternion.identity, this.transform);

        // C förmiges Hindernis, Mitte
        cell_array[10, 10] = Instantiate(blocking, new Vector3(10, 10, 0), Quaternion.identity, this.transform);
        cell_array[10, 11] = Instantiate(blocking, new Vector3(10, 11, 0), Quaternion.identity, this.transform);
        cell_array[10, 12] = Instantiate(blocking, new Vector3(10, 12, 0), Quaternion.identity, this.transform);
        cell_array[10, 13] = Instantiate(blocking, new Vector3(10, 13, 0), Quaternion.identity, this.transform);
        cell_array[10, 9] = Instantiate(blocking, new Vector3(10, 9, 0), Quaternion.identity, this.transform);
        cell_array[10, 8] = Instantiate(blocking, new Vector3(10, 8, 0), Quaternion.identity, this.transform);
        cell_array[10, 7] = Instantiate(blocking, new Vector3(10, 7, 0), Quaternion.identity, this.transform);
        cell_array[10, 6] = Instantiate(blocking, new Vector3(10, 6, 0), Quaternion.identity, this.transform);

        cell_array[6, 13] = Instantiate(blocking, new Vector3(6, 13, 0), Quaternion.identity, this.transform);
        cell_array[7, 13] = Instantiate(blocking, new Vector3(7, 13, 0), Quaternion.identity, this.transform);
        cell_array[8, 13] = Instantiate(blocking, new Vector3(8, 13, 0), Quaternion.identity, this.transform);
        cell_array[9, 13] = Instantiate(blocking, new Vector3(9, 13, 0), Quaternion.identity, this.transform);
        cell_array[6, 6] = Instantiate(blocking, new Vector3(6, 6, 0), Quaternion.identity, this.transform);
        cell_array[7, 6] = Instantiate(blocking, new Vector3(7, 6, 0), Quaternion.identity, this.transform);
        cell_array[8, 6] = Instantiate(blocking, new Vector3(8, 6, 0), Quaternion.identity, this.transform);
        cell_array[9, 6] = Instantiate(blocking, new Vector3(9, 6, 0), Quaternion.identity, this.transform);

        for (int i = 0; i < x_range; i++)
        {
            for (int j = 0; j < y_range; j++)
            {
                if (cell_array[i, j].GetComponent<Cell>().object_type == 1)
                {
                    obstacles.Add(cell_array[i, j]);
                }

                if (cell_array[i, j].GetComponent<Cell>().object_type == 2)
                {
                    destinations.Add(cell_array[i, j]);
                }
            }
        }

        for (int k = 0; k < x_range; k++)
        {
            Destroy(cell_array[k, y_range - 1]);
            Destroy(cell_array[k, 0]);
            Instantiate(blocking, new Vector3(k, y_range - 1, 0), Quaternion.identity, this.transform);
            Instantiate(blocking, new Vector3(k, 0, 0), Quaternion.identity, this.transform);

        }

        for (int l = 0; l < y_range; l++)
        {
            Destroy(cell_array[x_range - 1, l]);
            Destroy(cell_array[0, l]);
            Instantiate(blocking, new Vector3(x_range - 1, l, 0), Quaternion.identity, this.transform);
            Instantiate(blocking, new Vector3(0, l, 0), Quaternion.identity, this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey(KeyCode.W))
        {
            Flood_Fill(13, 12);
        }


    }

    void generateTilemap()
    {
        cell_array = new GameObject[x_range, y_range];

        for (int i = 0; i < x_range; i++)
        {
            for (int j = 0; j < y_range; j++)
            {
                cell_array[i, j] = Instantiate(tile, new Vector3(i, j, 0), Quaternion.identity, this.transform);
            }
        }
    }

    void Flood_Fill(int x_start, int y_start)
    {

        List<Vector2Int> candidates = new List<Vector2Int>();
        
        Vector2Int lowest_candidate = new Vector2Int(- 1, - 1);
        Vector2Int temp_neighbor;
        float dx;
        float dy;
        float d_1_to_5;
        float d_3_to_7;

        for (int i = 0; i < x_range; i++)
        {
            for (int j = 0; j < y_range; j++)
            {
                Distance_Map[i, j] = Mathf.Infinity;
                state[i, j] = true;
            }
        }

        Distance_Map[x_start, y_start] = 0;
        cell_array[x_start, y_start].GetComponent<Cell>().traveltime = 0;

        // Anstatt Gameobjects Koordinaten verwenden => Duplikationsproblem
        candidates.Add(cell_array[x_start, y_start].GetComponent<Cell>().position2d);

        while (candidates.Count > 0)
        {
            float lowest = Mathf.Infinity;

            foreach (Vector2Int o in candidates)
            {
                if (Distance_Map[o.x, o.y] <= lowest)
                {
                    lowest = Distance_Map[o.x, o.y];
                    lowest_candidate = o;
                }
            }

            state[lowest_candidate.x, lowest_candidate.y] = false;

            candidates.RemoveAll(o => (o.x == lowest_candidate.x && o.y == lowest_candidate.y));
            
            // Nachbarn des Lowest Candidate

            for (int k = 0; k < cell_array[lowest_candidate.x, lowest_candidate.y].GetComponent<Cell>().neighbors.Length; k++)
            {

                if (cell_array[lowest_candidate.x, lowest_candidate.y].GetComponent<Cell>().neighbors[k] != null)
                {
                    temp_neighbor = cell_array[lowest_candidate.x, lowest_candidate.y].GetComponent<Cell>().neighbors[k].GetComponent<Cell>().position2d;
                } else
                {
                    continue;
                }

                // für alle lebenden Nachbarn führe aus
                // Neighbor Tiles Nummerierung
                //  5 6 7
                //  4 X 0   
                //  3 2 1

                if (temp_neighbor != null && state[temp_neighbor.x, temp_neighbor.y] && cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().object_type != 1)
                {

                    candidates.Add(temp_neighbor);

                    float d_right = Mathf.Infinity;
                    float d_left = Mathf.Infinity;
                    float d_down = Mathf.Infinity;
                    float d_up = Mathf.Infinity;
                    float d_1 = Mathf.Infinity;
                    float d_3 = Mathf.Infinity;
                    float d_5 = Mathf.Infinity;
                    float d_7 = Mathf.Infinity;


                    // Werte der Distance Map: Abstände von rechts, links, unten, oben zur Nachbarzelle k
                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[0] != null)
                    {
                        d_right = Distance_Map[temp_neighbor.x + 1, temp_neighbor.y];
                    }

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[4] != null)
                    {
                        d_left = Distance_Map[temp_neighbor.x - 1, temp_neighbor.y];
                    }

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[2] != null)
                    {
                        d_down = Distance_Map[temp_neighbor.x, temp_neighbor.y - 1];
                    }

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[6] != null)
                    {
                        d_up = Distance_Map[temp_neighbor.x, temp_neighbor.y + 1];
                    }

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[1] != null)
                    {
                        d_1 = Distance_Map[temp_neighbor.x + 1, temp_neighbor.y - 1];
                    }

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[3] != null)
                    {
                        d_3 = Distance_Map[temp_neighbor.x - 1, temp_neighbor.y - 1];
                    }

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[5] != null)
                    {
                        d_5 = Distance_Map[temp_neighbor.x - 1, temp_neighbor.y + 1];
                    }

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().neighbors[7] != null)
                    {
                        d_7 = Distance_Map[temp_neighbor.x + 1, temp_neighbor.y + 1];
                    }

                    dx = Mathf.Min(d_right, d_left);
                    dy = Mathf.Min(d_down, d_up);
                    d_1_to_5 = Mathf.Min(d_1, d_5);
                    d_3_to_7 = Mathf.Min(d_3, d_7);

                    float lowest_even = Mathf.Min(dx,dy);
                    float lowest_odd = Mathf.Min(d_1_to_5, d_3_to_7);

                    // Nach L-Norm W(j) unterscheiden (4er oder 8er Nachbarschaft) / Funktion schreiben

                    float w = 1;

                    // Nach Dijkstra
                    float dijkstra_value = Mathf.Min(dx + w, dy + w, d_1_to_5 + w, d_3_to_7 + w);

                    // Nach Fast Marching Method
                    float fmm_value;
                    if (lowest_even <= lowest_odd)
                    {
                        fmm_value = lowest_even + w / 2;
                    }
                    else if (w<= 2*Mathf.Sqrt(2)*(lowest_even-lowest_odd))
                    {
                        fmm_value = lowest_odd + Mathf.Sqrt(2) * w / 2;
                    } else
                    {
                        fmm_value = lowest_even + Mathf.Sqrt(Mathf.Pow(w,2) - 4*(Mathf.Pow(lowest_even - lowest_odd, 2))) / 2;
                    }
                    
                    Distance_Map[temp_neighbor.x, temp_neighbor.y] = fmm_value;
                    cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().traveltime = fmm_value;

                }
            }
        }
    }
}
