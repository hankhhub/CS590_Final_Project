using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRaycast : MonoBehaviour
{
    Camera cam;
   
    List<int> hlt_list = new List<int>();
    Vector3 meshCentroid;
    MeshParser meshParser;

    public static bool isAligned = false;

    List<SurfaceInfo> selectObjects = new List<SurfaceInfo>();
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
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
        
        //HighlightByArea(hitTransform, TriList[hit.triangleIndex].area);  
        hlt_list.Clear();
        hlt_list = meshParser.HighlightBySurface(hitTransform, hit.triangleIndex);
        Vector3 surfaceCentroid = meshParser.SurfaceCentroid(hlt_list);
        meshParser.FillHighlight(mesh, hlt_list, Color.magenta);

        //Debug.DrawLine(hitTransform.TransformPoint(surfaceCentroid), Vector3.zero, Color.red, 0.4f, false);

        AlignObjects(hit, surfaceCentroid);


        if (Input.GetKey(KeyCode.M))
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
