using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TimeTrialManager : TrialManager
{
    [SerializeField] private MenuController menuController;
    [SerializeField] private PlayfabManager playfabManager;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private bool trialActive;
    [SerializeField] private int difficulty; //0 == Normal, 1 == Hard.
    [SerializeField] private int targetsToSpawn = 3;
    [SerializeField] private GameObject spawnPointParent;
    [SerializeField] private float timeLimit;
    [SerializeField] private float trialCountdownTime;
    [SerializeField] private GameObject startButtonTextObject;
    [SerializeField] private GameObject trialUiParent;
    [SerializeField] private GameObject targetParent;
    private Text _startButtonText;
    private Text _timerText;
    private Text _hitsText;
    private Text _countdownText;
    private Text _accuracyText;
    private Text _scoreText;
    private int _targetCount;
    private int _hitCount;
    private int _shotCount;
    private float _accuracy;
    private int _score;
    private float _trialStartTime;
    private float _countdownStartTime;
    private bool _countdownActive;
    private String currentLeaderboardName;
    private String trialName;

    private void Awake()
    {
        _startButtonText = startButtonTextObject.GetComponent<Text>();
        
        _timerText = trialUiParent.transform.Find("Timer").GetComponent<Text>();
        _hitsText = trialUiParent.transform.Find("Hits").GetComponent<Text>();
        _countdownText = trialUiParent.transform.Find("Countdown").GetComponent<Text>();
        _accuracyText = trialUiParent.transform.Find("Accuracy").GetComponent<Text>();
        _scoreText = trialUiParent.transform.Find("Score").GetComponent<Text>();
        _countdownActive = false;
        trialName = transform.parent.name;
    }

    private void Update()
    {
        String difficultyName = "Easy";
        if (difficulty == 1)
        {
            difficultyName = "Hard";
        }
        currentLeaderboardName = menuController.versionName + " " + trialName + " " + difficultyName;
        
        bool isPaused = menuController.GetIsPaused();
        if (!isPaused)
        {
            if (_countdownActive)
            {
                trialActive = false;
                _timerText.text = "Time Remaining: " + timeLimit + "s";
                _hitsText.text = "Hits: 0";
                _accuracyText.text = "Accuracy: 100%";
                _scoreText.text = "Score: 0";
                float countdownTimeElapsed = (Time.time - _countdownStartTime);
                float countdownTimeRemaining = trialCountdownTime - countdownTimeElapsed;
                bool countdownElapsed = countdownTimeElapsed >= trialCountdownTime;
                if (!countdownElapsed)
                {
                    _countdownText.text = countdownTimeRemaining.ToString("0");
                }
                else
                {
                    _countdownText.gameObject.SetActive(false);
                    trialActive = true;
                    _trialStartTime = Time.time;
                    _countdownActive = false;
                }
            }

            float currentTimeElapsed = Time.time - _trialStartTime;
            float timeRemaining = timeLimit - currentTimeElapsed;
            bool timeTrialFinished = currentTimeElapsed >= timeLimit;

            if (trialActive)
            {
                _timerText.text = "Time Remaining: " + timeRemaining.ToString("0.00") + "s";
                _hitsText.text = "Hits: " + _hitCount;
                _accuracyText.text = "Accuracy: " + _accuracy.ToString("0.00") + "%";
                _scoreText.text = "Score: " + _score;
                StartCoroutine(FillTargets());
                if (timeTrialFinished)
                {
                    menuController.ShowScore(_score, _hitCount, _accuracy);
                    playfabManager.SendLeaderboard(_score, currentLeaderboardName);
                    StopTrial();
                }
            }
        }
    }

    IEnumerator FillTargets()
    {
        if (difficulty == 0)
        {
            targetPrefab.transform.localScale = new Vector3(2,2,2);
        } else if (difficulty == 1)
        {
            targetPrefab.transform.localScale = new Vector3(1, 1, 1);
        }
        _targetCount = targetParent.transform.childCount;
        //Debug.Log("# Targets: " + _targetCount);
        while (_targetCount < targetsToSpawn)
        {
            int numSections = spawnPointParent.transform.childCount;
            int randomSectionIndex = Random.Range(0, numSections);
            Transform spawnSection = spawnPointParent.transform.GetChild(randomSectionIndex).transform;
            //If easy, only use central section.
            if (difficulty == 0)
            {
                spawnSection = spawnPointParent.transform.GetChild(0).transform;
            }
            int numPointsInSection = spawnSection.childCount;
            int randomSpawnIndex = Random.Range(0, numPointsInSection);
            Vector3 spawnPoint = spawnSection.transform.GetChild(randomSpawnIndex).transform.position;
            GameObject target = Instantiate(targetPrefab, targetParent.transform);
            target.transform.position = spawnPoint;
            _targetCount++;
            //Debug.Log("Not enough targets, new one spawned.");
        }
        yield return null;
    }

    public override void StartTrial()
    {
        ResetTrial();
        _countdownActive = true;
        _startButtonText.text = "Stop";
        trialUiParent.SetActive(true);
        trialUiParent.transform.Find("Countdown").gameObject.SetActive(true);
    }

    public override void StopTrial()
    {
        trialActive = false;
        _countdownActive = false;
        _startButtonText.text = "Start";
        trialUiParent.SetActive(false);
        for (int i = 0; i < targetParent.transform.childCount; i++)
        {
            Destroy(targetParent.transform.GetChild(i).gameObject);
        }
    }


    public override void TargetDestroyed()
    {
        _hitCount++;
        _shotCount++; 
        _accuracy = ((float) _hitCount / _shotCount ) * 100;
        _score = (int) Math.Round(_hitCount * (_accuracy / 100));
    }

    public override void TargetMissed()
    {
        _shotCount++;
        _accuracy = ((float) _hitCount / _shotCount)*100;
        _score = (int) Math.Round(_hitCount * (_accuracy / 100));
    }
    
    public void ResetTrial()
    {
        _countdownStartTime = Time.time;
        _hitCount = 0;
        _shotCount = 0;
        _accuracy = 100;
        _score = 0;
    }

    public override bool GetActive()
    {
        return trialActive || _countdownActive;
    }

    public override bool GetTrialActive()
    {
        return trialActive;
    }

    public override void SetDifficulty(int difficultyValue)
    {
        difficulty = difficultyValue;
    }

    public override int GetDifficulty()
    {
        return difficulty;
    }
}
