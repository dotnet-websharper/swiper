// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2016 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}

IntelliFactory = {
    Runtime: {
        Ctor: function (ctor, typeFunction) {
            ctor.prototype = typeFunction.prototype;
            return ctor;
        },

        Class: function (members, base, statics) {
            var proto = members;
            if (base) {
                proto = new base();
                for (var m in members) { proto[m] = members[m] }
            }
            var typeFunction = function (copyFrom) {
                if (copyFrom) {
                    for (var f in copyFrom) { this[f] = copyFrom[f] }
                }
            }
            typeFunction.prototype = proto;
            if (statics) {
                for (var f in statics) { typeFunction[f] = statics[f] }
            }
            return typeFunction;
        },

        Clone: function (obj) {
            var res = {};
            for (var p in obj) { res[p] = obj[p] }
            return res;
        },

        NewObject:
            function (kv) {
                var o = {};
                for (var i = 0; i < kv.length; i++) {
                    o[kv[i][0]] = kv[i][1];
                }
                return o;
            },

        DeleteEmptyFields:
            function (obj, fields) {
                for (var i = 0; i < fields.length; i++) {
                    var f = fields[i];
                    if (obj[f] === void (0)) { delete obj[f]; }
                }
                return obj;
            },

        GetOptional:
            function (value) {
                return (value === void (0)) ? null : { $: 1, $0: value };
            },

        SetOptional:
            function (obj, field, value) {
                if (value) {
                    obj[field] = value.$0;
                } else {
                    delete obj[field];
                }
            },

        SetOrDelete:
            function (obj, field, value) {
                if (value === void (0)) {
                    delete obj[field];
                } else {
                    obj[field] = value;
                }
            },

        Apply: function (f, obj, args) {
            return f.apply(obj, args);
        },

        Bind: function (f, obj) {
            return function () { return f.apply(this, arguments) };
        },

        CreateFuncWithArgs: function (f) {
            return function () { return f(Array.prototype.slice.call(arguments)) };
        },

        CreateFuncWithOnlyThis: function (f) {
            return function () { return f(this) };
        },

        CreateFuncWithThis: function (f) {
            return function () { return f(this).apply(null, arguments) };
        },

        CreateFuncWithThisArgs: function (f) {
            return function () { return f(this)(Array.prototype.slice.call(arguments)) };
        },

        CreateFuncWithRest: function (length, f) {
            return function () { return f(Array.prototype.slice.call(arguments, 0, length).concat([Array.prototype.slice.call(arguments, length)])) };
        },

        CreateFuncWithArgsRest: function (length, f) {
            return function () { return f([Array.prototype.slice.call(arguments, 0, length), Array.prototype.slice.call(arguments, length)]) };
        },

        BindDelegate: function (func, obj) {
            var res = func.bind(obj);
            res.$Func = func;
            res.$Target = obj;
            return res;
        },

        CreateDelegate: function (invokes) {
            if (invokes.length == 0) return null;
            if (invokes.length == 1) return invokes[0];
            var del = function () {
                var res;
                for (var i = 0; i < invokes.length; i++) {
                    res = invokes[i].apply(null, arguments);
                }
                return res;
            };
            del.$Invokes = invokes;
            return del;
        },

        CombineDelegates: function (dels) {
            var invokes = [];
            for (var i = 0; i < dels.length; i++) {
                var del = dels[i];
                if (del) {
                    if ("$Invokes" in del)
                        invokes = invokes.concat(del.$Invokes);
                    else
                        invokes.push(del);
                }
            }
            return IntelliFactory.Runtime.CreateDelegate(invokes);
        },

        DelegateEqual: function (d1, d2) {
            if (d1 === d2) return true;
            if (d1 == null || d2 == null) return false;
            var i1 = d1.$Invokes || [d1];
            var i2 = d2.$Invokes || [d2];
            if (i1.length != i2.length) return false;
            for (var i = 0; i < i1.length; i++) {
                var e1 = i1[i];
                var e2 = i2[i];
                if (!(e1 === e2 || ("$Func" in e1 && "$Func" in e2 && e1.$Func === e2.$Func && e1.$Target == e2.$Target)))
                    return false;
            }
            return true;
        },

        ThisFunc: function (d) {
            return function () {
                var args = Array.prototype.slice.call(arguments);
                args.unshift(this);
                return d.apply(null, args);
            };
        },

        ThisFuncOut: function (f) {
            return function () {
                var args = Array.prototype.slice.call(arguments);
                return f.apply(args.shift(), args);
            };
        },

        ParamsFunc: function (length, d) {
            return function () {
                var args = Array.prototype.slice.call(arguments);
                return d.apply(null, args.slice(0, length).concat([args.slice(length)]));
            };
        },

        ParamsFuncOut: function (length, f) {
            return function () {
                var args = Array.prototype.slice.call(arguments);
                return f.apply(null, args.slice(0, length).concat(args[length]));
            };
        },

        ThisParamsFunc: function (length, d) {
            return function () {
                var args = Array.prototype.slice.call(arguments);
                args.unshift(this);
                return d.apply(null, args.slice(0, length + 1).concat([args.slice(length + 1)]));
            };
        },

        ThisParamsFuncOut: function (length, f) {
            return function () {
                var args = Array.prototype.slice.call(arguments);
                return f.apply(args.shift(), args.slice(0, length).concat(args[length]));
            };
        },

        Curried: function (f, n, args) {
            args = args || [];
            return function (a) {
                var allArgs = args.concat([a === void (0) ? null : a]);
                if (n == 1)
                    return f.apply(null, allArgs);
                if (n == 2)
                    return function (a) { return f.apply(null, allArgs.concat([a === void (0) ? null : a])); }
                return IntelliFactory.Runtime.Curried(f, n - 1, allArgs);
            }
        },

        Curried2: function (f) {
            return function (a) { return function (b) { return f(a, b); } }
        },

        Curried3: function (f) {
            return function (a) { return function (b) { return function (c) { return f(a, b, c); } } }
        },

        UnionByType: function (types, value, optional) {
            var vt = typeof value;
            for (var i = 0; i < types.length; i++) {
                var t = types[i];
                if (typeof t == "number") {
                    if (Array.isArray(value) && (t == 0 || value.length == t)) {
                        return { $: i, $0: value };
                    }
                } else {
                    if (t == vt) {
                        return { $: i, $0: value };
                    }
                }
            }
            if (!optional) {
                throw new Error("Type not expected for creating Choice value.");
            }
        },

        ScriptBasePath: "./",

        ScriptPath: function (a, f) {
            return this.ScriptBasePath + (this.ScriptSkipAssemblyDir ? "" : a + "/") + f;
        },

        OnLoad:
            function (f) {
                if (!("load" in this)) {
                    this.load = [];
                }
                this.load.push(f);
            },

        Start:
            function () {
                function run(c) {
                    for (var i = 0; i < c.length; i++) {
                        c[i]();
                    }
                }
                if ("load" in this) {
                    run(this.load);
                    this.load = [];
                }
            },
    }
}

