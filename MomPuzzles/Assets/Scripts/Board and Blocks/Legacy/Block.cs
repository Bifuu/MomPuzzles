using UnityEngine;
using System.Collections.Generic;

public class Block : MonoBehaviour {

    public BlockColor BlockColor = BlockColor.Blue;
    public Sprite[] BlockSprites;
    public Vector2 BoardPos;
    public BlockType BlockType = BlockType.Normal;

    private SpriteRenderer sr;
    public BoardManager BoardManager;

    public float BlockWidth
    {
        get { return sr.sprite.border.x; }
    }

    public BoardManager BoardManger{ get; set; }

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();

        UpdateSprite();
        
	}

    public void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite = BlockSprites[(int)BlockColor];
    }

    public void HighlightBlock()
    {
        sr.color = Color.black;
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnMouseEnter()
    {
        BoardManager.SetTargetBlock(this);
    }
}


