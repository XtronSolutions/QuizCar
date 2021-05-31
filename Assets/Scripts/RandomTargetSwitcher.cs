using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RandomTargetSwitcher : MonoBehaviour {

    public List<vehicleHandling> Targets;
    //public BC_AI_Helper[] AI_Cars;
    public List<BC_AI_NavMeshPathCalculator> AI_Cars_Path_Calculator;
    int targetIndex = 0;
    // Use this for initialization
    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        Targets = FindObjectsOfType<vehicleHandling>().ToList();
        AI_Cars_Path_Calculator = FindObjectsOfType<BC_AI_NavMeshPathCalculator>().ToList();
      
        StartCoroutine(SwitchTargets());
    }
    public void RemoveDestroyedItemsFromList()
    {
        Targets = Targets.Where(x => x != null).ToList();
        AI_Cars_Path_Calculator = AI_Cars_Path_Calculator.Where(x => x != null).ToList();
        StopCoroutine(SwitchTargets());
        StartCoroutine(SwitchTargets());
    }

    IEnumerator SwitchTargets()
    {
        foreach (BC_AI_NavMeshPathCalculator Target_Calculator in AI_Cars_Path_Calculator)
        {
            if (Target_Calculator.target == null)
            {
                Target_Calculator.target = ChooseRandomTarget().transform;
                while (CheckForSelfTarget(Target_Calculator))
                {
                    Target_Calculator.target = ChooseRandomTarget().transform;
                    yield return new WaitForEndOfFrame();
                }
            }
            else 
            {
                while (CheckForSelfTarget(Target_Calculator))
                {
                    Target_Calculator.target = ChooseRandomTarget().transform;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        if (!IsPlayerCarTargetted())
        {
            AI_Cars_Path_Calculator[Random.Range(0, AI_Cars_Path_Calculator.Count - 1)].target = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER_CAR).gameObject.transform;
        }
        yield return new WaitForSeconds(30f);
        StartCoroutine(SwitchTargets());
    }
    GameObject ChooseRandomTarget()
    {
        GameObject target = Targets[targetIndex++].gameObject;
        if (targetIndex == Targets.Count)
            targetIndex = 0;
        return target;
    }
    bool CheckForSelfTarget(BC_AI_NavMeshPathCalculator car)
    {
        bool IsTargetingSelf = (car.target.gameObject == car.gameObject);
        return IsTargetingSelf;

    }

    bool IsPlayerCarTargetted()
    {
        foreach (BC_AI_NavMeshPathCalculator Target_Calculator in AI_Cars_Path_Calculator)
        {
            if (Target_Calculator.target.tag.Equals(Constants.TAG_PLAYER_CAR))
            {
                return true;
            }
        }
        return false;
    }
}