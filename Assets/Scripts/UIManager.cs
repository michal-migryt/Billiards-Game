using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject settings, powerSliderGO, endMessage;
    [SerializeField] private Slider massSlider, dragSlider, angDragSlider, shotPowerSlider;
    [SerializeField] private TextMeshProUGUI massText, dragText, angDragText, endMessageText;
    [SerializeField] private Sprite[] Ball2DSprites = new Sprite[15];
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject[] player1Balls, player2Balls;
    //[SerializeField] private Image[] player1Balls, player2Balls;
    [SerializeField] private Image shotPowerSliderFill;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Camera mainCam, uiCam;
    [SerializeField] private Color fillColor;
    private void Start()
    {
        SetupUI();
        SetUpSliders();
        UpdateSliders();
        mainCam = Camera.main;
    }
    public void ShowGameMenu()
    {
        if (!gameMenu.activeSelf && !settings.activeSelf)
        {
            gameMenu.SetActive(!gameMenu.activeSelf);
            Time.timeScale = 0;
        }
        else
        {
            DisableMenus();
        }
    }
    public void DisableMenus()
    {
        gameMenu.SetActive(false);
        settings.SetActive(false);
        Time.timeScale = 1;
    }
    public void SetUpSliders()
    {
        massSlider.value = PhysicsController.instance.getDefaultBallMass();
        massSlider.onValueChanged.AddListener((v) => {
            PhysicsController.instance.setTempMass(v);
            if (v.ToString().Length > 4)
                massText.text = v.ToString().Substring(0, 4);
            else
                massText.text = v.ToString();
        });
        dragSlider.value = PhysicsController.instance.getDefaultDrag();
        dragSlider.onValueChanged.AddListener((v) => {
            PhysicsController.instance.setTempDrag(v);
            if (v.ToString().Length > 4)
                dragText.text = v.ToString().Substring(0, 4);
            else
                dragText.text = v.ToString();
        });
        angDragSlider.value = PhysicsController.instance.getDefaultAngularDrag();
        angDragSlider.onValueChanged.AddListener((v) => {
            PhysicsController.instance.setTempAngularDrag(v);
            if (v.ToString().Length > 4)
                angDragText.text = v.ToString().Substring(0, 4);
            else
                angDragText.text = v.ToString();
        });
    }

    public void UpdateSliders()
    {
        massSlider.value = PhysicsController.instance.getBallMass();
        string temp = PhysicsController.instance.getBallMass().ToString();
        if (temp.Length > 4)
            temp = temp.Substring(0, 4);
        massText.text = temp;
        dragSlider.value = PhysicsController.instance.getDrag();
        temp = PhysicsController.instance.getDrag().ToString();
        if (temp.Length > 4)
            temp = temp.Substring(0, 4);
        dragText.text = temp;
        angDragSlider.value = PhysicsController.instance.getAngularDrag();
        temp = PhysicsController.instance.getAngularDrag().ToString();
        if (temp.Length > 4)
            temp = temp.Substring(0, 4);
        angDragText.text = temp;
    }
    public void ApplyPhysicsButton()
    {
        PhysicsController.instance.ApplyPhysicsChanges();
        PhysicsController.physicsDelegate.Invoke();
    }
    public void onDefaultPhysicsButton()
    {
        PhysicsController.instance.SetDefaultPhysics();
        UpdateSliders();
        PhysicsController.physicsDelegate.Invoke();
    }
    public void onMenuCloseButton(string menuName)
    {
        string temp = string.Concat(menuName.Where(c => !char.IsWhiteSpace(c)));
        temp = temp.ToLower();
        if (temp.Contains("main"))
        {
            DisableMenus();
        }
        if (temp.Contains("settings"))
        {
            settings.SetActive(false);
            gameMenu.SetActive(true);
        }
    }
    public void onResumeButtonClick()
    {
        DisableMenus();
    }
    public void onSettingsButtonClick()
    {
        gameMenu.SetActive(false);
        UpdateSliders();
        settings.SetActive(true);
    }
    public void onMainMenuButtonClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main menu");
    }
    public void onExitButtonClick()
    {
        //exit game
        Application.Quit();
    }
    #region Shot Power Slider
    // Enables Shot Slider above given position
    public void EnableShotSlider(Vector3 pos)
    {
        var screen = mainCam.WorldToScreenPoint(pos + new Vector3(0.1f, 0, 0));
        screen.z = (canvas.transform.position - uiCam.transform.position).magnitude;
        var position = uiCam.ScreenToWorldPoint(screen);
        shotPowerSlider.transform.position = position;
        powerSliderGO.SetActive(true);
        shotPowerSlider.enabled = true;
    }
    public void DisableShotSlider()
    {
        powerSliderGO.SetActive(false);
        shotPowerSlider.enabled = false;
        shotPowerSlider.value = 1;
    }
    public void UpdateShotSlider(float shotPower)
    {
        shotPowerSlider.value = shotPower;
        float red, green, blue;
        red = 5.1f * shotPower;
        green = 255 - 5.1f * (shotPower - 50);
        blue = 0;
        red = Mathf.Clamp(red, 0, 255);
        green = Mathf.Clamp(green, 0, 255);
        fillColor = new Color32((byte)red, (byte)green, (byte)blue, 255);
        shotPowerSliderFill.color = fillColor;
    }
    #endregion
    public void SetupUI()
    {
        for (int i = 0; i < player1Balls.Count(); i++)
        {
            Image ballImage = player1Balls[i].GetComponent<Image>();
            ballImage.enabled = false;
        }
        for (int i = 0; i < player2Balls.Count(); i++)
        {
            Image ballImage = player2Balls[i].GetComponent<Image>();
            ballImage.enabled = false;
        }
        turnText.text = GameController.instance.GetGameState() == GameState.PLAYER1TURN ? "P1 TURN" : "P2 TURN";
    }
    public void UpdateUI(List<int> PocketedBallsP1, List<int> PocketedBallsP2)
    {
        turnText.text = GameController.instance.GetGameState() == GameState.PLAYER1TURN ? "P1 TURN" : "P2 TURN";
        for (int i = 0; i < PocketedBallsP1.Count; i++)
        {
            Image ballImage = player1Balls[i].GetComponent<Image>();
            ballImage.sprite = Ball2DSprites[PocketedBallsP1[i]];
            ballImage.enabled = true;
        }
        for (int i = 0; i < PocketedBallsP2.Count; i++)
        {
            Image ballImage = player2Balls[i].GetComponent<Image>();
            ballImage.sprite = Ball2DSprites[PocketedBallsP2[i]];
            ballImage.enabled = true;

        }
    }
    public void OnGameEnd(string message)
    {
        endMessageText.text = message;
        endMessage.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnPlayAgainButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}