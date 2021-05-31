using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit;

public class HUD_Script : MonoBehaviour 
{
    public Text coinText;
	private Racer_Detail racerDetail;
	private Race_Manager raceManager;
	private Racer_Register racerReg;
	private List<Racer_Detail> racerLists = new List<Racer_Detail> ();

	public Text Player_Place;
	public Text Total_Place;
	public Text Current_Lap;
	public Text Total_Lap;
	public Image progressBar;
	public Slider progressBar2;
	public Text WrongWay_Text;

	public bool isPlayerFinished = false;

	private float val, A, B, a, b, factor;

    public Sprite []dummyAvatars;
    public Sprite carAvatar;

    public Slider[] opponentsProgress;

    public GameObject carPointer;
    List<CarAvatar> carAvatarsList = new List<CarAvatar>();
	class OpponentProgressBars{
		public Slider slider;
		public Racer_Detail racerDetail;

		public OpponentProgressBars(Slider slider, Racer_Detail racerDetail){
			this.slider = slider;
			this.racerDetail = racerDetail;
		}
	}

	List<OpponentProgressBars> listOfOpponentProgressBars;

	void Start()
	{
		a = 0;
		b = 1;
		A = 0;

		racerReg = PlayerManagerScript.instance.Car.GetComponent<Racer_Register> ();
		raceManager = this.GetComponent<Race_Manager> ();
		isPlayerFinished = false;

		listOfOpponentProgressBars = new List<OpponentProgressBars> ();
	}

	public void GetRaceDetail(List<Racer_Detail> listsOfRacers)
	{
		racerLists = listsOfRacers;
        //		foreach (Racer_Detail _RD in racerLists) 
        //		{
        //			if (_RD.IsPlayer) 
        //			{
        //				racerDetail = _RD;
        //				B = raceManager.RaceLength;
        //				val = (racerDetail.RacerDistance - A)/((B - A) * (b - a) + a);
        //				factor = 1;
        ////				Debug.Log("dISTANCE---" + racerDetail.RacerDistance);
        //			}
        //		}
       // Debug.Log("Raceers: "+racerLists.Count);
        for(int k=0;k<opponentsProgress.Length;k++)
        {
            var img = opponentsProgress[k].targetGraphic.GetComponent<Image>();
            img.enabled = false;
        }
		if (racerLists.Count >= 2) {
			int j = 0;
			for (int i = 0; i < racerLists.Count; i++) {
               // Debug.Log("Raceers: " + racerLists[i].RacerName);
                if (!racerLists [i].IsPlayer) {
                    if (!isIDExist(racerLists[i])) { 
                        var img = opponentsProgress [j].targetGraphic.GetComponent<Image> ();
					img.enabled = true;
					img.sprite = racerLists [i].avatar;
					racerLists [i].avatarholder = img;
					listOfOpponentProgressBars.Add (new OpponentProgressBars (opponentsProgress [j], racerLists [i]));
                    
                    
                        GameObject c = Instantiate(carPointer);
                        c.GetComponent<CarAvatar>().SetTexture(img.sprite);
                        c.GetComponent<CarAvatar>().target = racerLists[i].me;
                        c.GetComponent<CarAvatar>().ID = racerLists[i].ID;
                        carAvatarsList.Add(c.GetComponent<CarAvatar>());
                    }
                    
					j++;
				}
                if(Constants.isMultiplayerSelected)
                {
                    if (racerLists[i].ID!=PlayerManagerScript.instance.Car.GetComponent<Racer_Register>().RacerDetail.ID)
                    {
                        if (!isIDExist(racerLists[i]))
                        {
                            var img = opponentsProgress[j].targetGraphic.GetComponent<Image>();
                        img.enabled = true;
                        if (racerLists[i].avatar == null)
                        {
                            img.sprite = dummyAvatars[i];
                        }
                        else
                        {
                            img.sprite = racerLists[i].avatar;
                        }
                        racerLists[i].avatarholder = img;
                        listOfOpponentProgressBars.Add(new OpponentProgressBars(opponentsProgress[j], racerLists[i]));

                       
                            GameObject c = Instantiate(carPointer);
                            c.GetComponent<CarAvatar>().SetTexture(img.sprite);
                            c.GetComponent<CarAvatar>().target = racerLists[i].me;
                            c.GetComponent<CarAvatar>().ID = racerLists[i].ID;
                            carAvatarsList.Add(c.GetComponent<CarAvatar>());
                        }
                        j++;
                    }


                }
			}
		}

	}
    bool isIDExist(Racer_Detail rd)
    {
        foreach(CarAvatar cv in carAvatarsList)
        {
            if (cv.ID == rd.ID)
                return true;
        }
        return false;
    }
	void Update()
	{
		if (PlayerManagerScript.instance.Car.GetComponent<Racer_Register> () == null) 
		{
			racerReg = PlayerManagerScript.instance.Car.GetComponent<Racer_Register> ();
		}

		if (racerDetail == null) {
			racerDetail = racerReg.RacerDetail;
			racerDetail.avatarholder = progressBar2.targetGraphic.GetComponent<Image> ();
		}



//		val = (racerDetail.RacerDistance - A)/((B - A) * (b - a) + a);
//		val = Mathf.Abs( val - factor );

		val = 1 - (racerDetail.RacerDistance / (racerReg.TotalDistance * raceManager.RaceLaps));
      //  Debug.Log(racerReg + " " + racerDetail + " " + val);
		if (val > 0 && val < 1) 
		{
			progressBar2.value = val;
		}

		if (racerDetail != null) 
		{
			if (!racerReg.IsRacerFinished) 
			{
				
				Player_Place.text = racerDetail.RacerStanding + "";
				//Total_Place.text = "/ " + raceManager.RacePlayers;
				Current_Lap.text = racerDetail.RacerLap + "";
				Total_Lap.text = "/ " + raceManager.RaceLaps;
			}
		}
       // Debug.Log("update: " + racerDetail.RacerName + " ---- ");
        if ((racerDetail.RacerWrongWay || racerDetail.RacerOutOfBound) && !racerReg.IsRacerFinished)
		{
			if (racerDetail.RacerWrongWay) {
				WrongWay_Text.text = "Wrong way";
			}
			else if(racerDetail.RacerOutOfBound){
				WrongWay_Text.text = "Out of bounds";
			}
			WrongWay_Text.gameObject.SetActive (true);
		} 
		else 
		{
			WrongWay_Text.gameObject.SetActive (false);
		}
        int t = 1;
      //  Debug.Log(listOfOpponentProgressBars.Count);
		foreach (var p in listOfOpponentProgressBars) 
        {

            if (!p.racerDetail.RacerFinished && p.racerDetail.RacerDestroyed)
                p.slider.targetGraphic.enabled = false;
            else
            {
                p.slider.targetGraphic.enabled = true;
                t++;

             //   Debug.Log(t);
                
            }
                  

			p.slider.value = 1 - (p.racerDetail.RacerDistance / (racerReg.TotalDistance * raceManager.RaceLaps));
          //  Debug.Log("update: " + p.racerDetail.RacerName + " ---- "+ p.slider.gameObject.name);
            //			p.avatar.sprite = racerLists [p.id].avatar;
        }
        Total_Place.text = "/ " + Mathf.Clamp(t,1,raceManager.RacePlayers+1);

        if (progressBar2.targetGraphic.GetComponent<Image>().sprite==null)
        {
            progressBar2.targetGraphic.GetComponent<Image>().sprite = carAvatar;
        }

    }

}
