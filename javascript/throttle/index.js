function throttle(fn, limit) {
  let lastCall = 0;
  return function(...args) {
    const now = Date.now();
    if (now - lastCall >= limit) {
      lastCall = now;
      fn.apply(this, args);
    }
  };
}

const onScroll = throttle((pos) => {
  console.log(`скролл: ${pos}px`);
}, 200);

// симулируем частые вызовы
let pos = 0;
const interval = setInterval(() => {
  pos += 10;
  onScroll(pos);
  if (pos >= 100) clearInterval(interval);
}, 50);
