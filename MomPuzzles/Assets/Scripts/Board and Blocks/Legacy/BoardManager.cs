using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

    public int BoardWidth = 10;
    public int BoardHeight = 10;
    public float BlockSpeed = 10;
    public int AmountNeededToClear = 2;

    public Block BlockPrefab;

    public BoardState BoardState = BoardState.Ready;

    private Block TargettedBlock;

    Dictionary<Vector2, Block> Board;
    List<Block> BlockChain;
    List<Block> BlocksToCheck;

    int lowestY = -1;

    System.Array ColorsArray;

    //Events
    public delegate void BlockCleared(int amount);
    public static event BlockCleared OnBlockCleared;


	// Use this for initialization
	void Start () {
        //For testing, always same board
        //Random.InitState(2938798);

        Input.backButtonLeavesApp = true;
        ColorsArray = System.Enum.GetValues(typeof(BlockColor));

        InitBoard();
	}

    void InitBoard()
    {
        lowestY = BoardHeight;
        Board = new Dictionary<Vector2, Block>();
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
	void Update () {

	    if (Input.GetMouseButtonUp(0))
        {
            //If we click on a block
            if (TargettedBlock)
            {
                List<Block> blocksToBeCleared = new List<Block>();
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
            
        }

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
                    Block newBlock = GetBlockAbove(pos);
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
                if (Board[pos].transform.position.y > Board[pos].BoardPos.y)
                {
                    int dir = -1; //TODO: This is pointless for now since we are only going down....
                    float yChange = Time.deltaTime * dir * BlockSpeed;
                    Board[pos].transform.position = new Vector3(Board[pos].transform.position.x, Board[pos].transform.position.y + yChange, Board[pos].transform.position.z);
                    if (Board[pos].transform.position.y <= Board[pos].BoardPos.y)
                    {
                        Board[pos].transform.position = pos;
                    }

                    BoardState = BoardState.Moving;
                }
                else
                {
                    Board[pos].transform.position = pos;
                }
            }
        }
    }

    //Move a block to pos
    void MoveBlock(Block block, Vector2 pos)
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
        //Store the lowest Y Value of the blocks coming in to make placement above the board better
        if (pos.y < lowestY)
        {
            lowestY = (int)pos.y;
        }

        //Create a block GameObject
        Block b = Instantiate(BlockPrefab, new Vector3(pos.x, pos.y + BoardHeight - lowestY, 0), Quaternion.identity, this.transform) as Block;

        //Randomize the color

        b.BlockColor = (BlockColor)ColorsArray.GetValue(Random.Range(0, ColorsArray.Length));

        //Update block data
        b.BoardManager = this;
        b.BoardPos = pos;
        //Add the block to the Board Dictionary
        Board[pos] = b;
    }

    void BreakBlock(Block block)
    {
        if (Board.ContainsValue(block)) // MAke sure block is a part of the board
        {
            Vector2 key = block.BoardPos; // Store the position since thats our key value
            Board[key] = null; // Set that block to null indicating a blank
        }
        Destroy(block.gameObject); // Destroy the game object
    }

    //Use Flood fill Algorithm to find all similar tiles near-by
    List<Block> FindAllSimilarTiles(Block startBlock)
    {
        //Store selected block's color to look for
        BlockColor startColor = startBlock.BlockColor;
        //Create list and queue for storage
        List<Block> similarBlocks = new List<Block>();
        Queue<Block> queue = new Queue<Block>();
        //Put first block in queue for processing
        queue.Enqueue(startBlock);

        //Loop while things are still in queue
        while (queue.Count > 0)
        {
            //get next block out of queue
            Block block = queue.Dequeue();

            // Get the cardinal neighbors of the block
            List<Block> neighborBlocks = GetNeighborsAll(block.BoardPos);

            //Loop through each of those neightbors
            foreach(Block b in neighborBlocks)
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

    //Helper method for Block mouse over
    public void SetTargetBlock(Block block)
    {
        TargettedBlock = block;
    }

    //Returns a block that is offset by a Vector2 amount
    Block GetNeighborOffset(Vector2 pos, Vector2 Offset)
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
    Block GetNeighborNorth(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(0, 1));
    }

    //Returns the block to the south (Below) of a position
    Block GetNeighborSouth(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(0, -1));
    }

    //Returns the block to the East (Right) of a position
    Block GetNeighborEast(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(1, 0));
    }

    //Returns the block to the west (Left) of a position
    Block GetNeighborWest(Vector2 pos)
    {
        return GetNeighborOffset(pos, new Vector2(-1, 0));
    }

    //Returns all cardinal neighbors to a position
    List<Block> GetNeighborsAll(Vector2 pos)
    {
        List<Block> returnList = new List<Block>();
        returnList.Add(GetNeighborNorth(pos));
        returnList.Add(GetNeighborSouth(pos));
        returnList.Add(GetNeighborEast(pos));
        returnList.Add(GetNeighborWest(pos));
        returnList.RemoveAll(b => b == null);
        return returnList;
    }

    //Returns the first block above pos, or return null if no blocks are above
    Block GetBlockAbove(Vector2 pos)
    {
        Vector2 testPos = new Vector2(pos.x, pos.y);
        for (int y = (int)pos.y; y < BoardHeight; y++)
        {
            testPos.y = y;
            Block northernBlock = GetNeighborNorth(testPos);
            if (northernBlock)
            {
                return northernBlock;
            }
        }

        //WE looped through all above options and found nothing
        return null;
    }
}

public enum BoardState
{
    Ready,
    Moving
}
