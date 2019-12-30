using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this will force the playercontroller script to this object, too
[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5f;
    PlayerController controller;
    GunController gunController;

    public Crosshairs crosshairs;
    Camera viewCamera;

    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        // assuming that the playercontroller is attached to the same game object as the player script
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight); // we need to intersect the ray with the ground plane to see where the player will be looking - so make a flat plane
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) // if ray intersects with ground plane
        {
            Vector3 pointOfIntersection = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, pointOfIntersection, Color.red);
            controller.LookAt(pointOfIntersection);
            crosshairs.transform.position = pointOfIntersection;
            crosshairs.DetectTargets(ray);
            if((new Vector2(pointOfIntersection.x, pointOfIntersection.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude >= 1)
                gunController.Aim(pointOfIntersection);
            {

            }
        }

        // Weapon input
        if(Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

    }
}
