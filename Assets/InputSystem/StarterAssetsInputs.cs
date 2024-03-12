using System;
using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		private Vector2 oldMoveDirection;
		public Vector2 look;
		public bool sprint;
		[Tooltip("If the input direction was changed in the opposite direction instantly")]
		public bool movedOppositeDirection;
		
		[Header("Jumping")] 
		public bool jump = false;
		public float timeOfLastJump = -1;
		private float timeOfLastLastJump;
		private float timeOfLastRelease;
		public float timeHeldJumpButton;
		
		[Header("Special Moves")] 
		public bool diving;
		public bool backFlip;
		
		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("UI Settings")]
		public bool isInUiRange;
		public bool isUsingUi;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

		private void Start()
		{
			timeOfLastJump = -1;
		}

		private void Update()
		{
			if (jump) timeHeldJumpButton += Time.deltaTime;
		}

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
			
		}

		public void OnDive(InputValue value)
		{
			if (isInUiRange)
			{
				if (isUsingUi) diving = true;
				isUsingUi = true;
				return;
			}
			DiveInput(value.isPressed);
		}
		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnReset(InputValue value)
		{
			if(value.isPressed)GameObject.FindWithTag("ProgressSystem").GetComponent<ProgressSystem>().ResetPlayerToBeforeGates();
		}
		
		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnBackFlip(InputValue value)
		{
			BackFlipInput(value.isPressed);	
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			if (newMoveDirection * -1 == oldMoveDirection) movedOppositeDirection = true;
			if(newMoveDirection!=Vector2.zero)oldMoveDirection = newMoveDirection;
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			//Set time only one first press
			if (!jump && newJumpState )
			{
				timeOfLastLastJump = timeOfLastJump;
				timeOfLastJump = Time.time;
				
			}
			jump = newJumpState;
			if (!jump)
			{
				timeOfLastRelease = Time.time;
				timeHeldJumpButton = 0;
			}
		}

		public bool HasPlayerReleasedJumpButtonSinceLastPress(float lastTimeGrounded)
		{
			return timeOfLastLastJump <= timeOfLastRelease && timeOfLastRelease <= timeOfLastJump &&
			       timeOfLastLastJump <= lastTimeGrounded && lastTimeGrounded <= timeOfLastJump;
		}

		public bool HasReleasedJumpButtonSinceLastJump(float lastTimeGrounded)
		{
			return lastTimeGrounded <= timeOfLastRelease;
		}	
		
		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		private void DiveInput(bool newState)
		{
			diving = newState;
		}

		private void BackFlipInput(bool newstate)
		{
			backFlip = newstate;
		}
	}
	
}