using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinPopUp : PopupProperties
{
    public static SpinPopUp instance;
    public int currentSpin = 0;
    public int total = 0;
    [SerializeField]
     GameObject button;
    [SerializeField]
     TMP_InputField inputField;

    private void OnEnable()
    {
        button.SetActive(true);
        inputField.gameObject.SetActive(false);
        inputField.text = "";
    }
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void BtnChoose(int index)
    {
        currentSpin = index;
        if (currentSpin == 2) EndSpin();
        else
        {
            button.SetActive(false);    
            inputField.gameObject.SetActive(true);
        }
    }

    bool success;
    public void EndSpin()
    {
        if (inputField.text != "") {
            success = int.TryParse(inputField.text, out total);
            if (success && total > 0) { 
                gameObject.SetActive(false);
                GameController.instance.ShowRandomNumberPopUP();
            }
        }
    }
}
