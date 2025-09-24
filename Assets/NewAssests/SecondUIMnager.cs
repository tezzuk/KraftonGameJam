using UnityEngine;
using UnityEngine.SceneManagement;


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
    }
    public void Close()
    {
        InstructionPanel.SetActive(false);
        TutorialPanel.SetActive(false);
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


}
