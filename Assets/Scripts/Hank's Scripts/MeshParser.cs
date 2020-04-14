using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MeshParser : MonoBehaviour
{
    HashSet<int> TriSet = new HashSet<int>();
    Camera cam;
    public Mesh meshObject;

    Dictionary<Vector2, int> edgeTable = new Dictionary<Vector2, int>(new EdgeComparer());
    Dictionary<Vector3, int> vertTable = new Dictionary<Vector3, int>(new VertexComparer());

    [HideInInspector]
    public List<Triangle> TriList = new List<Triangle>();
    Vector3 meshCentroid;

    Color defaultColor;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        if(cam == null)
        {
            Debug.Log("camera null reference");
        }
        

        ParseTriangles(meshObject);       
        TriangleEdges();
        FindAdjacencies();

        meshCentroid = MeshCentroid(TriList);
        defaultColor = gameObject.GetComponent<MeshRenderer>().material.color;
        InitializeMeshColor(meshObject);            

    }

    public int ConnectTriByArea(List<int> highlights)
    {
        TriangleDisjointSets tds = new TriangleDisjointSets(highlights.Count);
        Dictionary<int, int> refIndex = new Dictionary<int, int>();
        for (int i = 0; i < highlights.Count; i++)
        {
            refIndex.Add(highlights[i], i);
        }
         
        for (int i = 0; i < highlights.Count; i++)
        {
            // Condition 1: must be adjacent
            foreach(var j in TriList[highlights[i]].adjList)
            {
                // Condition 2: must be highlighted
                if (refIndex.ContainsKey(j))
                {
                    tds.union(i, refIndex[j]);
                }               
            }
        }
        refIndex.Clear();
        return tds.totalSets();
    }

    private void InitializeMeshColor(Mesh mesh)
    {
        Color[] colors = new Color[mesh.vertices.Length];
        int[] triangles = mesh.triangles;
        
        for (int t = 0; t < TriList.Count; t++)
        {
            colors[triangles[TriList[t].ID]] = defaultColor;
            colors[triangles[TriList[t].ID + 1]] = defaultColor;
            colors[triangles[TriList[t].ID + 2]] = defaultColor;           
        }
        mesh.colors = colors;
    }

    // Require custom shader that takes vertex color input
    public void FillHighlight(Mesh mesh, List<int> highlights, Color color)
    {
        Color[] colors = new Color[mesh.vertices.Length];
        int[] triangles = mesh.triangles;
        mesh.colors.CopyTo(colors, 0);      
        for (int t = 0; t < TriList.Count; t++)
        {
            if (highlights.Contains(t))
            {
                colors[triangles[TriList[t].ID]] = color;
                colors[triangles[TriList[t].ID + 1]] = color;
                colors[triangles[TriList[t].ID + 2]] = color;
            }           
        }     
        mesh.colors = colors;
    }

   

    public List<int> HighlightBySurface(Transform hitTransform, int hit_index)
    {
        List<int> highlights = new List<int>();
        Triangle hit_triangle = TriList[hit_index];
        //highlight by normal + local depth
        
        float depth_x = hit_triangle.centroid.x;
        float depth_y = hit_triangle.centroid.y;
        float depth_z = hit_triangle.centroid.z;
        Vector3 hit_normal = hit_triangle.normal;
        
        int cnt = 0;
        
        foreach(Triangle t in TriList)
        {
            if(t.normal == hit_normal && (depth_x == t.centroid.x || depth_y == t.centroid.y || depth_z == t.centroid.z))
            {
                Vector3 p0 = t.points[0];
                Vector3 p1 = t.points[1];
                Vector3 p2 = t.points[2];
                p0 = hitTransform.TransformPoint(p0); // convert local to world position
                p1 = hitTransform.TransformPoint(p1);
                p2 = hitTransform.TransformPoint(p2);
                Debug.DrawLine(p0, p1, Color.green, 0.1f, false);
                Debug.DrawLine(p1, p2, Color.green, 0.1f, false);
                Debug.DrawLine(p2, p0, Color.green, 0.1f, false);
                highlights.Add(cnt);
            }
            cnt++;
        }
        return highlights;
    }


    public List<int> HighlightByArea(Transform hitTransform, float hitArea)
    {
        List<int> highlights = new List<int>();
        int cnt = 0;
        foreach (Triangle t in TriList)
        {
            if(Mathf.Abs(t.area - hitArea) <= 0.003f)
            {
                Vector3 p0 = t.points[0];
                Vector3 p1 = t.points[1];
                Vector3 p2 = t.points[2];
                p0 = hitTransform.TransformPoint(p0); // convert local to world position
                p1 = hitTransform.TransformPoint(p1);
                p2 = hitTransform.TransformPoint(p2);
                Debug.DrawLine(p0, p1, Color.blue, 0.1f, false);
                Debug.DrawLine(p1, p2, Color.blue, 0.1f, false);
                Debug.DrawLine(p2, p0, Color.blue, 0.1f, false);
                highlights.Add(cnt);
            }
            cnt++;

        }
        return highlights;
    }

    /// <summary>
    /// Computes averaged centroid using all highlighted triangles
    /// </summary>
    /// <param name="highlights"></param>
    /// <returns></returns>
    public Vector3 SurfaceCentroid(List<int> highlights)
    {
        Vector3 centroid = Vector3.zero;
        float surface_area = 0;
        foreach (int i in highlights)
        {
            centroid += TriList[i].area * TriList[i].centroid;
            surface_area += TriList[i].area;
        }
        return centroid / surface_area;
    }

    public Vector3 MeshCentroid(List<Triangle> tlist)
    {
        Vector3 centroid = Vector3.zero;
        float total_area = 0;
        foreach (Triangle t in tlist)
        {
            centroid += t.area * t.centroid;
            total_area += t.area;
        }
        return centroid / total_area;
    }

    void FindAdjacencies()
    {
        Vector2 a;
        Vector2 b;
        Vector2 c;
        foreach (var t in TriList)
        {
            a = new Vector2(t.p1, t.p0); //reversed edges of edgeTable
            b = new Vector2(t.p2, t.p1);
            c = new Vector2(t.p0, t.p2);
            if (edgeTable.ContainsKey(a))
            {
                t.adjList.Add(edgeTable[a]);
            }
            if (edgeTable.ContainsKey(b))
            {
                t.adjList.Add(edgeTable[b]);
            }
            if (edgeTable.ContainsKey(c))
            {
                t.adjList.Add(edgeTable[c]);
            }
            Debug.Log(t.adjList.Count);
        }      
    }

    void TriangleEdges()
    {     
        for(int i = 0; i < TriList.Count; i++)
        {
            edgeTable.Add(new Vector2(TriList[i].p0, TriList[i].p1), i);
            edgeTable.Add(new Vector2(TriList[i].p1, TriList[i].p2), i);
            edgeTable.Add(new Vector2(TriList[i].p2, TriList[i].p0), i);
        }
        /*foreach (var t in TriList)
        {           
            edgeTable.Add(new Vector2(t.p0, t.p1), t.ID);
                                                 
            edgeTable.Add(new Vector2(t.p1, t.p2), t.ID);
            
            edgeTable.Add(new Vector2(t.p2, t.p0), t.ID);                     
        }*/
    }

    void ParseTriangles(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] t = mesh.triangles;
        //int tricnt = 0;
        for (int i = 0; i < t.Length; i += 3)
        {
            Triangle tri = new Triangle(vertices[t[i]], vertices[t[i + 1]], vertices[t[i + 2]]);
            tri.ID = i;
            ParseVertices(tri, vertices, t, i);
            tri.normal = mesh.normals[t[i]];
            TriList.Add(tri);
            //tricnt++;
        }
        vertTable.Clear(); //clean up
    }

    void ParseVertices(Triangle tri, Vector3[] v, int[] t, int i)
    {
        if (vertTable.ContainsKey(v[t[i]])) {
            tri.p0 = vertTable[v[t[i]]];
        }
        else{
            vertTable.Add(v[t[i]], t[i]);
            tri.p0 = t[i];
        }

        if (vertTable.ContainsKey(v[t[i + 1]]))
        {
            tri.p1 = vertTable[v[t[i + 1]]];
        }
        else
        {
            vertTable.Add(v[t[i + 1]], t[i + 1]);
            tri.p1 = t[i + 1];
        }

        if (vertTable.ContainsKey(v[t[i + 2]]))
        {
            tri.p2 = vertTable[v[t[i + 2]]];
        }
        else
        {
            vertTable.Add(v[t[i + 2]], t[i + 2]);
            tri.p2 = t[i + 2];
        }
    }

    public class Triangle
    {
        // Triangle ID by starting vertex (stride: 3) ex: {0, 1, 3} ID = 0
        public int ID { get; set; }
        // Reindexed vertices for adjacency computation
        public int p0 { get; set; } 
        public int p1 { get; set; }
        public int p2 { get; set; }

        public Vector3[] points = new Vector3[3];
        public List<int> adjList = new List<int>();
        public float area;
        public Vector3 normal;
        public Vector3 orthogonal;
        public Vector3 centroid;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            points[0] = a;
            points[1] = b;
            points[2] = c;
            Vector3 A = b - a;
            Vector3 B = c - b;
            orthogonal = Vector3.Cross(A, B);           
            area = 0.5f * Vector3.Magnitude(orthogonal);
            centroid = new Vector3((a.x + b.x + c.x) / 3, (a.y + b.y + c.y) / 3, (a.z + b.z + c.z) / 3);
        }      
    }


    class EdgeComparer: IEqualityComparer<Vector2>
    {
        public bool Equals(Vector2 v1, Vector2 v2)
        {
            if((v1.x == v2.x && v1.y == v2.y))
            {              
                return true;
            }           
            else
            {
                return false;
            }
        }
        public int GetHashCode(Vector2 v)
        {
            return v.x.GetHashCode() ^ v.y.GetHashCode() << 2;
        }
    }

    class VertexComparer : IEqualityComparer<Vector3>
    {
        public bool Equals(Vector3 v1, Vector3 v2)
        {
            if (v1.Equals(v2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(Vector3 v)
        {
            return v.x.GetHashCode() ^ v.y.GetHashCode() << 2 ^ v.z.GetHashCode() >> 2;
        }
    }




}
