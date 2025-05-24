using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class ActivityController : MonoBehaviour
{
    public int index;
    [SerializeField]
     Image frameIndex;
    [SerializeField]
     Image frameText;
    [SerializeField]
     Image framePlanLesson;
    public Text textIndex;
    public Text textActivity;
    public GameObject line;

    GameController gameController;
    ListActivityController listActivityController;

    private void OnEnable()
    {
        if (gameController == null) gameController = GameController.instance;
        if (listActivityController == null) listActivityController = ListActivityController.instance;
        DataParam.actionBtnActivity += DisplayFrameText;
        DataParam.actionBtnPlanLesson += DisplayFramePlanLesson;
        DisplayFrameText();
        DisplayFramePlanLesson();
    }

    private void OnDisable()
    {
        DataParam.actionBtnActivity -= DisplayFrameText;
        DataParam.actionBtnPlanLesson -= DisplayFramePlanLesson;
    }

    void DisplayFrameText()
    {
        if (index == gameController.currentActivity)
        {
            frameIndex.sprite = listActivityController.spsFrameIndex[1];
            frameText.color = Color.white;
            textIndex.color = Color.white;
        }
        else
        {
            frameIndex.sprite = listActivityController.spsFrameIndex[0];
            frameText.color = new Color(1, 1, 1, 0);
            textIndex.color = Color.black;
        }
    }

    void DisplayFramePlanLesson()
    {
        if (index == listActivityController.currentPlanLesson)
        {
            framePlanLesson.color = Color.white;
        }
        else
        {
            framePlanLesson.color = new Color(1, 1, 1, 0);
        }

    }

    public void BtnActivity()
    {
        if (gameController.currentActivity == index) return;
        gameController.isSecondaryActivity = false;
        gameController.currentActivity = index;
        gameController.currentStep = 0;
        gameController.currentSlide = 0;
        gameController.CreateActivity();
        gameController.SetCurrentActivity();
        gameController.SetSlider();
        gameController.DropDownActivity();
        DataParam.actionBtnActivity();
        gameController.listStep.SetListStep();
        DataParam.actionStep();
        if (gameController.lessonPlanPanel != null) gameController.lessonPlanPanel.gameObject.SetActive(false);
        listActivityController.currentPlanLesson = -1;
        DataParam.actionBtnPlanLesson();
    }

    public void BtnPlanLesson()
    {
        if (gameController.lessonPlanPanel != null) gameController.lessonPlanPanel.gameObject.SetActive(false);
        if (listActivityController.currentPlanLesson == index)
        {
            listActivityController.currentPlanLesson = -1;    
        }
        else
        {
            listActivityController.currentPlanLesson = index;
            gameController.BtnPlanLesson();
        }
        DataParam.actionBtnPlanLesson();
    }
}