def two_sum(nums, target):
    seen = {}
    for i, n in enumerate(nums):
        need = target - n
        if need in seen:
            return [seen[need], i]
        seen[n] = i
    return []

tests = [
    ([2, 7, 11, 15], 9),
    ([3, 2, 4], 6),
    ([1, 5, 3, 7, 2], 9),
]

for nums, target in tests:
    result = two_sum(nums, target)
    print(f"{nums}, target={target} -> {result}")
