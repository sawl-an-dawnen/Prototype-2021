using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Platformer.Mechanics
{
    using static Rune;
    public class TurnbasedDialogHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        public BattleSystem bs;
        public RunePanelController rpc;
        public SpellController[] scs = new SpellController[4];
        public SubmitController sc;
        public ResourceHandler resourceHandler;
        public bool isEnabled = true;
        public GameObject TutorialButtonBackground;
        public GameObject TutorialButton;
        private List<Action> spellEffects = new();
        private RuneController[] rcs = new RuneController[6];
        private GameManager.Spell[] spells;
        private SelectableButton currentSelected;
        private SelectableButton rerollButton;
        private SelectableButton submitButton;
        private 
        void Start()
        {
            TutorialButton = transform.parent.Find("TutorialButton").gameObject;
            TutorialButtonBackground = transform.parent.Find("TutorialButtonBackground").gameObject;
            resourceHandler = bs.resourceHandler;
            bs.RegisterStartBattleListener(() => {
                spells = bs.spells;
                for (int i = 0; i < 4; i++)
                {
                    scs[i].SetSpell(spells[i], i, OnSpellButton);
                }
                return "Setting up the battle";
            });
            bs.RegisterPlayerTurnBeginListener(() => {Enable(); return "Enabled player turn";});
            bs.RegisterPlayerTurnBeginListener(() => {
                SetupNewRound();
                return "Setting up new round";
            });
            bs.RegisterPlayerTurnEndListener(() => {Disable(); return "Disabled player turn";});
        }

        // Update is called once per frame
        void Update()
        {
            var keyPresses = GameManager.Instance.inputManager.GetInputs();
            if (keyPresses.Contains(InputManager.InputType.SELECT))
            {
                currentSelected.DoClick();                
            }
            else if (keyPresses.Contains(InputManager.InputType.ONE))
            {
                OnSpellButton(0);
            }
            else if (keyPresses.Contains(InputManager.InputType.TWO))
            {
                OnSpellButton(1);
            }
            else if (keyPresses.Contains(InputManager.InputType.THREE))
            {
                OnSpellButton(2);
            }
            else if (keyPresses.Contains(InputManager.InputType.FOUR))
            {
                OnSpellButton(3);
            }

            if (currentSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(currentSelected.gameObject);
            }
        }
        
        public void Enable()
        {
            isEnabled = true;
            transform.gameObject.SetActive(true);            
            TutorialButton.SetActive(true);
            TutorialButtonBackground.SetActive(true);
            if (rerollButton != null)
            {
                EventSystem.current.SetSelectedGameObject(rerollButton.gameObject);
                rerollButton.OnSelect(null);
            }
        }

        public void Disable()
        {
            isEnabled = false;
            transform.gameObject.SetActive(false);            
            TutorialButton.SetActive(false);
            TutorialButtonBackground.SetActive(false);
        }
        
        public void SetupNewRound()
        {
            if (rerollButton != null)
            {
                EventSystem.current.SetSelectedGameObject(rerollButton.gameObject);
                rerollButton.OnSelect(null);
                if (rerollButton == currentSelected)
                {
                    rerollButton.EnableSelectImage();
                }
            }
            if (submitButton != null)
            {
                submitButton.DisableSelectImage();
            }
            resourceHandler.Initialize(null);
            rpc.DoReset(resourceHandler);
            ColorButtons();
        }

        /* All of these return a string because c# doesn't have function pointers */
        public void SubmitSelected()
        {
			bs.StopPlayerTurnTimer();
			if (sc.enabled)
            {
                foreach (var func in spellEffects)
                {
                    func();
                }
                spellEffects.Clear();
				bs.OnEndTurnButton();
			}
        }
        
        public void RuneSelected()
        {
            var i = currentSelected.buttonType switch {
                ButtonType.RUNE1 => 0,
                ButtonType.RUNE2 => 1,
                ButtonType.RUNE3 => 2,
                ButtonType.RUNE4 => 3,
                ButtonType.RUNE5 => 4,
                ButtonType.RUNE6 => 5,
                _                => throw new Exception("Error: Rune clicked for non-rune button")
            };
            if (resourceHandler.GetRuneTypes()[i] != USED)
            {
                rpc.ToggleRune(i);
            }
        }

        public void RuneRerollSelected()
        {
            resourceHandler.Reroll(rpc.rerolls);
            rpc.DoReroll(resourceHandler);
            ColorButtons();
        }
        
        public void OnResetButton()
        {
            resourceHandler.UncommitRunes();
            rpc.WipeRerolls();
            rpc.ColorRunes(resourceHandler);
            spellEffects.Clear();
            ColorButtons();
        }
        
        public string OnSpellButton(int i)
        {
            if (resourceHandler.CanCastSpell(spells[i]))
            {
                var runesSpent = resourceHandler.CommitRunesForSpell(spells[i]);
                for (int idx = 0; idx < 6; idx++)
                {
                    if (runesSpent[idx])
                    {
                        rpc.rerolls[idx] = false;
                        rpc.runes[idx].SetRuneUnselected();
                    }
                }

                spellEffects.Add(spells[i].eventFunc);
                rpc.ColorRunes(resourceHandler);
                ColorButtons();
                EventSystem.current.SetSelectedGameObject(currentSelected.gameObject);
            }
            return string.Format("Pressed spell {0}!", i+1);
        }
        
        public void ColorButtons()
        {
            for (int i = 0; i < 4; i++)
            {
                if (resourceHandler.CanCastSpell(spells[i]))
                {
                    scs[i].DoEnable();
                }
                else
                {
                    scs[i].DoDisable();
                }
            }
            if (rpc.HasRollsLeft && spellEffects.Count == 0)
            {
                sc.DoDisable();
            }
            else
            {
                sc.DoEnable();
            }
            if (currentSelected != null && !currentSelected.IsInteractable)
            {
                EventSystem.current.SetSelectedGameObject(submitButton.gameObject);
            }
        }
        
        public void RegisterSelectableButton(SelectableButton b)
        {
            switch (b.buttonType) {
                case ButtonType.RUNE1:
                    rcs[0] = b.GetComponentInParent<RuneController>();
                    b.SetAction(RuneSelected);
                    break;
                case ButtonType.RUNE2:
                    rcs[1] = b.GetComponentInParent<RuneController>();
                    b.SetAction(RuneSelected);
                    break;
                case ButtonType.RUNE3:
                    rcs[2] = b.GetComponentInParent<RuneController>();
                    b.SetAction(RuneSelected);
                    break;
                case ButtonType.RUNE4:
                    rcs[3] = b.GetComponentInParent<RuneController>();
                    b.SetAction(RuneSelected);
                    break;
                case ButtonType.RUNE5:
                    rcs[4] = b.GetComponentInParent<RuneController>();
                    b.SetAction(RuneSelected);
                    break;
                case ButtonType.RUNE6:
                    rcs[5] = b.GetComponentInParent<RuneController>();
                    b.SetAction(RuneSelected);
                    break;
                case ButtonType.REROLL:
                    b.SetAction(RuneRerollSelected);
                    rerollButton = b;
                    break;
                case ButtonType.RESET:
                    b.SetAction(OnResetButton);
                    break;
                case ButtonType.SUBMIT:
                    b.SetAction(SubmitSelected);
                    submitButton = b;
                    break;
                case ButtonType.SPELL1:
                    break;
                case ButtonType.SPELL2:
                    break;
                case ButtonType.SPELL3:
                    break;
                case ButtonType.SPELL4:
                    break;
            }
        }
        
        public void SetSelectedButton(SelectableButton b)
        {
            currentSelected = b;
        }
        
        [Serializable]
        public enum ButtonType
        {
            RUNE1,
            RUNE2,
            RUNE3,
            RUNE4,
            RUNE5,
            RUNE6,
            REROLL,
            RESET,
            SUBMIT,
            SPELL1,
            SPELL2,
            SPELL3,
            SPELL4
        }
    }
    
}
