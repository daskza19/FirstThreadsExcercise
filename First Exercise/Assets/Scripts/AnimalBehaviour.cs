using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalBehaviour : MonoBehaviour
{
    //All animal data (to do the behaviour)
    public ScriptableAnimal animalData;
    private Sprite _sprite;
    private string _name = "Default";
    private string _description = "Default";
    private bool _isMoving;
    private float _maxLife = 5;
    private float actualLife = 5;

    //All UI animal data (to show to the player)
    public SpriteRenderer animalSpriteUI;
    public Image cooldownSpriteUI;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (animalData == null) //If the animal data isn't loaded do a debug
        {
            Debug.Log("Animal data isn't loaded (Animal " + gameObject.name + ")");
        }
        else //If the animal data is loaded, change the value of the variables of this script
        {
            _sprite = animalData.sprite;
            animalSpriteUI.sprite = _sprite; //Actualize the animal base UI image
            _name = animalData.animalName;
            _description = animalData.description;
            _isMoving = animalData.isMoving;
            _maxLife = animalData.maxLife;
            actualLife = _maxLife;
        }
        RandomizePosition();
        if (_isMoving) animator.SetTrigger("Activate");
        StartCoroutine(DeadCounter()); //Do the coroutine that will destroy the actual gameobject when pass the maxLife value
    }

    // Update is called once per frame
    void Update()
    {
        //Actualize the life time left and actualize the UI
        actualLife -= Time.deltaTime;
        cooldownSpriteUI.fillAmount = actualLife / _maxLife;
    }

    IEnumerator DeadCounter()
    {
        yield return new WaitForSeconds(_maxLife);
        Destroy(this.gameObject);
    }

    public void RandomizePosition()
    {
        int x = Random.Range(-5, 5);
        int y = Random.Range(-3, 3);
        transform.position = new Vector3(x, y, 0);
    }
}
