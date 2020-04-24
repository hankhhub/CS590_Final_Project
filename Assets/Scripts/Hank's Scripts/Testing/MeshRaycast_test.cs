using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing info that defines animation (screw case only)
/// </summary>
public class AnimeRule
{
    public List<string> objNames;
    public List<Vector3> contactlist;
    public Vector3 sourceNormal;
    public Vector3 targetNormal;
    public Vector3 targetCentroid;
    public Vector3 startpt;
    public Vector3 endpt;
    public Vector3 contactpt;
    
    public float gearAngle;
    public AnimeRule()
    {
        objNames = new List<string>();
        contactlist = new List<Vector3>();
    }
    public AnimeRule(List<string> list)
    {
        objNames = new List<string>(list);
    }
}

public class MeshRaycast_test : MonoBehaviour
{
    Camera cam;   
    List<int> hlt_list = new List<int>();  
    MeshParser meshParser;

    public static bool isAligned = false;

    List<Vector3> selectNormals = new List<Vector3>();

    public AnimeRule animationRule;

    [HideInInspector]
    public enum MarkerPoints
    {
        Start,
        End,
        Contact
    }

    public enum HighlightMode
    {
        surfaceNormal,
        area
    }
    public HighlightMode hightlightMode;

    private int markerType = 0;

    public GameObject[] gears = null;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        animationRule = new AnimeRule();
        if (cam == null)
        {
            Debug.Log("camera null reference");
        }             
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;
        
        meshParser = hit.collider.transform.gameObject.GetComponent<MeshParser>();       
        if(meshParser == null)
        {
            Debug.LogError("meshParser null reference");
            return;
        }
        Mesh mesh = meshCollider.sharedMesh;
                  
        
        Transform hitTransform = hit.collider.transform;
                
        hlt_list.Clear();
        if(hightlightMode == HighlightMode.area)
        {
            hlt_list = meshParser.HighlightByArea(hitTransform, meshParser.TriList[hit.triangleIndex].area);
        }
        else if(hightlightMode == HighlightMode.surfaceNormal)
        {
            hlt_list = meshParser.HighlightBySurface(hitTransform, hit.triangleIndex);
        }    
        
        Vector3 surfaceCentroid = meshParser.SurfaceCentroid(hlt_list); // local point not global

        // Debugging graphics
        //meshParser.FillHighlight(mesh, hlt_list, Color.magenta);
        //Debug.DrawLine(hitTransform.TransformPoint(surfaceCentroid), Vector3.zero, Color.red, 0.4f, false);
        ComputeSubCentroids();
        GetRotationAxis(hit);
        GetGearInfo();
        GetAlignment(hit, surfaceCentroid);
        SelectAnimationObjects(hit);
        CreateMarkers(hit, mesh, surfaceCentroid);
    }

 

    // [Ananya] Function to be included in drop down menu for gear case
    public void GetGearInfo()
    {
        if (Input.GetKey(KeyCode.G))
        {
            if (Input.GetMouseButtonDown(0))
            {
                meshParser.ConnectTriByArea(hlt_list);
                int cogNum = meshParser.tds.totalSets();
                float angleOffset = 0.5f * (360.0f / (float)cogNum);
                animationRule.gearAngle = angleOffset;
                Debug.Log("Gear tooth: " + cogNum);
            }
        }
    }

    // [Ananya] Function to be included in drop down menu for gear case
    public void GetRotationAxis(RaycastHit hit)
    {
        if (Input.GetKey(KeyCode.R))
        {
            if (Input.GetMouseButtonDown(0))
            {
                animationRule.targetNormal = hit.transform.InverseTransformVector(hit.normal).normalized;               
            }
        }
    }

    /// <summary>
    /// [Ananya] : move selection to drop down menu
    /// </summary>
    public void SetStart()
    {
        markerType = (int)MarkerPoints.Start;
    }
    public void SetEnd()
    {
        markerType = (int)MarkerPoints.End;
    }
    public void SetContact()
    {
        markerType = (int)MarkerPoints.Contact;
    }

    /// <summary>
    /// Select start/end/contact points by mouseclick holding M [change to drop down menu]
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="mesh"></param>
    /// <param name="centroid"></param>
    void CreateMarkers(RaycastHit hit, Mesh mesh, Vector3 centroid)
    {
        if (Input.GetKey(KeyCode.M))
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Set start/end/contact flag by UI button
                switch (markerType)
                {
                    case 0:
                        animationRule.startpt = centroid;
                        meshParser.FillHighlight(mesh, hlt_list, Color.blue);
                        break;
                    case 1:
                        animationRule.endpt = centroid;
                        meshParser.FillHighlight(mesh, hlt_list, Color.blue);
                        break;
                    case 2:
                        animationRule.contactlist.Add(centroid);
                        meshParser.FillHighlight(mesh, hlt_list, Color.red);                        
                        break;
                }
               
            }
        }
    }

    /// <summary>
    /// Choose source and target by mouse click holding B key
    /// </summary>
    /// <param name="hit"></param>
    void SelectAnimationObjects(RaycastHit hit)
    {
        if (Input.GetKey(KeyCode.B))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(animationRule.objNames.Count >= 2)
                {
                    animationRule.objNames.Clear();
                }
                if(animationRule.objNames.Count < 2)
                {
                    animationRule.objNames.Add(hit.transform.gameObject.name);                  
                }               
            }
        }
    }

    public void ComputeSubCentroids()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (Input.GetMouseButtonDown(0))
            {
                meshParser.ConnectTriByArea(hlt_list);
                animationRule.contactlist = meshParser.SurfaceSubCentroids();
            }
        }
    }

    void GetAlignment(RaycastHit hit, Vector3 surfaceCentroid)
    {
        // Align by centroid click, get centroid and hit normal align with second object       
        if (Input.GetButton("AlignMode"))
        {           
            if (Input.GetMouseButtonDown(0))
            {
                if (selectNormals.Count < 2)
                {
                
                    selectNormals.Add(meshParser.TriList[hit.triangleIndex].normal);
                    
                }
                if(selectNormals.Count == 2)
                {
                    animationRule.targetCentroid = surfaceCentroid;
                    animationRule.sourceNormal = selectNormals[0];
                    animationRule.targetNormal = selectNormals[1];
                    selectNormals.Clear();
                }              
            }
        }       
    }

    struct SurfaceInfo
    {
        public GameObject obj;
        public Vector3 normal;
        public Vector3 centroid;
    }
   
}
