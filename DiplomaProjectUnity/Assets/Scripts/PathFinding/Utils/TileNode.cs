namespace DiplomaProject.PathFinding.Utils
{
    [System.Serializable]
    public class TileNode
    {
        public int X;
        public int Y;
        public bool IsWalkable;
        public bool IsGoal;
        
        public TileNode(int x, int y)
        {
            X = x;
            Y = y;
            
            IsWalkable = true;
            IsGoal = false;
        }

        public TileNode(TileNode other)
        {
            X = other.X;
            Y = other.Y;
            
            IsWalkable = other.IsWalkable;
            IsGoal = other.IsGoal;
        }

        public void SwitchWalkable()
        {
            IsWalkable = !IsWalkable;
        }
    }
}