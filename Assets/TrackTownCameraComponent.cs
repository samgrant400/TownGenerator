using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTownCameraComponent : MonoBehaviour
{
    private Transform thing;

    private Vector3 place;

    public bool trackPosition = true;

    public float offset = 0;

    private void OnEnable()
    {
        thing = TownCameraComponent.TownCamera;

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

public static class TownCameraComponent
{
    private static Transform cam;

    public static Transform TownCamera
    {
        get
        {
            if (cam == null)
            {
                cam = GameObject.Find("TownCamera").transform;
            }
            return cam;
        }
        private set { }
    }
}


