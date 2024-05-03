//////////////////////////////////////////////
//Assignment/Lab/Project: Blackjack Project
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 03/04/2024
/////////////////////////////////////////////

using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Re-used from Virtual Pet Game Project.
//Rules Screen Manager class. Controls the Rules screen, by changing which page is currently active to simulate flipping through pages in a book.
public class RuleScreenManager : MonoBehaviour
{
    //REFERENCES
    [SerializeField] Transform pageParent;
    [SerializeField] TextMeshProUGUI pageCountText;

    //GAMEOBJECT LIST
    List<GameObject> pages = new List<GameObject>(); //Not an array because it is set automatically, at runtime.

    //TRACKING VARIABLE
    int currentPage = 0; //integer that keeps track of the current page/current index of the pages list.

    //CUSTOM FUNCTION
    void GetPages() //Gets Pages (pageParent GameObject children), adds them to pages list, and then sets page one (pages[0]) to active.
    {
        for(int i = 0; i < pageParent.childCount; i++)
        {
            GameObject page = pageParent.GetChild(i).gameObject;
            page.SetActive(false);
            pages.Add(page);
        }
        pages[0].SetActive(true);
    }

    //EVENT FUNCTIONS
    void Start() //Initialization
    {
        GetPages();
        UpdatePageCountText();
    }

    void UpdatePageCountText() //Updates the text to display current page out of total pages. Ex: Page: 2/7
    {
        pageCountText.text = "Page: " + (currentPage + 1) + "/" + pages.Count;
    }

    public void NextPage() //Goes to next page. If you've exceeded the page count, goes to first page.
    {
        pages[currentPage].SetActive(false);

        currentPage++;
        
        if(currentPage >= pages.Count)
        {
            currentPage = 0;
        }

        pages[currentPage].SetActive(true);

        UpdatePageCountText();
    }

    public void PreviousPage() //Goes to previous page. If you've descended the page count, goes to last page.
    {
        pages[currentPage].SetActive(false);

        currentPage--;
        
        if(currentPage < 0)
        {
            currentPage = pages.Count - 1;
        }

        pages[currentPage].SetActive(true);

        UpdatePageCountText();        
    }

}
