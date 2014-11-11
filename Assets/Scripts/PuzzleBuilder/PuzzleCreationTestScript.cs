using UnityEngine;
using System.Collections;

public class PuzzleCreationTestScript : MonoBehaviour {
    public int Size = 4;
    public GameObject Cube;
	// Use this for initialization
	void Start () {
        PuzzleBuilder pb = new PuzzleBuilder();
        var puzzle = pb.GenerateCubeLevel(Size,new System.Collections.Generic.List<int>() {  1, 2, 3, 4 } );

        for(int i = 0; i < puzzle.GetLength(0); ++i)
            for(int j = 0; j < puzzle.GetLength(1); ++j)
                for(int k = 0; k < puzzle.GetLength(2); ++k)
            {
                GameObject go = (GameObject)GameObject.Instantiate(Cube,new Vector3(i,j,k),Quaternion.identity);
                (go.renderer as MeshRenderer).material.color = getColor(puzzle[i,j,k]);
            }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    Color getColor(int i)
    {
        switch(i)
        {
            case 0: return Color.magenta;
            case 1: return Color.yellow;
            case 2: return Color.blue;
            case 3: return Color.cyan;
            case 4: return Color.red;
            case -1: return Color.white;
            default: return Color.black;
        }
    }
}
