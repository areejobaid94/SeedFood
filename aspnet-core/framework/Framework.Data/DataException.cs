using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Framework.Data
{
    public class DataException : Exception
    {
        /// <summary>
        /// Gets or sets the Priority of exception.
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Initializes a new instance of the BaseException class.
        /// </summary>
        public DataException()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the BaseException class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public DataException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the BaseException class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="exception">Inneer exception value.</param>
        public DataException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the BaseException class.
        /// </summary>
        /// <param name="info">Message of the exception.</param>
        /// <param name="context">InnerException value.</param>
        protected DataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //// thats because we have extra fields we need to deserialze it .
            if (info != null)
            {
                this.Priority = info.GetString("Priority");
            }
        }

        /// <summary>
        /// This override function responsible for performs a custom serialization.
        /// </summary>
        /// <param name="info">SerializationInfo object.</param>
        /// <param name="context">StreamingContext object.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // this is because we have extra feild we need to serialze it .
            if (info != null)
            {
                info.AddValue("Priority", this.Priority);
            }

            base.GetObjectData(info, context);
        }
    }
}