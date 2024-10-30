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
        Console.WriteLine("\nSo sánh các phần tử trùng nhau trong mảng gốc và mảng đã sắp xếp:");
        Console.WriteLine("Format: [Giá trị (Vị trí gốc)]");

        // tìm các giá trị xuất hiện nhiều lần trong mảng gốc
        var duplicateGroups = original
            .GroupBy(x => x.Value)
            .Where(g => g.Count() > 1)
            .OrderByDescending(g => g.Count())
            .Take(3);  // chỉ lấy 3 nhóm có số lần xuất hiện nhiều nhất

        foreach (var group in duplicateGroups)
        {
            Console.WriteLine($"\nPhần tử có giá trị {group.Key} (xuất hiện {group.Count()} lần):");

            // lấy vị trí các phần tử trong mảng gốc
            var originalPositions = original
                .Select((item, index) => new { Item = item, Index = index })
                .Where(x => x.Item.Value == group.Key)
                .Take(5);  // chỉ hiển thị tối đa 5 phần tử

            Console.Write("Trong mảng gốc: ");
            foreach (var pos in originalPositions)
            {
                Console.Write($"[{pos.Item.Value}({pos.Item.OriginalPosition})] ");
            }
            Console.WriteLine();

            // lấy vị trí các phần tử trong mảng đã sắp xếp
            var sortedPositions = sorted
                .Select((item, index) => new { Item = item, Index = index })
                .Where(x => x.Item.Value == group.Key)
                .Take(5);  

            Console.Write("Trong mảng đã sắp xếp: ");
            foreach (var pos in sortedPositions)
            {
                Console.Write($"[{pos.Item.Value}({pos.Item.OriginalPosition})] ");
            }
            Console.WriteLine();

            Console.Write("Chỉ số hiện tại của các phần tử trong mảng sau khi đã sắp xếp: ");
            foreach (var pos in sortedPositions)
            {
                Console.Write($"[{pos.Item.Value}({pos.Index})] ");
            }
            Console.WriteLine();

        }
    }

    static void PrintArray(List<Item> arr, string message)
    {
        Console.WriteLine(message);
        foreach (var item in arr)
        {
            Console.Write($"[{item.Value}({item.OriginalPosition})] ");
        }
        Console.WriteLine("\n");
    }


    static void Main()
    {
        Console.OutputEncoding = Encoding.Unicode;
        Console.OutputEncoding = Encoding.Unicode;

        const int ARRAY_SIZE = 1000; //CHỈNH ĐỂ DÙNG HÀM IN MẢNG


        const int NUM_RUNS = 100;

        Timing timing = new Timing();
        List<TimeSpan> runTimes = new List<TimeSpan>();
        bool isStable = true;

        Console.WriteLine("Bắt đầu demo Selection Sort:");
        Console.WriteLine($"- Kích thước mảng: {ARRAY_SIZE}");
        Console.WriteLine($"- Số lần chạy: {NUM_RUNS}");
        Console.WriteLine();

        // chạy và so sánh với mảng 
        List<Item> arr = GenerateRandomArray(ARRAY_SIZE);
        List<Item> originalArr = new List<Item>(arr);

        //PrintArray(originalArr, "Mảng gốc:"); //in mảng gốc

        timing.startTime();
        StableSelectionSort(arr);
        timing.StopTime();

        //PrintArray(arr, "Mảng sau khi sắp xếp:"); //in mảng đã sắp xếp

        // kiểm tra các phần tử trùng nhau
        CompareArrays(originalArr, arr);

        // kiểm tra tính ổn định và thời gian cho các lần chạy còn lại
        for (int run = 1; run < NUM_RUNS; run++)
        {
            arr = GenerateRandomArray(ARRAY_SIZE);
            originalArr = new List<Item>(arr);

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


