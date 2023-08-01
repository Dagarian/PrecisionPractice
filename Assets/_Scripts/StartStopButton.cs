using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStopButton : Button
{
    private TrialManager _trialManager;
    public void Awake()
    {
        _trialManager = gameObject.transform.parent.transform.parent.GetComponentInChildren<TrialManager>();
    }

    public override void HitButton()
    {
        if (_trialManager.GetActive())
        {
            _trialManager.StopTrial();
        }
        else
        {
            _trialManager.StartTrial();
        }
    }
}
