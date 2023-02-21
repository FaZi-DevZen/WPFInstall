using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace MusicTeachingInstall.Helpers
{
    public class JsonHelper
    {
        public static string Serializer<T>(T t)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            return javaScriptSerializer.Serialize(t);
        }

        public static T Deserializer<T>(string jsonText)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            T t = javaScriptSerializer.Deserialize<T>(jsonText);
            return t;
        }
    }
}
