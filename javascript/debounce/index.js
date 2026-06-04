function debounce(fn, delay) {
  let timer = null;
  return function(...args) {
    clearTimeout(timer);
    timer = setTimeout(() => fn.apply(this, args), delay);
  };
}

// пример: функция срабатывает только через 300мс после последнего вызова
const search = debounce((query) => {
  console.log(`ищем: ${query}`);
}, 300);

search("p");
search("py");
search("pyt");
search("pyth");
search("python");
// выведет только "ищем: python"

setTimeout(() => search("js"), 500);
