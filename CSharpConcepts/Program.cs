// See https://aka.ms/new-console-template for more information

using CSharpConcepts.Features;

Func<string[], int> f = 1 switch
{
    1 => Enumerables.Execute,
    _ => throw new InvalidOperationException()
};

f.Invoke(args);
