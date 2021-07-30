using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Terrain_Chunk_GPU : Terrain_Chunk
{
	[SerializeField] private ComputeShader computeShader = default;
	private ComputeBuffer verticesOut;
	private const int verticesOutStride = sizeof(float) * 3;

	private ComputeBuffer trianglesOut;
	private const int trianglesOutStride = sizeof(float) * 1;

	public override void GenerateMesh()
	{
		verticesOut = new ComputeBuffer(12 * blockResolution * blockResolution * blockResolution, verticesOutStride, ComputeBufferType.Structured);
		trianglesOut = new ComputeBuffer(5 * 3 * blockResolution * blockResolution * blockResolution, trianglesOutStride, ComputeBufferType.Structured);

		int kernelID = computeShader.FindKernel("Chunk");

		computeShader.SetFloats("origin", new float[3]{transform.position.x, transform.position.y, transform.position.z});
		computeShader.SetInt("resolution", blockResolution);
		computeShader.SetFloat("size", chunkSize);
		computeShader.SetFloat("threshold", threshold);
		computeShader.SetBuffer(kernelID, "verticesOut", verticesOut);
		computeShader.SetBuffer(kernelID, "trianglesOut", trianglesOut);

		int asdf = Mathf.Max(1, Mathf.CeilToInt(blockResolution/8f));
		computeShader.Dispatch(kernelID, asdf, asdf, asdf);

		Vector3[] vertices = new Vector3[12 * blockResolution * blockResolution * blockResolution];
		int[] triangles = new int[5 * 3 * blockResolution * blockResolution * blockResolution];
		verticesOut.GetData(vertices);
		trianglesOut.GetData(triangles);

		verticesOut.Release();
		trianglesOut.Release();	

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}
}
