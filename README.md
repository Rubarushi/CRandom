# CRandom
A pseudo random number generator for Windows  
Added Gatcha Function
# Usuage

> Program Code  
```cs
CRandom random = new CRandom();
random.Randomize();
random.AddItem(1, 1);
random.AddItem(2, 30);
random.AddItem(3, 50);
random.AddItem(4, 70);
random.AddItem(5, 20);
random.AddItem(5, 70);
random.AddItem(6, 100);

int[] a = new int[6];

for(int i = 0; i < 10000; i++)
{
    var b = random.GetItem();
    a[b]++;
}
            
for(int i = 0; i < a.Length; i++)
{
    Console.WriteLine(i+ 1 + ":" + a[i]);
}
```
