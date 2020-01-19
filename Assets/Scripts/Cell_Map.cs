using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using System.Linq;


public class Cell_Map : MonoBehaviour
{


    public int x_range, y_range;
    public float x_gap, y_gap;
    public GameObject[,] cell_array;
    public List<Vector2Int> obstacles;
    public List<Vector2Int> destinations;
    public List<Vector2Int> sources;
    public List<Human_Behaviour> agents;
    public List<Vector2Int> nav_nodes;
    public List<Node> nodes;
    public GameObject blocking;
    public GameObject tile;
    public GameObject destination;
    public GameObject source;
    public Graph graph;

    public float[,] Distance_Map;
    public bool[,] state;
    float penalty_factor = 0.5f; // veränderbarer Parameter für FFM; Einbeziehung von Personenhindernissen
    int node_distance = 2; // erhöht die Distanz der Navigationsknoten zu den Hindernissen

    // Start is called before the first frame update
    void Start()
    {
        graph = new Graph();

        Distance_Map = new float[x_range, y_range];
        state = new bool[x_range, y_range];
        nodes = new List<Node>();

        GenerateTilemap();
        Custom_Tilemap();
        Instantiate_Boundaries();
        Destroy(cell_array[0, 5]);
        cell_array[0, 5] = Instantiate(source, new Vector3(0, 5, 0), Quaternion.identity, this.transform);
        Destroy(cell_array[29, 27]);
        cell_array[29, 27] = Instantiate(destination, new Vector3(29, 27, 0), Quaternion.identity, this.transform);
        GetAllObstaclesAndDestinations();
        GetAllAgents();
    }

    private void GetAllObstaclesAndDestinations()
    {
        for (int i = 0; i < x_range; i++)
        {
            for (int j = 0; j < y_range; j++)
            {
                if (cell_array[i, j].GetComponent<Cell>().object_type == 1)
                {
                    obstacles.Add(new Vector2Int((int) (cell_array[i, j].GetComponent<Cell>().transform.position.x), (int) (cell_array[i, j].GetComponent<Cell>().transform.position.y)));
                }

                if (cell_array[i, j].GetComponent<Cell>().object_type == 2)
                {
                    destinations.Add(new Vector2Int((int) (cell_array[i, j].GetComponent<Cell>().transform.position.x), (int) (cell_array[i, j].GetComponent<Cell>().transform.position.y)));
                    //nav_nodes.Add(new Vector2Int((int)(cell_array[i, j].GetComponent<Cell>().transform.position.x), (int)(cell_array[i, j].GetComponent<Cell>().transform.position.y)));
                }

                if (cell_array[i, j].GetComponent<Cell>().object_type == 3)
                {
                    sources.Add(new Vector2Int((int) (cell_array[i, j].GetComponent<Cell>().transform.position.x), (int) (cell_array[i, j].GetComponent<Cell>().transform.position.y)));
                    //nav_nodes.Add(new Vector2Int((int)(cell_array[i, j].GetComponent<Cell>().transform.position.x), (int)(cell_array[i, j].GetComponent<Cell>().transform.position.y)));
                }
            }
        }
    }

    private void Get_Source_And_Dest_in_Nodes()
    {
        foreach (Vector2Int d in destinations)
        {
            Node node = new Node(d, Get_VC_of_Node(new Node(d), nodes));
            nodes.Add(node);
        }

        foreach (Vector2Int s in sources)
        {
            Node node = new Node(s, Get_VC_of_Node(new Node(s), nodes));
            nodes.Add(node);
        }
    }

    private void Instantiate_Boundaries()
    {
        for (int k = 0; k < x_range; k++)
        {
            Destroy(cell_array[k, y_range - 1]);
            Destroy(cell_array[k, 0]);
            cell_array[k, y_range - 1] = Instantiate(blocking, new Vector3(k, y_range - 1, 0), Quaternion.identity, this.transform);
            cell_array[k, 0] = Instantiate(blocking, new Vector3(k, 0, 0), Quaternion.identity, this.transform);
        }

        for (int l = 0; l < y_range; l++)
        {
            Destroy(cell_array[x_range - 1, l]);
            Destroy(cell_array[0, l]);
            cell_array[x_range - 1, l] = Instantiate(blocking, new Vector3(x_range - 1, l, 0), Quaternion.identity, this.transform);
            cell_array[0, l] = Instantiate(blocking, new Vector3(0, l, 0), Quaternion.identity, this.transform);
        }
    }

