using static System.Console;

namespace CSharpConcepts.Features
{
    internal static class FuncAndActions
    {
        public static int Execute(string[] args)
        {
            Action action = 1 switch
            {
                1 => Syntax,
                2 => () => MethodToBeOptimised(0, 1),
                3 => () => Optimised(0, 1),
                _ => throw new NotSupportedException()
            };

            action.Invoke();

            return 0;
        }

        public static void Syntax()
        {
            Func<string[], int> StringArrayParameterReturnsInt = Execute;
            Action NoParamaeterNoReturn = F1;
            Func<double> NoParameterReturnDouble = F2;

            string[] param = new[] { "abc" };
            NoParamaeterNoReturn = () => { Execute(param); }; // Closure - simple but might have side effects(we are passing local state)
            NoParamaeterNoReturn(); // Invocation - no parameters needed

            var b = (string[] p) => { Execute(p); }; // Alternative to closure - explicit parameter
            b(new[] { "abc" }); // Invocation - a value has to passed in
            b(new[] { "cde" }); // more flexible as caller can pass another value
        }

        private static void F1() { WriteLine("F1"); }

        private static double F2() { WriteLine("F2"); return 1.0; }

        public static void MethodToBeOptimised(int a, int b)
        {
            if (a == 1)
            {
                //Do Something1
                F1();
                //Do Something2
            }

            if (b == 2)
            {
                //Do Something1
                F2();
                //Do Something2
            }
        }

        public static void Optimised(int a, int b)
        {
            DoCommon(a, F1);
            DoCommon(b, () => F2()); // Need to adapt F2 as it is Func<double> instead of Action
        }

        private static void DoCommon(int parameter, Action action)
        {
            //Do Something1
            action.Invoke();
            //Do Something2
        }

        public static double MethodToBeOptimised2(int a, int b)
        {
            double d = 1;

            {
                if (a == 1)
                {
                    //Do Something1
                    F1();
                    //Do Something2
                }
            }

            {
                if (b == 2)
                {
                    //Do Something1
                    F2();
                    //Do Something2
                }
            }

            return d;
        }

        public static void Optimised2(int a, int b)
        {
            double d = 1;
            DoCommon2(a, d, () => { F1(); return d; }); // Need to adapt F1 as it is Action instead of Func<double>
            d = DoCommon2(b, d, F2); 
        }

        private static double DoCommon2(int parameter, double initialParam, Func<double> action)
        {
            var g = initialParam;
            if (parameter == 1)
            {
                //Do Something1
                g = action.Invoke();
                //Do Something2
            }

            return g;
        }
    }
}
