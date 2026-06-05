class BankAccount {
  #balance;

  constructor(owner, initialDeposit = 0) {
    this.owner = owner;
    this.#balance = initialDeposit;
    this.history = [];
  }

  deposit(amount) {
    if (amount <= 0) throw new Error("сумма должна быть положительной");
    this.#balance += amount;
    this.history.push({ type: "deposit", amount, balance: this.#balance });
  }

  withdraw(amount) {
    if (amount > this.#balance) throw new Error("недостаточно средств");
    this.#balance -= amount;
    this.history.push({ type: "withdraw", amount, balance: this.#balance });
  }

  get balance() {
    return this.#balance;
  }

  printHistory() {
    console.log(`\nсчёт: ${this.owner}`);
    for (const entry of this.history) {
      const sign = entry.type === "deposit" ? "+" : "-";
      console.log(`  ${sign}${entry.amount} -> баланс: ${entry.balance}`);
    }
  }
}

const acc = new BankAccount("Dunvtu", 1000);
acc.deposit(500);
acc.withdraw(200);
acc.deposit(100);
acc.withdraw(800);
acc.printHistory();
console.log("итого:", acc.balance);