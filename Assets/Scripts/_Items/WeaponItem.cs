using UnityEngine;

namespace _Items {
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item {
        public GameObject modelPrefab;
        public bool isUnarmed;
        [Header("Damage")]
        public int baseDamage = 25;
        public int criticalDamageMultiplier = 5;

        [Header("Absorption")]
        public float physicalDamageAbsorption = 10f; 
        
        [Header("Idle Animations")]
        public string rightHandIdle01;
        public string leftHandIdle01;
        public string twoHandIdle;

        [Header("Attack Animations")]
        public string lightAttack01;
        public string lightAttack02;
        public string lightAttack03;

        [Header("Weapon Art -- Special Abilities")]
        public string weaponArt;

        [Header("Stamina Costs")]
        public int baseStaminaCost;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;

        [Header("Weapon Type")]
        public bool isMeleeWeapon;
        public bool isShieldWeapon;
    }
}