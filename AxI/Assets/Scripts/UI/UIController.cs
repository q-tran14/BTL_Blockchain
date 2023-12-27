using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AxieCore.AxieMixer;
using AxieMixer.Unity;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using UnityEngine.Networking;
using Jint.Native.Number;
using Unity.VisualScripting;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Windows;
using System.IO;
using Directory = System.IO.Directory;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEditor;

[System.Serializable]
public class Axie
{
    public int order;
    public string id;
    public GameObject axie;
    public Axie(int or, string axieID, GameObject axieOj)
    {
        order = or;
        id = axieID;
        axie = axieOj;
    }
    public override string ToString() => $"{order}+{id}+{axie.name}";
}

public class UIController : MonoBehaviour
{
    public Player player;
    public Text addressPlayer;
    public Text axieNum;
    public Button nextButton;
    public Button prevButton;
    public Button exitButton;
    public GameObject claim;
    public GameObject notice;
    public int currentAxiesIndex = 0;

    public List<Axie> axies;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        addressPlayer.text = player.getPlayerAccout().Substring(0, 7) + "..." + player.getPlayerAccout().Substring(player.getPlayerAccout().Length - 5, 5);
        UpdateUI();
        if (player.getTotalAxies() == 0) notice.SetActive(true);
        else
        {
            Debug.Log("Load List");
            loadList(player.getListAxies());
        }
        if (LevelManager.LInstance.Won == true) notice.SetActive(true);
    }
    public void UpdateUI() {
        axieNum.text = player.getTotalAxies().ToString();
    }

    public void loadList(List<string> l) {
        // Load list of axies
        StartCoroutine(LoadAxiesSequentially(l));
        nextButton.onClick.AddListener(NextAxie);
        prevButton.onClick.AddListener(PrevAxie);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void updateList(string axieId)
    {
        StartCoroutine(updateListAxie(axieId));
    }

    private IEnumerator LoadAxiesSequentially(List<string> axiesL) {
        for (int i = 0; i < axiesL.Count; i++) {
            yield return StartCoroutine(LevelManager.LInstance.GetAxiesGenes(axiesL[i].ToString(), true, false, i));
            currentAxiesIndex = i;
        }
        currentAxiesIndex = 0;
        LevelManager.LInstance.axieSelect = axies[currentAxiesIndex].id;
    }
    private IEnumerator updateListAxie(string axieId)
    {
        player.addAxie(axieId);
        yield return StartCoroutine(LevelManager.LInstance.GetAxiesGenes(axieId.ToString(), true, false, axies.Count));
    }
    public void NextAxie() {
        if (currentAxiesIndex == axies.Count - 1)
        {
            axies[currentAxiesIndex].axie.SetActive(false);
            currentAxiesIndex = 0;
            axies[currentAxiesIndex].axie.SetActive(true);
        }
        else
        {
            // tới vị trí kế
            axies[currentAxiesIndex].axie.SetActive(false);
            currentAxiesIndex++;
            axies[currentAxiesIndex].axie.SetActive(true);
        }
        LevelManager.LInstance.axieSelect = axies[currentAxiesIndex].id;
    }

    public void PrevAxie() {
        if (currentAxiesIndex == 0)
        {
            axies[currentAxiesIndex].axie.SetActive(false);
            currentAxiesIndex = axies.Count - 1;
            axies[currentAxiesIndex].axie.SetActive(true);
        }
        else
        {
            // lùi 1 vị trí
            axies[currentAxiesIndex].axie.SetActive(false);
            currentAxiesIndex--;
            axies[currentAxiesIndex].axie.SetActive(true);
        }
        LevelManager.LInstance.axieSelect = axies[currentAxiesIndex].id;
    } 

    public async void BattleGame() {
        // load next scene
        var scene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        scene.allowSceneActivation = false;
        while (scene.progress < 0.9f) await Task.Delay(1);

        scene.allowSceneActivation = true;
    }

    public void ExitGame() {
        
        string[] files = Directory.GetFiles(Application.persistentDataPath);
        if (files.Length != 0)
        {
            foreach (var f in Directory.GetFiles(Application.persistentDataPath))
            {
                if (!Path.GetFileName(f).Contains("Player"))
                {
                    Debug.Log("Delete: " + f);
                    FileInfo i = new FileInfo(f);
                    i.Delete();
                }
            }
        }
        Debug.Log("Exit");
        #if !UNITY_EDITOR
            Debug.Log("Quit");
            Application.Quit();
        #else
            EditorApplication.ExitPlaymode();
        #endif
    }
    
    public void Gift()
    {
        claim.SetActive(true);
        
        notice.SetActive(false);
    }
}