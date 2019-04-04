using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QAHandler : MonoBehaviour
{
    public GameObject Panel;
    public Button button;
    void Start()
    {
        Panel.gameObject.SetActive(false);
        button.onClick.AddListener(OnClick);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {
        Panel.SetActive(!Panel.activeSelf);
    }
}
