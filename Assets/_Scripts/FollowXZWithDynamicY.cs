using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowXZWithDynamicY : MonoBehaviour
{
    public Transform thingXZ;

    public Transform thingY;

    public float Y_Offset;

    private Vector3 placeXZ;

    private Vector3 placeY;

    public bool trackPlayerPostion = true;

    public bool rotateWithPlayer = false;

    private void OnEnable()
    {
        placeXZ = thingXZ.position;
        placeY = thingY.position;
    }

    void LateUpdate()
    {
        if (trackPlayerPostion)
        {
            placeXZ = thingXZ.position;
            placeY = thingY.position;

            transform.position = new Vector3(placeXZ.x, placeY.y + Y_Offset, placeXZ.z);
        }

        if (!rotateWithPlayer)
            return;

        transform.eulerAngles = new Vector3(90, thingXZ.rotation.eulerAngles.y, 0);
    }
}
