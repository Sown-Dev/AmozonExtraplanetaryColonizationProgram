using System;
using System.Collections;
using Systems.Block;
using Systems.Items;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class Cart : MonoBehaviour{
    public Container container;

    public SpriteRenderer sr;
    public Sprite horiz;
    public Sprite vert;
    public Vector2 startPos;
    public RailBlock CurrenRailBlock;
    public Orientation Orientation;

    public static bool CreateCart(Vector2Int pos, GameObject cartPrefab){

        if (TerrainManager.Instance.GetBlock(pos) is RailBlock railBlock){
            if (railBlock.myCart != null || railBlock.next?.rail.myCart != null) return false;
            
            //Raycast square around the block to check if there is a rail block
            RaycastHit2D hit = Physics2D.BoxCast(pos, Vector2.one/2f , 0, Vector2.zero);
            
            if (hit.collider != null) return false;
            
            Cart cart = Instantiate(cartPrefab, (Vector2)pos, Quaternion.identity).GetComponent<Cart>();
            cart.CurrenRailBlock = railBlock;
            cart.CurrenRailBlock.myCart = cart;
            cart.CurrenRailBlock.OnCartEnter(cart);
            cart.SetCurrent(railBlock);
            return true;
        }
        
        return false;
    }
  
    private void Start(){
        container = new Container(new ContainerProperties(12));
        startPos = transform.position;
        Orientation = CurrenRailBlock.next.orientation;
    }


    float timer;
    private float speed = 0.5f;

    void SetCurrent(RailBlock block){
        CurrenRailBlock.myCart = null;
        CurrenRailBlock = block;
        startPos = transform.position;
        timer = 0;
        block.OnCartEnter(this);

    }


    private bool direction;

    private void Update(){
        if (CurrenRailBlock == null){
            BreakCart();
            return;
        }

        Orientation prevOrientation = Orientation;

        sr.sprite = Orientation.isVertical() ? vert : horiz;
        sr.flipX = !Orientation.isVertical() && direction;

        timer += Time.deltaTime * speed;
        speed += Time.deltaTime / 2f;
        transform.position = Vector2.Lerp(startPos, CurrenRailBlock.transform.position, timer * 2);
        if (Vector2.Distance(transform.position, CurrenRailBlock?.transform.position ?? transform.position) < 0.02f){
            if (direction){
                if (CurrenRailBlock.next == null){
                    direction = !direction;
                    speed *= 0.5f;
                }

                Orientation = CurrenRailBlock?.next.orientation ?? Orientation;
                SetCurrent(CurrenRailBlock.next.rail);
            }
            else{
                if (CurrenRailBlock.previous == null){
                    direction = !direction;
                    speed *= 0.5f;
                }

                Orientation = CurrenRailBlock?.previous?.orientation ?? Orientation;

                SetCurrent(CurrenRailBlock.previous.rail);
            }
        }

        if (prevOrientation != Orientation){
            speed *= 0.6f;
        }
    }

    public void BreakCart(){
        Destroy(gameObject);
        Utils.Instance.CreateItemDrop(new ItemStack(Utils.Instance.railCart, 1), transform.position);
        foreach (ItemStack itemStack in container.GetItems()){
            Utils.Instance.CreateItemDrop(itemStack, transform.position);
        }
    }
}