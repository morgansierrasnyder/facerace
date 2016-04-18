using System.Collections;
using UnityEngine;

public class MorphVertices : MonoBehaviour
{
	public enum Control {Expand, Contract, Move};
	public Control control = Control.Move;

	public float radius = 0.5f;
	public float pull = 0.5f;

	private MeshFilter unappliedMesh;

	// variables for dragging
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
			
		RaycastHit hit, hit0;
		Ray prevRay = Camera.main.ScreenPointToRay (prevMousePos);
		Ray currRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast (currRay, out hit)) {
			Physics.Raycast (prevRay, out hit0);

			MeshFilter filter = hit.collider.GetComponent<MeshFilter> ();

			if (filter) {
				if (filter != unappliedMesh) {
					ApplyMeshCollider ();
					unappliedMesh = filter;
				}

				Vector3 currPt = filter.transform.InverseTransformPoint (hit.point);
				Vector3 prevPt = filter.transform.InverseTransformPoint (hit0.point);

				switch (control) {
					case Control.Contract:
						ContractMesh (filter.mesh, currPt, pull, radius);
						break;
					case Control.Expand:
						ExpandMesh (filter.mesh, currPt, pull, radius);
						break;
					case Control.Move:
						DragMesh (filter.mesh, prevPt, currPt, pull, radius);
						prevMousePos = Input.mousePosition;
						break;
				}
			}
		}	
	}

	void ContractMesh(Mesh mesh, Vector3 intercept, float pull, float radius)
	{
		Vector3[] vertices = mesh.vertices;
		Bounds bounds = mesh.bounds;

		for (int i = 0; i < vertices.Length; i++) {
			// Ignore vertices at the edge of the frame
			if ((bounds.max.x - vertices[i].x) < 0.05 || (vertices[i].x - bounds.min.x) < 0.05 ||
				(bounds.max.z - vertices[i].z) < 0.05 || (vertices[i].z - bounds.min.z) < 0.05) {
				continue;
			}
			float distance = (vertices[i] - intercept).magnitude;
			if (distance > radius) {
				continue;
			}
			vertices [i] -= (pull / 100f) * (vertices [i] - intercept);
			if (!bounds.Contains (vertices [i])) {
				vertices[i].x = Mathf.Clamp (vertices [i].x, bounds.min.x, bounds.max.x);
				vertices[i].z = Mathf.Clamp (vertices [i].z, bounds.min.z, bounds.max.z);
			}
		}
		mesh.vertices = vertices;
	}

	void ExpandMesh(Mesh mesh, Vector3 intercept, float pull, float radius)
	{
		Vector3[] vertices = mesh.vertices;
		Bounds bounds = mesh.bounds;

		for (int i = 0; i < vertices.Length; i++) {
			// Ignore vertices at the edge of the frame
			if ((bounds.max.x - vertices[i].x) < 0.05 || (vertices[i].x - bounds.min.x) < 0.05 ||
				(bounds.max.z - vertices[i].z) < 0.05 || (vertices[i].z - bounds.min.z) < 0.05) {
				continue;
			}
			float distance = (vertices[i] - intercept).magnitude;
			if (distance > radius) {
				continue;
			}
			vertices [i] += (pull / 100f) * (vertices [i] - intercept);
			if (!bounds.Contains (vertices [i])) {
				vertices[i].x = Mathf.Clamp (vertices [i].x, bounds.min.x, bounds.max.x);
				vertices[i].z = Mathf.Clamp (vertices [i].z, bounds.min.z, bounds.max.z);
			}
		}
		mesh.vertices = vertices;
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

