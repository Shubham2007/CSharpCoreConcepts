using static System.Console;

namespace CSharpConcepts.Features
{
    internal static class Enumerables
    {
        public static int Execute(string[] args)
        {
            Action action = 2 switch
            {
                1 => Syntax,
                2 => Issues,
                _ => throw new InvalidOperationException()
            };

            action.Invoke();
            return 0;
        }

        private static void Log<T>(string name, IEnumerable<T> items)
        {
            var stringValues = string.Join(",", items);
            WriteLine($"{name}: {stringValues}");
        }

        public static void Syntax()
        {
            IEnumerable<string> items1 = new[] { "abc", "cde" };
            var items2 = new[] { "f", "g" };

            Log("empty", Yield(-1));
            Log("some", Yield(2));
            Log("break3", Break(3));
            Log("yield break3", YieldBreak(3));
            Log("yield break5", YieldBreak(5));
        }       
      
        private static IEnumerable<object> Yield(int data)
        {
            if (data > 0)
            {
                yield return 1;
            }
        }

        private static IEnumerable<object> Break(int limit)
        {
            var items = new List<string>();

            for(int i = 0; i < 5; i++)
            {
                if (i > limit)
                {
                    //return items
                    break;
                }
                items.Add(i.ToString());
            }
            items.Add("LastOne");
            WriteLine("Hello");
            return items;
        }

        private static IEnumerable<object> YieldBreak(int limit)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i > limit)
                {
                    yield break; //exit the method
                }
                yield return i.ToString();
            }
            yield return  "LastOne";
            WriteLine("Hello");
        }

        public static void Issues()
        {
            DefferedExecution();
            DefferedExecutionExt();
            MultipleIteration();
            Log("Enumerable I am:", CapturedContext());
            Log("Enumerable I am:", CapturedContextExt());
            Log("Collection I am:", CapturedContextReadOnlyCollection());
        }

        private static void DefferedExecution()
        {
            var values = new[] { "abc", "cde" };
            var result = values.Select(x => x.Length);
            values[0] = "a";
            Log("deffered", result);
            values[0] = "ab";
            Log("deffered", result);
        }

        private static void DefferedExecutionExt()
        {
            var values = new[] { "abc", "cde" };
            var result = values.Select(x => x.Length).ToList();
            values[0] = "a";
            Log("deffered", result);
            values[0] = "ab";
            Log("deffered", result);
        }

        private static void MultipleIteration()
        {
            var values = GetValues(1).ToList();
            var count = values.Count(); // first iteration
            if (count > 0)
            {
                Log("values", values); // second iteration
            }
        }

        private static IEnumerable<string> GetValues(int sleepSeconds)
        {
            //return new List<string>(new string[] { "abs", "aas" });

            WriteLine("Getting value1");
            Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));
            yield return "value1";

            WriteLine("Getting value2");
            Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));
            yield return "value2";
        }

        class Disp : IDisposable
        {
            public string Value { get; set; }
            public Disp(string value) { Value = value; }
            public void Dispose() { Value = "DEAD"; }
        }

        private static IEnumerable<string> CapturedContext()
        {
            var inputs = new[] { 1, 2, 3 };
            using var connectionToDB = new Disp("Alive");
            // Something that is capturing disposable
            return inputs.Select(item =>
                connectionToDB.Value);
            //here disposable has been disposed but iteration has not been started
        }

        private static IEnumerable<string> CapturedContextExt()
        {
            var inputs = new[] { 1, 2, 3 };
            using var connectionToDB = new Disp("Alive");
            foreach(int item in inputs)
            {
                yield return connectionToDB.Value;
            }
        }

        private static IReadOnlyCollection<string> CapturedContextReadOnlyCollection()
        {
            var inputs = new[] { 1, 2, 3 };
            using var connectionToDB = new Disp("Alive");
            return inputs.Select(item =>
                connectionToDB.Value).ToList(); //does not compile witout ToList() which forces iteration
        }
    }
}
