using System;
using GitHub.TreeSitter;
using System.Runtime.InteropServices;

namespace TreeSitter.TypeScript
{
    /// <summary>
    /// TypeScript 解析器模块 - 负责 Tree-sitter TypeScript 语言的初始化和基础解析功能
    /// </summary>
    public class TypeScriptParser : IDisposable
    {
        private readonly TSLanguage lang;
        private TSParser parser;
        private bool disposed = false;

        [DllImport("tree-sitter-typescript.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr tree_sitter_typescript();

        public TypeScriptParser()
        {
            lang = new TSLanguage(tree_sitter_typescript());
            parser = new TSParser();
            parser.set_language(lang);
        }

        /// <summary>
        /// 获取 TypeScript 语言对象
        /// </summary>
        public TSLanguage Language => lang;

        /// <summary>
        /// 解析 TypeScript 源代码字符串
        /// </summary>
        /// <param name="sourceCode">要解析的 TypeScript 源代码</param>
        /// <returns>解析后的语法树</returns>
        public TSTree ParseString(string sourceCode)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(TypeScriptParser));

            return parser.parse_string(null, sourceCode);
        }

        /// <summary>
        /// 创建语法树游标
        /// </summary>
        /// <param name="tree">语法树</param>
        /// <returns>TSCursor 对象</returns>
        public TSCursor CreateCursor(TSTree tree)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(TypeScriptParser));

            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            return new TSCursor(tree.root_node(), lang);
        }

        /// <summary>
        /// 检查解析器是否可用
        /// </summary>
        public bool IsAvailable => !disposed && parser != null && lang != null;

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

        ~TypeScriptParser()
        {
            Dispose(false);
        }
    }
}