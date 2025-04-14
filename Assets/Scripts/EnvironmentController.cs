using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EnvironmentController : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private TextMeshProUGUI environmentNameText;

    [SerializeField] private List<GameObject> environments = new List<GameObject>();

    private int currentIndex;
    private GameObject currentEnvironment;

    void Start()
    {
        nextButton.onClick.AddListener(NextEnvironment);
        previousButton.onClick.AddListener(PreviousEnvironment);
        applyButton.onClick.AddListener(ApplySelectedEnvironment);

        foreach(GameObject environment in environments)
        {
            environment.SetActive(false);
        }
        
        //initialize first environment
        currentIndex = 0;
        currentEnvironment = environments[0];
        currentEnvironment.SetActive(true);

        UpdateUI();
    }

    public void NextEnvironment()
    {
        currentIndex = (currentIndex + 1) % environments.Count;
        UpdateUI();
    }

    public void PreviousEnvironment()
    {
        currentIndex = (currentIndex - 1 + environments.Count) % environments.Count;
        UpdateUI();
    }

    public void ApplySelectedEnvironment()
    {
        currentEnvironment.SetActive(false);
        environments[currentIndex].SetActive(true);
        currentEnvironment = environments[currentIndex];
    }

    private void UpdateUI()
    {
        environmentNameText.text = environments[currentIndex].name;
        applyButton.interactable = environments[currentIndex] != currentEnvironment;
    }
}