using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Den.Tools;
using Den.Tools.Splines;
using Den.Tools.Matrices;
using Den.Tools.GUI;
using MapMagic.Core;
using MapMagic.Products;
using System.Linq;

namespace MapMagic.Nodes.SplinesGenerators
{

    [System.Serializable]
    [GeneratorMenu(
menu = "Spline",
name = "WiggleV3",
iconName = "GeneratorIcons/Constant",
colorType = typeof(SplineSys),
disengageable = true,
helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/map_generators/constant")]
    public class WigglerV3 : Generator, IInlet<SplineSys>, IOutlet<SplineSys>
    {
        [Val("Input", "Inlet")] public readonly Inlet<SplineSys> input = new Inlet<SplineSys>();

        [Val("Output", "Outlet")] public readonly Outlet<SplineSys> output = new Outlet<SplineSys>();

        /// <summary>
        /// Make the random "repeatable"
        /// </summary>
        [Val("Repeatable?")] public bool useNoise = false;

        /// <summary>
        /// use this to get unique patterns without changing anything else.
        /// </summary>
        [Val("ImaginaryPart")] public float FractalStep = 1f;


        [Val("Divisions")] public int divisions = 2;

        [Val("Wiggly")] public int wiggliness = 1;

        /// <summary>
        /// Not entirely sure...
        /// </summary>
        //[Val("NodeType?")] public Node.TangentType nodeType = Node.TangentType.auto;

       // [Val("Bendy?")] public bool doBendy = false;

        [Val("Bendiness")] public int bendiness = 0;

        [Val("Relax?")] public bool doRelax = false;

        [Val("RelaxIterations")] public int ri = 4;

        [Val("RelaxBlur")] public float blur = 1f;


        private Vector2 Full_I_Value = Vector2.zero;


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(WigglerV3));
#endif 

        public override void Generate(TileData data, StopToken stop)
        {


            // GUARD VALUES

            if (divisions < 2) { divisions = 2; }
            if (blur < 1) { blur = 1; }
            if (ri < 1) { ri = 1; }



            SplineSys src = data.ReadInletProduct(this);

            if (src == null) return;

            SplineSys dst = new SplineSys(src);

           

            if (!enabled)
            {
                data.StoreProduct(this, src);
                return;
            }

            // setup the clamp mask
            var tileLocation = data.area.Coord.ToVector3(1000);
            var tileSize = new Vector3(1000, 500, 1000);

            // now magically create perfect size slices for this tile.  Thanks Denis.
            dst.Clamp(tileLocation, tileSize);


            // avoid non offsets for our imaginary pair.
            if (FractalStep == 0)
                FractalStep = 1;

            // setup the imaginary offset
            Full_I_Value = new Vector2(FractalStep, FractalStep);

            /// if (data.isDraft) return;
            dst.Subdivide(divisions);


            
            foreach (var item in dst.lines)
            {

                //for (int j = 0; j < item.segments.Length; j++)
                //{
                //    // Skip the very first two.
                //    if (j > 0)
                //    {

                //        Node start = item.segments[j].start;
                //        start.type = Node.TangentType.auto;
                //        Vector2 startpos = new Vector2(start.pos.x, start.pos.z);


                //        if (useNoise)
                //        {
                //            Vector3 place = ReturnWigglyVector3UsingPerlinNoise(bendiness, startpos.V3(), false);

                //            start.dir =
                //                    (FlipALocationCoin(startpos)) ?
                //                  //start.dir + 
                //                  place :
                //                  // start.dir 
                //                  -place;

                //        }
                //        else
                //        {
                //            Vector3 place = ReturnWigglyVector3(bendiness);

                //            start.dir =
                //                 (FlipALocationCoin(startpos)) ?
                //               // start.dir + 
                //               place :
                //               // start.dir
                //               -place;
                //        }



                //        item.segments[j].start = start;
                //    }
                //    // Skip the lasties
                //    if (j < item.segments.Length)
                //    {

                //        Node end = item.segments[j].end;
                //        end.type = Node.TangentType.auto;
                //        Vector2 endpos = new Vector2(end.pos.x, end.pos.z);

                //        if (useNoise)
                //        {

                //            Vector3 place = ReturnWigglyVector3UsingPerlinNoise(bendiness, endpos.V3(), false);

                //            end.dir =
                //                    (FlipALocationCoin(endpos)) ?
                //                  // end.dir + 
                //                  place :
                //                  //  end.dir
                //                  -place;
                //        }
                //        else
                //        {

                //            Vector3 place =
                //            ReturnWigglyVector3(bendiness);
                //            end.dir = (FlipALocationCoin(endpos)) ?
                //            // end.dir + 
                //            place :
                //            //  end.dir 
                //            -place;
                //        }


                //        item.segments[j].end = end;
                //    }
                //}
            


            for (int i = 1; i < item.NodesCount - 1; i++)
                {




                    var cur = item.GetNodePos(i);
                    var rotin = item.GetNodeInTangent(i);
                    var rotout= item.GetNodeOutTangent(i);

                    var nv = Vector3.zero;
                    var newFull_I_Value = new Vector3(Full_I_Value.x, 0, Full_I_Value.y);
                    var loc = new Vector3(cur.x, 0, cur.y);
                    Vector3? placein = Vector3.zero;
                    Vector3? placeout = Vector3.zero;



                    if (useNoise)
                    {
                        // return an offset that includes the world location
                        nv = ReturnWigglyVector3UsingPerlinNoise(wiggliness, cur, true);
                        
                        // just return an offset from 0,0
                        placein = ReturnWigglyVector3UsingPerlinNoise(bendiness, cur, false) + rotin;
                        placeout = ReturnWigglyVector3UsingPerlinNoise(bendiness, cur, false) + rotout;
                    }
                    else
                    {
                        // similar but for non repeatable operation
                        nv = new Vector3(cur.x + ReturnWiggly(wiggliness), cur.y, cur.z + ReturnWiggly(wiggliness));

                        // ditto
                        placein = ReturnWigglyVector3(bendiness);
                        placeout = ReturnWigglyVector3(bendiness);
                    }

                    // place the node
                    item.SetNodePos(i, nv);
                    // set the end rotations (just set both for now until I get this to actually do something)

                    var actualSegmentNumber = Mathf.Max(0, i - 1);

                   // item.SetNodeRotations(i, new Vector3?[] { placein, placeout });

                    item.SetNodeRotations(i, (Vector3)placein);

                 //   item.SetNodeRotationByTerminationType(LineHelper.NodeTerminationType.start, actualSegmentNumber, (Vector3)placein);
                    // Do we need to do this?
                 //   item.UpdateTangent(actualSegmentNumber);
                  //  item.SetNodeRotationByTerminationType(LineHelper.NodeTerminationType.end, i, (Vector3)placeout);
                    // Do we need to do this?
                  //  item.UpdateTangent(i);
                    
                }
              //  item.Update();
            }



            if (doRelax)
            {
                dst.Relax(blur, ri);
            }


                if (dst.NodesCount == 0)
                {
                    data.StoreProduct(this, src);
                    return;

                }



                // now magically create perfect size slices for this tile.  Thanks Denis.
                dst.Clamp(tileLocation, tileSize);

         //       dst.UpdateTangents();

                data.StoreProduct(this, dst);
         
        }

        // persistence?
        private bool FlipALocationCoin(Vector2 location)
        {
            
            return ReturnPerlinNoiseValueAtSpot(1, location) < 0f;
        }



        private Vector3 ReturnWigglyVector3UsingPerlinNoise(float factor, Vector3 location, bool addOffset)
        {

          //  var loc = new Vector3(location.x, 0, location.y);
            var newFull_I_Value = new Vector3(Full_I_Value.x, 0, Full_I_Value.y);

         //   new Vector3(cur.x + ReturnPerlinNoiseValueAtSpot(wiggliness, loc), cur.y, cur.z + ReturnPerlinNoiseValueAtSpot(wiggliness, loc + newFull_I_Value));



            var ret =

             new Vector3(
                ReturnPerlinNoiseValueAtSpot(factor, location),
                0,
                 ReturnPerlinNoiseValueAtSpot(factor, location + newFull_I_Value)
                 );
            if (addOffset)
            {
                return ret + location;
            }
            return ret + new Vector3(0,1,0);

        }



        private float ReturnPerlinNoiseValueAtSpot(float factor, Vector2 location)
        {

            return  ((Mathf.PerlinNoise(location.x, location.y) - 0.5f) *2) * factor;

           
        }


        private float ReturnWiggly(float factor)
        {
            // to  simplify changing the random here is an example.
            //   return (((UnityEngine.Random.value - 0.5f) * 2) * factor);
         //   return (((UnityEngine.Random.value - 0.5f) * 2) * factor);
            return RandomGen.Next(10, -10) * 0.1f * factor;
        }

        private Vector3 ReturnWigglyVector3(float factor)
        {

            return new Vector3(RandomGen.Next(10, -10) * 0.1f * factor, 0, RandomGen.Next(10, -10) * factor);
      
        
        }



    }



    public static class LineHelper {

        public enum NodeTerminationType { start, end };

        public static void SetNodeRotationByTerminationType(this Line line, NodeTerminationType use, int segmentnumber, Vector3 rotation )
        {
            //	segments (3):	    0		1		2
            //	nodes (4):		0 ----- 1 ----- 2 ----- 3
            // figure out segment that a node could belong to
            var actualSegmentNumber = Mathf.Max(0, segmentnumber - 1);

            switch (use)
            {
                case NodeTerminationType.start:
                    if (actualSegmentNumber != line.segments.Length)
                        line.segments[actualSegmentNumber].start.dir = rotation;
                    break;
                case NodeTerminationType.end:
                    if (actualSegmentNumber != 0)
                        line.segments[actualSegmentNumber - 1].end.dir = rotation;
                    break;
                default:
                    break;
            }

           

        }


        public static void SetNodeStartRotation(this Line line, int n, Vector3 rotation)
        {
            if (n != line.segments.Length)
                line.segments[n].start.dir = rotation;
         
        }

        public static void SetNodeEndRotation(this Line line, int n, Vector3 rotation)
        {            
            if (n != 0)
                line.segments[n - 1].end.dir = rotation;
        }

        public static void SetNodeRotations(this Line line, int n, Vector3 rotation)
        {
            var actualSegmentNumber = Mathf.Max(0, n - 1);

            if (actualSegmentNumber != line.segments.Length)
            {
                line.segments[actualSegmentNumber].start.dir = rotation;
             
            }
            if (actualSegmentNumber != 0)
            {
                line.segments[actualSegmentNumber - 1].end.dir = rotation;
            }

            line.UpdateTangents();

        }


        public static void SetNodeRotations(this Line line, int n, Vector3?[] rotation)
        {
            if (n != line.segments.Length)
            {
                line.segments[n].start.dir = (Vector3)rotation[0];

            }
            if (n != 0)
            {
                line.segments[n - 1].end.dir = (Vector3)rotation[1];
            }

            line.UpdateTangents();

        }




    }


}
