using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner : MonoBehaviour
{
	[Range(0,7)]
	public int id;
	public bool active = false;
	
	private Material mat;
	private Master master;

	void Awake()
	{
		mat = gameObject.GetComponent<MeshRenderer>().material;
	}

	public void Toggle()
	{
		active = !active;
		mat.SetInt("Active", active ? 1 : 0);
	}
}
