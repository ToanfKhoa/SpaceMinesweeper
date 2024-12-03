using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;


public class Booster : MonoBehaviour
{
    public GameObject laserPrefab;
    public void AutoLaser()
    {
        int width = Game.Instance.width;
        int height = Game.Instance.height;

        List<Cell> listCanDigCell = new List<Cell>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = Game.Instance.grid.cells[x, y];

                if (cell.type != Cell.Type.Mine && cell.type != Cell.Type.Block && cell.revealed == false)
                {
                    listCanDigCell.Add(cell);
                }
            }
        }
        Cell randomCell = listCanDigCell[Random.Range(0, listCanDigCell.Count)];
        if (!Game.Instance.generated)
        {
            Game.Instance.grid.GenerateMines(randomCell, Game.Instance.mineCount);
            Game.Instance.grid.GenerateNumbers();
            Game.Instance.generated = true;
        }
        GameObject laserEffect = Instantiate(laserPrefab, randomCell.position + new Vector3(0.5f, 0.5f), Quaternion.identity);
        Destroy(laserEffect, 0.5f);
        Game.Instance.Reveal(randomCell);
        Game.Instance._userDatas.diamond -= 5;
        Game.Instance.SaveData();
    }

}
