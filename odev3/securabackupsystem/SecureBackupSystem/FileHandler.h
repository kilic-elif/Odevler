#pragma once
#include <string>

// Dosya islemlerini yonetecek sinifimiz (Class)
class FileHandler {
public:
    // Dosyayi kaynak (source) konumundan hedef (destination) konumuna kopyalar
    bool copyFile(const std::string& source, const std::string& destination);

    // Eger yedekleme klasoru yoksa, isletim sistemine yeni klasor actirir
    bool createDirectory(const std::string& path);

    // Dosyanin boyutunu byte cinsinden dondurur (Ileride loglama yaparken lazim olacak)
    uintmax_t getFileSize(const std::string& path);
};