using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LessonSelector : MonoBehaviour
{
    public int lesson;
    [SerializeField]
     TMP_Text lessonNumber;
    [SerializeField]
     TMP_Text lessonName;

    string pathTemp;
    ActivityManager activity;
    Data data;
    private void OnEnable()
    {
        if (data == null) data = DataController.instance.data;
        lessonNumber.text = "" + (lesson + 1);
        lessonName.text = data.typeClasses[MenuController.currentClass].typeLessons[lesson].text;
    }
    public void OpenLessonScene()
    {
        if (data.typeClasses[MenuController.currentClass].typeLessons[lesson].typeActives.Length == 0) return;
        pathTemp = "Prefabs/" + data.typeClasses[MenuController.currentClass].typeLessons[lesson].name + "/" + data.typeClasses[MenuController.currentClass].typeLessons[lesson].typeActives[0].name;
        activity = Resources.Load<ActivityManager>(pathTemp);
        if (activity == null) return;
        MenuController.currentLesson = lesson;
        Debug.Log("currentLesson = " + MenuController.currentLesson);
        SceneManager.LoadScene("Gameplay");
    }
}
