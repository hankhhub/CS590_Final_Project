﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing info that defines animation (screw case only)
/// </summary>
public class AnimationRule
{
    public List<string> objNames;
    public Quaternion sourceRotation;
    public Vector3 sourceNormal;
    public Vector3 targetCentroid;
    public Vector3 targetNormal;
    public Vector3 sourcePosition;
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

public class AnimationRule_BeltandPulley
{
    public List<string> objNames;
    public Vector3 surfaceNormal;

    public AnimationRule_BeltandPulley()
    {
        objNames = new List<string>();
    }
    public AnimationRule_BeltandPulley(List<string> list)
    {
        objNames = new List<string>(list);
    }
}

public class MeshRaycast : MonoBehaviour
{
    Camera cam;

    List<int> hlt_list = new List<int>();
    Vector3 meshCentroid;
    MeshParser meshParser;

    public static bool isAligned = false;

    List<SurfaceInfo> selectObjects = new List<SurfaceInfo>();

    public AnimationRule animationRule;
    public AnimationRule_BeltandPulley animationRule_BeltandPulley;

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

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        animationRule = new AnimationRule();
        animationRule_BeltandPulley = new AnimationRule_BeltandPulley();
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
        if (meshParser == null)
        {
            Debug.LogError("meshParser null reference");
            return;
        }
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Transform hitTransform = hit.collider.transform;

        hlt_list.Clear();
        if (hightlightMode == HighlightMode.area)
        {
            hlt_list = meshParser.HighlightByArea(hitTransform, meshParser.TriList[hit.triangleIndex].area);
        }
        else if (hightlightMode == HighlightMode.surfaceNormal)
        {
            hlt_list = meshParser.HighlightBySurface(hitTransform, hit.triangleIndex);
        }

        Vector3 surfaceCentroid = meshParser.SurfaceCentroid(hlt_list); // local point not global

        // Debugging graphics
        //meshParser.FillHighlight(mesh, hlt_list, Color.magenta);
        //Debug.DrawLine(hitTransform.TransformPoint(surfaceCentroid), Vector3.zero, Color.red, 0.4f, false);

        switch (SelectObject.combinationName)
        {
            case "Screw_Nut_Interaction":
                AlignObjects(hit, surfaceCentroid);
                SelectAnimationObjects(hit);
                CreateMarkers(hit, mesh, surfaceCentroid);
                break;
            case "Gear":
                // Gear case functions
                CreateMarkers(hit, mesh, surfaceCentroid);
                break;
            case "Belt and Pulley":
                FindNormals(hit, surfaceCentroid);
                CreateMarkers(hit, mesh, surfaceCentroid);
                break;
        }
    }


    // [Ananya] Function to be included in drop down menu for gear case
    public void GetGearInfo()
    {
        meshParser.ConnectTriByArea(hlt_list);
        int cogNum = meshParser.tds.totalSets();
        float angleOffset = 0.5f * (360.0f / (float)cogNum);
        animationRule.gearAngle = angleOffset;
        SaveFunction_BeltandPulley.CustomFunction_Gear.gearAngle = animationRule.gearAngle.ToString("F4");
    }

