using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelGenerator2D))]
public class LevelGenerator2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator2D LG = (LevelGenerator2D)target;

        if (GUILayout.Button("Generate LAND"))
        {
            LG.Generate();
        }

        if (GUILayout.Button("add border variety"))
        {
            LG.AddBorderVarietyBiomes();
        }

        if (GUILayout.Button("spawn TREES"))
        {
            LG.SpawnTrees();
        }

        if (GUILayout.Button("Clear"))
        {
            LG.Clear();
        }
    }
}
#endif

