using System;
using System.Collections;
using UnityEngine;
using Vuforia;

public class PinDeckController : MonoBehaviour
{
    [SerializeField] private Transform arCamera;
    [SerializeField] private GameObject bowlingLanePrefab;
    [SerializeField] private GameObject pinDeckPrefab;
    [SerializeField] private PlaneFinderBehaviour planeFinder;
    [SerializeField] private GameState gameState;
    
    private GameObject _pinDeckClone;
    private Transform _pinDeckSpawnPoint;
    private bool _pinDeckCreated;
    private Pin[] _pins;
    
    private void OnEnable()
    {
        gameState.onBallPlayEnd.AddListener(StartBallPlayEnded);
        gameState.onResettingDeck.AddListener(StartPlaceNewDeckOnLane);
    }

    private void OnDisable()
    {
        gameState.onBallPlayEnd.RemoveListener(StartBallPlayEnded);
        gameState.onResettingDeck.RemoveListener(StartPlaceNewDeckOnLane);
    }

    private void Awake()
    {
        // set ar camera position for editor testing
#if UNITY_EDITOR
        var arCameraTransform = arCamera.transform;
        arCameraTransform.position = new Vector3(0, 1.4f, 4);
        
        var eulerAngles = arCameraTransform.eulerAngles;
        arCameraTransform.eulerAngles = new Vector3(
            eulerAngles.x,
            eulerAngles.y + 180, 
            eulerAngles.z
        );
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (_pinDeckCreated) return;
        if (!Input.GetMouseButtonDown(0)) return;
        
        _pinDeckCreated = true;
        CreatePinDeck();
        Debug.Log("Mouse Left Button Clicked");
#endif
    }
    
    public void CreatePinDeck()
    {
        StartCoroutine(SetupBowlingLaneRoutine());
    }

    private IEnumerator SetupBowlingLaneRoutine()
    {
        // Get plane indicator's transform
        var defaultPlaneIndicator = planeFinder.PlaneIndicator.transform;

        // Gets camera direction for rotation
        // Invert direction for editor testing 
#if UNITY_EDITOR
        var directionTowardsCamera = defaultPlaneIndicator.position - arCamera.position;
#else
        var directionTowardsCamera = defaultPlaneIndicator.position + arCamera.position;
#endif

        // Reset the Y position to keep the lane straight
        directionTowardsCamera.y = 0;

        // Store rotation of the track to face ARCamera
        var lookRotation = Quaternion.LookRotation(-directionTowardsCamera, Vector3.up);

        // Create a new pin deck base
        var bowlingLaneClone = Instantiate(bowlingLanePrefab, defaultPlaneIndicator.position, lookRotation);

        // Get position and rotation for new pin deck from the spawn point
        _pinDeckSpawnPoint = bowlingLaneClone.transform.Find("PinDeckSpawnPoint");

        // Creates a new pin deck
        _pinDeckClone = Instantiate(pinDeckPrefab, _pinDeckSpawnPoint.position, _pinDeckSpawnPoint.rotation);

        yield return new WaitForSeconds(1);
        
        gameState.CurrentGameState = GameState.GameStateEnum.SetupBalls;
        
        _pins = _pinDeckClone.transform.GetComponentsInChildren<Pin>();

        LowerPinDeck();

        yield return new WaitForSeconds(1);
    }
    
    private void LowerPinDeck()
    {
        _pinDeckClone.GetComponent<AudioSource>().Play();
        
        foreach (var pin in _pins)
        {
            pin.StartLowerPin();
        }
    }
    
    private void RaisePinDeck()
    {
        foreach (var pin in _pins)
        {
            if (!pin.IsPinDown())
            {
                pin.StartRaisePin();
            }
        }
        
        _pinDeckClone.GetComponent<AudioSource>().Play();
    }
    
    private void StartBallPlayEnded()
    {
        Debug.Log("BallPlayEnded()");

        StartCoroutine(BallPlayEnded());
    }

    private IEnumerator BallPlayEnded()
    {
        foreach (var pin in _pins)
        {
            if (pin.IsPinDown())
            {
                gameState.Score++;
                gameState.StrikeCounter++;
            }
        }

        if (gameState.StrikeCounter == 10)
        {
            gameState.CurrentGameState = GameState.GameStateEnum.StrikeAchieved;
            gameState.Score += gameState.StrikeExtraPoints;
            yield return new WaitForSeconds(2);
        }

        gameState.StrikeCounter = 0;
        
        RaisePinDeck();
        
        yield return new WaitForSeconds(2);

        gameState.CurrentGameState = GameState.GameStateEnum.TurnEnd;
    }
    
    private void StartPlaceNewDeckOnLane()
    {
        Debug.Log("PLACE NEW DECK ON LANE");

        StartCoroutine(PlaceNewDeckOnLane());
    }

    private IEnumerator PlaceNewDeckOnLane()
    {
        foreach (var pin in _pins)
        {
            pin.Reset();
            pin.StartLowerPin();
        }
        
        yield return new WaitForSeconds(2);

        gameState.CurrentGameState = GameState.GameStateEnum.ReadyToThrow;
    }
}
