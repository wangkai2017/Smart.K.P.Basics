//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Chloe.Core
//{
//    public class SqlState : ISqlState
//    {
//        List<object> _sqlStorage;
//        int _recursiveDepth = 0;
//        public SqlState()
//            : this(0)
//        {
//        }

//        public int RecursiveDepth { get { return this._recursiveDepth; } }

//        public SqlState(int capacity)
//        {
//            this._sqlStorage = new List<object>(capacity);
//        }

//        public static SqlState Create(object obj)
//        {
//            SqlState state = new SqlState(1);
//            state.Append(obj);
//            return state;
//        }
//        public static SqlState Create(object obj1, object obj2)
//        {
//            SqlState state = new SqlState(2);
//            state.Append(obj1).Append(obj2);
//            return state;
//        }
//        public static SqlState Create(object obj1, object obj2, object obj3)
//        {
//            SqlState state = new SqlState(3);
//            state.Append(obj1).Append(obj2).Append(obj3);
//            return state;
//        }
//        public static SqlState Create(object obj1, object obj2, object obj3, object obj4)
//        {
//            SqlState state = new SqlState(4);
//            state.Append(obj1).Append(obj2).Append(obj3).Append(obj4);
//            return state;
//        }
//        public static SqlState Create(object obj1, object obj2, object obj3, object obj4, object obj5)
//        {
//            SqlState state = new SqlState(5);
//            state.Append(obj1).Append(obj2).Append(obj3).Append(obj4).Append(obj5);
//            return state;
//        }
//        public static SqlState Create(params object[] objs)
//        {
//            if (objs == null)
//                return new SqlState();

//            SqlState state = new SqlState(objs.Length);
//            return state.Append(objs);
//        }


//        public SqlState Append(object obj)
//        {
//            this._sqlStorage.Add(obj);
//            return this;
//        }
//        public SqlState Append(object obj1, object obj2)
//        {
//            return this.Append(obj1).Append(obj2);
//        }
//        public SqlState Append(object obj1, object obj2, object obj3)
//        {
//            return this.Append(obj1).Append(obj2).Append(obj3);
//        }
//        public SqlState Append(object obj1, object obj2, object obj3, object obj4)
//        {
//            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4);
//        }
//        public SqlState Append(object obj1, object obj2, object obj3, object obj4, object obj5)
//        {
//            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4).Append(obj5);
//        }
//        public SqlState Append(params object[] objs)
//        {
//            if (objs == null)
//                return this;

//            foreach (object obj in objs)
//            {
//                this.Append(obj);
//            }
//            return this;
//        }


//        public override string ToString()
//        {
//            return this.ToSql();
//        }
//        public string ToSql()
//        {
//            string s = null;
//            StringBuilder sb = new StringBuilder();
//            this._recursiveDepth = this.ToSql(sb);
//            s = sb.ToString();
//            return s;
//        }
//        public int ToSql(StringBuilder sb)
//        {
//            this._recursiveDepth = 0;

//            for (int i = 0; i < this._sqlStorage.Count; i++)
//            {
//                var obj = this._sqlStorage[i];
//                SqlState state = obj as SqlState;
//                if (state != null)
//                {
//                    int recursiveDepth = 1;
//                    recursiveDepth += state.ToSql(sb);
//                    if (this._recursiveDepth < recursiveDepth)
//                        this._recursiveDepth = recursiveDepth;
//                    continue;
//                }

//                sb.Append(obj);
//            }

//            return this._recursiveDepth;
//        }

//        StringBuilder InnerToSql()
//        {
//            StringBuilder sb = new StringBuilder();

//            Stack<object> stack = new Stack<object>();
//            stack.Push(this);

//            do
//            {
//                object obj = stack.Pop();
//                SqlState state = obj as SqlState;
//                if (state != null)
//                {
//                    for (int i = state._sqlStorage.Count - 1; i >= 0; i--)
//                    {
//                        stack.Push(state._sqlStorage[i]);
//                    }
//                }
//                else
//                {
//                    sb.Append(obj);
//                }
//            }
//            while (stack.Count > 0);

//            return sb;
//        }

//    }
//}
