using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using System;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using Newtonsoft.Json;
using Google;
using System.Threading.Tasks;
using Firebase.Auth;

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
    private bool SignInAutomatically = false;
    private bool SignInGoogleAutomatically = false;
    private bool SignInFBAutomatically = false;
    PlayerDataSave jsonObject;

    public static FirebaseManager Instance=null;
   // public event Action OnLoggin;
    [HideInInspector]
    public UIMainManager uIMainManager;
    [HideInInspector]
    public bool IsGoogleSignIn = false;
    private string googleSignClientID= "562499227773-qu1kc39mn99s9ltjo8pmfkgq0cqeivsc.apps.googleusercontent.com";
    private GoogleSignInConfiguration config;

    public void CheckRef()
    {
        uIMainManager = GameObject.FindObjectOfType<UIMainManager>();
    }
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        FBManager.Instance.OnFacebookLoginSuccessful += OnFacebookLogin;
        FBManager.Instance.OnFacebookInitialized += OnFacebookLogin;
        FBManager.Instance.OnFacebookLoginFail += OnFacebookLoginFail;
    }

    private void Update()
    {
#if UNITY_STANDALONE
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
#endif
    }
    void OnEnable()
    {
        CheckRef();
        
        //PlayerPrefs.DeleteAll();

        config= new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = googleSignClientID
        };

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
          Destroy(this.gameObject);
          return;
        }

        SignInAutomatically = false;
        SignInGoogleAutomatically = false;
        SignInFBAutomatically = false;

        string _data = GameData.GetSavePlayerData();

        if (_data != "" && _data != null)
        {
            jsonObject = JsonConvert.DeserializeObject<PlayerDataSave>(_data);
            SignInAutomatically = true;
        }

        string _googledata = GameData.GetGoogleData();

        if (_googledata != "" && _googledata != null)
        {
            SignInGoogleAutomatically = true;
        }

        string _fbdata = GameData.GetFBData();

        if (_fbdata != "" && _fbdata != null)
        {
            SignInFBAutomatically = true;
        }

        initialized = false;
        activateFetched = false;
        //OnLoggin += OnSuccessLogin;
        InitializeFirebase(OnFirebaseInitialize);
    }

    public void OnSuccessLogin()
    {
        Debug.Log("successcalled");
        GetFireStoreData(true);
        GetQuestionsData(false);
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

    public void LogginFailed()
    {
        CheckRef();
        GameData.DeletePrefData();
        uIMainManager.ToggleLoadingScreen(false);
        uIMainManager.NextScreen(0);
    }

    public void OnFirebaseInitialize(DependencyStatus Status)
    {
        CheckRef();
        if (Status == DependencyStatus.Available)
        {
            FirebaseAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            DatabaseInstance= FirebaseFirestore.DefaultInstance;
            //Debug.LogError("initialize done");
            //uIMainManager.ToggleLoadingScreen(false);
            FirebaseAuth.StateChanged += AuthStateChanged;

            if(SignInAutomatically)
            {
                uIMainManager.ToggleLoadingScreen(true);
                SignInWithEmail(jsonObject.email, jsonObject.pass);
            }else if(SignInGoogleAutomatically)
            {
                uIMainManager.ToggleLoadingScreen(true);
                SignInWithGoogle(false);
            }else if(SignInFBAutomatically)
            {
                uIMainManager.ToggleLoadingScreen(true);
                FBManager.Instance.Login();
            }
            else
            {
                LogginFailed();
            }
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
            LogginFailed();
            return;
        }

        if (FirebaseAuth.CurrentUser != FirebaseUser)
        {
            //bool signedIn = FirebaseUser != FirebaseAuth.CurrentUser && FirebaseAuth.CurrentUser != null;
            //Debug.Log("signedIn");
            //if (!signedIn && FirebaseUser != null)
            //{
            //    Debug.Log("Signed out " + FirebaseUser.UserId);
            //    uIMainManager.ToggleLoadingScreen(false);
            //}
            //FirebaseUser = FirebaseAuth.CurrentUser;
            //if (signedIn)
            //{
            //    Debug.Log("Signed in " + FirebaseUser.UserId);
            //    OnSuccessLogin();
            //}
        }
        else
        {
            LogginFailed();
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
        CheckRef();
        FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                uIMainManager.OnSignUpFailed(0);
                LogginFailed();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                uIMainManager.OnSignUpFailed(1);
                LogginFailed();
                return;
            }

            // Firebase user has been created.
            FirebaseUser = task.Result;

            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                FirebaseUser.DisplayName, FirebaseUser.UserId);

            UpdateUserProfile(FirebaseUser, name, "https://example.com/jane-q-user/profile.jpg");
            SetProfileData(name, FirebaseUser.UserId, phone, FirebaseUser.Email, 0, 0);
            AddFireStoreData(userProfile);
            GetQuestionsData(false);
            uIMainManager.OnSignUpSuccess();
        });

    }

    public void GuestSignIn()
    {
        string ID=UnityEngine.Random.Range(1111,9999).ToString();
        string name="Guest"+ID;
        string email=name+"@gmail.com";
        string phone="065845410";
        SetProfileData(name, ID, phone, email, 0, 0);
        GetQuestionsData(false);
        uIMainManager.OnSignUpSuccess();
    }

    public void SignInWithGoogle(bool linkWithCurrentAnonUser)
    {
        IsGoogleSignIn = true;
        CheckRef();
        GoogleSignIn.Configuration = config;
        //TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("google sign in was cancelled");
                //signInCompleted.SetCanceled();
                uIMainManager.OnSignUpFailed(1);
                LogginFailed();
                return;
            }
            else if (task.IsFaulted)
            {
                Debug.Log("google sign in was faulted");
                uIMainManager.OnSignUpFailed(1);
                LogginFailed();
                return;
            }
            else
            {
                Credential _credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                if (linkWithCurrentAnonUser)
                {
                //FirebaseAuth.CurrentUser.LinkWithCredentialAsync(_credential).ContinueWith(main => {
                //    if(main.IsCanceled)
                //    {
                //        return;
                //    }
                //    else if (main.IsFaulted)
                //    {
                //        return;
                //    }
                //    else
                //    {

                //    }
                //});
                }
                else
                {
                    SignInWithCredentials(_credential);
                }
            }
        });
    }

    public void SignInWithCredentials(Credential credential)
    {
        CheckRef();
        FirebaseAuth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                uIMainManager.OnSignUpFailed(1);
                LogginFailed();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                uIMainManager.OnSignUpFailed(1);
                LogginFailed();
                return;
            }

            FirebaseUser = task.Result;

            Debug.LogFormat("Firebase user created successfullyyyyyyyyy: {0} ({1})",
                FirebaseUser.DisplayName, FirebaseUser.UserId);

            CheckUser(FirebaseUser.UserId);
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
        CheckRef();
        FirebaseAuth.SignInWithEmailAndPasswordAsync(_email, _pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                uIMainManager.OnSignInFailed(0);
                LogginFailed();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception.ToString());
                Debug.Log(task.Exception.Message);
                uIMainManager.OnSignInFailed(1);
                LogginFailed();
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
        SignInGoogleAutomatically = false;
        SignInAutomatically = false;

        GameData.DeletePrefData();

        if (FirebaseAuth!=null)
            FirebaseAuth.SignOut();
    }

    //public void OnDestroy()
    //{
    //    FirebaseAuth.StateChanged -= AuthStateChanged;
    //    FirebaseAuth = null;
    //}


    #region Firestore DataBase
    public void AddFireStoreData(UserProfile _data)
    {
        DocumentReference docRef = DatabaseInstance.Collection("users").Document(_data.UID);
        docRef.SetAsync(_data).ContinueWithOnMainThread(task => {
            Debug.Log("Added data document in the users collection.");
        });

        //var JsonString = JsonConvert.SerializeObject(_data);
        //PlayerPrefs.SetString("PlayerData", JsonString);
    }
    public void CheckUser(string _id)
    {
        CheckRef();
        DocumentReference docRef = DatabaseInstance.Collection("users").Document(_id);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            var snapshot = task.Result;

            if (snapshot.Exists)
            {
                Debug.Log("user exists loggin in!");
                OnSuccessLogin();

                if(IsGoogleSignIn)
                    GameData.SetGoogleData();
                else
                    GameData.SetFBData();
            }
            else
            {
                Debug.Log("user does not exists, creating new one");

                string _email = FirebaseUser.Email;
                if (_email == "")
                    _email = FirebaseUser.UserId + "@gmail.com";
                
                UpdateUserProfile(FirebaseUser, FirebaseUser.DisplayName, "https://example.com/jane-q-user/profile.jpg");
                SetProfileData(FirebaseUser.DisplayName, FirebaseUser.UserId, "090084241", _email, 0, 0);
                AddFireStoreData(userProfile);
                GetQuestionsData(false);
                uIMainManager.OnSignUpSuccess();

                if (IsGoogleSignIn)
                    GameData.SetGoogleData();
                else
                    GameData.SetFBData();
            }
        });
    }


    public void GetFireStoreData(bool isLogin=false)
    {
        CheckRef();
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

                //var JsonString = JsonConvert.SerializeObject(userProfile);
                //PlayerPrefs.SetString("PlayerData", JsonString);

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
        CheckRef();
        DataQuestions.Clear();
        DataQuestionsLocal.Clear();
        Query QuestionCollection = DatabaseInstance.Collection("fl_content");
        QuestionCollection.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log(task.Result);
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

    #region Facebbok
    void OnFacebookLogin()
    {
        IsGoogleSignIn = false;
        //var request = new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString };
        // PlayFabClientAPI.LoginWithFacebook(request, OnLoginSuccess, OnLoginFailure);
        Debug.Log("login success: "+ FBManager.Instance.AccessTokenFB);
        Credential _credential = Firebase.Auth.FacebookAuthProvider.GetCredential(FBManager.Instance.AccessTokenFB);
        SignInWithCredentials(_credential);
    }

    void OnFacebookLoginFail()
    {
        //if (FBLoginCallback != null)
        //    FBLoginCallback(false, "fail");
        //if(FBLoginCallback != null)
        //{
        uIMainManager.OnSignUpFailed(1);
        Debug.Log("failure");
        CheckRef();
        GameData.DeletePrefData();
        uIMainManager.ToggleLoadingScreen(false);
        uIMainManager.NextScreen(0);
        //}
    }

    #endregion

}
