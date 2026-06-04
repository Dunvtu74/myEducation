#include <iostream>
#include <cstdlib>
#include <ctime>

int main() {
    srand(time(nullptr));
    int secret = rand() % 100 + 1;
    int guess = 0;
    int tries = 0;

    std::cout << "угадай число от 1 до 100\n";

    while (guess != secret) {
        std::cin >> guess;
        tries++;
        if (guess < secret) std::cout << "больше\n";
        else if (guess > secret) std::cout << "меньше\n";
        else std::cout << "точно! попыток: " << tries << "\n";
    }

    return 0;
}
