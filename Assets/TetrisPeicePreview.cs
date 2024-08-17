using System.Security.Claims;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TetrisPeicePreview : MonoBehaviour
{
    [SerializeField] private Vector3Int position;

    private Tilemap tilemap;
    private Vector3Int[] cells;

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    public void UpdatePrivew(TetrominoData data)
    {
        Clear();
        Copy(data);
        Set(data);
    }

    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy(TetrominoData data)
    {
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Set(TetrominoData data)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, data.tile);
        }
    }
}
