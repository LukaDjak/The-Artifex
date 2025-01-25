using UnityEngine;

public class ExplosiveThrow : MonoBehaviour
{
    public GameObject grenadePrefab; // Grenade prefab reference
    public Transform throwPoint; // Where the grenade will be thrown from
    public float throwForce = 10f; // Force applied to the grenade when thrown
    public float aimSpeed = 1f; // Speed at which the aim changes

    private bool isAimingGrenade = false;
    private GameObject currentGrenade = null;

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && player.chestMultipliers.grenades > 0) // Right click to equip grenade
            EquipGrenade();

        if (isAimingGrenade)
            AimGrenade(); // Let the player aim the trajectory

        if (Input.GetMouseButtonDown(0) && isAimingGrenade) // Left click to throw
            ThrowGrenade();
    }

    private void EquipGrenade()
    {
        isAimingGrenade = true;
        currentGrenade = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
    }

    private void AimGrenade()
    {
        // Move the grenade based on mouse position to simulate aiming
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // We are in 2D, so set the z-coordinate to 0
        Vector3 direction = mousePosition - currentGrenade.transform.position;
        currentGrenade.transform.up = direction.normalized; // Point grenade in the direction of the mouse
    }

    private void ThrowGrenade()
    {
        if (player.chestMultipliers.grenades > 0)
        {
            Rigidbody2D rb = currentGrenade.GetComponent<Rigidbody2D>();
            rb.isKinematic = false; // Enable physics

            // Throw the grenade in the aimed direction with force
            rb.AddForce(currentGrenade.transform.up * throwForce, ForceMode2D.Impulse);

            player.chestMultipliers.grenades--;
            isAimingGrenade = false;

            Destroy(currentGrenade, 5f); // Destroy the grenade after 5 seconds (or set it to explode)
        }
    }
}