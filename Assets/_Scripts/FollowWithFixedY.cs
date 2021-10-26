using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWithFixedY : MonoBehaviour
{
    public Transform thing;

    public float fixedY = 800f;

    private Vector3 place;

    public bool trackPlayerPostion = true;

    public bool rotateWithPlayer = false;

    private void OnEnable()
    {
        place = thing.position;
    }

    void LateUpdate()
    {
        if (trackPlayerPostion)
        {
            place = thing.position;

            transform.position = new Vector3(place.x, fixedY, place.z);
        }

        if (!rotateWithPlayer)
            return;

        transform.eulerAngles = new Vector3(90, thing.rotation.eulerAngles.y, 0);
    }
}
