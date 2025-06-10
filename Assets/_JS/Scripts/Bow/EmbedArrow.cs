using UnityEngine;

public class EmbedArrow : MonoBehaviour {
    [SerializeField] GameObject sparksPrefab = null;
    private GameObject sparks;
    private Rigidbody rb;
    private Collider Collder;
    private bool sparkExists = false;
    private void Start() { 
        rb = GetComponent<Rigidbody>();
        Collder = GetComponent<Collider>();
    }

    private void Update() {
        if (sparkExists && !sparks.GetComponent<ParticleSystem>().isEmitting) {
            Destroy(sparks, 1f);
            sparkExists = false; //we do this so we don't attempt to destroy a non-existing object            
        }
    }

    private void OnCollisionEnter(Collision col) {
        //ignore the player object as well as other arrow objects
        if (col.gameObject.tag == "Arrow" || col.gameObject.tag == "Player") return; 

        transform.GetComponent<ArrowForce>().enabled = false; 
        rb.isKinematic = true;

        sparks = Instantiate(sparksPrefab, transform) as GameObject; 
        sparksPrefab.transform.rotation = transform.rotation; 
        sparkExists = true; 

        transform.localScale += new Vector3(3, 3, 3); 

        transform.SetParent(GameObject.FindGameObjectWithTag("ArrowContainer").transform, true);
        Collder.isTrigger = true;
        
    }
}