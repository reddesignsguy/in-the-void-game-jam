using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    protected Door _door;

    protected virtual void OpenDoor()
    {
        if (_door == null)
        {
            Debug.LogError("Door not initialized to door trigger");
            return;
        }

        _door.OpenDoor();
    }

    protected virtual void CloseDoor()
    {
        if (_door == null)
        {
            Debug.LogError("Door not initialized to door trigger");
            return;
        }

        _door.CloseDoor();
    }
}
