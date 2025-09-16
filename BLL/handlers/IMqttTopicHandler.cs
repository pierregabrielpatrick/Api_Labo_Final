using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.handlers
{
    public interface IMqttTopicHandler
    {
        string TopicFilter { get; }
        Task HandleAsync(string payload);
    }
}
