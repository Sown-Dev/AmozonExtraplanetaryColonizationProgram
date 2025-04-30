using System;
using System.Collections;
using System.Linq;
using NewRunMenu;
using Newtonsoft.Json;
using Systems.Block;
using Systems.Block.CustomBlocks;
using Systems.Items;
using Systems.Round;
using Systems.Terrain;
using TMPro;
using UI;
using UI.BlockUI;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Unit = Systems.Unit;
using Terrain = Systems.Terrain.Terrain;

public partial class Player : Unit, IContainer{
    public static Player Instance;
    static Container inventorySave; //just for testing


    [SerializeField] public Cursor myCursor;

    [SerializeField] public SpriteRenderer buildingPreview;
    [SerializeField] private SlotVisualizer handVisualizer;
    [SerializeField] private Camera myCam;

    [SerializeField] private GameObject OnDeath;
    [SerializeField] private PopupListUI popupList;
    [SerializeField] private Item money; //reference to check against

    public Character myCharacter;

    [HideInInspector] public Material myMat;

    public Slot SelectedSlot;

    [Header("PlayerMove Fields")] [SerializeField]
    protected Collider2D playerCollider; // Reference to the player's collider

    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private Transform spriteHolder;
    [SerializeField] private SpriteRenderer hatSR;
    [SerializeField] private Animator am;
    [SerializeField] private Transform highlights;
    [SerializeField] private GameObject Invalid;
    [SerializeField] private TileIndicatorManager indicatorManager;
    [SerializeField] private GameObject DropPod; //only spawned at beginning, destroyed on landing
    [SerializeField] private GameObject DropPodDestroy;
    [SerializeField] private LayerMask wallLayer;


    //physics
    private float moveV = 4800f;
    private float maxSpeed = 10f;
    public float y;
    public float jumpVelocity = 10f; // The initial velocity applied when jumping   
    public float gravity = -36; // Gravity applied to the player
    private float yVelocity = 0f; // Current vertical velocity of the player
    private float groundLevel = 0f; // The Y position that represents the ground level
    private float disableColliderHeight = 0.75f; // Height at which the collider is disabled
    private bool m_Grounded = false;

    [HideInInspector] public float destroyTimer = 0;
    [HideInInspector] public float destroyDuration;
    float baseDestroyDuration = 1f;
    Vector2Int lastPos;

    private Block standingBlock;
    private Terrain standingTerrain;
    private TerrainProperties standingTerrainProperties;


    protected override void Awake(){
        Instance = this;

        base.Awake();
        myCharacter = GameManager.Instance.GetCharacter(GameManager.Instance?.currentWorld.playerCharacter) ?? myCharacter;

        myCursor.gameObject.SetActive(false); //disable at first, enable when we land


        myCursor.OnLeftClick.AddListener(ClickPos);
        myCursor.OnRightClick.AddListener(RightClickPos);
        myCursor.OnCTRLClick.AddListener(CTRLClickPos);
        //Inventory.AddOnInsert(OnInsert); //shit doesnt work bruh


        myMat = sr.material;
        myMat.SetColor("_ReplaceColor1", myCharacter.shirtColor);
        myMat.SetColor("_ReplaceColor2", myCharacter.pantsColor);
        myMat.SetColor("_ReplaceColor3", myCharacter.skinColor);

        //used to be in start, but this is prob fine. reason is to maybe stop the particle system from blasting a bunch due to position change.

        CalculateStats();
    }

    //called if new world
    public void InitPlayer(){
        y = 220;
        yVelocity = -12;


        //we only do this if new world, otherwise we load existing upgrades 

        //add starting upgrades
        foreach (UpgradeSO u in myCharacter.startingUpgrades){
            AddUpgrade((Upgrade)u.u, false);
        }

        //add starting items
        foreach (ItemStack i in myCharacter.startingItems){
            ItemStack item = i.Clone();
            Insert(ref item, false);
        }

        SelectSlot(Inventory.GetSlot(7)); //select first slot
    }

