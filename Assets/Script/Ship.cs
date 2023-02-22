using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{ 
    public float xCoordinate = 0;
    public float yCoordinate = 0;
    public int size;

    private GameObject cellPosition;
    List<GameObject> cells = new List<GameObject>();
    private float rotateAngle = 90f;
    private int hit = 0;

    public List<GameObject> GetCells()
    {
        return cells;
    }

    public void ClearCell()
    {
        foreach (GameObject cell in cells)
        {
            cell.GetComponent<Cell>().ResetCell();
        }
        cells.Clear();
    }

    public void AddCells(GameObject cell)
    {
        cells.Add(cell);
        cell.GetComponent<Cell>().TakeCell(gameObject);
    }

    public Vector2 SetPosition(GameObject cell)
    {
        cellPosition = cell;
        return new Vector2(cellPosition.transform.position.x + xCoordinate, cellPosition.transform.position.y + yCoordinate);
    }

    public void Rotate()
    {
        ClearCell();
        transform.localEulerAngles += new Vector3(0, 0, rotateAngle);
        rotateAngle *= -1;
        float tmp = xCoordinate;
        xCoordinate = yCoordinate;
        yCoordinate = tmp;
        transform.localPosition = new Vector2(cellPosition.transform.position.x + xCoordinate, cellPosition.transform.position.y + yCoordinate);
    }

    public float GetShipDirection()
    {
        return rotateAngle;
    }

    public bool Ready()
    {
        return cells.Count == size;
    }

    public void FlashColor(Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
        Invoke("ResetColor",1f);
    }

    private void ResetColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.black;
    }

    public void ShipHit()
    {
        hit++;
    }

    public bool ShipSank(string check)
    {
        float x = 698; // for ai ship to appear on gameboard
        if (hit >= size && check == "ai")
        {
            transform.localPosition = new Vector2(cellPosition.transform.position.x + xCoordinate-x, cellPosition.transform.position.y + yCoordinate);
        }
        return hit >= size;
    }
}
