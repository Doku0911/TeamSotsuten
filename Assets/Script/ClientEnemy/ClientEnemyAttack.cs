﻿//-------------------------------------------------------------
// クライアント側のエネミーの攻撃
//
// code by Gai Takakura
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class ClientEnemyAttack : MonoBehaviour {

    float lifeTime = 60f;   // テスト用ライフタイム

    // ターゲット(プレイヤー)
    [SerializeField]
    private GameObject targetCamera = null;
    public GameObject TargetCamera
    {
        get { return targetCamera; }
        set { if (targetCamera == null) targetCamera = value; }
    }

    // 攻撃のID
    private int id = -1;
    public int ID
    {
        get
        {
            return id;
        }
        set
        {
            if (id == -1) id = value;
        }
    }

    // Update is called once per frame
    void Update () {
	    if(targetCamera != null)
        {
            var targetVector = targetCamera.transform.position - this.transform.position;
            this.transform.position += targetVector.normalized * 3; // 暫定速度 3
        }

        if (GameManager.Instance.GetAttackData(ID).IsLife)
        {
            Destroy(this.gameObject);
            Debug.Log("攻撃は既に当たっています");
        }
            

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f) Destroy(this.gameObject);
    }
}
