using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager StaticMapManager;

    [SerializeField] private Tilemap Map;

    [SerializeField] private List<TileData> TileDatas;

    private Dictionary<TileBase, TileData> DataFromTiles;

    void Awake()
    {
        if (StaticMapManager == null)
        {
            DontDestroyOnLoad(gameObject);
            StaticMapManager = this;

            DataFromTiles = new Dictionary<TileBase, TileData>();

            foreach (var TileData in TileDatas)
            {
                foreach (var tile in TileData.tiles)
                {
                    DataFromTiles.Add(tile, TileData);
                }
            }
        }
        else if (StaticMapManager != this)
        {
            Destroy(gameObject);
        }
    }

    public string GetTileType(GameObject tile)
    {
        Vector3Int gridPosition = Map.WorldToCell(tile.transform.position);
        TileBase fishingTile = Map.GetTile(gridPosition);
        return DataFromTiles[fishingTile].watertype;
    }
}
