using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public GameObject[] ships;
    public GameObject[] cells;
    public GameManager gameManager;

    private Ship ship;
    private int shipIndex = 0;
    private int[] aiCells = new int[100];   // -1 = unknown, 0 = miss, 1 = hit, 2 = sink

    // Start is called before the first frame update
    void Start()
    {
        ship = ships[shipIndex].GetComponent<Ship>();
        for (int i=0; i<aiCells.Length; i++)
        {
            aiCells[i] = -1;
        }
    }

    public void RotateShip()
    {
        ship.Rotate();
        gameManager.CheckPosition(ship);
    }

    public void NextShip()
    {
        shipIndex ++;
        if (shipIndex >= 5)
        {
            shipIndex = 0;
        }
        ship = ships[shipIndex].GetComponent<Ship>();
        ship.FlashColor(Color.yellow);
    }

    public void RandomShip()
    {
        // Reset all ships before random
        foreach (GameObject ship in ships)
        {
            ship.GetComponent<Ship>().ClearCell();
        }

        // Start randomly placing ships
        foreach (GameObject s in ships)
        {
            ship = s.GetComponent<Ship>();
            bool finished = false;
            while (!finished)
            {
                bool occupied = false;
                int cellIndex = UnityEngine.Random.Range(0,cells.Length);
                int rotate = UnityEngine.Random.Range(0,2);
                int increaseValue = 10;     // default ship is vertical
                if (rotate == 1)
                {
                    increaseValue = 1;      // ship is horizontal
                }
                for (int i=0; i<ship.size; i++) // break if ship cannot be placed at random cell
                {
                    // Condition: ship is not on gameboard
                    if (cellIndex+(increaseValue*i)>99)
                    {
                        occupied = true;
                        break;
                    }
                    // Condition: cell is occupied already
                    else if (cells[cellIndex+(increaseValue*i)].GetComponent<Cell>().Occupied())
                    {
                        occupied = true;
                        break;
                    }
                    // Condition: ship is horizontal and out-off gameboard
                    else if ((rotate == 1) && (cellIndex/10 != (cellIndex+(increaseValue*i))/10))
                    {
                        occupied = true;
                        break;
                    }
                }
                if (occupied == false)  // ship can be placed at the random position
                {
                    ship.transform.localPosition = ship.SetPosition(cells[cellIndex]);
                    if (rotate == 0 && ship.GetShipDirection() != 90f)
                    {
                        ship.Rotate();
                    }
                    if (rotate == 1 && ship.GetShipDirection() == 90f)
                    {
                        ship.Rotate();
                    }
                    for (int j=0; j<ship.size; j++)
                    {
                        ship.AddCells(cells[cellIndex+(increaseValue*j)]);
                    }
                    finished = true;
                }
            }
        }
    }

    public void PlaceShip(GameObject cell)
    {
        ship = ships[shipIndex].GetComponent<Ship>();
        ship.ClearCell();
        ships[shipIndex].transform.localPosition = ship.SetPosition(cell);
        gameManager.CheckPosition(ship);
    }

    public void ShipPlaced(GameObject cell)
    {
        ship.AddCells(cell);
    }

    public bool Ready()
    {
        foreach (GameObject ship in ships)
        {
            if (!ship.GetComponent<Ship>().Ready())
            {
                return false;
            }
        }
        return true;
    }

    public int CheckHit(GameObject cell)
    {
        Cell c = cell.GetComponent<Cell>();
        if (c.Occupied())
        {
            GameObject s = c.GetShip();
            s.GetComponent<Ship>().ShipHit();
            if (s.GetComponent<Ship>().ShipSank("player"))
            {
                return 2;   // 2 = sink
            }
            return 1;   // 1 = hit but not sink
        }
        return 0;   // 0 = miss
    }

    public bool CheckAttackedBefore(GameObject cell)
    {
        string cellNum = cell.name;
        string tmp = string.Empty;
        int num = -1;
        for (int i=0; i< cellNum.Length; i++)
        {
            if (Char.IsDigit(cellNum[i]))
            {
                tmp += cellNum[i];
            }
        }
        if (tmp.Length>0)
        {
            num = int.Parse(tmp);
        }
        if (aiCells[num] == -1)
        {
            return false;
        } 
        return true;
    }

    public void JotDown(int status, GameObject cell)
    {
        string cellNum = cell.name;
        string tmp = string.Empty;
        int num = -1;
        for (int i=0; i< cellNum.Length; i++)
        {
            if (Char.IsDigit(cellNum[i]))
            {
                tmp += cellNum[i];
            }
        }
        if (tmp.Length>0)
        {
            num = int.Parse(tmp);
        }
        aiCells[num] = status;
    }
}
