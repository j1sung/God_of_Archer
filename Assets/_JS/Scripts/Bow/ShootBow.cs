using System.Collections;
using UnityEngine;

public class ShootBow : MonoBehaviour {

    [SerializeField] GameObject arrowPrefab = null;
    [SerializeField] GameObject bow = null; 
    [SerializeField] public int arrowsRemaining = 10; 
    [SerializeField] int pullSpeed = 10; 

    private GameObject arrow; 
    private TrailRenderer trail; 
    private bool stopDraw = false;
    
    private bool reset = false;

    private bool knockedArrow = false;
    private float drawDistance = 0;

    private Quaternion originalRot;
    private Vector3 originalPos;

    [SerializeField] private float changeRotPitch = 45f / 46f;
    [SerializeField] private float changeRotYaw = -90f;
    [SerializeField] private float changeRotRoll = 0f;
    private float totalHipRotChange = -85f;

    [SerializeField] private Vector3 changeBowPos = new Vector3(0, 0.005f * 2f, -0.0025f * 2f);
    [SerializeField] private Vector3 changeArrowPos = new Vector3(-0.01651724137f*2f, -0.00655172413f*2f, 0.00125862068f*2f);

    private Vector3 totalBowPosChanges = new Vector3(0, 0, 0);
    private Vector3 totalArrowPosChanges = new Vector3(0, 0, 0);

    private void Start() {
        originalRot = bow.transform.localRotation; //store the bow's original rotatation
        originalPos = bow.transform.localPosition; //store the bow's original pos
        SpawnArrow(); //spawn an arrow once the game starts
    }

    private void Update() {
        ShootArrow();

        if (reset == true) {
            if (bow.transform.localRotation.eulerAngles.x <= originalRot.eulerAngles.x) reset = false;
            else StartCoroutine(ResetRotation());
        }
    }
    private void SpawnArrow() {
        if(arrowsRemaining > 0) { 

            if (reset) { 
                reset = false;
                bow.transform.localRotation = originalRot;
                bow.transform.localPosition = originalPos;
            }

            knockedArrow = true; //set the bool
            arrow = Instantiate(arrowPrefab, transform.position, transform.rotation) as GameObject; //create new arrow
            arrow.transform.SetParent(transform, true); //set the arrow's parent
            arrow.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            trail = arrow.GetComponent<TrailRenderer>(); //get the new trail component
        }
    }
    
    private IEnumerator ResetRotation() {
        while (bow.transform.localRotation.eulerAngles.x > originalRot.eulerAngles.x) {
            bow.transform.Rotate(Time.deltaTime * -5, 0, 0, Space.Self);

            if (bow.transform.localPosition.y >= -0.4) bow.transform.localPosition = new Vector3(0, bow.transform.localPosition.y - 0.002f, bow.transform.localPosition.z);

            yield return new WaitForSeconds(0.001f);
        }
    }
    private void ResetBow() {
        totalBowPosChanges.y = 0;
        bow.transform.localPosition -= totalBowPosChanges; //reset bow's pos
        reset = true;

        transform.localPosition -= totalArrowPosChanges; //reset ArrowSpawnPoint's pos

        totalBowPosChanges = new Vector3(0, 0, 0); //clear the total changes
        totalArrowPosChanges = new Vector3(0, 0, 0); //clear the total changes
        totalHipRotChange = -85f;
    }

    private void ShootArrow() {
        
            if (arrow == null)
            {
                if (arrowsRemaining > 0)
                {
                    SpawnArrow();
                }
                return;
            }
        if (arrowsRemaining > 0) { //if they have arrows left          

            SkinnedMeshRenderer bowSkin = bow.transform.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer arrowSkin = arrow.transform.GetComponent<SkinnedMeshRenderer>();

            Rigidbody arrowRB = arrow.transform.GetComponent<Rigidbody>(); //get the rigid body
            ArrowForce af = arrow.transform.GetComponent<ArrowForce>(); //get the arrowforce object

            if (Input.GetMouseButton(0) && !stopDraw) {
                if (Input.GetKeyDown(KeyCode.R)) { stopDraw = true; drawDistance = 0; ResetBow(); }

                if (!stopDraw) { 
                    drawDistance += Time.deltaTime * pullSpeed; //set the draw distance

                    if (drawDistance < 100) {
                        bow.transform.localPosition += changeBowPos; transform.localPosition += changeArrowPos;
                        bow.transform.localRotation = Quaternion.Euler(totalHipRotChange += changeRotPitch, changeRotYaw, changeRotRoll);
                        totalBowPosChanges += changeBowPos; totalArrowPosChanges += changeArrowPos;
                    }
                    else drawDistance = 100; //Keep drawdistance at or below 100
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (stopDraw) stopDraw = false; //if they didn't want to fire the arrow, then set the bool back to false and ignore everything else
                else if (drawDistance >= 10) { //if the draw distance is enough to actually launch the arrow
                    knockedArrow = false; //set it so the player no longer has an arrow drawn
                    arrowRB.isKinematic = false; //physics can be applied

                    arrow.transform.SetParent(null); //set the parent to null
                    arrow.transform.position = transform.position; //set the position to be the current position of the transform

                    arrowsRemaining -= 1; 

                    af.shootForce = af.shootForce * ((drawDistance / 100) + 0.05f); //calculate the force of the arrow based on the draw distance

                    drawDistance = 0; 
                    af.enabled = true;
                    trail.enabled = true; 
                    ResetBow(); 
                }
                else {
                    drawDistance = 0; 
                    ResetBow();
                }
            }
            bowSkin.SetBlendShapeWeight(0, drawDistance); 
            arrowSkin.SetBlendShapeWeight(0, drawDistance);
        }

        if (Input.GetMouseButtonDown(0) && !knockedArrow) SpawnArrow();        
    }
}
