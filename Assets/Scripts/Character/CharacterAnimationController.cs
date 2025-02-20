using DefaultNamespace;
using UnityEngine;

namespace Character
{
    public class CharacterAnimationController : MonoBehaviour, IAnimationController<CharacterForm>
    {
        
        [SerializeField] private Animator animator; 
        [SerializeField] private AnimatorOverrideController noArmorOverrideController; 
        [SerializeField] private RuntimeAnimatorController armorAnimatorController;
        [SerializeField] private AnimatorOverrideController frogOverrideController;
        public void PlayAnimationTrigger(CharacterForm form, string trigger)
        {
            CheckForm(form);
            animator.SetTrigger(trigger);
        }
        
        public void PlayAnimationBool(CharacterForm form, string parameter, bool setValue)
        {
            CheckForm(form);
            animator.SetBool(parameter,setValue);
        }
        

        

        private void CheckForm(CharacterForm form)
        {
            switch (form)
            {
                case CharacterForm.Armor:
                    animator.runtimeAnimatorController = armorAnimatorController;
                    break;

                case CharacterForm.NoArmor:
                    animator.runtimeAnimatorController = noArmorOverrideController;
                    break;

                case CharacterForm.Frog:
                    animator.runtimeAnimatorController = frogOverrideController;
                    break;
            }
        }

        public void ResetTrigger(string triggerName)
        {
            animator.ResetTrigger(triggerName);
        }

        public void ResetAllAnimations()
        {
            animator.enabled = false;
        }


        public void SetFloat(string floatParameter, float setValue)
        {
            animator.SetFloat(floatParameter, setValue);
        }
    }
    
    
}