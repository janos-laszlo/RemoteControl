﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ReceiverWinFormsApp.Utils
{
    static class JSONUtils
    {
        public static T FromJson<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(
                typeof(T),
                GetDataContractJsonSerializerSettings());

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var message = (T)serializer.ReadObject(ms);
            if (message is null)
            {
                throw new ArgumentException("The provided json couldn't be parsed");
            }

            return message;

        }

        public static string ToJson<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(
                typeof(T),
                GetDataContractJsonSerializerSettings());

            using var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            return Encoding.Default.GetString(ms.ToArray());
        }

        private static DataContractJsonSerializerSettings GetDataContractJsonSerializerSettings()
        {
            return new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}