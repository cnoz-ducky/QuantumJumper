#include <Windows.h>
#include <iostream>
#include <SDK.hpp>
#include <sstream>
#include <parser.cpp>
#include <fstream> // Add this include

std::string g_commandString = "";
std::ofstream logFile; // Add this global

// Add this macro for easy logging
#define DEBUG_PRINT(msg) \
    do { \
        std::cout << msg << std::endl; \
        logFile << msg << std::endl; \
        logFile.flush(); \
    } while(0)
// Global variable for the original version.dll handle
HMODULE hOriginalDLL = nullptr;

// Function pointers for the original version.dll functions
typedef DWORD(WINAPI* GetFileVersionInfoSizeAFunc)(LPCSTR, LPDWORD);
typedef DWORD(WINAPI* GetFileVersionInfoSizeBFunc)(LPCWSTR, LPDWORD);
typedef BOOL(WINAPI* GetFileVersionInfoAFunc)(LPCSTR, DWORD, DWORD, LPVOID);
typedef BOOL(WINAPI* GetFileVersionInfoWFunc)(LPCWSTR, DWORD, DWORD, LPVOID);
typedef BOOL(WINAPI* VerQueryValueAFunc)(LPCVOID, LPCSTR, LPVOID*, PUINT);
typedef BOOL(WINAPI* VerQueryValueWFunc)(LPCVOID, LPCWSTR, LPVOID*, PUINT);
typedef DWORD(WINAPI* VerLanguageNameAFunc)(DWORD, LPSTR, DWORD);
typedef DWORD(WINAPI* VerLanguageNameWFunc)(DWORD, LPWSTR, DWORD);
typedef DWORD(WINAPI* VerFindFileAFunc)(DWORD, LPCSTR, LPCSTR, LPCSTR, LPSTR, PUINT, LPSTR, PUINT);
typedef DWORD(WINAPI* VerFindFileWFunc)(DWORD, LPCWSTR, LPCWSTR, LPCWSTR, LPWSTR, PUINT, LPWSTR, PUINT);
typedef DWORD(WINAPI* VerInstallFileAFunc)(DWORD, LPCSTR, LPCSTR, LPCSTR, LPCSTR, LPCSTR, LPSTR, PUINT);
typedef DWORD(WINAPI* VerInstallFileWFunc)(DWORD, LPCWSTR, LPCWSTR, LPCWSTR, LPCWSTR, LPCWSTR, LPWSTR, PUINT);
typedef DWORD(WINAPI* GetFileVersionInfoSizeExAFunc)(DWORD, LPCSTR, LPDWORD);
typedef DWORD(WINAPI* GetFileVersionInfoSizeExWFunc)(DWORD, LPCWSTR, LPDWORD);
typedef BOOL(WINAPI* GetFileVersionInfoExAFunc)(DWORD, LPCSTR, DWORD, DWORD, LPVOID);
typedef BOOL(WINAPI* GetFileVersionInfoExWFunc)(DWORD, LPCWSTR, DWORD, DWORD, LPVOID);

// Function pointers to original functions
GetFileVersionInfoSizeAFunc OriginalGetFileVersionInfoSizeA = nullptr;
GetFileVersionInfoSizeBFunc OriginalGetFileVersionInfoSizeW = nullptr;
GetFileVersionInfoAFunc OriginalGetFileVersionInfoA = nullptr;
GetFileVersionInfoWFunc OriginalGetFileVersionInfoW = nullptr;
VerQueryValueAFunc OriginalVerQueryValueA = nullptr;
VerQueryValueWFunc OriginalVerQueryValueW = nullptr;
VerLanguageNameAFunc OriginalVerLanguageNameA = nullptr;
VerLanguageNameWFunc OriginalVerLanguageNameW = nullptr;
VerFindFileAFunc OriginalVerFindFileA = nullptr;
VerFindFileWFunc OriginalVerFindFileW = nullptr;
VerInstallFileAFunc OriginalVerInstallFileA = nullptr;
VerInstallFileWFunc OriginalVerInstallFileW = nullptr;
GetFileVersionInfoSizeExAFunc OriginalGetFileVersionInfoSizeExA = nullptr;
GetFileVersionInfoSizeExWFunc OriginalGetFileVersionInfoSizeExW = nullptr;
GetFileVersionInfoExAFunc OriginalGetFileVersionInfoExA = nullptr;
GetFileVersionInfoExWFunc OriginalGetFileVersionInfoExW = nullptr;

