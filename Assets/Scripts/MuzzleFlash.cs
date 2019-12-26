using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public float flashTime;

    private void Start()
    {
        Deactive();
    }

    public void Activate()
    {
        flashHolder.SetActive(true);
        int flashSprintIndex = Random.Range(0, flashSprites.Length);
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSprintIndex];
        }
        Invoke("Deactive", flashTime);
    }

    void Deactive()
    {
        flashHolder.SetActive(false);
    }
}
