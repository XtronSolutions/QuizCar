using UnityEngine;
using System.Collections;

public class CharacterAnimationController : MonoBehaviour {
	
	Animator _animator;
	
	public static int _still = Animator.StringToHash("Base Layer.Still");
	public static int _yayleft = Animator.StringToHash("Base Layer.YayLeft");
	public static int _yayright = Animator.StringToHash("Base Layer.YayRight");
	public static int _byeleft = Animator.StringToHash("Base Layer.ByeLeft");
	public static int _byeright = Animator.StringToHash("Base Layer.ByeRight");
	public static int _win = Animator.StringToHash("Base Layer.Win");
	public static int _lose = Animator.StringToHash("Base Layer.Lose");
	public static int _strokeleft = Animator.StringToHash("Base Layer.StrokeInAirLeft");
	public static int _strokeright = Animator.StringToHash("Base Layer.StrokeInAirRight");
	public static int _lookaround = Animator.StringToHash("Base Layer.LookAround");
	
	public enum Triggers
	{
		isStill,isYayLeft,isYayRight,isByeRight,isByeLeft,isWin,isLose,isStrokeLeft,isStrokeRight,isLookAround
	}
	
	public Triggers _currentTrigger;
	
	public void SetAnimationTrigger(Triggers trigger){
		_currentTrigger = trigger;
		if (_animator == null) {
			_animator = GetComponent<Animator> ();
		}
		_animator.SetTrigger (_currentTrigger.ToString());
		
	}
	
	IEnumerator BoostAnimations(){
//		SetAnimationTrigger (Triggers.isByeLeft);
		yield return new WaitForSeconds (0.5f);
//		SetAnimationTrigger (Triggers.isYayRight);
		yield return new WaitForSeconds (0.3f);
//		SetAnimationTrigger (Triggers.isYayLeft);
		yield return new WaitForSeconds (0.3f);
		SetAnimationTrigger (Triggers.isLookAround);
		
		
		yield return null;
	}
	public void PlayBoostAniamtions(){
		StartCoroutine (BoostAnimations());
	}
	
	// Use this for initialization
	void Start () {
		if (_animator == null)
		_animator = GetComponent<Animator> ();
		AnimationSequence ();
	}
	
	void AnimationSequence(){
		StartCoroutine (StartAnimationSequence());
	}
	
	IEnumerator StartAnimationSequence(){
		SetAnimationTrigger(Triggers.isLookAround);
		yield return new WaitForSeconds(1.0f);
//		SetAnimationTrigger(Triggers.isByeLeft);
		yield return new WaitForSeconds(2.0f);
		SetAnimationTrigger(Triggers.isLookAround);
		StartCoroutine (RaceAnimationSequence());
		yield return null;
		
	}
	
	IEnumerator RaceAnimationSequence(){
		while (true) {
			SetAnimationTrigger(Triggers.isLookAround);
			yield return new WaitForSeconds(1.0f);
//			SetAnimationTrigger(Triggers.isByeLeft);
			yield return new WaitForSeconds(2.0f);
			SetAnimationTrigger(Triggers.isLookAround);
			//yield return new WaitForSeconds(2.0f);
			//SetAnimationTrigger(Triggers.isStrokeRight);
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		//		if (_animator.GetCurrentAnimatorStateInfo (0).nameHash == _still) {
		//				_currentTrigger =Triggers.isStill;
		//		}
	}
	
}
