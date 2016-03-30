using System.Collections;
using UnityEngine;

public class DragVertices : MonoBehaviour
{
	public float radius = 0.1f;
	public float pull = 0.25f;
	private MeshFilter unappliedMesh;
	private bool dragging = false;


	void Start()
	{
		//mesh = GetComponent<MeshFilter>().mesh;
		//cam = GetComponent<Camera> ();
	}

	void Update ()
	{
		// When no button is pressed we update the mesh collider
		if (!Input.GetMouseButton (0)) {
			// Apply collision mesh when we let go of button
			ApplyMeshCollider ();
			return;
		} else {
			dragging = true;
		}

		if (dragging) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				Debug.Log ("mouse pos = " + Input.mousePosition);
				Debug.Log ("hit = " + hit.point);

				MeshFilter filter = hit.collider.GetComponent<MeshFilter> ();

				if (filter) {
					if (filter != unappliedMesh) {
						ApplyMeshCollider ();
						unappliedMesh = filter;
					}

					var localPoint = filter.transform.InverseTransformPoint (hit.point);
					DeformMesh (filter.mesh, localPoint, pull, radius);
				}

				// Find the vertices of the triangle that was hit by the raycast
				/*Vector3[] vertices = mesh.vertices;
				int[] triangles = mesh.triangles;

				Vector3 p0 = vertices [triangles [hit.triangleIndex * 3 + 0]];
				Vector3 p1 = vertices [triangles [hit.triangleIndex * 3 + 1]];
				Vector3 p2 = vertices [triangles [hit.triangleIndex * 3 + 2]];

				Transform hitTransform = hit.collider.transform;
				p0 = hitTransform.TransformPoint (p0);
				p1 = hitTransform.TransformPoint (p1);
				p2 = hitTransform.TransformPoint (p2);*/

			}	
		}
	}

	void DeformMesh(Mesh mesh, Vector3 intercept, float pull, float radius)
	{
		Vector3[] vertices = mesh.vertices;
		Bounds bounds = mesh.bounds;
		Debug.Log ("intercept = " + intercept);

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

