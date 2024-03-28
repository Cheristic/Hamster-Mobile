using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Unity.Services.Core;

public class CloudServicesManager : MonoBehaviour
{
    public static CloudServicesManager cloud { get; private set; }

    [Serializable]
    public enum Platform {
        Android,
        iOS,
        Itch
    }
    public Platform platform;

    internal PlatformManager platformMan;
    internal UnityServicesHandler unityServices;

    // Local variables
 
    private void Awake()
    {
        if (cloud != null && cloud != this)
        {
            Destroy(this);
        } else
        {
            cloud = this;
        }

        unityServices = GetComponent<UnityServicesHandler>();
        

        switch (platform)
        {
            case Platform.Android:
                platformMan = GetComponentInChildren<GPGSManager>();
                break;
            case Platform.iOS:
                platformMan = GetComponentInChildren<GameCenterManager>();
                break;
        }    
    }

    private async void Start()
    {
        await unityServices.Activate();
        platformMan.Activate();
        await platformMan.GetAuthenticationResults();
        await platformMan.AuthenticateWithUnity();
        GameManager.gameStart += InitializeAttemptLogin;
    }

    private void InitializeAttemptLogin() { AttemptLogin(); }

    public async Task AttemptLogin()
    {
        if (IsConnected)
        {
            Debug.Log("AttemptLogin() - Connected already");
        } else
        {
            Debug.Log("AttemptLogin() - Not connected, trying again...");

            if (await platformMan.GetAuthenticationResults())
            {
                //  && await platformMan.AuthenticateWithUnity()
                Debug.Log("Successfully Logged in!!!!!!!");
            }
            else
            {
                Debug.Log("Failed to log in from CloudServicesManager");
            }
        }
        
    }

    public bool IsConnected
    {
        get
        {
            return platformMan.IsConnected() && unityServices.IsConnected();
        }
    }

    public void ShowLeaderboard()
    {
        switch (platform)
        {
            case Platform.Android:
                platformMan.ShowLeaderboard();
                break;
        }
    }
}


public class PlatformManager : MonoBehaviour
{
    public bool connectedToPlatform;
    public virtual void Activate() { }
    public virtual Task<bool> GetAuthenticationResults() { return null; }

    public virtual Task<bool> ManuallyAuthenticate() { return null; }
    public virtual async Task<bool> AuthenticateWithUnity()
    { return true; }
    public virtual bool IsConnected() { return true; }
    public virtual void AddScore() { }
    public virtual void ShowLeaderboard() { }

}
