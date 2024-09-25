using System;
using System.Collections.Generic;
using System.Reflection;
using Systems.Block;
using Systems.Items;
using UI;
using UI.BlockUI;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using sRandom =System.Random;

public class Utils: MonoBehaviour{
    public static Utils Instance;

    public GameObject SlotUIPrefab;
    public GameObject ItemStackUIPrefab;
    
    
    public Sprite blankIcon;
    
    
    [SerializeField] public ItemDrop itemDropPrefab;

    private void Awake(){
        Instance = this;
    }
    
    
    public ItemDrop CreateItemDrop(ItemStack item, Vector3 pos){
        ItemDrop drop = Instantiate(itemDropPrefab, pos + new Vector3(0,Random.Range(-0.001f,0.001f),0), Quaternion.identity);
        drop.Init(item.Clone());
        return drop;
    }
    
    public void CreateItemstackUI(Transform parent, ItemStack itemStack){
        ItemStackUI ui = Instantiate(ItemStackUIPrefab, parent).GetComponent<ItemStackUI>();
        ui.Init(itemStack);
    }
    static sRandom rng = new sRandom();
    public static void Shuffle<T>(IList<T> list){
        int n = list.Count;
        while (n > 1){
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }



    [Header("Items")] [SerializeField] public Item furnaceBlock;
    [SerializeField] public Item crateBlock;
    [SerializeField] public Item refinerBlock;
    [SerializeField] public Item inserterBlock;
    [SerializeField] public Item drillBlock;
    [SerializeField] public Item largeCrateBlock;
    [SerializeField] public Item workstation;
    [SerializeField] public Item railBlock;
    [SerializeField] public Item railCart;
    [SerializeField] public Item conveyor;
    [SerializeField] public Item dynamight;
    
    
    
    [SerializeField] public Item copperOre;
    
    #if UNITY_EDITOR
     public static Color? FindMostProminentColor(Sprite sprite)
    {
        RenderTexture tempRT = RenderTexture.GetTemporary(32, 32, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(sprite.texture, tempRT);
        
        RenderTexture previousRT = RenderTexture.active;
        RenderTexture.active = tempRT;
        
        Texture2D tempTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        tempTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        tempTexture.Apply();
        
        RenderTexture.active = previousRT;
        RenderTexture.ReleaseTemporary(tempRT);

        Color prominentColor = CalculateMostProminentColor(tempTexture);

        // Cleanup
        DestroyImmediate(tempTexture);

        // Optionally convert from linear to gamma color space if working in Linear color space
        if (PlayerSettings.colorSpace == ColorSpace.Linear)
        {
            prominentColor = prominentColor.gamma;
        }

        return prominentColor;
    }

    private static Color CalculateMostProminentColor(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        Dictionary<Color32, int> colorCount = new Dictionary<Color32, int>();

        foreach (Color32 color in pixels)
        {
            if (color.a == 0) continue; // Ignore transparent pixels
            
            // Pre-multiply color by alpha to handle transparency correctly
            Color32 adjustedColor = new Color32((byte)(color.r * color.a / 255), (byte)(color.g * color.a / 255), (byte)(color.b * color.a / 255), color.a);

            if (colorCount.ContainsKey(adjustedColor))
                colorCount[adjustedColor]++;
            else
                colorCount[adjustedColor] = 1;
        }

        int maxCount = 0;
        Color32 mostProminent = new Color32();
        foreach (KeyValuePair<Color32, int> pair in colorCount)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                mostProminent = pair.Key;
            }
        }

        // Convert to Color to handle normalized color values correctly
        return (Color)mostProminent;
    }
    #endif


    public static void Actuate(Block b)
    {
        // Get the type of the Block instance
        Type blockType = b.GetType();

        // Get all fields of the Block instance
        FieldInfo[] fields = blockType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        // Iterate through the fields
        foreach (FieldInfo field in fields)
        {
            // Check if the field is of type BlockUIButton
            if (field.FieldType == typeof(BlockUIButton))
            {
                // Get the value of the field
                BlockUIButton button = (BlockUIButton)field.GetValue(b);

                // Call the Click method on the button
                button.OnClick();
            }
        }
    }
}