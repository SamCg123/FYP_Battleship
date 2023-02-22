using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AI : MonoBehaviour
{
    public GameObject[] ships;
    public GameObject[] cells;
    public GameManager gameManager;

    private int mode; 
    private Ship ship;
    private int shipIndex = 0;
    private int[] playerCells = new int[100];   // -1 = unknown, 0 = miss, 1 = hit, 2 = sink
    List<int> hit = new List<int>();
    private int[] remainingShip = new int[5] {2,3,3,4,5};
    
    // Start is called before the first frame update
    void Start()
    {
        ship = ships[shipIndex].GetComponent<Ship>();
        for (int i=0; i<playerCells.Length; i++)
        {
            playerCells[i] = -1;
        }
    }

    public void SetAIMode(int mode)
    {
        this.mode = mode;
    }

    public void StartGame()
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
                for (int i=0; i<ship.size; i++) // loop will break if ship cannot be placed at random cell
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

    public int CheckHit(GameObject cell)
    {
        Cell c = cell.GetComponent<Cell>();
        if (c.Occupied())
        {
            GameObject s = c.GetShip();
            s.GetComponent<Ship>().ShipHit();
            if (s.GetComponent<Ship>().ShipSank("ai"))
            {
                return 2;   // 2 = sink
            }
            return 1;   // 1 = hit but not sink
        }
        return 0;   // 0 = miss
    }

    public void JotDown(int status, GameObject cell)
    { 
        if (status == 2)
        {
            Cell c = cell.GetComponent<Cell>();
            Ship s = c.GetShip().GetComponent<Ship>();
            List<GameObject> shipCells = s.GetCells();
            foreach (GameObject g in shipCells)
            {
                string cellNum = g.name;
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
                playerCells[num] = status;
            }
            int index = Array.IndexOf(remainingShip, s.size);
            remainingShip = remainingShip.Where((e, i) => i != index).ToArray(); 
        } else {
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
            playerCells[num] = status;
        }
    }

    public void Attack()
    {
        hit.Clear();
        for (int i=0; i<100; i++)
        {
            if (playerCells[i]==1)
            {
                hit.Add(i);
            }
        }
        if(mode == 0)    // 0 = easy mode
        {
            gameManager.AIAttackPlayer(Easymode()); 
        }
        else             // 1 = hard mode
        {
            gameManager.AIAttackPlayer(Hardmode()); 
        }
    }
    
    private int Easymode()
    {
        int cellIndex = -1;
        if (hit.Count == 0)
        {
            bool done = false;
            while (!done)
            {
                cellIndex = UnityEngine.Random.Range(0,playerCells.Length);
                if (playerCells[cellIndex] == -1)
                {
                    done = true;
                }
            }
        } else {
            cellIndex = FindNearestPossibleShip();
        }
        return cellIndex;
    }
    
    private int Hardmode()
    {
        int cellIndex = -1;
        if (hit.Count == 0)
        {
            int[] copyCells = new int[100]; // copy current gameboard for calculation
            for (int i=0;i<100; i++)
            {
                copyCells[i] = 0;
                if (playerCells[i] != -1)
                {
                    copyCells[i] = -1;
                }
            } 
            // Game tree
            for(int i=0; i<remainingShip.Length; i++)
            {
                for (int j=0; j<copyCells.Length; j++)
                {
                    for (int k=0; k<2; k++)
                    {
                        bool occupied = false;
                        cellIndex = j;
                        int increaseValue = 10;     // default ship is vertical
                        if (k == 1)
                        {
                            increaseValue = 1;      // ship is horizontal
                        }
                        for (int l=0; l<remainingShip[i]; l++) // loop will break if ship cannot be placed at cell
                        {
                            // Condition: ship is not on gameboard
                            if (j+(increaseValue*l)>99)
                            {
                                occupied = true;
                                break;
                            }
                            // Condition: cell is hit already
                            else if (copyCells[j+(increaseValue*l)] == -1)
                            {
                                occupied = true;
                                break;
                            }
                            // Condition: ship is horizontal and out-off gameboard
                            else if ((k == 1) && (j/10 != (j+(increaseValue*l))/10))
                            {
                                occupied = true;
                                break;
                            }
                        }
                        if (occupied == false)  // ship can be placed at the random position
                        {
                            for (int l=0; l<remainingShip[i]; l++)
                            {
                                copyCells[cellIndex+(increaseValue*l)]++;
                            }
                        }
                    }  
                }
            }
            cellIndex = Array.IndexOf(copyCells, copyCells.Max());          
        } else {
            cellIndex = FindNearestPossibleShip();
        }
        return cellIndex;
    }

    private int FindNearestPossibleShip()
    {
        int possibleIndex = -1;
        if (hit.Count >= 2)
        {
            int direction = hit[1] - hit[0]; 
            possibleIndex = hit[0] + direction;
            if (possibleIndex > 99 || possibleIndex < 0 || (possibleIndex/10 != hit[0]/10))
            {
                direction *= -1;
                possibleIndex += direction;
            } 
            int run = 0;
            int count = 0;
            while (playerCells[possibleIndex] != -1 && run < 20 && count < remainingShip.Max())
            {
                possibleIndex += direction;
                
                if (possibleIndex > 99 || possibleIndex < 0 || playerCells[possibleIndex] == 0 || playerCells[possibleIndex] == 2 
                    || ((possibleIndex/10 != hit[0]/10) && (direction == 1 || direction == -1)))
                {
                    direction *= -1;
                    possibleIndex += direction;
                    count = 0;
                }
                if (playerCells[possibleIndex] == 1) {count++;}
                if (playerCells[possibleIndex] == 2) {count=5;}
                run++;
            }
            if (run > 19 || count >= remainingShip.Max()) // all cells in a row/column is searched or have 5 hit in a row, but still no sink
            {
                if (direction % 10 == 0) 
                {
                    direction = UnityEngine.Random.Range(0, 1) * 2 - 1;
                } else {
                    direction = (UnityEngine.Random.Range(0, 1) * 2 - 1) * 10;
                }
                possibleIndex = hit[0] + direction;
                while (playerCells[possibleIndex] != -1)
                {
                    if (possibleIndex > 99 || possibleIndex < 0 || playerCells[possibleIndex] == 0)
                    {
                        direction *= -1;
                    }
                    possibleIndex += direction;
                }
            } 
        } else {
            List<int> nearCells = new List<int>();
            nearCells.Add(1); 
            nearCells.Add(-1); 
            nearCells.Add(10); 
            nearCells.Add(-10);
            if (hit[0] < 10) {nearCells.RemoveAt(3);}
            if (hit[0] > 89) {nearCells.RemoveAt(2);}
            if (hit[0] % 10 == 0) {nearCells.RemoveAt(1);}
            if (hit[0] % 10 == 9) {nearCells.RemoveAt(0);}
            
            int direction = UnityEngine.Random.Range(0, nearCells.Count);
            possibleIndex = hit[0] + nearCells[direction];
            while(playerCells[possibleIndex] != -1 && nearCells.Count > 0)
            {
                nearCells.RemoveAt(direction);
                direction = UnityEngine.Random.Range(0, nearCells.Count);
                possibleIndex = hit[0] + nearCells[direction];
            }
        }
        return possibleIndex;
    }
}
