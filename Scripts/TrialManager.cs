using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrialManager : MonoBehaviour
{
    public abstract void StartTrial();
    public abstract void StopTrial();
    public abstract void TargetDestroyed();
    public abstract void TargetMissed();
    public abstract bool GetActive();
    public abstract bool GetTrialActive();
    public abstract void SetDifficulty(int difficultyValue);
    public abstract int GetDifficulty();
}
