using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TokenInputManager : MonoBehaviour
{
    [SerializeField] TMP_InputField tokenField;

    void Start()
    {
        if (PlayerPrefs.HasKey("Token"))
        {
            string token = PlayerPrefs.GetString("Token");
            tokenField.text = token;
        }
    }
}
