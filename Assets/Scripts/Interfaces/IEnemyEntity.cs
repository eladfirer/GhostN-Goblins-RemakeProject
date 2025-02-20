using UnityEngine;

namespace Interfaces
{
    public interface IEnemyEntity
    {
        public bool IsActive { get; }
        public void HitByWeapon();
    }
}