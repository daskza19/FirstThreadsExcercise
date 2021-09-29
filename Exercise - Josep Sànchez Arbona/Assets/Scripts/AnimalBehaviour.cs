using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBehaviour : MonoBehaviour
{
    public ScriptableAnimalBase animal;

    private string _animalName = "";
    private string _description = "";
    private bool _isMoving = false;
    private float _maxLifeTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        if (animal != null)
        {
            _animalName = animal.animalName;
            _description = animal.description;
            _isMoving = animal.isMoving;
            _maxLifeTime = animal.maxLifeTime;
        }
        StartCoroutine(DeadTime);
        SelectMovement();
    }

    public void SelectMovement()
    {
        if (!_isMoving)
            return;


    }


    public Update()
    {

    }

    IEnumerator DeadTime()
    {
        yield return new WaitForSeconds(_maxLifeTime);
    }
}
