using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSetup : MonoBehaviour
{
   

    [ExposeInEditor(RuntimeOnly = false)]
    public void SetupAnimation()
    {
        AnimationControl_test[] animeCtrls = GetComponentsInChildren<AnimationControl_test>();
        if(animeCtrls == null)
        {
            Debug.LogError("Null reference to child objects!");
            return;
        }
        Debug.Log(animeCtrls.Length);
        for (int i = 0; i < animeCtrls.Length; i++)
        {
            animeCtrls[i].ImportScrewRule(i);
        }

    }
}