    private void Custom_Tilemap()
    {
        Destroy(cell_array[10, 10]);
        Destroy(cell_array[10, 11]);
        Destroy(cell_array[10, 12]);
        Destroy(cell_array[10, 13]);
        Destroy(cell_array[10, 9]);
        Destroy(cell_array[10, 8]);
        Destroy(cell_array[10, 7]);
        Destroy(cell_array[10, 6]);
        //Destroy(cell_array[15, 12]);
        //Destroy(cell_array[13, 12]);

        Destroy(cell_array[6, 13]);
        Destroy(cell_array[7, 13]);
        Destroy(cell_array[8, 13]);
        Destroy(cell_array[9, 13]);
        Destroy(cell_array[6, 6]);
        Destroy(cell_array[7, 6]);
        Destroy(cell_array[8, 6]);
        Destroy(cell_array[9, 6]);

        Destroy(cell_array[22, 20]);
        Destroy(cell_array[22, 21]);
        Destroy(cell_array[22, 22]);
        Destroy(cell_array[22, 23]);
        Destroy(cell_array[22, 24]);
        Destroy(cell_array[22, 25]);

        Destroy(cell_array[4, 15]);
        //cell_array[15, 12] = Instantiate(destination, new Vector3(15,12, 0), Quaternion.identity, this.transform);
        //cell_array[13, 12] = Instantiate(destination, new Vector3(13, 12, 0), Quaternion.identity, this.transform);
        cell_array[4, 15] = Instantiate(blocking, new Vector3(4, 15, 0), Quaternion.identity, this.transform);
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

        // Hindernislinie oben rechts
        cell_array[22, 20] = Instantiate(blocking, new Vector3(22, 20, 0), Quaternion.identity, this.transform);
        cell_array[22, 21] = Instantiate(blocking, new Vector3(22, 21, 0), Quaternion.identity, this.transform);
        cell_array[22, 22] = Instantiate(blocking, new Vector3(22, 22, 0), Quaternion.identity, this.transform);
        cell_array[22, 23] = Instantiate(blocking, new Vector3(22, 23, 0), Quaternion.identity, this.transform);
        cell_array[22, 24] = Instantiate(blocking, new Vector3(22, 24, 0), Quaternion.identity, this.transform);
        cell_array[22, 25] = Instantiate(blocking, new Vector3(22, 25, 0), Quaternion.identity, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        // Zeitfluss modellieren
        // bestimmte Aktionen werden in jedem einzelnen Zeitschritt aufgerufen
        // Dazu zählen:
        // Cell_Map.Flood_Fill()
        // Human_Behaviour.Move()
        // Human_Behaviour.Strategy()
        //
        // Zeitfluss soll kontrollierbar sein durch Interface

        if (nav_nodes.Count == 0)
        {
            Generate_Nav_Nodes();
            Set_All_Nodes();
            Sum_up_Nav_Nodes(); 
            Set_All_Nodes();
            Get_Source_And_Dest_in_Nodes();
            graph.Generate_Graph(nodes);
            //Debug.Log(graph.edges.Count);
            graph.Set_Alpha();
            graph.Generate_Paths();
            //graph.edges.ForEach(o=>Debug.Log("Start: " + o.start.position + "Ende: " + o.end.position));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Flood_Fill_All();
            foreach (Human_Behaviour o in agents)
            {
                if (o != null)
                {
                    o.Move();
                }
                else
                {
                    continue;
                }
            }
        }
    }

    void Flood_Fill_All()
    {
        foreach (Vector2Int o in destinations)
        {
            Flood_Fill(o.x, o.y);
        }
    }

    void GenerateTilemap()
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

        Vector2Int lowest_candidate = new Vector2Int(-1, -1);
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
                }
                else
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

                    float lowest_even = Mathf.Min(dx, dy);
                    float lowest_odd = Mathf.Min(d_1_to_5, d_3_to_7);

                    // w ist die Ausbreitungsgeschwindigkeit der Welle; Mit der Ausbreitung der Welle über die Zellen erhöht sich die Traveltime
                    // mit dem Wert von w
                    // Fußgänger verlangsamen die Ausbreitung der Welle, also erhöhen sich die Reisekosten (travel_cost)

