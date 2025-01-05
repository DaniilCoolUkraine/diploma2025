namespace DiplomaProject.BehTree.Strategies
{
    public interface IStrategy
    {
        public Node.Status Process();
        public void Reset(){}
    }
}