using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterGenerator))]
public class WaterGeneratorEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        WaterGenerator mapGen = (WaterGenerator)target;

        if (DrawDefaultInspector()) 
        {
            mapGen.GenerateWater();
        }

        if (GUILayout.Button ("Update")) 
        {
            mapGen.GenerateWater();
        }
    }
}
