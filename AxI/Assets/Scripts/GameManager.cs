using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public BattleUIController ui;
    // Start is called before the first frame update
    async void Awake()
    {
        StartCoroutine(LevelManager.LInstance.GetAxiesGenes(LevelManager.LInstance.axieSelect, false, false, 0));
    }

    // Update is called once per frame
    public void Won()
    {
        LevelManager.LInstance.Won = true;
        ui.End(true);
    }

    public void Lose()
    {
        LevelManager.LInstance.Won = false;
        ui.End(false);
    }
}
