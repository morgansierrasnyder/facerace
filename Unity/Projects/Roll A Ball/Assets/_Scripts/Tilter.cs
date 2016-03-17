using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

public class Tilter : MonoBehaviour {

	private int flipbit;
	private int count;

	void Start() {
		count = 0;
		flipbit = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (count == 200) {
			flipbit = -flipbit;
			count = 0;
		}

		transform.Rotate (new Vector3 (0, 0, flipbit * 8.0f) * Time.deltaTime);
		transform.RotateAround( new Vector3(0, 0, 0), new Vector3(0, 1, 0), 1.0f);

		count++;
	}
}
