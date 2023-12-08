using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/CreateGameStateAsset")]
public class GameState : ScriptableObject
{
    public enum GameStateEnum
    {
        TitleScreen,
        PlacingPinDeckAndLane,
        SetupBalls,
        ReadyToThrow,
        BallInPlay,
        BallPlayEnd,
        StrikeAchieved,
        TurnEnd,
        ResettingDeck,
        GameEnded
    }

    [SerializeField] private GameStateEnum currentGameState;
    [SerializeField] private int score = 0;
    [SerializeField] private int remainingBalls = 0;
    [SerializeField] private int currentTurn = 0;
    [SerializeField] private int maxTurns = 5;
    [SerializeField] private int strikeCounter = 0;
    [SerializeField] private int strikeExtraPoints = 10;
    [SerializeField] private float throwPowerMultiplier = 0.05f;
    
    [HideInInspector] public UnityEvent<int> onScoreChanged;
    [HideInInspector] public UnityEvent onEnterBallSetup;
    [HideInInspector] public UnityEvent onReadyToThrow;
    [HideInInspector] public UnityEvent onBallInPlay;
    [HideInInspector] public UnityEvent onBallPlayEnd;
    [HideInInspector] public UnityEvent onTurnEnded;
    [HideInInspector] public UnityEvent onStrikeAchieved;
    [HideInInspector] public UnityEvent onResettingDeck;
    [HideInInspector] public UnityEvent onGameEnded;

    public GameStateEnum CurrentGameState
    {
        get => currentGameState;
        set
        {
            currentGameState = value;
            
            switch (value)
            {
                case GameStateEnum.SetupBalls:
                    onEnterBallSetup?.Invoke();
                    break;
                case GameStateEnum.ReadyToThrow:
                    onReadyToThrow?.Invoke();
                    break;
                case GameStateEnum.BallInPlay:
                    onBallInPlay?.Invoke();
                    break;
                case GameStateEnum.BallPlayEnd:
                    onBallPlayEnd?.Invoke();
                    break;
                case GameStateEnum.TurnEnd:
                    onTurnEnded?.Invoke();
                    break;
                case GameStateEnum.StrikeAchieved:
                    onStrikeAchieved?.Invoke();
                    break;
                case GameStateEnum.ResettingDeck:
                    onResettingDeck?.Invoke();
                    break;
                case GameStateEnum.GameEnded:
                    onGameEnded?.Invoke();
                    break;
            }
        }
    }

    public int Score
    {
        get => score;
        set
        {
            score = value;
            onScoreChanged?.Invoke(score);
        }
    }

    public int RemainingBalls
    {
        get => remainingBalls;
        set => remainingBalls = value;
    }

    public int CurrentTurn
    {
        get => currentTurn;
        set => currentTurn = value;
    }

    public int StrikeCounter
    {
        get => strikeCounter;
        set => strikeCounter = value;
    }

    public int MaxTurns
    {
        get => maxTurns;
        set => maxTurns = value;
    }

    public int StrikeExtraPoints
    {
        get => strikeExtraPoints;
        set => strikeExtraPoints = value;
    }

    public float ThrowPowerMultiplier
    {
        get => throwPowerMultiplier;
        set => throwPowerMultiplier = value;
    }
    
    public void ResetState()
    {
        currentTurn = 1;
        score = 0;
        remainingBalls = MaxTurns;
    }
    
    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}