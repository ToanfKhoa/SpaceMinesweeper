using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;


public class Game : MonoBehaviour
{
    #region Singleton
    private static Game _instance;
    public static Game Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
            _instance = this;

        board = GetComponentInChildren<Board>();

        if (PlayerPrefs.HasKey(DATA_KEY))
        {
            string savedJSON = PlayerPrefs.GetString(DATA_KEY);
            Debug.Log("Luu: " + savedJSON);
            this._userDatas = JsonUtility.FromJson<UserDatas>(savedJSON);
        }
        else
        {
            this._userDatas = new UserDatas();
            Debug.Log("Chua co Data luu: ");
        }
    }

    #endregion Singleton

    public World _world;

    public int width = 10;
    public int height = 7;
    public int mineCount = 10;
    public int rockCount = 4;

    public Board board;
    public CellGrid grid;
    private bool gameOver;
    public bool isGenerated;

    public TextMeshProUGUI textGold;
    public TextMeshProUGUI textDiamond;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textComplete;

    public GameObject goldPrefab;
    public GameObject diamondPrefab;
    

    public RectTransform bagIcon;
    public Vector3 initCameraPosition;

    public GameObject sweepScreen;
    public bool isImageVisible = false;
    public int sweepMode = -1;

    public GameObject gameOverScreen;
    public GameObject gameWinScreen;

    public UserDatas _userDatas;
    public const string DATA_KEY = "DATA_KEY";

    public GameObject UIDirtExplodePrefab;
    public GameObject UIMineExplodePrefab;
    public GameObject UIDirtDigPrefab;
    private bool isHaveUiDig = false;

    public GameObject bagScreen;
    public GameObject outOfHeartScreen;

    public GameObject bugPrefab;

    private const float HALF_CELL_WIDTH = 0.5f;
    private const float HALF_CAMERA = 0.5f;
    private const float CAMERA_Z = -10f;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }


    public void Start()
    {
        

        this.textGold.text = _userDatas.gold.ToString();
        this.textDiamond.text = _userDatas.diamond.ToString();
        this.textLevel.text = _userDatas.level.ToString();

        NewGame();

    }

    public void NewGame()
    {
        if (Game.Instance._userDatas.heart <= 0) 
        {
            gameOverScreen.SetActive(true);
            outOfHeartScreen.SetActive(true);
        }          
            
        StopAllCoroutines();
        if (_world != null)
        {
            if (_world.levels[_userDatas.level] != null)
            {
                width = _world.levels[_userDatas.level].width;
                height = _world.levels[_userDatas.level].height;
                mineCount = _world.levels[_userDatas.level].mineCount;
                rockCount = _world.levels[_userDatas.level].rockCount;
            }
        }
        Camera.main.transform.position = new Vector3(width * HALF_CAMERA, height * HALF_CAMERA, CAMERA_Z);
        initCameraPosition = Camera.main.transform.position;

        gameOver = false;
        isGenerated = false;
        
        holdTime = 0f;

        grid = new CellGrid(width, height);
        board.InitiateDraw(grid, rockCount);

        CameraPanAndZoom cameraController = FindObjectOfType<CameraPanAndZoom>();
        if (cameraController != null)
        {
            cameraController.ResetCamera();
        }
        else
        {
            Debug.LogError("No CameraPanAndZoom script found in the scene!");
        }
    }

    public void SetLevel()
    {
        if(_userDatas.level < 14)
        _userDatas.level++;
        SaveData();
    } 
    
        

    private bool isMouseButtonDown = false;
    private float holdTime = 0f;
    
    private void Update()
    {
        
        if (!gameOver)
        {
            Sweep();
            if(sweepMode == -1)
                RevealAndFlag();
            
        }
        AdjustText();
    }

    private const float UI_DIG_TIME = 0.3f;
    public void RevealAndFlag()
    {
        if(EventSystem.current.currentSelectedGameObject == null && bagScreen.activeSelf == false)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                isMouseButtonDown = true;
                
                //dig
            }

            if (TryGetCellAtMousePosition(out Cell cell) && isMouseButtonDown == true)
            {
                if(isHaveUiDig == false && cell.isRevealed == false)
                {
                    AudioManager.Instance.DigSound();
                    Vector3 centerCellPositiona = new Vector3(cell.position.x + HALF_CELL_WIDTH, 
                                                            cell.position.y + HALF_CELL_WIDTH, 
                                                            cell.position.z);
                    GameObject a = Instantiate(UIDirtDigPrefab, centerCellPositiona, Quaternion.identity);
                    Destroy(a, UI_DIG_TIME);
                    Invoke("DestroyUIdig", UI_DIG_TIME);
                    isHaveUiDig = true;
                }               
            }
            
            if (Input.GetMouseButtonUp(0))
            {

                if (holdTime < _userDatas.timeDig)
                {
                    Flag();
                }

                isMouseButtonDown = false;
                holdTime = 0f;

                isHaveUiDig = false;
            }

            if (isMouseButtonDown)
            {
                holdTime += Time.deltaTime;
                if (holdTime >= _userDatas.timeDig)
                {
                    Reveal();
                }
            }
        }    
    }
    
    public void DestroyUIdig()
    {
        isHaveUiDig = false;
    }
        
    public void AdjustText() 
    {
        textGold.text = _userDatas.gold.ToString();
        textDiamond.text = _userDatas.diamond.ToString();
        //textheart.text = heart.ToString();
        textLevel.text = _userDatas.level.ToString();
    }
        
    private void Reveal()
    {
        if (TryGetCellAtMousePosition(out Cell cell))
        {
            if (!isGenerated)
            {
                grid.GenerateMines(cell, mineCount);
                grid.GenerateNumbers();
                isGenerated = true;
            }
            Reveal(cell);
        }
        
    }

    public void Reveal(Cell cell)
    {
        
        if (cell.isRevealed) return;
        if (cell.isFlagged) return;
        Vector3 centerCellPositiona = new Vector3(cell.position.x + HALF_CELL_WIDTH, cell.position.y + HALF_CELL_WIDTH, cell.position.z);
        GameObject a = Instantiate(UIDirtExplodePrefab, centerCellPositiona, Quaternion.identity);
        Destroy(a, 1.5f);

        AudioManager.Instance.RockSmashSound();

        switch (cell.type)
        {
            case Cell.Type.Mine:

                Explode(cell);
                break;

            case Cell.Type.Empty:
                StartCoroutine(Flood(cell));
                CollectOre(cell);
                CheckWinCondition();
                break;

            default:
                cell.isRevealed = true;
                CollectOre(cell);
                CheckWinCondition();
                break;
        }
        
        board.Draw(grid);
    }


    private const float FLOOD_DELAY_TIME = 0.3f;
    private IEnumerator Flood(Cell cell)
    {
        if (gameOver) yield break;
        if (cell.isRevealed) yield break;
        if (cell.type == Cell.Type.Mine) yield break;
        if (cell.type == Cell.Type.Block) yield break;

        cell.isRevealed = true;
        Vector3 centerCellPosition = new Vector3(cell.position.x + HALF_CELL_WIDTH, cell.position.y + HALF_CELL_WIDTH, cell.position.z);
        GameObject a = Instantiate(UIDirtExplodePrefab, centerCellPosition, Quaternion.identity);
        Destroy(a, 2f);

        AudioManager.Instance.RockSmashSound();

        board.Draw(grid);
        CollectOre(cell);

        yield return null;

        CheckWinCondition();

        if (cell.type == Cell.Type.Empty)
        {
            yield return new WaitForSeconds(FLOOD_DELAY_TIME);
            if (grid.TryGetCell(cell.position.x - 1, cell.position.y, out Cell left)) {
                StartCoroutine(Flood(left));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y, out Cell right)) {
                StartCoroutine(Flood(right));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y - 1, out Cell down)) {
                StartCoroutine(Flood(down));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y + 1, out Cell up)) {
                StartCoroutine(Flood(up));
            }
            /*if (grid.TryGetCell(cell.position.x - 1, cell.position.y - 1, out Cell topLeft))
            {
                StartCoroutine(Flood(topLeft));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y - 1, out Cell topRight))
            {
                StartCoroutine(Flood(topRight));
            }
            if (grid.TryGetCell(cell.position.x - 1, cell.position.y + 1, out Cell bottomLeft))
            {
                StartCoroutine(Flood(bottomLeft));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y + 1, out Cell bottomRight))
            {
                StartCoroutine(Flood(bottomRight));
            }*/
        }
        
    }

    private void Flag()
    {
        if (!TryGetCellAtMousePosition(out Cell cell)) return;
        if (cell.isRevealed) return;
        cell.isFlagged = !cell.isFlagged;
        board.Draw(grid);
    }

    private void Explode(Cell cell)
    {
        Debug.Log("Game Over!");
        gameOver = true;

        // Set the mine as exploded
        cell.isExploded = true;
        cell.isRevealed = true;

       

        // Reveal all other mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.isRevealed = true;
                }

            }
        }
        
        Invoke("AllMineExplode", 1);
        Invoke("Lose", 1.5f);
    }

    public void AllMineExplode()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.isRevealed = true;

                    Vector3 centerCellPositiona2 = new Vector3(cell.position.x + HALF_CELL_WIDTH, 
                                                               cell.position.y + HALF_CELL_WIDTH, 
                                                               cell.position.z);
                    GameObject b = Instantiate(UIMineExplodePrefab, centerCellPositiona2, Quaternion.identity);
                    Destroy(b, 1.5f);
                }

            }
        }
        AudioManager.Instance.ExplodeSound();
    }
        

    public void Lose()
    {
        float totalarea = 0;
        float sumarea = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell.isRevealed 
                    && cell.type != Cell.Type.Block 
                    &&  cell.type != Cell.Type.Mine)
                {
                    totalarea++;
                    sumarea++;                 
                }
                if (cell.isRevealed == false 
                    && cell.type != Cell.Type.Block 
                    && cell.type != Cell.Type.Mine)
                    sumarea++;
            }
        }

        if ((totalarea / sumarea * 100) >= 100)
            textComplete.text = (totalarea / (totalarea + 2) * 100).ToString("F2") + "%";
        else
        textComplete.text = (totalarea / sumarea * 100).ToString("F2") + "%";
        gameOverScreen.SetActive(true);
        _userDatas.heart--;
        SaveData();
        Lives.Instance.LoseHeart();
    }
    
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                // All non-mine cells must be revealed to have won
                if (cell.type != Cell.Type.Mine && !cell.isRevealed) {
                    return; // no win
                }
            }
        }

        Invoke("Win", 1);
    }
    
    public void Win()
    {

        Debug.Log("Winner!");

        // Flag all the mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.isFlagged = true;
                }
            }
        }

        gameOver = true;
        AudioManager.Instance.WinSound();
        gameWinScreen.SetActive(true);
    }
    private bool TryGetCellAtMousePosition(out Cell cell)
    {
        if (Input.mousePosition.x < 0 ||
            Input.mousePosition.y < 0 ||
            Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y > Screen.height)
        {
            cell = null;
            return false;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tileMap.WorldToCell(worldPosition);
        return grid.TryGetCell(cellPosition.x, cellPosition.y, out cell); 
    }


    int probality = 100;
    private void CollectOre(Cell cell)
    {
        int bonusgold = 0, bonusdiamond = 0;
        int dice = Random.Range(1, probality);
        if (dice <= _userDatas.probalityGold)
            bonusgold = Random.Range(0, 5);
        if (dice <= _userDatas.probalityDiamond)
            bonusdiamond = Random.Range(0, 2);

        for(int i = 0; i < bonusgold; i++)
        {
            InstantiateAndMove(goldPrefab, cell.position);
            _userDatas.gold += 1;

            AudioManager.Instance.OreSound();
        }
        for (int i = 0; i < bonusdiamond; i++)
        {
            InstantiateAndMove(diamondPrefab, cell.position);
            _userDatas.diamond += 1;

            AudioManager.Instance.RareOreSound();
        }

        if(Random.value > 0.98)        //ti le spawn bug
            SpawnBug(cell);
        SaveData();
    }

    private IEnumerator MoveAndHandleObject(GameObject obj)
    {
        float moveDuration = 1.0f;
        float elapsedTime = 0.0f;

        Vector3 startPosition = obj.transform.position;
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.z = 0; // Keep movement in the XY plane
        float randomDistance = Random.Range(1.5f, 3.0f);
        Vector3 moveVector = randomDirection.normalized * randomDistance;
        Vector3 endPosition = startPosition + moveVector;

        // Calculate the deceleration needed to stop at the end position
        float deceleration = (2 * randomDistance) / (moveDuration * moveDuration);

        while (elapsedTime < moveDuration)
        {
            // Update the position without using Lerp, instead applying deceleration
            float currentSpeed = Mathf.Clamp(moveVector.magnitude - deceleration * elapsedTime, 0, moveVector.magnitude);
            obj.transform.position += 3 * moveVector.normalized * currentSpeed * Time.deltaTime;

            // Check if the object has moved beyond the end position
            if ((obj.transform.position - startPosition).sqrMagnitude >= moveVector.sqrMagnitude)
            {
                obj.transform.position = endPosition; // Clamp to end position
                break; // Exit the loop
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }

    public void InstantiateAndMove(GameObject prefab, Vector3 position)
    {
        Vector3 centerCellPosition = new Vector3(position.x + HALF_CELL_WIDTH, position.y + HALF_CELL_WIDTH, position.z);
        GameObject obj = Instantiate(prefab, centerCellPosition, Quaternion.identity);
        StartCoroutine(MoveAndHandleObject(obj));
    }

    private void Sweep() 
    {
        if (sweepMode == -1)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = grid[x, y];
                    if (cell.type == Cell.Type.Number && cell.isRevealed == true)
                    {
                        cell.type = Cell.Type.NumEmpty;
                        cell.isNumberEmpty = true;
                    }
                }
            }
        }
        else if (sweepMode == 1)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = grid[x, y];
                    if (cell.type == Cell.Type.NumEmpty)
                    {
                        cell.type = Cell.Type.Number;
                        cell.isNumberEmpty = false;
                    }
                }
            }
        }
        board.Draw(grid);
    }


    public void SaveData()
    {
        //JSON hóa data clas
        string dataJSON = JsonUtility.ToJson(this._userDatas);
        Debug.Log("DATA " + dataJSON);
        //save JSON string
        PlayerPrefs.SetString(DATA_KEY, dataJSON);

    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void SpawnBug(Cell cell)
    {
        GameObject bug = Instantiate(bugPrefab, cell.position, Quaternion.identity);
    }
}
        



