using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{

    public Tilemap tilemap { get; private set; }
    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileNumEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;
    public Tile tileRock;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(CellGrid grid)
    {
        int width = grid.Width;
        int height = grid.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if(!cell.block)
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    public void InitiateDraw(CellGrid grid, int rockCount) 
    {
        int width = grid.Width;
        int height = grid.Height;

        for (int i = 0; i < rockCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            Cell cell = grid[x, y];
            tilemap.SetTile(cell.position, tileRock);
            cell.block = true;
            cell.revealed = true;
            cell.type = Cell.Type.Block;
        }

    } 
        

    private Tile GetTile(Cell cell)

    {
        if (cell.block)
        {
            return tileRock;
        }    
        else if (cell.numempty)
        {
            return tileEmpty;
        }
        else if (cell.revealed) {
            return GetRevealedTile(cell);
        } else if (cell.flagged) {
            return tileFlag;
        } else if (cell.chorded) {
            return tileEmpty;
        }
        
        else {
            return tileUnknown;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }

}
