#pragma once
#include <string.h>
#include <fstream>
#include <sstream>
#include <cstdlib>
#include <iostream>
#include <unordered_map>

enum class ConfigDataType {
    TYPE_FLOAT,
    TYPE_BOOL,
    TYPE_STRING,
    TYPE_UNDEFINED,
};

//Why a union? Well if i opted for using a struct, let's say the config file had thousand's of elements, wouldn't that be wasting of free data?
union __int_m_data {
    __int_m_data() {}
    ~__int_m_data() {}   //All deallocations are handled later

    // FIXED: Proper copy constructor for union
    __int_m_data(const __int_m_data& other) {
        // Note: proper copying is handled by ConfigData copy constructor
        m_s_data = other.m_s_data;
    }

    char* m_s_data;
    float m_f_data;
    bool m_b_data;
};

struct ConfigData {
public:
    ConfigData(const std::string& label, const char* data) {
        m_data.m_s_data = (char*)malloc(strlen(data) + 1);
        strcpy(m_data.m_s_data, data);
        m_type = ConfigDataType::TYPE_STRING;
        m_Fatal = false;
        m_label = label;
    }
    ConfigData(const std::string& label, bool data) {
        m_data.m_b_data = data;
        m_type = ConfigDataType::TYPE_BOOL;
        m_Fatal = false;
        m_label = label;
    }
    ConfigData(const std::string& label, float data) {
        m_data.m_f_data = data;
        m_type = ConfigDataType::TYPE_FLOAT;
        m_Fatal = false;
        m_label = label;
    }

    //This is used to catch faulty issues
    ConfigData(bool Fatality) {
        m_type = ConfigDataType::TYPE_UNDEFINED;
        m_Fatal = Fatality;
        m_label = ""; // Initialize label
    }

    ConfigData() {
        m_type = ConfigDataType::TYPE_UNDEFINED;
        m_Fatal = false;
        m_label = ""; // Initialize label
    }

    // ADDED: Copy constructor - CRITICAL for unordered_map storage
    ConfigData(const ConfigData& other) : m_type(other.m_type), m_label(other.m_label), m_Fatal(other.m_Fatal) {
        if (m_type == ConfigDataType::TYPE_STRING && other.m_data.m_s_data) {
            size_t len = strlen(other.m_data.m_s_data) + 1;
            m_data.m_s_data = (char*)malloc(len);
            strcpy(m_data.m_s_data, other.m_data.m_s_data);
        }
        else {
            m_data = other.m_data;
        }
    }

    // ADDED: Assignment operator for completeness
    ConfigData& operator=(const ConfigData& other) {
        if (this != &other) {
            // Clean up existing string data
            if (m_type == ConfigDataType::TYPE_STRING && m_data.m_s_data) {
                free(m_data.m_s_data);
            }

            m_type = other.m_type;
            m_label = other.m_label;
            m_Fatal = other.m_Fatal;

            if (m_type == ConfigDataType::TYPE_STRING && other.m_data.m_s_data) {
                size_t len = strlen(other.m_data.m_s_data) + 1;
                m_data.m_s_data = (char*)malloc(len);
                strcpy(m_data.m_s_data, other.m_data.m_s_data);
            }
            else {
                m_data = other.m_data;
            }
        }
        return *this;
    }

    ConfigDataType getType() { return m_type; }

    bool getFatality() { return m_Fatal; }

    std::string getLabel() { return m_label; }

    std::string getAsString() const {
        if (m_type != ConfigDataType::TYPE_STRING) {
            throw std::runtime_error("Data is not a string");
        }
        return std::string(m_data.m_s_data);
    }

    float getAsFloat() const {
        if (m_type != ConfigDataType::TYPE_FLOAT) {
            throw std::runtime_error("Data is not a float");
        }
        return m_data.m_f_data;
    }

    bool getAsBool() const {
        if (m_type != ConfigDataType::TYPE_BOOL) {
            throw std::runtime_error("Data is not a bool");
        }
        return m_data.m_b_data;
    }

    // FIXED: Destructor logic was backwards
    ~ConfigData() {
        if (m_type == ConfigDataType::TYPE_STRING) {
            if (m_data.m_s_data)    // REMOVED the '!' - free when NOT null
                free((void*)m_data.m_s_data);
        }
    }

