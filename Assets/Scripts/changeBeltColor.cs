using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeBeltColor : MonoBehaviour
{

    private Mesh mesh;
    private int numVerts;
    private Color[] colors;
    private Color color;
    private int i = 0;

    float elapsed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<MeshFilter>())
        {
            mesh = GetComponent<MeshFilter>().mesh;
            if (mesh != null)
            {
                numVerts = mesh.vertexCount;
                colors = new Color[numVerts];
                color = colors[0];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveFunction_BeltandPulley.animationRun == true)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 0.3f)
            {
                elapsed = elapsed % 1f;
                if (mesh != null)
                {
                    changeColor();
                }
                if (i >= numVerts - 1)
                {
                    i = 0;
                }
                else
                {
                    i++;
                    //i+=10;
                }
            }
        }
    }

    void changeColor()
    {
        //2478
        for (int j = 0; j <numVerts; j++)
        {
            colors[j] = Color.white;
        }
        for (int j = 0; j < 100; j++)
        {
            if ((i + j) > numVerts - 1)
                break;
            colors[i+j] = Color.blue;
        }
        mesh.colors = colors;
    }
}
