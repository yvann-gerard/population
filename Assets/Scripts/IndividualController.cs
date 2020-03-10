using Pathfinding;
using UnityEngine;

public class IndividualController : MonoBehaviour
{
	public enum Status
	{
		searching,
		seeking,
		eating
	}

	public Status status;
	private Transform target;
	public PopulationController parentPopulation;
	AIDestinationSetter aiDestinationSetter;
	AIPath aIPath;
	private int[] stats;
	private float timeLeft = 10;
	private Transform newTarget;
	//Calculate the center of the arena
	Vector3 center = new Vector3(GameInfo.arenaBounds.center.x, 0, 112 / 2 * Mathf.Tan(Mathf.PI*(GameInfo.populationCount - 2)/(2 * GameInfo.populationCount)));

	public void Init(PopulationController parentPopulation, GameInfo.PopulationStat populationStat)
	{
		this.parentPopulation = parentPopulation;
		this.stats = populationStat.stats;
		SphereCollider agroCollider = GetComponentInChildren<SphereCollider>();
		aiDestinationSetter = GetComponent<AIDestinationSetter>();
		aIPath = GetComponent<AIPath>();
		
		//set the base stats of the individual from its population stats
		aIPath.maxSpeed = stats[(int)GameInfo.PopulationStat.stat.speed] * 2f + 15f;
		agroCollider.radius = stats[(int)GameInfo.PopulationStat.stat.vision] * 2 + 10;
		timeLeft += stats[(int)GameInfo.PopulationStat.stat.endurance] * 1.5f;

		newTarget = new GameObject().transform;
		target = generatePosition();
		aiDestinationSetter.target = target;
		aIPath.SearchPath();
	}

	private void Update() 
	{
		if(parentPopulation)
		{
			//take care of the removal of individual when time run out
			timeLeft -= Time.deltaTime;
			if(timeLeft <= 0)
			{
				parentPopulation.IndividualExhausted();
				Destroy(gameObject);
			}
			switch (status)
			{
				//search of a food node or individual if population is agressive
				case Status.searching:
					
					if(Vector3.Distance(target.position, transform.position) < 0.6f)
					{
						target = generatePosition();
						aiDestinationSetter.target = target;
						aIPath.SearchPath();
					}
				break;
				//if a target is found, go to it
				case Status.seeking:
					if(aiDestinationSetter.target && Vector3.Distance(aiDestinationSetter.target.position, transform.position) < 1.2f)
						status = Status.eating;
				break;
				//eat the food or individual, increasing the food amount for the population
				case Status.eating:
					if(aiDestinationSetter.target)
					{
						aIPath.maxSpeed = 0;
						if(aiDestinationSetter.target.tag == "Food")
							parentPopulation.foodEaten(Time.deltaTime / 2 + (stats[(int)GameInfo.PopulationStat.stat.bite] / 10));
						else
							parentPopulation.foodEaten(Time.deltaTime * (stats[(int)GameInfo.PopulationStat.stat.agression] / 20));
					}
				break;
			}
			//if the food has decayed, go back to finding a new target
			if(!aiDestinationSetter.target)
			{
				status = Status.searching;
				target = generatePosition();
				aiDestinationSetter.target = target;
				aIPath.maxSpeed = stats[(int)GameInfo.PopulationStat.stat.speed] * 2f + 15f;
				aIPath.SearchPath();
			}
		}
	}

	/// <summary>
	/// pick a random position in the arena
	/// </summary>
	/// <returns>return the generated position</returns>
	private Transform generatePosition()
	{ 
		float maximumDistance = GameInfo.populationCount == 3 ? 97 : GameInfo.arenaBounds.max.x;
		//generate the position between the edge of the arena and its maximum distance from it
		newTarget.position = Vector3.Lerp(new Vector3(Random.Range(0, 112), 1, 0), new Vector3(56, 1, maximumDistance), Random.Range(0f,1f));
		//rotate it to align it with the edge on wich the population spawn
		newTarget.RotateAround(center, Vector3.up, 360 - parentPopulation.populationOrder * (90 - ((GameInfo.populationCount - 2) * 180 / GameInfo.populationCount - 90)));
		return newTarget;
	} 

	private void OnTriggerEnter(Collider other) 
	{
		//If a node of food or individual get in the agrssion range
		if(status == Status.searching && (other.gameObject.tag == "Food" || (stats[(int)GameInfo.PopulationStat.stat.agression] > 0 && (other.gameObject.tag == "Individual" && other.gameObject.GetComponent<IndividualController>().parentPopulation != parentPopulation))))
		{
			status = Status.seeking;
			aiDestinationSetter.target = other.gameObject.transform;
			aIPath.SearchPath();
		}
	}
}
