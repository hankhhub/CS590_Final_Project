using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltColorChange : MonoBehaviour
{
    Renderer renderer1;
    GameObject obj;
    int i, index;
    float elapsed = 0f;
    bool colorCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in this.transform)
        {
            obj = child.gameObject;
            renderer1 = obj.GetComponent<Renderer>();
            if (obj.name == "Solid1")
            {
                renderer1.material.color = Color.black;
            }
            else
            {
                i = obj.name.IndexOf(":");
                index = int.Parse(obj.name.Substring(i+1));
                if ((index % 2) == 0)
                {
                    renderer1.material.color = Color.black;
                }
                else
                {
                    renderer1.material.color = Color.white;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((SelectObject.combinationName == "Belt and Pulley") && (SaveFunction_Novice.animationRun))
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 0.1f)
            {
                elapsed = elapsed % 0.1f;
                changeColor();
                colorCheck = !colorCheck;
            }
        }

        void changeColor()
        {
            foreach (Transform child in this.transform)
            {
                obj = child.gameObject;
                renderer1 = obj.GetComponent<Renderer>();
                if (obj.name == "Solid1")
                {
                }
                else
                {
                    i = obj.name.IndexOf(":");
                    index = int.Parse(obj.name.Substring(i + 1));
                    if (colorCheck)
                    {
                        if ((index % 2) == 0)
                        {
                            renderer1.material.color = Color.black;
                        }
                        else
                        {
                            renderer1.material.color = Color.white;
                        }
                    }
                    else
                    {
                        if ((index % 2) == 0)
                        {
                            renderer1.material.color = Color.white;
                        }
                        else
                        {
                            renderer1.material.color = Color.black;
                        }
                    }
                }
            }
        }
    }
}
