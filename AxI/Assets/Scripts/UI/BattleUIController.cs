using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public GameObject panel;
    public Text wonOrLose;
    public Button returnHome;

    public Vector2 offset;
    public GameObject axieHealthBar;
    public GameObject eneHealthBar;

    public void Start()
    {
        GameObject ene = GameObject.FindGameObjectWithTag("Enemy");
        
        float eneHp = ene.GetComponent<EnemyController>().enemy.HP;
        Debug.Log(eneHp);
        eneHealthBar.GetComponent<HealthBar>().SetUp(ene, eneHp);
        eneHealthBar.SetActive(true);
    }

    public void SetUpHealthBar(GameObject axie)
    {
        float axieHp = axie.GetComponent<AxieController>().data.hp;
        axieHealthBar.GetComponent<HealthBar>().SetUp(axie, axieHp);
        axieHealthBar.SetActive(true);
    }
    public void End(bool win)
    {
        panel.SetActive(true);
        Time.timeScale = 0;
        if (win == true)
        {
            wonOrLose.text = "YOU WON !!!";
        }
        else
        {
            wonOrLose.text = "YOU HAVE BEEN DEFEATED !!!";
        }
    }

    public async void ReturnHome()
    {
        var scene = await SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        scene.allowSceneActivation = false;
        await Task.Delay(5000);
        Time.timeScale = 1; 
        scene.allowSceneActivation = true;
    }
}
