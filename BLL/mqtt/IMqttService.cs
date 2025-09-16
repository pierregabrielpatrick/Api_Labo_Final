using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.mqtt
{
    public interface IMqttService
    {
        bool IsConnected { get; }
        Task<bool> PublishAsync(string topic, string payload);
    }
}
