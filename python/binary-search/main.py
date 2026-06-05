def binary_search(arr, target):
    left, right = 0, len(arr) - 1
    steps = 0

    while left <= right:
        mid = (left + right) // 2
        steps += 1

        if arr[mid] == target:
            return mid, steps
        elif arr[mid] < target:
            left = mid + 1
        else:
            right = mid - 1

    return -1, steps


if __name__ == "__main__":
    data = list(range(1, 1001))

    tests = [1, 500, 999, 42, 777, 1001]
    for t in tests:
        idx, steps = binary_search(data, t)
        if idx != -1:
            print(f"{t}: найден на позиции {idx} за {steps} шагов")
        else:
            print(f"{t}: не найден ({steps} шагов)")