using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : Button
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
            _trialManager.StartTrial();
        }
    }
}
