using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnsureSingleEventSystem : MonoBehaviour
{
    private void Awake() {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

        if (eventSystems.Length > 1) {
            Debug.LogWarning("Multiple EventSystems found. Removing extra EventSystems.");
            for (int i = 1; i < eventSystems.Length; i++) {
                Destroy(eventSystems[i].gameObject);
            }
        }
    }
}
