using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class RaceData : ScriptableObject {
	

	public int numberOfRaces;
	//public List<string> raceNames = new List<string>();
	public string[] raceNames;
	public bool[] raceLocked;
	public int[] unlockAmount;
	public int[] firstPrize;
	public int[] secondPrize;
	public int[] thirdPrize;
	public int raceNumber;
	public int[] raceLaps;
	public int[] lapLimit;
	public int[] raceRewards;
	[Range(1,64)]public int[] numberOfRacers;
	[Range(1,64)]public int[] racerLimit;
	public int vehicleNumber;
	public float bestTime;
	public int currency;
}