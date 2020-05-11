using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float MAX_VIEW_DST = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    private int _chunkSize;
    private int _chunkVisibleInViewDst;
    
    Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        _chunkSize = MapGenerator.MAP_CHUNK_SIZE - 1;
        _chunkVisibleInViewDst = Mathf.RoundToInt(MAX_VIEW_DST / _chunkSize);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisible();
    }

    void UpdateVisible()
    {
        foreach (var t in terrainChunksVisibleLastUpdate)
        {
            t.SetVisible (false);
        }
        terrainChunksVisibleLastUpdate.Clear ();
        
        Vector2 currentChunkCoords = new Vector2(Mathf.RoundToInt(viewerPosition.x / _chunkSize), Mathf.RoundToInt(viewerPosition.y / _chunkSize));

        for (int y = -_chunkVisibleInViewDst; y <= _chunkVisibleInViewDst; y++)
        {
            for (int x  = -_chunkVisibleInViewDst; x <= _chunkVisibleInViewDst; x++)
            {
                Vector2 viewedChunkCoords = new Vector2(currentChunkCoords.x + x, currentChunkCoords.y + y);

                if (_terrainChunkDictionary.ContainsKey(viewedChunkCoords))
                {
                    _terrainChunkDictionary[viewedChunkCoords].UpdateTerrainChunk ();
                    if (_terrainChunkDictionary[viewedChunkCoords].IsVisible ()) {
                        terrainChunksVisibleLastUpdate.Add (_terrainChunkDictionary[viewedChunkCoords]);
                    }
                }
                else
                {
                    _terrainChunkDictionary.Add (viewedChunkCoords, new TerrainChunk(viewedChunkCoords, _chunkSize, transform));
                }
            }
        }
    }
    
    public class TerrainChunk
    {
        private GameObject _meshObject;
        private Vector2 _position;
        private Bounds _bounds;

        public TerrainChunk(Vector2 coords, int size, Transform parent)
        {
            _position = coords * size;
            _bounds = new Bounds(_position, Vector2.one * size);
            Vector3 v3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _meshObject.transform.position = v3;
            _meshObject.transform.parent = parent;
            _meshObject.transform.localScale = Vector3.one * size /10f;
            SetVisible(false);
        }

        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= MAX_VIEW_DST;
            SetVisible (visible);
        }

  
        public void SetVisible(bool visible) {
            _meshObject.SetActive (visible);
        }

        public bool IsVisible() {
            return _meshObject.activeSelf;
        }
    }
}


