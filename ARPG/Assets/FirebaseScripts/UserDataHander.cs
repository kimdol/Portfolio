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
                Debug.LogError("������� ������ ������ ����߽��ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("������� ������ ���� �� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            Debug.LogFormat("������� ������ ���� ����: {0} ({1})", userId, statsJson);
        });

        string equipmentJson = playerEquipment.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(EquipmentDataPath).SetRawJsonValueAsync(equipmentJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Equipment ������ ������ ��ҵǾ����ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Equipment ������ ���� �� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            Debug.LogFormat("Equipment ������ ���� ����: {0} ({1})", userId, equipmentJson);

        });

        string inventoryJson = playerInventory.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(InventoryDataPath).SetRawJsonValueAsync(inventoryJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Inventory ������ ������ ��ҵǾ����ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Inventory ������ ���� �� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            Debug.LogFormat("Inventory ������ ���� ����: {0} ({1})", userId, inventoryJson);

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
                Debug.LogError("����� �������� �ε尡 ��ҵǾ����ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("����� �������� �ε� �� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            playerStats.FromJson(snapshot.GetRawJsonValue());
            Debug.LogFormat("����� �������� �ε� ����: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });

        databaseRef.Child(UserDataPath).Child(userId).Child(EquipmentDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("����� �������� �ε尡 ��ҵǾ����ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("����� �������� �ε� �� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            playerEquipment.FromJson(snapshot.GetRawJsonValue());
            Debug.LogFormat("����� �������� �ε� ����: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });

        databaseRef.Child(UserDataPath).Child(userId).Child(InventoryDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("����� �������� �ε尡 ��ҵǾ����ϴ�.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("����� �������� �ε� �� ������ �߻��߽��ϴ�: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            Debug.LogFormat("����� �������� �ε� ����: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });
    }
}
