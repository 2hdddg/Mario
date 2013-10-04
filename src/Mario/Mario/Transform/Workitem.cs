using System;
using System.Collections.Generic;

namespace Mario.Transform
{
    internal class Workitem
    {
        private readonly Dictionary<string, object> _work;

        public Workitem()
        {
            _work = new Dictionary<string, object>();
        }

        public Workitem(object work)
        {
            _work = new Dictionary<string, object>();
            Submit(work);
        }

        private string KeyFrom(Type t)
        {
            return t.ToString();
        }

        public T Require<T>()
        {
            return (T)Require(typeof(T));
        }

        private object Require(Type t)
        {
            var key = KeyFrom(t);
            if (_work.ContainsKey(key))
            {
                return _work[key];
            }
            throw new Exception("Requires " + key + " that hasnt been submitted");
        }

        public void Submit(object work)
        {
            var key = KeyFrom(work.GetType());
            _work[key] = work;
        }
    }
}