using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class EndCapDemo : MonoBehaviour {
	public Texture2D lineTex;
	public Texture2D lineTex2;
	public Texture2D lineTex3;
	public Texture2D frontTex;
	public Texture2D backTex;
	public Texture2D capTex;

	//[]
	public float frontScale = 1f;
	VectorLine line2;
	public float scaleTexture =2f;
	List<Vector2> splinePoints;

	void Start () {
		//VectorLine.SetEndCap ("arrow", EndCap.Front, lineTex, frontTex);
	
		//VectorLine.SetEndCap ("rounded", EndCap.Mirror, lineTex3, capTex);
		VectorLine.SetEndCap("arrow2", EndCap.Both, 0f, 0f, frontScale, 0, lineTex2, frontTex, backTex);
		//var line1 = new VectorLine("Arrow", new List<Vector2>(50), 30.0f, LineType.Continuous, Joins.Weld);
		//line1.useViewportCoords = true;
		//var splinePoints = new Vector2[] {new Vector2(.1f, .15f), new Vector2(.3f, .5f), new Vector2(.5f, .6f), new Vector2(.7f, .5f), new Vector2(.9f, .15f)};
		//line1.MakeSpline (splinePoints);
		//line1.endCap = "arrow";
		//line1.Draw();
		//VectorLine.SetEndCap("arrow2", EndCap.Both, 0f,0f,frontScale, 0f,  lineTex2, frontTex, backTex);
		splinePoints = new List<Vector2> { new Vector2(.1f, .85f), new Vector2(.3f, .5f), new Vector2(.5f, .4f), new Vector2(.7f, .5f), new Vector2(.9f, .85f) };
		//line2.MakeSpline(splinePoints);
		line2 = new VectorLine("Arrow2", splinePoints, 40.0f, LineType.Continuous, Joins.Weld);

		//line2 = new VectorLine("MyLine", splinePoints, lineTex2, 14.0f);
		
		line2.useViewportCoords = true;
		line2.textureScale = scaleTexture;
		line2.endCap = "arrow2";
		
		line2.Draw();
		
		
		//var line3 = new VectorLine("Rounded", new List<Vector2>{new Vector2(.1f, .5f), new Vector2(.9f, .5f)}, 20.0f);
		//line3.useViewportCoords = true;
		//line3.endCap = "rounded";
		//line3.Draw();
	}

	void Update() {

		//VectorLine.SetEndCap("arrow2", EndCap.Both, 0f, 0f, frontScale, 0, lineTex2, frontTex, backTex);
		//line2.points2.Clear();
		//line2.points2.AddRange(splinePoints);
		//line2.MakeSpline(splinePoints);
		//line2.endCap = "arrow2";
		if (Input.GetKeyDown(KeyCode.Space))
		{
			
			line2.textureScale = scaleTexture;
			line2.Draw();
			Debug.Log("re draw");
		}
	}



}