using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectAnima
{
	public bool bPlay = false;
	public ParticleSystem CtrScr;
	public float fStartTime;
}
public class TestEffect : MonoBehaviour {

	List<EffectAnima> EffectList = new List<EffectAnima>();
	ParticleSystem CtrScr;
	// Use this for initialization
	Vector3 vPos = Vector3.zero;
	float fStarTime = 0;
	public float fInvert = 0.2f;
	public float fDieTime = 5;
	public float fLast = 0.8f;
	public float fLength = 2;
    public float fWidth = 1;
	GameObject EffectPre;
	void Start () {
		CtrScr = GetComponent<ParticleSystem>();
		fStarTime = Time.time;
		EffectPre = transform.GetChild(0).gameObject;
		if(fDieTime > 0)
			Destroy(gameObject,fDieTime);
	}
	public void InitData(float fInvertTime,float fDestroyTime,float fLastTime,float fRaidu,Vector3 vBorn)
	{
		fInvert = fInvertTime;
		fDieTime = fDestroyTime;
		vPos = vBorn;
		fLast = fLastTime;
		fLength = fRaidu;

	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - fStarTime > fInvert)
		{
			bool bUseOld = false;
			fStarTime = Time.time;
			for(int i = 0; i < EffectList.Count; ++i)
			{
				if(!EffectList[i].bPlay)
				{
					EffectList[i].bPlay = true;
					EffectList[i].CtrScr.Play();
					Vector3 vNewPos = vPos;
					vNewPos.x += Random.Range(-fLength,fLength);
					vNewPos.z += Random.Range(-fWidth,fWidth);
					EffectList[i].CtrScr.transform.localPosition = vNewPos;
					EffectList[i].fStartTime = Time.time;
					bUseOld = true;
					break;
					//vPos.x = Random.Range(0,2);
					//vPos.z = Random.Range(0,2);
				}

			}
			if(!bUseOld)
			{
				GameObject obj = Instantiate(EffectPre,Vector3.zero,Quaternion.identity) as GameObject;
				if(obj != null)
				{
					obj.transform.parent = transform;

					obj.transform.localScale = new Vector3(1,1,1);
					Vector3 vNewPos = vPos;
					vNewPos.x += Random.Range(-fLength,fLength);
					vNewPos.z += Random.Range(-fLength,fLength);
					obj.transform.localPosition = vNewPos;
					EffectAnima info = new EffectAnima();
					info.bPlay = true;
					info.CtrScr = obj.GetComponent<ParticleSystem>();
					info.fStartTime = Time.time;
					EffectList.Add(info);

				}
			}
		}
		for(int i = 0; i < EffectList.Count; ++i)
		{
			if(EffectList[i].bPlay && (Time.time - EffectList[i].fStartTime > fLast))
			{
				EffectList[i].bPlay = false;
				EffectList[i].CtrScr.Stop();
			}
		}
	}
}
