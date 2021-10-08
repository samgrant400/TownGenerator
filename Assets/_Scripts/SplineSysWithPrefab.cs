using Den.Tools;
using Den.Tools.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Twobob.Mm2
{
    public class SplineSysWithPrefab : SplineSys
    {

        //  public SplineSys SplineSys;
        public ObjectsPool.Prototype chosenType;


        public SplineSysWithPrefab(SplineSys src) 
		{ 
			CopyLinesFrom(src.lines);

            guiDrawNodes = src.guiDrawNodes;
			guiDrawSegments = src.guiDrawSegments;
			guiDrawDots = src.guiDrawDots;
			guiDotsCount = src.guiDotsCount;
			guiDotsEquidist = src.guiDotsEquidist;
		}



    public SplineSysWithPrefab()  // Just in case the serialisier gets all upset with itsself again.
        { 
        
        
        
        
        }



    }

}