/*

Courtesy of Sebastian Lague:
    - Youtube       :   https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ
    - Github Repo   :   https://github.com/SebLague/Cloth-and-IK-Test/tree/main/Assets/Visualize

*/

using System.Collections.Generic;
using UnityEngine;

namespace Visualization.MeshGenerators
{
	public static class MeshUtility
	{
		public static void MeshFromMultipleSources(Mesh mesh, List<Vector3>[] vertexLists, List<int>[] triangleLists)
		{
			var vertices = new List<Vector3>();
			var tris = new List<int>();

			for (int i = 0; i < vertexLists.Length; i++)
			{
				int vertSkipCount = vertices.Count;
				vertices.AddRange(vertexLists[i]);

				for (int triIndex = 0; triIndex < triangleLists[i].Count; triIndex++)
				{
					tris.Add(triangleLists[i][triIndex] + vertSkipCount);
				}
			}

			mesh.SetVertices(vertices);
			mesh.SetTriangles(tris, 0);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}

	}
}