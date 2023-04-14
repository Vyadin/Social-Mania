using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    /* ==== References ==== */
    [SerializeField] TimeManager timeManager;
    [SerializeField] Stats stats;
    [SerializeField] Resources resources;
    [SerializeField] DataManager dataManager;
    
    /* ==== Game Objects ==== */
    /* -- Scenes -- */
    public GameObject mainScene;
    public GameObject profileScene;
    
    /* -- Popups -- */
    public GameObject authPopup;
    public GameObject userTakenPopup;
    public GameObject userLengthPopup;
    public GameObject offlinePopup;
    
    /* -- Buttons -- */
    public Button googleSignInButton;
    public Button returnToGameButton;
    public Button submitUsernameButton;
    
    /* -- Text -- */
    public TextMeshProUGUI googleSignInText;
    public TextMeshProUGUI lifetimeViewsValue;
    public TextMeshProUGUI numClicksValue;
    public TextMeshProUGUI[] leaderboardPositions;
    
    /* -- Misc -- */
    public TMP_InputField usernameInput;
    
    
    
    /* ==== Local Variables ==== */

    public string username;

    void Awake() // should move this to a scene handler or something, this is a band-aid for me not wanting to deactivate scenes all the time
    {
        authPopup.SetActive(false);
        returnToMain();
    }
    
    void FixedUpdate()
    {
        lifetimeViewsValue.text = stats.lifetimeViews.ToString();
        numClicksValue.text = stats.numClicks.ToString();
    }
    
    public void visitProfile() // Profile button clicked
    {
        // Change scenes
        mainScene.SetActive(false);
        profileScene.SetActive(true);
    }

    public void returnToMain()
    {
        profileScene.SetActive(false);
        mainScene.SetActive(true);
    }

    public void disableButtons()
    {
        returnToGameButton.interactable = false;
        googleSignInButton.interactable = false;
        submitUsernameButton.interactable = false;
    }

    public void enableButtons()
    {
        returnToGameButton.interactable = true;
        googleSignInButton.interactable = true;
        submitUsernameButton.interactable = true;
    }

    public void setUsername()
    {
        string userInput = usernameInput.text;
        bool usernameValid = true;

        if (!dataManager.checkUsernameTaken(userInput))
        {
            Debug.Log("This username is already taken!");
            usernameValid = false;
            userTakenPopup.SetActive(true);
            disableButtons();
        }

        if (userInput.Length > 16)
        {
            Debug.Log("That username is too long!");
            usernameValid = false;
            userLengthPopup.SetActive(true);
            disableButtons();
        }

        if (usernameValid)
        {
            username = userInput;
        }
    }

    public void closeTakenPopup()
    {
        userTakenPopup.SetActive(false);
        enableButtons();
    }

    public void closeLengthPopup()
    {
        userLengthPopup.SetActive(false);
        enableButtons();
    }

    public void closeOfflinePopup()
    {
        offlinePopup.SetActive(false);
        enableButtons();
    }

    public void hardReset()
    {
        resources.views = 0;
        resources.followers = 0;
        resources.attention = 1.00f;
        stats.lifetimeViews = 0;
        stats.numClicks = 0;
        timeManager.startDate = DateTime.Now;
    }
}
