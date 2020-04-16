using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing info that defines animation (screw case only)
/// </summary>
public class AnimationRule
{
    public List<string> objNames;
    public Vector3 targetNormal;
    public Vector3 startpt;
    public Vector3 endpt;
    public Vector3 contactpt;

    public float gearAngle;
    public AnimationRule()
    {
        objNames = new List<string>();
    }
    public AnimationRule(List<string> list)
    {
        objNames = new List<string>(list);
    }
}

public class MeshRaycast : MonoBehaviour
{
    Camera cam;   
    List<int> hlt_list = new List<int>();  
    MeshParser meshParser;

    public static bool isAligned = false;

    List<SurfaceInfo> selectObjects = new List<SurfaceInfo>();

    public AnimationRule animationRule;

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
        animationRule = new AnimationRule();
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
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;              
        
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
        GetRotationAxis(hit);
        GetGearInfo();
        AlignObjects(hit, surfaceCentroid);
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
                int cogNum = meshParser.ConnectTriByArea(hlt_list);
                float angleOffset = 0.5f * (360.0f / (float)cogNum);
                animationRule.gearAngle = angleOffset;
                              
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
                        animationRule.contactpt = centroid;
                        meshParser.FillHighlight(mesh, hlt_list, Color.red);
                        break;

                }
                //Set start/end/contact flag by UI button
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

   
    void AlignObjects(RaycastHit hit, Vector3 surfaceCentroid)
    {
        // Align by centroid click, get centroid and hit normal align with second object       
        if (Input.GetButton("AlignMode"))
        {           
            if (Input.GetMouseButtonDown(0))
            {
                if (selectObjects.Count < 2)
                {
                    GameObject go = hit.transform.gameObject;
                    SurfaceInfo surfInfo;
                    surfInfo.obj = go;
                    surfInfo.normal = hit.normal;
                    Debug.Log(surfInfo.normal);
                    surfInfo.centroid = surfaceCentroid;
                    selectObjects.Add(surfInfo);
                }

                if (selectObjects.Count == 2)
                {
                    selectObjects[0].obj.transform.rotation = Quaternion.FromToRotation(-selectObjects[0].normal, selectObjects[1].normal) * selectObjects[0].obj.transform.rotation;
                    selectObjects[0].obj.transform.position = selectObjects[1].obj.transform.TransformPoint(selectObjects[1].centroid ) + selectObjects[1].normal;
                    animationRule.targetNormal = selectObjects[1].normal;
                    selectObjects.Clear();
                    isAligned = true;
                }
                else
                {
                    isAligned = false;
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
