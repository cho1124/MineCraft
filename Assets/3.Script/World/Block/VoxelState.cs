using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxelState
{
    public byte id;

    [System.NonSerialized]
    private byte _light;

    [System.NonSerialized]
    public ChunkData chunkData;

    [System.NonSerialized]
    public VoxelNeighbours neighbours;
    [System.NonSerialized]
    public Vector3Int position;

    public byte light
    {

        get { return _light; }

        set
        {

            if(value != _light)
            {

                byte oldLightValue = _light;
                byte oldCastValue = castLight;

                _light = value;

                if (_light < oldLightValue)
                {
                    List<int> neigboursToDarken = new List<int>();

                    for(int p = 0; p<6;p++)
                    {
                        if(neighbours[p] != null)
                        {


                            if (neighbours[p].light <= oldCastValue)
                            {
                                neigboursToDarken.Add(p);
                            }

                            else
                            {
                                neighbours[p].PropogateLight();
                            }




                        }
                    }

                    foreach(int i in neigboursToDarken)
                    {
                        neighbours[i].light = 0;
                    }

                    if (chunkData.chunk != null)
                        World.Instance.AddChunkToUpdate(chunkData.chunk);

                }
                else if (_light > 1)
                    PropogateLight();
            }





        }
    }




    public VoxelState(byte _id, ChunkData _chunkData, Vector3Int _position)
    {
        id = _id;
        chunkData = _chunkData;
        neighbours = new VoxelNeighbours(this);
        position = _position;
        light = 0;

    }

    public Vector3Int globalPosition
    {
        get
        {
            return new Vector3Int(position.x + chunkData.position.x, position.y, position.z + chunkData.position.y);
        }
    }

    public float lightAsFloat
    {
        get { return (float)light * VoxelData.unitOfLight; }
    }

    public byte castLight
    {
        get
        {



            int lightLevel = _light - properties.opacity - 1;
            if (lightLevel < 0) lightLevel = 0;
            return (byte)lightLevel;
        }


    }

    public void PropogateLight()
    {

        if (light < 2)
            return;

        for (int p = 0; p < 6; p++)
        {
            if (neighbours[p] != null)
            {

                if (neighbours[p].light < castLight)
                    neighbours[p].light = castLight;



            }

            if (chunkData.chunk != null)
                World.Instance.AddChunkToUpdate(chunkData.chunk);

        }

    }


    public BlockType properties
    {
        get
        {
            return World.Instance.blockTypes[id];
        }

    }

}

public class VoxelNeighbours
{
    public readonly VoxelState parent;
    public VoxelNeighbours (VoxelState _parent) { parent = _parent; }

    private VoxelState[] _neighbours = new VoxelState[6];


    public int Length { get { return _neighbours.Length; } }

    public VoxelState this[int index]
    {
        get { 
            
            if(_neighbours[index] == null)
            {
                _neighbours[index] = World.Instance.worldData.GetVoxel(parent.globalPosition + VoxelData.faceChecks[index]);
                ReturnNeighbour(index);
            }
            return _neighbours[index]; 
        }
        set { 
            _neighbours[index] = value;
            ReturnNeighbour(index);
        }

    }
    void ReturnNeighbour (int index)
    {
        if (_neighbours[index] == null)
            return;

        if (_neighbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] != parent)
            _neighbours[index].neighbours[VoxelData.revFaceCheckIndex[index]] = parent;
    }

}