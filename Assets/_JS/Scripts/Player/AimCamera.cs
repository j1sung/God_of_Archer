using UnityEngine;

public class AimCamera : MonoBehaviour {
    private Rigidbody rb;
    private bool inAir = false;
    private bool crouch = false;
    private Vector3 ogScale;    
    [SerializeField] float verSensitivity = 2;
    [SerializeField] float horSensitivity = 2;
    [SerializeField] float speed = 0.5f;
    [SerializeField] float jumpForce = 1f;
    [SerializeField] float crouchScale = 1.5f;
    [SerializeField] Camera cam = null;

    private void Start() {
        rb = GetComponent<Rigidbody>(); 
        Cursor.visible = false; 
        ogScale = transform.localScale;
    }

    private void Update() {
        CheckForCameraMovement();
        if (Input.GetKeyDown(KeyCode.Space) && !inAir) {
            rb.AddForce(new Vector3(0, jumpForce, 0)); 
            inAir = true;
        }
       
        if (Input.GetKeyDown(KeyCode.LeftControl) && !inAir && !crouch){
            transform.localScale -= new Vector3(0, transform.localScale.y - crouchScale, 0);
            crouch = true;
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl) && !inAir && crouch){
            transform.localScale = ogScale;
            crouch = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Cursor.visible) Cursor.visible = false; 
            else Cursor.visible = true; 
        }

        if(!Input.GetMouseButton(0) && !Input.GetKey(KeyCode.Space)) CheckForMovement(speed);
        else CheckForMovement(speed / 1.5f);
    }

    private void OnCollisionEnter(Collision c) { if (c.gameObject.tag == "Terrain") inAir = false; }

    private void CheckForMovement(float moveSpeed) {        
        rb.MovePosition(transform.position + (transform.right * Input.GetAxis("Vertical") * moveSpeed) 
            + (transform.forward * -Input.GetAxis("Horizontal") * moveSpeed));        
    }

    private void CheckForCameraMovement() {
        float mouseX = Input.GetAxisRaw("Mouse X"); //get x input
        float mouseY = Input.GetAxisRaw("Mouse Y"); //get y input

        Vector3 rotateX = new Vector3(mouseY * verSensitivity, 0, 0); //calculate the x rotation based on the y input
        Vector3 rotateY = new Vector3(0, mouseX * horSensitivity, 0); //calculate the y rotation based on the x input

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotateY)); //rotate rigid body
        cam.transform.Rotate(-rotateX);
    }
}
