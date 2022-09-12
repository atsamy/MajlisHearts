﻿/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/* 
 * Daily Reward Object UI representation
 */
namespace NiobiumStudios
{
    /** 
     * The UI Representation of a Daily Reward.
     * 
     *  There are 3 states:
     *  
     *  1. Unclaimed and available:
     *  - Shows the Color Claimed
     *  
     *  2. Unclaimed and Unavailable
     *  - Shows the Color Default
     *  
     *  3. Claimed
     *  - Shows the Color Claimed
     *  
     **/
    public class DailyRewardUI : MonoBehaviour
    {
        public bool showRewardName;

        [Header("UI Elements")]
        public TextMeshProUGUI textDay;                // Text containing the Day text eg. Day 12
        public TextMeshProUGUI textReward;             // The Text containing the Reward amount
        public Image imageRewardBackground; // The Reward Image Background
        public Image imageReward;           // The Reward Image
        public Sprite claimedSprite;
        public Sprite readySprite;
        public Color colorClaim;            // The Color of the background when claimed
        private Color colorUnclaimed;       // The Color of the background when not claimed

        public GameObject Data;

        [Header("Internal")]
        public int day;

        [HideInInspector]
        public Reward reward;

        public DailyRewardState state;

        // The States a reward can have
        public enum DailyRewardState
        {
            UNCLAIMED_AVAILABLE,
            UNCLAIMED_UNAVAILABLE,
            CLAIMED
        }

        void Awake()
        {
            colorUnclaimed = imageReward.color;
        }

        public void Initialize()
        {
            textDay.text = string.Format(LanguageManager.Instance.GetString("day"), day.ToString());
            if (reward.reward > 0)
            {
                if (showRewardName)
                {
                    textReward.text = reward.reward + " " + reward.unit;
                }
                else
                {
                    textReward.text = reward.reward.ToString();
                }
            }
            else
            {
                textReward.text = reward.unit.ToString();
            }
            imageReward.sprite = reward.sprite;
        }

        // Refreshes the UI
        public void Refresh()
        {
            switch (state)
            {
                case DailyRewardState.UNCLAIMED_AVAILABLE:
                    imageRewardBackground.sprite = readySprite;
                    break;
                case DailyRewardState.UNCLAIMED_UNAVAILABLE:
                    imageRewardBackground.color = colorUnclaimed;
                    break;
                case DailyRewardState.CLAIMED:
                    imageRewardBackground.sprite = claimedSprite;
                    Data.SetActive(false);
                    break;
            }
        }
    }
}