    public void LoadPlayer(PlayerData data){
        transform.position = data.position;
        y = data.y;

        //print inventory
        Debug.Log("loaded:" + JsonConvert.SerializeObject(data, GameManager.JSONsettings));
        Inventory = new Container(data.Inventory);
        //Inventory = inventorySave;

        foreach (Upgrade u in data.upgrades){
            //AddUpgrade(u, false);
        }

        Slot selected = Inventory.GetSlot(data.selectedSlotID);
        selected.Selected = false;
        SelectSlot(selected);
    }


    public PlayerData SavePlayer(){
        PlayerData data = new PlayerData();
        data.position = transform.position;
        data.y = y;
        data.Inventory = Inventory;
        inventorySave = Inventory;
        data.upgrades = upgrades.ToArray();
        //get selected slot id
        for (int i = 0; i < Inventory.Size; i++){
            if (Inventory.GetSlot(i) == SelectedSlot){
                data.selectedSlotID = i;
                break;
            }
        }

        Debug.Log(JsonConvert.SerializeObject(data.Inventory, GameManager.JSONsettings));


        return data;
    }


    private void Start(){
        if (!GameManager.Instance.currentWorld.generated){
            InitPlayer();
        }
        else{
            LoadPlayer(GameManager.Instance.currentWorld.playerData);
            PlayerReady();
        }

        hatSR.sprite = myCharacter.HatSprite;
#if PLAYERITEMS1
        Debug.Log("Adding dev inventory");
        Inventory.Insert(new ItemStack(Utils.Instance.furnaceBlock, 12));
        Inventory.Insert(new ItemStack(Utils.Instance.drillBlock, 12));
        Inventory.Insert(new ItemStack(Utils.Instance.crateBlock, 6));
        Inventory.Insert(new ItemStack(Utils.Instance.refinerBlock, 8));
        Inventory.Insert(new ItemStack(Utils.Instance.inserterBlock, 24));
        Inventory.Insert(new ItemStack(Utils.Instance.workstation, 3));

        Inventory.Insert(new ItemStack(Utils.Instance.railBlock, 54));
        Inventory.Insert(new ItemStack(Utils.Instance.railCart, 6));


        Inventory.Insert(new ItemStack(Utils.Instance.copperOre, 24));
        Inventory.Insert(new ItemStack(Utils.Instance.copperOre, 24));

#endif

        //SelectSlot(Inventory.GetSlot(7));
        prevPos = transform.position;
    }

