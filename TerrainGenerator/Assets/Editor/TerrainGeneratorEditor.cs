using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator script = (TerrainGenerator)target;
        
        if(DrawDefaultInspector())
        {
            script.Init();
        }

        /*
        switch (script.distribution)
        {
            case Distribution.RANDOM:
                script.pointDensity = EditorGUILayout.IntSlider("Point density:", script.pointDensity , 4 , 6000);
                break;
            case Distribution.POISSON:
                script.radius = EditorGUILayout.Slider("Per point radius:", script.radius , 10 , 150);
                script.rejectionSamples = EditorGUILayout.IntSlider("Pejection samples:", script.rejectionSamples , 5 , 50);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (script.distribution == Distribution.RANDOM)
        {
          
        }
        */

        if (GUILayout.Button("New Seed"))
        {
            script.seed = Random.Range(0, 100);
            script.Init();
        }

        if (GUILayout.Button("Update"))
        {
            script.Init();
        }
    }
    
}