    // [Ananya] Function to be included in drop down menu for gear case
    public void GetRotationAxis(RaycastHit hit)
    {
        animationRule.targetNormal = hit.transform.InverseTransformVector(hit.normal).normalized;
        SaveFunction_BeltandPulley.CustomFunction_Gear.surfaceNormal = animationRule.targetNormal.ToString("F4");
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
        if (SaveFunction_BeltandPulley.funcEnable)
        {
            switch (SelectObject.combinationName)
            {
                case "Screw_Nut_Interaction":
                    if (Input.GetMouseButtonDown(0))
                    {
                        switch (SaveFunction_BeltandPulley.functionNumber)
                        {
                            case 0:
                                animationRule.startpt = centroid;
                                SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.startpt = centroid.ToString("F4");
                                meshParser.FillHighlight(mesh, hlt_list, Color.blue);
                                break;
                            case 1:
                                animationRule.endpt = centroid;
                                SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.endpt = centroid.ToString("F4");
                                meshParser.FillHighlight(mesh, hlt_list, Color.blue);
                                break;
                            case 2:
                                animationRule.contactpt = centroid;
                                SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.contactpt = centroid.ToString("F4");
                                meshParser.FillHighlight(mesh, hlt_list, Color.red);
                                break;

                        }
                        //Set start/end/contact flag by UI button
                    }
                    break;
                case "Gear":
                    if (Input.GetMouseButtonDown(0))
                    {
                        switch (SaveFunction_BeltandPulley.functionNumber)
                        {
                            case 0:
                                GetGearInfo();
                                break;
                            case 1:
                                GetRotationAxis(hit);
                                break;
                        }
                    }
                    break;
                case "Belt and Pulley":
                    Bounds bounds = mesh.bounds;
                    float radius = bounds.extents.x;
                    if (Input.GetMouseButtonDown(0))
                    {
                        switch (SaveFunction_BeltandPulley.functionNumber)
                        {
                            case 0:
                                Debug.Log("Big Pulley Radius");
                                SaveFunction_BeltandPulley.CustomFunction_BeltandPulley.big_pulley_radius = radius.ToString("F4");
                                meshParser.FillHighlight(mesh, hlt_list, Color.blue);
                                break;
                            case 1:
                                SaveFunction_BeltandPulley.CustomFunction_BeltandPulley.small_pulley_radius = radius.ToString("F4");
                                meshParser.FillHighlight(mesh, hlt_list, Color.green);
                                break;

                        }
                        //Set start/end/contact flag by UI button
                    }
                    break;
            }
        }
        //SaveFunction_BeltandPulley.funcEnable = false;
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
                if (animationRule.objNames.Count >= 2)
                {
                    animationRule.objNames.Clear();
                }
                if (animationRule.objNames.Count < 2)
                {
                    animationRule.objNames.Add(hit.transform.gameObject.name);
                }
            }
        }
    }


    void AlignObjects(RaycastHit hit, Vector3 surfaceCentroid)
    {
        // Align by centroid click, get centroid and hit normal align with second object       
        if (SaveFunction_BeltandPulley.funcEnable && (SaveFunction_BeltandPulley.functionNumber == 3))
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
                    animationRule.sourceRotation = selectObjects[0].obj.transform.rotation;
                    animationRule.sourcePosition = selectObjects[0].obj.transform.position;
                    selectObjects[0].obj.transform.rotation = Quaternion.FromToRotation(-selectObjects[0].normal, selectObjects[1].normal) * selectObjects[0].obj.transform.rotation;
                    selectObjects[0].obj.transform.position = selectObjects[1].obj.transform.TransformPoint(selectObjects[1].centroid) + selectObjects[1].normal;                   
                    animationRule.targetNormal = selectObjects[1].normal;               
                    SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.sourceNormal = selectObjects[0].normal.ToString("F4");
                    SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.targetCentroid = selectObjects[1].centroid.ToString("F4");
                    SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.sourceRotation = animationRule.sourceRotation.ToString("F4");
                    SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.targetNormal = animationRule.targetNormal.ToString("F4");
                    SaveFunction_BeltandPulley.CustomFunction_Screw_Nut_Interaction.sourcePosition = animationRule.sourcePosition.ToString("F4");
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

    void FindNormals(RaycastHit hit, Vector3 surfaceCentroid)
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (SaveFunction_BeltandPulley.functionNumber)
            {
                case 2:
                    GameObject go = hit.transform.gameObject;
                    SurfaceInfo surfInfo;
                    surfInfo.obj = go;
                    surfInfo.normal = hit.normal;
                    SaveFunction_BeltandPulley.CustomFunction_BeltandPulley.surfaceNormal = surfInfo.normal.ToString("F4");
                    Debug.Log(go.name + surfInfo.normal);
                    surfInfo.centroid = surfaceCentroid;
                    selectObjects.Add(surfInfo);
                    break;
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
