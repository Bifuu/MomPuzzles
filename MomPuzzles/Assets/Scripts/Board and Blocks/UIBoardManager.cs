using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIBoardManager : MonoBehaviour {

    public int BoardWidth = 10;
    public int BoardHeight = 10;
    public float BlockSpeed = 10;
    public int AmountNeededToClear = 2;

    public UIBlock BlockPrefab;

    private UIBlock TargettedBlock;

    public BoardState BoardState = BoardState.Ready;

    public RectTransform rectTransform;
    float BlockSize = 0;

    Dictionary<Vector2, UIBlock> Board;
    List<UIBlock> BlockChain;
    List<UIBlock> BlocksToCheck;

    int lowestY = -1;

    System.Array ColorsArray;


    //Events
    public delegate void BlockCleared(BlocksClearedEventArgs args);
    public static event BlockCleared OnBlockCleared;


    // Use this for initialization
    void Start()
    {
        //For testing, always same board
        //Random.InitState(2938798);

        //Input.backButtonLeavesApp = true;
        ColorsArray = System.Enum.GetValues(typeof(BlockColor));

        //rectTransform = GetComponent<RectTransform>();
        Debug.Log(rectTransform.sizeDelta);
        Debug.Log(Screen.height + "," + Screen.width);
        BlockSize = rectTransform.sizeDelta.x / BoardHeight;
        Debug.Log(BlockSize);

        InitBoard();
    }

    void InitBoard()
    {
        lowestY = BoardHeight;
        Board = new Dictionary<Vector2, UIBlock>();
        for (int y = 0; y < BoardWidth; y++)
        {
            for (int x = 0; x < BoardHeight; x++)
            {
                Vector2 pos = new Vector2(x, y);
                Board.Add(pos, null);
                PlaceRandomBlock(pos);
            }
        }

        BoardState = BoardState.Moving;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        /*if (Input.GetMouseButtonUp(0))
        {
            //If we click on a block
            if (TargettedBlock)
            {
                List<UIBlock> blocksToBeCleared = new List<UIBlock>();
                //Find all Tiles
                blocksToBeCleared = FindAllSimilarTiles(TargettedBlock);

                //If count is 2 or more then clear it
                if (blocksToBeCleared.Count >= AmountNeededToClear)
                {
                    blocksToBeCleared.ForEach(b => BreakBlock(b)); // Break the blocks in the list
                    BoardState = BoardState.Moving;
                    if (OnBlockCleared != null)
                    {
                        OnBlockCleared(blocksToBeCleared.Count);
                    }
                }
            }

        }*/

        if (BoardState == BoardState.Moving)
        {
            UpdateBoard();
        }
    }

    void UpdateBoard()
    {
        BoardState = BoardState.Ready;
        lowestY = BoardHeight;

        //Iterate from bottom to the top Column by Column
        for (int y = 0; y < BoardWidth; y++)
        {
            for (int x = 0; x < BoardHeight; x++)
            {
                Vector2 pos = new Vector2(x, y);

                if (Board[pos] == null) // No block in current spot
                {
                    UIBlock newBlock = GetBlockAbove(pos);
                    if (newBlock)
                    {
                        MoveBlock(newBlock, pos);
                    }
                    else
                    {
                        //Debug.Log("New Blocks Needed at: " + pos);
                        PlaceRandomBlock(pos);
                    }

                    //Something needs updating so stay in that state
                    //BoardState = BoardState.Moving;
                }

                //Check if we need to physically move the block
                if (Board[pos].rt.anchoredPosition.y > Board[pos].BoardPos.y * BlockSize)
                {
                    int dir = -1; //TODO: This is pointless for now since we are only going down....
                    float yChange = Time.deltaTime * dir * BlockSpeed;
                    Board[pos].rt.anchoredPosition += new Vector2(0, yChange);
                    if (Board[pos].rt.anchoredPosition.y <= Board[pos].BoardPos.y * BlockSize)
                    {
                        Board[pos].rt.anchoredPosition = pos * BlockSize;
                    }

                    BoardState = BoardState.Moving;
                }
                else
                {
                    //Board[pos].rt.anchoredPosition = pos * BlockSize;
                }

                /*if (Board[pos].rt.offsetMax.y >= 0)
                {
                    float dir = -1 * Mathf.Sign(Board[pos].rt.offsetMax.y);
                    float yChange = Time.deltaTime * dir * BlockSpeed;
                    Board[pos].rt.offsetMax += new Vector2(0, yChange);
                    if (Board[pos].rt.offsetMax.y <= 0)
                    {
                        Board[pos].rt.offsetMax = Vector2.zero;
                    }
                }

                if (Board[pos].rt.offsetMin.y >= 0)
                {
                    float dir = -1 * Mathf.Sign(Board[pos].rt.offsetMin.y);
                    float yChange = Time.deltaTime * dir * BlockSpeed;
                    Board[pos].rt.offsetMin += new Vector2(0, yChange);
                    if (Board[pos].rt.offsetMin.y <= 0)
                    {
                        Board[pos].rt.offsetMin = Vector2.zero;
                    }
                }
                */
            }
        }
    }

    //Move a block to pos
    void MoveBlock(UIBlock block, Vector2 pos)
    {
        //Store the old position
        Vector2 oldPos = block.BoardPos;
        //update the board dictionary
        Board[pos] = block; //set the new pos
        Board[oldPos] = null; //clear the old pos

        //Update the data the block itself holds
        block.BoardPos = pos;

    }

    //Place a random block at location
    void PlaceRandomBlock(Vector2 pos)
    {
        //Debug.Log("Placing block at: " + pos);
        //Store the lowest Y Value of the blocks coming in to make placement above the board better
        if (pos.y < lowestY)
        {
            lowestY = (int)pos.y;
        }

        //Create a UIBlock GameObject
        //UIBlock b = Instantiate(BlockPrefab, new Vector3(pos.x, (pos.y + BoardHeight - lowestY), 0), Quaternion.identity, this.transform) as UIBlock;
        UIBlock b = Instantiate(BlockPrefab) as UIBlock;
        b.transform.SetParent(this.transform, false);
        //RectTransform rt = b.GetComponent<RectTransform>(); //Get the RectTransform
        Vector2 aboveBoardPos = new Vector2(pos.x, pos.y + BoardHeight - lowestY);

        b.rt.sizeDelta = new Vector2(BlockSize, BlockSize); //Set the size of he blocks to fill the square
        b.rt.anchoredPosition = new Vector2(aboveBoardPos.x * BlockSize, aboveBoardPos.y * BlockSize);


        //rt.position = Vector3.zero;
        //rt.position = new Vector3(pos.x, (pos.y + BoardHeight - lowestY)); //Set the position

        //Randomize the color

        b.BlockColor = (BlockColor)ColorsArray.GetValue(Random.Range(1, ColorsArray.Length));


        float rndType = Random.value;
        float ac = PlayerController.Controller.Player.AttackBlockChance;
        float hc = PlayerController.Controller.Player.HealBlockChance;
        
        Text typeText = b.GetComponentInChildren<Text>();
        //typeText.transform.SetParent(b.transform);
        //Debug.Log(rndType + ": " + ac + " " + hc);
        if (rndType - ac <= 0)
        {
            b.BlockType = BlockType.Attack;
            b.name = "A" + b.name;
            typeText.text = "A";
            typeText.color = Color.red;
        } else if (rndType - ac - hc <= 0)
        {
            b.BlockType = BlockType.Heal;
            b.name = "H" + b.name;
            typeText.text = "H";
            typeText.color = Color.green;
        } else
        {
            b.BlockType = BlockType.Normal;
        }



        //Update block data
        b.UIBoardManager = this;
        b.BoardPos = pos;
        //Add the block to the Board Dictionary
        Board[pos] = b;
    }

    void BreakBlock(UIBlock block)
    {
        if (Board.ContainsValue(block)) // MAke sure block is a part of the board
        {
            Vector2 key = block.BoardPos; // Store the position since thats our key value
            Board[key] = null; // Set that block to null indicating a blank
        }
        Destroy(block.gameObject); // Destroy the game object
    }

    //Use Flood fill Algorithm to find all similar tiles near-by
    List<UIBlock> FindAllSimilarTiles(UIBlock startBlock)
    {
        //Store selected block's color to look for
        BlockColor startColor = startBlock.BlockColor;
        //Create list and queue for storage
        List<UIBlock> similarBlocks = new List<UIBlock>();
        Queue<UIBlock> queue = new Queue<UIBlock>();
        //Put first block in queue for processing
        queue.Enqueue(startBlock);

        //Loop while things are still in queue
        while (queue.Count > 0)
        {
            //get next block out of queue
            UIBlock block = queue.Dequeue();

            // Get the cardinal neighbors of the block
            List<UIBlock> neighborBlocks = GetNeighborsAll(block.BoardPos);

            //Loop through each of those neightbors
            foreach (UIBlock b in neighborBlocks)
            {
                if (b.BlockColor == startColor) //Make sure that the colors are the same
                {
                    if (!queue.Contains(b)) //Ignore if block is already in queue for processing
                    {
                        if (!similarBlocks.Contains(b)) //Ignore if block is already in the final list;
                        {
                            queue.Enqueue(b); // Add to queue
                        }
                    }
                }
            }

            //Add this block to similar blocks.
            similarBlocks.Add(block);

        }

        return similarBlocks;
    }

    //Helper method for Clicking a Block
    public void SetTargetBlock(UIBlock block)
    {
        TargettedBlock = block;

        List<UIBlock> blocksToBeCleared = new List<UIBlock>();
        //Find all Tiles
        blocksToBeCleared = FindAllSimilarTiles(TargettedBlock);

        //If count is 2 or more then clear it
        if (blocksToBeCleared.Count >= AmountNeededToClear)
        {
            
            
            if (OnBlockCleared != null)
            {
                BlocksClearedEventArgs args;
                args.AmountCleared = blocksToBeCleared.Count;
                args.Color = blocksToBeCleared[0].BlockColor;
                args.AttackCount = 0;
                args.HealCount = 0;
                foreach (UIBlock b in blocksToBeCleared)
                {
                    if (b.BlockType == BlockType.Attack)
                        args.AttackCount++;
                    if (b.BlockType == BlockType.Heal)
                        args.HealCount++;
                }

                OnBlockCleared(args);
            }

            blocksToBeCleared.ForEach(b => BreakBlock(b)); // Break the blocks in the list
            BoardState = BoardState.Moving;
        }
    }

    //Returns a block that is offset by a Vector2 amount
    UIBlock GetNeighborOffset(Vector2 pos, Vector2 Offset)
    {
        Vector2 neighborPos = pos + Offset;

        if (Board.ContainsKey(neighborPos))
        {
            return Board[neighborPos];
        }
        else
        {
            return null;
        }
    }

    //Returns the block to the north (Above) of a position
    UIBlock GetNeighborNorth(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(0, 1));
    }

    //Returns the block to the south (Below) of a position
    UIBlock GetNeighborSouth(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(0, -1));
    }

    //Returns the block to the East (Right) of a position
    UIBlock GetNeighborEast(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(1, 0));
    }

    //Returns the block to the west (Left) of a position
    UIBlock GetNeighborWest(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(-1, 0));
    }

    //Returns all cardinal neighbors to a position
    List<UIBlock> GetNeighborsAll(Vector2 pos)
    {
        List<UIBlock> returnList = new List<UIBlock>();
        returnList.Add(GetNeighborNorth(pos));
        returnList.Add(GetNeighborSouth(pos));
        returnList.Add(GetNeighborEast(pos));
        returnList.Add(GetNeighborWest(pos));
        returnList.RemoveAll(b => b == null);
        return returnList;
    }

    //Returns the first block above pos, or return null if no blocks are above
    UIBlock GetBlockAbove(Vector2 pos)
    {
        Vector2 testPos = new Vector2(pos.x, pos.y);
        for (int y = (int)pos.y; y < BoardHeight; y++)
        {
            testPos.y = y;
            UIBlock northernBlock = GetNeighborNorth(testPos);
            if (northernBlock)
            {
                return northernBlock;
            }
        }

        //WE looped through all above options and found nothing
        return null;
    }
}

public struct BlocksClearedEventArgs
{
    public int AmountCleared;
    public BlockColor Color;
    public int AttackCount;
    public int HealCount;
}
