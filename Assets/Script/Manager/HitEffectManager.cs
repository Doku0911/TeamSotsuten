﻿// ---------------------------------------------------
//  ヒットエフェクトを管理するスクリプト
// 
//  code by m_yamada
// ---------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitEffectManager : Singleton<HitEffectManager> {

    [System.Serializable]
    public struct HitEffectData
    {
        /// <summary>
        /// 呼び出す攻撃エフェクトを登録
        /// </summary>
        public GameObject strength;
        public GameObject weak ;

        public CharacterSelectManager.JobType type;
    }

    [SerializeField]
    HitEffectData[] hitEffectTypeList = new HitEffectData[2];

    [SerializeField]
    int createNum = 5;


    List<ParticleSystem> weakHitEffectList = new List<ParticleSystem>();
    List<ParticleSystem> strengthHitEffectList = new List<ParticleSystem>();

    int weakPlayIndex = 0;
    int strengthPlayIndex = 0;

    PhotonView view = null;

    public override void Awake()
    {
        base.Awake();

    }

    public override void Start()
    {
        base.Update();

        if (!ConnectionManager.IsSmartPhone)
        {
            Destroy(gameObject);
            return;
        }

        foreach (var hitEffectTyoe in hitEffectTypeList)
        {
            if (hitEffectTyoe.type == PlayerManager.Instance.Data.Job)
            {
                CreateHitEffect(hitEffectTyoe.weak, weakHitEffectList);
                CreateHitEffect(hitEffectTyoe.strength, strengthHitEffectList);
            }
        }

        
        view = GetComponent<PhotonView>();
    }

    /// <summary>
    /// 生成する。
    /// </summary>
    /// <param name="position"></param>
    void CreateHitEffect(GameObject hitEffect, List<ParticleSystem> effectList)
    {
        for (int i = 0; i < createNum; i++)
        {
            var obj = Instantiate(hitEffect);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            effectList.Add(obj.GetComponent<ParticleSystem>());
        }
    }

    /// <summary>
    /// ヒットエフェクトを再生する。
    /// </summary>
    /// <param name="hitSkilType"></param>
    /// <param name="position"></param>
    public void EffectPlay(MotionManager.MotionSkillType hitSkilType,Vector3 position)
    {
        if (hitSkilType == MotionManager.MotionSkillType.STRENGTH)
        {
            EffectPlay(strengthHitEffectList, position, ref strengthPlayIndex);
        }
        else if (hitSkilType == MotionManager.MotionSkillType.WEAK)
        {
            EffectPlay(weakHitEffectList, position, ref weakPlayIndex);
        }
    }


    /// <summary>
    /// 弱い方のヒットエフェクトを再生する
    /// </summary>
    /// <param name="position"></param>
    void EffectPlay(List<ParticleSystem> effectList, Vector3 position, ref int playIndex)
    {
        effectList[playIndex].gameObject.SetActive(true);
        effectList[playIndex].transform.position = position;
        effectList[playIndex].Play();
        playIndex++;

        playIndex = effectList.Count <= playIndex ? 0 : playIndex;

    }


    public override void Update()
    {
        base.Update();

        Stop(strengthHitEffectList);
        Stop(weakHitEffectList);
    }

    void Stop(List<ParticleSystem> hitEffectList)
    {
        for (int i = 0; i < hitEffectList.Count; i++)
        {
            if (!hitEffectList[i].gameObject.activeInHierarchy) continue;

            if (!hitEffectList[i].isPlaying)
            {
                hitEffectList[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 書かないといけない関数
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
