using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Core
{
    public class DdnsMessageContainer
    {
        private readonly List<string> _messages; 
        public DdnsMessageContainer() 
        {
            _messages = new List<string>();
        }
        internal void AddMessage(string message) 
        {
            _messages.Add(message);
        }
        public IEnumerable<string> GetMessages() 
        { 
            foreach (var message in _messages) 
            {
                yield return message;
            }
        }
    }
}
