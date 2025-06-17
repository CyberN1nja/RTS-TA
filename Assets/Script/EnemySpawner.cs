using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;         // Prefab musuh
    public float spawnInterval = 2f;       // Waktu antar spawn
    public int maxEnemies = 10;            // Jumlah maksimum musuh
    public Vector2 spawnAreaMin;           // Offset area minimal dari base
    public Vector2 spawnAreaMax;           // Offset area maksimal dari base

    private int currentEnemies = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies)
            return;

        Vector3 spawnOffset = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            0f,
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Vector3 intendedSpawnPos = transform.position + spawnOffset;

        // Pastikan spawn di atas NavMesh
        if (NavMesh.SamplePosition(intendedSpawnPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            GameObject newEnemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            currentEnemies++;

            // ✅ Atur AttackController dan daftarkan enemy ke GameManager lewat Unit.cs
            AttackController ac = newEnemy.GetComponent<AttackController>();
            if (ac != null)
            {
                ac.isPlayer = false;
            }
            else
            {
                Debug.LogWarning("❗ Enemy prefab tidak memiliki AttackController! Harus diisi agar bisa dikenali GameManager.");
            }

            // ✅ Pastikan ada Unit.cs
            Unit unit = newEnemy.GetComponent<Unit>();
            if (unit == null)
            {
                Debug.LogWarning("❗ Enemy prefab tidak memiliki komponen Unit.cs. Tambahkan agar bisa hitung menang/kalah.");
            }
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] Gagal spawn musuh: posisi tidak valid di NavMesh: " + intendedSpawnPos);
        }
    }

    public void OnEnemyDestroyed()
    {
        currentEnemies--;
    }
}
