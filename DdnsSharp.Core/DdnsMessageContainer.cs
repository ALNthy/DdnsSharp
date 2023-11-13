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
        private readonly object _lock = new object();
        public DdnsMessageContainer() 
        {
            _messages = new List<string>();
        }
        internal void AddMessage(string message) 
        {
            const int message_len = 20;
            lock (_lock) 
            {
                if (_messages.Count < message_len)
                {
                    _messages.Add(message);
                }
                else
                {
                    var list = _messages.GetRange(_messages.Count - message_len, _messages.Count);
                    for (int i = 0; i < _messages.Count - 1; i++)
                    {
                        if (i < message_len)
                        {
                            _messages[i] = list[i]; ;
                        }
                        else if (i == message_len)
                        {
                            _messages[i] = message;
                        }
                        else
                        {
                            _messages.Remove(_messages[i]);
                        }
                    }
                }
            } 
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
