using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public Transform playerLeaf;
    public InputActionReference inputReload;
    public InputActionReference inputMenu;
    public InputActionReference inputSkill;

    float time;
    Vector3 oldPosition;
    float velocity;

    [Header("Time Bar")]
    public float targetTimeSecond = 60;
    public Image uiTimeBar;


    [Header("Height bar")]
    public RectTransform rulerPointer;
    public float startY = 0;
    public float endY =-100;

    [Header("Gas Bar")]
    public Image uiGasBar;
    public float gasBarMax = 10;
    public float gasBarValue = 10;
    public float gasBarRegenerate = 1;
    bool usingGas;

    [Header("Game End")]
    public GameObject moveHint;
    public Text moveHintCDText;
    public float DeadVelo = 1;
    public float cdWhenVeloLowerThanDeadVelo = 3;
    public float cd = 3;
    static bool gameOver = false;
    public static GameObject gameOverPanel;
    public static GameObject gameWinPanel;
    public Text gameWinText;

    [Header("Skill")]
    public GameObject skillArea;
    public Text skillRemainDisplay;
    public int skillRemain = 2;
    public float skillduration = 1;
    float skillUseTimeCD;

    void Start()
    {
        gameOverPanel = GameObject.Find("UI").transform.Find("GameOver").gameObject;
        gameWinPanel = GameObject.Find("UI").transform.Find("GameWin").gameObject;
        gameOver = false;
        skillRemainDisplay.text = skillRemain.ToString();
    }

    void Update()
    {
        time += Time.deltaTime;

        velocity = Vector3.Distance(playerLeaf.position, oldPosition) / Time.deltaTime;
        oldPosition = playerLeaf.position;

        //display the time bar
        uiTimeBar.fillAmount = (targetTimeSecond - time) / targetTimeSecond;

        //display player`s leaf Height
        rulerPointer.anchoredPosition = new Vector3(rulerPointer.anchoredPosition.x, Mathf.LerpUnclamped(1030, 50, (playerLeaf.position.y - startY) / (endY - startY)));

        //display gas bar
        uiGasBar.fillAmount = gasBarValue / gasBarMax;
        //regenerate gas
        if (usingGas)
            gasBarValue -= Time.deltaTime;
        else if (gasBarValue < gasBarMax)
            gasBarValue += gasBarRegenerate * Time.deltaTime;
        if (gasBarValue > gasBarMax)
            gasBarValue = gasBarMax;
        usingGas = false;

        if (velocity < DeadVelo)
            cd -= Time.deltaTime;
        else
            cd = cdWhenVeloLowerThanDeadVelo;
        if (cd < 3 && cd > 0)
        {
            if (!moveHint.activeSelf)
                moveHint.SetActive(true);
            moveHintCDText.text = cd.ToString("0");
        } else if (moveHint.activeSelf)
            moveHint.SetActive(false);

        if (cd < 0)
        {
            GameOver();
        }

        var inputReloadValue = inputReload.action.ReadValue<float>();
        if (inputReloadValue == 1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        var inputMenuValue = inputMenu.action.ReadValue<float>();
        if (inputMenuValue == 1)
            SceneManager.LoadScene(0);

        var inputSkillValue = inputSkill.action.ReadValue<float>();
        if (inputSkillValue == 1 && skillRemain > 0 && skillUseTimeCD <= 0)
        {
            skillUseTimeCD = skillduration;
            skillArea.SetActive(true);
            skillRemain -= 1;
            skillRemainDisplay.text = skillRemain.ToString();
        }

        if (skillUseTimeCD > 0)
            skillUseTimeCD -= Time.deltaTime;

        if (skillUseTimeCD <= 0 && skillArea.activeSelf)
            skillArea.SetActive(false);



        if (!gameOver && time > targetTimeSecond)
        {
            gameWinPanel.SetActive(true);
            gameWinText.text = "Your leaf's height\n" + (playerLeaf.position.y - endY);
            gameOver = true;
        }
    }

    public bool UseGas(float amount)
    {
        if(gasBarValue >= amount)
        {
            usingGas = true;
            return true;
        } 
        else
        {
            return false; 
        }
    }

    public static void GameOver()
    {
        if (!gameOverPanel.activeSelf)
            gameOverPanel.SetActive(true);

        gameOver = true;
    }

}
