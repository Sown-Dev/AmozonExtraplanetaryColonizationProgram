using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using Systems.Block;
using Systems.Block.CustomBlocks;
using Systems.Items;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player : Unit{
    
    [Header("PlayerMove Fields")]
    [SerializeField] protected Collider2D playerCollider; // Reference to the player's collider
    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private Transform spriteHolder;
    [SerializeField] private Animator am;
    [SerializeField] private Transform highlights;
    [SerializeField] private GameObject Invalid;
    [SerializeField] private TileIndicatorManager indicatorManager;
    [SerializeField] private GameObject DropPod; //only spawned at beginning, destroyed on landing
    [SerializeField] private GameObject DropPodDestroy;

    private float moveV = 4600f;
    private float maxSpeed = 10f;
    public float jumpVelocity = 10f; // The initial velocity applied when jumping   
    public float gravity = -36; // Gravity applied to the player
    private float yVelocity = 0f; // Current vertical velocity of the player
    private float groundLevel = 0f; // The Y position that represents the ground level
    private float disableColliderHeight = 1f; // Height at which the collider is disabled
    private bool m_Grounded = false;

    private void Update(){
        if(Time.timeScale<=0) return;
        
        Block standingBlock = TerrainManager.Instance.GetBlock(Vector2Int.RoundToInt(transform.position));
        
        handVisualizer.Refresh();

        
        sr.sortingOrder = 0;
        shadow.sortingOrder = 0;
        shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b,
            Mathf.Max(0.4f * (6 - spriteHolder.transform.localPosition.y) / 8, 0.05f));
        // Apply gravity if player is not grounded
        if (!m_Grounded){
            yVelocity += gravity * Time.deltaTime; // Apply gravity to vertical velocity
            spriteHolder.transform.localPosition +=
                new Vector3(0, yVelocity * Time.deltaTime, 0); // Update position based on velocity

            // Check if player has landed
            if (spriteHolder.transform.localPosition.y <= groundLevel){
                Land();
               
            }
        }
        else{
            //IF GROUNDED:

            // Jumping and gravity simulation
            if (Input.GetKeyDown(KeyCode.Space) && m_Grounded){
                rb.AddForce(rb.velocity.normalized * 10f, ForceMode2D.Impulse);
                yVelocity = jumpVelocity; // Apply initial jump velocity
                m_Grounded = false; // Player is no longer grounded
            }

            //Conveyor
            if (standingBlock is ConveyorBeltBlock conveyor){
                rb.velocity += ((conveyor.rotation.GetOpposite().GetVector2() * (conveyor.Speed * 16 * Time.deltaTime)));
                sr.sortingOrder = 2;
                shadow.sortingOrder = 1;
            }
            else{
                sr.sortingOrder = 0;
            }
        }

        am.SetFloat("xVel", rb.velocity.magnitude);
        am.SetFloat("yVel", yVelocity);


        
        //round position
        
        // Disable or enable the collider based on height
        playerCollider.enabled = spriteHolder.transform.localPosition.y < disableColliderHeight;

        if (sr.sortingOrder == 0){
            sr.sortingOrder = spriteHolder.transform.localPosition.y < disableColliderHeight ? 0 : 2;
        }
        handVisualizer.spriteRenderer.sortingOrder = sr.sortingOrder;

        rb.drag = m_Grounded ? 10 : 9;

        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        
        if(move != Vector2.zero){
            TutorialManager.Instance.StartTutorial("interaction",1);
        }

        if (move.x > 0){
            sr.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (move.x < 0){
            sr.transform.localScale = new Vector3(-1, 1, 1);
        }
        
        foreach (Transform child in highlights.transform){
            Destroy(child.gameObject);
        }
      
        
        if (SelectedSlot.ItemStack?.item is BlockItem block && PlayerUI.Instance.OnTop.childCount == 0){
            
              
            int sx = block.blockPrefab.properties.size.x;
            int sy = block.blockPrefab.properties.size.y;
            
            if(( myCursor.cursorRotation== Orientation.Right || myCursor.cursorRotation== Orientation.Left) && block.blockPrefab.properties.rotatable){
                (sx,sy) = (sy,sx);
            }

            buildingPreview.color = new Color(1, 1, 1, 0.5f);
            myCursor.sr.size = Vector2.Lerp( myCursor.sr.size, new Vector2(sx,sy),Time.deltaTime * 12);
            
                
            Orientation rot = block.blockPrefab.properties.rotatable ? myCursor.cursorRotation : Orientation.Up;
            
            //indicators
            if (block.blockPrefab.GetIndicators()?.Count > 0){
                indicatorManager.DrawIndicators(block.blockPrefab.GetIndicators(), myCursor.currentPos, rot);
            }
            
            //invalid
            foreach (var v2 in TerrainManager.Instance.GetBlockPositions(myCursor.currentPos, block.blockPrefab.properties.size.x, block.blockPrefab.properties.size.y,rot)){
                if (TerrainManager.Instance.GetBlock(v2) != null || TerrainManager.Instance.IsWall((Vector3Int)v2)){
                    GameObject go = Instantiate(Invalid, highlights);
                    go.transform.position = (Vector3Int)v2;
                }

            }
            

            if ((block.blockPrefab.currentState?.rotateable ?? false) && block.blockPrefab.currentState
                    .rotations[(int)myCursor.cursorRotation].sprites.Length > 0){
                buildingPreview.sprite =
                    block.blockPrefab.currentState.rotations[(int)myCursor.cursorRotation].sprites[0];
            }
            else
                buildingPreview.sprite = block.blockPrefab.sr.sprite;
          
            myCursor.buildingPreview.transform.localPosition = new Vector2(
                sx % 2 == 0 ? (sx / 2f) - 0.5f : 0,
                sy% 2 == 0 ? (sy / 2f) - 0.5f : 0);

            myCursor.directionArrow.gameObject.SetActive(block.blockPrefab.properties.rotatable);
        }
        else{
            myCursor.sr.size = Vector2.one;
            buildingPreview.color = Color.clear;
            myCursor.buildingPreview.transform.localPosition = Vector2.zero;
            myCursor.directionArrow.gameObject.SetActive(false);
        }

        
        
        //Destroying Blocks

        if (myCursor.currentPos != lastPos){
            lastPos = myCursor.currentPos;
            destroyTimer = 0;
        }

        if (Input.GetKey(KeyCode.X) && Vector2.Distance(transform.position, myCursor.currentPos) < 8){
            if ( TerrainManager.Instance.GetBlock(myCursor.currentPos)  != null){
                destroyDuration = TerrainManager.Instance.GetBlock(myCursor.currentPos).properties.destroyTime *
                               baseDestroyDuration * finalStats[Statstype.MiningSpeed];
                if (!TerrainManager.Instance.GetBlock(myCursor.currentPos)?.properties.indestructible ?? false){
                    move = Vector2.zero;
                    destroyTimer += Time.deltaTime;
                    if (destroyTimer > destroyDuration){
                        destroyTimer = 0;
                        TerrainManager.Instance.RemoveBlock(myCursor.currentPos);
                    }
                }
            }
            else if (TerrainManager.Instance.GetOre(myCursor.currentPos) != null){
                destroyDuration = baseDestroyDuration * finalStats[Statstype.MiningSpeed];
                move = Vector2.zero;
                destroyTimer += Time.deltaTime;
                if (destroyTimer > destroyDuration){
                    destroyTimer = 0;
                    var extractOre = TerrainManager.Instance.ExtractOre(myCursor.currentPos, (int) finalStats[Statstype.MiningAmount]);
                    Insert(ref extractOre);
                    /*if (extractOre != null || extractOre.amount >= 0){
                        Utils.Instance.CreateItemDrop(extractOre, (Vector2)myCursor.currentPos);
                    }*/
                }
            }
            else{
                destroyTimer = 0;
            }
        }
        else{
            destroyTimer = 0;
        } 

        rb.AddForce(move * (moveV * finalStats[Statstype.Movespeed]) * Time.deltaTime * (m_Grounded ? 1f : 1.2f));
    }
bool firstLand = true;
    public void Land(){
        spriteHolder.transform.localPosition = new Vector3(0, groundLevel, 0); // Reset to ground level
        Debug.Log("Landed" + yVelocity);
        if (yVelocity < -30){
            TerrainManager.Instance.CreateBlockDebris((Vector2)transform.position, Color.gray);
        }

        yVelocity = 0; // Reset vertical velocity
        m_Grounded = true; // Player is now grounded
        
        if (firstLand){
            firstLand = false;
            myCursor.gameObject.SetActive(true);
            TutorialManager.Instance.StartTutorial("controls",1);
            Instantiate(DropPodDestroy, DropPod.transform.position, quaternion.identity);
            Destroy(DropPod);
        }
        

    }

    [HideInInspector] public float destroyTimer = 0;
    [HideInInspector] public float destroyDuration;
    float baseDestroyDuration = 1f;
    Vector2Int lastPos;
}