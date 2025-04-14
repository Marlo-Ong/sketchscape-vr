using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class EnvironmentController : MonoBehaviour
{
    [Serializable] public class ButtonEnvironmentPair
    {
        public Button button;
        public GameObject environment;
    }
    
    [SerializeField] public List<ButtonEnvironmentPair> environmentPairs = new List<ButtonEnvironmentPair>();
    public GameObject currentEnvironment;
    
    void Start()
    {
        currentEnvironment = environmentPairs[0].environment;
        currentEnvironment.SetActive(true);

        foreach (var pair in environmentPairs)
        {
            pair.button.onClick.AddListener(() => SwitchEnvironment(pair.environment));
        }
    
        UpdateButtons();
    }
    
    private void SwitchEnvironment(GameObject newEnvironment)
    {
        currentEnvironment.SetActive(false);
        newEnvironment.SetActive(true);
        currentEnvironment = newEnvironment;
        
        UpdateButtons();
    }
    
    private void UpdateButtons()
    {
        foreach (var pair in environmentPairs)
        {
            if (pair.environment == currentEnvironment)
            {
                pair.button.interactable = false;
            } 
            else 
            {
                pair.button.interactable = true;
            }
        }
    
    }
}