                    float w = 1;

                    if (cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().is_occupied == true)
                    {
                        w = 1 * (1 + penalty_factor);
                    }

                    // Nach Dijkstra
                    float dijkstra_value = Mathf.Min(dx + w, dy + w, d_1_to_5 + w, d_3_to_7 + w);

                    // Nach Fast Marching Method
                    float travel_cost;
                    if (lowest_even <= lowest_odd)
                    {
                        travel_cost = lowest_even + w / 2;
                    }
                    else if (w <= 2 * Mathf.Sqrt(2) * (lowest_even - lowest_odd))
                    {
                        travel_cost = lowest_odd + Mathf.Sqrt(2) * w / 2;
                    }
                    else
                    {
                        travel_cost = lowest_even + Mathf.Sqrt(Mathf.Pow(w, 2) - 4 * (Mathf.Pow(lowest_even - lowest_odd, 2))) / 2;
                    }

                    Distance_Map[temp_neighbor.x, temp_neighbor.y] = travel_cost;
                    cell_array[temp_neighbor.x, temp_neighbor.y].GetComponent<Cell>().traveltime = travel_cost;

                }
            }
        }
    }

    void GetAllAgents()
    {
        Human_Behaviour[] k = Object.FindObjectsOfType<Human_Behaviour>();

        for (int i = 0; i < k.Length; i++)
        {
            agents.Add(k[i]);
        }
    }

    void Generate_Nav_Nodes()
    {

        Cell cur_cell;
        Cell neighbor_cc;
        Vector2Int direction;
        Vector2Int new_Node;
        Vector2Int fixed_new_Node;
        int distance = 0;

        foreach (Vector2Int o in obstacles)
        {
            cur_cell = NullOrCell(o);            

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    neighbor_cc = cur_cell.neighbors[2 * i + 1].GetComponent<Cell>();
                }
                catch
                {
                    continue;
                }

                if (neighbor_cc == null || neighbor_cc.object_type == 1)
                {
                    neighbor_cc.invalidNode = true;
                    continue;
                }
                
                for (int j = 0; j < 4; j++)
                {
                    if (neighbor_cc.neighbors[2 * j].GetComponent<Cell>().object_type == 1 || neighbor_cc.neighbors[2 * j] == null)
                    {
                        neighbor_cc.invalidNode = true;
                        break;
                    }
                }

                if (neighbor_cc.invalidNode != true)
                {                    
                    direction = new Vector2Int(neighbor_cc.x_pos - o.x, neighbor_cc.y_pos - o.y);
                    new_Node = new Vector2Int(o.x + direction.x * node_distance, o.y + direction.y * node_distance);
                    
                    if (Get_View_Contact(neighbor_cc.position2d, direction))
                    {
                        cell_array[new_Node.x, new_Node.y].GetComponent<Cell>().isNodeAlready = true;
                        nav_nodes.Add(new_Node);    
                    }
                    else
                    {
                        distance = 0;
                        for (int k = 1; k < node_distance; k++)
                        {
                            fixed_new_Node = new Vector2Int(o.x + direction.x * k, o.y + direction.y * k);
                            
                            if (NullOrCell(fixed_new_Node) == null || NullOrCell(fixed_new_Node).object_type == 1)
                            {
                                break;
                            }
                            distance++;
                        }

                        fixed_new_Node = new Vector2Int(o.x + direction.x * Mathf.CeilToInt((float) (distance) / 2), o.y + direction.y * Mathf.CeilToInt((float) (distance) / 2));

                        NullOrCell(fixed_new_Node).isNodeAlready = true;
                        nav_nodes.Add(fixed_new_Node);
                    }
                }
            }
        }
    }

    void Set_All_Nodes()
    {
        RaycastHit2D hit;

        nodes.Clear();

        foreach (Vector2Int o in nav_nodes)
        {
            List<Node> list = new List<Node>();

            foreach (Vector2Int p in nav_nodes)
            {
                if (o != p)
                {
                    hit = Physics2D.Raycast(o, new Vector3(p.x - o.x, p.y - o.y, 0), Vector2.Distance(o, p));

                    if (hit.collider != null)
                    {
                        continue;
                    }

                    list.Add(new Node(p));
                }
            }
            nodes.Add(new Node(o, list));
        }
    }

    bool Get_View_Contact(Vector2Int o, Vector2Int direction)
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(o, direction, node_distance);
        
        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    void Sum_up_Nav_Nodes()
    {
        if (nav_nodes.Count > 1)
        {
            List<Vector2Int> new_nodes = new List<Vector2Int>();
            List<Vector2Int> candidates = new List<Vector2Int>(nav_nodes);
            Vector2Int v;
                        
            while (candidates.Count > 0)
            {
                new_nodes.Clear();

                foreach (Vector2Int o in nav_nodes)
                {
                    foreach (Vector2Int p in nav_nodes)
                    {
                        if (Vector2Int.Distance(o, p) <= this.node_distance + 1 && o != p)
                        {

                            v = new Vector2Int(Mathf.FloorToInt((o.x + p.x) / 2), (o.y + p.y) / 2);
                            
                            // Prüfe ob nach der Zusammenfassung der Knoten immernoch eine Sichtverbindung zu den Knoten des Zusammengefassten Knoten oder noch mehr besteht
                            List<Vector2Int> v_list = Get_VC_of_Node(v, candidates);
                            List<Vector2Int> o_list = Get_VC_of_Node(o, candidates);

                            v_list.RemoveAll(x => x == o);

                            // v_list enthält alle Elemente die auch o_list enthält
                            if (v_list.All(o_list.Contains) && v_list.Count >= o_list.Count)
                                {
                                    cell_array[o.x, o.y].GetComponent<Cell>().isNodeAlready = false;
                                    cell_array[p.x, p.y].GetComponent<Cell>().isNodeAlready = false;
                                    new_nodes.Add(v);
                                    candidates.Add(v);
                                }                                                                                        
                        }
                        candidates.RemoveAll(j => j == o || j == p);
                    }
                }

                foreach (Vector2Int o in new_nodes)
                {
                    if (cell_array[o.x, o.y].GetComponent<Cell>().invalidNode != true)
                    {
                        cell_array[o.x, o.y].GetComponent<Cell>().isNodeAlready = true;
                    }

                    if (!nav_nodes.Contains(o))
                    {
                        nav_nodes.Add(o);
                    }
                }

                nav_nodes.RemoveAll(k => cell_array[k.x, k.y].GetComponent<Cell>().isNodeAlready == false);
            }
        }
    }

    bool IsValid_Node(Cell v)
    {
        for (int j = 0; j < 4; j++)
        {
            if (v.neighbors[2 * j].GetComponent<Cell>().object_type == 1 || v.neighbors[2 * j] == null)
            {
                return false;
            }
        }
        return true;
    }

    List<Vector2Int> Get_VC_of_Node(Vector2Int node, List<Vector2Int> nodes)
    {
        RaycastHit2D hit;
        List<Vector2Int> list = new List<Vector2Int>();

        foreach (Vector2Int p in nodes)
        {
            if (node != p)
            {
                hit = Physics2D.Raycast(node, new Vector3(p.x - node.x, p.y - node.y, 0), Vector2.Distance(node, p));

                if (hit.collider != null)
                {
                    continue;
                }

                list.Add(p);
            }
        }
        return list;
    }

    List<Node> Get_VC_of_Node(Node node, List<Node> nodes)
    {
        RaycastHit2D hit;
        List<Node> list = new List<Node>();

        foreach (Node p in nodes)
        {
            if (node != p)
            {
                hit = Physics2D.Raycast(node.position, new Vector3(p.position.x - node.position.x, p.position.y - node.position.y, 0), Vector2.Distance(node.position, p.position));

                if (hit.collider != null)
                {
                    continue;
                }

                list.Add(p);
            }
        }
        return list;
    }

    bool Nodes_Too_Near(List<Vector2Int> list)
    {
        foreach (Vector2Int o in list)
        {
            foreach (Vector2Int p in list)
            {
                if (Vector2Int.Distance(o, p) <= this.node_distance + 1 && o != p)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Cell NullOrCell(Vector2Int o)
    {
        if (o.x >= 0 && o.x < x_range && o.y >= 0 && o.y < y_range)
        {
            return cell_array[o.x, o.y].GetComponent<Cell>();
        }
        else
        {
            return null;
        }
    }
}
