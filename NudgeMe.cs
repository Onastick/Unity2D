using UnityEngine;

public class NudgeMe : MonoBehaviour {

	public float xDir = 1000;
	public float yDir = 1000;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey (KeyCode.Space)){
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDir,yDir));
		}
	}
}