IntelliFactory.Runtime.OnLoad(function () {
    if (self.WebSharper && WebSharper.Activator && WebSharper.Activator.Activate)
        WebSharper.Activator.Activate()
});

// Polyfill

if (!Date.now) {
    Date.now = function () {
        return new Date().getTime();
    };
}

if (!Math.trunc) {
    Math.trunc = function (x) {
        return x < 0 ? Math.ceil(x) : Math.floor(x);
    }
}

if (!Object.setPrototypeOf) {
  Object.setPrototypeOf = function (obj, proto) {
    obj.__proto__ = proto;
    return obj;
  }
}

function ignore() { };
function id(x) { return x };
function fst(x) { return x[0] };
function snd(x) { return x[1] };
function trd(x) { return x[2] };

if (!console) {
    console = {
        count: ignore,
        dir: ignore,
        error: ignore,
        group: ignore,
        groupEnd: ignore,
        info: ignore,
        log: ignore,
        profile: ignore,
        profileEnd: ignore,
        time: ignore,
        timeEnd: ignore,
        trace: ignore,
        warn: ignore
    }
};
(function()
{
 "use strict";
 var Global,WebSharper,Swiper,Tests,Client,Operators,Testing,Runner,EventTarget,Node,Obj,RunnerControlBody,Arrays,SC$1,WindowOrWorkerGlobalScope,Enumerator,Pervasives,TestCategoryBuilder,SubtestBuilder,TestBuilder,Unchecked,Runner$1,T,Concurrency,AsyncBody,CT,Scheduler,SC$2,Error,OperationCanceledException,CancellationTokenSource,QUnit,IntelliFactory,Runtime,console,Date;
 Global=self;
 WebSharper=Global.WebSharper=Global.WebSharper||{};
 Swiper=WebSharper.Swiper=WebSharper.Swiper||{};
 Tests=Swiper.Tests=Swiper.Tests||{};
 Client=Tests.Client=Tests.Client||{};
 Operators=WebSharper.Operators=WebSharper.Operators||{};
 Testing=WebSharper.Testing=WebSharper.Testing||{};
 Runner=Testing.Runner=Testing.Runner||{};
 EventTarget=Global.EventTarget;
 Node=Global.Node;
 Obj=WebSharper.Obj=WebSharper.Obj||{};
 RunnerControlBody=Runner.RunnerControlBody=Runner.RunnerControlBody||{};
 Arrays=WebSharper.Arrays=WebSharper.Arrays||{};
 SC$1=Global.StartupCode$WebSharper_Swiper_Tests$Client=Global.StartupCode$WebSharper_Swiper_Tests$Client||{};
 WindowOrWorkerGlobalScope=Global.WindowOrWorkerGlobalScope;
 Enumerator=WebSharper.Enumerator=WebSharper.Enumerator||{};
 Pervasives=Testing.Pervasives=Testing.Pervasives||{};
 TestCategoryBuilder=Pervasives.TestCategoryBuilder=Pervasives.TestCategoryBuilder||{};
 SubtestBuilder=Pervasives.SubtestBuilder=Pervasives.SubtestBuilder||{};
 TestBuilder=Pervasives.TestBuilder=Pervasives.TestBuilder||{};
 Unchecked=WebSharper.Unchecked=WebSharper.Unchecked||{};
 Runner$1=Pervasives.Runner=Pervasives.Runner||{};
 T=Enumerator.T=Enumerator.T||{};
 Concurrency=WebSharper.Concurrency=WebSharper.Concurrency||{};
 AsyncBody=Concurrency.AsyncBody=Concurrency.AsyncBody||{};
 CT=Concurrency.CT=Concurrency.CT||{};
 Scheduler=Concurrency.Scheduler=Concurrency.Scheduler||{};
 SC$2=Global.StartupCode$WebSharper_Main$Concurrency=Global.StartupCode$WebSharper_Main$Concurrency||{};
 Error=Global.Error;
 OperationCanceledException=WebSharper.OperationCanceledException=WebSharper.OperationCanceledException||{};
 CancellationTokenSource=WebSharper.CancellationTokenSource=WebSharper.CancellationTokenSource||{};
 QUnit=Global.QUnit;
 IntelliFactory=Global.IntelliFactory;
 Runtime=IntelliFactory&&IntelliFactory.Runtime;
 console=Global.console;
 Date=Global.Date;
 Client.RunTests=function()
 {
  Runner.RunTests([Client.Tests()]).ReplaceInDom(self.document.querySelector("#container"));
 };
 Client.Tests=function()
 {
  SC$1.$cctor();
  return SC$1.Tests;
 };
 Operators.FailWith=function(msg)
 {
  throw new Error(msg);
 };
 Runner.RunTests=function(tests)
 {
  return new RunnerControlBody.New(function()
  {
   var e,t;
   e=Enumerator.Get(tests);
   try
   {
    while(e.MoveNext())
     {
      t=e.Current();
      QUnit.module(t.name);
      t.run();
     }
   }
   finally
   {
    if(typeof e=="object"&&"Dispose"in e)
     e.Dispose();
   }
  });
 };
 Obj=WebSharper.Obj=Runtime.Class({
  Equals:function(obj)
  {
   return this===obj;
  }
 },null,Obj);
 Obj.New=Runtime.Ctor(function()
 {
 },Obj);
 RunnerControlBody=Runner.RunnerControlBody=Runtime.Class({
  ReplaceInDom:function(e)
  {
   var fixture,qunit,parent;
   Unchecked.Equals(self.document.querySelector("#qunit"),null)?(fixture=self.document.createElement("div"),fixture.setAttribute("id","qunit-fixture"),qunit=self.document.createElement("div"),qunit.setAttribute("id","qunit"),parent=e.parentNode,parent.replaceChild(fixture,e),parent.insertBefore(qunit,fixture)):void 0;
   this.run();
  }
 },Obj,RunnerControlBody);
 RunnerControlBody.New=Runtime.Ctor(function(run)
 {
  Obj.New.call(this);
  this.run=run;
 },RunnerControlBody);
 Arrays.get=function(arr,n)
 {
  Arrays.checkBounds(arr,n);
  return arr[n];
 };
 Arrays.length=function(arr)
 {
  return arr.dims===2?arr.length*arr.length:arr.length;
 };
 Arrays.checkBounds=function(arr,n)
 {
  if(n<0||n>=arr.length)
   Operators.FailWith("Index was outside the bounds of the array.");
 };
 SC$1.$cctor=function()
 {
  var swiper,s;
  SC$1.$cctor=Global.ignore;
  SC$1.Tests=(swiper=new Global.Swiper(".swiper-container"),(s={
   name:Pervasives.TestCategory("General").name,
   run:function()
   {
    var b,b$1;
    b=Pervasives.Test("SlideTo test");
    b.Run(b.EqualMsg(b.For(b.EqualMsg(b.For(b.EqualMsg(b.For(b.EqualMsg((swiper.slideTo(0),b.Yield()),function()
    {
     return swiper.activeIndex;
    },function()
    {
     return 0;
    },"SlideTo 0"),function()
    {
     swiper.slideTo(4);
     return b.Yield();
    }),function()
    {
     return swiper.activeIndex;
    },function()
    {
     return 4;
    },"SlideTo 4"),function()
    {
     swiper.slideTo(3);
     return b.Yield();
    }),function()
    {
     return swiper.activeIndex;
    },function()
    {
     return 3;
    },"SlideTo 3"),function()
    {
     swiper.slideTo(0);
     return b.Yield();
    }),function()
    {
     return swiper.activeIndex;
    },function()
    {
     return 0;
    },"SlideTo 0"));
    b$1=Pervasives.Test("Sliding around the start of the slides");
    b$1.Run(b$1.EqualMsg(b$1.For(b$1.EqualMsg(b$1.For(b$1.Equal((swiper.slideTo(0),b$1.Yield()),function()
    {
     return swiper.activeIndex;
    },function()
    {
     return 0;
    }),function()
    {
     swiper.slidePrev();
     return b$1.Yield();
    }),function()
    {
     return swiper.activeIndex;
    },function()
    {
     return 0;
    },"Can not swipe left from the first slide"),function()
    {
     swiper.slideTo(0);
     swiper.slideNext();
     return b$1.Yield();
    }),function()
    {
     return swiper.activeIndex;
    },function()
    {
     return 1;
    },"Slide right from the first slide"));
   }
  },(QUnit.module(s.name),s)));
 };
 Enumerator.Get=function(x)
 {
  return x instanceof Global.Array?Enumerator.ArrayEnumerator(x):Unchecked.Equals(typeof x,"string")?Enumerator.StringEnumerator(x):x.GetEnumerator();
 };
 Enumerator.ArrayEnumerator=function(s)
 {
  return new T.New(0,null,function(e)
  {
   var i;
   i=e.s;
   return i<Arrays.length(s)&&(e.c=Arrays.get(s,i),e.s=i+1,true);
  },void 0);
 };
 Enumerator.StringEnumerator=function(s)
 {
  return new T.New(0,null,function(e)
  {
   var i;
   i=e.s;
   return i<s.length&&(e.c=s[i],e.s=i+1,true);
  },void 0);
 };
 Pervasives.TestCategory=function(name)
 {
  return new TestCategoryBuilder.New(name);
 };
 Pervasives.Test=function(name)
 {
  return new TestBuilder.New(function(c)
  {
   QUnit.test(name,c);
  });
 };
 TestCategoryBuilder=Pervasives.TestCategoryBuilder=Runtime.Class({},Obj,TestCategoryBuilder);
 TestCategoryBuilder.New=Runtime.Ctor(function(name)
 {
  Obj.New.call(this);
  this.name=name;
 },TestCategoryBuilder);
 SubtestBuilder=Pervasives.SubtestBuilder=Runtime.Class({
  EqualMsg:function(r,actual,expected,message)
  {
   function t(asserter,args)
   {
    var actual$1,expected$1;
    actual$1=actual(args);
    expected$1=expected(args);
    return asserter.push(Unchecked.Equals(actual$1,expected$1),actual$1,expected$1,message);
   }
   return function(a)
   {
    return Runner$1.AddTest(t,r,a);
   };
  },
  For:function(r,y)
  {
   return function(asserter)
   {
    var m,a,b;
    m=r(asserter);
    return m.$==1?(a=m.$0,{
     $:1,
     $0:(b=null,Concurrency.Delay(function()
     {
      return Concurrency.Bind(a,function(a$1)
      {
       var m$1;
       m$1=(y(a$1))(asserter);
       return m$1.$==1?m$1.$0:Concurrency.Return(m$1.$0);
      });
     }))
    }):(y(m.$0))(asserter);
   };
  },
  Yield:function(x)
  {
   return function()
   {
    return{
     $:0,
     $0:x
    };
   };
  },
  Equal:function(r,actual,expected)
  {
   function t(asserter,args)
   {
    var actual$1,expected$1;
    actual$1=actual(args);
    expected$1=expected(args);
    return asserter.push(Unchecked.Equals(actual$1,expected$1),actual$1,expected$1);
   }
   return function(a)
   {
    return Runner$1.AddTest(t,r,a);
   };
  }
 },Obj,SubtestBuilder);
 SubtestBuilder.New=Runtime.Ctor(function()
 {
  Obj.New.call(this);
 },SubtestBuilder);
 TestBuilder=Pervasives.TestBuilder=Runtime.Class({
  Run:function(e)
  {
   this.run(function(asserter)
   {
    var m,asy,done,b;
    try
    {
     m=e(asserter);
     m.$==1?(asy=m.$0,(done=asserter.async(),Concurrency.Start((b=null,Concurrency.Delay(function()
     {
      return Concurrency.TryFinally(Concurrency.Delay(function()
      {
       return Concurrency.TryWith(Concurrency.Delay(function()
       {
        return Concurrency.Bind(Runner$1.WithTimeout(1000,asy),function()
        {
         return Concurrency.Return(null);
        });
       }),function(a)
       {
        asserter.equal(a,null,"Test threw an unexpected asynchronous exception");
        return Concurrency.Return(null);
       });
      }),function()
      {
       done();
      });
     })),null))):null;
    }
    catch(e$1)
    {
     asserter.equal(e$1,null,"Test threw an unexpected synchronous exception");
    }
   });
  }
 },SubtestBuilder,TestBuilder);
 TestBuilder.New=Runtime.Ctor(function(run)
 {
  SubtestBuilder.New.call(this);
  this.run=run;
 },TestBuilder);
 Unchecked.Equals=function(a,b)
 {
  var m,eqR,k,k$1;
  if(a===b)
   return true;
  else
   {
    m=typeof a;
    if(m=="object")
    {
     if(a===null||a===void 0||b===null||b===void 0)
      return false;
     else
      if("Equals"in a)
       return a.Equals(b);
      else
       if(a instanceof Global.Array&&b instanceof Global.Array)
        return Unchecked.arrayEquals(a,b);
       else
        if(a instanceof Global.Date&&b instanceof Global.Date)
         return Unchecked.dateEquals(a,b);
        else
         {
          eqR=[true];
          for(var k$2 in a)if(function(k$3)
          {
           eqR[0]=!a.hasOwnProperty(k$3)||b.hasOwnProperty(k$3)&&Unchecked.Equals(a[k$3],b[k$3]);
           return!eqR[0];
          }(k$2))
           break;
          if(eqR[0])
           {
            for(var k$3 in b)if(function(k$4)
            {
             eqR[0]=!b.hasOwnProperty(k$4)||a.hasOwnProperty(k$4);
             return!eqR[0];
            }(k$3))
             break;
           }
          return eqR[0];
         }
    }
    else
     return m=="function"&&("$Func"in a?a.$Func===b.$Func&&a.$Target===b.$Target:"$Invokes"in a&&"$Invokes"in b&&Unchecked.arrayEquals(a.$Invokes,b.$Invokes));
   }
 };
 Unchecked.arrayEquals=function(a,b)
 {
  var eq,i;
  if(Arrays.length(a)===Arrays.length(b))
   {
    eq=true;
    i=0;
    while(eq&&i<Arrays.length(a))
     {
      !Unchecked.Equals(Arrays.get(a,i),Arrays.get(b,i))?eq=false:void 0;
      i=i+1;
     }
    return eq;
   }
  else
   return false;
 };
 Unchecked.dateEquals=function(a,b)
 {
  return a.getTime()===b.getTime();
 };
 Runner$1.WithTimeout=function(timeOut,a)
 {
  return a;
 };
 Runner$1.AddTest=function(t,r,asserter)
 {
  return Runner$1.Map(function(args)
  {
   t(asserter,args);
   return args;
  },r(asserter));
 };
 Runner$1.Map=function(f,x)
 {
  var args,b;
  return x.$==1?(args=x.$0,{
   $:1,
   $0:(b=null,Concurrency.Delay(function()
   {
    return Concurrency.Bind(args,function(a)
    {
     return Concurrency.Return(f(a));
    });
   }))
  }):{
   $:0,
   $0:f(x.$0)
  };
 };
 T=Enumerator.T=Runtime.Class({
  MoveNext:function()
  {
   return this.n(this);
  },
  Current:function()
  {
   return this.c;
  },
  Dispose:function()
  {
   if(this.d)
    this.d(this);
  }
 },Obj,T);
 T.New=Runtime.Ctor(function(s,c,n,d)
 {
  Obj.New.call(this);
  this.s=s;
  this.c=c;
  this.n=n;
  this.d=d;
 },T);
 Concurrency.Delay=function(mk)
 {
  return function(c)
  {
   try
   {
    (mk(null))(c);
   }
   catch(e)
   {
    c.k({
     $:1,
     $0:e
    });
   }
  };
 };
 Concurrency.TryFinally=function(run,f)
 {
  return function(c)
  {
   run(AsyncBody.New(function(r)
   {
    try
    {
     f();
     c.k(r);
    }
    catch(e)
    {
     c.k({
      $:1,
      $0:e
     });
    }
   },c.ct));
  };
 };
 Concurrency.TryWith=function(r,f)
 {
  return function(c)
  {
   r(AsyncBody.New(function(a)
   {
    if(a.$==0)
     c.k({
      $:0,
      $0:a.$0
     });
    else
     if(a.$==1)
      try
      {
       (f(a.$0))(c);
      }
      catch(e)
      {
       c.k(a);
      }
     else
      c.k(a);
   },c.ct));
  };
 };
 Concurrency.Bind=function(r,f)
 {
  return Concurrency.checkCancel(function(c)
  {
   r(AsyncBody.New(function(a)
   {
    var x;
    if(a.$==0)
     {
      x=a.$0;
      Concurrency.scheduler().Fork(function()
      {
       try
       {
        (f(x))(c);
       }
       catch(e)
       {
        c.k({
         $:1,
         $0:e
        });
       }
      });
     }
    else
     Concurrency.scheduler().Fork(function()
     {
      c.k(a);
     });
   },c.ct));
  });
 };
 Concurrency.Return=function(x)
 {
  return function(c)
  {
   c.k({
    $:0,
    $0:x
   });
  };
 };
 Concurrency.Start=function(c,ctOpt)
 {
  var ct,d;
  ct=(d=(Concurrency.defCTS())[0],ctOpt==null?d:ctOpt.$0);
  Concurrency.scheduler().Fork(function()
  {
   if(!ct.c)
    c(AsyncBody.New(function(a)
    {
     if(a.$==1)
      Concurrency.UncaughtAsyncError(a.$0);
    },ct));
  });
 };
 Concurrency.checkCancel=function(r)
 {
  return function(c)
  {
   if(c.ct.c)
    Concurrency.cancel(c);
   else
    r(c);
  };
 };
 Concurrency.defCTS=function()
 {
  SC$2.$cctor();
  return SC$2.defCTS;
 };
 Concurrency.UncaughtAsyncError=function(e)
 {
  console.log("WebSharper: Uncaught asynchronous exception",e);
 };
 Concurrency.cancel=function(c)
 {
  c.k({
   $:2,
   $0:new OperationCanceledException.New(c.ct)
  });
 };
 Concurrency.scheduler=function()
 {
  SC$2.$cctor();
  return SC$2.scheduler;
 };
 AsyncBody.New=function(k,ct)
 {
  return{
   k:k,
   ct:ct
  };
 };
 CT.New=function(IsCancellationRequested,Registrations)
 {
  return{
   c:IsCancellationRequested,
   r:Registrations
  };
 };
 Scheduler=Concurrency.Scheduler=Runtime.Class({
  Fork:function(action)
  {
   var $this;
   $this=this;
   this.robin.push(action);
   this.idle?(this.idle=false,Global.setTimeout(function()
   {
    $this.tick();
   },0)):void 0;
  },
  tick:function()
  {
   var loop,$this,t;
   $this=this;
   t=Date.now();
   loop=true;
   while(loop)
    if(this.robin.length===0)
     {
      this.idle=true;
      loop=false;
     }
    else
     {
      (this.robin.shift())();
      Date.now()-t>40?(Global.setTimeout(function()
      {
       $this.tick();
      },0),loop=false):void 0;
     }
  }
 },Obj,Scheduler);
 Scheduler.New=Runtime.Ctor(function()
 {
  Obj.New.call(this);
  this.idle=true;
  this.robin=[];
 },Scheduler);
 SC$2.$cctor=function()
 {
  SC$2.$cctor=Global.ignore;
  SC$2.noneCT=CT.New(false,[]);
  SC$2.scheduler=new Scheduler.New();
  SC$2.defCTS=[new CancellationTokenSource.New()];
  SC$2.Zero=Concurrency.Return();
  SC$2.GetCT=function(c)
  {
   c.k({
    $:0,
    $0:c.ct
   });
  };
 };
 OperationCanceledException=WebSharper.OperationCanceledException=Runtime.Class({},Error,OperationCanceledException);
 OperationCanceledException.New=Runtime.Ctor(function(ct)
 {
  OperationCanceledException.New$1.call(this,"The operation was canceled.",null,ct);
 },OperationCanceledException);
 OperationCanceledException.New$1=Runtime.Ctor(function(message,inner,ct)
 {
  this.message=message;
  this.inner=inner;
  Global.Object.setPrototypeOf(this,OperationCanceledException.prototype);
  this.ct=ct;
 },OperationCanceledException);
 CancellationTokenSource=WebSharper.CancellationTokenSource=Runtime.Class({},Obj,CancellationTokenSource);
 CancellationTokenSource.New=Runtime.Ctor(function()
 {
  Obj.New.call(this);
  this.c=false;
  this.pending=null;
  this.r=[];
  this.init=1;
 },CancellationTokenSource);
 Runtime.OnLoad(function()
 {
  Client.RunTests();
 });
}());


if (typeof IntelliFactory !=='undefined') {
  IntelliFactory.Runtime.ScriptBasePath = '/Content/';
  IntelliFactory.Runtime.Start();
}