    private void Update(){
        if (Time.timeScale <= 0) return;


        standingBlock = TerrainManager.Instance.GetBlock(Vector2Int.RoundToInt(transform.position));

        standingTerrain = TerrainManager.Instance.GetTerrain(Vector2Int.RoundToInt(transform.position));

        if (standingTerrain != null)
            standingTerrainProperties = TerrainManager.Instance.GetTerrainProperties(standingTerrain.myProperties);
        else{
            standingTerrainProperties = null;
        }

        handVisualizer.Refresh();


        sr.sortingOrder = 0;
        hatSR.sortingOrder = 0;
        shadow.sortingOrder = 0;
        shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b,
            Mathf.Max(0.4f * (6 - y) / 8, 0.05f));
        // Apply gravity if player is not grounded
        if (!m_Grounded){
            yVelocity += gravity * Time.deltaTime; // Apply gravity to vertical velocity
            y += yVelocity * Time.deltaTime; // Update position based on velocity

            // Check if player has landed
            if (y <= groundLevel){
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
                rb.velocity += ((conveyor.data.rotation.GetOpposite().GetVector2() * (conveyor.Speed * 16 * Time.deltaTime)));
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
        playerCollider.enabled = y < disableColliderHeight;

        if (sr.sortingOrder == 0){
            sr.sortingOrder = y < disableColliderHeight ? 0 : 2;
        }

        handVisualizer.spriteRenderer.sortingOrder = sr.sortingOrder;

        rb.drag = m_Grounded ? 10 : 9;

        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        if (move != Vector2.zero){
            TutorialManager.Instance.StartTutorial("interaction", 1);
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

            if ((myCursor.cursorRotation == Orientation.Right || myCursor.cursorRotation == Orientation.Left) && block.blockPrefab.properties.rotatable){
                (sx, sy) = (sy, sx);
            }

            buildingPreview.color = new Color(1, 1, 1, 0.5f);
            myCursor.sr.size = Vector2.Lerp(myCursor.sr.size, new Vector2(sx, sy), Time.deltaTime * 12);


            Orientation rot = block.blockPrefab.properties.rotatable ? myCursor.cursorRotation : Orientation.Up;

            //indicators
            if (block.blockPrefab.GetIndicators()?.Count > 0){
                indicatorManager.DrawIndicators(block.blockPrefab.GetIndicators(), myCursor.currentPos, rot);
            }

            //invalid
            foreach (var v2 in TerrainManager.Instance.GetBlockPositions(myCursor.currentPos, block.blockPrefab.properties.size.x,
                         block.blockPrefab.properties.size.y, rot)){
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
                sy % 2 == 0 ? (sy / 2f) - 0.5f : 0);

            myCursor.directionArrow.gameObject.SetActive(block.blockPrefab.properties.rotatable);
        }
        else{
            myCursor.sr.size = Vector2.one;
            buildingPreview.color = Color.clear;
            myCursor.buildingPreview.transform.localPosition = Vector2.zero;
            myCursor.directionArrow.gameObject.SetActive(false);

            if (myCursor.lookingBlock){
                indicatorManager.DrawIndicators(myCursor.lookingBlock.GetIndicators(), Vector2Int.zero,
                    myCursor.lookingBlock.properties.rotatable ? myCursor.lookingBlock.data.rotation : Orientation.Up);
            }
        }


        //Destroying Blocks

        if (myCursor.currentPos != lastPos){
            lastPos = myCursor.currentPos;
            destroyTimer = 0;
        }

        if (Input.GetKey(KeyCode.X) && Vector2.Distance(transform.position, myCursor.currentPos) < 8){
            if (TerrainManager.Instance.GetBlock(myCursor.currentPos) != null){
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
                    var extractOre = TerrainManager.Instance.ExtractOre(myCursor.currentPos, (int)finalStats[Statstype.MiningAmount]);
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

        hatSR.sortingOrder = sr.sortingOrder;

        spriteHolder.transform.localPosition = new Vector3(0,
            y, 0);

        float moveForce = (moveV * finalStats[Statstype.Movespeed]);
        moveForce *= (m_Grounded ? standingTerrainProperties?.walkSpeed ?? 1 : 1.2f); //either use terrain speed or air speed, which is 1.2x base speed.

        rb.AddForce(move * moveForce * Time.deltaTime);
    }


    private bool popup = true;

    public bool Insert(ref ItemStack s, bool simulate = false){
        if (s?.item == money){
            RoundManager.Instance.AddMoney(s.amount, false);
            s = null;
            return true;
        }

        if (s == null){
            return false;
        }

        if (!simulate && Inventory.Insert(ref s, true)){
            Popup($"+{s.amount} {s.item.name}");
        }

        bool ret = Inventory.Insert(ref s, simulate);

        popup = true; //
        return Inventory.Insert(s);
    }

    public void OnInventoryChange(){
        //check if we have any money
        if (Inventory.Contains(money)){
            //iterate to find it
            for (int i = 0; i < Inventory.Size; i++){
                if (Inventory.GetSlot(i).ItemStack?.item == money){
                    //remove it
                    RoundManager.Instance.AddMoney(Inventory.GetSlot(i).ItemStack.amount, false);

                    Inventory.GetSlot(i).ItemStack = null;
                    break;
                }
            }
        }
    }

    public ItemStack Extract(){
        return Inventory.Extract();
    }


    public void SelectSlot(Slot slot){
        if (SelectedSlot != null)
            SelectedSlot.Selected = false;

        SelectedSlot = slot;
        SelectedSlot.Selected = true;

        handVisualizer.mySlot = SelectedSlot;
    }


    private void ClickPos(Vector2Int pos){
        SelectedSlot.ItemStack?.item.Use(pos, this, SelectedSlot);
    }


    private void RightClickPos(Vector2Int pos){
        Block b = TerrainManager.Instance.GetBlock(pos);

        if (b){
            //raycast to target, using layoutmask wall. if we hit a wall, false, otherwise true
            bool canPlace = !Physics2D.Raycast(transform.position, (Vector2)pos - (Vector2)transform.position, Vector2.Distance(pos, transform.position),
                wallLayer);

            if (Vector2.Distance(b.transform.position, transform.position) > 10f && canPlace){
                if (BlockUIManager.Instance.currentBlockUI?.block == b){
                    //b.Use(this); could do this also, same thing basically as using should just close it       
                    BlockUIManager.Instance.CloseBlockUI();
                }
                else{
                    Popup("Out of reach", new Color(0.9f, 0.4f, 0.4f));
                    return;
                }
            }
            else{
                b.Use(this);
            }
        }
    }

    private void CTRLClickPos(Vector2Int pos){
        Debug.Log("ctrl click");
        if (Vector2.Distance(pos, transform.position) > 12f)
            return;
        if (TerrainManager.Instance.GetBlock(pos) is IContainerBlock containerBlock){
            //extract to my inventory
            bool b = true;
            while (b)
                b = CU.Transfer(containerBlock, this);
        }
    }

    public void Die(){
        

        Instantiate(OnDeath, transform.position, quaternion.identity);
        myCam.transform.SetParent(transform.parent.parent);
        Destroy(gameObject);
        StartCoroutine(AfterDeath());
    }

    public IEnumerator AfterDeath(){
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ExitToMain(false);
        try{
            GameManager.Instance.DeleteWorld(GameManager.Instance.currentWorld);
        }
        catch (Exception e){
            Debug.LogError(e);
        }

    }

    public override Stats CalculateStats(){
        finalStats = base.CalculateStats();
        finalStats.Combine(myCharacter.stats);

        return finalStats;
    }

    public void Popup(string s){
        popupList.AddPopup(s, Color.white);
    }

    public void Popup(string s, Color c){
        popupList.AddPopup(s, c);
    }


    private float distMoved;
    private Vector3 prevPos;

    private float footstepDist = 1.8f;

    private void FixedUpdate(){
        if (m_Grounded)
            distMoved += Vector3.Distance(transform.position, prevPos);
        prevPos = transform.position;

        //take a step
        if (distMoved > footstepDist){
            distMoved = 0;
            if (standingTerrain != null){
                AudioClip[] clips = TerrainManager.Instance.GetTerrainProperties(standingTerrain.myProperties).footsteps;
                if (clips.Length > 0){
                    audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                    audioSource.Play();
                }
            }
        }
    }


    bool firstLand = true;

    public void Land(){
        y = groundLevel; // Reset to ground level
        Debug.Log("Landed" + yVelocity);
        if (yVelocity < -40 && !firstLand){
            TerrainManager.Instance.CreateBlockDebris((Vector2)transform.position, Color.gray);
        }

        yVelocity = 0; // Reset vertical velocity
        m_Grounded = true; // Player is now grounded

        if (firstLand){
            firstLand = false;
            RoundManager.Instance.StartCooldown(30);

            TutorialManager.Instance.StartTutorial("controls", 1);
            Instantiate(DropPodDestroy, DropPod.transform.position, quaternion.identity);
            PlayerReady();
        }
    }

    //call when player is ready to start interacting with world
    public void PlayerReady(){
        firstLand = false;
        myCursor.gameObject.SetActive(true);
        if (DropPod != null){
            Destroy(DropPod);
        }
    }

    public Slot GetSlot(int id){
        return Inventory.GetSlot(id);
    }
}

[Serializable]
public class PlayerData{
    public Vector2 position;
    public float y;

    public int selectedSlotID;

    public Container Inventory;

    //TODO: finish
    public Upgrade[] upgrades; //save player upgrades
}