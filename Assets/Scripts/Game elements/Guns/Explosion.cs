using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float damage;

    public float speed;

    public MeshRenderer mr;

    private Color tempcolor;

    void Start()
    {
        tempcolor = mr.material.color;
    }

    void Update()
    {
        tempcolor.a = Mathf.MoveTowards(tempcolor.a, 0, transform.localScale.x / 100); //The bigger the explosion, the faster it fades.

        mr.material.color = tempcolor; //The explosion fades away over time.

        transform.localScale += Vector3.one * Time.deltaTime * speed; //The explosion expands in all directions (defined by Vector3.one) at a given speed.

        if (tempcolor.a <= 0) //The explosion stops doing damage once it reaches 0 visibility.
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") //Explosions will ignore players, shields, collectibles and boundaries, but damage ships directly.
        {
            other.GetComponent<Ship>().Damaged(damage);
        }
    }
}
