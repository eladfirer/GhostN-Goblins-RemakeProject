using System;

namespace DefaultNamespace
{
    public interface IAnimationController<T>  where T : struct, Enum
    {
        public void PlayAnimationTrigger(T form, string trigger);
        public void PlayAnimationBool(T form, string parameter, bool setValue);
        public void ResetTrigger(string triggerName);
        public void ResetAllAnimations();
    }
}