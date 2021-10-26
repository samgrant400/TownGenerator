using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Den.Tools;
using TMPro;

namespace Twobob.Mm2
{
    public class KeyBind : MonoBehaviour
    {
        public float FloorSpeed = 5.0f;
        public KeyCode floorcode = KeyCode.F;

        public float JumpDistance = 500f;
        public KeyCode godscode = KeyCode.G;
        public KeyCode downscode = KeyCode.B;

        private bool Lerping = true;

        private Vector3 heightFudge = new Vector3(0, 1.1f, 0);

        public KeyCode nextcode = KeyCode.N;
        static float time = 5f;
        static float height = 3500f;
        Transform curpos;

        TextMeshProUGUI textMeshProUGUI;

        public KeyCode hugscode = KeyCode.H;
        public bool floorHug = true;
        public float HugVerticalOffset = 1.0f;

        void Update()
        {
            //Floor
            if (Input.GetKeyDown(floorcode))
            {
                Vector3 positionToMoveTo =
                    GetTerrainPos(transform.position.x, transform.position.z) + heightFudge;

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
                HugVerticalOffset += JumpDistance;
            }

            // Gods
            if (Input.GetKeyDown(downscode))
            {
                Lerping = false;
                transform.position += new Vector3(0, -JumpDistance, 0);
                HugVerticalOffset += -JumpDistance;
            }

            // HUG floor
            if (Input.GetKeyDown(hugscode))
            {
                floorHug = !floorHug;
            }

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                time = 5f;
                Debug.Log("AUTO TELEPORTING THRU ALL TOWNS");
                height = transform.position.y;
                this.JumpInvoked();
            }

            if (Input.GetKeyDown(nextcode))
            {
                forceJumps = false;
                DoJumps();
            }
        }

        public void GoTown(float chosenX, float chosenZ)
        {
            curpos.position = new Vector3(chosenX, 2500, chosenZ);

            Coord newPositionAsCoord = new Coord((int)(chosenX * 0.001), (int)(chosenZ * 0.001));

            // change it to the name if we have one.
            if (TownGlobalObject.townsData.ContainsKey(newPositionAsCoord))
                requestedPlacename = TownGlobalObject.townsData[newPositionAsCoord].name;

            KeyBind thing = this;
            thing.JumpFloor();
        }

        private void LateUpdate()
        {
            if (floorHug)
            {
                curpos = transform;
                curpos.position =
                    GetTerrainPosUnmasked(curpos.position.x, curpos.position.z)
                    + new Vector3(0, HugVerticalOffset, 0);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Code Quality",
            "IDE0051:Remove unused private members",
            Justification = "Invoke"
        )]
        private void TryToFloor()
        {
            curpos = transform;

            //  MapMagic.Terrains.TerrainTile tileFound = TownHolder.Instance.MapMagicObjectReference.tiles.FindByWorldPosition(curpos.position.x, curpos.position.z);

            //  float height = tileFound.ActiveTerrain.terrainData.GetHeight((int)curpos.position.x, (int)curpos.position.z);

            //curpos.position = new Vector3(, height + 1, curpos.position.z);
            curpos.position =
                GetTerrainPos(curpos.position.x, curpos.position.z) + new Vector3(0, 1, 0);

            curpos.gameObject.GetComponent<FlyCam>().rotationY = 0;

            textMeshProUGUI.text = requestedPlacename;
        }

        public void JumpFloor()
        {
            this.Invoke("TryToFloor", .5f);
        }

        public static string requestedPlacename = "Manual";

        public void CheckTownCodes()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                forceJumps = true;
                DoJumps();
            }
        }

        private bool forceJumps = false;

        public void JumpInvoked()
        {
            forceJumps = true;
            this.Invoke("DoJumps", time);
            Debug.LogFormat(
                "Jumping to Town {0} {1}",
                TownGlobalObject.LastPreviewedTownId,
                TownGlobalObject.NextTownPreviewName
            );
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Code Quality",
            "IDE0051:Remove unused private members",
            Justification = "Invoke"
        )]
        private void DoJumps()
        {
            bool jumponce = !forceJumps;

            TownGlobalObject.PreviewActive = true;
            curpos = transform;

            Den.Tools.Coord mine = new Den.Tools.Coord(
                (int)(curpos.position.x * 0.001f),
                (int)(curpos.position.z * 0.001f)
            );

            //  var locality = TownGlobalObject.GetIndexAtCoord(mine);

            var sortedDict =
                from entry in TownGlobalObject.townsData
                orderby entry.Value.Patches.Count ascending
                select entry;

            TownGlobalObject.NextTownPreviewName =
                sortedDict.ElementAt(TownGlobalObject.LastPreviewedTownId).Value.name;

            var CoordToGoTo = sortedDict.ElementAt(TownGlobalObject.LastPreviewedTownId).Key;
            TownGlobalObject.LastPreviewedTownId = TownGlobalObject.LastPreviewedTownId + 1;

            float offsetter = 0;

            if (jumponce)
            {
                offsetter = transform.position.y;
            }
            else
            {
                offsetter = height;
            }

            var newvec = new Vector3(CoordToGoTo.x * 1000, offsetter, CoordToGoTo.z * 1000);

            Debug.Log("going to " + TownGlobalObject.NextTownPreviewName);

            curpos.position = newvec;

            KeyBind thing = this; // curpos.gameObject.GetComponent<KeyBind>();
            if (TownGlobalObject.LastPreviewedTownId >= sortedDict.Count())
            {
                TownGlobalObject.LastPreviewedTownId = 0;
                TownGlobalObject.PreviewActive = false;
                return;
            }
            else if (!jumponce)
            {
                thing.JumpInvoked();
            }
        }

        static Vector3 GetTerrainPos(float x, float y) // Get the default layer // The actual terrain. Ignoring Objects.
        {
            string mask = "Default";
            return GetTerrainPosLayered(x, y, mask);
        }

        static Vector3 GetTerrainPosUnmasked(float x, float y) // The terrain. Including Objects.
        {
            return GetTerrainPosLayered(x, y, null);
        }

        static Vector3 GetTerrainPosMasked(float x, float y, string layername) // A specific Layer arrangement... Should include terrain
        {
            return GetTerrainPosLayered(x, y, null);
        }

        static Vector3 GetTerrainPosLayered(float x, float y, string maskname) // The actual terrain. Ignoring Objects.
        {
            //Create object to store raycast data

            //Create origin for raycast that is above the terrain. I chose 500.
            Vector3 origin = new Vector3(x, 500f, y);

            //Send the raycast.
            // Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 501f);


            // TODO : MASK SELECTION
            //  LayerMask mask = LayerMask.GetMask("Default");

            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit foundhit;

            if (string.IsNullOrEmpty(maskname))
            {
                Physics.Raycast(ray, out RaycastHit hit, 501f);
                foundhit = hit;
            }
            else
            {
                LayerMask mask = LayerMask.GetMask(maskname);
                Physics.Raycast(ray, out RaycastHit hit, 501f, mask);
                foundhit = hit;
            }

            //  Debug.Log("Terrain location found at " + hit.point);
            return foundhit.point;
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
