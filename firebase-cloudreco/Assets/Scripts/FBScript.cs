using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FBScript : MonoBehaviour
{

    private FirebaseAuth auth;
    private FirebaseUser user;
    private string privateUserName;
    private string privatePassword;

    public ARController arcontroller;
    public InputField UserNameInput, PasswordInput;
    public Button LoginButton;
    public Text ErrorText;
    public Text greeting;
    public Text coin;
    public Image pp;
    public GameObject blur;
    public string uid;
    public string ppUrl;
    public string idu;
    public GameObject loginElement, logOutElement;
    public GameObject loading, logout;

    private void Awake()
    {
        Debug.Log(PlayerPrefs.GetString("username"));
        Debug.Log(PlayerPrefs.GetString("password"));
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        LoginButton.onClick.AddListener(() => Login(UserNameInput.text, PasswordInput.text));
        StartCoroutine(InitializeLogin());
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void InitializeDatabase()
    {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://macroapp-6af98.firebaseio.com/");
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseDatabase.DefaultInstance.GetReference("users").Child(idu).Child("coin").ValueChanged += HandleValueChanged;
    }

    private void UpdateErrorMessage(string message)
    {
        ErrorText.text = message;
        Invoke("ClearErrorMessage", 3);
    }

    void ClearErrorMessage()
    {
        ErrorText.text = "";
    }

    public void Logout()
    {
        arcontroller.burgerBoel = false;
        StartCoroutine(logOUt());
    }

    public void Login(string email, string password)
    {
        loading.SetActive(true);
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync canceled.");
                loading.SetActive(false);
                logOutElement.SetActive(true);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync error: " + task.Exception);
                if (task.Exception.InnerExceptions.Count > 0)
                    UpdateErrorMessage(task.Exception.InnerExceptions[0].Message);
                loading.SetActive(false);
                logOutElement.SetActive(true);
                return;
            }

            user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1}) | {2} | {3} | {4}", user.DisplayName, user.UserId, user.Email, user.ProviderId, user.ProviderData);
            uid = user.UserId;
            getUserData();
            InitializeDatabase();
            PlayerPrefs.SetString("username", UserNameInput.text);
            PlayerPrefs.SetString("password", PasswordInput.text);
            PlayerPrefs.SetString("LoginUser", user != null ? user.Email : "Unknown");
        });
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot

        Debug.Log("Value Changed: " + args.Snapshot.GetRawJsonValue());
        string value = args.Snapshot.GetRawJsonValue();
        coin.text = value;

    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                loading.SetActive(false);
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
            }
        }
    }

    private void getUserData()
    {
        string id;
        string nama;
        string profilepicture;
        string coins;

        // First of all get the old poin
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("uid").EqualTo(uid).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Faulted");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("The snapshot rad json value: " + snapshot.GetRawJsonValue());
                IEnumerable<DataSnapshot> children = snapshot.Children;

                if(snapshot.HasChildren)
                {
                    foreach(DataSnapshot child in snapshot.Children)
                    {
                        id = child.Child("id").Value.ToString();
                        nama = child.Child("name").Value.ToString();
                        profilepicture = child.Child("profilePicture").Value.ToString();
                        coins = child.Child("coin").Value.ToString();
                        LoginCondition(nama, profilepicture, coins);
                        idu = id;
                        loading.SetActive(false);
                        break;
                    }
                }
            }
        });
    }

    //-LL4C621vNC3JU0Atr9h

    public void AddPointToSpecifiedUser(string PointToAdd)
    {
        // First of all get the old poin
        int oldPoin = Convert.ToInt32(coin.text);
        int newPoin;
        newPoin = oldPoin + Convert.ToInt32(PointToAdd);
        Debug.Log(oldPoin);
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(idu).Child("coin").SetValueAsync(newPoin);
        InitializeDatabase();
        Debug.Log("data berhasil di simpan");
    }

    void LoginCondition(string Nama, string ProfilePicture, string Coin)
    {
        loginElement.SetActive(true);
        logOutElement.SetActive(false);
        greeting.text = "Welcome, " + Nama + " !";
        ppUrl = ProfilePicture;
        coin.text = Coin ;
        blur.SetActive(false);
        GameObject.Find("CloudRecognition").GetComponent<SimpleHandler>().enabled = true;
        StartCoroutine(profilePicture(ppUrl));
    }

    IEnumerator profilePicture(string url)
    {
        // Start a download of the given URL
        using (WWW www = new WWW(url))
        {
            // Wait for download to complete
            yield return www;
            Texture tex;
            tex= www.texture;
            // assign texture
            pp.sprite = Sprite.Create(www.texture, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }
     
    IEnumerator InitializeLogin()
    {
        yield return new WaitForSeconds(3);
        UserNameInput.text = PlayerPrefs.GetString("username", privateUserName);
        PasswordInput.text = PlayerPrefs.GetString("password", privatePassword);
        string usr = UserNameInput.text;
        string pwd = PasswordInput.text;
        Login(usr , pwd);
    }

    IEnumerator logOUt()
    {
        UserNameInput.text = "";
        PasswordInput.text = "";

        loading.SetActive(true);
        PlayerPrefs.SetString("username", "");
        PlayerPrefs.SetString("password", "");

        yield return new WaitForSeconds(2);

        logOutElement.SetActive(true);
        loginElement.SetActive(false);
        loading.SetActive(false);
    }
}