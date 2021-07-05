using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using System;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using Newtonsoft;
using Newtonsoft.Json;

[FirestoreData]
public class UserData
{
    [FirestoreProperty]
    public int Score { get; set; }
    [FirestoreProperty]
    public int QuestionAnswered { get; set; }
}

[FirestoreData]
public class UserProfile
{
    [FirestoreProperty]
    public string Name { get; set; }
    [FirestoreProperty]
    public string UID { get; set; }
    [FirestoreProperty]
    public string PhoneNumber { get; set; }
    [FirestoreProperty]
    public string EmailAddress { get; set; }
    [FirestoreProperty]
    public UserData Data { get; set; }
}

[FirestoreData]
public class fl_meta
{
}

[FirestoreData]
public class QuestionData
{
    [FirestoreProperty]
    public fl_meta _fl_meta_ { get; set; }
    [FirestoreProperty]
    public bool containAutioQuestion { get; set; }
    [FirestoreProperty]
    public bool containBoolTrueFalseAnswer { get; set; }
    [FirestoreProperty]
    public bool containImageQuestion { get; set; }
    [FirestoreProperty]
    public bool containTextAnswer { get; set; }
    [FirestoreProperty]
    public bool containTextQuestion { get; set; }
    [FirestoreProperty]
    public string audioQuestionUrl { get; set; }
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public int order { get; set; }
    [FirestoreProperty]
    public int parentId { get; set; }
    [FirestoreProperty]
    public string previewText { get; set; }
    [FirestoreProperty]
    public string questionImageUrl { get; set; }
    [FirestoreProperty]
    public string questionText { get; set; }
    [FirestoreProperty]
    public string select { get; set; }
    [FirestoreProperty]
    public string textAnswer { get; set; }
}

public class FirebaseManager : MonoBehaviour
{
    private Firebase.Auth.FirebaseAuth FirebaseAuth;
    private Firebase.Auth.FirebaseUser FirebaseUser;
    private FirebaseFirestore DatabaseInstance;
    public UserProfile userProfile;
    public List<QuestionData> DataQuestions = new List<QuestionData>();
    public List<QuestionData> DataQuestionsLocal = new List<QuestionData>();

    private List<Action<DependencyStatus>> initializedCallbacks = new List<Action<DependencyStatus>>();
    private DependencyStatus dependencyStatus;
    private List<Action> activateFetchCallbacks = new List<Action>();

    private bool initialized = false;
    private bool activateFetched = false;

    public static FirebaseManager Instance;
   // public event Action OnLoggin;
    public UIMainManager uIMainManager;

