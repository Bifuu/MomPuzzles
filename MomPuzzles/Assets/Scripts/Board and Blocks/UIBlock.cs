using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIBlock : MonoBehaviour, IPointerClickHandler {

    public BlockColor BlockColor = BlockColor.Blue;
    public Sprite[] BlockSprites;
    public Vector2 BoardPos;
    public BlockType BlockType = BlockType.Normal;

    private Image image;
    public RectTransform rt;
    public UIBoardManager UIBoardManager;

    public float BlockWidth
    {
        get { return rt.sizeDelta.x; }
    }

    public BoardManager BoardManger { get; set; }

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    // Use this for initialization
    void Start()
    {
        

        UpdateSprite();

    }

    public void UpdateSprite()
    {
        //image.sprite = BlockSprites[(int)BlockColor - 1];
        switch (BlockColor)
        {
            case BlockColor.Blue:
                image.sprite = BlockSprites[0];
                break;

            case BlockColor.Red:
                image.sprite = BlockSprites[1];
                break;

            case BlockColor.Green:
                image.sprite = BlockSprites[2];
                break;

            case BlockColor.Yellow:
                image.sprite = BlockSprites[3];
                break;

            case BlockColor.White:
                image.sprite = BlockSprites[4];
                break;
        }
    }

    public void HighlightBlock()
    {
        image.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIBoardManager.SetTargetBlock(this);
    }
}
