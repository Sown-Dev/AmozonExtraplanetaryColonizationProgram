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

    public GameObject EmptyUI;
    
    [SerializeField] public ItemDrop itemDropPrefab;

    private void Awake(){
        Instance = this;
    }
    public static Vector3 SnapToGrid(Vector3 position, float gridSize = 1f / 16f)
    {
        position.x = Mathf.Round(position.x / gridSize) * gridSize;
        position.y = Mathf.Round(position.y / gridSize) * gridSize;
        return position;
    }
    
    public ItemDrop CreateItemDrop(ItemStack item, Vector3 pos){
        ItemDrop drop = Instantiate(itemDropPrefab, pos + (Vector3)(Random.insideUnitCircle*0.2f), Quaternion.identity);
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

/*    
    public void GenerateHighlights(Vector2 origin, List<Vector2> positions, Transform parent){
        foreach (Vector2 pos in positions){
            //generate highlight: sprite if not ui, otherwise image
            
            GameObject highlight = Instantiate(highlightSprite, parent);
            highlight.transform.position = pos + origin;
            //set color if block is valid and no wall
            if (TerrainManager.Instance.GetBlock(Vector2Int.RoundToInt(origin + pos)) == null && !TerrainManager.Instance.IsWall(Vector3Int.RoundToInt(origin + pos))){
                highlight.GetComponent<SpriteRenderer>().color = new Color(0.5f, 1, 0,0.75f);

            }
            else{
                highlight.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.1f, 0.1f, 0.7f);
            }
            
        }
    }*/


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
    
    public static Color GenerateUniqueColor(object obj)
    {
        // Serialize the fields of the class
        string serializedFields = SerializeObjectFields(obj);

        // Hash the serialized string
        int hash = serializedFields.GetHashCode();

        // Convert the hash to RGB values
        return HashToColor(hash);
    }

    // Serialize object fields into a string
    private static string SerializeObjectFields(object obj)
    {
        Type type = obj.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(obj);
            sb.Append(field.Name);
            sb.Append(value != null ? value.ToString() : "null");
        }

        return sb.ToString();
    }

    // Convert hash to an RGB color
    private static Color HashToColor(int hash)
    {
        // Generate color components using bit manipulation
        float r = ((hash >> 16) & 0xFF) / 255f;
        float g = ((hash >> 8) & 0xFF) / 255f;
        float b = (hash & 0xFF) / 255f;

        return new Color(r, g, b);
    }
    #if UNITY_EDITOR
    public static string GetResourcesPath(UnityEngine.Object asset)
    {
        string fullPath = AssetDatabase.GetAssetPath(asset); // e.g., "Assets/Resources/Sprites/Upgrades/myIcon.png"
        int index = fullPath.IndexOf("Resources/");
        if (index < 0)
        {
            Debug.LogWarning("Asset is not in a Resources folder!");
            return null;
        }
        // Extract path after "Resources/"
        string relativePath = fullPath.Substring(index + "Resources/".Length);
        // Remove extension
        relativePath = System.IO.Path.ChangeExtension(relativePath, null);
        return relativePath;
    }
#endif
    
    
    //Draw Arrow
    public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.DrawRay(pos, direction);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}

	public static void ArrowGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.color = color;
		Gizmos.DrawRay(pos, direction);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}

	public static void ArrowDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength);
		Debug.DrawRay(pos + direction, left * arrowHeadLength);
	}
	public static void ArrowDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction, color);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
		Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
	}
}