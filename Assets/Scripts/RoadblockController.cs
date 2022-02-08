using UnityEngine;
using System.Collections;

public class RoadblockController : MonoBehaviour {

    public AudioSource bumperSound;
    public Material bumperOff;
    public Material bumperOn;

    MeshRenderer renderer;

    public int hitCount = 0;
    bool bHitLight = false;
    float hitLightTimer = 0;
    

    private void Start()
    {
        renderer = gameObject.GetComponent<MeshRenderer>();
        bumperSound = GetComponent<AudioSource>();
    }

    // Before rendering each frame..
    void Update () 
	{
         //continuously rotates cuvbe
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);

        // assign material depending on whether bumper hit or not
        Material[] materials = renderer.materials;
        if ((bHitLight)&&(hitLightTimer<5))
        {
            materials[0] = bumperOn;
            hitLightTimer = hitLightTimer + 1;
        }
        else {
            materials[0] = bumperOff;
            bHitLight = false;
        };
        renderer.materials = materials;
    }

    System.Collections.IEnumerator resetWinText() {
        yield return new WaitForSeconds(0.3F);
        GameObject.Find("Pinball Table").GetComponent<PinballGame>().winText.text = "";
    }

    void OnCollisionEnter(Collision myCollision)
    {
        if (myCollision.gameObject.tag == "Ball")
        {
            // each time bumper is hit, hitCount increases by one
            hitCount = hitCount + 1;
            //if bumper gets hit 3 times, it disappears (gets set inactive and isn't displayed in scene anymore)

            // myCollision.gameObject.GetComponent<MeshRenderer>().enabled = false;
            if (hitCount == 1)
            {
                int option = UnityEngine.Random.Range(1,2);
                switch(option) {
                    case 1:
                        GameObject.Find("Pinball Table").GetComponent<PinballGame>().score -= 100;
                        GameObject.Find("Pinball Table").GetComponent<PinballGame>().winText.text = "You lost 100 score";
                        StartCoroutine(resetWinText());
                        break;
                    case 2: 
                        GameObject.Find("Pinball Table").GetComponent<PinballGame>().remaining -= 5;
                        if (GameObject.Find("Pinball Table").GetComponent<PinballGame>().remaining < 0)
                            GameObject.Find("Pinball Table").GetComponent<PinballGame>().remaining = 0;
                        GameObject.Find("Pinball Table").GetComponent<PinballGame>().winText.text = "You lost 5 seconds";
                        break;
                    default:
                        break;
                }
                // this.gameObject.SetActive(false);
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                this.gameObject.GetComponent<BoxCollider>().enabled = false;

                // GameObject.Find("Cylinder").GetComponent<MeshRenderer>().enabled = true;
                // GameObject.Find("Cylinder").transform.position = this.gameObject.transform.position;

            }
            //trigger hit light (change material assigned to bumper object, so bumper "lights up"), reset hitlight timer 
            bHitLight = true;
            hitLightTimer = 0;

            bumperSound.Play();
        }
    }
}	