using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    // Mảng chứa các loại đạn và số lượng tương ứng
    [SerializeField] AmmoSlot[] ammoSlots;

    // Lớp định nghĩa thông tin về 1 loại đạn
    [System.Serializable] // Cho phép hiển thị trong Inspector
    private class AmmoSlot
    {
        public AmmoType ammoType; // Loại đạn (enum)
        public int ammoAmount;    // Số lượng đạn hiện có
    }

    // Lấy số lượng đạn hiện tại của loại chỉ định
    public int GetCurrentAmmo(AmmoType ammoType)
    {
        return GetAmmoSlot(ammoType).ammoAmount;
    }

    // Giảm 1 viên đạn của loại chỉ định (khi bắn)
    public void ReduceCurrentAmmo(AmmoType ammoType)
    {
        GetAmmoSlot(ammoType).ammoAmount--;
    }

    // Tăng đạn cho loại chỉ định (khi nhặt được đạn)
    public void IncreaseCurrentAmmo(AmmoType ammoType, int ammoAmount)
    {
        GetAmmoSlot(ammoType).ammoAmount += ammoAmount;
    }

    // Hàm trợ giúp - tìm slot đạn tương ứng với loại đạn
    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            if (slot.ammoType == ammoType)
            {
                return slot; // Trả về slot nếu tìm thấy
            }
        }
        return null; // Trả về null nếu không tìm thấy
    }
}