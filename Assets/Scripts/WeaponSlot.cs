using _Items;
using UnityEngine;

public class WeaponSlot : MonoBehaviour {
    public Transform parentOverride;
    public WeaponItem currentWeapon;

    public bool isLeftHandSlot;
    public bool isRightHandSlot;
    public bool isLeftFootSlot;
    public bool isRightFootSlot;

    public GameObject currentWeaponModel;

    public void UnloadWeapon()
    {
        if ( currentWeaponModel != null )
        {
            currentWeaponModel.SetActive(false);
        }
    }

    public void UnloadWeaponAndDestroy()
    {
        if ( currentWeaponModel != null )
        {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeaponModel(WeaponItem weaponItem)
    {
        UnloadWeaponAndDestroy();

        if ( weaponItem == null )
        {
            UnloadWeapon();
            return;
        }

        GameObject model = Instantiate(weaponItem.modelPrefab) as GameObject;
        if ( model != null )
        {
            if ( parentOverride != null )
            {
                model.transform.parent = parentOverride;
            }
            else
            {
                model.transform.parent = transform;
            }

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }

        currentWeaponModel = model;
    }
}