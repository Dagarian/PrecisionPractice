using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : Button
{
    [SerializeField] private MenuController menuController;
    private TrialManager _trialManager;
    public void Awake()
    {
        _trialManager = transform.parent.parent.GetChild(0).GetComponent<TrialManager>();
        if (_trialManager == null)
        {
            Debug.LogError("Trial manager not set.");
        }
    }
    public override void HitButton()
    {
        if (!_trialManager.GetActive())
        {
            menuController.ShowTrialSettings(_trialManager);
        }
    }
}
