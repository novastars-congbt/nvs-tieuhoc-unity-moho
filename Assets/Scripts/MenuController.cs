using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : UIProperties
{
    public static MenuController instance;
    public static int currentClass = -1;
    public static int currentLesson;
    public static int stateUI;

    [SerializeField]
     Transform classView;
    [SerializeField]
     Transform lessonView;

    [SerializeField]
     Sprite[] sprTech;
    public List<LessonSelector> lessons = new List<LessonSelector>();

    private void Awake()
    {
        if (instance == null) instance = this;
        SetStateUI();
    }

    public void BtnClass(int i)
    {
        currentClass = i;
        classView.gameObject.SetActive(false);
        lessonView.GetComponent<ListLessonController>().Display();
        lessonView.gameObject.SetActive(true);
    }

    public void BtnBackClass()
    {
        currentClass = -1;
        lessonView.gameObject.SetActive(false);
        classView.gameObject.SetActive(true);
    }

    public void BtnTech()
    {
        if (classView.gameObject.activeSelf)
        {
            TechSupportPopUp.spr = sprTech[0];
        }
        else if (lessonView.gameObject.activeSelf)
        {
            TechSupportPopUp.spr = sprTech[1];
        }
        ShowTechSupportPanel();
    }

    public void BtnClose()
    {
        ShowCloseAppPanel();
    }

    void SetStateUI()
    {
        switch (stateUI)
        {
            case 0:
                classView.gameObject.SetActive(true);
                break;
            case 1:
                lessonView.GetComponent<ListLessonController>().Display();
                lessonView.gameObject.SetActive(true);
                break;
        }
    }
}
