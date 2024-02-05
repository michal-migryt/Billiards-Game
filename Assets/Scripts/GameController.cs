using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using UnityEngine;

public enum GameState { START, PLAYER1TURN, PLAYER2TURN, END }
public class GameController : MonoBehaviour
{
    [SerializeField] private GameState _gameState = GameState.START;
    [SerializeField] private GameState _previousState = GameState.START;
    [SerializeField] private BallType player1BType, player2BType;
    [SerializeField] private LayerMask tableLayer;
    [SerializeField] private Vector3 waitingPoint;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private List<int> P1PocketedBalls = new List<int>(), P2PocketedBalls = new List<int>();
    [SerializeField] private List<BallController> Balls = new List<BallController>();
    [SerializeField] private List<StrikeBall> Player1Balls = new List<StrikeBall>(), Player2Balls = new List<StrikeBall>();
    //[SerializeField] private Stack<StrikeBall> strikeBallsP1 = new Stack<StrikeBall>(), strikeBallsP2 = new Stack<StrikeBall>();
    private WaitForSeconds waitForBallsStopTime = new WaitForSeconds(1f);
    private StrikeBall activePlayer1Ball, activePlayer2Ball;
    private bool coroutineIsRunning = false;
    private bool ballInPocket = false;
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
        _gameState = GameState.PLAYER1TURN;

    }
    void Start()
    {
        _uiManager.DisableMenus();
        PhysicsController.instance.SetDefaultPhysics();
        player1BType = BallType.WHITE;
        player2BType = BallType.BLACK;
    }
    public void CheckPocketedBall(BallController ballController)
    {
        ballInPocket = true;
        BallType ballType = ballController.getBallType();
        switch (ballType)
        {
            case BallType.BLACK:
                if (player1BType == BallType.BLACK)
                {
                    P1PocketedBalls.Add(ballController.getBallNumber());
                }
                else
                {
                    P2PocketedBalls.Add(ballController.getBallNumber());
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                if (!coroutineIsRunning)
                {
                    StartCoroutine(WaitForBallsToStopAndChangeTurn(GameState.PLAYER1TURN));
                }
                break;
            case BallType.WHITE:
                if (player1BType == BallType.WHITE)
                {
                    P1PocketedBalls.Add(ballController.getBallNumber());
                }
                else
                {
                    P2PocketedBalls.Add(ballController.getBallNumber());
                }
                RemoveFromBalls(ballController);
                _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
                if (!coroutineIsRunning)
                {
                    StartCoroutine(WaitForBallsToStopAndChangeTurn(GameState.PLAYER2TURN));
                }
                break;
        }
        if (P1PocketedBalls.Count == 8)
        {
            _uiManager.OnGameEnd("Player 2 wins");
            RemoveFromBalls(ballController);
            _gameState = GameState.END;
        }
        if (P2PocketedBalls.Count == 8)
        {
            _uiManager.OnGameEnd("Player 1 wins");
            RemoveFromBalls(ballController);
            _gameState = GameState.END;
        }
    }

    private IEnumerator WaitForBallsToStopAndChangeTurn(GameState newState)
    {
        coroutineIsRunning = true;
        yield return waitForBallsStopTime;

        while (AreBallsMoving())
        {
            yield return waitForBallsStopTime;
        }

        coroutineIsRunning = false;
        _gameState = newState;
        _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
        SelectBall();
    }
    private StrikeBall GetBall(List<StrikeBall> balls)
    {
        balls.RemoveAll(ball => ball == null);
        return balls[0];
    }
    private void SelectBall()
    {
        if (StrikeBall.CurrentActiveBall != null)
        {
            StrikeBall.CurrentActiveBall.DisableController();
        }
        if (_gameState == GameState.PLAYER1TURN)
        {
            activePlayer1Ball = GetBall(Player1Balls);
            activePlayer1Ball.EnabledController();
            activePlayer1Ball.EnabledStick();
            StrikeBall.SetCurrentActiveBall(activePlayer1Ball);
        }
        if (_gameState == GameState.PLAYER2TURN)
        {
            activePlayer2Ball = GetBall(Player2Balls);
            activePlayer2Ball.EnabledController();
            activePlayer2Ball.EnabledStick();
            StrikeBall.SetCurrentActiveBall(activePlayer2Ball);
        }
    }
    public void CheckChangeTurn()
    {
        if (!ballInPocket)
        {
            _gameState = _gameState == GameState.PLAYER1TURN ? GameState.PLAYER2TURN : GameState.PLAYER1TURN;
            _uiManager.UpdateUI(P1PocketedBalls, P2PocketedBalls);
            SelectBall();
            return;
        }
        ballInPocket = false;
    }
    public LayerMask WhatIsTable()
    {
        return tableLayer;
    }
    public bool AreBallsMoving()
    {
        foreach (BallController ballController in Balls)
        {
            if (ballController.isMoving())
            {
                return true;
            }
        }
        return false;
    }
    public IEnumerator WaitForBallsToStop()
    {
        yield return waitForBallsStopTime;

        while (AreBallsMoving())
        {
            yield return waitForBallsStopTime;
        }
        CheckChangeTurn();
    }
    public void RemoveFromBalls(BallController ballController)
    {
        BallType ballType = ballController.getBallType();

        List<StrikeBall> strikeBalls = ballType == player1BType ? Player1Balls : Player2Balls;
        strikeBalls.RemoveAll(ball => ball.GetBallController() == ballController);

        Balls.Remove(ballController);
    }
    public GameState GetGameState()
    {
        return _gameState;
    }

}