using UnityEngine;

namespace ARPG.Core
{
    public interface IInteractable
    {
        float Distance
        {
            get;
        }

        bool Interact(GameObject other);
        void StopInteract(GameObject other);
    }
}
