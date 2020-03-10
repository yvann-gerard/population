using UnityEngine;

public class PopulationController : MonoBehaviour
{
	public GameObject individual;
	public int population;
	public int populationLeft;
	public float offset = 1.05f;
	//size of one side of the arena
	private float size = 109.9f;
	private Color color;
	private GameInfo.PopulationStat stats;
	public float food = 0;
	public GameController gameController;
	public int populationOrder;

	public void Init(Color color, GameInfo.PopulationStat stats){
		this.color = color;
		this.stats = stats;
	}

	private void Start() {
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		populationLeft = population;
		InstantiateIndividuals();
	}

	/// <summary>
	/// spawn all the individual of this population
	/// </summary>
	public void InstantiateIndividuals()
	{
		for (float i = size / (population + 1); i + offset < size ; i += size / (population + 1))
		{
			GameObject currentIndivudual = Instantiate(individual, transform.position + transform.right * (i + offset), transform.rotation);
			InitIndividual(currentIndivudual);
		}
	}

	private void InitIndividual(GameObject currentIndivudual)
	{
		currentIndivudual.GetComponent<Renderer>().material.color = color;
		currentIndivudual.GetComponent<IndividualController>().Init(this, stats);
	}

	public void foodEaten(float amount)
	{
		food += amount;
	}

	public void IndividualExhausted()
	{
		populationLeft -= 1;
		if(populationLeft == 0)
		{
			//change the total number of indivual from the amount of food eaten by them
			populationLeft = population = Mathf.FloorToInt(Mathf.Clamp(food / population, 0, stats.stats[(int)GameInfo.PopulationStat.stat.stockage] / 5 + 2) * population);
			gameController.DeactivatePopulation();
		}
	}
}