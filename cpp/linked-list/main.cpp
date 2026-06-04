#include <iostream>

struct Node {
    int val;
    Node* next;
    Node(int v) : val(v), next(nullptr) {}
};

struct LinkedList {
    Node* head = nullptr;

    void push_back(int val) {
        Node* n = new Node(val);
        if (!head) { head = n; return; }
        Node* cur = head;
        while (cur->next) cur = cur->next;
        cur->next = n;
    }

    void push_front(int val) {
        Node* n = new Node(val);
        n->next = head;
        head = n;
    }

    bool remove(int val) {
        if (!head) return false;
        if (head->val == val) {
            Node* tmp = head;
            head = head->next;
            delete tmp;
            return true;
        }
        Node* cur = head;
        while (cur->next && cur->next->val != val) cur = cur->next;
        if (!cur->next) return false;
        Node* tmp = cur->next;
        cur->next = tmp->next;
        delete tmp;
        return true;
    }

    void reverse() {
        Node* prev = nullptr;
        Node* cur = head;
        while (cur) {
            Node* next = cur->next;
            cur->next = prev;
            prev = cur;
            cur = next;
        }
        head = prev;
    }

    void print() const {
        Node* cur = head;
        while (cur) {
            std::cout << cur->val;
            if (cur->next) std::cout << " -> ";
            cur = cur->next;
        }
        std::cout << "\n";
    }

    ~LinkedList() {
        Node* cur = head;
        while (cur) {
            Node* next = cur->next;
            delete cur;
            cur = next;
        }
    }
};

int main() {
    LinkedList list;

    for (int i = 1; i <= 5; i++) list.push_back(i);
    list.print();

    list.push_front(0);
    list.print();

    list.remove(3);
    list.print();

    list.reverse();
    list.print();

    return 0;
}
