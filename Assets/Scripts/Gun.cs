﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single }
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100; // rate of file
    public float muzzleVelocity = 35; // speed at which the bullet will leave the gun

    public int burstCount;
    

    public Transform shell;
    public Transform shellEjection;

    MuzzleFlash muzzleFlash;
    float nextShotTime;
    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;

    private void Start()
    {
        shotsRemainingInBurst = burstCount;
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            if(fireMode == FireMode.Burst)
            {
                if(shotsRemainingInBurst == 0) return;
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot) return;
            }

            for(int i = 0; i < projectileSpawn.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        shotsRemainingInBurst = burstCount;
        triggerReleasedSinceLastShot = true;
    }

}
