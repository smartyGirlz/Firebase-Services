﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Auth;

using Facebook.Unity;


using UnityEngine.SceneManagement;

public class facebookLoginHandler : MonoBehaviour
{
   public Text name;
	Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    void Awake()
	{
		//Setup for Facebook Sign In
		if (!FB.IsInitialized)
		{
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		}
		else
		{
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
	}
	
    public void FacebookSignIn_Click()
	{
		OnFacebookSignIn();
	}

	void OnFacebookSignIn()
	{
		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, OnFacebookAuthenticationFinished);
	}

	void OnFacebookAuthenticationFinished(IResult result)
	{
		if (FB.IsLoggedIn)
		{
			//Firebase Auth
			FacebookAuth(AccessToken.CurrentAccessToken.TokenString);
		}
		else
		{
			Debug.Log("User cancelled login");
		}
	}

	private void FacebookAuth(string accessToken)
	{
		Firebase.Auth.Credential credential =
		Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);

		auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
		{
			if (task.IsCanceled)
			{
				Debug.Log("SignInWithCredentialAsync was canceled.");
				return;
			}
			if (task.IsFaulted)
			{
				Debug.Log("SignInWithCredentialAsync encountered an error: ");
				return;
			}

			user = auth.CurrentUser;

            name.text = user.DisplayName;

            SceneManager.LoadScene("Game", LoadSceneMode.Single);
	
    		Debug.Log(string.Format("User signed in successfully: {0} ({1})",
				user.DisplayName, user.UserId));
		});
	}

	private void InitCallback()
	{
		Debug.Log("FB Init done.");

		if (FB.IsLoggedIn)
		{
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
		}
		else
		{
			Debug.Log("User cancelled login");
		}
	}

	private void OnHideUnity(bool isGameShown)
 
	{
		if (!isGameShown)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}
}