using DiplomaProject.EventSystem.Core;

namespace DiplomaProject.EventSystem.Extendables
{
    public class FireEvent : IEvent
    {
        public int AmmoCount { get; private set; }

        public FireEvent(int ammoCount)
        {
            AmmoCount = ammoCount;
        }
    }
}