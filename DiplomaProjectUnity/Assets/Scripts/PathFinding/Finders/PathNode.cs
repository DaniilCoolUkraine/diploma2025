namespace DiplomaProject.PathFinding.Finders
{
    public struct PathNode
    {
        public int X;
        public int Y;
        
        public int Index;
        public int PreviousIndex;

        public int gCost;
        public int hCost;
        public int fCost;
        
        public bool Walkable;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}