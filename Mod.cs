﻿using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace SausageFestModNS
{
    public class SausageFestMod : Mod
    {
        

        public override void Ready()
        {
            Logger.Log("Ready!");

        }
    }

/*
* Had to rename SlaughterHouse into SausageFestSlaugtherhous to make a distinction with the default Stacklands 'Butchery' (which is actually called SlaughterHouse already in the Stacklands implementation...)
*/
	public class SausageFestSlaughterhouse : SlaughterHouse
	{
		//public float SlaughterinTime = 30f;
		public override bool DetermineCanHaveCardsWhenIsRoot => true;
				
		public override bool CanHaveCardsWhileHasStatus()
		{
			return true;
		}

		// this method decides whether a card should stack onto this one
		protected override bool CanHaveCard(CardData otherCard)
		{
			int num = GetChildCount() + (1 + otherCard.GetChildCount());
			if (otherCard.Id == "cow")
			{
				return num <= 5;
			}
			return false;
		}

		// this method is called every frame, it is the CardData equivalent of the Update method
		public override void UpdateCard()
		{
			if (MyGameCard.HasChild && MyGameCard.Child.CardData is Animal)
			{
				MyGameCard.StartTimer(30f, SlaughteringAnimal, SokLoc.Translate("sausagefestmod_action_slaughtering_status"), GetActionId("SlaughteringAnimal"));
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("SlaughteringAnimal"));
			}
			//base.UpdateCard();
		}

		[TimedAction("slaughtering_animal")]
		public void SlaughteringAnimal()
		{
			if (MyGameCard.HasChild && MyGameCard.Child.CardData is Animal)
			{
				GameCard child = MyGameCard.Child;
				RemoveFirstChildFromStack();
				child.DestroyCard();
				// Raw meat
				CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "raw_meat", checkAddToStack: false);
				WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
				// Organ meat
				cardData = WorldManager.instance.CreateCard(base.transform.position, "sausagefestmod_organ_meat", checkAddToStack: false);
				WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir); // required to make organ meat auto-stack if there is a card in the vicinity.
				// Blood
				cardData = WorldManager.instance.CreateCard(base.transform.position, "sausagefestmod_blood", checkAddToStack: false);
				WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir); // required to make blood auto-stack if there is a card in the vicinity.

				WorldManager.instance.CreateSmoke(base.transform.position);
			}
		}
	}


	public class SausageFestMeatGrinder : CardData
	{
		public float MeatGrindingTime = 30f;
		//private int MeatGrindingType = 0; // 0 = regular sausage   1 = blood sausage
		
		// this method decides whether a card should stack onto this one
		protected override bool CanHaveCard(CardData otherCard)
		{
			if (otherCard.Id != "sausagefestmod_organ_meat" && otherCard.Id != "sausagefestmod_blood")
				return false;
			return true;
		}
		
		// this method is called every frame, it is the CardData equivalent of the Update method
		public override void UpdateCard()
		{
			/*
			if (ChildrenMatchingPredicateCount((CardData c) => (c.Id == "sausagefestmod_organ_meat")) >= 1) {
				if (ChildrenMatchingPredicateCount((CardData c) => (c.Id == "sausagefestmod_blood")) >= 1) {
					MeatGrindingType = 1; // blood sausage
					MyGameCard.StartTimer(MeatGrindingTime, CompleteMaking, SokLoc.Translate("sausagefestmod_raw_blood_sausage_status"), GetActionId("CompleteMaking"));
				}
				else {
					MeatGrindingType = 0; // regulare sausage
					MyGameCard.StartTimer(MeatGrindingTime, CompleteMaking, SokLoc.Translate("sausagefestmod_raw_sausage_status"), GetActionId("CompleteMaking"));
				}
			}
			else
			{
				MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
			}
			*/
			base.UpdateCard();
		} 

		public override bool CanHaveCardsWhileHasStatus()
		{
			return true;
		}

		/*
		[TimedAction("complete_making")]
		public void CompleteMaking()
		{
			MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == "sausagefestmod_organ_meat", 1);
			if (MeatGrindingType == 1) { // in case of blood sausage
				MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == "sausagefestmod_blood", 1);
				CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "sausagefestmod_raw_blood_sausage", faceUp: false, checkAddToStack: false);
				WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
			}
			else {
				CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "sausagefestmod_raw_sausage", faceUp: false, checkAddToStack: false);
				WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
			}
		}
		*/
	}
}