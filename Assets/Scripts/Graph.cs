using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Graph
    {
        public List<Edge> edges;
        public List<Node> nodes;
        public List<Node> path;
        
        public float a_min;
        public float a_max;
        public float alpha;
        float epsilon = 160;

        public Graph(List<Edge> edges)
        {
            path = new List<Node>();
            this.edges = edges;
        }

        public Graph(){
            path = new List<Node>();
        }

        public void Set_Alpha()
        {
            // Sucht den jeweils größten und Kleinsten Winkel zwischen zwei ausgehenden Kanten im Graph

            float temp_Angle;
            float min_Angle = Mathf.Infinity;
            float max_Angle = Mathf.NegativeInfinity;

            foreach (Edge e_1 in edges)
            {
                foreach (Edge e_2 in edges)
                {
                    if (e_1.start.position == e_2.start.position && !e_2.IsEqualTo(e_1))
                    {
                        temp_Angle = Vector2.Angle(e_1.vector, e_2.vector);

                        if (temp_Angle == 0)
                        {
                            temp_Angle = 180;
                        }

                        if (temp_Angle < min_Angle)
                        {
                            min_Angle = temp_Angle;
                        }

                        if (temp_Angle > max_Angle)
                        {
                            max_Angle = temp_Angle;
                        }
                        
                    }
                }
            }

            a_max = max_Angle;
            a_min = min_Angle;
            alpha = a_max - a_min;
        }

        public void Set_Max_Alpha()
        {
            // Nach Kneidl
            float alpha = (a_max - a_min) / 2;
            float temp;
            
            while (alpha < Mathf.Clamp(epsilon, 0, 179))
            {
                if (A_Star(nodes.Find(o => o.type == 3), nodes.Find(o => o.type == 2)))
                {
                    temp = alpha;
                    alpha = (a_max - alpha) / 2 + alpha;
                    a_min = temp;
                } else
                {
                    Debug.Log("ds");
                    temp = alpha;
                    alpha = alpha - (a_max - alpha) / 2;
                    a_max = temp;
                }

                this.alpha = alpha;
                
                Generate_Paths();
                Set_All_Nodes();
            }
        }

        public void Generate_Paths()
        {
            // Kein Rückgabewert: Setzt das Feld 'edges'
            // generiert aus Nav_Nodes Pfade:
            // Kegelbasierter Algorithmus nach Kneidl

            Graph output = new Graph();
            List<Node> node_list = new List<Node>(nodes);
            List<Edge> edge_list = new List<Edge>();
            List<Triangle> cut_cones_list = new List<Triangle>();

            // Suchbereich

            Vector2 p1;
            Vector2 p2;
            Vector2 p3;
            float distance;
            float alpha = this.alpha / 2 * Mathf.PI / 180;
            float direction_angle;
            
            foreach (Node n in node_list)
            {
                cut_cones_list.Clear();
                
                // Liste aller sichtbaren Knoten im Suchbereich
                // Ordnen der Liste

                n.connected_nodes = Get_Search_Area_And_VC_Nodes(n, node_list);             
                n.connected_nodes = n.connected_nodes.OrderBy(o => Vector2.Distance(o.position, n.position)).ToList();
                
                foreach (Node neighbor in n.connected_nodes)
                {
                    if (!cut_cones_list.Exists(o => o.PointInTriangle(neighbor)))
                    {
                        Edge e = new Edge(n, neighbor);
                        // Kante hinzufügen
                        if (!edge_list.Exists(o => o.IsEqualTo(e)))
                        {
                            edge_list.Add(e);
                        }                        

                        // Kegelförmigen Bereich ausschneiden

                        p1 = new Vector2(n.position.x, n.position.y);

                        distance = 100;

                        direction_angle = Mathf.Atan2(neighbor.position.y - n.position.y, neighbor.position.x - n.position.x);
                        
                        p2 = new Vector2((n.position.x + Mathf.Cos(direction_angle + alpha/2) * distance), (n.position.y + Mathf.Sin(direction_angle + alpha/2) * distance));
                        p3 = new Vector2((n.position.x + Mathf.Cos(direction_angle - alpha/2) * distance), (n.position.y + Mathf.Sin(direction_angle - alpha/2) * distance));                        

                        Triangle cut_cone = new Triangle(p1,p2,p3);
                        cut_cones_list.Add(cut_cone);
                    }
                }
            }
            edges = edge_list;
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

        List<Node> Get_Search_Area_And_VC_Nodes(Node node, List<Node> nodes)
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

        public void Generate_Graph(List<Node> nodes)
        {
            // verbindet alle Knoten in Sichtkontakt miteinander einmal
            // dient als Grundlage für Generate Paths, sowie Set_Alpha und Set_Max_Alpha
            List<Node> node_list = new List<Node>(nodes);
            List<Edge> edge_list = new List<Edge>();

            foreach (Node n in nodes)
            {
                n.connected_nodes = Get_VC_of_Node(n, nodes).OrderBy(o => Vector2Int.Distance(n.position, o.position)).ToList();

                foreach (Node nvc in n.connected_nodes)
                {
                    Edge edge = new Edge(n, nvc);

                    if (!edge_list.Exists(e => e.IsEqualTo(edge)))
                    {
                        edge_list.Add(edge);
                    }
                }
            }

            this.nodes = node_list;
            this.edges = edge_list;
        }

        public bool A_Star(Node Start, Node End)
        {
            if (edges != null)
            {
                List<Node> open_list = new List<Node>();
                List<Node> closed_list = new List<Node>();
                Node current = new Node(new Vector2Int(0, 0))
                {
                    f = Mathf.Infinity
                };
                float tentative_g = 0;
                float f_min;
                open_list.Add(Start);

                while (open_list.Count > 0)
                {

                    // wählt günstigsten Kandidaten aus
                    f_min = Mathf.Infinity;
                    foreach (Node o in open_list)
                    {
                        if (o.f <= f_min)
                        {
                            f_min = o.f;
                            current = o;
                        }
                    }

                    if (current == End)
                    {
                        return true;
                    }

                    // Kandidat wird betrachtet, und kann dementsprechend in die geschlossene Liste (geprüfte Nodes)
                    open_list.RemoveAll(o => o.position == current.position);
                    closed_list.Add(current);

                    // Für jeden Nachbarn von current werden h,g und f ermittelt 
                    foreach (Node successor in current.connected_nodes)
                    {
                        successor.g = current.g + Vector2.Distance(successor.position, current.position);
                        successor.h = Vector2.Distance(successor.position, End.position);
                        successor.f = successor.h + successor.g;

                        // wenn der Knoten bereits betrachtet wurde, überspringe diesen
                        if (closed_list.Contains(successor))
                        {
                            continue;
                        }

                        tentative_g = current.g + Vector2.Distance(current.position, successor.position);

                        // wenn ein besserer Kandidat als successor in der open_list ist, überspringe diesen
                        if (open_list.Contains(successor) && successor.g <= tentative_g)
                        {
                            continue;
                        }

                        // wenn der Knoten nicht bereits betrachtet wurden, und es keinen bessere in der offenen Liste gibt: berechne neu und setze parent
                        successor.parent = current;
                        successor.g = tentative_g;
                        successor.f = successor.g + successor.h;

                        if (!open_list.Contains(successor))
                        {
                            open_list.Add(successor);
                        }
                    }
                }
                return false;
            }
            else return false;
        }

        public void Set_All_Nodes()
        {
            // Setzt das Feld 'nodes' nach gegebenen Edges
            
            foreach (Node n in nodes)
            {
                n.connected_nodes.Clear();
                foreach (Edge e in edges)
                {
                    if (n == e.start)
                    {
                        n.connected_nodes.Add(e.end);
                    } 

                    if (n == e.end)
                    {
                        n.connected_nodes.Add(e.start);
                    }
                }
            }
        }
        
        public Node GetPath(Node cur)
        {
            // Gibt Rekursiv den Pfad aus A* zurück
            // macht sich das Feld parent der Nodes zu Nutze
            if (cur != null)
            {
                path.Add(cur);
                return GetPath(cur.parent);
            }
            else
                //path.Reverse();
                return null;
        }
    }
}
