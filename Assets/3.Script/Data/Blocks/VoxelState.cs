using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class VoxelState
{

    public byte id;
    public int orientation;
    
    //[System.NonSerialized] private byte _light;
    public float globalLightPercent;

    [System.NonSerialized] public ChunkData chunkData;

    [System.NonSerialized] public VoxelNeighbours neighbours;
    [System.NonSerialized] public Vector3Int position;

    

    public VoxelState(byte _id, ChunkData _chunkData, Vector3Int _position)
    {

        id = _id;
        orientation = 1;
        chunkData = _chunkData;

        neighbours = new VoxelNeighbours(this);
        position = _position;
        globalLightPercent = 0f;

    }

    public Vector3Int globalPosition
    {

        get
        {

            return new Vector3Int(position.x + chunkData.position.x, position.y, position.z + chunkData.position.y);

        }

    }


    public BlockType properties
    {

        get { return World.Instance.blockTypes[id]; }

    }

}

public class VoxelNeighbours
{

    public readonly VoxelState parent;
    public VoxelNeighbours(VoxelState _parent) { parent = _parent; }

    private VoxelState[] _neighbours = new VoxelState[6];

    public int Length { get { return _neighbours.Length; } }

    public VoxelState this[int index]
    {

        get
        {

            // If the requested neighbour is null, attempt to get it from WorldData.GetVoxel.
            if (_neighbours[index] == null)
            {

                _neighbours[index] = World.Instance.worldData.GetVoxel(parent.globalPosition + VoxelData.faceChecks[index]);
                ReturnNeighbour(index);

            }

            // Return whatever we have. If it's null at this point, it means that neighbour doesn't exist yet.
            return _neighbours[index];

        }
        set
        {
            _neighbours[index] = value;
            ReturnNeighbour(index);
        }

    }

    void ReturnNeighbour(int index)
    {

        if (_neighbours[index] == null)
            return;

        if (_neighbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] != parent)
            _neighbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] = parent;

    }

}
