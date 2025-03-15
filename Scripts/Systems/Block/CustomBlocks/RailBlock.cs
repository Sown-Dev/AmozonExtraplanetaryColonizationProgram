using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Systems.Items;
using UI.BlockUI;

namespace Systems.Block
{
    public class RailBlock : Block, IContainerBlock
    {
        public RailOrientation next;
        public RailOrientation previous;

        private void Start()
        {
            CalculateOrientation();
            SetSprite();
        }

        public virtual void OnCartEnter(Cart cart)
        {
            myCart = cart;
        }

        private void CalculateOrientation()
        {
            Vector2Int pos = Vector2Int.RoundToInt(transform.position);
            Orientation[] directions = { Orientation.Up, Orientation.Down, Orientation.Left, Orientation.Right };

            foreach (Orientation dir in directions)
            {
                Vector2Int neighborPos = pos + Vector2Int.RoundToInt(dir.GetVector2());
                Block block = TerrainManager.Instance.GetBlock(neighborPos);

                if (block != null && block is RailBlock neighborRail)
                {
                    // Check if neighbor's next points to this rail
                    if (neighborRail.next != null)
                    {
                        Vector2Int nextPos = neighborPos + Vector2Int.RoundToInt(neighborRail.next.orientation.GetVector2());
                        if (nextPos == pos)
                        {
                            RailBlock oldNext = neighborRail.next.rail;
                            neighborRail.RemoveNext();
                            neighborRail.SetNext(this, dir.GetOpposite());
                            this.SetPrevious(neighborRail, dir.GetOpposite().GetOpposite());

                            if (oldNext != null && oldNext != this)
                                this.SetNext(oldNext, neighborRail.next.orientation);
                        }
                    }

                    // Check if neighbor's previous points to this rail
                    if (neighborRail.previous != null)
                    {
                        Vector2Int prevPos = neighborPos + Vector2Int.RoundToInt(neighborRail.previous.orientation.GetVector2());
                        if (prevPos == pos)
                        {
                            RailBlock oldPrev = neighborRail.previous.rail;
                            neighborRail.RemovePrevious();
                            neighborRail.SetPrevious(this, dir.GetOpposite());
                            this.SetNext(neighborRail, dir.GetOpposite().GetOpposite());

                            if (oldPrev != null && oldPrev != this)
                                this.SetPrevious(oldPrev, neighborRail.previous.orientation);
                        }
                    }

                    // Connect to rails with null connections
                    if (neighborRail.next == null)
                    {
                        if (previous == null)
                        {
                            neighborRail.SetNext(this, dir.GetOpposite());
                            continue;
                        }
                        if (next == null)
                        {
                            InvertRecursivePrevious();
                            neighborRail.SetNext(this, dir.GetOpposite());
                            continue;
                        }
                    }

                    if (neighborRail.previous == null)
                    {
                        if (next == null)
                        {
                            neighborRail.SetPrevious(this, dir.GetOpposite());
                            continue;
                        }
                        if (previous == null)
                        {
                            InvertRecursiveNext();
                            neighborRail.SetPrevious(this, dir.GetOpposite());
                            continue;
                        }
                    }
                }
            }
        }

        public void SetNext(RailBlock nextBlock, Orientation nextOrientation)
        {
            if (nextBlock == this) return;

            next?.rail.RemovePrevious();
            next = new RailOrientation(nextBlock, nextOrientation);
            nextBlock.previous = new RailOrientation(this, nextOrientation.GetOpposite());
            SetSprite();
        }

        public void SetPrevious(RailBlock previousBlock, Orientation previousOrientation)
        {
            if (previousBlock == this) return;

            previous?.rail.RemoveNext();
            previous = new RailOrientation(previousBlock, previousOrientation);
            previousBlock.next = new RailOrientation(this, previousOrientation.GetOpposite());
            SetSprite();
        }

        public override bool BlockDestroy(bool dropLoot = true)
        {
            next?.rail.RemovePrevious();
            previous?.rail.RemoveNext();
            return base.BlockDestroy(dropLoot);
        }

        public void RemoveNext()
        {
            if (next == null) return;
            next.rail.previous = null;
            next = null;
            SetSprite();
        }

        public void RemovePrevious()
        {
            if (previous == null) return;
            previous.rail.next = null;
            previous = null;
            SetSprite();
        }

        private void Invert()
        {
            RailOrientation tempPrev = previous;
            RailOrientation tempNext = next;

            RemovePrevious();
            RemoveNext();

            if (tempPrev != null) SetNext(tempPrev.rail, tempPrev.orientation);
            if (tempNext != null) SetPrevious(tempNext.rail, tempNext.orientation);
        }

        public void InvertRecursiveNext()
        {
            Invert();
            next?.rail.InvertRecursiveNext();
        }

        public void InvertRecursivePrevious()
        {
            Invert();
            previous?.rail.InvertRecursivePrevious();
        }

        // Sprite configuration and SetSprite method remain unchanged
        public Sprite vert;
        public Sprite horiz;
        public Sprite upLeft;
        public Sprite upRight;
        public Sprite downLeft;
        public Sprite downRight;

        public void SetSprite()
        {
            if (next == null && previous == null)
            {
                GetComponent<SpriteRenderer>().sprite = vert;
                return;
            }

            Orientation? primary = next?.orientation ?? previous?.orientation;
            Orientation? secondary = previous?.orientation ?? next?.orientation;

            if (primary == null || secondary == null) return;

            bool vertical = primary.Value.isVertical() && secondary.Value.isVertical();
            bool horizontal = !primary.Value.isVertical() && !secondary.Value.isVertical();

            if (vertical)
            {
                GetComponent<SpriteRenderer>().sprite = vert;
            }
            else if (horizontal)
            {
                GetComponent<SpriteRenderer>().sprite = horiz;
            }
            else
            {
                HashSet<Orientation> orientations = new HashSet<Orientation> { primary.Value, secondary.Value };
                if (orientations.Contains(Orientation.Up) && orientations.Contains(Orientation.Right))
                    GetComponent<SpriteRenderer>().sprite = upRight;
                else if (orientations.Contains(Orientation.Up) && orientations.Contains(Orientation.Left))
                    GetComponent<SpriteRenderer>().sprite = upLeft;
                else if (orientations.Contains(Orientation.Down) && orientations.Contains(Orientation.Right))
                    GetComponent<SpriteRenderer>().sprite = downRight;
                else if (orientations.Contains(Orientation.Down) && orientations.Contains(Orientation.Left))
                    GetComponent<SpriteRenderer>().sprite = downLeft;
            }
        }

        [HideInInspector] public Cart myCart;

        public bool Insert(ref ItemStack stack, bool simulate = false)
        {
            return myCart?.container.Insert(ref stack, simulate) ?? false;
        }

        public ItemStack Extract()
        {
            return myCart?.container.Extract();
        }

        public override void Use(Unit user)
        {
            base.Use(user);
            if (myCart)
                BlockUIManager.Instance.GenerateContainerUI(myCart.container);
        }
    }

    public class RailOrientation
    {
        public RailBlock rail;
        public Orientation orientation;

        public RailOrientation(RailBlock rail, Orientation orientation)
        {
            this.rail = rail;
            this.orientation = orientation;
        }
    }
}