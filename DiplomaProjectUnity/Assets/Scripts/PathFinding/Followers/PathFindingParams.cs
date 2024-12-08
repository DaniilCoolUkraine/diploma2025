using Unity.Entities;
using Unity.Mathematics;

namespace DiplomaProject.PathFinding.Followers
{
    public struct PathFindingParams : IBufferElementData
    {
        public int2 StartPosition;
        public int2 EndPosition;
    }
}