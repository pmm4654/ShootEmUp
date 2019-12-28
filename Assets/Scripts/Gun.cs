using System.Collections;
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
    public int projectilesPerMagazine;
    public float reloadDelayTime = .2f;
    public float reloadTime = .3f;
    public float maxReloadAngle = 30;


    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3,5);
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;

    MuzzleFlash muzzleFlash;
    float nextShotTime;
    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    int projectilesRemainingInMagazine;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotationSmoothDampVelocity;
    float recoilAngle;


    private void Start()
    {
        shotsRemainingInBurst = burstCount;
        muzzleFlash = GetComponent<MuzzleFlash>();
        projectilesRemainingInMagazine = projectilesPerMagazine;
    }

    private void LateUpdate()
    {
        // animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotationSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.right * -recoilAngle;

        if(!isReloading && projectilesRemainingInMagazine == 0)
        {
            Reload();
        }
    }

    void Shoot()
    {
        if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMagazine > 0)
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
                if(projectilesRemainingInMagazine == 0)
                {
                    break;
                }
                projectilesRemainingInMagazine--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y); // kick gun back for recoil effect
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0f, recoilAngleMinMax.y);
        }
    }

    public void Reload()
    {
        if(!isReloading && projectilesRemainingInMagazine != projectilesPerMagazine)
        {
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDelayTime);

        float percent = 0;
        float reloadSpeed = 1 / reloadTime;
        Vector3 initialRotation = transform.localEulerAngles;
        while (percent < 1)
        {
            percent += reloadSpeed * Time.deltaTime;
            float interpolation = (-Mathf.Pow(-percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.right * -reloadAngle;
            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMagazine = projectilesPerMagazine;
    }

    public void Aim(Vector3 aimPoint)
    {
        if(!isReloading)
        {
            transform.LookAt(aimPoint);
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
