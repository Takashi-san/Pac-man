using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; 

public class GridBoard : SingletonMonobehaviour<GridBoard>
{
    [Header("Walkable")]
    [SerializeField] Tilemap _tilemapWalkable = null;
    [SerializeField] TileBase _tileWalkable = null;
    [SerializeField] TileBase _tileRespawnWall = null;
    [SerializeField] TileBase _tileTeleport = null;

    [Header("Spawns")]
    [SerializeField] Tilemap _tilemapSpawns = null;
    [SerializeField] TileBase _tileSpawnPacman = null;
    [SerializeField] TileBase _tileSpawnBlinky = null;
    [SerializeField] TileBase _tileSpawnPinky = null;
    [SerializeField] TileBase _tileSpawnInky = null;
    [SerializeField] TileBase _tileSpawnClyde = null;

    [Header("Scatter")]
    [SerializeField] Tilemap _tilemapScatter = null;
    [SerializeField] TileBase _tileScatterBlinky = null;
    [SerializeField] TileBase _tileScatterPinky = null;
    [SerializeField] TileBase _tileScatterInky = null;
    [SerializeField] TileBase _tileScatterClyde = null;
    [SerializeField] TileBase _tileRespawn = null;

    Dictionary<Vector3Int, TerrainType> _walkableGridData;
    Dictionary<Character, Vector3Int> _spawnGridData;
    Dictionary<Character, Vector3Int> _scatterGridData;
    Vector3Int _respawnGridData;

    protected override void Awake() {
        base.Awake();
        if (!IsSingletonInstance()) {
            return;
        }

        SetupWalkableData();
        SetupSpawnData();
        SetupScatterData();
    }

    #region Position
    public Vector3Int GetPositionWorldToCell(Vector3 p_position) {
        return _tilemapWalkable.WorldToCell(p_position);
    }

    public Vector3 GetPositionCellToWorld(Vector3Int p_position) {
        return _tilemapWalkable.GetCellCenterWorld(p_position);
    }

    public Vector3 GetPositionWorldToWorld(Vector3 p_position) {
        return GetPositionCellToWorld(GetPositionWorldToCell(p_position));
    }
    #endregion

    #region Walkable
    void SetupWalkableData() {
        _walkableGridData = new Dictionary<Vector3Int, TerrainType>();
        foreach (var position in _tilemapWalkable.cellBounds.allPositionsWithin) {
            if (!_tilemapWalkable.HasTile(position)) {
                continue;
            }

            if (_tilemapWalkable.GetTile(position) == _tileWalkable) {
                _walkableGridData.Add(position, TerrainType.Walkable);
                continue;
            }
            if (_tilemapWalkable.GetTile(position) == _tileRespawnWall) {
                _walkableGridData.Add(position, TerrainType.RespawnWall);
            }
        }
    }

    public bool IsTileWalkable(Vector3Int p_gridPosition, List<TerrainType> p_allowed) {
        if (_walkableGridData.ContainsKey(p_gridPosition)) {
            return p_allowed.Contains(_walkableGridData[p_gridPosition]);
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

    public Vector3 GetSpawnWorldPosition(Character p_character) {
        return GetPositionCellToWorld(GetSpawnCellPosition(p_character));
    }

    public Vector3Int GetRespawnCellPosition() {
        return _respawnGridData;
    }
    #endregion

    #region Scatter
    void SetupScatterData() {
        _scatterGridData = new Dictionary<Character, Vector3Int>();
        foreach (var position in _tilemapScatter.cellBounds.allPositionsWithin) {
            if (!_tilemapScatter.HasTile(position)) {
                continue;
            }
            
            var tileBase = _tilemapScatter.GetTile(position);
            if (tileBase == _tileScatterBlinky) {
                _scatterGridData.Add(Character.Blinky, position);
                continue;
            }
            if (tileBase == _tileScatterPinky) {
                _scatterGridData.Add(Character.Pinky, position);
                continue;
            }
            if (tileBase == _tileScatterInky) {
                _scatterGridData.Add(Character.Inky, position);
                continue;
            }
            if (tileBase == _tileScatterClyde) {
                _scatterGridData.Add(Character.Clyde, position);
                continue;
            }
            if (tileBase == _tileRespawn) {
                _respawnGridData = position;
                continue;
            }
        }
    }

    public Vector3Int GetScatterCellPosition(Character p_character) {
        if (_scatterGridData.ContainsKey(p_character)) {
            return _scatterGridData[p_character];
        }
        return Vector3Int.zero;
    }

    public Vector3 GetScatterWorldPosition(Character p_character) {
        return GetPositionCellToWorld(GetScatterCellPosition(p_character));
    }
    #endregion
}
