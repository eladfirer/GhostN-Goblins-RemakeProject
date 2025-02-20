using UnityEngine;

namespace Interfaces
{
    public interface IWeapon
    {
        public bool Attack(Vector3 position, int direction);
    }
}