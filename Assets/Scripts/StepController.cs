using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class StepController : MonoBehaviour
{
    public int index;
    //public Image frameStep;
    public TextMeshProUGUI textStep;

    GameController gameController;
    ListStepController listStepController;

    private void OnEnable()
    {
        if (gameController == null) gameController = GameController.instance;
        if (listStepController == null) listStepController = ListStepController.instance;
        DataParam.actionStep += Display;
        Display();
    }

    private void OnDisable()
    {
        DataParam.actionStep -= Display;
    }

    void Display()
    {
        //if (index == gameController.currentStep) frameStep.color = new Color(1f, 1f, 1f);
        //else frameStep.color = new Color(0.5f, 0.5f, 0.5f);
        if (index == gameController.currentStep) textStep.color = listStepController.colorTextStep[1];
        else textStep.color = listStepController.colorTextStep[0];
    }

    public void BtnStep()
    {
        if (gameController.currentStep == index) return;
        gameController.currentStep = index;
        gameController.currentSlide = 0;
        //gameController.CreateActivity();
        //gameController.SetCurrentActivity();
        //gameController.listStep.SetListStep();
        DataParam.actionStep();
        if (gameController.pause) gameController.BtnPause();
        //gameController.currentTimeline.time = gameController.data.typeClasses[gameController.currentClass].typeLessons[gameController.currentLesson].typeActives[gameController.currentActivity].typeSteps[gameController.currentStep].typeSlides[gameController.currentSlide].time;
        gameController.SetSlider();
        //if (gameController.pause)
        //{
        //    gameController.currentTimeline.Resume();
        //    if (gameController.currentStep == 0) gameController.currentTimeline.time = 0;
        //    else gameController.currentTimeline.time = gameController.data.typeClasses[gameController.currentClass].typeLessons[gameController.currentLesson].typeActives[gameController.currentActivity].typeSteps[gameController.currentStep - 1].time;
        //    gameController.currentTimeline.Pause();
        //}
        //else
        //{
        //if (gameController.currentStep == 0) gameController.currentTimeline.time = 0;
            //else gameController.currentTimeline.time = gameController.data.typeClasses[gameController.currentClass].typeLessons[gameController.currentLesson].typeActives[gameController.currentActivity].typeSteps[gameController.currentStep - 1].time;
        //}
    }
}
