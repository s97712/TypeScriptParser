using System;
using TypeScriptParser.TreeSitter;
using System.Runtime.InteropServices;

namespace TypeScriptParser
{
    /// <summary>
    /// TypeScript 解析器模块 - 负责 Tree-sitter TypeScript 语言的初始化和基础解析功能
    /// </summary>
    public class Parser : IDisposable
    {
        private readonly TSLanguage lang;
        private TSParser parser;
        private bool disposed = false;

        [DllImport("tree-sitter-typescript", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tree_sitter_typescript();

        public Parser()
        {
            lang = new TSLanguage(tree_sitter_typescript());
            parser = new TSParser();
            parser.set_language(lang);
        }

        /// <summary>
        /// 解析 TypeScript 源代码字符串
        /// </summary>
        /// <param name="sourceCode">要解析的 TypeScript 源代码</param>
        /// <returns>解析后的语法树</returns>
        public TSTree ParseString(string sourceCode)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(Parser));
            return parser.parse_string(null, sourceCode);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    parser?.Dispose();
                }
                parser = null;
                disposed = true;
            }
        }

        ~Parser()
        {
            Dispose(false);
        }
    }
}