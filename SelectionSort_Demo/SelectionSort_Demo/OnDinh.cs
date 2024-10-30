using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Program
{
    class Item : IComparable<Item>
    {
        public int Value { get; set; }
        public int OriginalPosition { get; set; }

        public Item(int value, int position)
        {
            Value = value;
            OriginalPosition = position;
        }

       /* public int CompareTo(Item other)
        {
            return Value.CompareTo(other.Value);
        }
*/
        // sửa đổi 1: thay đổi cách so sánh để xét cả giá trị lẫn vị trí gốc
        public int CompareTo(Item other)
        {

            int valueComparison = Value.CompareTo(other.Value);
            if (valueComparison == 0)
            {
                return OriginalPosition.CompareTo(other.OriginalPosition);
            }
            return valueComparison;
        }
    }

    static void StableSelectionSort(List<Item> arr)
    {
        int n = arr.Count;

        for (int i = 0; i < n - 1; i++)
        {
            int minIdx = i;


/*
            for (int j = i + 1; j < n; j++)
            {
                if (arr[j].CompareTo(arr[minIdx]) < 0)
                {
                    minIdx = j;
                }
            }*/

            //sửa đổi 2: thay đổi cách tìm phần tử nhỏ nhất
            for (int j = i + 1; j < n; j++)
            {
                if (arr[j].Value < arr[minIdx].Value ||
                    (arr[j].Value == arr[minIdx].Value &&
                     arr[j].OriginalPosition < arr[minIdx].OriginalPosition))
                {
                    minIdx = j;
                }
            }

           /* if (minIdx != i)
            {
                var temp = arr[i];
                arr[i] = arr[minIdx];
                arr[minIdx] = temp;
            }
*/
            // sửa đổi 3: sử dụng phương pháp dịch chuyển thay vì hoán đổi
            if (minIdx != i)
            {
                Item minItem = arr[minIdx];
                // dịch chuyển các phần tử để giữ nguyên thứ tự tương đối
                arr.RemoveAt(minIdx);
                arr.Insert(i, minItem);
            }
        }
    }

    static List<Item> GenerateRandomArray(int size)
    {
        Random rand = new Random();
        List<Item> arr = new List<Item>();
        for (int i = 0; i < size; i++)
        {
            int value = rand.Next(1, size / 3);
            arr.Add(new Item(value, i));
        }
        return arr;
    }

    static bool CheckStability(List<Item> original, List<Item> sorted)
    {
        for (int i = 0; i < sorted.Count - 1; i++)
        {
            if (sorted[i].Value == sorted[i + 1].Value &&
                sorted[i].OriginalPosition > sorted[i + 1].OriginalPosition)
            {
                return false;
            }
        }
        return true;
    }

    static void CompareArrays(List<Item> original, List<Item> sorted)
    {
        Console.WriteLine("\nSo sánh mảng gốc và mảng đã sắp xếp:");
        Console.WriteLine("Format: [Giá trị (Vị trí gốc)]");

        // tìm các phần tử có giá trị giống nhau để demo tính ổn định
        var duplicateValues = original
            .GroupBy(x => x.Value)
            .Where(g => g.Count() > 1)
            .Take(3) // lấy 3 nhóm giá trị trùng nhau trong mảng để kiểm tra
            .ToList();

        foreach (var group in duplicateValues)
        {
            Console.WriteLine($"\nCác phần tử có giá trị {group.Key}:");

            Console.Write("Mảng gốc:     ");
            var originalItems = original.Where(x => x.Value == group.Key)
                                     .Take(4) // hiển thị ra 4 phần tử
                                     .ToList();
            foreach (var item in originalItems)
            {
                Console.Write($"[{item.Value}({item.OriginalPosition})] ");
            }

            Console.Write("\nMảng đã sắp xếp: ");
            var sortedItems = sorted.Where(x => x.Value == group.Key)
                                  .Take(4)
                                  .ToList();
            foreach (var item in sortedItems)
            {
                Console.Write($"[{item.Value}({item.OriginalPosition})] ");
            }
            Console.WriteLine();
        }
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;
        const int ARRAY_SIZE = 1000;
        const int NUM_RUNS = 100;

        Timing timing = new Timing();
        List<TimeSpan> runTimes = new List<TimeSpan>();
        bool isStable = true;

        Console.WriteLine("Bắt đầu demo Selection Sort:");
        Console.WriteLine($"- Kích thước mảng: {ARRAY_SIZE}");
        Console.WriteLine($"- Số lần chạy: {NUM_RUNS}");
        Console.WriteLine();

        // chạy demo với một mảng nhỏ hơn để so sánh
        List<Item> demoArr = GenerateRandomArray(100); // sử dụng mảng nhỏ hơn để demo
        List<Item> demoOriginal = new List<Item>(demoArr);
        StableSelectionSort(demoArr);
        CompareArrays(demoOriginal, demoArr);

        // kiểm tra thời gian chạy
        for (int run = 0; run < NUM_RUNS; run++)
        {
            List<Item> arr = GenerateRandomArray(ARRAY_SIZE);
            List<Item> originalArr = new List<Item>(arr);

            timing.startTime();
            StableSelectionSort(arr);
            timing.StopTime();

            runTimes.Add(timing.Result());

            if (!CheckStability(originalArr, arr))
            {
                isStable = false;
                Console.WriteLine($"Phát hiện mất tính ổn định ở lần chạy {run + 1}");
            }
        }

        TimeSpan averageTime = new TimeSpan((long)runTimes.Average(t => t.Ticks));
        Console.WriteLine("\nKết quả: ");
        Console.WriteLine($"- Thời gian chạy trung bình: {averageTime.TotalMilliseconds:F3}ms");
        Console.WriteLine($"- Thuật toán có ổn định không? {(isStable ? "Có" : "Không")}");

        Console.WriteLine("\nThống kê thời gian:");
        Console.WriteLine($"- Thời gian ngắn nhất: {runTimes.Min().TotalMilliseconds:F3}ms");
        Console.WriteLine($"- Thời gian dài nhất: {runTimes.Max().TotalMilliseconds:F3}ms");
    }
}

