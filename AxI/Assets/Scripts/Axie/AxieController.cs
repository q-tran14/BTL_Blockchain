using MongoDB.Driver;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AxieData
{
    public float hp;
    public float dmg;

    public AxieData(float HP, float d)
    {
        hp = HP;
        dmg = d;
    }
}
public class AxieController : MonoBehaviour
{
    [SerializeField]public AxieData data;
    
    public string currentState;
    public string previousState;

    public SkeletonAnimation axieSke;

    public bool collisionEnemy;
    public bool move = false;
    public float speed = 2;
    public HealthBar enemyHealth;

    public float countDown;
    public bool canCount;
    public GameObject enemyObj;
    // Start is called before the first frame update
    void Start()
    {
        float hp = 500;
        float dmg = 300;
        data = new AxieData(hp, dmg);

        previousState = "";
        currentState = "action/idle/normal";
        axieSke = gameObject.GetComponent<SkeletonAnimation>();
        axieSke.state.SetAnimation(0, currentState, true);
        gameObject.AddComponent<BoxCollider2D>();
        gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        move = true;

        // Set up health Bar for axie
        GameObject ui = GameObject.Find("UIController");
        ui.GetComponent<BattleUIController>().SetUpHealthBar(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (collisionEnemy == false)
        {
            if (move == true)
            {
                changeState("action/move-forward");
                gameObject.transform.position += transform.right * speed * Time.deltaTime;
            }
        }
        if (data.hp <= 0)
        {
            changeState("action/idle/normal");
            Die();
        }
        //Bug
        if (collisionEnemy == true && canCount == true && enemyObj != null) { 
            countDown += 0.001f;
            if (countDown >= axieSke.state.GetCurrent(0).AnimationEnd)
            {
                if (enemyObj.GetComponent<EnemyController>().enemy.HP > 0)
                {
                    enemyObj.GetComponent<EnemyController>().enemy.HP -= data.dmg;
                    enemyHealth.OnHPChange(data.dmg);
                    Debug.Log("Enemy HP: -" + data.dmg);
                    countDown = 0;
                }
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
            axieSke.state.SetAnimation(0, currentState, true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enter enemy");
        enemyHealth = GameObject.Find("Enemy Health Bar").GetComponent<HealthBar>();
        string[] type = {"mouth-bite","horn-gore","multi-attack","normal-attack" };
        int i = Random.Range(0, type.Length);
        if (collision.collider.tag == "Enemy" && collision.collider.GetComponent<EnemyController>().enemy.HP > 0)
        {
            //Bug
            countDown = 0;
            canCount = true;
            //
            collisionEnemy = true;
            changeState("attack/melee/" + type[i]);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Enter collision on stay enemy");
        if (collision.collider.tag == "Enemy" && collision.collider.GetComponent<EnemyController>().enemy.HP > 0)
        {
            Debug.Log("Start fight with enemy");
            StartFighting(collision.gameObject);
            ////Bug
            //Debug.Log(canCount);
            //if (countDown >= axieSke.state.GetCurrent(0).AnimationEnd + 2)
            //{
            //    //Bug
            //    canCount = false;
            //    countDown = 0;
            //    //
            //    if (collision.collider.GetComponent<EnemyController>().enemy.HP - data.dmg > 0) {
            //        collision.collider.GetComponent<EnemyController>().enemy.HP -= data.dmg;
            //        enemyHealth.OnHPChange(data.dmg);
            //        Debug.Log("Enemy HP: -" + data.dmg);
            //    }
            //}
            ////Bug
            //canCount = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        collisionEnemy = false;
        StartFighting(null);
    }

    public void Die()
    {
        Debug.Log("Die");
        axieSke.state.SetAnimation(0, "action/idle/normal", false);

        GameObject.Find("Game Manager").GetComponent<GameManager>().Lose();
        Destroy(gameObject, 10);
    }

    public void StartFighting(GameObject ene)
    {
        enemyObj = ene;
    }
}
