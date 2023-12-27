 using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

public class WalletLogin: MonoBehaviour
{
    ProjectConfigScriptableObject projectConfigSO = null;
    public Toggle rememberMe;
    public string account;

    public GameObject background;

    public GameObject panel;
    public GameObject loginBtn;
    public GameObject toggleBox;

    public GameObject loading;
    public GameObject loadingVideo;
    public GameObject loginError;
    void Start() {
        projectConfigSO = (ProjectConfigScriptableObject)Resources.Load("ProjectConfigData", typeof(ScriptableObject));
        PlayerPrefs.SetString("ProjectID", projectConfigSO.ProjectId);
        PlayerPrefs.SetString("ChainID", projectConfigSO.ChainId);
        PlayerPrefs.SetString("Chain", projectConfigSO.Chain);
        PlayerPrefs.SetString("Network", projectConfigSO.Network);
        PlayerPrefs.SetString("RPC", projectConfigSO.Rpc);
        
        // if remember me is checked, set the account to the saved account
        //#if !UNITY_EDITOR
        //if(PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.HasKey("Account"))
        //{
        //    if (PlayerPrefs.GetInt("RememberMe") == 1 && PlayerPrefs.GetString("Account") != "")
        //    {
        //        // move to next scene
        //        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //        LoadHome();
        //    }
        //}
        //#endif
    }

    async public void OnLogin()
    {
        // get current timestamp
        int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // set expiration time
        int expirationTime = timestamp + 60;
        // set message
        string message = expirationTime.ToString();
        // sign message
        string signature = "";

        Player user = GameObject.Find("Player").GetComponent<Player>(); 
        try
        {
            signature = await Web3Wallet.Sign(message);
            // verify account
            account = await EVM.Verify(message, signature);
            int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            // validate
            if (account.Length == 42 && expirationTime >= now)
            {
                // save account
                PlayerPrefs.SetString("Account", account);
                if (rememberMe.isOn) PlayerPrefs.SetInt("RememberMe", 1);
                else PlayerPrefs.SetInt("RememberMe", 0);
                user.setAccount(account);
                DontDestroyOnLoad(user);
                checkDataInSmartContract();

                await Task.Delay(5000);
                LoadHome();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            loginError.SetActive(true);
            await Task.Delay(3000);
            loginError.SetActive(false);
        }
    }

    public void checkDataInSmartContract()
    {
        string[] args = {account};
        Debug.Log("Check User Axies");
        ContractManager.CInstance.GetUserAxieFromSmartContract(args);
    }

    public async void LoadHome()
    {
        // load next scene
        var scene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        scene.allowSceneActivation = false;
        
        // Disable UI
        panel.SetActive(false);
        loginBtn.SetActive(false);
        toggleBox.SetActive(false);

        // Enable loading Object
        loadingVideo.SetActive(true);
        loading.SetActive(true);
        // Disable background
        SceneLoader.SInstance.Run(background.GetComponent<Image>());
        
        await Task.Delay(5000);
        scene.allowSceneActivation = true;
    }

    public void Exit()
    {
        #if !UNITY_EDITOR
            Application.Quit();
        #else
            EditorApplication.ExitPlaymode();
        #endif
    }
}
