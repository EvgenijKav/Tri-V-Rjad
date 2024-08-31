using UnityEngine;
using UnityEngine.UI;
using StaticData;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using TMPro;

public class BoardService : MonoBehaviour {
	[SerializeField] private RectTransform _boardRect;
	[SerializeField] private Cell _cellPrefab;
	[SerializeField] private Sprite[] _cellSprites;
    [SerializeField] private GameOverScreen _gameOverScreen;
    [SerializeField] private TextMeshProUGUI scoreText;

	private Cell[,] _cells;
	private Cell _firstSelectedCell;
    private Cell _secondSelectedCell;
    private int _score;
    private int _bestScore;

	public static BoardService Instance {get; private set;}

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

	private void Start() {
		_cells = new Cell[Config.BoardWidth, Config.BoardHeight];

		for(int x = 0; x < Config.BoardWidth; x++) {
			for(int y = 0; y < Config.BoardHeight; y++) {
				var cell = InstantiateCell();
				var point = new Point(x, y);
				cell.rect.anchoredPosition = GetBoardPositionFromPoint(point);
				var cellType = GetValidRandomCellType(x, y);
				cell.Initialize(cellType, point, _cellSprites[(int)(cellType - 1)]);
				_cells[x, y] = cell;
			}
		}
        _score = 0;
        _bestScore = PlayerPrefs.GetInt("Лучший счёт", 0);
        UpdateScoreText();

        CheckAndRemoveAllMatches();
	}

    private void UpdateScoreText() {
        scoreText.text = "Счёт: " + _score;
    }

    private void IncrementScore() {
        _score++;
        UpdateScoreText();
    }

	private Cell.CellType GetValidRandomCellType(int x, int y) {
        Cell.CellType cellType;
        do {
            cellType = GetRandomCellType();
        } while (IsCreatingMatch(x, y, cellType));
        return cellType;
    }

    private bool IsCreatingMatch(int x, int y, Cell.CellType cellType) {
        if (x >= 2 && _cells[x - 1, y]?.Type == cellType && _cells[x - 2, y]?.Type == cellType) {
            return true;
        }
        if (y >= 2 && _cells[x, y - 1]?.Type == cellType && _cells[x, y - 2]?.Type == cellType) {
            return true;
        }
        return false;
    }

	private Cell.CellType GetRandomCellType()
		=> (Cell.CellType)(Random.Range(1, _cellSprites.Length) + 1);
	
	private Cell InstantiateCell()
		=> Instantiate(_cellPrefab, _boardRect);

	private Vector2 GetBoardPositionFromPoint(Point point) {
		return new Vector2(Config.PieceSize / 2 + Config.PieceSize * point.x,
							-Config.PieceSize / 2 - Config.PieceSize * point.y);
	}

	public void OnCellClicked(Cell cell) {
        if (_firstSelectedCell == null) {
            _firstSelectedCell = cell;
        } else {
            if (AreNeighbors(_firstSelectedCell, cell)) {
            _secondSelectedCell = cell;
				if (CanSwapAndMatch(_firstSelectedCell, _secondSelectedCell)) {
            		SwapCells(_firstSelectedCell, _secondSelectedCell);
					CheckAndRemoveAllMatches();
                    CheckForNoMoreMoves();
				}
        	}
            _firstSelectedCell = null;
            _secondSelectedCell = null;
        }
    }

	private bool AreNeighbors(Cell firstCell, Cell secondCell) {
        Point firstPoint = firstCell.Point;
        Point secondPoint = secondCell.Point;

		return (Mathf.Abs(firstPoint.x - secondPoint.x) == 1 && firstPoint.y == secondPoint.y) || 
               (Mathf.Abs(firstPoint.y - secondPoint.y) == 1 && firstPoint.x == secondPoint.x);
	}

	private bool CanSwapAndMatch(Cell firstCell, Cell secondCell) {
        SwapCells(firstCell, secondCell, true);
        bool isMatch = CheckForPotentialMatches();
        SwapCells(firstCell, secondCell, true);
        return isMatch;
    }

