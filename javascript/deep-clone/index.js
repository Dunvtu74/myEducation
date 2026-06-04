function deepClone(obj) {
  if (obj === null || typeof obj !== "object") return obj;
  if (Array.isArray(obj)) return obj.map(deepClone);
  return Object.fromEntries(
    Object.entries(obj).map(([k, v]) => [k, deepClone(v)])
  );
}

const original = {
  name: "test",
  nested: { a: 1, b: [1, 2, 3] },
  arr: [{ x: 10 }, { x: 20 }]
};

const clone = deepClone(original);
clone.nested.a = 999;
clone.arr[0].x = 0;

console.log("original:", JSON.stringify(original));
console.log("clone:   ", JSON.stringify(clone));
