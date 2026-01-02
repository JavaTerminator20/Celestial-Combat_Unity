using UnityEngine;

public class moonExplode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wait = false;
    }

    void Awake()
    {
        Debug.Log("moon is awake again");
    }
    ParticleSystem[] explotionParticles;
    bool wait;
    void OnCollisionEnter(Collision collision){
        if (!wait){
            wait = true;
            Debug.Log("moon hit target");
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero; //to je duplicirano zato ker v nasprotnem primeru se particle origin premakne med ...
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;   //... animiranjem flash-a - izgleda kot da na vec mestih explodira
            explotionParticles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem sys in explotionParticles){
                sys.Play();
                FindFirstObjectByType<PlayerController>().GetComponent<Animator>().SetBool("knockdown", true);
                Invoke("clearKnockdown", 1.0f);
                Invoke("ResetPosition", 1.5f);
            }
        }
    }

    void ResetPosition(){
        gameObject.SetActive(false);
        transform.position = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<MeshRenderer>().enabled = true;
        FindFirstObjectByType<OponentController>().BringBackMoon();

    }

    void clearKnockdown(){
        wait = false;
        FindFirstObjectByType<PlayerController>().GetComponent<Animator>().SetBool("knockdown", false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
