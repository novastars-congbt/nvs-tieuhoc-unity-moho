using UnityEngine;

using EasySpringBone;


public class DemoScriptControl : MonoBehaviour
{
    public SpringBone[] springBones;

    private enum WIND { LEFT, RIGHT, OFF };
    private float windForce = 0.05f;
    private float timePassed = 0;


    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 160, 110), "SpringBone Control");

        if(GUI.Button(new Rect(20, 40, 140, 30), "Freeze"))
        {
            freeze();
        }

        if(GUI.Button(new Rect(20, 80, 140, 30), "Unfreeze"))
        {
            unfreeze();
        }
    }


    //==========================================================================
    //  Freeze spring bones
    //==========================================================================
    private void freeze()
    {
        for(int i = 0; i < springBones.Length; i++)
        {
            springBones[i].ignoreSpringBone = true;
        }
    }

    //==========================================================================
    //  Unfreeze spring bones
    //==========================================================================
    private void unfreeze()
    {
        for(int i = 0; i < springBones.Length; i++)
        {
            springBones[i].ignoreSpringBone = false;
        }
    }
}
