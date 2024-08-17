using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class TetrisBoard : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrisPiece activePiece { get; private set; }

    [SerializeField] private TetrisPeicePreview peicePreview;
    [SerializeField] private SpriteRenderer gridSprite;
    [SerializeField] private AudioSource lineCleardSound;
    [SerializeField] private AudioSource defetedSound;
    [SerializeField] private AudioSource sizeShiftSound;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highestScoreText;
    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public int boardSizeMaxX = 14;
    public int boardSizeMinX = 6;
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public bool gameStarted = false;
    public int score = 0;
    public int level = 1;

    private TetrominoData nextPeiceData;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<TetrisPiece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("HighestScore"))
        {
            highestScoreText.text = "Highest Score: " + PlayerPrefs.GetInt("HighestScore");
        }
    }

    public void StartGame()
    {
        score = 0;
        chooseNextPeice(false);
        SpawnPiece();
        gameStarted = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C) && CanExpandBoard())
        {
            sizeShiftSound.Play();
            boardSize.x += 2;
            gridSprite.size = new Vector2(gridSprite.size.x + 2, gridSprite.size.y);
        }
        else if (Input.GetKeyDown(KeyCode.Z) && CanSquzzeBoard())
        {
            sizeShiftSound.Play();
            boardSize.x -= 2;
            gridSprite.size = new Vector2(gridSprite.size.x - 2, gridSprite.size.y);
            ClearLines();
        }
    }

    private void AdvanceLevel()
    {
        if (score > 500 && level == 1)
        {
            level = 2;
            activePiece.stepDelay -= activePiece.stepDelaySubtractionPerLevel;
        }
        if (score > 1000 && level == 2)
        {
            level = 3;
            activePiece.stepDelay -= activePiece.stepDelaySubtractionPerLevel;
        }
        if (score > 1500 && level == 3)
        {
            level = 4;
            activePiece.stepDelay -= activePiece.stepDelaySubtractionPerLevel;
        }
    }

    private bool CanExpandBoard()
    {
        return boardSize.x < boardSizeMaxX;
    }

    private bool CanSquzzeBoard()
    {
        int lastColumn = boardSize.x / 2;
        int firstColumn = -lastColumn;
        lastColumn -= 1; // 0-indexed

        if (activePiece.IsInColumn(lastColumn) || activePiece.IsInColumn(firstColumn))
        {
            return false;
        }

        return boardSize.x > boardSizeMinX;
    }

    private void chooseNextPeice(bool shouldUpdatePreview)
    {
        int random = Random.Range(0, tetrominoes.Length);
        nextPeiceData = tetrominoes[random];

        if (shouldUpdatePreview)
        {
            peicePreview.UpdatePrivew(nextPeiceData);
        }
    }

    public void SpawnPiece()
    {
        activePiece.Initialize(spawnPosition, nextPeiceData);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }

        chooseNextPeice(true);
    }

    private void SetHighestScore()
    {
        int highestScore = 0;
        if (PlayerPrefs.HasKey("HighestScore"))
        {
            highestScore = PlayerPrefs.GetInt("HighestScore");
        }

        if (score > highestScore)
        {
            PlayerPrefs.SetInt("HighestScore", score);
            highestScoreText.text = "Highest Score: " + score.ToString();
        }
    }

    public void RestartGame()
    {
        tilemap.ClearAllTiles();
        SetHighestScore();
        score = 0;
        scoreText.text = "Score: " + score.ToString();
        SpawnPiece();
    }

    public void GameOver()
    {
        defetedSound.Play();
        RestartGame();
    }

    public void Set(TetrisPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(TetrisPiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(TetrisPiece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesCleared = 0;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared += 1;
            }
            else
            {
                row++;
            }
        }

        if (linesCleared > 0)
        {
            lineCleardSound.Play();
            switch (linesCleared)
            {
                case 1:
                    score += 5 * boardSize.x * level;
                    break;
                case 2:
                    score += 13 * boardSize.x * level;
                    break;
                case 3:
                    score += 40 * boardSize.x * level;
                    break;
                case 4:
                    score += 160 * boardSize.x * level;
                    break;
            }
            scoreText.text = "Score: " + score.ToString();
            AdvanceLevel();
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        if (activePiece.IsInRow(row) && !activePiece.isLocked)
        {
            activePiece.Lock(true);
        }

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                if (activePiece.isLocked || !activePiece.IsInPosition(new Vector3Int(col, row, 0)))
                {
                    Vector3Int positionAbove = new Vector3Int(col, row + 1, 0);
                    TileBase above = tilemap.GetTile(positionAbove);
                    if (!activePiece.isLocked && activePiece.IsInPosition(positionAbove))
                    {
                        above = null;
                    }

                    Vector3Int position = new Vector3Int(col, row, 0);
                    tilemap.SetTile(position, above);
                }
            }

            row++;
        }
    }

}
