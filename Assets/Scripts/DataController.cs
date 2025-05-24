using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;
    public Data data;
    [SerializeField] bool debug;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        Debug.unityLogger.logEnabled = debug;
        if (instance == null) instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
