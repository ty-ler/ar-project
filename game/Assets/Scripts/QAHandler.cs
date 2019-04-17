using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QAHandler : MonoBehaviour
{
    public GameObject Panel;
    public Button button;
    public Text Question;
    public Image HealthBar;
    public float health;
    public float startHealth;
    public Text HealthPercent;
    void Start()
    {
        Panel.gameObject.SetActive(false);
        button.onClick.AddListener(OnClick);
        Question.text = "What is the sqaure root of 49?";
        HealthBar.fillAmount = startHealth;
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick()
    {
        Panel.SetActive(!Panel.activeSelf);
    }
    public void OnDamage() 
    {
        health = health - 0.1f;
        HealthBar.fillAmount = health;
        HealthPercent.text = health / 1 + "%";
    }
}
