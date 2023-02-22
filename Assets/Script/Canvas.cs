using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Canvas : MonoBehaviour
{
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI PrepareInfoText;
    public TextMeshProUGUI FightingInfoText;
    public TextMeshProUGUI AIGameboardText;
    public TextMeshProUGUI PlayerGameboardText;
    public GameObject replayButton;
    private GameObject[] aiGameboard;

    // hide and show different objects
    public void PrepareGame()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
        replayButton.SetActive(false);
        aiGameboard = GameObject.FindGameObjectsWithTag("Gameboard");
        foreach (GameObject a in aiGameboard)
        {
            a.SetActive(false);
        }
    }

    // hide and show different objects
    public void StartGame()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
        foreach (GameObject a in aiGameboard)
        {
            a.SetActive(true);
        }
    }     

    public void Gameover(int winner)
    {
        if (winner == 0)
        {
            HeaderText.GetComponent<TextMeshProUGUI>().text = "You Win!";
        } else {
            HeaderText.GetComponent<TextMeshProUGUI>().text = "AI Win!";
        }
        replayButton.SetActive(true);
    }

    public void HomePage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }

    // Text Display
    public void DisplayPrepareInfoText(string t, Color color)
    {
        PrepareInfoText.GetComponent<TextMeshProUGUI>().text = t;
        PrepareInfoText.GetComponent<TextMeshProUGUI>().color = color;
        Invoke("ResetPrepareInfoText",2f);
    }

    private void ResetPrepareInfoText()
    {
        PrepareInfoText.GetComponent<TextMeshProUGUI>().text = "Please place ships by clicking the gameboard. Press 'Next' to place the next ship";
        PrepareInfoText.GetComponent<TextMeshProUGUI>().color = Color.black;
    }

    public void DisplayFightingInfoText(string t)
    {
        FightingInfoText.GetComponent<TextMeshProUGUI>().text = t;
        Invoke("ResetFightingInfoText",1f);
    }

    private void ResetFightingInfoText()
    {
        FightingInfoText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void DisplayAIText(string t)
    {
        AIGameboardText.GetComponent<TextMeshProUGUI>().text = t;
        Invoke("ResetAIText",1f);
    }

    private void ResetAIText()
    {
        AIGameboardText.GetComponent<TextMeshProUGUI>().text = "AI Gameboard";
    }

    public void DisplayPlayerText(string t)
    {
        PlayerGameboardText.GetComponent<TextMeshProUGUI>().text = t;
        Invoke("ResetPlayerText",1f);
    }

    private void ResetPlayerText()
    {
        PlayerGameboardText.GetComponent<TextMeshProUGUI>().text = "Player Gameboard";
    }

    public void DisplayNextTurn(bool playerTurn)
    {
        if (playerTurn)
        {
            HeaderText.GetComponent<TextMeshProUGUI>().text = "Your Turn";
        } else {
            HeaderText.GetComponent<TextMeshProUGUI>().text = "AI Turn";
        }       
    }
}
