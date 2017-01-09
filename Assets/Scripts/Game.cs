using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    
    public static int gridWidth = 10;
    public static int gridHeight = 20;
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    public Text hud_score;
    public Text hud_lines;
    public Text hud_level;
    public int currentScore = 0;
    public int currentLevel = 0;
    public int linesCleared = 0;
    public int levelCounter = 0;
    public const int SCORE_ONE_LINE = 40;
    public const int SCORE_TWO_LINE = 100;
    public const int SCORE_THREE_LINE = 300;
    public const int SCORE_FOUR_LINE = 1200;
    public int numFullRowsThisTurn = 0;

    private GameObject nextTetrimino;
    private GameObject previewTetrimino;
    private GameObject holdTetrimino = null;

    private bool hasGameStarted = false;

    private Vector2 previewTetPosit = new Vector2(-9f, 12f);



    // Use this for initialization
    void Start () {
        SpawnNextTetrimino();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateScore();
        UpdateUI();
	}

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_lines.text = linesCleared.ToString();
        hud_level.text = currentLevel.ToString();
    }

    public void UpdateScore()
    {
        if(numFullRowsThisTurn > 0)
        {
            if(numFullRowsThisTurn == 1)
            {
                ClearedLine(SCORE_ONE_LINE);
            }
            else if (numFullRowsThisTurn == 2)
            {
                ClearedLine(SCORE_TWO_LINE); 
            }
            else if (numFullRowsThisTurn == 3)
            {
                ClearedLine(SCORE_THREE_LINE);
            }
            else if (numFullRowsThisTurn == 4)
            {
                ClearedLine(SCORE_FOUR_LINE);
            }

            linesCleared += numFullRowsThisTurn;
            levelCounter += numFullRowsThisTurn;
            if(levelCounter >= 10)
            {
                LevelUp();
                levelCounter = 0;
            }

            numFullRowsThisTurn = 0;
        }
    }

    public void ClearedLine(int lineScore) {
        currentScore += (lineScore * (currentLevel + 1));
    }

    public void LevelUp()
    {
        currentLevel += 1;
        Tetrimino.LevelUpSpeed();
    }

    public bool CheckIsAboveGrid(Tetrimino tet)
    {
        for(int x = 0; x<gridWidth; ++x)
        {
            foreach(Transform mino in tet.transform)
            {
                Vector2 pos = Round(mino.position);
                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRow(int y)
    {
        for(int x = 0; x < gridWidth; ++x)
        {
            if(grid[x,y] == null)
            {
                return false;
            }
        }
        numFullRowsThisTurn++;
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if(grid[x,y] != null)
            {
                grid[x,(y-1)] = grid[x, y];
                grid[x, y] = null;
                grid[x, (y-1)].position += new Vector3(0, -1, 0);
            }

        }
    }

    public void MoveAllRowsDown(int y)
    {
        for(int i=y; i<gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for(int y = 0; y<gridHeight; ++y)
        {
            if (IsFullRow(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }

    public void UpdateGrid(Tetrimino tet)
    {
        for(int y=0; y<gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tet.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach(Transform mino in tet.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if(pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }
    
    public void SpawnNextTetrimino()
    {
        if (!hasGameStarted)
        {
            hasGameStarted = true;
            nextTetrimino = (GameObject)Instantiate(Resources.Load(GetRandomTetrimino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetrimino = (GameObject)Instantiate(Resources.Load(GetRandomTetrimino(), typeof(GameObject)), previewTetPosit, Quaternion.identity);
            previewTetrimino.GetComponent<Tetrimino>().enabled = false;
        }
        else
        {
            previewTetrimino.transform.localPosition = new Vector2(5.0f, 19.0f);
            nextTetrimino = previewTetrimino;
            nextTetrimino.GetComponent<Tetrimino>().enabled = true;
            previewTetrimino = (GameObject)Instantiate(Resources.Load(GetRandomTetrimino(), typeof(GameObject)), previewTetPosit, Quaternion.identity);
            previewTetrimino.GetComponent<Tetrimino>().enabled = false;

        }
    }

    public void PutOnHold(GameObject toHold)
    {
        if(holdTetrimino != null)
        {
            nextTetrimino = holdTetrimino;
            nextTetrimino.transform.localPosition = new Vector2(5.0f, 19.0f);
            nextTetrimino.GetComponent<Tetrimino>().enabled = true;
            holdTetrimino = toHold;   
        } 
        else
        {
            holdTetrimino = toHold;
            SpawnNextTetrimino();
        }
    }
   
    public bool CheckIsInsideGrid(Vector2 posit)
    {
        return ( (int)posit.x >= 0 && (int)posit.x < gridWidth && (int)posit.y >= 0 );
    }

    public Vector2 Round(Vector2 posit)
    {
        return new Vector2(Mathf.Round(posit.x), Mathf.Round(posit.y));
    }

    string GetRandomTetrimino()
    {
        int randomTet = Random.Range(1, 8);
        string randomName = "Prefabs/Tetrimino_long";
        switch (randomTet)
        {
            case 1:
                randomName = "Prefabs/Tetrimino_j";
                break;
            case 2:
                randomName = "Prefabs/Tetrimino_l";
                break;
            case 3:
                randomName = "Prefabs/Tetrimino_long";
                break;
            case 4:
                randomName = "Prefabs/Tetrimino_s";
                break;
            case 5:
                randomName = "Prefabs/Tetrimino_square";
                break;
            case 6:
                randomName = "Prefabs/Tetrimino_t";
                break;
            case 7:
                randomName = "Prefabs/Tetrimino_z";
                break;
        }
        return randomName;  
    }

    public void GameOver()
    {
        //Application.LoadLevel("GameOver");
        SceneManager.LoadScene("GameOver");
    }
}
