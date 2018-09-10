using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

public class AddingUserPointToGoogleFirebase : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private string displayName;
    private string emailAddress;

    public string UserIdToAddPoint;
    public int PointToAdd;

    // Use this for initialization
    void Start()
    {
        InitializeFirebase();
        InitializeDatabase();
    }

    void InitializeDatabase()
    {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://macroapp-6af98.firebaseio.com/");
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseDatabase.DefaultInstance
        .GetReference(UserIdToAddPoint)
        .ValueChanged += HandleValueChanged;
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
    }

    // Update is called once per frame
    void Update()
    {

        // Add the user poin by 10
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddPointToSpecifiedUser();
        }
    }

    private void AddPointToSpecifiedUser()
    {
        // First of all get the old poin
        FirebaseDatabase.DefaultInstance.GetReference("id").GetValueAsync().ContinueWith(task => {
            int oldPoin = 0;

            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Faulted");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("The snapshot rad json value: " + snapshot.GetRawJsonValue());

                // Do something with snapshot...
                var items = snapshot.Value as Dictionary<string, object>;
                oldPoin = Convert.ToInt32(items["coin"]);
                Debug.Log(oldPoin);

                // Finally adding new poin
                int newPoin = oldPoin + PointToAdd;
               // FirebaseDatabase.DefaultInstance.GetReference(UserIdToAddPoint).Child("coin").SetValueAsync(newPoin);
            }
        });
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        auth.SignInWithEmailAndPasswordAsync("viktor@student.umn.ac.id", "zzzz1111").ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
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
                displayName = user.DisplayName ?? "";
                emailAddress = user.Email ?? "";
            }
        }
    }

}
