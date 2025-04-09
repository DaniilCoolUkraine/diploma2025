namespace DiplomaProject.Goap.Strategies
{
    public interface IActionStrategy
    {
        bool CanPerform { get; }
        bool Complete { get; }

        void Start()
        {
        }

        void Update(float deltaTime)
        {
        }

        void Stop()
        {
        }
    }
}