using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackYComponent : MonoBehaviour
{
    public Transform thing;

    private Vector3 place;

    public bool trackPosition = true;

    public float offset = 0;

    private void OnEnable()
    {
        place = thing.position;
    }

    void LateUpdate()
    {
        if (trackPosition)
        {
            place = thing.position;

            transform.position = new Vector3(
                transform.position.x,
                place.y + offset,
                transform.position.z
            );
        }
    }
}
