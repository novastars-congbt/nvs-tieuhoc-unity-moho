using UnityEngine;

using EasySpringBone;


public class DemoExtraForceAndConstraint : MonoBehaviour
{
    public SpringBoneManager[] springBoneManagers;
    public GameObject[] winds;

    private enum WIND { LEFT, RIGHT, OFF };
    private float windForce = 0.05f;
    private float timePassed = 0;


    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 160, 150), "Wind Control");

        if(GUI.Button(new Rect(20, 40, 140, 30), "Turn On Left Wind"))
        {
            addExtraForce(0);
            switchWind(WIND.LEFT);
        }

        if(GUI.Button(new Rect(20, 80, 140, 30), "Turn On Right Wind"))
        {
            addExtraForce(180);
            switchWind(WIND.RIGHT);
        }

        if(GUI.Button(new Rect(20, 120, 140, 30), "Turn Off Wind"))
        {
            removeExtraForce();
            switchWind(WIND.OFF);
        }
    }

    private void Update()
    {
        updateWindForce();
    }


    //==========================================================================
    //  Apply extra force to spring bones
    //==========================================================================
    private void addExtraForce(float angle)
    {
        for(int i = 0; i < springBoneManagers.Length; i++)
        {
            springBoneManagers[i].extraForceDir = angle;
            springBoneManagers[i].forceLength = windForce;
            springBoneManagers[i].withExtraForce = true;
            springBoneManagers[i].alwaysUpdate = true;
        }
    }

    //==========================================================================
    //  Remove extra force from spring bones
    //==========================================================================
    private void removeExtraForce()
    {
        for(int i = 0; i < springBoneManagers.Length; i++)
        {
            springBoneManagers[i].withExtraForce = false;
        }
    }

    //==========================================================================
    //  Turn on/off particle game objects for wind VFX
    //==========================================================================
    private void switchWind(WIND wind)
    {
        if(wind == WIND.LEFT)
        {
            winds[0].SetActive(true);
            winds[1].SetActive(false);
        }
        else if(wind == WIND.RIGHT)
        {
            winds[0].SetActive(false);
            winds[1].SetActive(true);
        }
        else if(wind == WIND.OFF)
        {
            winds[0].SetActive(false);
            winds[1].SetActive(false);
        }
    }

    //==========================================================================
    //  Simple way to simulate wind force changes
    //==========================================================================
    private void updateWindForce()
    {
        timePassed += Time.deltaTime;
        if(timePassed > 0.1f)
        {
            timePassed = 0;
            windForce = Mathf.PerlinNoise(Time.time, 0) * 0.08f;

            changeExtraForce();
        }
    }

    //==========================================================================
    //  Change extra force to spring bones
    //==========================================================================
    private void changeExtraForce()
    {
        for(int i = 0; i < springBoneManagers.Length; i++)
        {
            springBoneManagers[i].forceLength = windForce;
        }
    }
}
