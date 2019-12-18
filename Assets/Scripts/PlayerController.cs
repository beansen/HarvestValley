using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class PlayerController : MonoBehaviour
{

	public FarmingManager FarmingManager;
	public GameObject Marker;

	public Texture2D[] Cursors;

	[Inject] private Inventory inventory;

	[Inject] private UiController uiController;

	private Animator animator;

	private const float speed = 4.5f;
	
	private readonly List<KeyCode> pressedKeys = new List<KeyCode>(4);

	private readonly KeyCode[] movementKeys = new[] {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W};

	private Dictionary<KeyCode, int> alphaKeys;
	
	private LayerMask terrainLayer;

	private int currentSelectedCursor;
	
	// Use this for initialization
	void Start ()
	{
		terrainLayer =~ LayerMask.NameToLayer("Terrain");
		animator = GetComponent<Animator>();
		alphaKeys = new Dictionary<KeyCode, int>(10)
		{
			{KeyCode.Alpha1, 0},
			{KeyCode.Alpha2, 1},
			{KeyCode.Alpha3, 2},
			{KeyCode.Alpha4, 3},
			{KeyCode.Alpha5, 4},
			{KeyCode.Alpha6, 5},
			{KeyCode.Alpha7, 6},
			{KeyCode.Alpha8, 7},
			{KeyCode.Alpha9, 8},
			{KeyCode.Alpha0, 9}
		};

		Cursor.SetCursor(Cursors[0], Vector2.zero, CursorMode.ForceSoftware);
	}
	
	// Update is called once per frame
	void Update () {
		HandleKeys();
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Vector3 mousePosition = Vector3.zero;
		
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
		{
			mousePosition = hit.point;
			mousePosition.x = Mathf.Floor(mousePosition.x);
			mousePosition.z = Mathf.Floor(mousePosition.z);

			if (FarmingManager.CanBeHarvested((int) mousePosition.x - 20, (int) mousePosition.z - 20))
			{
				if (currentSelectedCursor == 0)
				{
					currentSelectedCursor = 1;
					Cursor.SetCursor(Cursors[currentSelectedCursor], Vector2.zero, CursorMode.ForceSoftware);
				}
			}
			else
			{
				if (currentSelectedCursor == 1)
				{
					currentSelectedCursor = 0;
					Cursor.SetCursor(Cursors[currentSelectedCursor], Vector2.zero, CursorMode.ForceSoftware);
				}
			}

			if (IsMouseNearPlayer(mousePosition))
			{
				Marker.SetActive(true);
				mousePosition.x += 0.5f;
				mousePosition.z += 0.5f;
				mousePosition.y = 0.011f;
				Marker.transform.position = mousePosition;
			}
			else
			{
				Marker.SetActive(false);
			}
		}

		if (pressedKeys.Count > 0)
		{
			animator.SetBool("walking", true);
			KeyCode keyCode = pressedKeys[pressedKeys.Count - 1];
			transform.rotation = GetRotation(keyCode);
			transform.position += GetDirection(keyCode);
		}
		else
		{
			animator.SetBool("walking", false);
		}

		if (Input.GetMouseButtonDown(0))
		{
			mousePosition.x = Mathf.Floor(mousePosition.x);
			mousePosition.z = Mathf.Floor(mousePosition.z);

			if (IsMouseNearPlayer(mousePosition))
			{
				FarmingManager.Action(inventory.GetPlayerAction(), (int) mousePosition.x - 20, (int) mousePosition.z - 20, inventory.GetSeed());
			}
		}
	}

	private Quaternion GetRotation(KeyCode keyCode)
	{
		int y = 0;
		
		if (keyCode == KeyCode.A)
		{
			y = 270;
		}
		
		if (keyCode == KeyCode.S)
		{
			y = 180;
		}
		
		if (keyCode == KeyCode.D)
		{
			y = 90;
		}
		
		return Quaternion.Euler(0, y, 0);
	}

	private Vector3 GetDirection(KeyCode keyCode)
	{
		Vector3 direction = new Vector3(0, 0, Time.deltaTime * speed);
		
		if (keyCode == KeyCode.A)
		{
			direction.x = Time.deltaTime * -speed;
			direction.z = 0;
		}
		
		if (keyCode == KeyCode.S)
		{
			direction.z *= -1;
		}
		
		if (keyCode == KeyCode.D)
		{
			direction.x = Time.deltaTime * speed;
			direction.z = 0;
		}

		return direction;
	}

	private void HandleKeys()
	{
		foreach (KeyCode key in movementKeys)
		{
			if (Input.GetKeyDown(key))
			{
				pressedKeys.Add(key);
			}
			
			if (Input.GetKeyUp(key))
			{
				pressedKeys.Remove(key);
			}
		}

		foreach (KeyCode key in alphaKeys.Keys)
		{
			if (Input.GetKeyDown(key))
			{
				inventory.SelectItem(alphaKeys[key]);
				uiController.SwitchSelection(alphaKeys[key]);
			}
		}
		
		if (Input.GetKeyDown(KeyCode.B))
		{
			FarmingManager.UpdateFarmPatches();
		}
	}

	private bool IsMouseNearPlayer(Vector3 mousePos)
	{
		Vector3 playerPos = transform.position;
		playerPos.x = Mathf.Floor(playerPos.x);
		playerPos.z = Mathf.Floor(playerPos.z);

		int xDiff = (int) (mousePos.x - playerPos.x);
		int zDiff = (int) (mousePos.z - playerPos.z);

		return (xDiff >= -1 && xDiff <= 1) && (zDiff >= -1 && zDiff <= 1);
	}
}

public enum PlayerAction
{
	Plow,
	Water,
	Seed,
	None
}
