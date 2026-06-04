# sorting

Three sorting algorithms compared against std::sort on a random array of 5000 integers.

```bash
g++ -O2 -o sort main.cpp
./sort
```

bubble and selection are O(n²), merge is O(n log n). The benchmark makes the difference obvious even at n=5000.
