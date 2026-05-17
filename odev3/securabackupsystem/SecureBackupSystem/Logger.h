#ifndef LOGGER_H
#define LOGGER_H

#include <iostream>
#include <fstream>
#include <string>
#include <ctime>

class Logger {
public:
    static void log(const std::string& message) {
        std::ofstream logFile("backup_system.log", std::ios_base::app);
        if (logFile.is_open()) {
            std::time_t now = std::time(0);
            char dt[26];
            ctime_s(dt, sizeof(dt), &now);
            std::string timeStr(dt);
            timeStr.pop_back(); // Satır sonundaki boşluğu siler

            logFile << "[" << timeStr << "] " << message << std::endl;
            logFile.close();
        }
    }
};

#endif