using UnityEngine;

public class ArrowForce : MonoBehaviour {
    
    private Rigidbody rb;
    public float shootForce = 2000;

    private void OnEnable() {
        rb = GetComponent<Rigidbody>(); //we'll get the rigidbody of the arrow
        rb.velocity = Vector3.zero; //zero-out the velocity
        ApplyForce(); //Apply force so the arrow flies
    }

    private void Update() { transform.right = Vector3.Slerp(transform.right, transform.GetComponent<Rigidbody>().velocity.normalized, Time.deltaTime); }

    private void ApplyForce() { rb.AddRelativeForce(Vector3.right * shootForce); }
}
