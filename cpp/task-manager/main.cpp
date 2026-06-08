#include <iostream>
#include <string>
#include <vector>
#include <fstream>
#include <Windows.h>
#define RESET   "\033[0m"
#define RED     "\033[31m"
#define GREEN   "\033[32m"
#define YELLOW  "\033[33m"
#define BOLD    "\033[1m"

// контейнер для id name done/non-done
struct Task {
  int id;
  std::string title;
  bool done;
};

void saveToFile(const std::vector<Task>& tasks) {
  std::ofstream file("tasks.txt");
  for (const auto& t : tasks) {
    file << t.id << "|" << t.done << "|" << t.title << "\n";
  }
}

void loadFromFile(std::vector<Task>& tasks) {
  std::ifstream file("tasks.txt");
  if (!file.is_open()) return;

  std::string line;
  while (std::getline(file, line)) {
    Task t;
    t.id = std::stoi(line.substr(0, line.find('|')));
    line = line.substr(line.find('|') + 1);
    t.done = line[0] == '1';
    t.title = line.substr(2);
    tasks.push_back(t);
  }
}

void addTask(std::vector<Task>& tasks) {
  Task t;
  t.id = tasks.size() + 1;
  t.done  = false;

  std::cout << "Название: ";
  std::cin.ignore(); //чтобы getline не жрал пустую строку
  std::getline(std::cin, t.title);

  tasks.push_back(t);
  std::cout << "Добавлено.\n";
}

void showTasks(const std::vector<Task>& tasks) {
    if (tasks.empty()) {
        std::cout << YELLOW << "Задач нет." << RESET << "\n";
        return;
    }
    for (const auto& t : tasks) {
        if (t.done) {
            std::cout << GREEN << "[x] " << t.id << ". " << t.title << RESET << "\n";
        } else {
            std::cout << RED << "[ ] " << t.id << ". " << t.title << RESET << "\n";
        }
    }
}

void completeTask(std::vector<Task>& tasks) {
  if (tasks.empty()) {
    std::cout << "Задач нет.\n";
    return;
  }
  showTasks(tasks);
  std::cout << "Номер задачи: ";
  int id;
  std::cin >> id;

  for (auto& t : tasks) {
    if (t.id == id) {
      t.done = true;
      std::cout << "Готово.\n";
      return;
    }
  }
  std::cout << "Не найдено.\n";
}

int main() {
  SetConsoleOutputCP(65001); //крутой терминал виндовса переделываем из CP1251 в UTF-8
  SetConsoleCP(65001);
  
  std::vector<Task> tasks;
  loadFromFile(tasks);

  std::cout << "\nTask Manager\n" << RESET;
  int choice = 0;
  while (true)
  {
    std::cout << "\n1. Добавить задачу\n";
    std::cout << "2. Показкать задачи\n";
    std::cout << "3. Выход\n";
    std::cout << "4. Отметить выполненой\n";
    std::cin >> choice;

    if (choice == 3) {
      saveToFile(tasks);
      break;
    }
    if (choice == 1) addTask(tasks);
    if (choice == 2) showTasks(tasks);
    if (choice == 4) completeTask(tasks);
  }
  return 0;
}
