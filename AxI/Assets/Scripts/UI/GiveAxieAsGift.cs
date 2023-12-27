using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using System.Threading.Tasks;

public class GiveAxieAsGift : MonoBehaviour
{
    [SerializeField] private int axieId;
    public Text axieIdTxt;
    public GameObject flag;
    public bool uploaded;
    public Text earnTxt;
    // Start is called before the first frame update
    private void OnEnable()
    {
        UIController ui = GameObject.Find("UIController").GetComponent<UIController>();
        do
        {
            axieId = Random.Range(5, 11935533);
            StartCoroutine(LevelManager.LInstance.GetAxiesGenes(axieId.ToString(),false,true, ui.axies.Count));
        } while (LevelManager.LInstance.flag == false);

        axieIdTxt.text = "#" + axieId.ToString();
    }

    private void OnDisable()
    {
        earnTxt.text = "Earn";
        var skeletonGraphics = FindObjectsOfType<SkeletonGraphic>();
        foreach (var p in skeletonGraphics)
        {
            if (p.transform.gameObject.tag == "Gift") Destroy(p.transform.gameObject);

        }
    }

    public async void ClaimAxie()
    {
        earnTxt.text = "Earning";
        ScreenshotHandler.SInstance.objScreenShot = flag;
        ScreenshotHandler.SInstance.axieID = axieId.ToString();
        StartCoroutine(ScreenshotHandler.SInstance.ScreenShot());

        while (uploaded != true)
        {
            await Task.Delay(1);
        }

        string uri = "https://ipfs.io/ipfs/" + ScreenshotHandler.SInstance.cid;


        string[] args = { PlayerPrefs.GetString("Account"), uri };
        string response = await ContractManager.CInstance.SendTransaction("safeMint", args);
        Debug.Log(response);
        Player player = GameObject.FindGameObjectWithTag("PlayerUser").GetComponent<Player>();
        player.addAxie(axieId.ToString());
        GameObject.Find("UIController").GetComponent<UIController>().UpdateUI();
        GameObject.Find("UIController").GetComponent<UIController>().updateList(axieId.ToString());
        gameObject.SetActive(false);
    }
}
