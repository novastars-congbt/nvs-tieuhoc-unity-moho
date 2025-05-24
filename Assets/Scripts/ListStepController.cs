using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListStepController : MonoBehaviour
{
    public static ListStepController instance;
    //public Sprite[] spsFrame;
    [SerializeField]
     List<StepController> steps = new List<StepController>();
    public Color[] colorTextStep;

    GameController gameController;
    //int currentClass;
    //int currentLesson;
    //int currentActivity;

    public void OnAwake()
    {
        if (instance == null) instance = this;
        if (gameController == null) gameController = GameController.instance;
        //currentClass = gameController.currentClass;
        //currentLesson = gameController.currentLesson;
        SetListStep();
    }

    public void SetListStep()
    {
        //currentActivity = gameController.currentActivity;
        for (int i = 0; i < steps.Count; i++)
        {
            steps[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < gameController.typeActive.typeSteps.Length; i++)
        {
            steps[i].index = i;
            steps[i].textStep.text = gameController.typeActive.typeSteps[i].text;
            steps[i].gameObject.SetActive(true);
        }
    }
}
