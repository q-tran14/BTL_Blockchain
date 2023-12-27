using AxieMixer.Unity;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Enemy
{
    public float HP;
    public float dmg;

    public Enemy(float hp, float d)
    {
        HP = hp;
        dmg = d;
    }
}

public class EnemyController : MonoBehaviour
{
    public Enemy enemy;
    public string currentState;
    public string previousState;
    public List<SkeletonAnimation> eneAniList;
    public SkeletonAnimation eneAniActive;
    public bool collisionPlayer = false;
    public bool move = false;
    public float speed;

    void Awake()
    {
        float hp = Random.Range(300, 501);
        float dmg = Random.Range(10, 101);
        enemy = new Enemy(hp, dmg);

        eneAniActive = eneAniList[Random.Range(0, eneAniList.Count)];
        eneAniActive.gameObject.tag = "Enemy"; 
        eneAniActive.gameObject.SetActive(true);

        previousState = "";
        currentState = "action/idle/normal";
        eneAniActive.GetComponent<SkeletonAnimation>().state.SetAnimation(0, currentState, true);
        move = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (collisionPlayer == false)
        {
            if (move == true)
            {
                changeState("action/move-forward");
                gameObject.transform.position -= transform.right * speed * Time.deltaTime;
            }
            else if (move == false)
            {
                Debug.Log("Run");
                changeState("action/idle/normal");
            }
        }
    }

    public void changeState(string newState)
    {
        if (newState != currentState && currentState != previousState)
        {
            Debug.Log(newState + "  :  " + currentState);
            previousState = currentState;
            currentState = newState;
            eneAniActive.state.SetAnimation(0, currentState, true);
        }
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enter player");
        
        if (collision.collider.tag == "Player" && collision.collider.GetComponent<AxieController>().data.hp > 0)
        {
            collisionPlayer = true;
            changeState("attack/melee/normal-melee");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Enter on collision stay player");
        if (collision.collider.tag == "Player" && collision.collider.GetComponent<AxieController>().data.hp > 0)
        {
            if (eneAniActive.state.GetCurrent(0).IsComplete == true)
            {
                if (collision.collider.GetComponent<AxieController>().data.hp - enemy.dmg > 0) collision.collider.gameObject.GetComponent<AxieController>().data.hp -= enemy.dmg;
                else
                {
                    collisionPlayer = false;
                    move = false;
                    collision.collider.gameObject.GetComponent<AxieController>().Die();
                }
            }
        }
    }

    public void Die()
    {
        Debug.Log("Enemy died");
        eneAniActive.state.SetAnimation(0, "action/idle/normal", false);
        eneAniActive.GetComponent<MeshRenderer>().materials[0].SetColor("Tint", new Color(82, 69, 69, 255)); ;

        GameObject.Find("Game Manager").GetComponent<GameManager>().Won();
        Destroy(gameObject, 100);
    }
}

