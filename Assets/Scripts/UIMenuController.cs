using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenuController : MonoBehaviour
{
  [SerializeField] private GameObject menuPanel;
  [SerializeField] private GameObject changeEnvironmentPanel;
  [SerializeField] private GameObject viewOptionsPanel;

  [SerializeField] private Button startButton;
  [SerializeField] private Button changeButton;
  [SerializeField] private Button exitButton;
  [SerializeField] private Button closeButton;
  [SerializeField] private Button viewOptionsButton;
  [SerializeField] private Button returnButton;

  void Start()
  {
    OpenPanel(viewOptionsPanel);
    
    viewOptionsButton.onClick.AddListener(() => OpenPanel(menuPanel));
    changeButton.onClick.AddListener(() => OpenPanel(changeEnvironmentPanel));
    closeButton.onClick.AddListener(() => OpenPanel(viewOptionsPanel));
    returnButton.onClick.AddListener(() => OpenPanel(menuPanel));
    
    startButton.onClick.AddListener(StartGame);
    exitButton.onClick.AddListener(ExitGame);
  }

  public void StartGame()
  {
    //do stuff
    // SceneManager.LoadScene(1);
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
    
    panelToOpen.SetActive(true);
  }
}