using UnityEngine;
using System.Collections;

public class Tetrimino : MonoBehaviour {

    float fall = 0;
    public static double fallSpeed = 0.8;
    public bool allowRotation = true;
    public bool limitRotation = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        CheckUserInput();
    }

    void CheckUserInput() {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!CheckIsValidPosition())
            {
                transform.position += new Vector3(-1, 0, 0);
            }
            else
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!CheckIsValidPosition())
            {
                transform.position += new Vector3(1, 0, 0);
            }
            else
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (allowRotation)
            {
                if (limitRotation)
                {
                    if(transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
                if (!CheckIsValidPosition())
                {
                    if (limitRotation)
                    {
                        if (transform.rotation.eulerAngles.z >= 90)
                        {
                            transform.Rotate(0, 0, -90);
                        }
                        else
                        {
                            transform.Rotate(0, 0, 90);
                        }
                    }
                    else
                    {
                        transform.Rotate(0, 0, -90);
                    }
                }
                else
                {
                    FindObjectOfType<Game>().UpdateGrid(this);
                }
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow) || (Time.time-fall) >= fallSpeed)
        {
            transform.position += new Vector3(0, -1, 0);
            if (!CheckIsValidPosition())
            {
                transform.position += new Vector3(0, 1, 0);
                FindObjectOfType<Game>().DeleteRow();

                if(FindObjectOfType<Game>().CheckIsAboveGrid(this))
                {
                    FindObjectOfType<Game>().GameOver();
                }

                enabled = false;
                FindObjectOfType<Game>().SpawnNextTetrimino();
            }
            else
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            fall = Time.time;
        }
        else if(Input.GetKeyDown(KeyCode.LeftShift))
        {
           
            transform.position = new Vector2(-9f, 6f);
            enabled = false;
            FindObjectOfType<Game>().PutOnHold(this.gameObject);
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
          
        }
    }

    public void SetEnabled(bool set)
    {
        enabled = set;
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            if(FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
            {
                return false;
            }
            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null
                && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    public static void LevelUpSpeed()
    {
        fallSpeed -= 0.1;
    }
}
