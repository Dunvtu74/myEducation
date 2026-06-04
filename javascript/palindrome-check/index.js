function isPalindrome(str) {
  const clean = str.toLowerCase().replace(/[^a-zа-яё0-9]/gi, "");
  return clean === clean.split("").reverse().join("");
}

const words = ["level", "hello", "racecar", "мадам", "javascript", "топот", "noon"];

for (const w of words) {
  console.log(`${w}: ${isPalindrome(w) ? "палиндром" : "нет"}`);
}
