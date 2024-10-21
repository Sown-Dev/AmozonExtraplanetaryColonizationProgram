using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class ToolTippableEditor : Editor
{
    private IToolTippable toolTippable { get { return (target as IToolTippable); } }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        if (toolTippable != null && toolTippable.icon != null)
        {
            Type t = GetType("UnityEditor.SpriteUtility");
            if (t != null)
            {
                MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                if (method != null)
                {
                    object ret = method.Invoke(null, new object[] { toolTippable.icon, Color.white, width, height });
                    if (ret is Texture2D)
                        return ret as Texture2D;
                }
            }
        }
        return base.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    private static Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null)
            return type;

        if (typeName.Contains("."))
        {
            var assemblyName = typeName.Substring(0, typeName.IndexOf('.'));
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (toolTippable != null)
        {
            EditorGUILayout.LabelField("Name:", toolTippable.name);
            EditorGUILayout.LabelField("Description:", toolTippable.description);
        }
    }
}
