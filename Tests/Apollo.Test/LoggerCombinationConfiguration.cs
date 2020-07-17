using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo.Test
{
    public class LoggerCombinationConfiguration
    {
        /// <summary>
        /// 1.httpapi,local
        /// 2.kafka,local
        /// 3.kafka,httpapi
        /// </summary>
        public int LogMode { get; set; }

        /// <summary>
        /// 是否同步写入日志
        /// </summary>
        public bool SyncLog { get; set; }

        /// <summary>
        /// 所属的业务线信息
        /// </summary>
        public HierarchicalMessageBase MessageBase { get; set; }

        /// <summary>
        /// 获取调用链路信息
        /// </summary>
        public Func<TraceMessageModel> TraceMessageFunc { get; set; }

        /// <summary>
        /// 通过http 写远程日志的配置信息
        /// </summary>
        public HttpApiConfiguration HttpApi { get; set; }

        /// <summary>
        /// 本地日志配置信息
        /// </summary>
        public LocalLoggerConfiguration Local { get; set; }

        /// <summary>
        /// 容错配置信息
        /// </summary>
        public FaultTolerantConfig FaultTolerant { get; set; }

        /// <summary>
        /// Kafka配置信息
        /// </summary>
        public KafkaLogConfig Kafka { get; set; }
    }

    public class HierarchicalMessageBase : MessageBase
    {
        /// <summary>
        /// 记录日志的最低等级
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 日志框架内部日志配置
        /// </summary>
        public MessageBase InnerMessage { get; set; }
    }
    public class MessageBase
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public int Subject { get; set; }

        /// <summary>
        /// 子项目ID
        /// </summary>
        public int SubjectLine { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string SubjectName { get; set; }
    }
    public enum LogLevel
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        Debug = 0,
        /// <summary>
        /// 运行信息
        /// </summary>
        Info = 1,
        /// <summary>
        /// 警告信息
        /// </summary>
        Warning = 2,
        /// <summary>
        /// 错误信息
        /// </summary>
        Error = 3,
        /// <summary>
        /// 严重错误信息
        /// </summary>
        FatalError = 4,
    }
    public class TraceMessageModel
    {
        public string TraceID { get; set; }

        public string SpanID { get; set; }
    }
    public class HttpApiConfiguration
    {
        /// <summary>
        /// http api地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }
    public class LocalLoggerConfiguration
    {
        /// <summary>
        /// 本地日志存放路径
        /// </summary>
        public string LogPath { get; set; }

        /// <summary>
        /// 定时写入时间间隔
        /// </summary>
        public TimeSpan IntervalTime { get; set; }

        /// <summary>
        /// 一次性写入的数量
        /// </summary>
        public int BatchNum { get; set; }
    }
    public class FaultTolerantConfig
    {
        /// <summary>
        /// 开启熔断功能
        /// </summary>
        public bool EnableCircuitBreaker { get; set; }

        /// <summary>
        /// 熔断配置
        /// </summary>
        public CircuitBreakerConfig CircuitBreakerConfig { get; set; }

        /// <summary>
        /// 日志无法使用之后的降级处理
        /// </summary>
        public Action<Exception, LogMessage> OnFallback { get; set; }

        /// <summary>
        /// 日志无法使用之后的降级处理
        /// </summary>
        public Action<LogMessage> Fallback { get; set; }

        /// <summary>
        /// 告警配置信息
        /// </summary>
        public AlarmConfig AlarmConfig { get; set; }
    }
    public class KafkaLogConfig
    {
        public KafkaLogConfig()
        {
            KafkaParam = new Dictionary<string, string>();
        }

        /// <summary>
        /// 日志写入的主题名
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 生产日志到kafka的超时时间
        /// </summary>
        public TimeSpan ProduceTimeout { get; set; }

        /// <summary>
        /// kafka地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 连接池最大容量
        /// </summary>
        public int ConnectionPoolLimit { get; set; }

        /// <summary>
        /// 连接闲置时间
        /// </summary>
        public TimeSpan ConnectionIdleTime { get; set; }

        public IKafkaLog KafkaLog { get; set; }

        public bool UseKafkaLog { get; set; }

        /// <summary>
        /// 当所有连接都被使用时，等待释放的时间
        /// </summary>
        public TimeSpan WaitConnectionFreeTime { get; set; }

        /// <summary>
        /// kafka配置参数
        /// </summary>
        public IDictionary<string, string> KafkaParam { get; set; }
    }
    public class CircuitBreakerConfig
    {
        /// <summary>
        /// 错误率（当错误率大于设置值时会开启熔断）
        /// </summary>
        public double FailureThreshold { get; set; }

        /// <summary>
        /// 取样时间（计算在该时间段内的错误率）
        /// </summary>
        public TimeSpan SamplingDuration { get; set; }

        /// <summary>
        /// 取样的最小错误次数，必须大于1，具体值取决于不同场景下的调用频率
        /// </summary>
        public int MinimumThroughput { get; set; }

        /// <summary>
        /// 熔断器开启持续时间
        /// </summary>
        public TimeSpan DurationOfBreak { get; set; }

        /// <summary>
        /// 熔断器开启回调
        /// </summary>
        public Action<Exception, CircuitState, TimeSpan, Context> OnBreak { get; set; }

        /// <summary>
        /// 熔断器重置回调
        /// </summary>
        public Action<Context> OnReset { get; set; }

        /// <summary>
        /// 熔断器半开回调
        /// </summary>
        public Action OnHalfOpen { get; set; }
    }
    public abstract class LogMessage : TraceMessage
    {
        protected LogMessage()
        {
            AddTime = DateTime.Now;
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public abstract int Type { get; }

        /// <summary>
        /// 产生日志的客户端地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public int Subject { get; set; }

        /// <summary>
        /// 项目 - 线（项目下的子项目）
        /// </summary>
        public int SubjectLine { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public string ThreadID { get; set; }

        /// <summary>
        /// 日志产生时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 是否是日志组件内部的日志
        /// </summary>
        public bool IsInnerMessage { get; set; }
    }
    public abstract class TraceMessage
    {
        public string TraceID { get; set; }

        public string SpanID { get; set; }
    }
    /// <summary>
    /// 告警配置信息
    /// </summary>
    public class AlarmConfig
    {
        /// <summary>
        /// 告警方式（1.邮件告警）
        /// </summary>
        public int AlarmType { get; set; }

        /// <summary>
        /// 告警间隔
        /// </summary>
        public TimeSpan AlarmInterval { get; set; }

        /// <summary>
        /// 邮箱告警配置
        /// </summary>
        public EmailAlarmConfig EmailAlarm { get; set; }

        /// <summary>
        /// 告警通知配置信息
        /// </summary>
        public IList<AlarmNotifier> AlarmNotifiers { get; set; }
    }
    /// <summary>
    /// 邮件告警配置
    /// </summary>
    public class EmailAlarmConfig
    {
        /// <summary>
        /// 邮箱HOST
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 邮箱服务端用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱服务端密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 告警邮件发送来源
        /// </summary>
        public string From { get; set; }
    }
    public class AlarmNotifier
    {
        public string Email { get; set; }

        public string Phone { get; set; }
    }
    public interface IKafkaLog
    {
        void DebugFormat(string format, params object[] args);
        void ErrorFormat(string format, params object[] args);
        void FatalFormat(string format, params object[] args);
        void InfoFormat(string format, params object[] args);
        void WarnFormat(string format, params object[] args);
    }
    public enum CircuitState
    {
        //
        // 摘要:
        //     Closed - When the circuit is closed. Execution of actions is allowed.
        Closed = 0,
        //
        // 摘要:
        //     Open - When the automated controller has opened the circuit (typically due to
        //     some failure threshold being exceeded by recent actions). Execution of actions
        //     is blocked.
        Open = 1,
        //
        // 摘要:
        //     Half-open - When the circuit is half-open, it is recovering from an open state.
        //     The duration of break of the preceding open state has typically passed. In the
        //     half-open state, actions may be executed, but the results of these actions may
        //     be treated with criteria different to normal operation, to decide if the circuit
        //     has recovered sufficiently to be placed back in to the closed state, or if continuing
        //     failures mean the circuit should revert to open perhaps more quickly than in
        //     normal operation.
        HalfOpen = 2,
        //
        // 摘要:
        //     Isolated - When the circuit has been placed into a fixed open state by a call
        //     to Polly.CircuitBreaker.CircuitBreakerPolicy.Isolate. This isolates the circuit
        //     manually, blocking execution of all actions until a call to Polly.CircuitBreaker.CircuitBreakerPolicy.Reset
        //     is made.
        Isolated = 3
    }
}
