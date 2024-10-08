using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using Systems.Items;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player : Unit
{

    [SerializeField] protected Collider2D playerCollider; // Reference to the player's collider
    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private Animator am;

    private float moveV = 4600f;
    public float jumpVelocity = 10f; // The initial velocity applied when jumping   
    public float gravity = -32; // Gravity applied to the player
    private float yVelocity = 0f; // Current vertical velocity of the player
    private float groundLevel = 0f; // The Y position that represents the ground level
    private float disableColliderHeight = 1f; // Height at which the collider is disabled
    private bool m_Grounded = true;

    private void Update()
    {


        sr.sortingOrder = 0;
        shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b,
            Mathf.Max(0.4f * (6 - sr.transform.localPosition.y) / 8, 0.05f));
        // Apply gravity if player is not grounded
        if (!m_Grounded)
        {
            yVelocity += gravity * Time.deltaTime; // Apply gravity to vertical velocity
            sr.transform.localPosition += new Vector3(0, yVelocity * Time.deltaTime, 0); // Update position based on velocity

            // Check if player has landed
            if (sr.transform.localPosition.y <= groundLevel)
            {
                sr.transform.localPosition = new Vector3(0, groundLevel, 0); // Reset to ground level
                yVelocity = 0; // Reset vertical velocity
                m_Grounded = true; // Player is now grounded
            }
        }
        else
        {
            // Jumping and gravity simulation
            if (Input.GetKeyDown(KeyCode.Space) && m_Grounded)
            {

                rb.AddForce(rb.velocity.normalized * 10f, ForceMode2D.Impulse);
                yVelocity = jumpVelocity; // Apply initial jump velocity
                m_Grounded = false; // Player is no longer grounded
            }
        }

        am.SetFloat("xVel", rb.velocity.magnitude);
        am.SetFloat("yVel", yVelocity);


        // Disable or enable the collider based on height
        playerCollider.enabled = sr.transform.localPosition.y < disableColliderHeight;
        sr.sortingOrder = sr.transform.localPosition.y < disableColliderHeight ? 0 : 2;
        rb.drag = m_Grounded ? 10 : 9;

        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        if (move.x > 0)
        {
            sr.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (move.x < 0)
        {
            sr.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (SelectedSlot.ItemStack?.item is BlockItem block && PlayerUI.Instance.OnTop.childCount == 0)
        {
            buildingPreview.color = new Color(1, 1, 1, 0.4f);
            myCursor.sr.size = block.blockPrefab.properties.size;


            if ((block.blockPrefab.currentState?.rotateable ?? false) && block.blockPrefab.currentState.rotations[(int)myCursor.cursorRotation].sprites.Length > 0)
            {
                buildingPreview.sprite = block.blockPrefab.currentState.rotations[(int)myCursor.cursorRotation].sprites[0];
            }
            else
                buildingPreview.sprite = block.blockPrefab.sr.sprite;

            myCursor.buildingPreview.transform.localPosition = new Vector2(
                block.blockPrefab.properties.size.x % 2 == 0 ? (block.blockPrefab.properties.size.x / 2f) - 0.5f : 0,
                block.blockPrefab.properties.size.y % 2 == 0 ? (block.blockPrefab.properties.size.y / 2f) - 0.5f : 0);
            
            myCursor.directionArrow.gameObject.SetActive(block.blockPrefab.properties.rotateable);

        }
        else
        {
            myCursor.sr.size = Vector2.one;
            buildingPreview.color = Color.clear;
            myCursor.buildingPreview.transform.localPosition = Vector2.zero;
            myCursor.directionArrow.gameObject.SetActive(false);

        }


        if (myCursor.currentPos != lastPos)
        {
            lastPos = myCursor.currentPos;
            destroyTimer = 0;
        }
        if (Input.GetKey(KeyCode.X) && Vector2.Distance(transform.position, myCursor.currentPos) < 8)
        {

            if (TerrainManager.Instance.GetBlock(myCursor.currentPos) != null)
            {
                if (!TerrainManager.Instance.GetBlock(myCursor.currentPos)?.properties.indestructible ?? false)
                {
                    move = Vector2.zero;
                    destroyTimer += Time.deltaTime;
                    if (destroyTimer > destroyDuration)
                    {
                        destroyTimer = 0;
                        TerrainManager.Instance.RemoveBlock(myCursor.currentPos);
                    }
                }
            }
            else if (TerrainManager.Instance.GetOre(myCursor.currentPos) != null)
            {
                move = Vector2.zero;
                destroyTimer += Time.deltaTime;
                if (destroyTimer > destroyDuration)
                {
                    destroyTimer = 0;
                    var extractOre = TerrainManager.Instance.ExtractOre(myCursor.currentPos, 1);
                    Inventory.Insert(ref extractOre);

                }
            }
            else
            {
                destroyTimer = 0;
            }

        }
        else
        {
            destroyTimer = 0;
        }



        rb.AddForce(move * (moveV* finalStats[Statstype.Movespeed]) * Time.deltaTime * (m_Grounded ? 1f : 1.2f));

    }
    [HideInInspector] public float destroyTimer = 0;
    public float destroyDuration = 0.4f;
    Vector2Int lastPos;
}