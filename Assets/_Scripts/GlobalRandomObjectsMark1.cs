using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Den.Tools;  
using Den.Tools.GUI;
using Den.Tools.Matrices;
using MapMagic.Products;
using MapMagic.Nodes;
using Den.Tools.Splines;

namespace Twobob.Mm2
{ 
	

	[System.Serializable]
	[GeneratorMenu (menu="Objects/Initial", name ="GlobalRandom", iconName="GeneratorIcons/Random", disengageable = true, 
		colorType = typeof(TransitionsList),
		helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/object_generators/Scatter")]
	public class GlobalRandomObjectsMark1 : Generator, IInlet<MatrixWorld>, IOutlet<TransitionsList>
	{
		[Val("Seed")]			public int seed = 12345;
		[Val("Density")]		public float density = 10;
		[Val("Uniformity")]		public float uniformity = 0.1f;
        [Val("Multiplier")]     public float factor = 10;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(GlobalRandomObjectsMark1));
#endif
        static SplineSys splineSys;


        public override void Generate (TileData data, StopToken stop)
		{
			if (!enabled) return;
			MatrixWorld probMatrix = data.ReadInletProduct(this);

            if (factor< float.Epsilon)
            {
                factor = 1;
            }

			Noise random = new Noise(data.random, seed);

			float square = data.area.active.worldSize.x * data.area.active.worldSize.z; //note using the real size, density should not depend on margins
			float count = square*(density/1000000); //number of items per terrain

			PosTab posTab = new PosTab((Vector3)data.area.full.worldPos, (Vector3)data.area.full.worldSize * factor, 16);
			RandomScatter((int)count, uniformity, (Vector3)data.area.full.worldPos, (Vector3)data.area.full.worldSize * factor, posTab, random, probMatrix, stop:null);  
			TransitionsList transitions = posTab.ToTransitionsList();





			data.StoreProduct(this, transitions);
		}


		public static void RandomScatter (int count, float uniformity, Vector3 offset, Vector3 size, PosTab posTab, Noise rnd, MatrixWorld prob, StopToken stop = null)
		{
			//int candidatesNum = (int)(uniformity*100);

            int candidatesNum = 100;

            if (candidatesNum < 1) candidatesNum = 1;
			
			for (int i=0; i<count; i++)
			{
				if (stop!=null && stop.stop) return;

				float bestCandidateX = 0;
				float bestCandidateZ = 0;
				float bestDist = 0;
				
				for (int c=0; c<candidatesNum; c++)
				{
					float candidateX = (offset.x+1) + (rnd.Random((int)posTab.pos.x, (int)posTab.pos.z, i*candidatesNum+c, 0)*(size.x-2.01f)); //TODO: do not use pos since it changes between preview/full
					float candidateZ = (offset.z+1) + (rnd.Random((int)posTab.pos.x, (int)posTab.pos.z, i*candidatesNum+c, 1)*(size.z-2.01f));

					//checking if candidate is the furthest one
					Transition closest = posTab.Closest(candidateX, candidateZ, minDist:0.001f);
					float dist = (closest.pos.x-candidateX)*(closest.pos.x-candidateX) + (closest.pos.z-candidateZ)*(closest.pos.z-candidateZ);

					//distance to the edge
					float bd = (candidateX-offset.x)*2; if (bd*bd < dist) dist = bd*bd;
					bd = (candidateZ-offset.z)*2; if (bd*bd < dist) dist = bd*bd;
					bd = (offset.x+size.x-candidateX)*2; if (bd*bd < dist) dist = bd*bd;
					bd = (offset.z+size.z-candidateZ)*2; if (bd*bd < dist) dist = bd*bd;

					//probability
					if (prob != null)
					{
						float probValue = prob.GetWorldInterpolatedValue(candidateX, candidateZ);
						dist *= probValue;
					}

					if (dist>bestDist) { bestDist=dist; bestCandidateX = candidateX; bestCandidateZ = candidateZ; }
				}

				if (bestDist>0.001f) //adding only if some suitable candidate found
				{
					Transition trs = new Transition(bestCandidateX, bestCandidateZ);
					posTab.Add(trs); 
				}
			}
			posTab.Flush();
		}
	}

	
}
