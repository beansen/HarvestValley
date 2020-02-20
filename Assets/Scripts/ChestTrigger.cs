using UnityEngine;
using Zenject;

public class ChestTrigger : MonoBehaviour
{

	[Inject] private UiController uiController;
	private Animator animator;
	private readonly int openTrigger = Animator.StringToHash("Open");
	private readonly int closeTrigger = Animator.StringToHash("Close");


	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator>();
	}
	
	

	private void OnTriggerEnter(Collider other)
	{
		if (other.name.Equals("Character"))
		{
			animator.SetTrigger(openTrigger);
			uiController.ChestMode = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.name.Equals("Character"))
		{
			animator.SetTrigger(closeTrigger);
			uiController.ChestMode = false;
		}
	}
}
