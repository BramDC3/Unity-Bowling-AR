using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform arCamera;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameState gameState;

    private GameObject _currentBall;
    private Vector2 _touchInitialPosition;
    private Vector2 _touchFinalPosition;
    private float _ySwipeDelta;
    
    private void Awake()
    {
        // listen to Enter Ball Setup Event
        gameState.onEnterBallSetup.AddListener(EnableSelf);
        
        // Resets game state to the needed values for a new game
        gameState.ResetState();

        gameState.CurrentGameState = GameState.GameStateEnum.PlacingPinDeckAndLane;
    }
    
    private void EnableSelf()
    {
        enabled = true;
    }
    
    private void OnEnable()
    {
        BallInitialSetup();
    }
    
    private void OnDisable()
    {
        gameState.onEnterBallSetup.RemoveListener(EnableSelf);
    }

    private void Update()
    {
        switch (gameState.CurrentGameState)
        {
            case GameState.GameStateEnum.ReadyToThrow:
                // track touch to throw, device only
                DetectScreenSwipe();

#if UNITY_EDITOR
                // desktop editor only, track mouse button to throw
                if (Input.GetMouseButtonDown(1))
                {
                    Debug.Log("Mouse Right Button Clicked");
                    ThrowBall();
                }
#endif
                break;
            
            case GameState.GameStateEnum.BallInPlay:
                // if the ball falls below -20, you have ended the play
                if (_currentBall.transform.position.y < -20)
                {
                    Debug.Log("PLAYER PLAY END!");

                    // reset ball
                    _currentBall.transform.position = new Vector3(0, 1000, 0);
                    _currentBall.transform.rotation = Quaternion.identity;

                    // reset rigidbody force
                    var rb = _currentBall.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = false;

                    // ball has been thrown, set ball play end state
                    gameState.CurrentGameState = GameState.GameStateEnum.BallPlayEnd;
                }
                break;
        }
    }

    private void BallInitialSetup()
    {
        // instantiate ball
        _currentBall = Instantiate(ballPrefab, new Vector3(0, 1000, 0), Quaternion.identity);
        
        // switch to ReadyToThrow state
        gameState.CurrentGameState = GameState.GameStateEnum.ReadyToThrow;
    }

    private void DetectScreenSwipe()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _touchInitialPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _touchFinalPosition = touch.position;

                if (_touchFinalPosition.y > _touchInitialPosition.y)
                {
                    _ySwipeDelta = _touchFinalPosition.y - _touchInitialPosition.y;
                }

                ThrowBall();
            }
        }
    }

    // Gets a ball and sets position, rotation and adds force to it
    private void ThrowBall()
    {
        // enable gravity
        _currentBall.GetComponent<Rigidbody>().useGravity = true;

        // store force multiplier
        var throwPowerMultiplier = 0.05f;

        // store ar camera rotation
        var lookRotation = arCamera.rotation;

#if UNITY_EDITOR
        // store camera and mouse position and convert to a world direction
        var cam = arCamera.GetComponent<Camera>();
        var mousePos = Input.mousePosition;
        var mouseDir = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.farClipPlane));

        // store rotation direction
        lookRotation = Quaternion.LookRotation(mouseDir, Vector3.up);

        // override swipe and power for editor only
        _ySwipeDelta = 1.5f;
        throwPowerMultiplier = 60.00f;
#endif

        // set start ball position facing ar camera
        _currentBall.transform.position = arCamera.position;
        _currentBall.transform.rotation = lookRotation;

        // calculate force and apply to the ball's rigidbody
        var forceVector = _currentBall.transform.forward * (_ySwipeDelta * throwPowerMultiplier);
        _currentBall.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);

        // update balls remaining
        gameState.RemainingBalls--;

        // set ball in play state
        gameState.CurrentGameState = GameState.GameStateEnum.BallInPlay;
    }
}