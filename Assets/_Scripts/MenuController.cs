using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] public TrialManager[] trials;
    
    //UI Menu Objects
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject scoreScreen;
    [SerializeField] private GameObject leaderboardScreen;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject trialSettings;
    
    //Game Settings
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private float maxSens;
    private Slider _volumeSlider;
    private Text _volumePlaceholderText;
    private InputField _volumeInputText;
    private Slider _sensSlider;
    private Text _sensPlaceholderText;
    private InputField _sensInputText;
    
    //General
    public String versionName;
    private static bool _isPaused;
    private static PlayfabManager _pfm;

    //Score screen
    private Text _inputPlaceholderText;
    private InputField _inputText;
    private Text _scoreText;
    private Text _accuracyText;
    private Text _hitsText;
    
    private void Start()
    {
        _pfm = GetComponent<PlayfabManager>();
        _scoreText = scoreScreen.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        _accuracyText = scoreScreen.transform.GetChild(0).transform.GetChild(4).GetComponent<Text>();
        _hitsText = scoreScreen.transform.GetChild(0).transform.GetChild(5).GetComponent<Text>();
        _inputPlaceholderText = scoreScreen.transform.GetChild(0).transform.GetChild(1).transform.Find("Placeholder")
            .GetComponent<Text>();
        _inputText = scoreScreen.transform.GetChild(0).transform.GetChild(1).GetComponent<InputField>();
        
        _volumeInputText = settingsMenu.transform.GetChild(4).transform.GetChild(2).GetComponent<InputField>();
        _volumePlaceholderText = settingsMenu.transform.GetChild(4).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        _volumeSlider = settingsMenu.transform.GetChild(4).transform.GetChild(1).GetComponent<Slider>();
        
        _sensInputText = settingsMenu.transform.GetChild(3).transform.GetChild(2).GetComponent<InputField>();
        _sensPlaceholderText = settingsMenu.transform.GetChild(3).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        _sensSlider = settingsMenu.transform.GetChild(3).transform.GetChild(1).GetComponent<Slider>();
        _sensSlider.maxValue = maxSens;
        
        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                if (scoreScreen.activeSelf)
                {
                    HideScore();
                }
                else if (settingsMenu.activeSelf)
                {
                    HideSettings();
                }
                else if (leaderboardScreen.activeSelf)
                {
                    HideLeaderboard();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    #region Score
    public void ShowScore(int score, int hitCount, float accuracy)
    {
        _scoreText.text = "Score: " + score;
        _accuracyText.text = "Accuracy: " + accuracy.ToString("0.00") + "%";
        _hitsText.text = "Hits: " + hitCount;
        _inputPlaceholderText.text = _pfm.playerDisplayName;
        _inputText.text = "";
        
        PauseGame();
        pauseMenu.SetActive(false);
        leaderboardScreen.SetActive(false);
        scoreScreen.SetActive(true);
    }

    public void HideScore()
    {
        ResumeGame();
        scoreScreen.SetActive(false);
    }
    
    public void UpdateDisplayName()
    {
        String inputName = _inputText.text;
        bool changeName = !inputName.Equals("") && !inputName.Equals(_pfm.playerDisplayName);
        if (changeName)
        {
            _pfm.UpdateDisplayName(inputName);
            Debug.Log("Name was updated.");
        }
        else
        {
            Debug.Log("Name was not updated.");
        }
    }
    #endregion

    #region Leaderboard
    
    private String _leaderboardName;
    private String _currentTrialName = "Time Trial";
    private int _currentDifficulty = 0;
    public void ShowLeaderboard()
    {
        RefreshLeaderboard();
        PauseGame();
        scoreScreen.SetActive(false);
        pauseMenu.SetActive(false);
        leaderboardScreen.SetActive(true);
    }
    
    public void HideLeaderboard()
    {
        ResumeGame();
        _pfm.ClearLeaderboard();
        leaderboardScreen.SetActive(false);
    }

    public void UpdateLeaderboardDifficulty(int difficulty)
    {
        _currentDifficulty = difficulty;
        RefreshLeaderboard();
    }

    public void UpdateLeaderboardTrial(String trial)
    {
        _currentTrialName = trial;
        RefreshLeaderboard();
    }

    private void RefreshLeaderboard()
    {
        string difficultyName = "Easy";
        if (_currentDifficulty == 1)
        {
            difficultyName = "Hard";
        }
        _leaderboardName = versionName + " " + _currentTrialName + " " + difficultyName;
        _pfm.ClearLeaderboard();
        _pfm.GetLeaderboard(_leaderboardName);
    }
    #endregion

    #region Settings
    public void ShowSettings()
    {
        float currentSens = playerLook.GetSens();
        _sensPlaceholderText.text = currentSens.ToString("0.00");
        _sensInputText.text = "";
        _sensSlider.value = currentSens;

        float currentVolume = AudioListener.volume;
        _volumePlaceholderText.text = currentVolume.ToString("0.0");
        _volumeInputText.text = "";
        _volumeSlider.value = AudioListener.volume;
        settingsMenu.SetActive(true);
    }

    public void HideSettings()
    {
        settingsMenu.SetActive(false);
    }

    public void ApplySettings()
    {
        //Sens
        playerLook.SetSens(_sensSlider.value);
        
        //Volume
        AudioListener.volume = _volumeSlider.value;
        
        //Gun Model
        TMP_Dropdown gunModelDropdown = settingsMenu.transform.GetChild(5).transform.GetChild(1)
            .GetComponent<TMP_Dropdown>();
        GameObject gunModelParent = GameObject.Find("Player").transform.GetChild(1).gameObject;
        for (int i = 0; i < gunModelParent.transform.childCount; i++)
        {
            gunModelParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        gunModelParent.transform.GetChild(gunModelDropdown.value).gameObject.SetActive(true);

        HideSettings();
    }

    public void OnSensSliderValueChange()
    {
        _sensInputText.text = "";
        _sensPlaceholderText.text = _sensSlider.value.ToString("0.00");
    }

    public void OnSensInputEndEdit()
    {
        float inputSens = float.Parse(_sensInputText.text);
        if (inputSens > maxSens)
        {
            inputSens = maxSens;
        } 
        else if (inputSens < 0)
        {
            inputSens = 0;
        }
        
        _sensSlider.value = inputSens;
    }
    
    
    public void OnVolumeSliderValueChange()
    {
        _volumeInputText.text = "";
        _volumePlaceholderText.text = _volumeSlider.value.ToString("0.0");
    }

    public void OnVolumeInputEndEdit()
    {
        float inputVolume = float.Parse(_volumeInputText.text);
        if (inputVolume > _volumeSlider.maxValue)
        {
            inputVolume = _volumeSlider.maxValue;
        } 
        else if (inputVolume < 0)
        {
            inputVolume = 0;
        }
        
        _volumeSlider.value = inputVolume;
    }

    #endregion

    #region Trial Settings

    private TrialManager _trialManager;
    [SerializeField] private Material targetMaterial;
    private GameObject _targetColourPanel;
    private GameObject _redPanel;
    private GameObject _greenPanel;
    private GameObject _bluePanel;
    private float _redVal;
    private float _greenVal;
    private float _blueVal;
    private Image _currentColour;
    private Image _newColour;
    public void ShowTrialSettings(TrialManager trialManager)
    {
        _trialManager = trialManager;
        _targetColourPanel = trialSettings.transform.GetChild(4).gameObject;
        _currentColour = _targetColourPanel.transform.GetChild(2).GetComponent<Image>();
        _newColour = _targetColourPanel.transform.GetChild(3).GetComponent<Image>();
        _currentColour.color = _newColour.color = targetMaterial.color;
        
        //Update Placeholder texts
        _redPanel = _targetColourPanel.transform.GetChild(1).transform.GetChild(0).gameObject;
        _redPanel.transform.GetChild(2).transform.GetChild(0).transform.Find("Placeholder").GetComponent<TextMeshProUGUI>().text 
            = (targetMaterial.color.r * 255).ToString("0");
        _redPanel.transform.GetChild(1).GetComponent<Slider>().value = targetMaterial.color.r * 255;
        
        _greenPanel = _targetColourPanel.transform.GetChild(1).transform.GetChild(1).gameObject;
        _greenPanel.transform.GetChild(2).transform.GetChild(0).transform.Find("Placeholder").GetComponent<TextMeshProUGUI>().text 
            = (targetMaterial.color.g * 255).ToString("0");
        _greenPanel.transform.GetChild(1).GetComponent<Slider>().value = targetMaterial.color.g * 255;
        
        _bluePanel = _targetColourPanel.transform.GetChild(1).transform.GetChild(2).gameObject;
        _bluePanel.transform.GetChild(2).transform.GetChild(0).transform.Find("Placeholder").GetComponent<TextMeshProUGUI>().text 
            = (targetMaterial.color.b * 255).ToString("0");
        _bluePanel.transform.GetChild(1).GetComponent<Slider>().value = targetMaterial.color.b * 255;
        PauseGame();
        pauseMenu.SetActive(false);
        
        trialSettings.transform.GetChild(0).GetComponent<Text>().text = 
            _trialManager.gameObject.transform.parent.name + " Settings";
        int currentDifficulty = _trialManager.GetDifficulty();
        TMP_Dropdown difficultyDropdown = trialSettings.transform.GetChild(3).transform.GetChild(1)
                .GetComponent<TMP_Dropdown>();
        difficultyDropdown.value = currentDifficulty;
        
        trialSettings.SetActive(true);
    }

    public void OnColourSliderChange(GameObject thisPanel)
    {
        TMP_InputField inputText = thisPanel.transform.GetChild(2).GetComponent<TMP_InputField>();
        Slider slider = thisPanel.transform.GetChild(1).GetComponent<Slider>();
        inputText.text = slider.value.ToString("0");
        UpdateColourValues(thisPanel, slider.value);
    }

    public void OnColourInputChange(GameObject thisPanel)
    {
        TMP_InputField inputText = thisPanel.transform.GetChild(2).GetComponent<TMP_InputField>();
        int inputValue = int.Parse(inputText.text);
        if (inputValue > 255)
        {
            inputValue = 255;
        } 
        else if (inputValue < 0)
        {
            inputValue = 0;
        }
        thisPanel.transform.GetChild(1).GetComponent<Slider>().value = inputValue;
        UpdateColourValues(thisPanel, inputValue);
        
    }

    private void UpdateColourValues(GameObject thisPanel, float value)
    {
        // Divide by 255 as standard hex is 0 - 255 but Color colours are normalised.
        if (thisPanel.name.Equals("Red"))
        {
            _redVal = value / 255;
        } else if (thisPanel.name.Equals("Green"))
        {
            _greenVal = value / 255;
        } else if (thisPanel.name.Equals("Blue"))
        {
            _blueVal = value / 255;
        }
        _newColour.color = new Color(_redVal, _greenVal, _blueVal, 1);
    }
    
    public void HideTrialSettings()
    {
        trialSettings.SetActive(false);
        ResumeGame();
    }

    public void ApplyTrialSettings()
    {
        TMP_Dropdown difficultyDropdown = trialSettings.transform.GetChild(3).transform.GetChild(1)
            .GetComponent<TMP_Dropdown>();
        int menuDifficulty = difficultyDropdown.value;
        _trialManager.SetDifficulty(menuDifficulty);
        HideTrialSettings();

        //If colour is not the same, change it!
        if (!(targetMaterial.color.r == _redVal && targetMaterial.color.g == _greenVal &&
              targetMaterial.color.b == _blueVal))
        {
            targetMaterial.color = _newColour.color;
        }
        else
        {
            Debug.Log("Colour not changed.");
        }
    }

    #endregion
    public bool GetIsPaused()
    {
        return _isPaused;
    }
}