// Load original version.dll and get function addresses
void LoadOriginalDLL() {
    if (hOriginalDLL) return;

    // Load from System32 to avoid recursion
    wchar_t systemPath[MAX_PATH];
    GetSystemDirectoryW(systemPath, MAX_PATH);
    wcscat_s(systemPath, L"\\version.dll");

    hOriginalDLL = LoadLibraryW(systemPath);
    if (!hOriginalDLL) {
        return;
    }

    // Get all function addresses
    OriginalGetFileVersionInfoSizeA = (GetFileVersionInfoSizeAFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoSizeA");
    OriginalGetFileVersionInfoSizeW = (GetFileVersionInfoSizeBFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoSizeW");
    OriginalGetFileVersionInfoA = (GetFileVersionInfoAFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoA");
    OriginalGetFileVersionInfoW = (GetFileVersionInfoWFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoW");
    OriginalVerQueryValueA = (VerQueryValueAFunc)GetProcAddress(hOriginalDLL, "VerQueryValueA");
    OriginalVerQueryValueW = (VerQueryValueWFunc)GetProcAddress(hOriginalDLL, "VerQueryValueW");
    OriginalVerLanguageNameA = (VerLanguageNameAFunc)GetProcAddress(hOriginalDLL, "VerLanguageNameA");
    OriginalVerLanguageNameW = (VerLanguageNameWFunc)GetProcAddress(hOriginalDLL, "VerLanguageNameW");
    OriginalVerFindFileA = (VerFindFileAFunc)GetProcAddress(hOriginalDLL, "VerFindFileA");
    OriginalVerFindFileW = (VerFindFileWFunc)GetProcAddress(hOriginalDLL, "VerFindFileW");
    OriginalVerInstallFileA = (VerInstallFileAFunc)GetProcAddress(hOriginalDLL, "VerInstallFileA");
    OriginalVerInstallFileW = (VerInstallFileWFunc)GetProcAddress(hOriginalDLL, "VerInstallFileW");
    OriginalGetFileVersionInfoSizeExA = (GetFileVersionInfoSizeExAFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoSizeExA");
    OriginalGetFileVersionInfoSizeExW = (GetFileVersionInfoSizeExWFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoSizeExW");
    OriginalGetFileVersionInfoExA = (GetFileVersionInfoExAFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoExA");
    OriginalGetFileVersionInfoExW = (GetFileVersionInfoExWFunc)GetProcAddress(hOriginalDLL, "GetFileVersionInfoExW");
}

// Exported proxy functions - these avoid conflicts by not being declared in headers
extern "C" {
    __declspec(dllexport) DWORD WINAPI GetFileVersionInfoSizeA_Proxy(LPCSTR lptstrFilename, LPDWORD lpdwHandle) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoSizeA) {
            return OriginalGetFileVersionInfoSizeA(lptstrFilename, lpdwHandle);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI GetFileVersionInfoSizeW_Proxy(LPCWSTR lptstrFilename, LPDWORD lpdwHandle) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoSizeW) {
            return OriginalGetFileVersionInfoSizeW(lptstrFilename, lpdwHandle);
        }
        return 0;
    }

    __declspec(dllexport) BOOL WINAPI GetFileVersionInfoA_Proxy(LPCSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoA) {
            return OriginalGetFileVersionInfoA(lptstrFilename, dwHandle, dwLen, lpData);
        }
        return FALSE;
    }

    __declspec(dllexport) BOOL WINAPI GetFileVersionInfoW_Proxy(LPCWSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoW) {
            return OriginalGetFileVersionInfoW(lptstrFilename, dwHandle, dwLen, lpData);
        }
        return FALSE;
    }

    __declspec(dllexport) BOOL WINAPI VerQueryValueA_Proxy(LPCVOID pBlock, LPCSTR lpSubBlock, LPVOID* lplpBuffer, PUINT puLen) {
        LoadOriginalDLL();
        if (OriginalVerQueryValueA) {
            return OriginalVerQueryValueA(pBlock, lpSubBlock, lplpBuffer, puLen);
        }
        return FALSE;
    }

    __declspec(dllexport) BOOL WINAPI VerQueryValueW_Proxy(LPCVOID pBlock, LPCWSTR lpSubBlock, LPVOID* lplpBuffer, PUINT puLen) {
        LoadOriginalDLL();
        if (OriginalVerQueryValueW) {
            return OriginalVerQueryValueW(pBlock, lpSubBlock, lplpBuffer, puLen);
        }
        return FALSE;
    }

    __declspec(dllexport) DWORD WINAPI VerLanguageNameA_Proxy(DWORD wLang, LPSTR szLang, DWORD nSize) {
        LoadOriginalDLL();
        if (OriginalVerLanguageNameA) {
            return OriginalVerLanguageNameA(wLang, szLang, nSize);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI VerLanguageNameW_Proxy(DWORD wLang, LPWSTR szLang, DWORD nSize) {
        LoadOriginalDLL();
        if (OriginalVerLanguageNameW) {
            return OriginalVerLanguageNameW(wLang, szLang, nSize);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI VerFindFileA_Proxy(DWORD uFlags, LPCSTR szFileName, LPCSTR szWinDir, LPCSTR szAppDir, LPSTR szCurDir, PUINT lpuCurDirLen, LPSTR szDestDir, PUINT lpuDestDirLen) {
        LoadOriginalDLL();
        if (OriginalVerFindFileA) {
            return OriginalVerFindFileA(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI VerFindFileW_Proxy(DWORD uFlags, LPCWSTR szFileName, LPCWSTR szWinDir, LPCWSTR szAppDir, LPWSTR szCurDir, PUINT lpuCurDirLen, LPWSTR szDestDir, PUINT lpuDestDirLen) {
        LoadOriginalDLL();
        if (OriginalVerFindFileW) {
            return OriginalVerFindFileW(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI VerInstallFileA_Proxy(DWORD uFlags, LPCSTR szSrcFileName, LPCSTR szDestFileName, LPCSTR szSrcDir, LPCSTR szDestDir, LPCSTR szCurDir, LPSTR szTmpFile, PUINT lpuTmpFileLen) {
        LoadOriginalDLL();
        if (OriginalVerInstallFileA) {
            return OriginalVerInstallFileA(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI VerInstallFileW_Proxy(DWORD uFlags, LPCWSTR szSrcFileName, LPCWSTR szDestFileName, LPCWSTR szSrcDir, LPCWSTR szDestDir, LPCWSTR szCurDir, LPWSTR szTmpFile, PUINT lpuTmpFileLen) {
        LoadOriginalDLL();
        if (OriginalVerInstallFileW) {
            return OriginalVerInstallFileW(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI GetFileVersionInfoSizeExA_Proxy(DWORD dwFlags, LPCSTR lpwstrFilename, LPDWORD lpdwHandle) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoSizeExA) {
            return OriginalGetFileVersionInfoSizeExA(dwFlags, lpwstrFilename, lpdwHandle);
        }
        return 0;
    }

    __declspec(dllexport) DWORD WINAPI GetFileVersionInfoSizeExW_Proxy(DWORD dwFlags, LPCWSTR lpwstrFilename, LPDWORD lpdwHandle) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoSizeExW) {
            return OriginalGetFileVersionInfoSizeExW(dwFlags, lpwstrFilename, lpdwHandle);
        }
        return 0;
    }

    __declspec(dllexport) BOOL WINAPI GetFileVersionInfoExA_Proxy(DWORD dwFlags, LPCSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoExA) {
            return OriginalGetFileVersionInfoExA(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
        }
        return FALSE;
    }

    __declspec(dllexport) BOOL WINAPI GetFileVersionInfoExW_Proxy(DWORD dwFlags, LPCWSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData) {
        LoadOriginalDLL();
        if (OriginalGetFileVersionInfoExW) {
            return OriginalGetFileVersionInfoExW(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
        }
        return FALSE;
    }
}
DWORD MonitorGameState(LPVOID LPmod);

HWND FindEACWindow() {
    return FindWindow(nullptr, L"Error");
}
bool eacterminated = false;

void SuppressEACError() {
    HWND eacWindow = FindEACWindow();
    if (eacWindow) {
        ShowWindow(eacWindow, SW_HIDE);
        // Or send WM_CLOSE message
        SendMessage(eacWindow, WM_CLOSE, 0, 0);
        eacterminated = true;
    }
    DEBUG_PRINT("Looking for EAC window");
}
DWORD MainThread(HMODULE Module)
{
    /* Code to open a console window */
    AllocConsole();
    FILE* Dummy;
    freopen_s(&Dummy, "CONOUT$", "w", stdout);
    freopen_s(&Dummy, "CONIN$", "r", stdin);

    // Open log file
    logFile.open("quantum_debug.txt", std::ios::app);
    DEBUG_PRINT("=== DLL Injection Started ===");

    SDK::UEngine* Engine = SDK::UEngine::GetEngine();
    SDK::UWorld* World = SDK::UWorld::GetWorld();
    CreateThread(0, 0, [](LPVOID) -> DWORD {
        while (!eacterminated) {
            Sleep(1000);
            SuppressEACError();
        }
        return 0;
    }, nullptr, 0, nullptr);
    // loop while game boots up.
    while (!World || !World->OwningGameInstance ||
        World->OwningGameInstance->LocalPlayers.Num() == 0 ||
        !World->OwningGameInstance->LocalPlayers[0]->PlayerController) {
        DEBUG_PRINT("Game not ready yet, retrying...");
        Sleep(1000);
        World = SDK::UWorld::GetWorld();
        Engine = SDK::UEngine::GetEngine();
    }
    std::unordered_map<std::string, ConfigData> buff = LoadConfigFile("config.txt");
    DEBUG_PRINT("" << "=== CONFIG DEBUG ===" << std::endl);
    for (auto& pair : buff) {
        DEBUG_PRINT("" << "Key: '" << pair.first << "' Type: " << (int)pair.second.getType() << std::endl);
    }
    DEBUG_PRINT("" << "===================" << std::endl);

    bool hosting = 1;
    std::string map = "QuantumArenaNight";
    std::string ipAddress = "127.0.0.1";
    float port = 7777;
    if (buff["IP"].getType() == ConfigDataType::TYPE_STRING) {

        ipAddress = buff["IP"].getAsString();
        ipAddress = ipAddress.substr(2, ipAddress.size() - 2);
        DEBUG_PRINT("" << "IP : " << ipAddress << std::endl);
    }
    else
    {
        DEBUG_PRINT("" << "Invalid IP: " << buff["IP"] << std::endl);
        ipAddress = "localhost";
    }
    if (buff["Port"].getType() == ConfigDataType::TYPE_FLOAT) {
        port = buff["Port"].getAsFloat();
        DEBUG_PRINT("" << "Port : " << port << std::endl);
    }
    else
    {
        DEBUG_PRINT("" << "Invalid port : " << buff["Port"] << " Defaulting to 7777" << std::endl);
        port = 7777;
    }
    if (buff["Map"].getType() == ConfigDataType::TYPE_STRING) {
        map = buff["Map"].getAsString();
        DEBUG_PRINT("" << "Map : " << map << std::endl);
    }
    else
    {
        DEBUG_PRINT("" << "Invalid Map: " << buff["Map"] << std::endl);
        map = "QuantumArenaNight";
        Sleep(5000);
    }
    if (buff["Host"].getType() == ConfigDataType::TYPE_BOOL) {
        hosting = buff["Host"].getAsBool();
        DEBUG_PRINT("" << "Host : " << hosting << std::endl);
    }
    else
    {
        DEBUG_PRINT("" << "Invalid Hosting flag: " << buff["Host"] << std::endl);
        hosting = 1;
    }
    SDK::APlayerController* MyController = World->OwningGameInstance->LocalPlayers[0]->PlayerController;
    if (hosting) {

        MyController = World->OwningGameInstance->LocalPlayers[0]->PlayerController;

        //// Give the thread time to start
        ////Sleep(2000);
        std::string menuString = "open " + map + "?listen";
        DEBUG_PRINT("menu string is : " << menuString);
        std::wstring menuwstr = std::wstring(menuString.begin(), menuString.end());
        SDK::FString menuCommand = menuwstr.c_str();
        DEBUG_PRINT("Executing command " << menuCommand);

        // This command might block or cause context switch
        SDK::UKismetSystemLibrary::ExecuteConsoleCommand(World, menuCommand, MyController);

        DEBUG_PRINT("Menu command completed (if we reach this line)");

    }
    else if (!hosting) {
        std::string commstring = "open " + ipAddress + ":" + std::to_string(port);
        g_commandString = commstring;
        std::wstring commandwstr = std::wstring(g_commandString.begin(), g_commandString.end());
        SDK::FString commandString = commandwstr.c_str();
        DEBUG_PRINT("" << "Executing command " << commandString << std::endl);
        SDK::UKismetSystemLibrary::ExecuteConsoleCommand(World, commandString, MyController);
    }

    //DEBUG_PRINT("" << Engine->ConsoleClass->GetFullName() << std::endl);

    /* Manually iterating GObjects and printing the FullName of every UObject that is a Pawn (not recommended) */
    //for (int i = 0; i < SDK::UObject::GObjects->Num(); i++)
    //{
    //    SDK::UObject* Obj = SDK::UObject::GObjects->GetByIndex(i);

    //    if (!Obj)
    //        continue;

    //    if (Obj->IsDefaultObject())
    //        continue;

    //    /* Only the 'IsA' check using the cast flags is required, the other 'IsA' is redundant */
    //    if (Obj->IsA(SDK::APawn::StaticClass()) || Obj->HasTypeFlag(SDK::EClassCastFlags::Pawn))
    //    {
    //        DEBUG_PRINT("" << Obj->GetFullName() << "\n");
    //    }
    //}

    ///* You might need to loop all levels in UWorld::Levels */
    //SDK::ULevel* Level = World->PersistentLevel;
    //SDK::TArray<SDK::AActor*>& Actors = Level->Actors;

    //for (SDK::AActor* Actor : Actors)
    //{
    //    /* The 2nd and 3rd checks are equal, prefer using EClassCastFlags if available for your class. */
    //    if (!Actor || !Actor->IsA(SDK::EClassCastFlags::Pawn) || !Actor->IsA(SDK::APawn::StaticClass()))
    //        continue;

    //    SDK::APawn* Pawn = static_cast<SDK::APawn*>(Actor);
    //    // Use Pawn here
    //}

    /*
    * Changes the keyboard-key that's used to open the UE console
    *
    * This is a rare case of a DefaultObjects' member-variables being changed.
    * By default you do not want to use the DefaultObject, this is a rare exception.
    */
    SDK::UInputSettings::GetDefaultObj()->ConsoleKeys[0].KeyName = SDK::UKismetStringLibrary::Conv_StringToName(L"F2");

    /* Creates a new UObject of class-type specified by Engine->ConsoleClass */
    SDK::UObject* NewObject = SDK::UGameplayStatics::SpawnObject(Engine->ConsoleClass, Engine->GameViewport);

    /* The Object we created is a subclass of UConsole, so this cast is **safe**. */
    Engine->GameViewport->ViewportConsole = static_cast<SDK::UConsole*>(NewObject);

    return 0;
}
/** currently not used, likely the solution to host launching first
* Current issue is that once loading mainmenu and other user connects, if using serverTravel it kicks the host back to mainmenu and leaves client hanging
*/
//DWORD MonitorGameState(LPVOID lpParam) {
//    DEBUG_PRINT("=== MonitorGameState Thread Started ===");
//
//    DEBUG_PRINT("Waiting for game to stabilize after menu load...");
//    Sleep(15000); // Longer wait for mainmenu to fully load and start hosting
//
//    int attemptCount = 0;
//    while (true) {
//        attemptCount++;
//        DEBUG_PRINT("=== Monitor Loop Start (Attempt " << attemptCount << ") ===");
//
//        try {
//            SDK::UWorld* CurrentWorld = SDK::UWorld::GetWorld();
//            if (!CurrentWorld) {
//                DEBUG_PRINT("World null, retrying...");
//                Sleep(3000);
//                continue;
//            }
//
//            // Check if we're actually hosting and ready
//            if (CurrentWorld->NetDriver && CurrentWorld->NetDriver->ClientConnections.IsValidIndex(0)) {
//                DEBUG_PRINT("Client connected! Preparing for serverTravel...");
//
//                // Give client time to fully connect and sync
//                Sleep(5000);
//
//                // Execute serverTravel with proper setup
//                if (CurrentWorld->OwningGameInstance &&
//                    CurrentWorld->OwningGameInstance->LocalPlayers.Num() > 0 &&
//                    CurrentWorld->OwningGameInstance->LocalPlayers[0]->PlayerController) {
//
//                    SDK::APlayerController* Controller = CurrentWorld->OwningGameInstance->LocalPlayers[0]->PlayerController;
//
//                    // Try a different command format that works better
//                    std::string betterCommand = "servertravel " + g_commandString.substr(12); // Remove "serverTravel " and add fresh
//                    // OR try: "travel " + mapname + "?listen"
//
//                    std::wstring commandwstr = std::wstring(betterCommand.begin(), betterCommand.end());
//                    SDK::FString commandString = commandwstr.c_str();
//
//                    DEBUG_PRINT("Executing improved command: " << commandString);
//                    SDK::UKismetSystemLibrary::ExecuteConsoleCommand(CurrentWorld, commandString, Controller);
//
//                    DEBUG_PRINT("ServerTravel command executed!");
//                    break;
//                }
//            }
//            else {
//                DEBUG_PRINT("No client connection detected yet, waiting...");
//                Sleep(3000);
//            }
//
//        }
//        catch (...) {
//            DEBUG_PRINT("Exception in monitoring, retrying...");
//            Sleep(3000);
//        }
//
//        if (attemptCount > 30) break; // Safety limit
//    }
//
//    return 0;
//}
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
        CreateThread(0, 0, (LPTHREAD_START_ROUTINE)MainThread, hModule, 0, 0);
        break;
    case DLL_PROCESS_DETACH:
        if (hOriginalDLL) {
            FreeLibrary(hOriginalDLL);
        }
        break;
    }
    return TRUE;
}