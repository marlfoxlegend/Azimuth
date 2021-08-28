using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azimuth
{
	[CreateAssetMenu(fileName = "New Path", menuName = "Path", order = 11)]
	public class Path : ScriptableObject
	{
		[SerializeField] List<GameObject> paths;
		[SerializeField] PathCreator pathCreator;

		public VertexPath CreatedPath => pathCreator.path;

		//public List<Transform> GetPathTransforms(int pathIndex = -1)
		//{
		//	if (pathIndex >= paths.Count)
		//	{
		//		Debug.LogWarning("Trying to access a path that does not exist.");
		//		pathIndex = -1;
		//	}
		//	if (pathIndex < 0)
		//	{
		//		pathIndex = Random.Range(0, paths.Count);
		//	}
		//	List<Transform> transforms = new List<Transform>();
		//	foreach (Transform t in paths[pathIndex].transform)
		//	{
		//		transforms.Add(t);
		//	}
		//	return transforms;
		//}
	}
}