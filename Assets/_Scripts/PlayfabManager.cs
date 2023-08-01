using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private Transform contentPanel;
    private UserAccountInfo _accountInfo;
    public string playerDisplayName;
    private static string _playerID;
    
    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        //Debug.Log("Successful Login or account creation.");
        _playerID = result.PlayFabId;
        //Debug.Log("Id: " + _playerID);
        var request = new GetAccountInfoRequest
        {
            PlayFabId = _playerID
        };
        PlayFabClientAPI.GetAccountInfo(request, OnAccountRequestSuccess, OnError);
    }

    void OnAccountRequestSuccess(GetAccountInfoResult result)
    {
        _accountInfo = result.AccountInfo;
        playerDisplayName = _accountInfo.TitleInfo.DisplayName;
        //Debug.Log("Display Name: " + playerDisplayName);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        playerDisplayName = result.DisplayName;
        ClearLeaderboard();
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error whilst logging in or creating an account.");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(int score, String leaderboardName)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score sent to leaderboard.");
    }

    public void GetLeaderboard(String leaderboardName)
    {
        //Debug.Log("Trying to request leaderboard: " + leaderboardName);
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            PlayerLeaderboardEntry playerLeaderboardEntry = result.Leaderboard[i];
            //Debug.Log("Adding entry " + playerLeaderboardEntry.DisplayName);
            Transform entryPanel = contentPanel.GetChild(i).transform;
            GameObject tempEntry = Instantiate(leaderboardEntryPrefab, entryPanel);
            LeaderboardEntry entry = tempEntry.GetComponent<LeaderboardEntry>();
            entry.entryName.text = playerLeaderboardEntry.DisplayName;
            entry.entryScore.text = playerLeaderboardEntry.StatValue.ToString();
            //Debug.Log("Pos: " + (playerLeaderboardEntry.Position + 1) +
            //          ". Name: " + playerLeaderboardEntry.DisplayName +
            //          ". Score: " + playerLeaderboardEntry.StatValue);
        }
    }
    
    //Cleans out current existing entries in table for re-viewing.
    public void ClearLeaderboard()
    {
        for (int i = 0; i < contentPanel.childCount -1; i++)
        {
            Transform currentEntry = contentPanel.GetChild(i).transform;
            if (currentEntry.childCount > 0)
            {
                Destroy(currentEntry.GetChild(0).gameObject);
            }
        }
    }
    
    //Sends request to update display name.
    public void UpdateDisplayName(String inputName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = inputName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }
}
