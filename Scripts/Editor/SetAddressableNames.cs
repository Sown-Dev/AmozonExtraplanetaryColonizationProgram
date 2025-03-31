using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class SetAddressableNames
{
    [MenuItem("Tools/Addressables/Set Names to Prefab Names")]
    public static void SetPrefabNames()
    {
        // Get Addressable settings
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable settings not found. Make sure Addressables is initialized.");
            return;
        }

        foreach (Object obj in Selection.objects)
        {
            if (obj is GameObject prefab)
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), settings.DefaultGroup);
                if (entry != null)
                {
                    entry.SetAddress(prefab.name); // Set Addressable name to Prefab name
                    Debug.Log($"Set Addressable name for {prefab.name}");
                }
            }
        }

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
        AssetDatabase.SaveAssets();
        Debug.Log("Finished setting Addressable names!");
    }
}