using DiplomaProject.EventSystem.Core;
using DiplomaProject.EventSystem.Extendables;
using UnityEngine;

namespace DiplomaProject.Interactable
{
    public class RefillInteractable : MonoBehaviour, IInteractable
    {
        public void Interact(Transform interactor)
        {
            GlobalEvents.Publish<RefillEvent>(new RefillEvent(interactor));
        }
    }
}