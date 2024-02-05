using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class StrikeBall : MonoBehaviour
{
    public GameObject stick;
    [SerializeField] private Slider slider;
    private LineRenderer lineRenderer;
    private Rigidbody rb;
    private float currentYaw = 0f;
    private Vector3 shotForce = Vector3.forward * 2;
    private float shotAngle, shotPower = 1;
    private bool isTakingShot = false;
    private Camera mainCam;
    private GameController _gameController;
    private UIManager _uiManager;
    private Coroutine handleShotPowerCoroutine = null;
    private WaitForSeconds shotPoweringUpTime = new WaitForSeconds(0.03f);
    private static StrikeBall currentActiveBall;
    private static bool firstMove = true;
    private BallController ballController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _uiManager = FindObjectOfType<UIManager>();
        mainCam = Camera.main;
        _gameController = GameController.instance;
        lineRenderer = FindObjectOfType<LineRenderer>();
        ballController = GetComponent<BallController>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Ray ray = mainCam.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _gameController.WhatIsTable()))
                {
                    currentYaw = CalculateDegree(transform.position, hit.point);
                }
            }
        }
        if (slider.value > 1 && handleShotPowerCoroutine == null && isTakingShot == false)
        {
            handleShotPowerCoroutine = StartCoroutine(HandleShotPower());
        }
        if (slider.value == 1 && handleShotPowerCoroutine != null)
        {
            StopCoroutine(handleShotPowerCoroutine);
            handleShotPowerCoroutine = null;
            shotPower = 1;
            _uiManager.DisableShotSlider();
            isTakingShot = false;
        }

        ManageRotation();
        ManageLine();
    }

    private void ManageRotation()
    {
        float distanceFromBall = 0.1f + (0.2f * shotPower / 90);
        if (!isTakingShot)
        {
            stick.transform.position = transform.position - new Vector3(0, 0, distanceFromBall);
            stick.transform.RotateAround(transform.position, Vector3.up, currentYaw);
            stick.transform.LookAt(transform.position);
        }

        shotAngle = stick.transform.eulerAngles.y;
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);
    }

    private void ManageLine()
    {
        lineRenderer.enabled = true;
        RaycastHit raycastHit;
        Physics.Raycast(transform.position, shotForce, out raycastHit);
        Vector3 startPosition = transform.position,
        targetPosition = raycastHit.point;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, targetPosition);
    }

    private IEnumerator HandleShotPower()
    {
        isTakingShot = true;
        _uiManager.EnableShotSlider(transform.position);
        yield return shotPoweringUpTime;

        while (true)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
                break;

            shotPower = slider.value;
            _uiManager.UpdateShotSlider(shotPower);
            float distanceFromBall = 0.1f + (0.2f * shotPower / 90);

            stick.transform.position = transform.position - new Vector3(0, 0, distanceFromBall);
            stick.transform.RotateAround(transform.position, Vector3.up, currentYaw);
            stick.transform.LookAt(transform.position);
            yield return shotPoweringUpTime;
        }
        stick.SetActive(false);
        _uiManager.DisableShotSlider();
        lineRenderer.enabled = false;
        Shoot();
        firstMove = false;
        isTakingShot = false;
        shotPower = 1;
        slider.value = 1;
        DisableController();
    }

    private void Shoot()
    {
        shotForce = Quaternion.Euler(0, shotAngle, 0) * new Vector3(0, 0, shotPower / 10 * 0.9f);
        rb.AddForce(shotForce, ForceMode.Impulse);
        GameController.instance.StartCoroutine(GameController.instance.WaitForBallsToStop());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + shotForce);
    }

    private float CalculateDegree(Vector3 from, Vector3 to)
    {
        Vector3 vectorBetween = to - from;
        float degree = Mathf.Atan2(vectorBetween.x, vectorBetween.z) * Mathf.Rad2Deg;

        return (degree + 360f) % 360f;
    }

    public static void SetCurrentActiveBall(StrikeBall ball)
    {
        currentActiveBall = ball;
    }

    public static void SetFirstMove(bool value)
    {
        firstMove = value;
    }

    public BallController GetBallController()
    {
        return ballController;
    }

    public static StrikeBall CurrentActiveBall
    {
        get { return currentActiveBall; }
    }

    public static bool FirstMove
    {
        get { return firstMove; }
    }

    public void EnabledController()
    {
        enabled = true;
    }

    public void DisableController()
    {
        enabled = false;
    }

    public void EnabledStick()
    {
        stick.SetActive(true);
    }
}
