using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;

public static class Prompts
{
    public static readonly string[] prompts =
    {
        "A drawer of snakes",
        "Bear in a paper boat on the river rapids",
        "Blue ribbon pie",
        "Boots with the fur",
        "Bridge made of pasta",
        "Catching shooting stars",
        "Evil rabbit with four eyes",
        "Failed science experiment",
        "Fish bowl of eyeballs",
        "Fish swimming through clouds",
        "Frog in PJs",
        "Green eggs and ham",
        "Haunted sock monkey",
        "House made of candy",
        "Mice having a tea party",
        "Money that grows on trees",
        "Origami crane in the ocean",
        "Pickle soup",
        "Rabbit wearing clown shoes",
        "Shrek in a bikini",
        "Singing sardines",
        "Snail delivery-man",
        "Snowman made of sand",
        "Sock puppet concert",
        "Spaghettios",
        "Squid ink pasta",
        "Squid with a lollipop",
        "Stapler in jello",
        "Trail of dirty laundry",
        "Tree with hands and feet",
        "Volcano spewing grape soda"
    };
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int timeLimit;

    [Header("References To Assign")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button startButton;

    private List<string> unusedPrompts;
    private Coroutine roundEndTimer;
    public static event Action OnRoundStart;

    private void Start()
    {
        this.startButton.onClick.AddListener(StartRound);
        this.unusedPrompts = new();
        this.promptPanel.SetActive(false);
    }


    #region Game Loop Methods

    public void StartRound()
    {
        OnRoundStart?.Invoke();

        this.startButton.interactable = false;
        this.SetPromptRandom();
        this.promptPanel.SetActive(true);

        if (this.roundEndTimer != null)
        {
            StopCoroutine(this.roundEndTimer);
            this.roundEndTimer = null;
        }
        this.roundEndTimer = StartCoroutine(this.WaitForCallback(this.timeLimit, this.EndRound));
    }

    public void EndRound()
    {
        this.startButton.interactable = true;
        this.roundEndTimer = null;
        this.promptPanel.SetActive(false);
    }

    #endregion

    #region Prompt Text Methods

    public void SetText(string text)
    {
        Debug.Assert(this.promptText != null);
        this.promptText.text = text;
    }

    public void SetPromptRandom()
    {
        Debug.Assert(this.unusedPrompts != null);

        // Refresh prompts if all have been used.
        if (this.unusedPrompts.Count == 0)
            this.unusedPrompts.AddRange(Prompts.prompts);

        // Pick a random unused prompt.
        int i = UnityEngine.Random.Range(0, this.unusedPrompts.Count);
        this.SetText(this.unusedPrompts[i]);
    }

    #endregion

    private IEnumerator WaitForCallback(int seconds, Action callback)
    {
        float timeRemaining = seconds;
        while (timeRemaining > 0)
        {
            if (this.timerText != null)
            {
                int mins = (int)(timeRemaining / 60);
                int secs = (int)(timeRemaining % 60);
                this.timerText.text = $"{mins:D2}:{secs:D2}";
            }

            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        callback();
    }
}