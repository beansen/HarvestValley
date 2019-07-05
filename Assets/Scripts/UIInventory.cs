using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{

	public Sprite[] Borders;

	private int currentSelection = 0;

	private Image[] borderImages;
	private Image[] foregroundImages;

	void Start()
	{
		int childCount = transform.childCount;
		borderImages = new Image[childCount];
		foregroundImages = new Image[childCount];

		for (int i = 0; i < childCount; i++)
		{
			borderImages[i] = transform.GetChild(i).GetComponent<Image>();
		}
	}

	public void SwitchSelection(int index)
	{
		if (index != currentSelection)
		{
			borderImages[currentSelection].sprite = Borders[0];
			borderImages[index].sprite = Borders[1];
			currentSelection = index;
		}
	}
}
