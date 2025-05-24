using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivitiesManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> timelinePrefabList; // Danh sách prefab hoạt động

    private GameObject currentTimelineInstance;   // Reference to the current timeline instance
    private int currentPrefabIndex = 0;          // Index of the current timeline prefab

    void Start()
    {
        LoadTimelinePrefab(currentPrefabIndex); // Load the first timeline prefab initially
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextPrefab();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousPrefab();
        }
    }

    public void LoadTimelinePrefab(int prefabIndex)
    {
        if (currentTimelineInstance != null)
        {
            Destroy(currentTimelineInstance); // Destroy the old timeline prefab instance
        }

        if (prefabIndex >= 0 && prefabIndex < timelinePrefabList.Count)
        {
            GameObject timelinePrefab = timelinePrefabList[prefabIndex];
            currentTimelineInstance = Instantiate(timelinePrefab);
        }
    }

    public void NextPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex + 1) % timelinePrefabList.Count;
        LoadTimelinePrefab(currentPrefabIndex);
    }

    public void PreviousPrefab()
    {
        currentPrefabIndex = (currentPrefabIndex - 1 + timelinePrefabList.Count) % timelinePrefabList.Count;
        LoadTimelinePrefab(currentPrefabIndex);
    }
}
