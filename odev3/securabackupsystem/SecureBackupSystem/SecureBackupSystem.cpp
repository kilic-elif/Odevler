#include <iostream>
#include <string>
#include <vector>
#include <filesystem>
#include <algorithm>
#include "CryptoManager.h"
#include "Logger.h"

namespace fs = std::filesystem;

// Mevcut yedekleri listeleyen ve bir liste (vector) olarak dönen fonksiyon
std::vector<std::string> listBackups() {
    std::vector<std::string> backups;
    std::cout << "\n--- MEVCUT YEDEKLER LISTESI---\n";
    
    int i = 1;
    if (fs::exists("Yedekler")) {
        for (const auto& entry : fs::directory_iterator("Yedekler")) {
            if (entry.path().extension() == ".aes") {
                std::string fileName = entry.path().filename().string();
                backups.push_back(fileName);
                std::cout << i << ". " << fileName << std::endl;
                i++;
            }
        }
    }

    if (backups.empty()) {
        std::cout << "[!] Klasorde hic yedek (.aes) bulunamadi.\n";
    }
    return backups;
}

void showMenu() {
    std::cout << "\n==========================================" << std::endl;
    std::cout << "       SECURE BACKUP SYSTEM v1.0        " << std::endl;
    std::cout << "==========================================" << std::endl;
    std::cout << " 1. Yeni Yedek Olustur (Sifrele)" << std::endl;
    std::cout << " 2. Yedekten Geri Yukle (Coz)" << std::endl;
    std::cout << " 3. Sistem Loglarini Goruntule" << std::endl;
    std::cout << " 4. Cikis" << std::endl;
    std::cout << "------------------------------------------" << std::endl;
    std::cout << "Seciminiz: ";
}

int main() {
    CryptoManager crypto;
    int choice;
    std::string inputPath, outputPath, password;

    if (!fs::exists("Yedekler")) fs::create_directory("Yedekler");

    while (true) {
        showMenu();
        if (!(std::cin >> choice)) {
            std::cin.clear();
            std::cin.ignore(1000, '\n');
            continue;
        }

        if (choice == 4) break;

        switch (choice) {
            case 1: {
                std::cout << "\n[+] Yedeklenecek dosya: ";
                std::cin >> inputPath;
                if (inputPath.find('.') == std::string::npos) inputPath += ".txt";

                std::time_t now = std::time(0);
                std::tm ltm;
                localtime_s(&ltm, &now);
                char ts[20];
                strftime(ts, sizeof(ts), "%Y%m%d_%H%M%S", &ltm);
                outputPath = "Yedekler/backup_" + std::string(ts) + ".aes";

                std::cout << "[+] Sifre: ";
                std::cin >> password;

                if (crypto.encryptFile(inputPath, outputPath, password)) {
                    std::cout << "[>>>] BASARILI: " << outputPath << " olusturuldu.\n";
                    Logger::log("YEDEKLEME: " + inputPath + " -> " + outputPath);
                } else {
                    std::cout << "[!] HATA: Dosya bulunamadi!\n";
                }
                break;
            }

            case 2: {
                std::vector<std::string> backups = listBackups();
                if (backups.empty()) break;

                std::cout << "\n[+] Cozulecek yedek numarasi: ";
                int fileIndex;
                std::cin >> fileIndex;

                if (fileIndex > 0 && fileIndex <= backups.size()) {
                    inputPath = "Yedekler/" + backups[fileIndex - 1];
                } else {
                    std::cout << "[!] HATA: Gecersiz numara!\n";
                    break;
                }

                std::cout << "[+] Geri yuklenecek isim: ";
                std::cin >> outputPath;
                if (outputPath.find('.') == std::string::npos) outputPath += ".txt";

                std::cout << "[+] Sifre: ";
                std::cin >> password;

                if (crypto.decryptFile(inputPath, outputPath, password)) {
                    std::cout << "[>>>] BASARILI: Veri kurtarildi -> " << outputPath << "\n";
                    Logger::log("GERI YUKLEME: " + inputPath + " -> " + outputPath);
                } else {
                    std::cout << "[!] HATA: Yanlis sifre veya bozuk dosya!\n";
                }
                break;
            }

            case 3:
                std::cout << "\n--- SISTEM LOGLARI ---\n";
                system("type backup_system.log");
                break;

            default:
                std::cout << "\n[!] Gecersiz secim!\n";
        }
    }
    return 0;
}