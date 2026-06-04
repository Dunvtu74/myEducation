#include <iostream>
#include <vector>

using Matrix = std::vector<std::vector<int>>;

Matrix multiply(const Matrix& a, const Matrix& b) {
    int n = a.size();
    Matrix result(n, std::vector<int>(n, 0));
    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            for (int k = 0; k < n; k++)
                result[i][j] += a[i][k] * b[k][j];
    return result;
}

void print(const Matrix& m) {
    for (const auto& row : m) {
        for (int val : row) std::cout << val << "\t";
        std::cout << "\n";
    }
}

int main() {
    Matrix a = {{1, 2}, {3, 4}};
    Matrix b = {{5, 6}, {7, 8}};

    std::cout << "A:\n"; print(a);
    std::cout << "B:\n"; print(b);
    std::cout << "A * B:\n"; print(multiply(a, b));

    return 0;
}
