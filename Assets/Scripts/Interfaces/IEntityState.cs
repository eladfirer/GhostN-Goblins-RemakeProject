using System;

namespace DefaultNamespace
{
    public interface IEntityState<T>  where T : struct, Enum
    {
        IEntityState<T> OnUpdate(IEntityContext<T> context);
        void EnterState(IEntityContext<T> context);
        void ExitState(IEntityContext<T> context);
    }
}