	private void SwapCells(Cell firstCell, Cell secondCell, bool isVirtual = false) {
		var firstType = firstCell.Type;
		var firstSprite = firstCell.GetComponent<Image>().sprite;

		firstCell.SetType(secondCell.Type, secondCell.GetComponent<Image>().sprite);
        secondCell.SetType(firstType, firstSprite);

		if (!isVirtual) {
			_cells[firstCell.Point.x, firstCell.Point.y] = firstCell;
        	_cells[secondCell.Point.x, secondCell.Point.y] = secondCell;
		}
	}

	private bool CheckForPotentialMatches() {
        for (int x = 0; x < Config.BoardWidth; x++) {
            for (int y = 0; y < Config.BoardHeight; y++) {
                if (IsPotentialMatch(x, y)) {
                    return true;
                }
            }
        }
        return false;
    }

	private bool IsPotentialMatch(int x, int y) {
        if (x < Config.BoardWidth - 2) {
            if (_cells[x, y].Type == _cells[x + 1, y].Type && _cells[x, y].Type == _cells[x + 2, y].Type) {
                return true;
            }
        }
        if (y < Config.BoardHeight - 2) {
            if (_cells[x, y].Type == _cells[x, y + 1].Type && _cells[x, y].Type == _cells[x, y + 2].Type) {
                return true;
            }
        }
        return false;
    }

	private void CheckAndRemoveAllMatches() {
        bool foundMatch;
        do {
            foundMatch = false;
            for (int x = 0; x < Config.BoardWidth; x++) {
                for (int y = 0; y < Config.BoardHeight; y++) {
                    if (CheckHorizontalMatches(x, y)) {
                        foundMatch = true;
                    }
                    if (CheckVerticalMatches(x, y)) {
                        foundMatch = true;
                    }
                }
            }
        } while (foundMatch);
    }

	private bool CheckHorizontalMatches(int x, int y) {
    	if (x < Config.BoardWidth - 2) {
        	if (_cells[x, y].Type == _cells[x + 1, y].Type && _cells[x, y].Type == _cells[x + 2, y].Type) {
				RemoveCell(x, y);
                RemoveCell(x + 1, y);
                RemoveCell(x + 2, y);

                ReplaceCell(x, y);
                ReplaceCell(x + 1, y);
                ReplaceCell(x + 2, y);

                IncrementScore();

                return true;
			}
    	} return false;
	}

	private bool CheckVerticalMatches(int x, int y) {
    	if (y < Config.BoardHeight - 2) {
        	if (_cells[x, y].Type == _cells[x, y + 1].Type && _cells[x, y].Type == _cells[x, y + 2].Type) {
            	RemoveCell(x, y);
                RemoveCell(x, y + 1);
                RemoveCell(x, y + 2);

                ReplaceCell(x, y);
                ReplaceCell(x, y + 1);
                ReplaceCell(x, y + 2);

                IncrementScore();

                return true;
        	}
    	} return false;
	}

	private void RemoveCell(int x, int y) {
        if (_cells[x, y] != null) {
            Destroy(_cells[x, y].gameObject);
            _cells[x, y] = null;
        }
    }

	private void ReplaceCell(int x, int y) {
        var cell = InstantiateCell();
        var point = new Point(x, y);
        cell.rect.anchoredPosition = GetBoardPositionFromPoint(point);
        var cellType = GetValidRandomCellType(x, y);
        cell.Initialize(cellType, point, _cellSprites[(int)(cellType - 1)]);
        _cells[x, y] = cell;
    }

    private void CheckForNoMoreMoves() {
        if (!CheckForPotentialMatches()) {
            if (_score > _bestScore) {
                PlayerPrefs.SetInt("Лучший счёт", _score);
            }
            _bestScore = PlayerPrefs.GetInt("Лучший счёт", 0);
            _gameOverScreen.ShowGameOver(_score, _bestScore);
        }
    }
}