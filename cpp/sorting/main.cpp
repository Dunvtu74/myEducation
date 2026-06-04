#include <iostream>
#include <vector>
#include <algorithm>
#include <chrono>
#include <random>

void bubble_sort(std::vector<int>& v) {
    for (int i = 0; i < (int)v.size() - 1; i++) {
        for (int j = 0; j < (int)v.size() - i - 1; j++) {
            if (v[j] > v[j+1]) std::swap(v[j], v[j+1]);
        }
    }
}

void selection_sort(std::vector<int>& v) {
    for (int i = 0; i < (int)v.size(); i++) {
        int min = i;
        for (int j = i+1; j < (int)v.size(); j++) {
            if (v[j] < v[min]) min = j;
        }
        std::swap(v[i], v[min]);
    }
}

void merge(std::vector<int>& v, int l, int m, int r) {
    std::vector<int> left(v.begin()+l, v.begin()+m+1);
    std::vector<int> right(v.begin()+m+1, v.begin()+r+1);
    int i = 0, j = 0, k = l;
    while (i < (int)left.size() && j < (int)right.size()) {
        v[k++] = (left[i] <= right[j]) ? left[i++] : right[j++];
    }
    while (i < (int)left.size()) v[k++] = left[i++];
    while (j < (int)right.size()) v[k++] = right[j++];
}

void merge_sort(std::vector<int>& v, int l, int r) {
    if (l >= r) return;
    int m = (l + r) / 2;
    merge_sort(v, l, m);
    merge_sort(v, m+1, r);
    merge(v, l, m, r);
}

std::vector<int> random_vec(int n) {
    std::mt19937 rng(42);
    std::uniform_int_distribution<int> dist(0, 10000);
    std::vector<int> v(n);
    for (auto& x : v) x = dist(rng);
    return v;
}

template<typename Fn>
long long measure(Fn fn) {
    auto t0 = std::chrono::high_resolution_clock::now();
    fn();
    auto t1 = std::chrono::high_resolution_clock::now();
    return std::chrono::duration_cast<std::chrono::microseconds>(t1 - t0).count();
}

int main() {
    const int N = 5000;

    auto v1 = random_vec(N);
    auto v2 = v1;
    auto v3 = v1;
    auto v4 = v1;

    auto t1 = measure([&]{ bubble_sort(v1); });
    auto t2 = measure([&]{ selection_sort(v2); });
    auto t3 = measure([&]{ merge_sort(v3, 0, N-1); });
    auto t4 = measure([&]{ std::sort(v4.begin(), v4.end()); });

    std::cout << "n = " << N << "\n";
    std::cout << "bubble:    " << t1 << " us\n";
    std::cout << "selection: " << t2 << " us\n";
    std::cout << "merge:     " << t3 << " us\n";
    std::cout << "std::sort: " << t4 << " us\n";

    return 0;
}
