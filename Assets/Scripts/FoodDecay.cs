using UnityEngine;

/// <summary>
/// class taking care of the disparation of food when eaten
/// </summary>
public class FoodDecay : MonoBehaviour
{
	// amount given by the food node
	private float foodAmount = 2;
	int decayRate = 0;

    void Update()
    {
		//decay the food by the amout of individual eating it
		foodAmount -= Time.deltaTime * decayRate;
		if(foodAmount <= 0)
			Destroy(gameObject);
    }

	//when an individual begin to eat the food
	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Individual")
			decayRate++;
	}
}
