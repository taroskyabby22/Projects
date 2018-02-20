using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementPlayer : MonoBehaviour {

    float speed = 0.1f;
    public Vector2 randomsize = new Vector2(-4f, 4f);
    public bool frozen = false;

    Vector3 direction;
	// Use this for initialization
	void Start () {
        direction = new Vector3(Random.Range(randomsize.x, randomsize.y), 0f, Random.Range(randomsize.x, randomsize.y));
        direction = direction.normalized;
    }

    // Update is called once per frame
    void Update() {
        if (!frozen)
        {
            transform.Translate(direction);
            if (Mathf.Abs(transform.position.z) > 4)
            {
                direction.x = Random.Range(randomsize.x, randomsize.y) * 4f;
                direction.z = Random.Range(0, -transform.position.z) * 4f;
                direction = direction.normalized * Random.Range(0.1f, speed);
            }
            if (Mathf.Abs(transform.position.x) > 4)
            {
                direction.z = Random.Range(randomsize.x, randomsize.y) * 4f;
                direction.x = Random.Range(0, -transform.position.x) * 4f;
                direction = direction.normalized * Random.Range(0.1f, speed);
            }
        }
    }
}
