using UnityEngine;
using System.Collections;

public class MoveHurdle : MonoBehaviour {

	// Use this for initialization
	public GameObject target;
	public Method method;
	public MethodType type;
	public EaseType easetype;
	public LoopType loopType;
//	public Axis axis1;
//	public float AxisValue;
//	public Axis axis2;
//	public Axis axis3;
	public float XValue;
	public float YValue;
	public float ZValue;

	public float duration;
	public float Delay;
	public float startDelay = 0f;
	public bool ignoreTimeScale = false;
	public enum EaseType{
		easeInQuad,
		easeOutQuad,
		easeInOutQuad,
		easeInCubic,
		easeOutCubic,
		easeInOutCubic,
		easeInQuart,
		easeOutQuart,
		easeInOutQuart,
		easeInQuint,
		easeOutQuint,
		easeInOutQuint,
		easeInSine,
		easeOutSine,
		easeInOutSine,
		easeInExpo,
		easeOutExpo,
		easeInOutExpo,
		easeInCirc,
		easeOutCirc,
		easeInOutCirc,
		linear,
		spring,
		/* GFX47 MOD START */
		//bounce,
		easeInBounce,
		easeOutBounce,
		easeInOutBounce,
		/* GFX47 MOD END */
		easeInBack,
		easeOutBack,
		easeInOutBack,
		/* GFX47 MOD START */
		//elastic,
		easeInElastic,
		easeOutElastic,
		easeInOutElastic,
		/* GFX47 MOD END */
		punch
	}
	public enum Method{
		add,
		by,
		from,
		to,
		update
	}
	public enum MethodType{
		move,
		rotate,
		scale
	}
	public enum LoopType{

		none,

		loop,

		pingPong
	}
	public enum Axis{
		x,
		y,
		z
	}
	public bool PlayOnStart = true;
	void Start () 
	{
		if(PlayOnStart)
		Invoke("startAnim",startDelay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startAnim()
	{
		//iTween.Launch(target, iTween.Hash("x",XValue,"y",YValue,"z",ZValue, "easetype",easetype.ToString(), "looptype", loopType.ToString(),"time", duration, "delay", Delay,"type",type.ToString(), "method",method.ToString(),"islocal",true,"ignoretimescale",ignoreTimeScale ));
	}
	public void pauseAnim()
	{
		iTween.Pause(target);
	}
	public void resumeAnim()
	{
		iTween.Resume(target);
	}
	public void stopAnim()
	{
		iTween.Stop(target);
	}

	public void stopOnCompleteSingleIteration(){
		StartCoroutine(stopAfterDuration());
	}

    public void Restart()
    {
      //  stopAnim();
        this.transform.localPosition = Vector3.zero;
        startAnim();
    }

	IEnumerator stopAfterDuration(){
		yield return new WaitForSeconds(duration + Delay);
		iTween.Pause(target);
	}

	private Vector3 originalPos;

	public void startAnimDisableOnComplete()
	{
		originalPos = target.transform.localPosition;
		//iTween.Launch(target, iTween.Hash("x",XValue,"y",YValue,"z",ZValue, 
		//	"easetype",easetype.ToString(), "looptype", loopType.ToString(),
		//	"time", duration, "delay", Delay,"type",type.ToString(), 
		//	"method",method.ToString(),"islocal",true,"ignoretimescale",ignoreTimeScale, 
		//	"oncompletetarget", this.gameObject, "oncomplete", "DisableTarget" ));
	}


	void DisableTarget(){
		target.SetActive (false);
		target.transform.localPosition = originalPos;
	}
}