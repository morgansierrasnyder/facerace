using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Text countText;
	public Text winText;

	private Rigidbody rb;
	private int count;

	void Start() {
		
		rb = GetComponent<Rigidbody> ();
		count = 0;
		countText.text = "Count = " + count.ToString ();
		winText.text = "";
	}

	void FixedUpdate () {

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rb.AddForce (speed * movement);
	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.CompareTag ("Pick Up")) {
			other.gameObject.SetActive (false);
			count++;
			countText.text = "Count = " + count.ToString ();
			if (count == 13) {
				winText.text = "YOU WIN!";
			}
		}
	}

	void SetCountText() {
	
		countText.text = "Count = " + count.ToString ();
	}
}
