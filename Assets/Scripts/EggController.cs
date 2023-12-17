using DG.Tweening;
using MimicSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggController : MonoBehaviour
{
    private float LayTime;
    private float timeToEclosion = 10f;
    [SerializeField]
    private GameObject mimicPref;
    private bool isDestroying;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        LayTime = Time.time + timeToEclosion;
        GameManager.instance.Eggs.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(LayTime <= Time.time && !isDestroying)
        {
            //Spawn mimic
            GameObject mimic = Instantiate(mimicPref, transform.position, transform.rotation);
            //Add mimic to GameManager
            GameManager.instance.Mimics.Add(mimic);
            //Init mimic
            mimic.GetComponent<EnemyController>().Init();
            //Self destroy
            StartCoroutine(SelfDestroy());
        }
    }

    /// <summary>
    /// Self destroy proccess
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelfDestroy()
    {
        isDestroying = true;
        //Stop anim
        anim.SetTrigger("End");
        //Remove from GameManager
        GameManager.instance.KillEgg(this.gameObject);
        yield return new WaitForSeconds(0.5f);
        //Shrink before destroy
        transform.DOScaleY(0.01f, 2.5f);
        transform.DOScaleX(0.01f, 2.5f).SetEase(Ease.InCirc);
        transform.DOScaleZ(0.01f, 2.5f).SetEase(Ease.InCirc);
        yield return new WaitForSeconds(2.5f);
        //Destroy
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Handle particle collision with fire particles
    /// </summary>
    /// <param name="other"></param>
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire") && !isDestroying)
        {
            StartCoroutine(SelfDestroy());
        }
    }
}
