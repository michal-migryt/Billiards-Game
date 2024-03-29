using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { START, PLAYER1TURN, PLAYER2TURN, END }
public class GameController : MonoBehaviour
{
    [SerializeField] private GameState _gameState = GameState.START;
    //private GameState previousState = GameState.START;
    [SerializeField] private bool foul;
    [SerializeField] private BallType player1BType = BallType.NULL, player2BType = BallType.NULL;
    [SerializeField] private PocketType player1ChosenPocket, player2ChosenPocket;
    [SerializeField] private LayerMask tableLayer;
    [SerializeField] private Vector3 tableCenter, waitingPoint;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private List<int> P1PocketedBalls = new List<int>(), P2PocketedBalls = new List<int>();
    [SerializeField] private List<BallController> Balls = new List<BallController>();
    private bool didPocketOwnBall = false;
    public static GameController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        foul = false;
        _gameState = GameState.PLAYER1TURN;

    }
    void Start()
    {
        _uiManager.DisableMenus();
        PhysicsController.instance.SetDefaultPhysics();
        
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_uiManager != null)
            {
                _uiManager.ShowGameMenu();
            }
        }
        //if(Application.targetFrameRate != 60) {Application.targetFrameRate = 60;}
        //AreBallsMoving();
    }
    
    public void CheckPocketedBall(BallController ballController, PocketType pocketType)
    {
        BallType ballType = ballController.getBallType();
        BallType otherBallType = ballType == BallType.HALF ? BallType.FULL : BallType.HALF;
        if (player1BType == BallType.NULL)
        {
            if (ballType != BallType.WHITE && ballType != BallType.BLACK)
            {
                //player1BType = _gameState == GameState.PLAYER1TURN ? ballType : otherBallType;
                if (_gameState == GameState.PLAYER1TURN)
                {
                    player1BType = ballType;
                    player2BType = otherBallType;
                    P1PocketedBalls.Add(ballController.getBallNumber());
                    didPocketOwnBall = true;
                }
                else
                {
                    player2BType = ballType;
                    player1BType = otherBallType;
                    P2PocketedBalls.Add(ballController.getBallNumber());
                    didPocketOwnBall = true;
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                return;
            }
            if (ballType == BallType.WHITE)
            {
                foul = true;
                //_gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
                ballController.TakeToWaitingPoint();
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                return;
            }
            if (ballType == BallType.BLACK)
            {
                string winner = "";
                if (_gameState == GameState.PLAYER1TURN)
                {
                        winner = "Player 2 wins!";
                        P1PocketedBalls.Add(ballController.getBallNumber());
                }
                else
                {
                    winner = "Player 1 wins!";
                    P2PocketedBalls.Add(ballController.getBallNumber());
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                _gameState = GameState.END;
                _uiManager.OnGameEnd(winner);
            }

        }
        else
        {
            switch (ballType)
            {
                case BallType.HALF:
                    if (player1BType == BallType.HALF)
                    {
                        P1PocketedBalls.Add(ballController.getBallNumber());
                        if (!didPocketOwnBall)
                            didPocketOwnBall = _gameState == GameState.PLAYER1TURN ? true : false;
                    }
                    else
                    {
                        P2PocketedBalls.Add(ballController.getBallNumber());
                        if (!didPocketOwnBall)
                            didPocketOwnBall = _gameState == GameState.PLAYER2TURN ? true : false;
                    }
                    RemoveFromBalls(ballController);
                    _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                    break;
                case BallType.FULL:
                    if (player1BType == BallType.FULL)
                    {
                        P1PocketedBalls.Add(ballController.getBallNumber());
                        if (!didPocketOwnBall)
                            didPocketOwnBall = _gameState == GameState.PLAYER1TURN ? true : false;
                    }
                    else
                    {
                        P2PocketedBalls.Add(ballController.getBallNumber());
                        if (!didPocketOwnBall)
                            didPocketOwnBall = _gameState == GameState.PLAYER2TURN ? true : false;
                    }
                    RemoveFromBalls(ballController);
                    _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                    break;
                case BallType.WHITE:
                    foul = true;
                    ballController.TakeToWaitingPoint();
                    break;
                case BallType.BLACK:
                    string winner = "";
                    if (_gameState == GameState.PLAYER1TURN)
                    {
                        winner = P1PocketedBalls.Count == 7 && !foul ? "Player 1 wins" : "Player 2 wins";
                        P1PocketedBalls.Add(ballController.getBallNumber());
                    }
                    if (_gameState == GameState.PLAYER2TURN)
                    {
                        winner = P2PocketedBalls.Count == 7 && !foul ? "Player 2 wins" : "Player 1 wins";
                        P2PocketedBalls.Add(ballController.getBallNumber());
                    }
                    RemoveFromBalls(ballController);
                    _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                    _gameState = GameState.END;
                    _uiManager.OnGameEnd(winner);
                    break;
            }
        }
    }
    
    public void OnWhiteBallFirstHit(WhiteBallController whiteBallController, BallType otherBallType)
    {
        if (player1BType != BallType.NULL)
        {
            if (otherBallType != BallType.BLACK)
            {
                if (_gameState == GameState.PLAYER1TURN && player1BType != otherBallType)
                {
                    foul = true;
                    return;
                }
                if (_gameState == GameState.PLAYER2TURN && player2BType != otherBallType)
                {
                    foul = true;
                    return;
                }
            }
            else
            {
                if(_gameState == GameState.PLAYER1TURN && P1PocketedBalls.Count != 7)
                {
                    foul = true;
                    return;
                }
                if(_gameState == GameState.PLAYER2TURN && P2PocketedBalls.Count != 7)
                {
                    foul = true;
                    return;
                }
            }
        }
    }
    public void Foul()
    {
        Debug.Log("Foul");
        foul = true;
    }
    public Vector3 GetWaitingPoint()
    {
        return waitingPoint;
    }
    //TODO Finish CheckChangeTurn
    public void CheckChangeTurn()
    {

        if (!didPocketOwnBall)
        {
            _gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
            _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
            return;
        }
        if (foul)
        {
            _gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
            _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
            didPocketOwnBall = false;
            return;
        }
        didPocketOwnBall = false;
    }
    public LayerMask WhatIsTable()
    {
        return tableLayer;
    }
    public void EndFoul()
    {
        foul = false;
    }
    public bool IsFoul()
    {
        return foul;
    }
    public bool AreBallsMoving()
    {
        bool temp = false;
        foreach (BallController ballController in Balls)
        {
            if (ballController.isMoving())
            {
                temp = true;
                break;
            }
        }
        //Debug.Log(temp);
        return temp;
    }
    /*
    public IEnumerator AreBallsMovingEnumerator()
    {
        bool areMoving = true;
        yield return new WaitForSeconds(0.1f);
        while(areMoving)
        {
        foreach (BallController ballController in Balls)
        {
            if(ballController.isMoving())
            {
                areMoving = true;
                break;
            }
            else
            {
                areMoving = false;
            }
        }
        yield return new WaitForSeconds(0.1f);
        Debug.Log(areMoving);
        }
        yield return areMoving;
    }
    */
    public void RemoveFromBalls(BallController ballController)
    {

        Balls.Remove(ballController);
        Debug.Log("Removed ball nr " + ballController.getBallNumber());

    }
    public GameState GetGameState()
    {
        return _gameState;
    }
}
