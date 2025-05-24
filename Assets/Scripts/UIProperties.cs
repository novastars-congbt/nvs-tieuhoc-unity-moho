using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EasyUI.PickerWheelUI;

public class UIProperties : MonoBehaviour
{
    public Transform parentPopup;
    [HideInInspector]
    public CloseAppPopUp closeAppPanel;
    [HideInInspector]
    public TechSupportPopUp techSupportPanel;
    [HideInInspector]
    public TimerMenuPopUp timerMenuPanel;
    [HideInInspector]
    public SpinPopUp spinPanel;
    [HideInInspector]
    public PickerWheelPopUp pickerWheelPanel;
    [HideInInspector]
    public SettingsPopUp settingPanel;
    [HideInInspector]
    public LessonPlanPopUp lessonPlanPanel;
    [HideInInspector]
    public ExpiredPopUp expiredPanel;
    [HideInInspector]
    public RandomNumberPopUp randomNumberPanel;

    public void ShowCloseAppPanel()
    {
        if (closeAppPanel == null)
        {
            closeAppPanel = Instantiate(Resources.Load<CloseAppPopUp>("Panel/CloseAppPopUP"));
            //closeAppPanel.transform.parent = parentPopup.transform;
            closeAppPanel.transform.SetParent(parentPopup.transform, false);
            closeAppPanel.SetRect(true);
        }
        closeAppPanel.OpenMe1();
        closeAppPanel.transform.SetAsLastSibling();
    }

    public void ShowTechSupportPanel()
    {
        if (techSupportPanel == null)
        {
            techSupportPanel = Instantiate(Resources.Load<TechSupportPopUp>("Panel/TechSupportPopUP"));
            //techSupportPanel.transform.parent = parentPopup.transform;
            techSupportPanel.transform.SetParent(parentPopup.transform, false);
            techSupportPanel.SetRect(true);
        }
        techSupportPanel.OpenMe1();
        techSupportPanel.transform.SetAsLastSibling();
    }

    public void ShowTimerMenuPanel()
    {
        if (timerMenuPanel == null)
        {
            timerMenuPanel = Instantiate(Resources.Load<TimerMenuPopUp>("Panel/TimerTaskbarPopUP"));
            //techSupportPanel.transform.parent = parentPopup.transform;
            timerMenuPanel.transform.SetParent(parentPopup.transform, false);
            timerMenuPanel.SetRect(false);
        }
        timerMenuPanel.OpenMe();
        timerMenuPanel.transform.SetAsLastSibling();
    }

    public void ShowSpinPanel()
    {
        if (spinPanel == null)
        {
            spinPanel = Instantiate(Resources.Load<SpinPopUp>("Panel/SpinPopUP"));
            //techSupportPanel.transform.parent = parentPopup.transform;
            spinPanel.transform.SetParent(parentPopup.transform, false);
            spinPanel.SetRect(false);
        }
        spinPanel.OpenMe();
        spinPanel.transform.SetAsLastSibling();
    }

    public void ShowPickerWheelPopUp()
    {
        if (pickerWheelPanel != null) Destroy(pickerWheelPanel);

            pickerWheelPanel = Instantiate(Resources.Load<PickerWheelPopUp>("Panel/PickerWheelPopUp"));
            //techSupportPanel.transform.parent = parentPopup.transform;
            pickerWheelPanel.transform.SetParent(parentPopup.transform, false);
            pickerWheelPanel.SetRect(false);
        
        pickerWheelPanel.OpenMe();
        pickerWheelPanel.transform.SetAsLastSibling();
    }

    public void ShowSettingPanel()
    {
        if (settingPanel == null)
        {
            settingPanel = Instantiate(Resources.Load<SettingsPopUp>("Panel/SettingsPopUp"));
            //techSupportPanel.transform.parent = parentPopup.transform;
            settingPanel.transform.SetParent(parentPopup.transform, false);
            settingPanel.SetRect(false);
        }
        settingPanel.OpenMe();
        settingPanel.transform.SetAsLastSibling();
    }

    public void ShowLessonPlanPanel()
    {
        if (lessonPlanPanel == null)
        {
            lessonPlanPanel = Instantiate(Resources.Load<LessonPlanPopUp>("Panel/LessonPlanPopUp"));
            //techSupportPanel.transform.parent = parentPopup.transform;
            lessonPlanPanel.transform.SetParent(parentPopup.transform, false);
            lessonPlanPanel.SetRect(false);
        }
        lessonPlanPanel.OpenMe();
        lessonPlanPanel.transform.SetAsLastSibling();
    }

    public void ShowExpiredPopUp()
    {
        if (expiredPanel == null)
        {
            expiredPanel = Instantiate(Resources.Load<ExpiredPopUp>("Panel/ExpiredPopUp"));
            //expiredPanel.transform.parent = parentPopup.transform;
            if (parentPopup != null) expiredPanel.transform.SetParent(parentPopup.transform, false);
            expiredPanel.SetRect(false);
        }
        expiredPanel.OpenMe();
        expiredPanel.transform.SetAsLastSibling();
    }

    public void ShowRandomNumberPopUP()
    {
        if (randomNumberPanel == null)
        {
            randomNumberPanel = Instantiate(Resources.Load<RandomNumberPopUp>("Panel/RandomNumberPopUp"));
            //expiredPanel.transform.parent = parentPopup.transform;
            if (parentPopup != null) randomNumberPanel.transform.SetParent(parentPopup.transform, false);
            randomNumberPanel.SetRect(false);
        }
        randomNumberPanel.OpenMe();
        randomNumberPanel.transform.SetAsLastSibling();
    }
}
