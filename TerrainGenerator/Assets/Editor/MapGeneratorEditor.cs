using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator) target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        mapGen.mapSize = EditorGUILayout.IntSlider("Map size:", mapGen.mapSize, 100, 1000);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        mapGen.autoUpdate = GUILayout.Toggle(mapGen.autoUpdate, "Auto update");
        mapGen.generateWater = GUILayout.Toggle(mapGen.generateWater, "Generate water");
        mapGen.island = GUILayout.Toggle(mapGen.island, "Make island");

        if (mapGen.island)
        {
            mapGen.islandSizeMultiplier = EditorGUILayout.Slider("Island size:", mapGen.islandSizeMultiplier, .5f, 2);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Point sampling", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        mapGen.distributionData.distributionType =
            (DistributionType) EditorGUILayout.EnumPopup("Vertex distribution type:",
                mapGen.distributionData.distributionType);

        switch (mapGen.distributionData.distributionType)
        {
            case DistributionType.Random:
                mapGen.distributionData.pointDensity = EditorGUILayout.IntSlider("Point density:",
                    mapGen.distributionData.pointDensity, 500, 1000);
                break;
            case DistributionType.Poisson:
                mapGen.distributionData.radius =
                    EditorGUILayout.Slider("Radius between vertex:", mapGen.distributionData.radius, 7f, 30f);
                mapGen.distributionData.rejectionSamples = EditorGUILayout.IntSlider("Rejection samples:",
                    mapGen.distributionData.rejectionSamples, 5, 50);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Noise settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        mapGen.noiseData.meshHeightMultiplier =
            EditorGUILayout.Slider("Mesh height:", mapGen.noiseData.meshHeightMultiplier, 0.0001f, 300);
        mapGen.noiseData.noiseScale =
            EditorGUILayout.Slider("Noise scale:", mapGen.noiseData.noiseScale, 0.0001f, 10000f);
        mapGen.noiseData.meshHeightCurve =
            EditorGUILayout.CurveField("Height curve:", mapGen.noiseData.meshHeightCurve);
        mapGen.noiseData.octaves = EditorGUILayout.IntSlider("Octaves:", mapGen.noiseData.octaves, 1, 5);
        mapGen.noiseData.persistenceType =
            (PersistenceType) EditorGUILayout.EnumPopup("Persistence type:", mapGen.noiseData.persistenceType);
        mapGen.noiseData.lacunarity = EditorGUILayout.Slider("Lacunarity:", mapGen.noiseData.lacunarity, 1f, 5f);
        mapGen.noiseData.seed = EditorGUILayout.IntField("Seed:", mapGen.noiseData.seed);
        mapGen.noiseData.offset = EditorGUILayout.Vector2Field("Offset:", mapGen.noiseData.offset);
        EditorGUILayout.Space();

        if (mapGen.generateWater)
        {
            EditorGUILayout.LabelField("Water settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            mapGen.waterGenerator =
                EditorGUILayout.ObjectField("Water generator:", mapGen.waterGenerator, typeof(WaterGenerator), true) as
                    WaterGenerator;
            mapGen.waterLevel = EditorGUILayout.Slider("Water level:", mapGen.waterLevel, -2f, 5f);
            EditorGUILayout.Space();
        }

        if (mapGen.autoUpdate && EditorGUI.EndChangeCheck()||GUILayout.Button ("Generate"))
        {
            mapGen.GenerateMap ();
        }
    }
}
