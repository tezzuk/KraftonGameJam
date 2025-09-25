using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SecondUIMnager : MonoBehaviour
{
    public GameObject InstructionPanel, TutorialPanel;
    public void play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Instruction()
    {
        InstructionPanel.SetActive(true);
    }
    public void Tutorial()
    {
        TutorialPanel.SetActive(true);
        panels[0].SetActive(true);

    }
    public void Close()
    {
        InstructionPanel.SetActive(false);
        TutorialPanel.SetActive(false);
        //currentIndex = 0;
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        currentIndex = 0;
    }
    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public GameObject[] panels;   // Assign your panels in order in the Inspector
    private int currentIndex = 0;

    public void ShowNextPanel()
    {
        if (currentIndex < panels.Length - 1)
        {
            panels[currentIndex].SetActive(false);   // Hide current
            currentIndex++;

            panels[currentIndex].SetActive(true);    // Show next
        }
        if (currentIndex == panels.Length-1)
        {
            Close();
        }
    }

    public void ShowPreviousPanel()
    {
        if (currentIndex > 0)
        {
            panels[currentIndex].SetActive(false);   // Hide current
            currentIndex--;

            panels[currentIndex].SetActive(true);    // Show previous
        }
    }
    public float delay = 32f;
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex ==1)
            StartCoroutine(WaitAndLoadScene());
    }
    IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(2);
    }
}

