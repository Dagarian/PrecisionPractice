using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Object = System.Object;

public class Target : MonoBehaviour
{
    [SerializeField] private int health;
    private TrialManager _trialManager;

    public void Awake()
    {
        _trialManager = gameObject.transform.parent.transform.parent.GetComponentInChildren<TrialManager>();
    }

    public void TakeDamage(int damage)
    {
        //Debug.Log("Target hit, taking " + damage + " damage.");
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        _trialManager.TargetDestroyed();
        Destroy(gameObject);
    }
}
