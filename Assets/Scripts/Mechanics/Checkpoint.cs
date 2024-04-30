using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Platformer.Mechanics
{
    [Serializable]
    public class Checkpoint
    {
        public int playerHealth;
        public int playerAttack;
        public int playerDefense;
        public SpawnsDict spawns = new();
        public PlayDoorSoundDict playDoorSound = new();
        public PlayerPosDict playerPos = new();
        public AvailableSpellsDict spells = new();
        public string SceneName;
        public float coins;
        public bool CanOpen;
        [SerializeField] public List<string> items;
		public Color playerColor;
        bool bigDragon;
        bool smallDragon;
		public bool clockPuzzleSolved;
		public bool counterPuzzleSolved;
		public bool slidingPuzzleSolved;
        private bool dragonPuzzleSolved;

        public Checkpoint(
            int playerHealth,
            int playerAttack,
            int playerDefense,
            SpawnsDict Spawns,
            PlayDoorSoundDict PlayDoorSound,
            PlayerPosDict PlayerPos,
            AvailableSpellsDict spells,
            string SceneName,
            float coins,
            bool CanOpen,
            List<string> items,
            Color playerColor,
            bool bigDragon,
            bool smallDragon,
			bool clockPuzzleSolved,
	        bool counterPuzzleSolved,
	        bool slidingPuzzleSolved,
            bool dragonPuzzleSolved
            )
        {
            //this.playerHealth = Math.Min(100, playerHealth + 30);        
            this.playerHealth = playerHealth;
            this.playerAttack = playerAttack;
            this.playerDefense = playerDefense;
            spawns = Spawns.GetCopy();
            playDoorSound = PlayDoorSound.GetCopy();
            playerPos = PlayerPos.GetCopy();
            this.spells = spells.GetCopy();
            this.SceneName = SceneName;
            this.coins = coins;
            this.CanOpen = CanOpen;
            this.items = items;
            this.playerColor = playerColor;
            this.bigDragon = bigDragon;
            this.smallDragon = smallDragon;
			this.clockPuzzleSolved = clockPuzzleSolved;
			this.counterPuzzleSolved = counterPuzzleSolved;
			this.slidingPuzzleSolved = slidingPuzzleSolved;
            this.dragonPuzzleSolved = dragonPuzzleSolved;
        }

        [Serializable]
        public class SceneSpawnsDict : SerializedDictionary<string, int>
        {
            public SceneSpawnsDict GetCopy()
            {
                var ret = new SceneSpawnsDict();
                foreach (var (key, val) in this)
                {
                    ret[key] = val;
                }
                return ret;
            }
        }
        [Serializable]
        public class SpawnsDict : SerializedDictionary<string, SceneSpawnsDict>
        {
            public SpawnsDict GetCopy()
            {
                var ret = new SpawnsDict();
                foreach (var (key, val) in this)
                {
                    ret[key] = val.GetCopy();
                }
                return ret;
            }
        }
        [Serializable]
        public class PlayDoorSoundDict : SerializedDictionary<string, bool>
        {
            public PlayDoorSoundDict GetCopy()
            {
                var ret = new PlayDoorSoundDict();
                foreach (var (key, val) in this)
                {
                    ret[key] = val;
                }
                return ret;
            }
        }

        [Serializable]
        public class PlayerPosDict : SerializedDictionary<string, Vector3>
        {
            public PlayerPosDict GetCopy()
            {
                var ret = new PlayerPosDict();
                foreach (var (key, val) in this)
                {
                    ret[key] = val;
                }
                return ret;
            }
        }
        [Serializable]
        public class AvailableSpellsDict : SerializedDictionary<string, bool>
        {
            public AvailableSpellsDict GetCopy()
            {
                var ret = new AvailableSpellsDict();
                foreach (var (key, val) in this)
                {
                    ret[key] = val;
                }
                return ret;
            }
        }
    }
}