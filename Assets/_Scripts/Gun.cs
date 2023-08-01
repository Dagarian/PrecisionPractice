using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private MenuController menuController;

    void Update()
    {
        bool isPaused = menuController.GetIsPaused();
        if (Input.GetButtonDown("Fire1") && !isPaused)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit))
        {
            Target target = hit.transform.GetComponent<Target>();
            Button button = hit.transform.GetComponent<Button>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            else if (button != null)
            {
                button.HitButton();
            }
            else
            {
                foreach (var trial in menuController.trials)
                {
                    if (trial.GetTrialActive())
                    {
                        trial.TargetMissed();
                        break;
                    }
                }
            }
        }
        
        //Animation & Sound
        GameObject gunParent = fpsCam.gameObject;
        int gunCount = gunParent.transform.childCount;

        for (int i = 0; i < gunCount; i++)
        {
            GameObject currentGun = gunParent.transform.GetChild(i).gameObject; 
            if (currentGun.activeSelf)
            {
                Animation gunAnimation = currentGun.GetComponent<Animation>();
                gunAnimation.Play();

                AudioSource gunAudio = currentGun.GetComponent<AudioSource>();
                gunAudio.Play();

            }
        }
    }
}
