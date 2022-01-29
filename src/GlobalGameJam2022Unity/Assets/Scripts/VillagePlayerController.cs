using UnityEngine;

[RequireComponent(typeof(VillageCharacterController))]
public class VillagePlayerController : MonoBehaviour
{
	private VillageCharacterController villageCharacterController;

	public VillageNPC killedCharacter;
	public bool IsBloodlusted = false;

	private void Awake()
	{
		villageCharacterController = GetComponent<VillageCharacterController>();
	}

	private void Update()
	{
		var inputMovement = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical"));

		villageCharacterController.SetControllerInput(inputMovement);
	}
}
