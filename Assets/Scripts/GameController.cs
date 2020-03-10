using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	//population prefab
	public GameObject population;
	//food prefab
	public GameObject food;
	//colors of each population
	private Color[] colors= {new Color(1, 0, 0), new Color(1, 0, 1), new Color(0, 0, 1), new Color(0, 1, 1), new Color(0, 1, 0), new Color(1, 1, 0)};
	//prefab for each arena type
	public GameObject[] arenas;
	//color sprites for the population in the menu
	public Image[] populationsColor;
	//text for the individual number in the menu
	public Text[] populationsValue;
	public PopulationController[] populationControllers;
	private int populationsActive = GameInfo.populationCount;
	private int turnCount = 0;
	public Button ReturnButton;
	public GameObject resultPanel;
	public Text resultText;

	private void Start() 
	{
		//set the menu's indicators at the right population number
		for (int i = 0; i < 6; i++)
		{
			if(i > GameInfo.populationCount - 1)
			{
				populationsColor[i].color = new Color(0,0,0,0); 
				populationsValue[i].text = "";
			}
			else
				populationsValue[i].text = "2";
		}
		//spawn the area corresponding to the population number 
		arenas[GameInfo.populationCount-3].SetActive(true);
		Bounds arenaBounds = arenas[GameInfo.populationCount-3].GetComponent<MeshCollider>().bounds;
		GameInfo.arenaBounds = arenaBounds;
		CreatePopulations();
		SpwanFood(10);
	}

	private void Update() 
	{
		if(populationsActive == 0)
		{
			//if there is no individual left and the game is not over then begin the next turn
			if(turnCount < 4)
			{
				int index = 0;
				populationsActive = GameInfo.populationCount;
				//reset the game with the new number of individual
				foreach (PopulationController populationController in populationControllers)
				{
					populationsValue[index].text = "" + populationController.population;
					populationController.InstantiateIndividuals();
					SpwanFood(Mathf.CeilToInt(populationController.population / GameInfo.populationCount / 1.2f));
					index++;
				}
				turnCount++;
			}
			//Show the end game screen if the game has ended
			else if(ReturnButton.interactable == true)
			{
				ReturnButton.interactable = false;
				resultPanel.SetActive(true);
				for (int i = 0; i < populationControllers.Length; i++)
				{
					populationsValue[i].text = "" + populationControllers[i].population;	
				}
				//Show "You Lost" if the player has a smaller number of individuals than the others and "You Win" else
				resultText.text = populationControllers.Any(p => p.population > populationControllers[0].population) ? "You Lost" : "You Won";
			}
		}
	}

	public void returnToMainMenu()
	{
		SceneManager.LoadScene("SampleScene");
	}

	public void DeactivatePopulation()
	{
		populationsActive--;
	}

	/// <summary>
	/// Create and place the populations with all their stats
	/// </summary>
	private void CreatePopulations() 
	{
		Transform populationTransfrom = new GameObject().transform;
		populationControllers = new PopulationController[GameInfo.populationCount];
		for (int i = 1; i <= GameInfo.populationCount; i++)
		{
			GameObject currentPopulation = Instantiate(population, populationTransfrom.position + Vector3.up * 1.5f, populationTransfrom.rotation);
			populationControllers[i-1] = currentPopulation.GetComponent<PopulationController>();
			populationControllers[i-1].Init(colors[i-1], GameInfo.populationStats[i-1]);
			populationControllers[i-1].populationOrder = i-1;
			populationTransfrom.rotation = Quaternion.Euler(0, i * (90 - ((GameInfo.populationCount - 2) * 180 / GameInfo.populationCount - 90)), 0);
			populationTransfrom.position += populationTransfrom.right * -112;
		}
	}

	/// <summary>
	/// Spawn food randomly but equally for all sides of the arena
	/// </summary>
	/// <param name="foodNumber">number of food to spawn wich will be multiplied by the number of sides of the arena</param>
	private void SpwanFood(int foodNumber)
	{
		Transform foodTransfrom = new GameObject().transform;
		Vector3 center = new Vector3(GameInfo.arenaBounds.center.x, 0, 112 / 2 * Mathf.Tan(Mathf.PI*(GameInfo.populationCount - 2)/(2 * GameInfo.populationCount)));
		for (int i = 0; i < foodNumber; i++)
		{
			foodTransfrom.position = Vector3.Lerp(new Vector3(Random.Range(center.x - 56, center.x + 56), 0, 0), center, Random.Range(0f,1f));
			for (int rotationIndex = 0; rotationIndex < GameInfo.populationCount; rotationIndex++)
			{
				Instantiate(food, foodTransfrom.position + Vector3.up * 0.5f, Quaternion.identity);
				foodTransfrom.RotateAround(center, Vector3.up, 360 - 1 * (90 - ((GameInfo.populationCount - 2) * 180 / GameInfo.populationCount - 90)));
			}
		}
	}
}