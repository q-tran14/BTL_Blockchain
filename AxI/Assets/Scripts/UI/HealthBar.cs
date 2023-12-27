using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider fill;
    public float hp;
    public GameObject objToFollow;
    public RectTransform targetCanvas;
    // Start is called before the first frame update
    public void SetUp(GameObject obj, float HP)
    {
        objToFollow = obj;
        hp = HP;
        fill = gameObject.GetComponent<Slider>();
        fill.maxValue = hp;
        fill.value = hp;
        repositionHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        repositionHealthBar();
    }

    public void OnHPChange(float dmg)
    {
        fill.value -= dmg;
    }

    public void repositionHealthBar()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objToFollow.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.3f)));
        //now you can set the position of the ui element
        gameObject.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
    }
}
