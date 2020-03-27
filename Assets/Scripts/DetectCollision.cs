using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    [SerializeField] private Animator myAnimationController;
    public static bool trigger = false;
    private bool triggerEnable = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       /* if (MeshRaycast.isAligned)
        {
            Debug.Log("Enable Animator");
          //  GetComponent<Animator>().enabled = true;
        } */

        if(MeshRaycast.isAligned && triggerEnable && Input.GetKey(KeyCode.Alpha8))
        {
            triggerEnable = false;
            myAnimationController.SetBool("Collision", true);
            trigger = true;
            Debug.Log("Enter");
        }

        if (!triggerEnable && Input.GetKey(KeyCode.Alpha9))
        {
            triggerEnable = true;
            myAnimationController.SetBool("Collision", false);
            trigger = false;
            Debug.Log("Exit");
        }

    }
}
