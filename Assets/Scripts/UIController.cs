using System.Collections;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private TMP_Text scoreUI;
    [SerializeField] private GameObject nextTurnPanel;
    [SerializeField] private GameObject placePinDeckPanel;
    [SerializeField] private GameObject controlsPanel1;
    [SerializeField] private GameObject controlsPanel2;
    [SerializeField] private TMP_Text remainingBallsUI;
    [SerializeField] private GameObject strikePanel;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text finalScoreUI;
    [SerializeField] private float turnWaitTime = 3;
    
    private bool _throwInstructionShowed = false;
    
    private void Start()
    {
        // show place pin panel
        placePinDeckPanel.SetActive(true);

        // update HUD score and balls
        UpdateAmountOfBallsUI();
        UpdateScoreUI(0);
    }
    
    private void OnEnable()
    {
        gameState.onEnterBallSetup.AddListener(HidePlaceInDeckPanel);

        gameState.onScoreChanged.AddListener(UpdateScoreUI);
        gameState.onBallInPlay.AddListener(UpdateAmountOfBallsUI);
        gameState.onTurnEnded.AddListener(ShowNextTurnUI);
        
        gameState.onStrikeAchieved.AddListener(ShowStrikeUI);
        gameState.onGameEnded.AddListener(ShowGameOverScreen);
    }

    private void OnDisable()
    {
        gameState.onEnterBallSetup.RemoveListener(HidePlaceInDeckPanel);

        gameState.onScoreChanged.RemoveListener(UpdateScoreUI);
        gameState.onBallInPlay.RemoveListener(UpdateAmountOfBallsUI);
        gameState.onTurnEnded.RemoveListener(ShowNextTurnUI);
        
        gameState.onStrikeAchieved.RemoveListener(ShowStrikeUI);
        gameState.onGameEnded.RemoveListener(ShowGameOverScreen);
    }

    private void HidePlaceInDeckPanel()
    {
        // hide place pin panel and show control instructions
        placePinDeckPanel.SetActive(false);

        ShowControls();
    }

    private void ShowControls()
    {
        if (_throwInstructionShowed) return;

        _throwInstructionShowed = true;
        controlsPanel1.SetActive(true);

        Invoke(nameof(HideControls_1), 3);
    }

    private void HideControls_1()
    {
        controlsPanel1.SetActive(false);
        controlsPanel2.SetActive(true);

        Invoke(nameof(HideControls_2), 3);
    }

    private void HideControls_2()
    {
        controlsPanel2.SetActive(false);
    }

    private void UpdateScoreUI(int newScore)
    {
        scoreUI.text = $"{newScore}";
    }

    private void UpdateAmountOfBallsUI()
    {
        remainingBallsUI.text = $"{gameState.RemainingBalls}";
    }

    private void ShowStrikeUI()
    {
        // display strike text
        strikePanel.SetActive(true);
    }

    private void ShowGameOverScreen()
    {
        strikePanel.SetActive(false);

        finalScoreUI.text = scoreUI.text;

        gameOverScreen.SetActive(true);
    }
    
    private void ShowNextTurnUI()
    {
        // hide strike text
        strikePanel.SetActive(false);

        StartCoroutine(ShowNextTurnRoutine());
    }

    private IEnumerator ShowNextTurnRoutine()
    {
        Debug.Log("SHOW NEXT TURN");

        // Increases the current turn number
        gameState.CurrentTurn++;

        if (gameState.CurrentTurn <= gameState.MaxTurns)
        {
            nextTurnPanel.SetActive(true);
            nextTurnPanel.GetComponentInChildren<TMP_Text>().text = $"Turn {gameState.CurrentTurn}";

            yield return new WaitForSeconds(turnWaitTime);

            nextTurnPanel.SetActive(false);
            gameState.CurrentGameState = GameState.GameStateEnum.ResettingDeck;
        }
        else
        {
            gameState.CurrentGameState = GameState.GameStateEnum.GameEnded;
        }
    }
}