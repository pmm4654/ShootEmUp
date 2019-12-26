using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRigidBody;
    public float forceMin;
    public float forceMax;

    public float lifetime = 4;
    public float fadetime = 2;

    // Start is called before the first frame update
    void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        myRigidBody.AddForce(transform.right * force);
        myRigidBody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float fadePercent = 0;
        float fadeSpeed = 1 / fadetime;
        Material material = GetComponent<Renderer>().material;
        Color initialColor = material.color;
        while(fadePercent < 1)
        {
            fadePercent += fadeSpeed * Time.deltaTime;
            material.color = Color.Lerp(initialColor, Color.clear, fadePercent);
            yield return null;
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
