using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemie : MonoBehaviour
{
    [SerializeField] bool isDoubler;
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;


    public Animator AnimatorController;
    public NavMeshAgent Agent;
    public GameObject miniGoblin;


    private float lastAttackTime = 0;
    private bool isDead = false;


    private void Start()
    {
        SceneManager.Instance.AddEnemie(this);
        Agent.SetDestination(SceneManager.Instance.Player.transform.position);
        Debug.Log("aaa");   
        

    }

    private void Update()
    {
        if(isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            Agent.isStopped = true;
            return;
        }

        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);
        if (distance <= AttackRange)
        {
            Agent.isStopped = true;
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                SceneManager.Instance.Player.Hp -= Damage;
                AnimatorController.SetTrigger("Attack");
            }
        }
        else
        {
            Agent.isStopped = false;
            Agent.SetDestination(SceneManager.Instance.Player.transform.position);
        }
        AnimatorController.SetFloat("Speed", Agent.speed); 

    }



    private void Die()
    {
        SceneManager.Instance.RemoveEnemie(this);
        SceneManager.Instance.Player.Hp += 1;
        isDead = true;
        AnimatorController.SetTrigger("Die");
        if (isDoubler)
        {
            for (int i = 0; i < 2; i++) 
            {
                Instantiate(miniGoblin, new Vector3(transform.position.x + i * 2, transform.position.y, transform.position.x), Quaternion.identity);
            }
            
        }
    }

}
