using UnityEngine;

public class FollowTool : MonoBehaviour
{
	public Transform tool;
	
	public float
		mouseSensitivity = 1f,
		positionSmoothTime = 0.12f,
		rotationSpeed = 2f;
	
	public LayerMask layermask;
	public Bounds bounds;
	
	Vector3
		position,
		positionSmoothVelocity;
	
	static readonly string
		mX = "Mouse X",
		mY = "Mouse Y";
	
	void Start(){
		position = tool.position;
	}
	
	void Update(){
		UpdatePosition();
		UpdateRotation();
	}
	
	void UpdatePosition(){
		if(Input.GetMouseButton(0)){
			var inputDirection = new Vector3(Input.GetAxis(mX), Input.GetAxis(mY));
			float sensitivity = mouseSensitivity * Time.deltaTime;
			
			position += inputDirection * sensitivity;
			
			position = new Vector3(
				Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
				Mathf.Clamp(position.y, bounds.min.y, bounds.max.y)
			);
		}
		
		tool.position = Vector3.SmoothDamp(
			tool.position,
			position,
			ref positionSmoothVelocity,
			positionSmoothTime
		);
	}
	
	void UpdateRotation(){
		Ray ray = new Ray(tool.position, tool.forward);
		bool raycast = Physics.Raycast(ray, out var hit, Mathf.Infinity, layermask);
		var direction = Vector3.forward;
		
		if(raycast)
			direction = -hit.normal;
		
		var rotation = Quaternion.LookRotation(direction);
		float rotationSpeedDelta = rotationSpeed * Time.deltaTime;
		
		tool.rotation = Quaternion.Slerp(
			tool.rotation,
			rotation,
			rotationSpeedDelta
		);
	}
	
	void OnDrawGizmosSelected(){
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}
}