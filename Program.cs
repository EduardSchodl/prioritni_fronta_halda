
using System;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

/**
 * Interval mezi dvema merenimi
 */
public class Interval
{
    /** Zacatek intervalu */
    public double start;
    /** Konec intervalu */
    public double end;

    /**
	 * Vytvori novy interval mezi dvema merenimi
	 */
    public Interval(double start, double end)
    {
        this.start = start;
        this.end = end;
    }
}

/**
 * Halda (s maximem v koreni) pro ulozeni intervalu
 */
public class Heap
{
    /** Hodnoty ulozene v halde */
    Interval[] values;
    /** Priority hodnot v halde */
    double[] priorities;
    /** Pocet hodnot v halde */
    int count = 0;

    /**
	 * Vytvori novou haldu se zadanou kapacitou
	 */
    public Heap(int capacity)
    {
        values = new Interval[capacity + 1];
        priorities = new double[capacity + 1];
    }

    /**
	 * Prida novou hodnotu (interval) do haldy
	 */
    public void Add(Interval e, double priority)
    {
        count++;

        if(count == values.Length)
        {
            IncreaseCapacity();
        }

        values[count] = e;
        priorities[count] = priority;
        FixUp(count);
    }

    /// <summary>
    /// Metoda zvýší kapacitu polí o dvojnásobek a překopíruje data.
    /// </summary>
    public void IncreaseCapacity()
    {
        Interval[] newValues = new Interval[values.Length * 2];
        double[] newPriorities = new double[priorities.Length * 2];

        for (int i = 0; i < values.Length; i++)
        {
            newValues[i] = values[i];
            newPriorities[i] = priorities[i];
        }

        this.values = newValues;
        this.priorities = newPriorities;
    }

    /**
	 * Prohodi hodnoty i priority na zadanych indexech
	 */
    void Swap(int x, int y)
    {
        double tmp = priorities[x];
        priorities[x] = priorities[y];
        priorities[y] = tmp;

        Interval val = values[x];
        values[x] = values[y];
        values[y] = val;
    }

    /**
	 * Opravi vlastnosti haldy smerem nahoru od zadaneho indexu
	 */
    void FixUp(int index)
    {
        if (index == 1)
        {
            return;
        }

        int pred = index / 2;
        if (priorities[pred] < priorities[index])
        {
            Swap(pred, index);
            FixUp(pred);
        }
    }

    /// <summary>
    /// Metoda vrátí nejdelší interval a zároveň ho odebere ze struktury.
    /// </summary>
    /// <returns>Odebraný interval</returns>
    public Interval RemoveMax()
    {
        Interval temp = values[1];

        priorities[1] = priorities[count];
        values[1] = values[count];
        count = count - 1;
        FixDown(1);

        return temp;
    }

    /// <summary>
    /// Metoda opraví strukturu haldy směrem dolů od zadaného indexu.
    /// </summary>
    /// <param name="heapBreach">Index počátečního prvku</param>
    void FixDown(int heapBreach)
    {
        int n = heapBreach;
        while (2 * n <= count)
        {
            int j = 2 * n;
            if ((j + 1) <= count)
                if (priorities[j + 1] > priorities[j])
                    j = j + 1;
            if (priorities[n] > priorities[j])
                return;
            else
            {
                Swap(j, n);
                n = j;
            }
        }
    }
}

public class Experiment
{
    public static Heap heap;

    public static void Main(String[] args)
    {
        /*
        Heap test = new Heap(100);
        
        //test.Add(new Interval(0, 0.1), 0.1);
        //test.Add(new Interval(0.1, 0.3), 0.2);
        //test.Add(new Interval(0.4, 0.5), 0.1);
        

        Random rand = new Random();
        double[] doubles = new double[1000];
        
        for(int i = 0; i < doubles.Length; i++)
        {
            double a = rand.NextDouble();
            double b = rand.NextDouble();

            test.Add(new Interval(a, a + b), b);
        }

        for(int i = 0; i < doubles.Length; i++)
        {
            Interval removed = test.RemoveMax();
            doubles[i] = removed.end - removed.start;
            string print = String.Format("{0,-25} : {1,-25} ----> {2,-10}", removed.start, removed.end,  doubles[i]);
            Console.WriteLine(print);
        }

        Console.WriteLine($"OK? - {checkHeap(doubles)}");
        */


        ///////////////////////////////////////////////////
        heap = new Heap(100);
        StreamWriter sw = new StreamWriter("newData.txt");
        string line = "";

        LoadData("data.txt");
        
        for(int i = 0; i < 100_000; i++)
        {
            Interval removed = heap.RemoveMax();
            double mid = (removed.end + removed.start) / 2;
            line = $"{mid} {removed.end - removed.start}";

            heap.Add(new Interval(removed.start, mid), mid-removed.start);
            heap.Add(new Interval(mid, removed.end), removed.end-mid);
            Console.WriteLine(line);

            sw.WriteLine(line);
        }
        sw.Flush();
        sw.Close();
    }

    /// <summary>
    /// Metoda načte data ze souboru do haldy.
    /// </summary>
    /// <param name="path">Cesta k souboru s daty</param>
    public static void LoadData(string path)
    {
        string[] lines = File.ReadAllLines(path);
        double[] values = new double[lines.Length];

        for(int i = 0; i < values.Length; i++)
        {
            values[i] = Double.Parse(lines[i], CultureInfo.InvariantCulture);
        }

        for(int i = 0; i < values.Length - 1; i++)
        {
            heap.Add(new Interval(values[i], values[i+1]), values[i + 1] - values[i]);
        }
    }

    /// <summary>
    /// Metoda Zkontroluje, zda nejsou porušena pravidla haldy.
    /// </summary>
    /// <param name="doubles">Pole priorit prvků</param>
    /// <returns>True, pokud nejsou porušena, jinak False</returns>
    public static bool checkHeap(double[] doubles)
    {
        for (int i = 0; i < doubles.Length - 1; i++)
        {
            if (doubles[i] < doubles[i + 1])
            {
                return false;
            }
        }

        return true;
    }
}