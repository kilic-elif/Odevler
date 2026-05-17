#include "FileHandler.h"
#include <filesystem>
#include <iostream>

namespace fs = std::filesystem;

bool FileHandler::copyFile(const std::string& source, const std::string& destination) {
    try {
        fs::copy(source, destination, fs::copy_options::overwrite_existing);
        return true;
    }
    catch (const fs::filesystem_error& e) {
        std::cerr << "[-] Kopyalama Hatasi: " << e.what() << '\n';
        return false;
    }
}

bool FileHandler::createDirectory(const std::string& path) {
    try {
        if (!fs::exists(path)) {
            return fs::create_directories(path);
        }
        return true;
    }
    catch (const fs::filesystem_error& e) {
        std::cerr << "[-] Klasor Olusturma Hatasi: " << e.what() << '\n';
        return false;
    }
}

uintmax_t FileHandler::getFileSize(const std::string& path) {
    try {
        if (fs::exists(path)) {
            return fs::file_size(path);
        }
        return 0;
    }
    catch (const fs::filesystem_error& e) {
        return 0;
    }
}