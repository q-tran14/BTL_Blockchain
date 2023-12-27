using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChooseEnemy : MonoBehaviour
{
    public List<SkeletonAnimation> eneAniList;
    public SkeletonAnimation eneAniActive;
    // Start is called before the first frame update
    void Awake()
    {
        eneAniActive = eneAniList[Random.Range(0, eneAniList.Count)];
        eneAniActive.gameObject.tag = "Enemy";
        eneAniActive.AddComponent<EnemyController>();
        eneAniActive.GetComponent<EnemyController>().SetEneAni(eneAniActive);
        eneAniActive.gameObject.SetActive(true);
    }
}
