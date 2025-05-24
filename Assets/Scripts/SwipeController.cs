using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] int maxPage;
    [SerializeField]
     int currentPage = 1;
    //Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;

    float dragThreshold;

    [SerializeField] Button previousButton, nextButton;

    Data data;

    private void OnEnable()
    {
        if (data == null) data = DataController.instance.data;
        levelPagesRect.localPosition = levelPagesRect.localPosition - (currentPage - 1) * pageStep;
        currentPage = 1;
        //MovePage();
        maxPage = (data.typeClasses[MenuController.currentClass].typeLessons.Length - 1) / 8 + 1;
    }

    private void Awake()
    {
        //targetPos = levelPagesRect.localPosition;
        dragThreshold = Screen.width / 15;

        UpdateArrowButton();
    }
    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            levelPagesRect.localPosition += pageStep;
            //targetPos += pageStep;
            MovePage();
        }
    }
    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            levelPagesRect.localPosition -= pageStep;
            //targetPos -= pageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        UpdateArrowButton();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshold)
        {
            if (eventData.position.x > eventData.pressPosition.x)
                Previous();
            else
                Next();
        }
        else
        {
            MovePage();
        }
    }
    void UpdateArrowButton()
    {
        nextButton.interactable = true;
        previousButton.interactable = true;
        if (currentPage == 1)
        {
            previousButton.interactable = false;
        } 
        if (currentPage == maxPage)
        {
            nextButton.interactable = false;
        }
    }
}
