#include <iostream>
#include <stack>
#include <string>
#include <vector>

bool check(const std::string& s) {
    std::stack<char> st;
    for (char c : s) {
        if (c == '(' || c == '[' || c == '{') {
            st.push(c);
        } else if (c == ')' || c == ']' || c == '}') {
            if (st.empty()) return false;
            char top = st.top(); st.pop();
            if (c == ')' && top != '(') return false;
            if (c == ']' && top != '[') return false;
            if (c == '}' && top != '{') return false;
        }
    }
    return st.empty();
}

int main() {
    std::vector<std::string> tests = {
        "(())",
        "{[()]}",
        "([)]",
        "(((",
        "{[]()}"
    };

    for (const auto& t : tests) {
        std::cout << t << " -> " << (check(t) ? "ok" : "bad") << "\n";
    }

    return 0;
}
