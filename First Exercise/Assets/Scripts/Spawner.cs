using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public List<GameObject> animals;
    public List<Image> cooldownImagesUI;
    public bool isSharedCooldown = false;
    public float actualSharedCooldown = 0;
    public List<float> animalsUniqueActualTime;

    /*This function will be called when the user press one of the UI buttons.
    i represents the position of the list animals, to know who animal we are going to spawn*/
    public void SpawnNewAnimal(int i) 
    {
        if (animals[i] == null) // First we check that if that number is in the list animals. If it doesn't, we go out of this function
            return;

        if (isSharedCooldown)
        {

        }
        else
        {
            if(animals[i].GetComponent<AnimalBehaviour>().animalData.creationCooldown < animalsUniqueActualTime[i])
            {
                animalsUniqueActualTime[i] = 0;
                StartCoroutine(DoEffectButtonUI(animals[i].GetComponent<AnimalBehaviour>().animalData.creationCooldown, i));
                Instantiate(animals[i]); // In the start function of this new GameObject, all the characteristics will be assigned by the Scriptable Object data
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < animals.Count; i++)
        {
            animalsUniqueActualTime.Add(animals[i].GetComponent<AnimalBehaviour>().animalData.creationCooldown);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < animals.Count; i++)
        {
            if (animalsUniqueActualTime[i] < 30) // To no go to high numbers, if one of the options reach 30s (no cooldown reaches this time) is still in this number
            {
                animalsUniqueActualTime[i] += Time.deltaTime;
            }
        }
    }

    IEnumerator DoEffectButtonUI(float time, int which)
    {
        while(animalsUniqueActualTime[which] < time)
        {
            cooldownImagesUI[which].fillAmount = 1 - animalsUniqueActualTime[which] / time;
            yield return new WaitForEndOfFrame();
        }
    }
}
