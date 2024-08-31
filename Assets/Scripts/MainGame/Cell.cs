using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler {
	public enum CellType {
		Hole = -1,
		Blank = 0,
		Cobblestone = 1,
		CryingObsidian = 2,
		Dirt = 3,
		Endstone = 4,
		Mud = 5,
		Netherrock = 6,
		Sandstone = 7
	}

	public RectTransform rect;
	[SerializeField] private Image _image;

	private CellType _cellType;
	private Point _point;
	
	public CellType Type => _cellType;
	public Point Point => _point;

	public void Initialize(CellType cellType, Point point, Sprite sprite){
		_cellType = cellType;
		_point = point;
		_image.sprite = sprite;
	}

	public void OnPointerClick(PointerEventData eventData) {
        BoardService.Instance.OnCellClicked(this);
    }

	public void SetType(CellType cellType, Sprite sprite) {
        _cellType = cellType;
        _image.sprite = sprite;
    }
}