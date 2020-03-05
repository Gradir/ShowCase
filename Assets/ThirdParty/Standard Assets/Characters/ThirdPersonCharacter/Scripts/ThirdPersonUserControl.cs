using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour
{
	[SerializeField] private GameObject sprintParticlesObject = null;
	public bool sprintUnlocked;

	private ThirdPersonCharacter m_Character;
	private Transform m_Cam;
	private Vector3 m_CamForward;
	private Vector3 m_Move;
	private bool m_Jump;
	private bool isSprinting;

	private void Start()
	{
		if (Camera.main != null)
		{
			m_Cam = Camera.main.transform;
		}

		m_Character = GetComponent<ThirdPersonCharacter>();
	}

	private void Update()
	{
		if (!m_Jump)
		{
			m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
		}
	}

	private void FixedUpdate()
	{
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");
		bool crouch = Input.GetKey(KeyCode.C);

		if (m_Cam != null)
		{
			m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
			m_Move = v * m_CamForward + h * m_Cam.right;
		}
		else
		{
			m_Move = v * Vector3.forward + h * Vector3.right;
		}

		if (sprintUnlocked)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				if (m_Move.magnitude > 0.1f)
				{
					isSprinting = true;
					sprintParticlesObject.SetActive(true);
				}
				else
				{
					sprintParticlesObject.SetActive(false);
				}
				m_Move *= 5f;
			}
			else
			{
				isSprinting = false;
				sprintParticlesObject.SetActive(false);
			}
		}

		m_Character.Move(m_Move, isSprinting, crouch, m_Jump);
		m_Jump = false;

		Vector3 viewDir = m_Cam.forward;

		viewDir.y = 0;
		viewDir.Normalize();

		Quaternion newRot = Quaternion.LookRotation(viewDir);

		transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 5f * Time.deltaTime);
	}
}
//	public float movementSpeed;

//	public float walkSpeed = 1;
//	public float runSpeed = 2;

//	bool running;
//	ThirdPersonCharacter charController;
//	private void Start()
//	{
//		charController = GetComponent<ThirdPersonCharacter>();
//	}

//	private void Update()
//	{
//		var inputX = Input.GetAxisRaw("Horizontal");
//		var inputY = Input.GetAxisRaw("Vertical");

//		Vector3 moveDir = transform.right * inputX + transform.forward * inputY;
//		running = Input.GetKey(KeyCode.LeftShift);
//		if (running)
//		{
//			movementSpeed = runSpeed;
//		}
//		else
//		{
//			movementSpeed = walkSpeed;
//		}

//		Vector3 moveAmount = moveDir * movementSpeed;

//		charController.Move(moveAmount * Time.deltaTime, false, false);

//		Vector3 viewDir = Camera.main.transform.forward;

//		viewDir.y = 0;
//		viewDir.Normalize();

//		Quaternion newRot = Quaternion.LookRotation(viewDir);

//		// player transform
//		transform.rotation = Quaternion.Slerp(transform.rotation, newRot, 5f * Time.deltaTime);

//	}
//}
//}
