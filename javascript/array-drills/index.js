// задания на массивы — то что дают в начале любого js-курса

const nums = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5];

// сумма элементов
const sum = nums.reduce((acc, n) => acc + n, 0);
console.log("сумма:", sum);

// уникальные значения
const unique = [...new Set(nums)];
console.log("уникальные:", unique);

// только чётные
const evens = nums.filter(n => n % 2 === 0);
console.log("чётные:", evens);

// максимум без Math.max
const max = nums.reduce((a, b) => a > b ? a : b);
console.log("макс:", max);

// перевернуть без reverse()
const flipped = nums.reduce((acc, n) => [n, ...acc], []);
console.log("перевёрнут:", flipped);

// сгруппировать по чётности
const grouped = nums.reduce((acc, n) => {
  const key = n % 2 === 0 ? "even" : "odd";
  acc[key].push(n);
  return acc;
}, { even: [], odd: [] });
console.log("сгруппированы:", grouped);