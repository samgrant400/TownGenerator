using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Den.Tools;
using TMPro;

namespace Twobob.Mm2
{

    public class MapKeyBind : MonoBehaviour
    {

        public KeyCode minuscode = KeyCode.KeypadMinus;
        public KeyCode pluscode = KeyCode.KeypadPlus;
        private bool Lerping;
      


        
        float Movement = 100f;

        float MoveSpeed = 1f;

        Camera cam;
        FollowWithFixedY fixedY;

        float minSize = 100;
        float maxSize = 620;

        Position currentPosition = Position.Wide;

        enum Position   { Close, Mid, Wide, World1, World2, World3, }
        enum RequestType { Cam, Follow }

        /// <summary>
        /// Allows encoding of multiple linked ranks of data without a dictionary
        /// </summary>
        /// <param name="pos">At which Position do you want the data looked up from</param>
        /// <param name="rank">Specify the Type of Request data you want lookup</param>
        /// <returns>The correct rank of looked-up data for the position (float)</returns>
        float  GetPositonFromEnum(Position pos, RequestType rank) {

            
            return pos switch
            {

            
                Position.Close => new float[]{100f,320f }[(int)rank],
                Position.Mid => new float[] { 320f, 500f }[(int)rank],
                Position.Wide => new float[] { 620f, 1500f }[(int)rank],
                Position.World1 => new float[] { 3000f, 2900f }[(int)rank],
                Position.World2 => new float[] { 6000f, 2900f }[(int)rank],
                Position.World3 => new float[] { 9000f, 2900f }[(int)rank],
                _ => new float[] { 620f, 620f }[(int)rank],
            };
        }

       void ChangeCurrentPosition(int val)
        {

            currentPosition += val;

            if (currentPosition < 0)
            {
                currentPosition = Position.Close;
            }
            if (currentPosition > Position.World3)
            {
                currentPosition = Position.World3;
            }


        }


        private void Start()
        {
            fixedY = gameObject.GetComponent<FollowWithFixedY>();

            cam = gameObject.GetComponent<Camera>();
            cam.orthographicSize = GetPositonFromEnum(currentPosition,RequestType.Cam);
        }

        public void LerpUp() {

            Lerping = false;

            ChangeCurrentPosition(1);

            StartCoroutine(LerpCamSize(GetPositonFromEnum(currentPosition, RequestType.Cam), MoveSpeed));
            StartCoroutine(LerpFollowHeight(GetPositonFromEnum(currentPosition, RequestType.Follow), MoveSpeed));

        }

        public void LerpDown() {

            Lerping = false;

            ChangeCurrentPosition(-1);

            StartCoroutine(LerpCamSize(GetPositonFromEnum(currentPosition, RequestType.Cam), MoveSpeed));
            StartCoroutine(LerpFollowHeight(GetPositonFromEnum(currentPosition, RequestType.Follow), MoveSpeed));

        }


        public void LerpLoop()
        {

            Lerping = false;

            
            if (currentPosition == Position.Close)
            {
                currentPosition = Position.World3;
            }
            else
            {
                ChangeCurrentPosition(-1);
            }


            StartCoroutine(LerpCamSize(GetPositonFromEnum(currentPosition, RequestType.Cam), MoveSpeed));
            StartCoroutine(LerpFollowHeight(GetPositonFromEnum(currentPosition, RequestType.Follow), MoveSpeed));

        }



        void Update()
        {
            //Floor
            if (Input.GetKeyDown(minuscode))
            {

                LerpUp();
            }

            if (Input.GetKeyDown(pluscode))
            {
                LerpDown();
            }


        }

        IEnumerator LerpCamSize(float targetSize, float duration)
        {
            Lerping = true;
            float time = 0;
            float startPosition = cam.orthographicSize;
            float startPostionForFixedY = fixedY.fixedY;

            while (time < duration && Lerping)
            {
                fixedY.fixedY = Mathf.Lerp(startPostionForFixedY, targetSize, time / duration);
                cam.orthographicSize =  Mathf.Lerp(startPosition, targetSize, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            cam.orthographicSize = targetSize;
        }


        IEnumerator LerpFollowHeight(float targetHeight, float duration)
        {
            Lerping = true;
            float time = 0;
          
            float startPostionForFixedY = fixedY.fixedY;

            while (time < duration && Lerping)
            {
                fixedY.fixedY = Mathf.Lerp(startPostionForFixedY, targetHeight, time / duration);
             
                time += Time.deltaTime;
                yield return null;
            }
            fixedY.fixedY = targetHeight;
        }


    }




}