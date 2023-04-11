using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Linq;
using System.Net.Security;
using FullSerializer;
using Models;
using TMPro;
using Unity.VisualScripting;

public class DataManager : MonoBehaviour
{
    /* ==== References ==== */
    [SerializeField] TimeManager timeManager;
    [SerializeField] Stats stats;
    [SerializeField] Resources resources;
    [SerializeField] Profile profile;
    UserData loadedUser;

    /* ==== Game Objects ==== */

    /* ==== Local Variables ==== */
    const string ProjectId = "Social-Mania";
    static readonly string DatabaseURL = "https://social-mania-12157807-default-rtdb.firebaseio.com/";
    static readonly fsSerializer Serializer = new fsSerializer();

    public string userAuth;
    bool signedIn;

    List<UserData> loadedUserList;

    void Awake()
    {
        InvokeRepeating("getUsers", 0, 180); // Update leaderboard every 3 minutes
        // InvokeRepeating("save", 0, 60) -- add autosave after local save implemented
    }
    
    // "Sign in with Google" button
    public void onClickGoogleSignIn()
    {
        GoogleAuthHandler.SignInWithGoogle();
        profile.authPopup.SetActive(true);
        profile.disableButtons();
    }

    // "Click here once you have signed in with Google!" to pull authToken from the handler
    public void userAuthenticated()
    {
        userAuth = FirebaseAuthHandler.localId;
        if (userAuth == null)
        {
            Debug.Log("Sign in failed -- Please make sure you are signed in properly!"); // replace with proper in-game error popup!
        }
        else
        {
            Debug.Log("User Auth: " + userAuth);
            load();
            signedIn = true;
        }

        profile.authPopup.SetActive(false);
        profile.enableButtons();
        profile.googleSignInText.text = "";
    }

    // Save user data
    public void save()
    {
        if (!signedIn)
        {
            Debug.Log("Can't save, you aren't signed in!");
        }
        else
        {
            UserData user = new UserData();
            saveData(user);
            uploadToDatabase(user);
        }
    }
    
    // Data to be saved (augment UserData class to change)
    void saveData(UserData user)
    {
        user.username = profile.username;
        user.followers = resources.followers;
        user.lifetimeViews = (int)resources.views;
        user.numClicks = stats.numClicks;
        user.timePlayed = timeManager.timeSinceStartDate.ToString();
        user.startDate = timeManager.startDate.ToString();
        user.lastSeen = DateTime.Now.ToString();
    }
    
    // Upload UserData class to database
    void uploadToDatabase(UserData userObj)
    {
        Debug.Log("Starting save...");
        RestClient.Put<UserData>($"{DatabaseURL}users/{userAuth}.json", userObj).Then(response =>
        {
            Debug.Log("The user was successfully uploaded to the database");
        });
    }

    // Load data from database, deconstruct response into saved data
    public void load()
    {
        Debug.Log("Starting load...");
        RestClient.Get<UserData>($"{DatabaseURL}users/{userAuth}.json").Then(response =>
        {
            Debug.Log("Load successful.");
            profile.username = response.username;
            resources.followers = response.followers;
            resources.views = response.lifetimeViews;
            stats.numClicks = response.numClicks;
            timeManager.startDate = DateTime.Parse(response.startDate);
            timeManager.lastSeen = DateTime.Parse(response.lastSeen);
            TimeSpan lastSeen = timeManager.calculateLastSeen();
            Debug.Log("Time since last login: "+ lastSeen.Hours + " hours, " + lastSeen.Minutes + " minutes, " + lastSeen.Seconds + " seconds.");
        });
    }

    public void getUsers()
    {
        Debug.Log("Updating leaderboard...");
        RestClient.Get($"{DatabaseURL}users.json").Then(response =>
        {
            var responseJson = response.Text;
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            Serializer.TryDeserialize(data, typeof(Dictionary<string, UserData>), ref deserialized);

            var usersDict = deserialized as Dictionary<string, UserData>;

            Debug.Log("Ordered list:\n");
            List<UserData> userList = usersDict.Values.ToList().OrderByDescending(userData => userData.followers).ToList();
            loadedUserList = userList; // store this info somewhere so we don't have to keep calling the database!

            for (int i = 0; i < userList.Count(); i++)
            {
                Debug.Log(i+1 + ": " + userList[i].username + " - " + userList[i].lifetimeViews + " lifetime views");
            }

            for (int i = 0; i < 10; i++)
            {
                profile.leaderboardPositions[i].text = /*i + 1 + ": " +*/ userList[i].username + " - " + userList[i].lifetimeViews + " views";
            }
        });
    }

    public bool checkUsernameTaken(string usernameInput)
    {
        foreach (UserData user in loadedUserList)
        {
            if (user.username.Equals(usernameInput, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }
        return true;
    }
}
