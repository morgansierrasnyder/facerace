using System.Collections;
using UnityEngine;

public class ExpandVertices : MonoBehaviour
{
	public float radius = 0.5f;
	public float pull = 0.02f;
	private MeshFilter unappliedMesh;

	void Update ()
	{
		// When no button is pressed we update the mesh collider
		if (!Input.GetMouseButton (0))
		{
			// Apply collision mesh when we let go of button
			ApplyMeshCollider();
			return;
		}

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast (ray, out hit)) {
			MeshFilter filter = hit.collider.GetComponent<MeshFilter> ();

			if (filter) {
				if (filter != unappliedMesh) {
					ApplyMeshCollider();
					unappliedMesh = filter;
				}

				Vector3 localPoint = filter.transform.InverseTransformPoint(hit.point);
				ExpandMesh(filter.mesh, localPoint, pull, radius);
			}
		}	
	}

	void ExpandMesh(Mesh mesh, Vector3 intercept, float pull, float radius)
	{
		Vector3[] vertices = mesh.vertices;
		Bounds bounds = mesh.bounds;

		for (int i = 0; i < vertices.Length; i++) {
			float distance = (vertices[i] - intercept).magnitude;
			if (distance > radius) {
				continue;
			}
			vertices [i] += pull * (vertices [i] - intercept);
			if (!bounds.Contains (vertices [i])) {
				vertices[i].x = Mathf.Clamp (vertices [i].x, bounds.min.x, bounds.max.x);
				vertices[i].z = Mathf.Clamp (vertices [i].z, bounds.min.z, bounds.max.z);
			}
		}
		mesh.vertices = vertices;
	}

	void ApplyMeshCollider() {
		if (unappliedMesh && unappliedMesh.GetComponent<MeshCollider>()) {
			unappliedMesh.GetComponent<MeshCollider>().GetComponent<MeshFilter>().mesh = unappliedMesh.mesh;
		}
		unappliedMesh = null;
	}

}

