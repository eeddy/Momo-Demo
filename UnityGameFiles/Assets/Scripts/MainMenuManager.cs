using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject instructionPanel; 
    public GameObject setupPanel;
    
    void Start() {
         instructionPanel.SetActive(false);
         setupPanel.SetActive(false);
    }

    public void InstructionClicked() {
        instructionPanel.SetActive(true);
    }

    public void BackFromInstructionClicked() {
        instructionPanel.SetActive(false);
    }

    public void KeyboardClicked() {
        PlayerPrefs.SetInt("control",0);
        SceneManager.LoadScene("MomoGame");
    }

    public void EMGControlClicked() {
        PlayerPrefs.SetInt("control",1);
        SceneManager.LoadScene("MomoGame");
    }

    public void PlayClicked() {
        setupPanel.SetActive(true);
    }

    public void Quit() {
        Application.Quit();
    }

}
