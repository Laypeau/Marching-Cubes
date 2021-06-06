using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform followingTransform;
	public Transform cameraFocusTransform;
	public Transform cameraTransform;

	public Vector3 cameraFocusOffset; //This is not for the camera itself. Set that in the inspector
	[Range(0f, 1f)] public float cameraLerp = 1f;

	[Range(-10f, 10f)] public float sensitivityX = 5f;
	[Range(-10f, 10f)] public float sensitivityY = -5f;
	[Range(-10f, 10f)] public float sensitivityScroll = 1f;

	[Range(0f, 100f)] public float minCamDistance;
	[Range(0f, 100f)] public float maxCamDistance;

	private float rotX = 0f;
	private float rotY = 0f;

	void Start()
	{
		if (followingTransform == null || cameraFocusTransform == null || cameraTransform == null) throw new UnityException("Camera references not set");
		if (minCamDistance > maxCamDistance) Debug.LogWarning("Camera min/max distance error");
	}

	void Update()
	{
		rotX += Input.GetAxis("Mouse Y") * sensitivityY;  //Up is negative, Down is positive
		rotX = Mathf.Clamp(rotX, -90f, 90f);
		rotY = rotY % 360f + Input.GetAxis("Mouse X") * sensitivityX;
		cameraFocusTransform.rotation = Quaternion.Euler(rotX, rotY, 0f);

		cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraTransform.localPosition.y, Mathf.Clamp(cameraTransform.localPosition.z - (Input.mouseScrollDelta.y * sensitivityScroll), -maxCamDistance, -minCamDistance));

		cameraFocusTransform.position = Vector3.Lerp(cameraFocusTransform.position, followingTransform.position, cameraLerp);
	}
}
