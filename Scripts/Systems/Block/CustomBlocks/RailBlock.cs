using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Block;
using Systems.Items;
using UI.BlockUI;
using UnityEngine;

namespace Systems.Block{
    public class RailBlock : Block, IContainerBlock{
        public RailOrientation next;
        public RailOrientation previous;


        private void Start(){
            CalculateOrientation();
            SetSprite();
        }

        public virtual void OnCartEnter(Cart cart){
            myCart = cart;
        }

        private void CalculateOrientation(){
            Vector2Int pos = Vector2Int.RoundToInt(transform.position);

            Orientation[] directions ={ Orientation.Up, Orientation.Down, Orientation.Left, Orientation.Right };


            foreach (Orientation dir in directions){
                Block block = TerrainManager.Instance.GetBlock(pos + Vector2Int.RoundToInt(dir.GetVector2()));
                if (block != null){
                    if (block is RailBlock railBlock){
                        if (railBlock.next == null){
                            if (previous == null){
                                railBlock.SetNext(this, dir.GetOpposite());
                                continue;
                            }
                            else{
                                InvertRecursivePrevious();
                                railBlock.SetNext(this, dir.GetOpposite());
                                continue;
                            }
                        }

                        if (railBlock.previous == null){
                            if (next == null){
                                railBlock.SetPrevious(this, dir.GetOpposite());
                                continue;
                            }
                            else{
                                InvertRecursiveNext();
                                railBlock.SetPrevious(this, dir.GetOpposite());
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public void SetNext(RailBlock nextBlock, Orientation nextOrientation){
            next = new RailOrientation(nextBlock, nextOrientation);

            nextBlock.previous = new RailOrientation(this, nextOrientation.GetOpposite());
            //nextBlock.DestroyAction += RemoveNext;
            //DestroyAction += nextBlock.RemovePrevious;
            SetSprite();
        }

        public void SetPrevious(RailBlock previousBlock, Orientation previousOrientation){
            previous = new RailOrientation(previousBlock, previousOrientation);

            previousBlock.next = new RailOrientation(this, previousOrientation.GetOpposite());
            //wtf was i smoking
            //previousBlock.DestroyAction += RemovePrevious;
            //DestroyAction += previousBlock.RemoveNext;

            SetSprite();
        }

        public override bool BlockDestroy(bool dropLoot = true){
            next?.rail.RemovePrevious();
            previous?.rail.RemoveNext();
            return base.BlockDestroy(dropLoot);
        }


        public void RemoveNext(){
            next.rail.previous = null;
            next = null;
            SetSprite();
        }

        public void RemovePrevious(){
            previous.rail.next = null;
            previous = null;
            SetSprite();
        }

        public void Invert(){
            previous.rail.DestroyAction -= RemovePrevious;
            DestroyAction -= previous.rail.RemoveNext;

            next.rail.DestroyAction -= RemoveNext;
            DestroyAction -= next.rail.RemovePrevious;

            RailOrientation temp = next;
            SetNext(previous.rail, previous.orientation);
            SetPrevious(temp.rail, temp.orientation);
        }

        public void InvertRecursiveNext(){
            Invert();
            if (next != null){
                if (next.rail.previous.rail != this)
                    next.rail.SetPrevious(this, next.orientation.GetOpposite());
                next.rail.InvertRecursiveNext();
            }
        }

        public void InvertRecursivePrevious(){
            Invert();
            if (previous != null){
                if (previous.rail.next.rail != this)
                    previous.rail.SetNext(this, previous.orientation.GetOpposite());
                previous.rail.InvertRecursivePrevious();
            }
        }


        public Sprite vert;
        public Sprite horiz;

        public Sprite upLeft; // |_
        public Sprite upRight; //  _|
        public Sprite downLeft;
        public Sprite downRight;

        public void SetSprite(){
            if (next == null && previous == null){
                sr.sprite = vert;
                return;
            }

            if (next == null){
                sr.sprite = previous.orientation.isVertical() ? vert : horiz;
                return;
            }

            if (previous == null){
                sr.sprite = next.orientation.isVertical() ? vert : horiz;
                return;
            }

            if (next.orientation.isVertical() && previous.orientation.isVertical()){
                sr.sprite = vert;
            }

            if (!next.orientation.isVertical() && !previous.orientation.isVertical()){
                sr.sprite = horiz;
            }

            if ((next.orientation == Orientation.Up && previous.orientation == Orientation.Left) ||
                previous.orientation == Orientation.Up && next.orientation == Orientation.Left){
                sr.sprite = upLeft;
            }

            if ((next.orientation == Orientation.Up && previous.orientation == Orientation.Right) ||
                previous.orientation == Orientation.Up && next.orientation == Orientation.Right){
                sr.sprite = upRight;
            }

            if ((next.orientation == Orientation.Down && previous.orientation == Orientation.Left) ||
                previous.orientation == Orientation.Down && next.orientation == Orientation.Left){
                sr.sprite = downLeft;
            }

            if ((next.orientation == Orientation.Down && previous.orientation == Orientation.Right) ||
                previous.orientation == Orientation.Down && next.orientation == Orientation.Right){
                sr.sprite = downRight;
            }
        }


        private void OnDrawGizmos(){
            Gizmos.color = Color.red;
            if (next != null)
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)next.orientation.GetVector2() / 4f);
            Gizmos.color = Color.blue;
            if (previous != null)
                Gizmos.DrawLine(transform.position,
                    transform.position + (Vector3)previous.orientation.GetVector2() / 4f);
        }

        [HideInInspector] public Cart myCart;

        public bool Insert(ref ItemStack s, bool simulate = false){
            if (myCart != null)
                return myCart.container.Insert(ref s, simulate);
                
            return false;
        }

        public ItemStack Extract(){
            if (myCart != null){
                return  myCart.container.Extract();
            }

            return null;
        }

        public override void Use(Unit user){
            base.Use(user);
            if (myCart)
                BlockUIManager.Instance.GenerateContainerUI(myCart.container);
        }
    }
}

public class RailOrientation{
    public RailBlock rail;
    public Orientation orientation;

    public RailOrientation(RailBlock rail, Orientation orientation){
        this.rail = rail;
        this.orientation = orientation;
    }
}