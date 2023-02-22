using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AI ai;
    public Player player;
    public Canvas canvas;
    public GameObject confirmedHit;
    public GameObject missedHit;

    private bool ready = false;
    private bool playerTurn = true;
    private Ship checkShip;
    private int playerShipCount = 5;
    private int aiShipCount = 5;
    private bool endGame = false;

    // Start is called before the first frame update
    void Start()
    {
        ai.SetAIMode(PlayerPrefs.GetInt("mode"));
        canvas.PrepareGame();
    }

    // Buttons' Functions
    public void RotateClicked()
    {
        player.RotateShip();
    }

    public void NextShipClicked()
    {
        player.NextShip();
    }

    public void RandomClicked()
    {
        player.RandomShip();
    }

    public void ReadyClicked()
    {
        if (player.Ready())
        {
            ready = true;
            ai.StartGame();
            canvas.StartGame();
        } else {
            canvas.DisplayPrepareInfoText("You are not ready!", Color.red);
        }
    }

    public void CellClicked(GameObject cell)
    {
        if (!ready)
        {
            player.PlaceShip(cell);
        } else if (!endGame && playerTurn)
        {
            if (!player.CheckAttackedBefore(cell))
            {
                PlayerAttackAI(cell);
            }
        }
    }

    // Preparation Pharse
    public void CheckPosition(Ship ship)
    {
        checkShip = ship.GetComponent<Ship>();
        Invoke("Check",0.1f);  // takes time for sprite position transformation
    }

    private void Check()
    {
        Collider2D collider = checkShip.GetComponent<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        List<Collider2D> results = new List<Collider2D>();
        if (collider.OverlapCollider(filter,results)>=checkShip.size)
        {
            foreach(Collider2D res in results)
            {
                if (!res.CompareTag("Cell"))
                {
                    WrongPosition();                 
                } else {
                    if (!ready)
                    { 
                        if (!res.gameObject.GetComponent<Cell>().Occupied()) 
                        {
                            player.ShipPlaced(res.gameObject);
                        } else {
                            WrongPosition();
                        }
                    }
                }
            }
        } else {
            WrongPosition();
        }
    }

    private void WrongPosition()
    {
        checkShip.FlashColor(Color.red);
        canvas.DisplayPrepareInfoText("Invalid position! Please place the ship again!", Color.red);
        checkShip.ClearCell(); 
    }

    // Fighting Pharse
    private void PlayerAttackAI(GameObject cell)
    {
        playerTurn = !playerTurn;
        int status = ai.CheckHit(cell);
        if (status != 0)
        {
            Instantiate(confirmedHit, cell.transform.position, Quaternion.identity);
            if (status == 2)
            {
                aiShipCount--;
                canvas.DisplayAIText("One AI ship is sank!");
            }
            if (aiShipCount == 0)
            {
                canvas.Gameover(0); // 0 = Player win
            } else {
                canvas.DisplayFightingInfoText("You hit an AI ship! You can attack again");
                playerTurn = true;
            }
        } else {
            Instantiate(missedHit, cell.transform.position, Quaternion.identity);
            canvas.DisplayFightingInfoText("You missed");
            canvas.DisplayNextTurn(playerTurn);
            ai.Invoke("Attack",1.0f);
        }
        player.JotDown(status, cell);
    }

    public void AIAttackPlayer(int cellIndex)
    {
        GameObject cell = player.cells[cellIndex];
        int status = player.CheckHit(cell);
        if (status != 0)
        {
            Instantiate(confirmedHit, cell.transform.position, Quaternion.identity);
            if (status == 2)
            {
                playerShipCount--;
                canvas.DisplayPlayerText("One player ship is sank!");
            }
            if (playerShipCount == 0)
            {
                canvas.Gameover(1);
            } else {
                canvas.DisplayFightingInfoText("AI hit your ship! AI can attack again");
                ai.Invoke("Attack",1.0f);
            }
        } else {
            Instantiate(missedHit, cell.transform.position, Quaternion.identity);
            canvas.DisplayFightingInfoText("AI missed");
            playerTurn = true;
            canvas.DisplayNextTurn(playerTurn);
        }
        ai.JotDown(status, cell);
    }
}
