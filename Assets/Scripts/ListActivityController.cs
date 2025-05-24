using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListActivityController : MonoBehaviour
{
    public static ListActivityController instance;
    public int currentPlanLesson = -1;
    //public Sprite[] spsFrame;
    public Sprite[] spsFrameIndex;
    [SerializeField]
     List<ActivityController> activities = new List<ActivityController>();
    GameController gameController;
    int currentClass;
    int currentLesson;

    public void OnAwake()
    {
        if (instance == null) instance = this;
        if (gameController == null) gameController = GameController.instance;
        currentClass = gameController.currentClass;
        currentLesson = gameController.currentLesson;
        for (int i = 0; i < gameController.data.typeClasses[currentClass].typeLessons[currentLesson].typeActives.Length; i++)
        {
            activities[i].index = i;
            activities[i].textIndex.text = "" + (activities[i].index + 1);
            activities[i].textActivity.text = gameController.data.typeClasses[currentClass].typeLessons[currentLesson].typeActives[i].text;
            if (i == gameController.data.typeClasses[currentClass].typeLessons[currentLesson].typeActives.Length - 1) activities[i].line.SetActive(false);
            else activities[i].line.SetActive(true);
            activities[i].gameObject.SetActive(true);
        }
    }
}