using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; 

public class GridBoard : SingletonMonobehaviour<GridBoard>
{
    [Header("Walkable")]
    [SerializeField] Tilemap _tilemapWalkable = null;
    [SerializeField] TileBase _tileWalkable = null;

    [Header("Spawns")]
    [SerializeField] Tilemap _tilemapSpawns = null;
    [SerializeField] TileBase _tileSpawnPacman = null;
    [SerializeField] TileBase _tileSpawnBlinky = null;
    [SerializeField] TileBase _tileSpawnPinky = null;
    [SerializeField] TileBase _tileSpawnInky = null;
    [SerializeField] TileBase _tileSpawnClyde = null;

    Dictionary<Vector3Int, bool> _walkableGridData;
    Dictionary<Character, Vector3Int> _spawnGridData;

    protected override void Awake() {
        base.Awake();
        if (!IsSingletonInstance()) {
            return;
        }

        SetupWalkableData();
        SetupSpawnData();
    }

    public Vector3Int GetPositionWorldToCell(Vector3 p_position) {
        return _tilemapWalkable.WorldToCell(p_position);
    }

    public Vector3 GetPositionCellToWorld(Vector3Int p_position) {
        return _tilemapWalkable.GetCellCenterWorld(p_position);
    }

    public Vector3 GetPositionWorldToWorld(Vector3 p_position) {
        return GetPositionCellToWorld(GetPositionWorldToCell(p_position));
    }

    #region Walkable
    void SetupWalkableData() {
        _walkableGridData = new Dictionary<Vector3Int, bool>();
        foreach (var position in _tilemapWalkable.cellBounds.allPositionsWithin) {
            if (!_tilemapWalkable.HasTile(position)) {
                continue;
            }

            if (_tilemapWalkable.GetTile(position) == _tileWalkable) {
                _walkableGridData.Add(position, true);
            }
        }
    }

    public bool IsTileWalkable(Vector3Int p_gridPosition) {
        if (_walkableGridData.ContainsKey(p_gridPosition)) {
            return _walkableGridData[p_gridPosition];
        }
        return false;
    }
    #endregion

    #region Spawn
    void SetupSpawnData() {
        _spawnGridData = new Dictionary<Character, Vector3Int>();
        foreach (var position in _tilemapSpawns.cellBounds.allPositionsWithin) {
            if (!_tilemapSpawns.HasTile(position)) {
                continue;
            }
            
            var tileBase = _tilemapSpawns.GetTile(position);
            if (tileBase == _tileSpawnPacman) {
                _spawnGridData.Add(Character.Pacman, position);
                continue;
            }
            if (tileBase == _tileSpawnBlinky) {
                _spawnGridData.Add(Character.Blinky, position);
                continue;
            }
            if (tileBase == _tileSpawnPinky) {
                _spawnGridData.Add(Character.Pinky, position);
                continue;
            }
            if (tileBase == _tileSpawnInky) {
                _spawnGridData.Add(Character.Inky, position);
                continue;
            }
            if (tileBase == _tileSpawnClyde) {
                _spawnGridData.Add(Character.Clyde, position);
                continue;
            }
        }
    }

    public Vector3Int GetSpawnCellPosition(Character p_character) {
        if (_spawnGridData.ContainsKey(p_character)) {
            return _spawnGridData[p_character];
        }
        return Vector3Int.zero;
    }
    #endregion
}
