using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameManager gameManager;
    private bool occupied = false;
    private GameObject ship;

    public void CellClicked()
    {
        gameManager.CellClicked(gameObject);
    }

    public bool Occupied()
    {
        return occupied;
    }

    public void TakeCell(GameObject s)
    {
        occupied = true;
        ship = s;
    }

    public void ResetCell()
    {
        occupied = false;
    }

    public GameObject GetShip()
    {
        return ship;
    }
}