    void Start()
    {
        if (!Instance || Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        initialized = false;
        activateFetched = false;
        //OnLoggin += OnSuccessLogin;
        InitializeFirebase(OnFirebaseInitialize);
    }

    public void OnSuccessLogin()
    {
        Debug.Log("successcalled");
        GetFireStoreData(false);
        GetQuestionsData(true);
    }

    public void InitializeFirebase(Action<DependencyStatus> callback)
    {
        lock (initializedCallbacks)
        {
            if (initialized)
            {
                callback(dependencyStatus);
                return;
            }
            else
            {
                initializedCallbacks.Add(callback);
            }

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                lock (initializedCallbacks)
                {
                    dependencyStatus = task.Result;
                    initialized = true;
                    CallInitializedCallbacks();
                }
            });
        }
    }

    public void CallInitializedCallbacks()
    {
        lock (initializedCallbacks)
        {
            foreach (var callback in initializedCallbacks)
            {
                callback(dependencyStatus);
            }
            initializedCallbacks.Clear();
        }
    }

    public void OnFirebaseInitialize(DependencyStatus Status)
    {
        if (Status == DependencyStatus.Available)
        {
            FirebaseAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            DatabaseInstance= FirebaseFirestore.DefaultInstance;
            Debug.Log("initialize done");
            //uIMainManager.ToggleLoadingScreen(false);
            FirebaseAuth.StateChanged += AuthStateChanged;
        }
        else
        {
            Debug.LogError("firebase dependencies not available");
        }
    }

    public void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("called");
        Debug.Log(FirebaseAuth.CurrentUser);
        Debug.Log(FirebaseUser);

        if(FirebaseAuth.CurrentUser==null)
        {
            uIMainManager.MainScreen();
            PlayerPrefs.DeleteAll();
            return;
        }

        if (FirebaseAuth.CurrentUser != FirebaseUser)
        {
            bool signedIn = FirebaseUser != FirebaseAuth.CurrentUser && FirebaseAuth.CurrentUser != null;
            Debug.Log("signedIn");
            if (!signedIn && FirebaseUser != null)
            {
                Debug.Log("Signed out " + FirebaseUser.UserId);
                uIMainManager.ToggleLoadingScreen(false);
            }
            FirebaseUser = FirebaseAuth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + FirebaseUser.UserId);
                OnSuccessLogin();
            }
        }
        else
        {
            uIMainManager.MainScreen();
            PlayerPrefs.DeleteAll();
        }
    }

    public void SetProfileData(string _name,string _uid,string _phone,string _email,int _score,int _answered)
    {
        userProfile = new UserProfile
        {
            Name = _name,
            UID = _uid,
            PhoneNumber = _phone,
            EmailAddress = _email,
            Data = new UserData
            {
                Score = _score,
                QuestionAnswered = _answered
            }
        };
    }

    public void SignUpWithEmail(string email,string password,string phone, string name)
    {
        FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                uIMainManager.OnSignUpFailed(0);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                uIMainManager.OnSignUpFailed(1);
                return;
            }

            // Firebase user has been created.
            FirebaseUser = task.Result;

            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                FirebaseUser.DisplayName, FirebaseUser.UserId);

            UpdateUserProfile(FirebaseUser, name, "https://example.com/jane-q-user/profile.jpg");
            SetProfileData(name, FirebaseUser.UserId, phone, FirebaseUser.Email, 0, 0);
            AddFireStoreData(userProfile);
            uIMainManager.OnSignUpSuccess();
        });

    }

    public void UpdateUserProfile(Firebase.Auth.FirebaseUser _user,string _name, string _Url)
    {
        Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
        {
            DisplayName = _name,
            PhotoUrl = new System.Uri(_Url),
        };

        _user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(ProfileTask => {
            if (ProfileTask.IsCanceled)
            {
                Debug.LogError("Profile update was canceled.");
                return;
            }
            if (ProfileTask.IsFaulted)
            {
                Debug.LogError("Profile update encountered an error: " + ProfileTask.Exception);
                return;
            }

     
            Debug.Log("Firebase user updated successfully ");
        });

    }

    public void SignInWithEmail(string _email,string _pass)
    {
        FirebaseAuth.SignInWithEmailAndPasswordAsync(_email, _pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                uIMainManager.OnSignInFailed(0);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception.ToString());
                Debug.Log(task.Exception.Message);
                uIMainManager.OnSignInFailed(1);
                return;
            }

            FirebaseUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                FirebaseUser.DisplayName, FirebaseUser.UserId);

            OnSuccessLogin();
        });
    }

    public void SignOutFirebase()
    {
        FirebaseAuth.SignOut();
        PlayerPrefs.DeleteAll();
    }

    public void OnDestroy()
    {
        FirebaseAuth.StateChanged -= AuthStateChanged;
        FirebaseAuth = null;
    }


    #region Firestore DataBase
    public void AddFireStoreData(UserProfile _data)
    {
        DocumentReference docRef = DatabaseInstance.Collection("users").Document(_data.UID);
        docRef.SetAsync(_data).ContinueWithOnMainThread(task => {
            Debug.Log("Added data document in the users collection.");
        });

        var JsonString = JsonConvert.SerializeObject(_data);
        PlayerPrefs.SetString("PlayerData", JsonString);
    }

    public void GetFireStoreData(bool isLogin=false)
    {
        DocumentReference docRef = DatabaseInstance.Collection("users").Document(FirebaseUser.UserId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            var snapshot = task.Result;

            if (snapshot.Exists)
            {
                userProfile = snapshot.ConvertTo<UserProfile>();

                Debug.Log(userProfile.Name);
                Debug.Log(userProfile.EmailAddress);
                Debug.Log(userProfile.PhoneNumber);
                Debug.Log(userProfile.Data.QuestionAnswered);

                var JsonString = JsonConvert.SerializeObject(userProfile);
                PlayerPrefs.SetString("PlayerData", JsonString);

                if(isLogin)
                    uIMainManager.OnSignInSuccess();
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            Debug.Log("Read all data from the users collection.");
        });
    }

    public void CopyList()
    {
        DataQuestionsLocal.Clear();
        for (int i = 0; i < DataQuestions.Count; i++)
        {
            DataQuestionsLocal.Add(DataQuestions[i]);
        }
    }

    public void GetQuestionsData(bool isLogin = false)
    {
        DataQuestions.Clear();
        DataQuestionsLocal.Clear();
        Query QuestionCollection = DatabaseInstance.Collection("fl_content");
        QuestionCollection.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot AllQuestions = task.Result;
            foreach (DocumentSnapshot documentSnapshot in AllQuestions.Documents)
            {
                QuestionData _data = documentSnapshot.ConvertTo<QuestionData>();
                DataQuestions.Add(_data);
                DataQuestionsLocal.Add(_data);
                Debug.Log(String.Format("Document data for {0} document:", documentSnapshot.Id));
                //Dictionary<string, object> city = documentSnapshot.ToDictionary();
                //foreach (KeyValuePair<string, object> pair in city)
                //{
                //    Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                //}

                // Newline to separate entries
               // Debug.Log("");
            }

            if (isLogin)
                uIMainManager.OnSignInSuccess();
        });
    }
    #endregion

    #region Experiment to print Questions




    #endregion
}
