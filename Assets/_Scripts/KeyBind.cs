using UnityEngine;
using System.Collections;

namespace Twobob.Mm2
{

    public class KeyBind : MonoBehaviour
    {
        public float FloorSpeed = 5.0f;
        public KeyCode floorcode = KeyCode.F;

        public float JumpDistance = 500f;
        public KeyCode godscode = KeyCode.G;

        private bool Lerping = true;

        private Vector3 heightFudge = new Vector3(0, 1.1f, 0);

        void Update()
        {
            //Floor
            if (Input.GetKeyDown(floorcode))
            {
                Vector3 positionToMoveTo = GetTerrainPos(transform.position.x, transform.position.z) + heightFudge;


                if (positionToMoveTo.y > 0)
                {
                    Lerping = true;

                    StartCoroutine(LerpPosition(positionToMoveTo, FloorSpeed));
                }

            }

            // Gods
            if (Input.GetKeyDown(godscode))
            {
                Lerping = false;
               transform.position += new Vector3(0, JumpDistance, 0);

            }

           

        }
        static Vector3 GetTerrainPos(float x, float y)  // The actual terrain. Ignoring Objects.
        {

            //Create object to store raycast data

            //Create origin for raycast that is above the terrain. I chose 500.
            Vector3 origin = new Vector3(x, 500f, y);

            //Send the raycast.
            // Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 501f);


            // TODO : MASK SELECTION
            LayerMask mask = LayerMask.GetMask("Default");

            Ray ray = new Ray(origin, Vector3.down);


            Physics.Raycast(ray, out RaycastHit hit, 501f, mask);


            //  Debug.Log("Terrain location found at " + hit.point);
            return hit.point;

        }

        IEnumerator LerpPosition(Vector3 targetPosition, float duration)
        {

            float time = 0;
            Vector3 startPosition = transform.position;

            while (time < duration && Lerping)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
        }
    }
}