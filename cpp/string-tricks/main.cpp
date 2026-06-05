#include <iostream>
#include <string>
#include <algorithm>
#include <map>

// разворот строки
std::string rev(std::string s) {
    std::reverse(s.begin(), s.end());
    return s;
}

// анаграммы
bool is_anagram(std::string a, std::string b) {
    std::sort(a.begin(), a.end());
    std::sort(b.begin(), b.end());
    return a == b;
}

// частота символов
std::map<char, int> freq(const std::string& s) {
    std::map<char, int> m;
    for (char c : s) m[c]++;
    return m;
}

// удалить дубли символов
std::string unique_chars(const std::string& s) {
    std::string result;
    for (char c : s) {
        if (result.find(c) == std::string::npos)
            result += c;
    }
    return result;
}

int main() {
    std::string word = "programming";

    std::cout << "слово:      " << word << "\n";
    std::cout << "разворот:   " << rev(word) << "\n";
    std::cout << "уникальные: " << unique_chars(word) << "\n";

    std::cout << "\nчастота символов:\n";
    for (auto& [ch, cnt] : freq(word)) {
        std::cout << "  " << ch << ": " << cnt << "\n";
    }

    std::cout << "\nanagram / nagaram: " << (is_anagram("anagram", "nagaram") ? "да" : "нет") << "\n";
    std::cout << "hello / world: " << (is_anagram("hello", "world") ? "да" : "нет") << "\n";

    return 0;
}