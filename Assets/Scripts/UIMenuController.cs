using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIMenuController : MonoBehaviour
{
  [SerializeField] private GameObject menuPanel;
  [SerializeField] private GameObject changeEnvironmentPanel;
  [SerializeField] private GameObject viewOptionsPanel;
  [SerializeField] private GameObject viewControlsPanel;

  [SerializeField] private Button startButton;
  [SerializeField] private Button changeButton;
  [SerializeField] private Button exitButton;
  [SerializeField] private Button closeButton;
  [SerializeField] private Button viewOptionsButton;
  [SerializeField] private Button viewControlsButton;
  [SerializeField] private  List<Button> returnButtons;

  void Start()
  {
    OpenPanel(viewOptionsPanel);
    
    viewOptionsButton.onClick.AddListener(() => OpenPanel(menuPanel));
    changeButton.onClick.AddListener(() => OpenPanel(changeEnvironmentPanel));
    closeButton.onClick.AddListener(() => OpenPanel(viewOptionsPanel));
    viewControlsButton.onClick.AddListener(() => OpenPanel(viewControlsPanel));

    foreach(Button button in returnButtons)
    {
      button.onClick.AddListener(() => OpenPanel(menuPanel));
    }
    
    exitButton.onClick.AddListener(ExitGame);
  }

  public void ExitGame()
  {
    // works in build, not in unity editor play test
    Debug.Log("Exit only works in build.");
    Application.Quit();
  }

  public void OpenPanel(GameObject panelToOpen)
  {
    menuPanel.SetActive(false);
    changeEnvironmentPanel.SetActive(false);
    viewOptionsPanel.SetActive(false);
    viewControlsPanel.SetActive(false);
    
    panelToOpen.SetActive(true);
  }
}