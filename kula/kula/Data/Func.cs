﻿using System;
using System.Collections.Generic;

using kula.Util;
using kula.Core;

namespace kula.Data
{
    delegate void KvmBuiltinFunc(object[] args, Stack<object> stack);
    class Func  // : IRunnable
    {
        // 静态内置方法 们
        public static Dictionary<string, KvmBuiltinFunc> BuiltinFunc { get => builtinFunc; }
        private static readonly Dictionary<string, KvmBuiltinFunc> builtinFunc = new Dictionary<string, KvmBuiltinFunc>()
        {
            // Float
            {"plus", (args, stack) => {
                ArgsCheck(args, new Type[]{typeof(float), typeof(float)});
                stack.Push((float)args[0] + (float)args[1]);
            } },
            {"minus", (args, stack) => {
                ArgsCheck(args, new Type[]{typeof(float), typeof(float)});
                stack.Push((float)args[0] - (float)args[1]);
            } },
            {"times", (args, stack) => {
                ArgsCheck(args, new Type[]{typeof(float), typeof(float)});
                stack.Push((float)args[0] * (float)args[1]);
            } },
            {"div", (args, stack) => {
                ArgsCheck(args, new Type[]{typeof(float), typeof(float)});
                stack.Push((float)args[0] / (float)args[1]);
            } },

            // IO
            {"print", (args, stack) => {
                foreach (var arg in args)
                {
                    Console.Write( arg );
                }
            } },
            {"println", (args, stack) => {
                foreach (var arg in args)
                {
                    Console.WriteLine( arg );
                }
            } },
            {"input", (args, stack) =>{
                stack.Push(Console.ReadLine());
            } },
            
            // String
            {"toStr", (args, stack) => {
                stack.Push(args[0].ToString());
            } },
            {"toNum", (args, stack) => {
                var arg = args[0];
                if (arg.GetType() != typeof(string))
                    throw new KulaException.FuncException();
                float.TryParse((string)arg, out float ans);
                stack.Push(ans);
            } },
            {"len", (args, stack) => {
                ArgsCheck(args, new Type[] { typeof(string) });
                stack.Push((float)((string)args[0]).Length);
            } },
            {"cut", (args, stack) => {
                ArgsCheck(args, new Type[] { typeof(string), typeof(float), typeof(float) });
                stack.Push(((string)args[0]).Substring((int)(float)args[1], (int)(float)args[2]));
            } },
            {"concat", (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(string), typeof(string) });
                stack.Push((string)args[0] + (string)args[1]);
            } },
            {"type", (args, stack)=> {
                var arg_type = args[0].GetType();
                switch (Type.GetTypeCode(arg_type))
                {
                    case TypeCode.Single:
                        stack.Push("Num");
                        break;
                    case TypeCode.String:
                        stack.Push("Str");
                        break;
                    default:
                        if(arg_type == typeof(FuncEnv))
                            stack.Push("Func");
                        else if (arg_type == typeof(Array))
                            stack.Push("Array");
                        else if (arg_type == typeof(Map))
                            stack.Push("Map");
                        else
                            stack.Push("None");
                        break;
                }
            } },

            // Bool
            {"equal", (args, stack)=> {
                stack.Push( object.Equals(args[0], args[1]) ? 1f : 0f);
            } },
            {"greater", (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(float), typeof(float) });
                stack.Push( ((float)args[0] > (float)args[1]) ? 1f : 0f);
            } },
            {"less",  (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(float), typeof(float) });
                stack.Push( ((float)args[0] < (float)args[1]) ? 1f : 0f);
            } },
            {"and",  (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(float), typeof(float) });
                bool flag = ((float)args[0] != 0) && ((float)args[1] != 0);
                stack.Push(flag ? 1f : 0f);
            } },
            {"or", (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(float), typeof(float) });
                bool flag = ((float)args[0] != 0) || ((float)args[1] != 0);
                stack.Push(flag ? 1f : 0f);
            } },
            {"not",  (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(float) });
                stack.Push((float)args[0] == 0f ? 1f : 0f);
            } },

            // Array
            {"newArray", (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(float) });
                Array tmp = new Array((int)(float)args[0]);
                stack.Push(tmp);
            } },
            {"fill", (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(Array), typeof(float), typeof(object) });
                ((Array)args[0])[(int)(float)args[1]] = args[2];
            } },

            // Map
            {"newMap", (args, stack)=>{
                Map tmp_map = new Map();
                stack.Push(tmp_map);
            } },
            {"set", (args, stack)=> {
                ArgsCheck(args, new Type[] { typeof(Map), typeof(string), typeof(object) });
                ((Map)args[0])[(string)args[1]] = args[2];
            } },
        };

        private static void ArgsCheck(object[] args, Type[] types)
        {
            bool flag = args.Length == types.Length;
            for (int i = 0; i < args.Length && flag; i++)
            {
                flag = types[i] == typeof(object) || args[i].GetType() == types[i];
            }
            if (flag == false) throw new KulaException.FuncException();
        }

        // 接口儿
        public List<LexToken> TokenStream { get => tokenStream; }
        public List<KvmNode> NodeStream { get => nodeStream; }
        public bool Compiled { get => compiled; set => compiled = true; }
        public List<Type> ArgTypes { get => argTypes; }
        public List<string> ArgNames { get => argNames; }
        public Type ReturnType { get => returnType; set => returnType = value; }


        private bool compiled;
        private readonly List<LexToken> tokenStream;
        private readonly List<KvmNode> nodeStream;

        private readonly List<Type> argTypes;
        private readonly List<string> argNames;
        private Type returnType;

        public Func(List<LexToken> tokenStream)
        {
            this.tokenStream = tokenStream;

            this.argTypes = new List<Type>();
            this.argNames = new List<string>();
            this.nodeStream = new List<KvmNode>();
        }

        public override string ToString()
        {
            return "{Func" + returnType ?? ( ":" + returnType.ToString() )+ "}";
        }
    }
}
