﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rig;
    private Vector3 velocity;
    private GameObject cameraz;
    float x;
    float y;
    public float sensetivity = 2;
    public float speed = 800;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    private CapsuleCollider Colid;
    public float jump = 3;
    public bool touchingGround;
    private float timer;
    // Use this for initialization
    void Start()
    {
        cameraz = transform.Find("Main Camera").gameObject;
        cameraz.GetComponent<AudioListener>().enabled = true;
        rig = GetComponent<Rigidbody>();
        Colid = GetComponent<CapsuleCollider>();
        cameraz.GetComponent<Camera>().depth = 5;
    }

    // Update is called once per frame
    void Update()
    {
        //This timer calculates how long you have to wait before being unparalyzed
        timer = timer + 1 * Time.deltaTime;
        //This locks the cursor so the screen so it isn't distracting or annoying
        Screen.lockCursor = true;

        //This does the calculations on how far the player moved the mouse
        x = sensetivity * Input.GetAxis("Mouse X");
        y = -sensetivity * Input.GetAxis("Mouse Y");

        //This does the calculation on how fast and how much to move the player
        //The Axises "Horizontal" and "Vertical" are linked to W,A,S,D and arrow keys in the editor
        velocity = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")) * speed * Time.deltaTime;
        //Actually moves the players using the input from the keyboard

        //A really bad system of crouching, will be fixed when player models with animations are added
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Halves the player's height
            Colid.height = 1;
            //Moves the player's position without affecting velocity
            if (touchingGround == true)
            {
                rig.MovePosition(rig.position + transform.right * velocity.x / 100);
                rig.MovePosition(rig.position + transform.forward * velocity.z / 100);
            }
        }
        else
        {
            //resets the player's height
            Colid.height = 2;
            //Moves the player normally(using velocity)
            if (touchingGround == true)
            {
                //moving
                //rig.AddForce(transform.forward * velocity.z * Time.fixedDeltaTime, ForceMode.Impulse);
                //rig.AddForce(transform.right * velocity.x * Time.fixedDeltaTime, ForceMode.Impulse);

                //Apparently you can only modify velocity once
                Vector3 motion = (transform.forward * velocity.z);
                motion += (transform.right * velocity.x);

                rig.velocity = motion;
            }
        }
        //Responsible of rotation lock
        //Best not to mess with this the current version of the game as there is only one gravity source
        if (Input.GetKey(KeyCode.Mouse2))
        {
            transform.Rotate(y, x, 0);
        }
        else
        {
            transform.Rotate(0, x, 0);
            cameraz.transform.Rotate(y, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Force();
        }
        if (Input.GetKeyDown(KeyCode.Space) && touchingGround)
        {
            rig.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }
        if (timer > 60)
        {
            rig.constraints = RigidbodyConstraints.FreezeRotation;
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        }
    }
    //This checks if the player has his something
    void OnCollisionEnter(Collision col)
    {
        //This tells the game the player can jump again
        //This is for testing purposes
        if (col.transform.tag == "Finish")
        {
            Destroy(gameObject);
        }
        //If the player has been hut by a bullet than they will be paralyzed
        if (col.transform.tag == "Bullet")
        {
            rig.constraints = RigidbodyConstraints.None;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        touchingGround = true;
    }
    void OnCollisionExit(Collision col)
    {
        touchingGround = false;
    }

    //Will be modified to act as a interaction key, In this version it will create a projectile
    void Force()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 5;

        // Destroy the bullet after 2 seconds to prevent the game from lagging
        Destroy(bullet, 10.0f);
    }
}
