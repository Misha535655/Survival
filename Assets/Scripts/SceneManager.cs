using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    public List<Enemie> Enemies;
    public GameObject Lose;
    public GameObject Win;
    public TextMeshProUGUI waweText;

    private int currWave = 0;
    private int allWave = 0;
    [SerializeField] private LevelConfig Config;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnWave();
        allWave = Config.Waves.Length;
    }
    private void Update()
    {
        waweText.text = allWave.ToString() + " / " + currWave.ToString(); 
    }

    public void AddEnemie(Enemie enemie)
    {
        Enemies.Add(enemie);
    }

    public void RemoveEnemie(Enemie enemie)
    {
        Enemies.Remove(enemie);
        if(Enemies.Count == 0)
        {
            SpawnWave();
        }
    }

    public void GameOver()
    {
        Lose.SetActive(true);
    }

    private void SpawnWave()
    {
        if (currWave >= Config.Waves.Length)
        {
            Win.SetActive(true);
            return;
        }

        var wave = Config.Waves[currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(character, pos, Quaternion.identity);
        }
        currWave++;

    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    

}
