using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	EventSystem eventSystem;
	//regex used to find element with the right name
	Regex regex = new Regex(@"Skill(\d+)");
	int availableSkills = 20;
	public Sprite filledSkill;
	public Sprite emptySkill;
	public GameObject availableSkillPointsPanel;
	public GameObject[] populationPanels;
	public GameObject[] skillPanels;

	private void Start() {
		eventSystem = this.GetComponent<EventSystem>();
		//Default the populations count to 4
		GameInfo.populationCount = 4;
	}

	public void SetPopulation(int populationIndex)
	{
		GameInfo.populationCount = populationIndex + 3;
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0))
			Clicked();
	}

	public void MainMenuPlay()
	{
		transform.Find("ParametersMenu").gameObject.SetActive(true);
		transform.Find("MainMenu").gameObject.SetActive(false);
		GameInfo.populationStats = GeneratePopulationStats();
		//show the stat panel for each generated population
		for (int i = 0; i < 5; i++)
		{
			if(i < GameInfo.populationCount - 1)
			{
				populationPanels[i].SetActive(true);
				int statIndex = 0;
				foreach(Text text in populationPanels[i].GetComponentsInChildren<Text>())
				{
					if(text.text == "0")
					{
						text.text =  GameInfo.populationStats[i+1].stats[statIndex].ToString();
						statIndex++;
					}
				}
			}
			else
				populationPanels[i].SetActive(false);
		}
	}

	//create a population et generate random stats for it
	private GameInfo.PopulationStat[] GeneratePopulationStats()
	{
		Random random = new Random();
		GameInfo.PopulationStat[] populationStat = new GameInfo.PopulationStat[GameInfo.populationCount];
		for (int i = 1; i < populationStat.Count(); i++)
		{
			int availablePoints = 20;
			populationStat[i] = new GameInfo.PopulationStat();
			//divide the points in the stats, trying to get one higher than the others
			for (int j = 0; j < populationStat[i].stats.Count(); j++)
			{
				int pointSpent = Random.Range(0,availablePoints+1);
				populationStat[i].stats[j] = pointSpent;
				availablePoints -= pointSpent;
				if(availablePoints == 0)
					break;
			}
			//shuffle the stats
			for (int j = 0; j < populationStat[i].stats.Count(); j++)
			{
				int firstRandom = Random.Range(0, populationStat[i].stats.Count());
				int secondRandom = Random.Range(0, populationStat[i].stats.Count());
				(populationStat[i].stats[firstRandom], populationStat[i].stats[secondRandom]) = (populationStat[i].stats[secondRandom], populationStat[i].stats[firstRandom]);
			}
		}
		return populationStat;
	}

	public void ParametersPlay()
	{
		GameInfo.populationStats[0] = new GameInfo.PopulationStat();
		//set the stats of the player's population to the number given in the skill pannels
		for (int i = 0; i < skillPanels.Length; i++)
		{
			GameInfo.populationStats[0].stats[i] = skillPanels[i].GetComponentsInChildren<Image>().Where(x => x.sprite == filledSkill).Count();
		}
		SceneManager.LoadScene("SampleSceneInGame");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	private void Clicked() {
		PointerEventData ped = new PointerEventData(eventSystem);
		ped.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(ped, results);
		//get the menu item clicked on
		foreach (RaycastResult uiElement in results)
		{
			Match matched = regex.Match(uiElement.ToString());
			//if the user clicked on a skill
			if(matched.Groups.Count > 1)
			{
				//if the skill is upgraded add points to the skill and remove them from the available points
				if(uiElement.gameObject.GetComponent<Image>().sprite == emptySkill)
				{
					int imageindex = 1;
					foreach (Image skillImage in uiElement.gameObject.transform.parent.GetComponentsInChildren<Image>())
					{
						if(skillImage.sprite != filledSkill && skillImage.sprite != emptySkill)
							continue;
						if(imageindex > int.Parse(matched.Groups[1].Value) || availableSkills == 0)
							break;
						if(skillImage.sprite != filledSkill)
						{
							skillImage.sprite = filledSkill;
							availableSkills--;
						}
						imageindex++;
					}
				}
				//if the skill is downgraded remove points from the skill and add them to the available points
				else
				{
					int imageindex = 1;
					foreach (Image skillImage in uiElement.gameObject.transform.parent.GetComponentsInChildren<Image>())
					{
						if(skillImage.sprite != filledSkill && skillImage.sprite != emptySkill)
							continue;
						imageindex++;
						if(imageindex <= int.Parse(matched.Groups[1].Value))
							continue;
						if(skillImage.sprite != emptySkill)
						{
							skillImage.sprite = emptySkill;
							availableSkills++;
						}
					}
				}
			}
			int availableSkillindex = 0;
			//update the dispay of available points
			foreach (Image skillImage in availableSkillPointsPanel.GetComponentsInChildren<Image>())
			{
				if(skillImage.sprite != filledSkill && skillImage.sprite != emptySkill)
					continue;
				if(availableSkillindex < availableSkills)
					skillImage.sprite = filledSkill;
				else
					skillImage.sprite = emptySkill;
				availableSkillindex++;
			}
		}
	}
}