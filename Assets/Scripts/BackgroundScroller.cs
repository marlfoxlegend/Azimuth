using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
	[Range(0.01f, 0.1f)] public float scrollSpeed;

	Material spaceMaterial;

	private void Awake()
	{
		spaceMaterial = GetComponent<MeshRenderer>().material;
	}

	private void Update()
	{
		var offset = spaceMaterial.mainTextureOffset;
		offset.x += scrollSpeed * Time.deltaTime;
		spaceMaterial.mainTextureOffset = offset;
	}
}
