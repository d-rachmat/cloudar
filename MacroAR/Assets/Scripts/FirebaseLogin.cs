using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseLogin : MonoBehaviour {

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    private void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://macroapp-6af98.firebaseio.com/users");

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        getUserData();
    }

    void getUserData()
    {
        FirebaseDatabase.DefaultInstance.GetReference("-LKbPtTDziRylNMPJ9QL").GetValueAsync().ContinueWith(task => {
         if (task.IsFaulted)
         {
             // Handle the error...
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
                Debug.Log("The snapshot raw json value: " + snapshot.GetRawJsonValue());
            }
     });
    }

}
