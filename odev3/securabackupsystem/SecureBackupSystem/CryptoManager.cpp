#include "CryptoManager.h"
#include <iostream>
#include <fstream>
#include <iomanip>
#include <sstream>

#include <openssl/evp.h>
#include <openssl/sha.h>


std::string CryptoManager::calculateHash(const std::string& filePath) {
    std::ifstream file(filePath, std::ios::binary);
    if (!file.is_open()) return "";

    EVP_MD_CTX* mdctx = EVP_MD_CTX_new();
    const EVP_MD* md = EVP_sha256();
    EVP_DigestInit_ex(mdctx, md, NULL);

    const int bufferSize = 4096;
    char buffer[bufferSize];

    while (file.read(buffer, bufferSize)) {
        EVP_DigestUpdate(mdctx, buffer, file.gcount());
    }
    EVP_DigestUpdate(mdctx, buffer, file.gcount());

    unsigned char md_value[EVP_MAX_MD_SIZE];
    unsigned int md_len;
    EVP_DigestFinal_ex(mdctx, md_value, &md_len);
    EVP_MD_CTX_free(mdctx);

    std::stringstream ss;
    for (unsigned int i = 0; i < md_len; i++) {
        ss << std::hex << std::setw(2) << std::setfill('0') << (int)md_value[i];
    }
    return ss.str();
}

bool CryptoManager::encryptFile(const std::string& inputFile, const std::string& outputFile, const std::string& key) {
    std::ifstream inFile(inputFile, std::ios::binary);
    std::ofstream outFile(outputFile, std::ios::binary);
    if (!inFile.is_open() || !outFile.is_open()) return false;

    unsigned char encryptionKey[32], iv[16];
    PKCS5_PBKDF2_HMAC_SHA1(key.c_str(), key.length(), NULL, 0, 1000, 32, encryptionKey);
    for (int i = 0; i < 16; i++) iv[i] = encryptionKey[i];

    EVP_CIPHER_CTX* ctx = EVP_CIPHER_CTX_new();
    EVP_EncryptInit_ex(ctx, EVP_aes_256_cbc(), NULL, encryptionKey, iv);

    const int bufferSize = 4096;
    unsigned char inBuffer[bufferSize];
    unsigned char outBuffer[bufferSize + EVP_MAX_BLOCK_LENGTH];
    int outLen;

    while (inFile.read(reinterpret_cast<char*>(inBuffer), bufferSize)) {
        EVP_EncryptUpdate(ctx, outBuffer, &outLen, inBuffer, inFile.gcount());
        outFile.write(reinterpret_cast<char*>(outBuffer), outLen);
    }
    EVP_EncryptUpdate(ctx, outBuffer, &outLen, inBuffer, inFile.gcount());
    outFile.write(reinterpret_cast<char*>(outBuffer), outLen);

    EVP_EncryptFinal_ex(ctx, outBuffer, &outLen);
    outFile.write(reinterpret_cast<char*>(outBuffer), outLen);

    EVP_CIPHER_CTX_free(ctx);
    return true;
}

bool CryptoManager::decryptFile(const std::string& inputFile, const std::string& outputFile, const std::string& key) {
    std::ifstream inFile(inputFile, std::ios::binary);
    std::ofstream outFile(outputFile, std::ios::binary);
    if (!inFile.is_open() || !outFile.is_open()) return false;

    unsigned char decryptionKey[32], iv[16];
    PKCS5_PBKDF2_HMAC_SHA1(key.c_str(), key.length(), NULL, 0, 1000, 32, decryptionKey);
    for (int i = 0; i < 16; i++) iv[i] = decryptionKey[i];

    EVP_CIPHER_CTX* ctx = EVP_CIPHER_CTX_new();
    EVP_DecryptInit_ex(ctx, EVP_aes_256_cbc(), NULL, decryptionKey, iv);

    const int bufferSize = 4096;
    unsigned char inBuffer[bufferSize];
    unsigned char outBuffer[bufferSize + EVP_MAX_BLOCK_LENGTH];
    int outLen;

    while (inFile.read(reinterpret_cast<char*>(inBuffer), bufferSize)) {
        EVP_DecryptUpdate(ctx, outBuffer, &outLen, inBuffer, inFile.gcount());
        outFile.write(reinterpret_cast<char*>(outBuffer), outLen);
    }
    EVP_DecryptUpdate(ctx, outBuffer, &outLen, inBuffer, inFile.gcount());
    outFile.write(reinterpret_cast<char*>(outBuffer), outLen);

    if (EVP_DecryptFinal_ex(ctx, outBuffer, &outLen) <= 0) {
        EVP_CIPHER_CTX_free(ctx);
        return false;
    }
    outFile.write(reinterpret_cast<char*>(outBuffer), outLen);

    EVP_CIPHER_CTX_free(ctx);
    return true;
}