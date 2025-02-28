using Systems.Block;
using Unity.Entities;
using UnityEngine;

public struct BlockComponent : IComponentData {
    public Vector2Int origin;
    public Orientation rotation;
    public Entity properties; // Reference to a properties entity
    public Entity currentState; // Reference to the block's current state
}

public struct SpriteComponent : IComponentData {
    public Entity spriteEntity; // Reference to a sprite entity
    public Color baseColor;
}

public struct LootTableComponent : IBufferElementData {
    public Entity itemEntity; // Reference to an item in the loot table
    public int quantity;
}

public struct BlockStateComponent : IComponentData {
    public Entity currentSprite; // Reference to the current sprite entity
    public bool rotateable;
}