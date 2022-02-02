using System;
using System.Diagnostics;
using System.Linq;
using Alex.MoLang.Attributes;
using Alex.MoLang.Parser;
using Alex.MoLang.Runtime;
using Alex.MoLang.Runtime.Struct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alex.MoLang.Tests;

[TestClass]
public class ScriptTest
{
    private Stopwatch _sw;
    private MoLangRuntime _runtime;
    [TestInitialize]
    public void Setup()
    {
        _sw = new Stopwatch();
        _runtime = new MoLangRuntime();
        _runtime.Environment.Structs.TryAdd("query", new InteropStruct(new TestClass(_sw)) { });
    }
    
    [TestMethod("Script Execution")]
    public void ScriptRun()
    {
        _sw.Start();
        var parsed = MoLangParser.Parse(@"
        t.a = 213 + 2 / 0.5 + 5 + 2 * 3;

        query.debug_output(1 + 2 * 3);

        array.test.0 = 100;
        array.test[1] = 200;
        array.test[2] = 10.5;

        query.debug_output(array.test[1]);

        t.b = 3;

        loop(10, {
	        array.test[t.b] = array.test[t.b - 1] + 2;
	        t.b = t.b + 1;
        });

        for_each(v.r, array.test, {
            t.a = t.a + v.r;
            query.debug_output('hello1', t.a, v.r);
        });

        t.b = 0;

        loop(100, {
            t.b = t.b + 1;
            t.a = t.a + math.cos((Math.PI / 180.0f) * 270) + t.b;
            query.debug_output(array.test[t.b]);
            query.debug_output('hello', 'test', t.a, array.test[2], t.b);
        });

        query.debug_output(query.life_time());
        return t.a;
");
        _sw.Stop();
        var timeElapsedOnParsing = _sw.Elapsed;
        
        Assert.IsNotNull(parsed);
        
        Console.WriteLine($"Parser completed in {timeElapsedOnParsing.TotalMilliseconds}ms");
        
        const int runs = 1000;
        const int warmupRuns = 10;
        
        TimeSpan totalElapsed = TimeSpan.Zero;
        TimeSpan max = TimeSpan.Zero;
        TimeSpan min = TimeSpan.MaxValue;
        
        for (int i = 0; i < runs + warmupRuns; i++)
        {
            _sw.Restart();
            _runtime.Execute(parsed);
            _sw.Stop();

            if (i >= warmupRuns)
            {
                totalElapsed += _sw.Elapsed;

                if (_sw.Elapsed < min)
                    min = _sw.Elapsed;

                if (_sw.Elapsed > max)
                    max = _sw.Elapsed;
            }
        }
        
        Console.WriteLine($"Executed {runs} runs. Total={totalElapsed.TotalMilliseconds}ms Avg={totalElapsed.TotalMilliseconds / runs}ms Max={max.TotalMilliseconds}ms Min={min.TotalMilliseconds}ms");
    }

    public class TestClass
    {
        private Stopwatch _sw;
        public TestClass(Stopwatch sw)
        {
            _sw = sw;
        }

        [MoProperty("life_time")]
        public double Lifetime
        {
            get
            {
                return _sw.Elapsed.TotalSeconds;
            }
        }

        [MoFunction("debug_output")]
        public void DebugOutput(MoParams mo)
        {
          //  var str = string.Join(" ", mo.GetParams().Select(x => x?.AsString()));
          //  Console.WriteLine(str);
        }
    }
}