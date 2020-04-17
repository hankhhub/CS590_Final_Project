using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshParser : MonoBehaviour
{
    HashSet<int> TriSet = new HashSet<int>();
    Camera cam;
    public Mesh meshObject;
    Dictionary<Vector2, int> edgeTable = new Dictionary<Vector2, int>(new EdgeComparer());

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
        //FindAdjacencies();
        meshCentroid = MeshCentroid(TriList);
        defaultColor = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Require custom shader that takes vertex color input
    public void FillHighlight(Mesh mesh, List<int> highlights, Color color)
    {
        Color[] colors = new Color[mesh.vertices.Length];
        
        Vector3[] vertices = mesh.vertices;     
        for(int t = 0; t < TriList.Count; t++)
        {
            if (highlights.Contains(t))
            {
                colors[TriList[t].p0] = color;
                colors[TriList[t].p1] = color;
                colors[TriList[t].p2] = color;
            }
            else
            {
                colors[TriList[t].p0] = defaultColor;
                colors[TriList[t].p1] = defaultColor;
                colors[TriList[t].p2] = defaultColor;
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
                /*Debug.DrawLine(p0, p1, Color.blue, 0.1f, false);
                Debug.DrawLine(p1, p2, Color.blue, 0.1f, false);
                Debug.DrawLine(p2, p0, Color.blue, 0.1f, false);*/
                highlights.Add(cnt);
            }
            cnt++;
        }
        return highlights;
    }


    public void HighlightByArea(Transform hitTransform, float hitArea)
    {
        foreach(Triangle t in TriList)
        {
            if(Mathf.Abs(t.area - hitArea) <= 0.003f)
            {
                Vector3 p0 = t.points[0];
                Vector3 p1 = t.points[1];
                Vector3 p2 = t.points[2];
                p0 = hitTransform.TransformPoint(p0); // convert local to world position
                p1 = hitTransform.TransformPoint(p1);
                p2 = hitTransform.TransformPoint(p2);
<<<<<<< Updated upstream
                Debug.DrawLine(p0, p1, Color.blue, 0.1f, false);
                Debug.DrawLine(p1, p2, Color.blue, 0.1f, false);
                Debug.DrawLine(p2, p0, Color.blue, 0.1f, false);
=======
                Debug.DrawLine(p0, p1, Color.cyan, 0.1f, false);
                Debug.DrawLine(p1, p2, Color.cyan, 0.1f, false);
                Debug.DrawLine(p2, p0, Color.cyan, 0.1f, false);
                highlights.Add(cnt);
>>>>>>> Stashed changes
            }
        }
    }

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
            a = new Vector2(t.p1, t.p0);
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
        }
    }

    void TriangleEdges()
    {      
        foreach (var t in TriList)
        {           
            edgeTable.Add(new Vector2(t.p0, t.p1), t.ID);
            edgeTable.Add(new Vector2(t.p1, t.p2), t.ID);
            edgeTable.Add(new Vector2(t.p2, t.p0), t.ID);
        }
    }
    void ParseTriangles(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] t = mesh.triangles;
        int tricnt = 0;
        for (int i = 0; i < t.Length; i += 3)
        {
            Triangle tri = new Triangle(vertices[t[i]], vertices[t[i + 1]], vertices[t[i + 2]]);
            tri.ID = tricnt;
            tri.p0 = t[i];
            tri.p1 = t[i + 1];
            tri.p2 = t[i + 2];
            tri.normal = mesh.normals[t[i]];
            TriList.Add(tri);
            tricnt++;
        }
    }

    public class Triangle
    {
        public int ID { get; set; }
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
            if(v1.x == v2.x && v1.y == v2.y)
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




}
