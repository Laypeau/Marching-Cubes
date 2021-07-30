using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Terrain_Master : MonoBehaviour
{
	public Transform focus;
	public GameObject chunkPrefab;
	[Range(1, 5)] public int viewRadius = 2;

	[Header("Tile Settings")]
	public float chunkSize = 16;
	public int blockResolution = 16;

	private Vector3Int focusCellpos;
	private Vector3Int[] activeTiles;
	private List<Vector3Int> existentTiles = new List<Vector3Int>();
	
	void Start()
	{
		if (focus == null || chunkPrefab == null) throw new UnityException("Focus/tile object not set");
		if (focus == gameObject) throw new UnityException("Don't make the terrain master the focus object >:(");

		Terrain_Chunk.chunkSize = chunkSize;
		Terrain_Chunk.blockResolution = blockResolution;

		focusCellpos = WorldToTileSpace(focus.position);
		Init();
	}

	void Update()
	{
		//if cell pos changes, redo chunks
		Vector3Int currentFocusCellpos = WorldToTileSpace(focus.position);
		if (currentFocusCellpos != focusCellpos)
		{
			focusCellpos = WorldToTileSpace(focus.position);
			Recalculate();
		}
	}

	void Init()
	{
		activeTiles = new Vector3Int[(int)Mathf.Pow(viewRadius * 2, 3)];

		int i = 0;
		for (int x = -viewRadius; x < viewRadius; x++)
		{
			for (int y = -viewRadius; y < viewRadius; y++)
			{
				for (int z = -viewRadius; z < viewRadius; z++)
				{
					Vector3Int currentTile = WorldToTileSpace(focus.position) + new Vector3Int(x, y, z);

					CreateTile(currentTile.ToString(), focus.position + ((int)chunkSize * currentTile));
					activeTiles[i] = currentTile;
					existentTiles.Add(currentTile);
					
					i++;
				}
			}
		}

	}

	void Recalculate()
	{
		activeTiles = new Vector3Int[(int)Mathf.Pow(viewRadius * 2, 3)];

		//Find what tiles should be active
		int i = 0;
		for (int x = -viewRadius; x < viewRadius; x++)
		{
			for (int y = -viewRadius; y < viewRadius; y++)
			{
				for (int z = -viewRadius; z < viewRadius; z++)
				{
					activeTiles[i] = WorldToTileSpace(focus.position) + new Vector3Int(x, y, z);
					i++;
				}
			}
		}

		//Go through every existent tile, activate/deactivate them if they're in activeTiles[]
			//will get slower as more tiles are created. is there a better way to do this?	
		foreach (Vector3Int tile in existentTiles)
		{
			if (activeTiles.Contains(tile))
				transform.Find(tile.ToString()).gameObject.SetActive(true);
			else
				transform.Find(tile.ToString()).gameObject.SetActive(false);
		}

		//Create a new tile if it doesn't already exist
		foreach (Vector3Int tile in activeTiles)
		{
			if (!existentTiles.Contains(tile))
			{
				CreateTile(tile.ToString(), Vector3.Scale(Vector3.one * chunkSize, tile));
				existentTiles.Add(tile);
			}
		}
	}

	private void CreateTile(string name, Vector3 wherest)
	{
		GameObject tile = Instantiate(chunkPrefab);
		tile.name = name;
		tile.transform.position = wherest;
		tile.transform.parent = transform; //for some reason, making the parent object the focus object breaks everything.
	}

	public Vector3Int WorldToTileSpace(Vector3 worldPos)
	{
		return new Vector3Int((int)Mathf.Floor((worldPos.x / chunkSize) + 0.5f), (int)Mathf.Floor((worldPos.y / chunkSize) + 0.5f), (int)Mathf.Floor((worldPos.z / chunkSize) + 0.5f)); //Nothing could go wrong with casting to int...
	}

	public Vector3 TileToWorldSpace(Vector2Int cellPos)
	{
		return new Vector3(cellPos.x * chunkSize, 0f, cellPos.y * chunkSize);
	}
}

