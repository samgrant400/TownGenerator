using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Profiling;

using Den.Tools;
using Den.Tools.Matrices;
using Den.Tools.GUI;
using MapMagic.Core;
using MapMagic.Products;
using MapMagic.Nodes.GUI;
using MapMagic.Nodes;

namespace Twobob.Mm2
{



    public partial class SplinesEditor 
    {

        [Draw.Editor(typeof(MapSplineOutMark1))]
        public static void DrawObjectsOutput(MapSplineOutMark1 gen)
        {
            if (gen.posSettings == null) gen.posSettings = MapSplineOutMark1.CreatePosSettings(gen);

            using (Cell.LineStd)
                DrawObjectPrefabs(ref gen.prefabs, gen.guiMultiprefab, treeIcon: true);

            using (Cell.LinePx(0))
            using (Cell.Padded(2, 2, 0, 0))
            {
                using (Cell.LineStd) Draw.ToggleLeft(ref gen.guiMultiprefab, "Multi-Prefab");

                Cell.EmptyRowPx(4);

                using (Cell.LinePx(0))
                //using (new Draw.FoldoutGroup(ref gen.guiProperties, "Properties"))
                //    if (gen.guiProperties)
                //    {
                //        Cell.current.fieldWidth = 0.481f;
                //        using (Cell.LineStd) Draw.ToggleLeft(ref gen.allowReposition, "Use Pool");
                //        using (Cell.LineStd) Draw.ToggleLeft(ref gen.instantiateClones, "As Clones");
                //        using (Cell.LineStd) Draw.Field(ref gen.biomeBlend, "Biome Blend");

                //        using (Cell.LineStd) GeneratorDraw.DrawGlobalVar(ref GraphWindow.current.mapMagic.globals.objectsNumPerFrame, "Num/Frame");
                //    }
                  
                Cell.EmptyRowPx(2);
             //   DrawPositioningSettings(gen.posSettings, billboardRotWaring: true);
            }
        }


    }
}