    friend std::ostream& operator<<(std::ostream& os, const ConfigData& data);
private:
    ConfigDataType m_type;
    __int_m_data m_data;
    std::string m_label;
    bool m_Fatal;
};

// ADDED: Helper function to remove BOM
inline std::string removeBOM(const std::string& str) {
    const char bom[] = { (char)0xEF, (char)0xBB, (char)0xBF };
    if (str.length() >= 3 && str.substr(0, 3) == std::string(bom, 3)) {
        return str.substr(3);
    }
    return str;
}

// ADDED: Helper function to trim whitespace
inline std::string trim(const std::string& str) {
    size_t first = str.find_first_not_of(' ');
    if (first == std::string::npos) return "";
    size_t last = str.find_last_not_of(' ');
    return str.substr(first, (last - first + 1));
}

inline ConfigData parseConfigLine(const std::string& Line, int line_of_file) {
    // FIXED: Remove BOM and trim the line first
    std::string cleanLine = trim(removeBOM(Line));

    if (cleanLine.empty()) {    // FIXED: Check cleaned line
        return ConfigData(false);
    }

    // FIXED: Check for comment at start of cleaned line
    if (cleanLine[0] == '#') {
        return ConfigData(false);
    }

    //Checking if a line has a header
    if (cleanLine[0] == '[' && cleanLine[cleanLine.length() - 1] == ']') {
        return ConfigData(false);
    }

    std::stringstream iss(cleanLine);
    std::string VariableName, Equals, Data;
    iss >> VariableName >> Equals >> Data;

    // FIXED: Better validation
    if (VariableName.empty() || Equals.empty()) {
        return ConfigData(false); // Skip malformed lines instead of fatal error
    }

    if (Equals != "=") {    //Some formatting issue
        return ConfigData(true); // This is still a fatal error
    }

    // FIXED: Handle case where Data might be empty (like IP = A_)
    if (Data.empty()) {
        return ConfigData(VariableName, ""); // Return empty string instead of fatal error
    }

    // FIXED: More robust boolean detection
    if (Data == "TRUE" || Data == "FALSE" || Data == "true" || Data == "false") {
        return ConfigData(VariableName, (Data == "TRUE" || Data == "true") ? true : false);
    }

    // Try to parse as float
    float f_data;
    bool not_Float = false;
    try {
        f_data = std::stof(Data);
    }
    catch (const std::exception& e) {
        not_Float = true;
    }
    if (!not_Float) {
        return ConfigData(VariableName, f_data);
    }

    // Default to string
    return ConfigData(VariableName, Data.c_str());
}

// FIXED: Major logic fix in LoadConfigFile
inline std::unordered_map<std::string, ConfigData> LoadConfigFile(const std::string& FileName) {
    unsigned int line_of_file = 1;
    std::unordered_map<std::string, ConfigData> configBuffer;
    std::fstream fileBuff(FileName);
    if (!fileBuff) {
        std::cout << "Failed loading config file: " << FileName << "\n";
        exit(-1);
    }
    std::string Line;
    while (std::getline(fileBuff, Line)) {
        ConfigData buffData(parseConfigLine(Line, line_of_file));

        // FIXED: Check if it's fatal first
        if (buffData.getFatality()) {
            std::cout << "ERROR: Formatting error at line " << line_of_file << ": " << Line << "\n";
            exit(-1);
        }

        // FIXED: Only add to map if it's valid data (has a label and isn't undefined)
        if (buffData.getType() != ConfigDataType::TYPE_UNDEFINED && !buffData.getLabel().empty()) {
            configBuffer[buffData.getLabel()] = buffData;
        }

        line_of_file++;
    }
    fileBuff.close();
    return configBuffer;
}

// FIXED: Safer operator<< using the safe accessors
inline std::ostream& operator<<(std::ostream& os, const ConfigData& data) {
    try {
        if (data.m_type == ConfigDataType::TYPE_BOOL) {
            os << data.getAsBool();
        }
        else if (data.m_type == ConfigDataType::TYPE_FLOAT) {
            os << data.getAsFloat();
        }
        else if (data.m_type == ConfigDataType::TYPE_STRING) {
            os << data.getAsString();
        }
        else {
            os << "UNDEFINED DATA TYPE";
        }
    }
    catch (const std::exception& e) {
        os << "ERROR: " << e.what();
    }
    return os;
}