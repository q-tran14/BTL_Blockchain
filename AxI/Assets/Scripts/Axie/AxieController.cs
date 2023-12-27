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
    // Start is called before the first frame update
    void Start()
    {
        float hp = Random.Range(300,501);
        float dmg = Random.Range(10,51);
        data = new AxieData(hp, dmg);

        previousState = "";
        currentState = "action/idle/normal";
        axieSke = gameObject.GetComponent<SkeletonAnimation>();
        axieSke.state.SetAnimation(0, currentState, true);
        gameObject.AddComponent<BoxCollider2D>();
        gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        move = true;

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
            axieSke.state.SetAnimation(0, currentState, true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enter enemy");
        
        string[] type = {"mouth-bite","horn-gore","multi-attack","normal-attack" };
        int i = Random.Range(0, type.Length);
        if (collision.collider.tag == "Enemy" && collision.collider.transform.parent.GetComponent<EnemyController>().enemy.HP > 0)
        {
            collisionEnemy = true;
            changeState("attack/melee/" + type[i]);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Enter collision on stay enemy");
        if (collision.collider.tag == "Enemy" && collision.collider.transform.parent.GetComponent<EnemyController>().enemy.HP > 0)
        {
            if (axieSke.state.GetCurrent(0).IsComplete == true)
            {
                if (collision.collider.transform.parent.GetComponent<EnemyController>().enemy.HP - data.dmg > 0) collision.collider.transform.parent.GetComponent<EnemyController>().enemy.HP -= data.dmg;
                else {
                    collisionEnemy = false;
                    move = false;
                    collision.collider.transform.parent.GetComponent<EnemyController>().Die();
                }
            }
        }
    }

    public void Die()
    {
        Debug.Log("Die");
        axieSke.state.SetAnimation(0, "action/idle/normal", false);

        GameObject.Find("Game Manager").GetComponent<GameManager>().Lose();
        Destroy(gameObject, 10);
    }
}
