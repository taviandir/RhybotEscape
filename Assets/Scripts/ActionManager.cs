using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public ActionArrow upArrow;
    public ActionArrow leftArrow;
    public ActionArrow rightArrow;
    public ActionArrow downArrow;

    public bool goUp;
    public bool goLeft;
    public bool goRight;
    public bool goDown;

    void Awake()
    {
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("ActionArrow");
        foreach (var arrow in arrows)
        {
            if (arrow.name.ToLowerInvariant().Contains("up"))
            {
                upArrow = arrow.GetComponent<ActionArrow>();
            }
            else if (arrow.name.ToLowerInvariant().Contains("left"))
            {
                leftArrow = arrow.GetComponent<ActionArrow>();
            }
            else if (arrow.name.ToLowerInvariant().Contains("right"))
            {
                rightArrow = arrow.GetComponent<ActionArrow>();
            }
            else if (arrow.name.ToLowerInvariant().Contains("down"))
            {
                downArrow = arrow.GetComponent<ActionArrow>();
            }
        }
    }

    void Update()
    {
        if (CheckMoveUpInput())
        {
            SetUpMove(true);
            SetLeftMove(false);
            SetRightMove(false);
            SetDownMove(false);

            if (CheckMoveRightInput())
            {
                SetRightMove(true);
            }
            else if (CheckMoveLeftInput())
            {
                SetLeftMove(true);
            }
        }
        else if (CheckMoveDownInput())
        {
            SetUpMove(false);
            SetLeftMove(false);
            SetRightMove(false);
            SetDownMove(true);

            if (CheckMoveRightInput())
            {
                SetRightMove(true);
            }
            else if (CheckMoveLeftInput())
            {
                SetLeftMove(true);
            }
        }
        else if (CheckMoveRightInput())
        {
            SetUpMove(false);
            SetLeftMove(false);
            SetRightMove(true);
            SetDownMove(false);
        }
        else if (CheckMoveLeftInput())
        {
            SetUpMove(false);
            SetLeftMove(true);
            SetRightMove(false);
            SetDownMove(false);
        }
    }

    public void PerformAction(bool turbo)
    {
        if (IsMovement())
        {
            // move player
            int xDir = goLeft ? -1 : goRight ? 1 : 0;
            int yDir = goDown ? -1 : goUp    ? 1 : 0;
            //xDir *= (turbo ? 2 : 1);
            //yDir *= (turbo ? 2 : 1);

            GameManager.instance.player.AttemptMove(xDir, yDir);
        }
    }

    private bool IsMovement()
    {
        return goUp || goLeft || goRight || goDown;
    }

    private bool CheckMoveUpInput()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
    }

    private bool CheckMoveLeftInput()
    {
        return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
    }

    private bool CheckMoveRightInput()
    {
        return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
    }

    private bool CheckMoveDownInput()
    {
        return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
    }

    private void SetUpMove(bool value)
    {
        goUp = value;
        upArrow.SetHighlight(value);
    }

    private void SetLeftMove(bool value)
    {
        goLeft = value;
        leftArrow.SetHighlight(value);
    }

    private void SetRightMove(bool value)
    {
        goRight = value;
        rightArrow.SetHighlight(value);
    }

    private void SetDownMove(bool value)
    {
        goDown = value;
        downArrow.SetHighlight(value);
    }

    private void DeselectAllMove()
    {
        SetUpMove(false);
        SetLeftMove(false);
        SetRightMove(false);
        SetDownMove(false);
    }
}
