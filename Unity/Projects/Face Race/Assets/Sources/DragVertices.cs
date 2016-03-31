using System.Collections;
using UnityEngine;

public class DragVertices : MonoBehaviour
{
	public float radius = 0.5f;
	public float pull = 0.02f;
	private MeshFilter unappliedMesh;
	private bool dragging = false;
	private Vector3 prevMousePos;

	void Update ()
	{ 
		if (Input.GetMouseButtonDown (0)) {
			dragging = true;
			prevMousePos = Input.mousePosition;
			return;
		}
		if (Input.GetMouseButtonUp (0)) {
			dragging = false;
			return;
		}
		// When no button is pressed we update the mesh collider
		if (!Input.GetMouseButton (0)) {
			// Apply collision mesh when we let go of button
			ApplyMeshCollider ();
			return;
		}

		if (dragging) {
			RaycastHit hit0, hit1;
			Ray ray0 = Camera.main.ScreenPointToRay (prevMousePos);
			Ray ray1 = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray1, out hit1)) {
				Physics.Raycast (ray0, out hit0);

				MeshFilter filter = hit0.collider.GetComponent<MeshFilter> ();

				if (filter) {
					if (filter != unappliedMesh) {
						ApplyMeshCollider ();
						unappliedMesh = filter;
					}

					Vector3 localPoint0 = filter.transform.InverseTransformPoint (hit0.point);
					Vector3 localPoint1 = filter.transform.InverseTransformPoint (hit1.point);
					DragMesh (filter.mesh, localPoint0, localPoint1, pull, radius);
				}
			}	
		}
	}

	void DragMesh(Mesh mesh, Vector3 intercept0, Vector3 intercept1, float pull, float radius)
	{
		Vector3[] vertices = mesh.vertices;
		Bounds bounds = mesh.bounds;

		for (int i = 0; i < vertices.Length; i++) {
			// Ignore vertices at the edge of the frame
			if ((bounds.max.x - vertices[i].x) < 0.05 || (vertices[i].x - bounds.min.x) < 0.05 ||
				(bounds.max.z - vertices[i].z) < 0.05 || (vertices[i].z - bounds.min.z) < 0.05) {
				continue;
			}

			float distance = (vertices[i] - intercept0).magnitude;
			if (distance > radius) {
				continue;
			}

			float falloff = 1.0f - (distance / radius); // Linear for now!
			vertices [i] += falloff * pull * (intercept1 - intercept0);
			// Clamp vertices to mesh's bounding box
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

