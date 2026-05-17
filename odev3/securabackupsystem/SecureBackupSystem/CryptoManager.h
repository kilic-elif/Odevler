#pragma once
#include <string>

// Sifreleme, sifre cozme ve hash (butunluk) islemlerini yapacak sinif
class CryptoManager {
public:
    // 1. Veri Butunlugu (Data Integrity): Dosyanin bozulup bozulmadigini anlamak icin SHA-256 Hash hesaplar
    std::string calculateHash(const std::string& filePath);

    // 2. Sifreleme (Encryption): Dosyayi alir, bir sifre (key) kullanarak AES-256 ile sifreler
    bool encryptFile(const std::string& inputFile, const std::string& outputFile, const std::string& key);

    // 3. Sifre Cozme (Decryption): Sifrelenmis dosyayi eski haline getirir
    bool decryptFile(const std::string& inputFile, const std::string& outputFile, const std::string& key);
};