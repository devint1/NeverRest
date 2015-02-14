using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Credit for this algorithm goes to runevision, on the Unity wiki.
 * URL: http://wiki.unity3d.com/index.php?title=Triangulator
 */
public class Tesselator {
	private List<Vector2> points = new List<Vector2>();
	private int[] indicesArray;
	
	public Tesselator(Vector2[] pointsArray) {
		points = new List<Vector2>(pointsArray);
		List<int> indices = new List<int>();
		
		int n = points.Count;
		if (n < 3) {
			indicesArray = indices.ToArray();
			return;
		}
		
		int[] V = new int[n];
		if (Area() > 0) {
			for (int v = 0; v < n; v++)
				V[v] = v;
		}
		else {
			for (int v = 0; v < n; v++)
				V[v] = (n - 1) - v;
		}
		
		int nv = n;
		int count = 2 * nv;
		for (int m = 0, v = nv - 1; nv > 2; ) {
			if ((count--) <= 0) {
				indicesArray = indices.ToArray();
				return;
			}
			
			int u = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			int w = v + 1;
			if (nv <= w)
				w = 0;
			
			if (Snip(u, v, w, nv, V)) {
				int a, b, c, s, t;
				a = V[u];
				b = V[v];
				c = V[w];
				indices.Add(a);
				indices.Add(b);
				indices.Add(c);
				m++;
				for (s = v, t = v + 1; t < nv; s++, t++)
					V[s] = V[t];
				nv--;
				count = 2 * nv;
			}
		}
		
		indices.Reverse();
		indicesArray = indices.ToArray();
	}

	private float Area () {
		int n = points.Count;
		float A = 0.0f;
		for (int p = n - 1, q = 0; q < n; p = q++) {
			Vector2 pval = points[p];
			Vector2 qval = points[q];
			A += pval.x * qval.y - qval.x * pval.y;
		}
		return (A * 0.5f);
	}
	
	private bool Snip (int u, int v, int w, int n, int[] V) {
		int p;
		Vector2 A = points[V[u]];
		Vector2 B = points[V[v]];
		Vector2 C = points[V[w]];
		if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
			return false;
		for (p = 0; p < n; p++) {
			if ((p == u) || (p == v) || (p == w))
				continue;
			Vector2 P = points[V[p]];
			if (InsideTriangle(A, B, C, P))
				return false;
		}
		return true;
	}

	private bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
		float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
		float cCROSSap, bCROSScp, aCROSSbp;
		
		ax = C.x - B.x; ay = C.y - B.y;
		bx = A.x - C.x; by = A.y - C.y;
		cx = B.x - A.x; cy = B.y - A.y;
		apx = P.x - A.x; apy = P.y - A.y;
		bpx = P.x - B.x; bpy = P.y - B.y;
		cpx = P.x - C.x; cpy = P.y - C.y;
		
		aCROSSbp = ax * bpy - ay * bpx;
		cCROSSap = cx * apy - cy * apx;
		bCROSScp = bx * cpy - by * cpx;
		
		return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
	}

	public void DrawTriangles(GameObject obj) {
		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[points.Count];
		for (int i = 0; i < points.Count; ++i) {
			vertices[i] = new Vector3(points[i].x, points[i].y, 0);
		}
		mesh.vertices = vertices;
		mesh.triangles = indicesArray;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		Material material = new Material(Shader.Find("Diffuse"));
		material.SetColor("_Color", new Color(Random.value, Random.value, Random.value));
		MeshFilter filter = obj.GetComponent<MeshFilter> ();
		MeshRenderer renderer = obj.GetComponent<MeshRenderer> ();
		if (!filter) {
			filter = obj.AddComponent<MeshFilter> ();
		}
		if (!renderer) {
			renderer = obj.AddComponent<MeshRenderer>();
		}
		filter.renderer.material = material;
		filter.mesh = mesh;
	}

	public Vector2[] GetRandomTriangle() {
		int numTriangles = indicesArray.Length / 3;
		int triangleNum = Random.Range(0, numTriangles);
		int startIndex = triangleNum * 3;
		Vector2[] triangle = new Vector2[3];
		triangle [0] = points [indicesArray[startIndex]];
		triangle [1] = points [indicesArray[startIndex + 1]];
		triangle [2] = points [indicesArray[startIndex + 2]];
		return triangle;
	}
}
