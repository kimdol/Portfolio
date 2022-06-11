using ARPG.InventorySystem.Inventory;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataHander : MonoBehaviour
{
    private DatabaseReference databaseRef;
    private string UserDataPath => "users";
    private string StatsDataPath => "stats";
    private string EquipmentDataPath => "equipment";
    private string InventoryDataPath => "inventory";

    public StatsObject playerStats;
    public InventoryObject playerEquipment;
    public InventoryObject playerInventory;

    void Start()
    {
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void OnClickSave()
    {
        var userId = FirebaseAuthManager.Instance.UserId;
        if (userId == string.Empty)
        {
            return;
        }

        string statsJson = playerStats.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).SetRawJsonValueAsync(statsJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("사용자의 데이터 저장을 취소했습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("사용자의 데이터 저장 중 오류가 발생했습니다: " + task.Exception);
                return;
            }

            Debug.LogFormat("사용자의 데이터 저장 성공: {0} ({1})", userId, statsJson);
        });

        string equipmentJson = playerEquipment.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(EquipmentDataPath).SetRawJsonValueAsync(equipmentJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Equipment 데이터 저장이 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Equipment 데이터 저장 중 오류가 발생했습니다: " + task.Exception);
                return;
            }

            Debug.LogFormat("Equipment 데이터 저장 성공: {0} ({1})", userId, equipmentJson);

        });

        string inventoryJson = playerInventory.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(InventoryDataPath).SetRawJsonValueAsync(inventoryJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Inventory 데이터 저장이 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Inventory 데이터 저장 중 오류가 발생했습니다: " + task.Exception);
                return;
            }

            Debug.LogFormat("Inventory 데이터 저장 성공: {0} ({1})", userId, inventoryJson);

        });
    }

    public void OnClickLoad()
    {
        var userId = FirebaseAuthManager.Instance.UserId;
        if (userId == string.Empty)
        {
            return;
        }

        databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("사용자 데이터의 로드가 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("사용자 데이터의 로드 중 오류가 발생했습니다: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            playerStats.FromJson(snapshot.GetRawJsonValue());
            Debug.LogFormat("사용자 데이터의 로드 성공: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });

        databaseRef.Child(UserDataPath).Child(userId).Child(EquipmentDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("사용자 데이터의 로드가 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("사용자 데이터의 로드 중 오류가 발생했습니다: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            playerEquipment.FromJson(snapshot.GetRawJsonValue());
            Debug.LogFormat("사용자 데이터의 로드 성공: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });

        databaseRef.Child(UserDataPath).Child(userId).Child(InventoryDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("사용자 데이터의 로드가 취소되었습니다.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("사용자 데이터의 로드 중 오류가 발생했습니다: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            Debug.LogFormat("사용자 데이터의 로드 성공: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });
    }
}
