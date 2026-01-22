using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class SaveSystem : MonoBehaviour
{
    private string savePath => Application.persistentDataPath + "/save.dat";
    private static readonly string encryptionKey = "PiggyRanchSecretKey123!"; // 128/192/256 bits for AES

    [System.Serializable]
    public class SaveData
    {
        public List<Pig> pigs;
        public int gold;
        public int food;
        public int energy;
        public int dayCount;
    }

    public void Save(List<Pig> pigs, int gold, int food, int energy, int dayCount)
    {
        SaveData data = new SaveData { pigs = pigs, gold = gold, food = food, energy = energy, dayCount = dayCount };
        string json = JsonUtility.ToJson(data);
        string encrypted = Encrypt(json, encryptionKey);
        File.WriteAllText(savePath, encrypted);
    }

    public SaveData Load()
    {
        if (!File.Exists(savePath)) return null;
        string encrypted = File.ReadAllText(savePath);
        string json = Decrypt(encrypted, encryptionKey);
        return JsonUtility.FromJson<SaveData>(json);
    }

    private string Encrypt(string plainText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            aes.Key = keyBytes.Length > 32 ? keyBytes[..32] : keyBytes;
            aes.IV = new byte[16]; // Zero IV for simplicity (improve for production)
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return System.Convert.ToBase64String(encryptedBytes);
        }
    }

    private string Decrypt(string cipherText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            aes.Key = keyBytes.Length > 32 ? keyBytes[..32] : keyBytes;
            aes.IV = new byte[16];
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] cipherBytes = System.Convert.FromBase64String(cipherText);
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}