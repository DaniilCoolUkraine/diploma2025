using DiplomaProject.EventSystem.Core;
using UnityEngine;

namespace DiplomaProject.EventSystem.Extendables
{
    public class RefillEvent : IEvent
    {
        public Transform EntityToRefill { get; private set; }

        public RefillEvent(Transform interactor)
        {
            EntityToRefill = interactor;
        }
    }
}